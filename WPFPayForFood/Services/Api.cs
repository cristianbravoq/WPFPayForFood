﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFPayForFood.Classes;
using WPFPayForFood.DataModel;
using WPFPayForFood.Services.Object;

namespace WPFPayForFood.Services
{
    public class Api
    {
        #region "Referencias"
        private HttpClient client;
        private RequestAuth requestAuth;
        private static RequestApi requestApi;
        private string basseAddress;
        private int type = 1;
        private static string token;
        #endregion

        #region "Constructor"
        public Api()
        {
            try
            {
                if (requestAuth == null)
                {
                    requestAuth = new RequestAuth();
                }

                if (requestApi == null)
                {
                    requestApi = new RequestApi();
                }

                basseAddress = Utilities.GetConfiguration("basseAddress");
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Métodos"
        public async Task<ResponseAuth> GetSecurityToken(CONFIGURATION_PAYDAD config)
        {
            try
            {
                if (config != null)
                {
                    client = new HttpClient();
                    client.BaseAddress = new Uri(basseAddress);

                    requestAuth.UserName = config.USER;
                    requestAuth.Password = config.PASSWORD;
                    requestAuth.Type = this.type;

                    var request = JsonConvert.SerializeObject(requestAuth);
                    var content = new StringContent(request, Encoding.UTF8, "Application/json");
                    var url = Utilities.GetConfiguration("GetToken");

                    var authentication = Encoding.ASCII.GetBytes(config.USER_API + ":" + config.PASSWORD_API);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));

                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        AdminPayPlus.SaveLog(new RequestLog
                        {
                            Reference = "",
                            Description = "No respondio el servicio GetSecurityToken. " + response.ReasonPhrase,
                            State = 2,
                            Date = DateTime.Now
                        }, ELogType.General);

                        return null;
                    }

                    var result = await response.Content.ReadAsStringAsync();

                    if (result != null)
                    {
                        var requestresponse = JsonConvert.DeserializeObject<ResponseAuth>(result);

                        if (requestresponse != null)
                        {
                            if (requestresponse.CodeError == 200)
                            {
                                token = requestresponse.Token;
                                requestApi.Session = Convert.ToInt32(requestresponse.Session);
                                requestApi.User = Convert.ToInt32(requestresponse.User);

                                return requestresponse;
                            }
                            else
                            {
                                AdminPayPlus.SaveLog(new RequestLog
                                {
                                    Reference = "",
                                    Description = string.Concat("Codigo de error: ", requestresponse.CodeError, " GetSecurityToken: ", requestresponse.Message),
                                    State = 2,
                                    Date = DateTime.Now
                                }, ELogType.General);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                return null;
            }
        }

        public async Task<object> CallApi(string controller, object data = null)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

                requestApi.Data = data;

                var request = JsonConvert.SerializeObject(requestApi);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration(controller);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = string.Concat("No respondio el servicio. ", controller, " response:", response.ReasonPhrase),
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);

                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseApi>(result);

                if (responseApi.CodeError == 200)
                {
                    if (responseApi.Data == null)
                    {
                        return "OK";
                    }

                    return responseApi.Data;
                }
                else
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = string.Concat("Codigo de error: ", responseApi.CodeError, " CallApi-", controller, " ", responseApi.Message),
                        State = 1,
                        Date = DateTime.Now
                    }, ELogType.General);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }
        #endregion
    }
}
