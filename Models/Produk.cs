using System;
namespace kalbestore_be.Models
{
	public class Produk
	{
		public int ProductID { get; set; }
        public string ProductCode { get; set; }
		public string ProductName { get; set; }
        public int Qty { get; set; }
		public decimal Price { get; set; }
		public DateTime Inserted { get; set; }
    }

    public class AddProdukRequest
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }

}

