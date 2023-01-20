using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WPFPayForFood.Classes;
using WPFPayForFood.Models;

namespace WPFPayForFood.Services.Object
{
    public class Response
    {
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
