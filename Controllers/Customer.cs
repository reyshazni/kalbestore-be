using System;
using kalbestore_be.Models;
using kalbestore_be.utils;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace kalbestore_be.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly string _connectionString = dbconfig.ConnectionString;

        // Add Customer
        [HttpPost("customer")]
        public IActionResult AddCustomer([FromBody] AddCustomerRequest customer)
        {
            int customerId;
            DateTime inserted = DateTime.Now;

            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                using var cmd = new NpgsqlCommand("INSERT INTO customer (txtCustomerName, txtCustomerAddress, bitGender, dtmBirthDate, dtInserted) VALUES (@CustomerName, @CustomerAddress, @Gender, @BirthDate, @Inserted) RETURNING intCustomerID", con);
                cmd.Parameters.AddWithValue("CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("CustomerAddress", customer.CustomerAddress);
                cmd.Parameters.AddWithValue("Gender", customer.Gender);
                cmd.Parameters.AddWithValue("BirthDate", customer.BirthDate);
                cmd.Parameters.AddWithValue("Inserted", inserted);

                customerId = (int)cmd.ExecuteScalar();
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error inserting data into database: {ex.Message}");
            }

            return Ok(new { message = "Berhasil menambahkan customer baru!", id = customerId, insertedDate = inserted, data = customer });
        }

        // Get Customer by Name
        [HttpGet("customer/{customerName}")]
        public IActionResult GetCustomerByName(string customerName)
        {
            var customers = new List<Customer>();
            try
            {
                using var con = new NpgsqlConnection(_connectionString);
                con.Open();
                using var cmd = new NpgsqlCommand("SELECT * FROM customer WHERE txtCustomerName ILIKE @CustomerName LIMIT 1", con);
                cmd.Parameters.AddWithValue("CustomerName", "%" + customerName + "%");

                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var customer = new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        CustomerName = reader.GetString(1),
                        CustomerAddress = reader.GetString(2),
                        Gender = reader.GetBoolean(3),
                        BirthDate = reader.GetDateTime(4),
                        Inserted = reader.GetDateTime(5)
                    };
                    customers.Add(customer);
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                return StatusCode(500, $"Error retrieving data from database: {ex.Message}");
            }

            if (customers.Count == 0)
            {
                return NotFound($"Customer bernama {customerName} tidak ditemukan!");
            }

            return Ok(new { data = customers });
        }

    }
}
