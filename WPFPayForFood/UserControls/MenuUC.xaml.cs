using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFPayForFood.Classes;
using WPFPayForFood.Classes.UseFull;
using WPFPayForFood.Models;
using WPFPayForFood.Resources;
using WPFPayForFood.Services;

namespace WPFPayForFood.UserControls
{
    /// <summary>
    /// Lógica de interacción para MenuUserControl.xaml
    /// </summary>
    public partial class MenuUC : UserControl
    {
        public class Restaurants
        {
            public int idRestaurant { get; set; }
            public string Nombre { get; set; }
            public string Imagen { get; set; }
            public bool enable { get; set; }
            public string opacity { get; set; }

        }

        #region "Referencias"
        private Transaction transaction;
        private TimerGeneric timer;
        private CollectionViewSource view;
        private ObservableCollection<Restaurants> lstPager;
        #endregion

        #region "Constructor"
        public MenuUC(Transaction transaction)
        {
            InitializeComponent();
            view = new CollectionViewSource();
            lstPager = new ObservableCollection<Restaurants>();
      

            if(transaction == null)
            {
                this.transaction = new Transaction();
               GetRestaurants();
            }
            else
            {
                this.transaction = transaction;
            }


            InitView();
        }
        #endregion

        #region "Métodos"
        private void InitView()
        {
            try
            {
                List<Restaurants> restaurants = new List<Restaurants>();

                if (transaction.LstRestaurantes != null)
                {
                    foreach (var data in transaction.LstRestaurantes.data)
                    {
                        restaurants.Add(new Restaurants
                        {
                            idRestaurant = data.iD_RESTAURANTE,
                            Nombre = data.nombrE_RESTAURANTE,
                            Imagen = $@"/Images/Restaurants/{data.nombrE_RESTAURANTE}.png",
                            enable = true,
                            opacity = "1"
                        });
                    }
                }
                

                foreach (var item in restaurants)
                {
                    lstPager.Add(item);
                }

                if (lstPager.Count > 0)
                {
                    view.Source = lstPager;
                    lv_Products.DataContext = view;
                }

                //restaurants.Add(new Restaurants
                //{
                //    idRestaurant = 2,
                //    Nombre = "Mitos",
                //    Imagen = "/Images/Restaurants/e.png",
                //    enable = false,
                //    opacity = "0.4"
                //}); 
                //restaurants.Add(new Restaurants
                //{
                //    idRestaurant = 3,
                //    Nombre = "Montolivo",
                //    Imagen = "/Images/Restaurants/f.png",
                //    enable = false,
                //    opacity = "0.4"
                //});
                //restaurants.Add(new Restaurants
                //{
                //    idRestaurant = 4,
                //    Nombre = "Frisby",
                //    Imagen = "/Images/Restaurants/q.png",
                //    enable = false,
                //    opacity = "0.4"
                //});


            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async void GetRestaurants()
        {
            try
            {
                Task.Run(async () =>
                {
                    transaction.LstRestaurantes = await AdminPayPlus.apiIntegration.GetRestaurantes();

                    Utilities.CloseModal();

                    if (transaction.LstRestaurantes == null)
                    {
                        Utilities.ShowModal("Ha ocurrido un error al consultar el menú. Por favor intenta de nuevo.", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Main, true);
                    }
                    //else
                    //{
                    //    Utilities.navigator.Navigate(UserControlView.Menu, transaction);
                    //}
                });
                //     ObtenerRestaurantes();

                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void SearchMenu(int idRestaurant)
        {
            try
            {
                Task.Run(() =>
                {
                    transaction.lstComidas =  AdminPayPlus.apiIntegration.SearchMenu(idRestaurant).GetAwaiter().GetResult();

                    Utilities.CloseModal();

                    if (transaction.lstComidas == null)
                    {
                        Utilities.ShowModal("Ha ocurrido un error al consultar el menú. Por favor intenta de nuevo.", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Main,true);
                    }
                    else
                    {
                        Utilities.navigator.Navigate(UserControlView.Products, transaction);
                    }
                });

                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

       

        private async Task ObtenerRestaurantes()
        {
            transaction.LstRestaurantes = await AdminPayPlus.apiIntegration.GetRestaurantes();
            Utilities.CloseModal();
        }

        #endregion

        #region "Eventos"
        private void Select_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                int restaurant = Convert.ToInt32((sender as Image).Tag);

                SearchMenu(restaurant);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void btnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Main);
        }
        #endregion

        #region "Timer"
        private void ActivateTimer()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerGenerico");
                timer = new TimerGeneric(tbTimer.Text);
                timer.CallBackClose = response =>
                {
                    Utilities.navigator.Navigate(UserControlView.Main);
                };
                timer.CallBackTimer = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        tbTimer.Text = response;
                    });
                };
            });
            GC.Collect();
        }

        private void SetCallBacksNull()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
            });
            GC.Collect();
        }
        #endregion

        private void Select_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int restaurant = Convert.ToInt32((sender as Image).Tag);

                SearchMenu(restaurant);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
    }
}
