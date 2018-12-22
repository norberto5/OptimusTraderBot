using System.Collections.Generic;
using System.Linq;
using System.Text;
using OptimusTraderBot.Enums;

namespace OptimusTraderBot.Models
{
	public class OrderBookResult
	{
		public string Currency { get; set; }

		public List<OrderBookItem> Bids { get; set; }
		public List<OrderBookItem> Asks { get; set; }

		public string GetString(int bids = 3, int asks = 3) => MakeString(bids, asks);

		public string GetStringWithMarkedOrders(IEnumerable<Order> orders, int bids = 3, int asks = 3) => MakeString(bids, asks, orders);

		private string MakeString(int bids = 3, int asks = 3, IEnumerable<Order> orders = null)
		{
			var sb = new StringBuilder();

			if(Bids != null && Bids.Count > 0)
			{
				foreach(OrderBookItem bid in Bids.OrderBy(b => b.Rate).TakeLast(bids))
				{
					string line = $"Bid/Buy: {bid} {Currency} (rate: {bid.Rate})";
					if(orders != null)
					{
						Order myOrder = orders.FirstOrDefault(o => o.Type == TradeType.Bid && o.OrderCurrency == Currency && o.Rate == bid.Rate);
						if(myOrder != null)
						{
							line = line + $"    <----- {myOrder.CurrentPrice.ToString("N2")} {myOrder.PaymentCurrency} for {myOrder.Units} {myOrder.OrderCurrency} ({myOrder.OrderId})";
						}
					}
					sb.AppendLine(line);
				}
			}
			sb.AppendLine(new string('-', 30));
			if(Asks != null && Asks.Count > 0)
			{
				foreach(OrderBookItem ask in Asks.OrderBy(a => a.Rate).Take(asks))
				{
					string line = $"Ask/Sell: {ask} {Currency} (rate: {ask.Rate})";
					if(orders != null)
					{
						Order myOrder = orders.FirstOrDefault(o => o.Type == TradeType.Ask && o.OrderCurrency == Currency && o.Rate == ask.Rate);
						if(myOrder != null)
						{
							line = line + $"    <----- {myOrder.CurrentPrice.ToString("N2")} {myOrder.PaymentCurrency} for {myOrder.Units} {myOrder.OrderCurrency} ({myOrder.OrderId})";
						}
					}
					sb.AppendLine(line);
				}
			}
			return sb.Remove(sb.Length - 2, 2).ToString();
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
