using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Services.ObjectIntegration;

namespace WPFPayForFood.UserControls
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class UserPointUC : UserControl
    {
        private const char Separator = '\t';

        #region "Referencias"
        private Transaction transaction;
        #endregion

        #region "Eventos"

        public UserPointUC(Transaction ts)
        {
            InitializeComponent();
            transaction = ts;
            ActivateScanner();
        }

        #endregion

        #region "Métodos"

        private void SaveUserPoints(List<String> res)
        {
            if (Validate(res[1]))
            {
                RequestCreatePayerPoints Request = new RequestCreatePayerPoints
                {
                    nombre = res[0],
                    email = res[1],
                    cel = res[2],
                    documentO_ID = res[3],
                    fechA_NACIMIENTO = res[4],
                    points = res[5]
                };

                transaction.PayerDocument = res[3];

                var Response = AdminPayPlus.apiIntegration.CreatePayer(Request);

                transaction.UserPoints = res[5];
                //Navegar al pago
                Utilities.navigator.Navigate(UserControlView.Pay, transaction);

                //IDPayer = Response.Result.data;
                //DialogResult = true;
            }
        }

        private bool Validate(string res)
        {
            try
            {
                if (res != string.Empty)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return false;
        }
        #endregion

        #region Lector de Cod Barras

        private void ActivateScanner()
        {
            try
            {
                AdminPayPlus.ControlScanner.callbackScanner = Reference =>
                {
                    if (!string.IsNullOrEmpty(Reference))
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {

                            var resLength = Reference.Length;

                            string[] subs = Reference.Split(@Separator);

                            var nombre = subs[3] + " " + subs[4] + " " + subs[1] + " " + subs[2];

                            List<String> req = new List<String>();
                            req.Add(nombre);
                            req.Add(subs[2]);
                            req.Add(subs[0]);
                            req.Add(subs[0]);
                            req.Add(subs[6]);
                            req.Add("1230");

                            //GetPayerPoints(res);
                            SaveUserPoints(req);

                            //Utilities.navigator.Navigate(UserControlView.DataFacture, transaction);
                        });
                    }
                };
                AdminPayPlus.ControlScanner.callbackErrorScanner = Error =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        //Utilities.ShowModal(Error, EModalType.Error);
                        ActivateScanner();
                    });
                };

                AdminPayPlus.ControlScanner.flagScanner = 0;
                AdminPayPlus.ControlScanner.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
