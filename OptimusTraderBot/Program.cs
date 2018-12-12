using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OptimusTraderBot.Connector;
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

			var apiConnector = new ApiConnector(apiSettings);

			var parameters = new Dictionary<string, string>();
			string result = apiConnector.CallApiOperation(ApiMethod.Info, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			InfoResult info = json.ToObject<InfoResult>();

			if(info.Success)
			{
				if(info.Balances.TryGetValue("LSK", out Money lsk))
				{
					Console.WriteLine($"LSK: {lsk}");
				}
				if(info.Addresses.TryGetValue("BTC", out string btcAddress))
				{
					Console.WriteLine(btcAddress);
				}
			}

			Console.Read();
		}
	}
}
