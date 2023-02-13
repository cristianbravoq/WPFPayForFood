using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Services.ObjectIntegration;

namespace WPFPayForFood.Windows.Alerts
{
    /// <summary>
    /// Interaction logic for DetailFoodW.xaml
    /// </summary>
    public partial class DetailFoodW : Window
    {
        #region "Referencias"
        private CollectionViewSource view;
        private ObservableCollection<Categoria> lstPager;
        private Transaction transaction;
        private decimal Valor;
        public Datum ProductsSelects;
        #endregion

        #region "Constructor"
        public DetailFoodW(Transaction ts)
        {
            InitializeComponent();
            view = new CollectionViewSource();
            lstPager = new ObservableCollection<Categoria>();
            transaction = ts;
            Valor = transaction.ComidaSelect.precio;
            this.DataContext = transaction.ComidaSelect;
            InitView();
        }
        #endregion

        #region "Métodos"
        private void InitView()
        {
            try
            {
                ProductsSelects = new Datum();

                ProductsSelects.iD_RESTAURANTE = transaction.ComidaSelect.iD_RESTAURANTE;
                ProductsSelects.iD_PRODUCTO = transaction.ComidaSelect.iD_PRODUCTO;
                ProductsSelects.categorias = new System.Collections.Generic.List<Categoria>();

           
                if(transaction.ComidaSelect.categorias.Count > 0)
                {
                    foreach (var product in transaction.ComidaSelect.categorias)
                    {
                        if (product.recetas != null)
                        {
                            foreach (var item in product.recetas)
                            {
                                item.data = product;
                                item.img = item.seleccionable ? "/Images/Others/radio.png" : "/Images/Others/checkInput.png";
                                item.selected = item.selected;
                                item.removible = item.removible;
                            }
                        }

                        lstPager.Add(product);
                    }

                    if (lstPager.Count > 0)
                    {
                        view.Source = lstPager;
                        lv_Products.DataContext = view;
                    }
                }
                else
                {
                    view.Source = null;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ModificProductsSelect(Categoria categoria, Receta receta, bool add)
        {
            try
            {
                var data = ProductsSelects.categorias.FirstOrDefault(x => x.iD_CATEGORIA == categoria.iD_CATEGORIA);

                if (add)
                {
                    if (data != null)
                    {
                        var rec = data.recetas.FirstOrDefault(r => r.iD_RECETA == receta.iD_RECETA);
                        if (rec == null)
                        {
                            data.recetas.Add(new Receta
                            {
                                cantidad = receta.cantidad,
                                iD_RECETA = receta.iD_RECETA
                            });
                        }
                    }
                    else
                    {
                        AdCategory(categoria, receta);
                    }
                }
                else
                {
                    if (data != null)
                    {
                        var rec = data.recetas.FirstOrDefault(r => r.iD_RECETA == receta.iD_RECETA);
                        if (rec != null)
                        {
                            data.recetas.Remove(rec);
                        }

                        if (data.recetas.Count() == 0)
                        {
                            ProductsSelects.categorias.Remove(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void AdCategory(Categoria categoria, Receta receta)
        {
            try
            {
                ProductsSelects.categorias.Add(new Categoria
                {
                    iD_CATEGORIA = categoria.iD_CATEGORIA,
                    recetas = new System.Collections.Generic.List<Receta>
                    {
                        new Receta
                        {
                            cantidad = receta.cantidad,
                            iD_RECETA= receta.iD_RECETA
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Operation(bool sumar)
        {
            try
            {
                int count = Convert.ToInt32(txtCount.Text);

                if (sumar)
                {
                    if (count < transaction.ComidaSelect.stock)
                    {
                        transaction.ComidaSelect.precio += Valor;
                        count++;
                        txtCount.Text = count.ToString();
                    }
                }
                else
                {
                    if (count > 1)
                    {
                        transaction.ComidaSelect.precio -= Valor;
                        count--;
                        txtCount.Text = count.ToString();
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region "Eventos"
        private void Select_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var categoriaAll = ((sender as Image).Parent as Grid).Tag as Categoria;

                var data = (sender as Image).DataContext as Receta;

                if (data != null && categoriaAll != null)
                {

                    data.selected = !data.selected;

                    if (data.removible)
                    {
                        data.img = data.selected ? "/Images/Others/CheckInputS.png" : "/Images/Others/CheckInput.png";
                        data.cantidad = data.selected ? 1 : 0;
                    }
                    else
                    if (data.seleccionable)
                    {
                        foreach (var item in categoriaAll.recetas)
                        {
                            if (item != data)
                            {
                                item.img = "/Images/Others/radio.png";
                                item.selected = false;
                                item.cantidad = 0;
                            }
                        }

                        data.img = data.selected ? "/Images/Others/radioOK.png" : "/Images/Others/radio.png";
                        data.cantidad = data.selected ? 1 : 0;
                    }

                    ModificProductsSelect(categoriaAll, data, data.selected);

                    lv_Products.Items.Refresh();
                }
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
                ProductsSelects.cantidad = int.Parse(txtCount.Text);
                ProductsSelects.precio = transaction.ComidaSelect.precio;
                ProductsSelects.nombrE_PRODUCTO = transaction.ComidaSelect.nombrE_PRODUCTO;
          //      ProductsSelects.comentarios = txtComentarios.Text;
                ProductsSelects.imagen = transaction.ComidaSelect.imagen;
                ProductsSelects.descripcion = transaction.ComidaSelect.descripcion;

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Close_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSum_TouchDown(object sender, TouchEventArgs e)
        {
            Operation(true);
        }

        private void btnSubs_TouchDown(object sender, TouchEventArgs e)
        {
            Operation(false);
        }

        private void txtComentarios_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, sender, this, 440);
        }
        #endregion

        private void lv_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
