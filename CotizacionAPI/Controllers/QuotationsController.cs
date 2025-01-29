using CotizacionAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CotizacionAPI.Controllers
{
    public class QuotationsController : ApiController
    {
        private QuotationDB quotationDB = new QuotationDB();

        // POST /quotations - Crear una nueva cotización
        [HttpPost]
        public IHttpActionResult CreateQuotation([FromBody] CreateQuotationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientName) || string.IsNullOrEmpty(request.ClientPhone))
            {
                return BadRequest("El nombre y teléfono del cliente son obligatorios.");
            }

            try
            {
                int quotationId = quotationDB.CreateQuotation(request.ClientName, request.ClientPhone);
                return Ok(new { QuotationId = quotationId }); // Devuelve el ID de la cotización creada
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al crear la cotización", ex));
            }
        }

        // POST /quotations/{quotationId}/details - Agregar productos a una cotización
        [HttpPost]
        [Route("api/quotations/{quotationId}/details")]
        public IHttpActionResult AddProductToQuotation(int quotationId, [FromBody] AddProductRequest request)
        {
            if (request == null || request.Quantity <= 0 || request.ProductId <= 0)
            {
                return BadRequest("Debe especificar un producto válido y una cantidad mayor a 0.");
            }

            try
            {
                quotationDB.CreateQuotationDetail(quotationId, request.ProductId, request.Quantity);
                return Ok("Producto agregado a la cotización con éxito.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al agregar el producto a la cotización", ex));
            }
        }

        // GET /quotations - Obtener todas las cotizaciones con sus productos
        [HttpGet]
        public IHttpActionResult GetAllQuotations()
        {
            try
            {
                List<Quotation> quotations = quotationDB.GetAllQuotations();
                return Ok(quotations); // Devuelve la lista de cotizaciones con sus productos
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al obtener las cotizaciones", ex));
            }
        }
    }

    // Clases de solicitud para crear cotización y agregar productos
    public class CreateQuotationRequest
    {
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
    }

    public class AddProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
