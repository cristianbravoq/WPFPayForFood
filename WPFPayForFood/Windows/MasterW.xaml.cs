using System;
using System.Reflection;
using System.Windows;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;

namespace WPFPayForFood.Windows
{
    /// <summary>
    /// Lógica de interacción para MasterWindow.xaml
    /// </summary>
    public partial class MasterW : Window
    {
        #region "Constructor"
        public MasterW()
        {
            InitializeComponent();

            SetUserControl();
        }
        #endregion

        #region "Métodos"
        private void SetUserControl()
        {
            try
            {
                if (Utilities.navigator == null)
                {
                    Utilities.navigator = new Navigation();
                }

                string a = Encryptor.Encrypt("usrapli");
                string b = Encryptor.Encrypt("1Cero12019$/*");
                string c = Encryptor.Encrypt("Ecity.Software");
                string d = Encryptor.Encrypt("Ecitysoftware2019#");
                string e = Encryptor.Encrypt("https://e-citypay.co/");

                WPKeyboard.Keyboard.ConsttrucKeyyboard(WPKeyboard.Keyboard.EStyle.style_2);

                Utilities.navigator.Navigate(UserControlView.Config);

                DataContext = Utilities.navigator;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
