using System;
using System.Runtime.Intrinsics.Arm;
using kalbestore_be.Models;
using kalbestore_be.utils;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace kalbestore_be.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class PenjualanController : ControllerBase
    {
        private readonly string _connectionString = dbconfig.ConnectionString;

        // Add Sales Order
        [HttpPost("salesorder")]
        public IActionResult AddSalesOrder([FromBody] AddPenjualanRequest salesOrder)
        {
            int salesOrderId;
            DateTime OrderDateTime = DateTime.Now;

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();

                // Handle qty pembelian lebih besar dari stok
                using var cmdQty = new NpgsqlCommand("SELECT intQty FROM produk WHERE intProductID = @ProductID", con);
                cmdQty.Parameters.AddWithValue("ProductID", salesOrder.ProductID);

                var qtyInDb = (int)cmdQty.ExecuteScalar();
                if (qtyInDb < salesOrder.Qty)
                {
                    return BadRequest($"Stok produk tidak cukup! Stok tersedia: {qtyInDb}");
                }


                using var cmd = new NpgsqlCommand("INSERT INTO penjualan (intCustomerID, intProductID, dtSalesOrder, intQty) VALUES (@CustomerID, @ProductID, @SalesOrder, @Qty) RETURNING intSalesOrderID", con);
                cmd.Parameters.AddWithValue("CustomerID", salesOrder.CustomerID);
                cmd.Parameters.AddWithValue("ProductID", salesOrder.ProductID);
                cmd.Parameters.AddWithValue("SalesOrder", OrderDateTime);
                cmd.Parameters.AddWithValue("Qty", salesOrder.Qty);

                salesOrderId = (int)cmd.ExecuteScalar();

                // Mengurangi qty di database salesOrderapabila terjadi pembelian
                var newQty = qtyInDb - salesOrder.Qty;
                using var cmdUpdateQty = new NpgsqlCommand("UPDATE produk SET intQty = @Qty WHERE intProductID = @ProductID", con);
                cmdUpdateQty.Parameters.AddWithValue("Qty", newQty);
                cmdUpdateQty.Parameters.AddWithValue("ProductID", salesOrder.ProductID);

                cmdUpdateQty.ExecuteNonQuery();

                using var cmdData = new NpgsqlCommand("SELECT penjualan.intSalesOrderID, customer.txtCustomerName, produk.txtProductName, customer.txtCustomerAddress, penjualan.intQty, produk.decPrice FROM penjualan INNER JOIN customer on penjualan.intcustomerid = customer.intcustomerid INNER JOIN produk on penjualan.intproductid = produk.intproductid WHERE penjualan.intSalesOrderID = @SalesOrderId;", con);
                cmdData.Parameters.AddWithValue("SalesOrderId", salesOrderId);

                using NpgsqlDataReader reader = cmdData.ExecuteReader();

                if (reader.Read())
                {
                    var penjualan = new GetPenjualan
                    {
                        SalesOrderID = reader.GetInt32(0),
                        CustomerName = reader.GetString(1),
                        ProdukName = reader.GetString(2),
                        AlamatPengiriman = reader.GetString(3),
                        Qty = reader.GetDouble(4),
                        TotalHarga = reader.GetInt32(5) * (int)reader.GetDouble(4)
                    };

                    return Ok(new { message = "Berhasil menambahkan pesanan baru!", id = salesOrderId, rawData = salesOrder, data = penjualan });
                }
                else
                {
                    return BadRequest("Gagal mendapatkan data penjualan.");
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error inserting data into database: {ex.Message}");
            }
        }

        // Get Penjualan
        [HttpGet("penjualan")]
        public IActionResult GetPenjualan()
        {
            List<GetPenjualan> penjualanList = new List<GetPenjualan>();

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();

                using var cmd = new NpgsqlCommand("SELECT penjualan.intSalesOrderID, customer.txtCustomerName, produk.txtProductName, customer.txtCustomerAddress, penjualan.intQty, produk.decPrice as TotalHarga FROM penjualan INNER JOIN customer on penjualan.intcustomerid = customer.intcustomerid INNER JOIN produk on penjualan.intproductid = produk.intproductid;", con);

                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetPenjualan penjualan = new GetPenjualan();
                    penjualan.SalesOrderID = reader.GetInt32(0);
                    penjualan.CustomerName = reader.GetString(1);
                    penjualan.ProdukName = reader.GetString(2);
                    penjualan.AlamatPengiriman = reader.GetString(3);
                    penjualan.Qty = reader.GetDouble(4);
                    penjualan.TotalHarga = reader.GetInt32(5) * (int)penjualan.Qty;
                    penjualanList.Add(penjualan);
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error retrieving data from database: {ex.Message}");
            }

            return Ok(new { data = penjualanList });
        }



    }
}

