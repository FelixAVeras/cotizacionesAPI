using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CotizacionAPI.Models
{
    public class QuotationDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}