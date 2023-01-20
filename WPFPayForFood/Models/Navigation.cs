using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Services.Object;
using WPFPayForFood.UserControls;
using WPFPayForFood.UserControls.Administrator;

namespace WPFPayForFood.Models
{
    public class Navigation : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private UserControl _view;

        public UserControl View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(View)));
            }
        }

        public void Navigate(UserControlView newWindow, object data = null, object complement = null) => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                switch (newWindow)
                {
                    case UserControlView.Config:
                        View = new ConfigurateUC();
                        break;
                    case UserControlView.Main:
                        View = new MainUC();
                        break;
                    case UserControlView.Menu:
                        View = new MenuUC();
                        break;     
                    case UserControlView.Products:
                        View = new ProductsUC((Transaction)data);
                        break;
                    case UserControlView.PaySuccess:
                        View = new SussesUC((Transaction)data);
                        break;
                    case UserControlView.Pay:
                        View = new PaymentUC((Transaction)data);
                        break;
                    case UserControlView.ReturnMony:
                        View = new ReturnMonyUC((Transaction)data);
                        break;
                    case UserControlView.UserPoints:
                        View = new UserPointUC((Transaction)data);
                        break;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Navigate", ex, ex.ToString());
            }
            GC.Collect();
        });
    }
}