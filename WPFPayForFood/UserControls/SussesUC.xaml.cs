using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Resources;
using WPFPayForFood.Services.Object;

namespace WPFPayForFood.UserControls
{
    /// <summary>
    /// Lógica de interacción para SussesUserControl.xaml
    /// </summary>
    public partial class SussesUC : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        #endregion

        #region "Constructor"
        public SussesUC(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            FinishTrnsaction();
        }
        #endregion

        #region "Métodos"
        private void FinishTrnsaction()
        {
            try
            {
                Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(transaction.Observation))
                    {
                        AdminPayPlus.SaveErrorControl(transaction.Observation, "", EError.Device, ELevelError.Medium);
                    }

                    //AdminPayPlus.UpdateTransaction(this.transaction);

                    transaction.StatePay = "Aprobado";

                    Utilities.PrintVoucher(this.transaction);

                    Thread.Sleep(5000);

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        if (transaction.State == ETransactionState.Error)
                        {
                            Utilities.RestartApp();
                        }
                        else
                        {
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                    });
                    GC.Collect();
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        private void Btn_Calificacion(object sender, System.Windows.Input.TouchEventArgs e)
        {
            try
            {
                int Tag = Convert.ToInt32((sender as Image).Tag);

                switch (Tag)
                {
                    case 1:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Hidden;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                         aceptar.Visibility = Visibility.Visible;
                        //  Transaction.calificacion = "1";

                        break;

                    case 2:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        //      Transaction.calificacion = "2";
                        break;
                    case 3:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                       aceptar.Visibility = Visibility.Visible;
                        //       Transaction.calificacion = "3";
                        break;
                    case 4:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        //          Transaction.calificacion = "4";

                        break;

                    case 5:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Visible;
                        aceptar.Visibility = Visibility.Visible;
                        //      Transaction.calificacion = "5";
                        break;
                }


            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private void btnaceptar_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);

        }
    }
}