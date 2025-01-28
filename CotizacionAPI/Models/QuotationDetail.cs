using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CotizacionAPI.Models
{
    public class QuotationDetail
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}