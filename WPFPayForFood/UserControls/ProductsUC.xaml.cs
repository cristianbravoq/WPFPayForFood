using Newtonsoft.Json;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Services.ObjectIntegration;
using WPFPayForFood.Windows.Alerts;

namespace WPFPayForFood.UserControls
{
    /// <summary>
    /// Interaction logic for ProductsUC.xaml
    /// </summary>
    public partial class ProductsUC : UserControl
    {
        #region "Referencias"
        private CollectionViewSource view;
        private ObservableCollection<Datum> lstPager;
        private Transaction transaction;
        #endregion

        #region "Constructor"
        public ProductsUC(Transaction ts)
        {
            InitializeComponent();
            view = new CollectionViewSource();
            lstPager = new ObservableCollection<Datum>();
            transaction = ts;
            transaction.productos = new List<Datum>();
            Restaurante.Text = transaction.lstComidas.Data[0].nombrE_RESTAURANTE;
            InitView();
        }
        #endregion

        #region "Métodos"
        private void InitView()
        {
            try
            {
                foreach (var product in transaction.lstComidas.Data)
                {
                    lstPager.Add(product);
                }

                if (lstPager.Count > 0)
                {
                    view.Source = lstPager;
                    lv_Products.DataContext = view;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Eventos"
        private void btnCarrito_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.Opacity = 0.3;
                BasketPay modal = new BasketPay(transaction.productos);
                modal.ShowDialog();

                if (modal.DialogResult.HasValue && modal.DialogResult.Value)
                {
                    if (modal.Amount > 0 && transaction.productos.Count() > 0)
                    {
                        transaction.Amount = Math.Ceiling(modal.Amount);

                        if (Utilities.ShowModal("Desea acumular puntos para adquirir descuentos en sus compras?", EModalType.Information, this))
                        {
                            Utilities.navigator.Navigate(UserControlView.UserPoints, transaction);
                        } else
                        {
                            Utilities.navigator.Navigate(UserControlView.Pay, transaction);
                        }
                            
                    }
                }
                else
                {
                    if (modal.clear)
                    {
                        transaction.productos = new List<Datum>();
                    }
                }

                this.Opacity = 1;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Agregar_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var comida = (sender as Image).DataContext as Datum;

                transaction.ComidaSelect = comida;

                this.Opacity = 0.3;
                DetailFoodW foodW = new DetailFoodW(transaction);
                foodW.ShowDialog();
                this.Opacity = 1;

                if (foodW.DialogResult.HasValue && foodW.DialogResult.Value)
                {
                    var product = transaction.productos.FirstOrDefault(p => p.iD_PRODUCTO == comida.iD_PRODUCTO);

                    if (product == null)
                    {
                        transaction.productos.Add(foodW.ProductsSelects);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void btnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void btnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Main);
        }
        #endregion
    }

}
