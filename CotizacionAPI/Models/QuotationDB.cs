using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CotizacionAPI.Models
{
    public class QuotationDB
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=CotizacionesDB";

        public bool OK()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();

            string query = "select * products";

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(query, connection);

                try
                {
                    connection.Open();

                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while(reader.Read())
                    {
                        Product product = new Product
                        {
                            ProductId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Description = reader.GetString(3)
                        };

                        products.Add(product);
                    }

                    reader.Close();
                    connection.Close();
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return products;
        }

        public int CreateQuotation(string clientName, string clientPhone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Quotations (ClientName, ClientPhone) OUTPUT INSERTED.QuotationId VALUES (@ClientName, @ClientPhone)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientName", clientName);
                cmd.Parameters.AddWithValue("@ClientPhone", clientPhone);

                return (int)cmd.ExecuteScalar(); // Devuelve el ID de la cotización creada
            }
        }

        public void CreateQuotationDetail(int quotationId, int productId, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO QuotationDetails (QuotationId, ProductId, Quantity) VALUES (@QuotationId, @ProductId, @Quantity)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@QuotationId", quotationId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                cmd.ExecuteNonQuery(); // Inserta el detalle en la base de datos
            }
        }

        public List<Quotation> GetAllQuotations()
        {
            List<Quotation> quotations = new List<Quotation>();

            string query = @"
                SELECT q.QuotationId, q.ClientName, q.ClientPhone, q.DateCreated, 
                       qd.ProductId, p.Name, p.Price, qd.Quantity, (qd.Quantity * p.Price) AS TotalAmount
                FROM Quotations q
                INNER JOIN QuotationDetails qd ON q.QuotationId = qd.QuotationId
                INNER JOIN Products p ON qd.ProductId = p.ProductId
                ORDER BY q.QuotationId DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // Buscar si la cotización ya existe en la lista
                        Quotation quotation = quotations.FirstOrDefault(q => q.QuotationId == reader.GetInt32(0));
                        if (quotation == null)
                        {
                            quotation = new Quotation
                            {
                                QuotationId = reader.GetInt32(0),
                                ClientName = reader.GetString(1),
                                ClientPhone = reader.GetString(2),
                                DateCreated = reader.GetDateTime(3),
                                Products = new List<QuotationDetail>()
                            };
                            quotations.Add(quotation);
                        }

                        // Agregar los detalles de la cotización
                        quotation.Products.Add(new QuotationDetail
                        {
                            ProductId = reader.GetInt32(4),
                            ProductName = reader.GetString(5),
                            Price = reader.GetDecimal(6),
                            Quantity = reader.GetInt32(7),
                            TotalAmount = reader.GetDecimal(8)
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return quotations;
        }

        public void InsertDefaultProducts()
        {
            List<Product> existingProducts = GetProducts(); // Obtén los productos existentes en la DB

            // Verificar si ya existen los productos
            if (existingProducts.Count == 0)
            {
                // Si no hay productos, inserta los 5 productos por defecto
                string[] productNames = new string[] {
            "Computadora de Escritorio",
            "Laptop HP Spectre x360",
            "Monitor 24 pulgadas",
            "Teclado mecánico Logitech",
            "Mouse inalámbrico Logitech"
        };

                decimal[] productPrices = new decimal[] { 1200.00m, 1500.00m, 300.00m, 100.00m, 50.00m };

                string[] productDescriptions = new string[] {
            "Computadora de escritorio con procesador Intel i7, 16GB de RAM y 512GB SSD",
            "Laptop convertible 2 en 1, procesador Intel i7, 16GB de RAM y 1TB SSD",
            "Monitor LED de 24 pulgadas Full HD con puertos HDMI y DisplayPort",
            "Teclado mecánico con retroiluminación RGB, switches Cherry MX",
            "Mouse inalámbrico ergonómico con conexión Bluetooth y batería recargable"
        };

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    for (int i = 0; i < productNames.Length; i++)
                    {
                        string query = "INSERT INTO Products (Name, Price, Description) VALUES (@Name, @Price, @Description)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Name", productNames[i]);
                        cmd.Parameters.AddWithValue("@Price", productPrices[i]);
                        cmd.Parameters.AddWithValue("@Description", productDescriptions[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}