using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFPayForFood.Classes.Printer;
using WPFPayForFood.Models;
using WPFPayForFood.Resources;
using WPFPayForFood.Services.ObjectIntegration;
using WPFPayForFood.Windows;
using Zen.Barcode;

namespace WPFPayForFood.Classes
{
    public class Utilities
    {
        #region "Referencias"
        public static Navigation navigator { get; set; }

        private static SpeechSynthesizer speechSynthesizer;

        private static ModalW modal { get; set; }

        public static List<Datum> Products = new List<Datum>();
        #endregion

        public static string GetConfiguration(string key, bool decodeString = false)
        {
            try
            {
                string value = "";
                AppSettingsReader reader = new AppSettingsReader();
                value = reader.GetValue(key, typeof(String)).ToString();
                if (decodeString)
                {
                    value = Encryptor.Decrypt(value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }

        public static bool ShowModal(string message, EModalType type, bool timer = false)
        {
            bool response = false;

            try
            {
                ModalModel model = new ModalModel
                {
                    //Tittle = "Estimado Cliente: ",
                    Messaje = message,
                    Timer = timer,
                    TypeModal = type,
                    ImageModal = @"Images/Backgrounds/modal.png",
                };

                if (type == EModalType.Error)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-error.png";
                }
                else if (type == EModalType.Information)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-info.png";
                }
                else if (type == EModalType.NoPaper)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-info.png";
                }
                else if (type == EModalType.Preload)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-info.png";
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
               //     uc.Opacity = 0.3;
                    modal = new ModalW(model);
                    modal.ShowDialog();

                    if (modal.DialogResult.HasValue && modal.DialogResult.Value)
                    {
                        response = true;
                    }
          //          uc.Opacity = 1;
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }

            GC.Collect();
            return response;
        }

        public static void CloseModal() => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                if (modal != null)
                {
                    modal.Close();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
            }
        });

        public static void RestartApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(Directory.GetCurrentDirectory(), GetConfiguration("NAME_APLICATION"));
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = GetConfiguration("APLICATION_UPDATE");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static void PrintVoucher(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 8, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 8, System.Drawing.FontStyle.Regular);
                    int y = 0;
                    int sum = 25;
                    int x = 150;
                    int xKey = 15;

                    var data = new List<DataPrinter>()
                    {
                        //new DataPrinter{ image = GetConfiguration("ImageBoucher"),  x = 80, y = 2 },

                        new DataPrinter{ brush = color, font = fontKey,   value = "Comprobante de pago", x = 80, y = y+=10 },

                        new DataPrinter { brush = color, font = fontValue, value = "Pay4Food", x = 110, y = y += sum+10 },

                        new DataPrinter{ brush = color, font = fontKey,   value = "NIT:", x = xKey, y = y+=sum+10 },
                        new DataPrinter{ brush = color, font = fontValue, value = "xxx xxx xxx-x" ?? string.Empty, x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value = "Transacción", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = "134514", x = x, y = y },
                        //new DataPrinter{ brush = color, font = fontValue, value = transaction.IdTransactionAPi.ToString(), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Referencia", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = transaction.reference, x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Fecha", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = DateTime.Now.ToString("yyyy/MM/dd"), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Hora", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = DateTime.Now.ToString("hh:mm:ss"), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Estado", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = transaction.StatePay, x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value =  "Valor a Pagar", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Amount), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value =  "Valor Ingresado", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Payment.ValorIngresado), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value =  "Valor Devuelto", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Payment.ValorSobrante), x = x, y = y },


                        new DataPrinter{ brush = color, font = fontKey,   value =  "Puntos", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.UserPoints > 0 ? transaction.UserPoints : 0), x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter { imageQR = GenerateCode(string.Concat(Utilities.GetConfiguration("Menu"),transaction.reference), 2), x = 100, y = y+sum },

                    };

                    y = y + 100;

                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "Escanea el código QR", x = xKey, y = y += sum });                   
                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "para conocer el estado de tu producto", x = xKey, y = y += 20 });

                    y = y + 20;

                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "========================================", x = xKey, y = y += sum });                   

                    if (!transaction.StateReturnMoney)
                    {
                        //data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.ReturnMoneyMs1, x = xKey, y = y += sum });
                        //data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.ReturnMoneyMs2, x = xKey, y = y += 20 });
                    }

                    data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.PrintMs1, x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.PrintMs2, x = xKey, y = y += 20 });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "E-city Software", x = 100, y = y += sum });

                    AdminPayPlus.PrintService.Start(data);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }
        }

        public static System.Drawing.Image GenerateCode(string code, int size)
        {
            try
            {
                CodeQrBarcodeDraw qrcode = BarcodeDrawFactory.CodeQr;
                return qrcode.Draw(code, 1, size);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
                return null;
            }
        }

        public static decimal RoundValue(decimal Total, bool arriba)
        {
            try
            {
                decimal roundTotal = 0;

                if (arriba)
                {
                    roundTotal = Math.Ceiling(Total / 100) * 100;
                }
                else
                {
                    roundTotal = Math.Floor(Total / 100) * 100;
                }

                return roundTotal;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return Total;
            }
        }

        public static bool ValidateModule(decimal module, decimal amount)
        {
            try
            {
                var result = (amount % module);
                return result == 0 ? true : false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return false;
            }
        }

        public static T ConverJson<T>(string path)
        {
            T response = default(T);
            try
            {
                using (StreamReader file = new StreamReader(path, Encoding.UTF8))
                {
                    try
                    {
                        var json = file.ReadToEnd().ToString();
                        if (!string.IsNullOrEmpty(json))
                        {
                            response = JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }

            return response;
        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return false;
            }
        }

        public static void Speak(string text)
        {
            try
            {
                if (GetConfiguration("ActivateSpeak").ToUpper() == "SI")
                {
                    if (speechSynthesizer == null)
                    {
                        speechSynthesizer = new SpeechSynthesizer();
                    }

                    speechSynthesizer.SpeakAsyncCancelAll();
                    speechSynthesizer.SelectVoice("Microsoft Sabina Desktop");
                    speechSynthesizer.SpeakAsync(text);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string[] ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }

            return null;
        }

        public static string GetIpPublish()
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(GetConfiguration("UrlGetIp"));
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }

            return GetConfiguration("IpDefoult");
        }

        public static void OpenKeyboard(bool keyBoard_Numeric, object objTxt, object thisView, int x = 0, int y = 0)
        {
            try
            {
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = objTxt,
                    userControl = thisView is UserControl ? thisView as UserControl : null,
                    eType = (keyBoard_Numeric == true) ? WPKeyboard.Keyboard.EType.Numeric : WPKeyboard.Keyboard.EType.Standar,
                    window = thisView is Window ? thisView as Window : null,
                    X = x,
                    Y = y
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string[] ErrorDevice()
        {
            try
            {
                string[] keys = Utilities.ReadFile(@"" + ConstantsResource.PathDevice);

                return keys;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return null;
            }
        }

        public static bool IsMultiple(decimal value)
        {
            try
            {
                if (value % 100 != 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return true;
        }
    }
}
