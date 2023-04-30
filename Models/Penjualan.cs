using System;
namespace kalbestore_be.Models
{
	public class Penjualan
	{
		public int SalesOrderID { get; set; }
		public int CustomerID { get; set; }
		public int ProductID { get; set; }
		public DateTime SalesOrder { get; set; }
		public double Qty { get; set; }
	}

	public class AddPenjualanRequest
	{
		public int CustomerID { get; set; }
		public int ProductID { get; set; }
		public double Qty { get; set; }
	}

	public class GetPenjualan
	{
        public int SalesOrderID { get; set; }
        public string CustomerName { get; set; }
		public string ProdukName { get; set; }
		public string AlamatPengiriman { get; set; }
        public double Qty { get; set; }
		public int TotalHarga { get; set; }
	}
}
