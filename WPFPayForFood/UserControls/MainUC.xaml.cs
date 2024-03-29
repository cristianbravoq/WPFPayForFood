﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPFPayForFood.Classes;
using WPFPayForFood.Resources;
using WPFPayForFood.Services.Object;

namespace WPFPayForFood.UserControls
{
    /// <summary>
    /// Lógica de interacción para MainUserControl.xaml
    /// </summary>
    public partial class MainUC : UserControl
    {
        #region "Referencias"
        private ImageSleader _imageSleader;
        private bool _validatePaypad;
        #endregion

        #region "Constructor"
        public MainUC(bool validatePaypad = true)
        {
            InitializeComponent();

            _validatePaypad = validatePaypad;

            Init();
        }
        #endregion

        #region "Métodos"
        private void Init()
        {
            try
            {
                ConfiguratePublish();
              //  AdminPayPlus.NotificateInformation();
                //AdminPayPlus.VerifyTransaction();
                InitValidation();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void InitValidation()
        {
            try
            {
                Task.Run(() =>
                {
                    while (_validatePaypad)
                    {
                        AdminPayPlus.ValidatePaypad();

                        Thread.Sleep(int.Parse(Utilities.GetConfiguration("DurationAlert")));
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ConfiguratePublish()
        {
            try
            {
                if (_imageSleader == null)
                {
                    string folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Publish");

                    _imageSleader = new ImageSleader((List<String>)AdminPayPlus.DataPayPlus.ListImages, folder);

                    this.DataContext = _imageSleader.imageModel;

                    _imageSleader.time = int.Parse(Utilities.GetConfiguration("TimerPublicity"));

                    _imageSleader.isRotate = true;

                    _imageSleader.Start();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ValidateStatus()
        {
            try
            {
                if (AdminPayPlus.DataPayPlus.StateUpdate)
                {
                    Utilities.ShowModal(MessageResource.UpdateAplication, EModalType.Error, this, true);
                    Utilities.UpdateApp();
                }
                else if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.ModoAdministrativo);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Balancing);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.ModoAdministrativo);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Upload);
                }
                else if (AdminPayPlus.DataPayPlus.State && AdminPayPlus.DataPayPlus.StateAceptance && AdminPayPlus.DataPayPlus.StateDispenser)
                {
                    //int response = AdminPayPlus.PrintService.StatusPrint();

                    //if (response != 0)
                    //{
                    //    if (response == 7 || response == 8)
                    //    {
                    //        AdminPayPlus.SaveErrorControl(AdminPayPlus.PrintService.MessageStatus(response), MessageResource.InformationError, EError.Nopapper, ELevelError.Medium);
                    //    }
                    //    else
                    //    {
                    //        AdminPayPlus.SaveErrorControl(AdminPayPlus.PrintService.MessageStatus(response), MessageResource.InformationError, EError.Printer, ELevelError.Medium);
                    //    }

                    //    if (response != 8)
                    //    {
                    //        AdminPayPlus.SaveLog(new RequestLog
                    //        {
                    //            Reference = "",
                    //            Description = MessageResource.PrinterNoPapper,
                    //            State = 2,
                    //            Date = DateTime.Now
                    //        }, ELogType.General);

                    //        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.PrinterNoPapper);

                    //        if (Utilities.ShowModal(MessageResource.ErrorNoPaper, EModalType.Information))
                    //        {
                    //            Redirect();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Redirect();
                    //    }
                    //}
                    //else
                    //{
                    Redirect();
                    //}
                }
                else
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = MessageResource.NoMoneyKiosco + " " + AdminPayPlus.DataPayPlus.Message,
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);

                    Utilities.ShowModal(MessageResource.NoService + " " + MessageResource.NoMoneyKiosco, EModalType.Error, this, true);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

   

        private void Redirect()
        {
            try
            {
                _validatePaypad = false;
                _imageSleader.Stop();

                AdminPayPlus.SaveLog(new RequestLog
                {
                    Reference = "",
                    Description = MessageResource.YesGoInitial,
                    State = 1,
                    Date = DateTime.Now
                }, ELogType.General);

                Utilities.navigator.Navigate(UserControlView.Menu);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Eventos"
        private void Next_TouchDown(object sender, TouchEventArgs e)
        {
            ValidateStatus();
        }
        #endregion

        private void siguientePantalla(object sender, MouseButtonEventArgs e)
        {
            ValidateStatus();
        }
    }
}