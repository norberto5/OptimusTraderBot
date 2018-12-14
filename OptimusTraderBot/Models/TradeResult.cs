using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OptimusTraderBot.Tools.JsonConverters;

namespace OptimusTraderBot.Models
{
	public class TradeResult
	{
		public bool Success { get; set; }
		[JsonProperty("order_id")]
		public long OrderId { get; set; }
		public decimal? Amount { get; set; }
		[JsonConverter(typeof(DecimalConverter))]
		public decimal Rate { get; set; }
		[JsonConverter(typeof(DecimalConverter))]
		public decimal Price { get; set; }
		public decimal Fee { get; set; }
		[JsonProperty("fee_currency")]
		public string FeeCurrency { get; set; }
		//public Dictionary<string, string> Wrong { get; set; }
		public List<TradeInfo> Bought { get; set; }
	}

	public class TradeInfo
	{
		public decimal Count { get; set; }
		public decimal Price { get; set; }
		public decimal Rate { get; set; }
		public decimal Active { get; set; } // ?
		[JsonProperty("cc_id")]
		public decimal CcId { get; set; } // ?
		[JsonProperty("price_currency")]
		public decimal PriceCurrency { get; set; }
		[JsonProperty("price_is_crypto")]
		public bool PriceIsCrypto { get; set; }
		public DateTime Changed { get; set; }
	}
}
