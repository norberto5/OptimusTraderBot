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
				{ "amount", amount.ToString("N8") },
				{ "payment_currency", paymentCurrency },
				{ "rate", rate.ToString("N2") }
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Trade, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			TradeResult tradeResult = json.ToObject<TradeResult>();

			if(tradeResult.Success)
			{
				//TODO: Upgrade message if Bought is not empty etc.
				Console.WriteLine($"Successfully placed a trade of type {tradeType} of {amount.ToString("N8")} {currency} for {tradeResult.Price.ToString("N2")} {paymentCurrency} (rate: {tradeResult.Rate.ToString("N2")})");
				if(tradeResult.OrderId != 0)
				{
					Console.WriteLine($"Order id: {tradeResult.OrderId}");
				}
			}
			else
			{
				Console.WriteLine($"Failed to place a trade of type {tradeType} of {amount.ToString("N8")} {currency}");
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
			//Console.WriteLine(json.ToString(Formatting.Indented));
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
			orderbookResult.Currency = orderCurrency;

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

			return orders;
		}

		public TransferResult Transfer(string currency, decimal quantity, string address)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "currency", currency },
				{ "quantity", quantity.ToString("N8") },
				{ "address", address },
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Transfer, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			TransferResult transferResult = json.ToObject<TransferResult>();

			return transferResult;
		}

		public WithdrawResult Withdraw(string currency, decimal quantity, long accountNumber, bool express, string swiftBicNumber)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "currency", currency },
				{ "quantity", quantity.ToString("N8") },
				{ "account", accountNumber.ToString() },
				{ "express", express.ToString().ToLower() },
				{ "bic", swiftBicNumber },
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Withdraw, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			WithdrawResult withdrawResult = json.ToObject<WithdrawResult>();

			return withdrawResult;
		}

		public List<HistoryEntry> GetHistory(string currency, int limit = 50)
		{
			var parameters = new Dictionary<string, string>()
			{
				{ "currency", currency },
				{ "limit", limit.ToString() },
			};

			string result = apiConnector.CallApiOperation(ApiMethod.History, parameters).Result;
			var json = JToken.Parse(result);
			//Console.WriteLine(json.ToString(Formatting.Indented));
			List<HistoryEntry> historyResult = json.ToObject<List<HistoryEntry>>();

			return historyResult;
		}

		public List<Transaction> GetTransactions(string cryptoCurrency = null, string priceCurrency = null)
		{
			var parameters = new Dictionary<string, string>();
			if(!string.IsNullOrEmpty(cryptoCurrency) && !string.IsNullOrEmpty(priceCurrency))
			{
				parameters.Add("market", $"{cryptoCurrency}-{priceCurrency}");
			}

			string result = apiConnector.CallApiOperation(ApiMethod.Transactions, parameters).Result;
			var json = JToken.Parse(result);
			//Console.WriteLine(json.ToString(Formatting.Indented));
			List<Transaction> transactions = json.ToObject<List<Transaction>>();

			foreach(Transaction transaction in transactions.Take(10).Reverse())
			{
				Console.WriteLine($"{transaction.Market} {transaction.Type}: {transaction.Amount} {transaction.CryptoCurrency} " +
					$"for {transaction.Price} {transaction.PriceCurrency} (rate: {transaction.Rate}) ({transaction.Date.ToString(Program.DateCulture)})");
			}

			return transactions;
		}
	}
}