﻿using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

			IConfigurationRoot config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var apiSettings = new ApiSettings();
			config.GetSection(nameof(ApiSettings)).Bind(apiSettings);
			var userSettings = new UserSettings();
			config.GetSection(nameof(UserSettings)).Bind(userSettings);

			var apiManager = new ApiManager(apiSettings);

			InfoResult info = apiManager.GetInfo();
			Console.WriteLine($"Connection to API is {(info.Success ? "OK" : "NOT OK! Something went wrong!")}");

			//TradeResult tradeResult = apiManager.Trade(TradeType.Buy, "LSK", 0.123m, "PLN", 1m);
			//long orderId = tradeResult.OrderId;
			//if(orderId != 0)
			//{
			//	CancelResult cancelResult = apiManager.CancelOrder(orderId);
			//}

			Console.WriteLine("Actual orders:");
			apiManager.GetOrders();
			Console.WriteLine(new string('-', 30));
			Console.WriteLine("Actual OrderBook for LSK/PLN:");
			apiManager.GetOrderBook("LSK", "PLN");
			Console.WriteLine(new string('-', 30));
			//apiManager.GetTransactions();

			var consoleController = new ConsoleController(apiManager, userSettings);
			string cmd = string.Empty;
			while(cmd != "exit")
			{
				cmd = Console.ReadLine();
				consoleController.ParseCommand(cmd);
			}

			Console.Read();
		}
	}
}
