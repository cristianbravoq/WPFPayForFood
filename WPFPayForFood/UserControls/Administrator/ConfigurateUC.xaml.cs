using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFPayForFood.Classes;
using WPFPayForFood.Resources;
using WPFPayForFood.Services;
using WPFPayForFood.Services.Object;

namespace WPFPayForFood.UserControls.Administrator
{
    /// <summary>
    /// Lógica de interacción para ConfigurateUserControl.xaml
    /// </summary>
    public partial class ConfigurateUC : UserControl
    {
        #region "Referencias"
        private AdminPayPlus init;
        private ApiIntegration integrationApi;
        #endregion

        #region "Constructor"
        public ConfigurateUC()
        {
            try
            {
                InitializeComponent();

                if (init == null)
                {
                    init = new AdminPayPlus();
                }

                if (integrationApi == null)
                {
                    integrationApi = new ApiIntegration();
                }

                txtMs.DataContext = init;

                Initial();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Métodos"
        private async void Initial()
        {
            try
            {
                init.callbackResult = result =>
                {
                    ProccesResult(result);
                };

                init.Start();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async void ProccesResult(bool result)
        {
            try
            {
                if (AdminPayPlus.DataPayPlus.StateUpdate)
                {
                    //Utilities.ShowModal(MessageResource.UpdateAplication, EModalType.Error, true);
                    //Utilities.UpdateApp();
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = MessageResource.UpdateAplication,
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);
                }
                else if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = MessageResource.ModoAdministrativo,
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Balancing);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = MessageResource.ModoAdministrativo,
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Upload);
                }
                else
                {
                    Finish(result);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Finish(bool state)
        {
            try
            {
                Task.Run(() =>
                {
                    if (state)
                    {
                        AdminPayPlus.SaveLog(new RequestLog
                        {
                            Reference = "",
                            Description = MessageResource.YesGoInitial,
                            State = 1,
                            Date = DateTime.Now
                        }, ELogType.General);

                        Utilities.navigator.Navigate(UserControlView.Main);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(AdminPayPlus.DataPayPlus.Message))
                        {
                            AdminPayPlus.SaveLog(new RequestLog
                            {
                                Reference = "",
                                Description = MessageResource.NoGoInitial + " " + AdminPayPlus.DataPayPlus.Message,
                                State = 6,
                                Date = DateTime.Now
                            }, ELogType.General);

                            Utilities.ShowModal(MessageResource.NoService + " " + MessageResource.NoMoneyKiosco, EModalType.Error, this, true);
                        }
                        else
                        {
                            AdminPayPlus.SaveLog(new RequestLog
                            {
                                Reference = "",
                                Description = MessageResource.NoGoInitial + " " + init.DescriptionStatusPayPlus,
                                State = 2,
                                Date = DateTime.Now
                            }, ELogType.General);

                            Utilities.ShowModal(MessageResource.NoService + " " + init.DescriptionStatusPayPlus, EModalType.Error, this, true);
                        }

                        Initial();
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
