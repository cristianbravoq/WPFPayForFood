using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Services.ObjectIntegration;

namespace WPFPayForFood.Windows.Alerts
{
    /// <summary>
    /// Interaction logic for BasketPay.xaml
    /// </summary>
    public partial class BasketPay : Window
    {
        #region "Referencias"
        private CollectionViewSource view;
        private ObservableCollection<Datum> lstPager;
        public decimal Amount;
        public bool clear;
        #endregion

        #region "Constructor"
        public BasketPay(List<Datum> productsCars)
        {
            InitializeComponent();

            try
            {
                Utilities.Products = productsCars;
                this.clear = false;
                this.view = new CollectionViewSource();
                this.lstPager = new ObservableCollection<Datum>();
                InitView();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Métodos"
        private void InitView()
        {
            try
            {
                Amount = 0;

                foreach (var item in Utilities.Products)
                {
                    Amount += item.precio;
                    lstPager.Add(item);
                }

                txtAmount.Text = String.Format("{0:C0}", Amount);
                view.Source = lstPager;
                lv_Productos.DataContext = view;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Eventos"
        private void BtnClose_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnClear_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Utilities.Products.Clear();
                clear = true;
                DialogResult = false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void BtnPay_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
        #endregion
    }
}
