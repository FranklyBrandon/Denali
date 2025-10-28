using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveBrokers.Models
{
    public class OrderRequest
    {
        public string accountId { get; set; }
        public int conid { get; set; }
        public string orderType { get; set; }
        public string side { get; set; }
        public float price { get; set; }
        public string tif { get; set; }
        public float quantity { get; set; }

        public string cOID { get; set; }
        public string parentId { get; set; }
    }
}
