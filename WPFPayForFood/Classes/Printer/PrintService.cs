using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Threading.Tasks;

namespace WPFPayForFood.Classes.Printer
{
    public class PrintService
    {
        private PrintController printController;

        private PrintDocument printDocument;

        private Graphics graphics;

        private List<DataPrinter> dataPrinter;

        private PrintProperties properties;

        public PrintService()
        {
            try
            {
                if (properties == null)
                {
                    properties = new PrintProperties(Utilities.GetConfiguration("PortPrinter"), Utilities.GetConfiguration("PrintBandrate"));
                }

                if (printController == null)
                {
                    printController = new StandardPrintController();
                }

                if (printDocument == null)
                {
                    printDocument = new PrintDocument();

                    printDocument.PrintController = printController;

                    printDocument.PrintPage += new PrintPageEventHandler(Print);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError("PrintService", "PrintService", ex, ex.ToString());
            }
        }

        public int StatusPrint()
        {
            int status = 1;

            try
            {
                if (properties != null)
                {
                    status = properties.GetPrintStatus();
                    if (status == 0)
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError("StatusPrint", "PrintService", ex, ex.ToString());
            }

            return status;
        }

        public string MessageStatus(int status)
        {
            return properties.MessageStatus(status);
        }

        public void Start(List<DataPrinter> dataPrinter)
        {
            try
            {
                Task.Run(() =>
                {
                    this.dataPrinter = dataPrinter;

                    if (dataPrinter != null)
                    {
                        printDocument.Print();
                    }
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError("Start", "PrintService", ex, ex.ToString());
            }
        }

        private void Print(object sender, PrintPageEventArgs e)
        {
            try
            {
                if (dataPrinter.Count > 0)
                {
                    foreach (var item in dataPrinter)
                    {
                        graphics = e.Graphics;

                        if (item.imageQR != null)
                        {
                            graphics.DrawImage(item.imageQR, item.x, item.y);
                        }
                        else
                        if (!string.IsNullOrEmpty(item.image))
                        {
                            graphics.DrawImage(Image.FromFile(item.image), item.x, item.y);
                        }
                        else
                        if (item.direction != null)
                        {
                            graphics.DrawString(item.value, item.font, item.brush, item.rectangle, item.direction);
                        }
                        else if (item.point.X != 0 && item.point.Y != 0)
                        {
                            graphics.DrawString(item.value, item.font, item.brush, item.point, item.direction);
                        }
                        else
                        {
                            graphics.DrawString(item.value, item.font, item.brush, item.x, item.y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError("Print", "PrintService", ex, ex.ToString());
            }
        }
    }
}