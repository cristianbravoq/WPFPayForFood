using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFPayForFood.Classes;
using WPFPayForFood.Services.ObjectIntegration;

namespace WPFPayForFood.Windows.Alerts
{
    /// <summary>
    /// Interaction logic for PhoneW.xaml
    /// </summary>
    public partial class NameW : Window
    {
        #region "Referencias"
        public string nombre;
        public int IDPayer;
        #endregion

        #region "Constructor"
        public NameW()
        {
            InitializeComponent();
            nombre = string.Empty;
        }
        #endregion

        #region "Eventos"
        private void TxtName_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, sender as TextBox, this, 250, 1030);
        }

        private void btnaceptar_TouchDown(object sender, TouchEventArgs e)
        {
            if (Validate())
            {

                RequestCreatePayerPoints Request = new RequestCreatePayerPoints
                {
                    cel = txtNum.Text,
                    email = txtCorreo.Text,
                    nombre = txtName.Text,
                
                };

                var Response = AdminPayPlus.apiIntegration.CreatePayer(Request);

                IDPayer = Response.Result.data;
                DialogResult = true;
            }
        }
        #endregion

        #region "Métodos"
        private bool Validate()
        {
            try
            {
                if (txtName.Text != string.Empty)
                {
                    nombre = txtName.Text;
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

        private void TxtNum_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, sender as TextBox, this, 250, 1030);
        }

        private void TxtCorreo_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, sender as TextBox, this, 250, 1030);
        }
    }
}
