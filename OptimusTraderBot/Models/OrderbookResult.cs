using System.Collections.Generic;

namespace OptimusTraderBot.Models
{
	public class OrderBookResult
	{
		public List<OrderBookItem> Bids { get; set; }
		public List<OrderBookItem> Asks { get; set; }
	}

	public class OrderBookItem
	{
		public string Currency { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }

		public decimal Rate => Price / Quantity;

		public override string ToString() => $"{Price} {Currency} for {Quantity}";
	}
}
