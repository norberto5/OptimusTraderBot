using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusTraderBot.Models
{
	public class OrderBookResult
	{
		public string Currency { get; set; }

		public List<OrderBookItem> Bids { get; set; }
		public List<OrderBookItem> Asks { get; set; }

		public string GetString(int bids = 3, int asks = 3)
		{
			var sb = new StringBuilder();

			if(Bids != null && Bids.Count > 0)
			{
				foreach(OrderBookItem bid in Bids.OrderBy(b => b.Rate).TakeLast(3))
				{
					sb.AppendLine($"Bid/Buy: {bid} {Currency} (rate: {bid.Rate})");
				}
			}
			sb.AppendLine(new string('-', 30));
			if(Asks != null && Asks.Count > 0)
			{
				foreach(OrderBookItem ask in Asks.OrderBy(a => a.Rate).Take(3))
				{
					sb.AppendLine($"Ask/Sell: {ask} {Currency} (rate: {ask.Rate})");
				}
			}
			return sb.Remove(sb.Length-2, 2).ToString();
		}
	}

	public class OrderBookItem
	{
		public string Currency { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }

		public decimal Rate => Price / Quantity;

		public override string ToString() => $"{Price.ToString("N2")} {Currency} for {Quantity}";
	}
}
