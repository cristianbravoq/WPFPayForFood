using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPayForFood.Models
{
    public class SMSModel
    {
        public int CLIENT_ID { get; } = 1;
        public string INDICATIVE { get; } = "57";
        public string PHONE_NUMBER { get; set; }
        public string SMS { get; set; }

    }
}
