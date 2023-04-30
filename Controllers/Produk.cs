using kalbestore_be.Models;
using kalbestore_be.utils;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;

namespace kalbestore_be.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ProdukController : ControllerBase
    {
        private readonly string _connectionString = dbconfig.ConnectionString;

        // Get All Produk
        [HttpGet("produk")]
        public ActionResult<IEnumerable<Produk>> GetAllProduk()
        {
            var produkList = new List<Produk>();

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                var sql = "SELECT * FROM produk";
                using var cmd = new NpgsqlCommand(sql, con);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var produk = new Produk
                    {
                        ProductID = rdr.GetInt32(0),
                        ProductCode = rdr.GetString(1),
                        ProductName = rdr.GetString(2),
                        Qty = rdr.GetInt32(3),
                        Price = rdr.GetDecimal(4),
                        Inserted = rdr.GetDateTime(5)
                    };
                    produkList.Add(produk);
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error retrieving data from database: {ex.Message}");
            }

            return Ok(new { data = produkList });
        }

        // Add Produk
        [HttpPost("/produk")]
        public IActionResult AddProduk([FromBody] AddProdukRequest produk)
        {
            int productId;
            DateTime inserted;

            inserted = DateTime.Now;

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                using var cmd = new NpgsqlCommand("INSERT INTO produk (txtProductCode, txtProductName, intQty, decPrice, dtInserted) VALUES (@ProductCode, @ProductName, @Qty, @Price, @Inserted) RETURNING intProductID", con);
                cmd.Parameters.AddWithValue("ProductCode", produk.ProductCode);
                cmd.Parameters.AddWithValue("ProductName", produk.ProductName);
                cmd.Parameters.AddWithValue("Qty", produk.Qty);
                cmd.Parameters.AddWithValue("Price", produk.Price);
                cmd.Parameters.AddWithValue("Inserted", inserted);

                productId = (int)cmd.ExecuteScalar();
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error inserting data into database: {ex.Message}");
            }

            return Ok(new { message = "Berhasil menambahkan produk baru!", id = productId, insertedDate = inserted, data = produk });
        }

        // Get Produk by ID
        [HttpGet("/product/{productId}")]
        public ActionResult<Produk> GetProdukById(int productId)
        {
            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                var sql = "SELECT * FROM produk WHERE intProductID = @ProductId";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("ProductId", productId);
                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var produk = new Produk
                    {
                        ProductID = rdr.GetInt32(0),
                        ProductCode = rdr.GetString(1),
                        ProductName = rdr.GetString(2),
                        Qty = rdr.GetInt32(3),
                        Price = rdr.GetDecimal(4),
                        Inserted = rdr.GetDateTime(5)
                    };
                    return Ok(new { data = produk });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error retrieving data from database: {ex.Message}");
            }
        }

        // Get Produk by Name
        [HttpGet("/produkNama")]
        public ActionResult<IEnumerable<Produk>> GetProdukByName([FromQuery(Name = "productName")] string productName)
        {
            var produkList = new List<Produk>();

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                var sql = "SELECT * FROM produk WHERE txtProductName ILIKE @ProductName";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("ProductName", "%" + productName + "%");
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var produk = new Produk
                    {
                        ProductID = rdr.GetInt32(0),
                        ProductCode = rdr.GetString(1),
                        ProductName = rdr.GetString(2),
                        Qty = rdr.GetInt32(3),
                        Price = rdr.GetDecimal(4),
                        Inserted = rdr.GetDateTime(5)
                    };
                    produkList.Add(produk);
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error retrieving data from database: {ex.Message}");
            }

            return Ok(new { data = produkList });
        }


        // Edit Produk
        [HttpPut("/produk/{id}")]
        public IActionResult EditProduk(int id, [FromBody] AddProdukRequest produk)
        {
            DateTime updated = DateTime.Now;

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                using var cmd = new NpgsqlCommand("UPDATE produk SET txtProductCode = @ProductCode, txtProductName = @ProductName, intQty = @Qty, decPrice = @Price, dtInserted = @Updated WHERE intProductID = @ID", con);
                cmd.Parameters.AddWithValue("ProductCode", produk.ProductCode);
                cmd.Parameters.AddWithValue("ProductName", produk.ProductName);
                cmd.Parameters.AddWithValue("Qty", produk.Qty);
                cmd.Parameters.AddWithValue("Price", produk.Price);
                cmd.Parameters.AddWithValue("Updated", updated);
                cmd.Parameters.AddWithValue("ID", id);

                var affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows == 0)
                {
                    return StatusCode(404, $"Produk dengan ID {id} tidak ditemukan");
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error updating data in database: {ex.Message}");
            }

            return Ok(new { message = "Berhasil mengubah produk!", id = id, updatedDate = updated, data = produk });
        }


    }
}
