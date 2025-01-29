using CotizacionAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CotizacionAPI.Controllers
{
    public class ProductsController : ApiController
    {
        public IHttpActionResult GetAllProducts()
        {
            try
            {
                QuotationDB quotationDB = new QuotationDB();

                List<Product> products = quotationDB.GetProducts();

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(products);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
