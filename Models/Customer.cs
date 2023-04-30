using System;
namespace kalbestore_be.Models
{
	public class Customer
	{
		public int CustomerID { get; set; }
		public string CustomerName { get; set; }
		public string CustomerAddress { get; set; }
		public bool Gender { get; set; }
		public DateTime BirthDate { get; set; }
		public DateTime Inserted { get; set; }
	}

    public class AddCustomerRequest
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
