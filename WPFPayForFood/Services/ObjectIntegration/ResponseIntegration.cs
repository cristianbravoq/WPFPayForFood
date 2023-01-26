using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPayForFood.Services.ObjectIntegration
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Receta
    {
        public Categoria data { get; set; }
        public bool selected { get; set; }
        public string img { get; set; }

        public int iD_RECETA { get; set; }

        public int iD_CATEGORIA { get; set; }

        public string iteM_RECETA { get; set; }

        public decimal valor { get; set; }
        public int cantidad { get; set; }

        public bool seleccionable { get; set; }

        public bool removible { get; set; }

        public int cantidaD_DISPONIBLE { get; set; }

        public bool disponible { get; set; }
    }

    public class Categoria
    {
        public int iD_CATEGORIA { get; set; }

        public int iD_PRODUCTO { get; set; }

        public string nombre { get; set; }

        public bool disponible { get; set; }

        public bool obligatoria { get; set; }

        public List<Receta> recetas { get; set; }
    }

    public class Datum : INotifyPropertyChanged
    {

        private decimal _Precio;
        public decimal precio
        {
            get { return _Precio; }
            set
            {
                _Precio = value;
                NotifyPropertyChanged("precio");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public int iD_RESTAURANTE { get; set; }

        public string nombrE_RESTAURANTE { get; set; }

        public object direccion { get; set; }

        public int horA_APERTURA { get; set; }

        public int horA_CIERRE { get; set; }

        public bool restaurantE_DISPONIBLE { get; set; }

        public int iD_PRODUCTO { get; set; }

        public string nombrE_PRODUCTO { get; set; }

        public string imagen { get; set; }

        public string descripcion { get; set; }

        public int cantidad { get; set; }

        public int stock { get; set; }

        public bool disponible { get; set; }

        public string comentarios { get; set; }

        public List<Categoria> categorias { get; set; }
    }

    public class Comidas
    {
        [JsonProperty("codeError")]
        public int CodeError { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }

    public class ProductsSelects
    {
        public List<Datum> productos { get; set; }
        public decimal totaL_VENTA { get; set; }
        public string nombrE_CLIENTE { get; set; }
        public string comentario { get; set; }

        public float Real_Amount { get; set; }

        public float Income_Amount { get; set; }

        public float Return_Amount { get; set; }

        public int PAYERID { get; set; }
        public int iD_RESTAURANTE { get; set; }

    }


    public class ResponsePayMenu
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }



    public class ResponseRestaurante
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public Restaurantes[] data { get; set; }
    }

    public class Restaurantes
    {
        public int iD_RESTAURANTE { get; set; }
        public string nombrE_RESTAURANTE { get; set; }
        public string direccion { get; set; }
    }


    public class ResponseCreatePayer
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public int data { get; set; }
    }

    public class ResponseGetPayerDocument
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public List<DataPayerDocument> data { get; set; }
    }

    public class DataPayerDocument
    {
        public string payer { get; set; }
        public string email { get; set; }
        public string cel { get; set; }
        public int iD_PAYER { get; set; }
        public string documentO_ID { get; set; }
        public string fechA_NACIMIENTO { get; set; }
        public string points { get; set; }
    }


}
