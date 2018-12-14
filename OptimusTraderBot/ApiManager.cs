using System;
using System.Collections.Generic;
using System.Globalization;
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
		private static readonly NumberFormatInfo numberFormat = NumberFormatInfo.InvariantInfo;
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
				{ "amount", amount.ToString(numberFormat) },
				{ "payment_currency", paymentCurrency },
				{ "rate", rate.ToString(numberFormat) }
			};

			string result = apiConnector.CallApiOperation(ApiMethod.Trade, parameters).Result;
			var json = JToken.Parse(result);
			Console.WriteLine(json.ToString(Formatting.Indented));
			TradeResult tradeResult = json.ToObject<TradeResult>();

			if(tradeResult.Success)
			{
				Console.WriteLine($"Successfully placed a trade of type {tradeType} of {amount.ToString(numberFormat)} {currency} for {tradeResult.Price.ToString(numberFormat)} {paymentCurrency} (rate: {tradeResult.Rate.ToString(numberFormat)})");
				if(tradeResult.OrderId != 0)
				{
					Console.WriteLine($"Order id: {tradeResult.OrderId}");
				}
			}
			else
			{
				Console.WriteLine($"Failed to place a trade of type {tradeType} of {amount.ToString(numberFormat)} {currency}");
			}

			return tradeResult;
		}
	}
}
