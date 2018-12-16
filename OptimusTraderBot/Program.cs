using System;
using Microsoft.Extensions.Configuration;
using OptimusTraderBot.Enums;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			IConfigurationRoot config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var apiSettings = new ApiSettings();
			config.GetSection("ApiSettings").Bind(apiSettings);

			var apiManager = new ApiManager(apiSettings);

			InfoResult info = apiManager.GetInfo();
			Console.WriteLine($"Connection to API is {(info.Success ? "OK" : "NOT OK! Something went wrong!")}");

			TradeResult tradeResult = apiManager.Trade(TradeType.Buy, "LSK", 0.123m, "PLN", 1m);
			long orderId = tradeResult.OrderId;
			if(orderId != 0)
			{
				CancelResult cancelResult = apiManager.CancelOrder(orderId);
			}

			Console.Read();
		}
	}
}
