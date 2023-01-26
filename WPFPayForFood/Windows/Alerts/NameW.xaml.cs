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
        public ResponseCreatePayer createPayer;
        
        #endregion

        #region "Constructor"
        public NameW(int idPayer)
        {
            InitializeComponent();
            nombre = string.Empty;
            IDPayer = idPayer;
            //var _idPayer = idPayer;
        }
        #endregion

        #region "Eventos"
        private void TxtName_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, sender as TextBox, this, 250, 1030);
        }

        private void btnaceptar_TouchDown(object sender, TouchEventArgs e)
        {
            if (!Validate())
            {

                RequestSetPayerData Request = new RequestSetPayerData
                {
                    cel = txtNum.Text,
                    email = txtCorreo.Text,
                    idPayer = IDPayer
                    //nombre = txtName.Text,
                    //};
                };

                var Response = AdminPayPlus.apiIntegration.SetPayer(Request);

                DialogResult = true;
            }
        }
        #endregion

        #region "Métodos"
        private bool Validate()
        {
            try
            {
                if (IDPayer == 0)
                {
                    RequestCreatePayerPoints Request = new RequestCreatePayerPoints
                    {
                        nombre = "-",
                        email = txtCorreo.Text,
                        cel = txtNum.Text,
                        documentO_ID = "-",
                        fechA_NACIMIENTO = "-",
                        points = "-"
                    };

                    createPayer = AdminPayPlus.apiIntegration.CreatePayer(Request).GetAwaiter().GetResult();

                    if(createPayer.codeError == 200)
                    {
                        IDPayer = createPayer.data;
                        DialogResult = true;
                    }
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
