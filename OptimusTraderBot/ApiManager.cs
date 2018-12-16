using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OptimusTraderBot.Connector;
using OptimusTraderBot.Enums;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	public class ApiManager
	{
		private readonly IApiConnector apiConnector;

		public ApiManager(ApiSettings apiSettings)
		{
			apiConnector = new ApiConnector(apiSettings);
		}

		public InfoResult GetInfo()
		{
			string result = apiConnector.CallApiOperation(ApiMethod.Info).Result;
			var json = JToken.Parse(result);
			//Console.WriteLine(json.ToString(Formatting.Indented));
			InfoResult info = json.ToObject<InfoResult>();
			return info;
		}

		public TradeResult Trade(TradeType tradeType, string currency, decimal amount, string paymentCurrency, decimal rate)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "type", tradeType.ToString().ToLower() },
				{ "currency", currency },
				{ "amount", amount.ToString() },
				{ "payment_currency", paymentCurrency },
				{ "rate", rate.ToString() }
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Trade, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			TradeResult tradeResult = json.ToObject<TradeResult>();

			if(tradeResult.Success)
			{
				//TODO: Upgrade message if Bought is not empty etc.
				Console.WriteLine($"Successfully placed a trade of type {tradeType} of {amount} {currency} for {tradeResult.Price} {paymentCurrency} (rate: {tradeResult.Rate})");
				if(tradeResult.OrderId != 0)
				{
					Console.WriteLine($"Order id: {tradeResult.OrderId}");
				}
			}
			else
			{
				Console.WriteLine($"Failed to place a trade of type {tradeType} of {amount} {currency}");
			}

			return tradeResult;
		}

		public CancelResult CancelOrder(long orderId)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "id", orderId.ToString() }
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Cancel, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			CancelResult cancelResult = json.ToObject<CancelResult>();

			if(cancelResult.Success)
			{
				Console.WriteLine($"Successfully cancelled an order of id {cancelResult.OrderId}");
			}
			else
			{
				Console.WriteLine($"Failed to cancel an order of id {cancelResult.OrderId}");
			}
			return cancelResult;
		}

		public OrderBookResult GetOrderBook(string orderCurrency, string paymentCurrency)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "order_currency", orderCurrency },
				{ "payment_currency", paymentCurrency }
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Orderbook, parameters).Result;
			var json = JToken.Parse(result);
			//Console.WriteLine(json.ToString(Formatting.Indented));
			OrderBookResult orderbookResult = json.ToObject<OrderBookResult>();

			if(orderbookResult.Bids != null && orderbookResult.Bids.Count > 0)
			{
				foreach(OrderBookItem bid in orderbookResult.Bids.OrderBy(b => b.Rate).TakeLast(10))
				{
					Console.WriteLine($"Bid/Buy: {bid} {orderCurrency} (rate: {bid.Rate})");
				}
			}
			Console.WriteLine(new string('-', 30));
			if(orderbookResult.Asks != null && orderbookResult.Asks.Count > 0)
			{
				foreach(OrderBookItem ask in orderbookResult.Asks.OrderBy(a => a.Rate).Take(10))
				{
					Console.WriteLine($"Ask/Sell: {ask} {orderCurrency} (rate: {ask.Rate})");
				}
			}
			return orderbookResult;
		}

		public List<Order> GetOrders(int limit = 50)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "limit", limit.ToString() },
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Orders, parameters).Result;
			var json = JToken.Parse(result);
			//Console.WriteLine(json.ToString(Formatting.Indented));
			List<Order> orders = json.ToObject<List<Order>>();

			foreach(Order order in orders)
			{
				string units = order.Units != order.StartUnits
					? $"{order.Units}/{order.StartUnits} {order.OrderCurrency}"
					: $"{order.Units} {order.OrderCurrency}";
				string price = order.CurrentPrice != order.StartPrice
					? $"{order.CurrentPrice}/{order.StartPrice} {order.PaymentCurrency}"
					: $"{order.CurrentPrice} {order.PaymentCurrency}";

				Console.WriteLine($"{order.Type} - {units} for {price} ({order.OrderDate.ToString(CultureInfo.GetCultureInfo("PL"))})");
			}

			return orders;
		}
	}
}
