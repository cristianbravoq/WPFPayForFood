using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;
using WPFPayForFood.Services.ObjectIntegration;

namespace WPFPayForFood.Services
{
    public class ApiIntegration
    {
        #region "Referencias"
        private string basseAddress; 
        private HttpClient client;
        #endregion

        public ApiIntegration()
        {
            basseAddress = Utilities.GetConfiguration("basseAddressIntegration");
        }

        public async Task<Comidas> SearchMenu(int id)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

                var data = new SearchProduct { id_Restaurante = id };

                var request = JsonConvert.SerializeObject(data);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration("seachMenu");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<Comidas>(result);

                if (responseApi.CodeError == 200)
                {
                    return responseApi;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }


        public async Task<ResponseRestaurante> GetRestaurantes()
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

         //       var request = JsonConvert.SerializeObject();
               var content = new StringContent("dato",Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration("Restaurants");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseRestaurante>(result);

                if (responseApi.codeError == 200)
                {
                    return responseApi; 
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public async Task<ResponseCreatePayer> CreatePayer(RequestCreatePayerPoints Payer)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

                var request = JsonConvert.SerializeObject(Payer);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration("CreatePayer");

                var response =  client.PostAsync(url, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseCreatePayer>(result);

                if (responseApi.codeError == 200)
                {
                    return responseApi;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public async Task<ResponseCreatePayer> GetPayer(RequestGetPayer idPayer)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

                var request = JsonConvert.SerializeObject(idPayer);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration("GetPayer");

                var response = client.PostAsync(url, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseCreatePayer>(result);

                if (responseApi.codeError == 200)
                {
                    return responseApi;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public async Task<string> GetPayerDocument(string idPayer)
        {
            try
            {
                var client = new RestClient("https://apipayforfood.e-city.co/Payer/GetPayerDocument");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{
" + "\n" +
                @"  ""documentO_ID"": cedula
" + "\n" +
                @"}";
                body = body.Replace("cedula", idPayer);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                return response.Content;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public async Task<ResponsePayMenu> NotifyMenu(ProductsSelects products)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

                var request = JsonConvert.SerializeObject(products);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration("payMenu");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponsePayMenu>(result);

                if (responseApi.codeError == 200)
                {
                    return responseApi;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }
    }
}
