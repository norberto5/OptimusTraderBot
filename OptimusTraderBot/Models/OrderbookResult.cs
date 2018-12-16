using System.Collections.Generic;

namespace OptimusTraderBot.Models
{
	public class OrderBookResult
	{
		public List<Order> Bids { get; set; }
		public List<Order> Asks { get; set; }
	}

	public class Order
	{
		public string Currency { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }

		public decimal Rate => Price / Quantity;

		public override string ToString() => $"{Price} {Currency} for {Quantity}";
	}
}
