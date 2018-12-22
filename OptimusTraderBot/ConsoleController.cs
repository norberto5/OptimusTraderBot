using System;
using System.Collections.Generic;
using System.Linq;
using OptimusTraderBot.Enums;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	public class ConsoleController
	{
		private readonly ApiManager apiManager;
		private readonly UserSettings userSettings;

		public ConsoleController(ApiManager apiManager, UserSettings userSettings)
		{
			this.apiManager = apiManager;
			this.userSettings = userSettings;
		}

		public void ParseCommand(string cmdToParse)
		{
			string[] parameters = cmdToParse.Split(' ');
			string cmd = parameters[0];

			switch(cmd)
			{
				case "info":
					InfoCommand();
					break;
				case "trade":
					TradeCommand(parameters);
					break;
				case "cancel":
					CancelCommand(parameters);
					break;
				case "orderbook":
					OrderbookCommand(parameters);
					break;
				case "orders":
					OrdersCommand(parameters);
					break;
				case "history":
					HistoryCommand(parameters);
					break;
			}
		}

		private void InfoCommand()
		{
			InfoResult info = apiManager.GetInfo();
			Console.WriteLine(new string('-', 30));
			if(info.Success)
			{
				IEnumerable<KeyValuePair<string, Money>> balancesNotZero = info.Balances.Where(b => b.Value.Available != 0 || b.Value.Locked != 0);
				if(balancesNotZero.Count() == 0)
				{
					Console.WriteLine("Your balances are empty!");
				}
				else
				{
					foreach(KeyValuePair<string, Money> balance in balancesNotZero)
					{
						Console.WriteLine($"{balance.Key} : {balance.Value}");
					}
				}
				Console.WriteLine(new string('-', 30));
				Console.WriteLine($"Fee: {info.Fee}");
			}
			else
			{
				Console.WriteLine("Command failed!");
			}
		}

		private void TradeCommand(string[] parameters)
		{
			if(parameters.Length != 5 && parameters.Length != 4)
			{
				Console.WriteLine($"Wrong parameters! Use: [{TradeType.Ask}/{TradeType.Bid}] [Rate] [Currency] (Amount (or max))");
				return;
			}
			if(!Enum.TryParse(parameters[1], true, out TradeType tradeType))
			{
				Console.WriteLine($"Wrong trade type! Use '{TradeType.Ask}' or '{TradeType.Bid}'");
				return;
			}
			if(!decimal.TryParse(parameters[2].Replace(',', '.'), out decimal rate))
			{
				Console.WriteLine($"Wrong Rate parameter! Use number with dot(i.e. 5.13)!");
				return;
			}
			if(parameters[3].Length != 3 && parameters[3].Length != 4 && !parameters[3].All(c => char.IsLetter(c)))
			{
				Console.WriteLine($"Wrong currency symbol! Use good currency symbol (i.e. LSK)!");
				return;
			}
			string currency = parameters[3].ToUpper();

			decimal amount = 0;
			if(parameters.Length == 5)
			{
				if(!decimal.TryParse(parameters[4].Replace(',', '.'), out amount))
				{
					Console.WriteLine($"Wrong amount parameter! Use number with dot(i.e. 157.53) or empty for max value!");
					return;
				}
			}
			else
			{
				InfoResult info = apiManager.GetInfo();
				if(!info.Success)
				{
					Console.WriteLine("Couldn't get actual amount of currency");
					return;
				}
				else
				{
					if(tradeType == TradeType.Ask)
					{
						amount = info.Balances[currency].Available;
					}
					else if(tradeType == TradeType.Bid)
					{
						amount = info.Balances[userSettings.PaymentCurrency].Available / rate;
					}
				}
			}

			apiManager.Trade(tradeType, currency, amount, userSettings.PaymentCurrency, rate);
		}

		private void CancelCommand(string[] parameters)
		{
			long? orderIdToCancel = null;

			List<Order> orders = apiManager.GetOrders();
			if(parameters.Length == 1)
			{
				orderIdToCancel = orders.LastOrDefault()?.OrderId;
				if(orderIdToCancel == null)
				{
					Console.WriteLine("No order to cancel");
				}
			}
			else if(parameters.Length == 2)
			{
				if(long.TryParse(parameters[1], out long parsedOrderId))
				{
					IEnumerable<Order> matchingOrders = orders.Where(o => o.OrderId.ToString().Contains(parameters[1]));
					if(matchingOrders.Count() == 1)
					{
						orderIdToCancel = matchingOrders.First().OrderId;
					}
					else if(matchingOrders.Count() > 1)
					{
						Console.WriteLine("More matching orders:");
						foreach(Order order in matchingOrders)
						{
							Console.WriteLine(order);
						}
					}
					else
					{
						Console.WriteLine("No orders match the order id pattern");
					}
				}
			}

			if(orderIdToCancel != null)
			{
				apiManager.CancelOrder(orderIdToCancel.Value);
			}
		}

		private void OrderbookCommand(string[] parameters)
		{
			if(parameters.Length != 2)
			{
				Console.WriteLine("Wrong number of parameters! Use: orderbook [currency]");
				return;
			}

			OrderBookResult result =apiManager.GetOrderBook(parameters[1].ToUpper(), userSettings.PaymentCurrency);
			Console.WriteLine(result.GetString());
		}

		private void OrdersCommand(string[] parameters)
		{
			Console.WriteLine("Actual orders:");
			List<Order> orders = apiManager.GetOrders();
			foreach(Order order in orders)
				Console.WriteLine(order);
		}

		private void HistoryCommand(string[] parameters)
		{
			if(parameters.Length != 2)
			{
				Console.WriteLine("Wrong number of parameters! Use: history [currency]");
				return;
			}

			List<HistoryEntry> history = apiManager.GetHistory(parameters[1].ToUpper(), 10);

			foreach(HistoryEntry entry in history)
			{
				Console.WriteLine(entry);
			}
		}
	}
}
