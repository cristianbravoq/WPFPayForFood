using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPayForFood.Services.ObjectIntegration
{
    public class SearchProduct
    {
        public int id_Restaurante { get; set; }
    }


    public class RequestCreatePayer
    {
        public string nombre { get; set; }
        public string email { get; set; }
        public string cel { get; set; }
    }


    public class RequestCreatePayerPoints
    {
        public string nombre { get; set; }
        public string email { get; set; }
        public string cel { get; set; }
        public string documentO_ID { get; set; }
        public string fechA_NACIMIENTO { get; set; }
        public string points { get; set; }
    }

    public class RequestSetPayerData
    {
        public string email { get; set; }
        public string cel { get; set; }
        public int idPayer { get; set; }
    }


    public class RequestGetPayer
    {
        public string iD_PAYER { get; set; }
    }

    public class RequestGetPayerDocument
    {
        public int documentO_ID { get; set; }
    }

    public class PagadorObtenido
    {
        public string codeError { get; set; }
        public string message { get; set; }
        public PagadorData data { get; set; }
    }
    public class PagadorData
    {
        public string payer { get; set; }
        public string email { get; set; }
        public string cel { get; set; }
        public string iD_PAYER { get; set; }
        public string documentO_ID { get; set; }
        public string fechA_NACIMIENTO { get; set; }
        public string points { get; set; }
    }

    public class RequestSetPayerPoints
    {
        public string Documento { get; set; }

        public string Points { get; set; }
    }

    public class ResponseSetPoints
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
