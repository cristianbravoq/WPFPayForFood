using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Resources;
using WPFPayForFood.Services.Object;
using WPFPayForFood.Services.ObjectIntegration;
using WPFPayForFood.ViewModel;
using WPFPayForFood.Windows.Alerts;

namespace WPFPayForFood.UserControls
{
    /// <summary>
    /// Lógica de interacción para PaymentUserControl.xaml
    /// </summary>
    public partial class PaymentUC : UserControl
    {
        private const char Separator = '\"';
        #region "Referencias"
        private Transaction transaction;
        private PaymentViewModel paymentViewModel;
        private int Intentos = 1;
        #endregion

        #region "Constructor"
        public PaymentUC(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            GetPayerPoints(transaction.PayerDocument);

            this.transaction.statePaySuccess = false;

            OrganizeValues();
        }
        #endregion

        #region "Eventos"
        private void BtnCancell_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            CancellPay();
        }
        #endregion

        #region Métodos
        private void OrganizeValues()
        {

            SendData();
            try
            {
                this.paymentViewModel = new PaymentViewModel
                {
                    UserPoints = transaction.UserPoints.ToString(),
                    PayValue = transaction.Amount,
                    ValorFaltante = transaction.Amount,
                    ImgContinue = Visibility.Hidden,
                    ImgCancel = Visibility.Visible,
                    ImgCambio = Visibility.Hidden,
                    ValorSobrante = 0,
                    ValorIngresado = 0,
                    viewList = new CollectionViewSource(),
                    Denominations = new List<DenominationMoney>(),
                    ValorDispensado = 0
                };

                this.DataContext = this.paymentViewModel;

                ActivateWallet();
             //   SavePay();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void GetPayerPoints(string res)
        {
            
            //var Response = AdminPayPlus.apiIntegration.GetPayerDocument(_payer);

            //if (Response != null)
            //{
            //    var pagador = Response.Result.ToString();

            //    string[] subs = pagador.Split(@Separator);

            //    var auxiliar = pagador.Substring(0, 3);

            //    transaction.UserPoints = Response.ToString();
            //}

            //transaction.UserPoints =  Response.ToString();
        }

        private void ActivateWallet()
        {
            try
            {
                Task.Run(() =>
                {
                    AdminPayPlus.ControlPeripherals.callbackValueIn = enterValue =>
                    {
                        if (enterValue.Item1 > 0)
                        {
                            if (!this.paymentViewModel.StatePay)
                            {
                                paymentViewModel.ValorIngresado += enterValue.Item1;

                                paymentViewModel.RefreshListDenomination(int.Parse(enterValue.Item1.ToString()), 1, enterValue.Item2);

                                AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, enterValue.Item1, 2, 1, enterValue.Item2, string.Empty);
                                LoadView();
                            }
                        }
                    };

                    AdminPayPlus.ControlPeripherals.callbackTotalIn = enterTotal =>
                    {
                        if (!this.paymentViewModel.StatePay)
                        {
                            this.paymentViewModel.ImgCancel = Visibility.Hidden;

                            AdminPayPlus.ControlPeripherals.StopAceptance();

                            if (enterTotal > 0 && paymentViewModel.ValorSobrante > 0)
                            {
                                this.paymentViewModel.ImgCambio = Visibility.Visible;

                                ReturnMoney(paymentViewModel.ValorSobrante, true);
                            }
                            else
                            {
                                SavePay();
                            }
                        }
                    };

                    AdminPayPlus.ControlPeripherals.callbackError = error =>
                    {
                        AdminPayPlus.SaveLog(new RequestLogDevice
                        {
                            Code = error.Item1,
                            Date = DateTime.Now,
                            Description = error.Item2,
                            Level = ELevelError.Medium,
                            TransactionId = transaction.IdTransactionAPi
                        }, ELogType.General);
                    };

                    AdminPayPlus.ControlPeripherals.StartAceptance(paymentViewModel.PayValue);
            //        AdminPayPlus.ControlPeripherals.StartAceptance(18000);
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ReturnMoney(decimal returnValue, bool state)
        {
            try
            {
                AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                {
                    if (state)
                    {
                        transaction.StateReturnMoney = true;
                        transaction.AmountToReturn = totalOut;
                        paymentViewModel.ValorDispensado = totalOut;
                        SavePay();
                    }
                };

                AdminPayPlus.ControlPeripherals.callbackLog = log =>
                {
                    paymentViewModel.SplitDenomination(log);
                    AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);
                };

                AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                {
                    AdminPayPlus.ControlPeripherals.callbackOut = null;
                    if (state)
                    {
                        paymentViewModel.ValorDispensado = valueOut;
                        transaction.StateReturnMoney = false;

                        if (paymentViewModel.ValorDispensado == paymentViewModel.ValorSobrante)
                        {
                            transaction.StateReturnMoney = true;
                            SavePay();
                        }
                        else
                        {
                            transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + valueOut.ToString();
                            //Utilities.ShowModal(MessageResource.IncompleteMony, EModalType.Error);
                            SavePay(ETransactionState.Error);
                        }
                    }
                };

                AdminPayPlus.ControlPeripherals.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void LoadView()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    paymentViewModel.viewList.Source = paymentViewModel.Denominations;
                    lv_denominations.DataContext = paymentViewModel.viewList;
                    lv_denominations.Items.Refresh();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void SavePay(ETransactionState statePay = ETransactionState.Initial)
        {
            try
            {
                if (!this.paymentViewModel.StatePay)
                {
                    this.paymentViewModel.StatePay = true;
                    transaction.Payment = paymentViewModel;
                    transaction.State = statePay;

                    //AdminPayPlus.ControlPeripherals.ClearValues();
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        this.Opacity = 0.3;

                        if(transaction.DataPayer == null)
                        {
                            DataPayerDocument Request = new DataPayerDocument
                            {
                                payer = "-",
                                iD_PAYER = 0,
                                email = "-",
                                cel = "-",
                                documentO_ID = "-",
                                fechA_NACIMIENTO = "-",
                                points = "-"
                            };
                            transaction.DataPayer = Request;
                        }

                        NameW modal = new 
                        NameW(transaction.auxId);
                        modal.ShowDialog();
                        this.Opacity = 1;

                        Task.Run(async () =>
                        {
                            Thread.Sleep(5000);

                            ProductsSelects products = new ProductsSelects();

                            products.productos = new List<Datum>();

                            string comentario = string.Empty;

                            foreach (var item in transaction.productos)
                            {
                                comentario += item.comentarios + Environment.NewLine;
                            }

                            products.nombrE_CLIENTE = modal.nombre;
                            products.comentario = comentario;
                            products.productos.AddRange(transaction.productos);
                            products.totaL_VENTA = transaction.Amount;
                            products.iD_RESTAURANTE = 1;
                            products.PAYERID = modal.IDPayer;
                            products.Return_Amount = Convert.ToInt64(paymentViewModel.ValorSobrante);
                            products.Income_Amount = Convert.ToInt64(paymentViewModel.ValorIngresado);
                            products.Real_Amount = 0;

                            var response = await AdminPayPlus.apiIntegration.NotifyMenu(products);

                            Utilities.CloseModal();

                            if (response != null)
                            {
                                transaction.reference = response.data;
                                transaction.statePaySuccess = true;
                                transaction.State = ETransactionState.Success;
                                UpdatePoints();

                                Utilities.navigator.Navigate(UserControlView.PaySuccess, transaction);
                            }
                            else
                            {
                                transaction.statePaySuccess = false;
                                transaction.State = ETransactionState.CancelError;
                                SendData();
                                CancelTransaction();
                            }
                        });

                        Utilities.ShowModal("Creando órden. Esperá un momento por favor.", EModalType.Preload);
                    });
                 }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                CancelTransaction();
            }
        }

        private async void UpdatePoints()
        {
            var auxPoints = transaction.Amount / 1000;

            transaction.UserPoints += Convert.ToInt32(auxPoints);

            RequestSetPayerPoints updatePoints = new RequestSetPayerPoints()
            {
                    Documento = transaction.Document,
                    Points = transaction.UserPoints.ToString()
            };

            var setPoints = await AdminPayPlus.apiIntegration.SetPayerPoints(updatePoints);
        }

        private void CancelTransaction()
        {
            try
            {
                string ms = "Estimado usuario, no se pudo notificar el pago. Se le hará la devolución del dinero ingresado.";
                Utilities.ShowModal(ms, EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.ReturnMony, transaction);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void CancellPay()
        {
            try
            {
                this.paymentViewModel.ImgContinue = Visibility.Hidden;

                this.paymentViewModel.ImgCancel = Visibility.Hidden;

                if (Utilities.ShowModal(MessageResource.CancelTransaction, EModalType.Information))
                {
                    AdminPayPlus.ControlPeripherals.StopAceptance();
                    AdminPayPlus.ControlPeripherals.callbackLog = null;
                    if (!this.paymentViewModel.StatePay)
                    {
                        if (paymentViewModel.ValorIngresado > 0)
                        {
                            transaction.Payment = paymentViewModel;
                            Utilities.navigator.Navigate(UserControlView.ReturnMony, transaction);
                        }
                        else
                        {
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                    }
                }
                else
                {
                    if (paymentViewModel.ValorIngresado > 0)
                    {
                        this.paymentViewModel.ImgContinue = Visibility.Visible;
                    }
                    this.paymentViewModel.ImgCancel = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async void SendData()
        {
            try
            {
                if (transaction.Amount > 0)
                {
                    Task.Run(async () =>
                    {
                        transaction.State = ETransactionState.Initial;
                        await AdminPayPlus.SaveTransaction(transaction);

                        Utilities.CloseModal();

                        if (this.transaction.IdTransactionAPi == 0)
                        {
                            Utilities.ShowModal("", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                        else
                        {
                            //Utilities.navigator.Navigate(UserControlView.Pay, false, transaction);
                        }
                    });
                    Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        #endregion
    }
}
