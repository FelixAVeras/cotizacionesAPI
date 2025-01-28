using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CotizacionAPI.Models
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public DateTime DateCreated { get; set; }
        public List<QuotationDetail> Details { get; set; }
    }
}