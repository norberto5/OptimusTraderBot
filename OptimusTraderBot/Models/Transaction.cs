using System;
using OptimusTraderBot.Enums;

namespace OptimusTraderBot.Models
{
	public class Transaction
	{
		public DateTime Date { get; set; }
		public TradeType Type { get; set; }
		public string Market { get; set; }
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Price { get; set; }

		public string CryptoCurrency => Market.Substring(0, 3);
		public string PriceCurrency => Market.Substring(4, 3);

		public override string ToString() => $"{Market} {Type}: {Amount} {CryptoCurrency} for {Price.ToString("N2")} {PriceCurrency} (rate: {Rate}) ({Date.ToString(Program.DateCulture)})";
	}
}
