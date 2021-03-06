﻿using System;
using Newtonsoft.Json;
using OptimusTraderBot.Enums;

namespace OptimusTraderBot.Models
{
	public class Order
	{
		[JsonProperty("order_id")]
		public long OrderId { get; set; }
		[JsonProperty("order_currency")]
		public string OrderCurrency { get; set; }
		[JsonProperty("order_date")]
		public DateTime OrderDate { get; set; }
		[JsonProperty("payment_currency")]
		public string PaymentCurrency { get; set; }
		public TradeType Type { get; set; }
		public string Status { get; set; } // TODO: Possible statutes
		public decimal Units { get; set; }
		[JsonProperty("start_units")]
		public decimal StartUnits { get; set; }
		[JsonProperty("current_price")]
		public decimal CurrentPrice { get; set; }
		[JsonProperty("start_price")]
		public decimal StartPrice { get; set; }

		public decimal Rate => CurrentPrice / Units;

		public override string ToString()
		{
			string units = Units != StartUnits
					? $"{Units}/{StartUnits} {OrderCurrency}"
					: $"{Units} {OrderCurrency}";
			string price = CurrentPrice != StartPrice
				? $"{CurrentPrice.ToString("N2")}/{StartPrice.ToString("N2")} {PaymentCurrency}"
				: $"{CurrentPrice.ToString("N2")} {PaymentCurrency}";

			return $"{Type} - {units} for {price} (rate: {Rate}) ({OrderDate.ToString(Program.DateCulture)}) ({OrderId})";
		}
	}
}
