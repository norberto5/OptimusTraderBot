using System.Collections.Generic;

namespace OptimusTraderBot.Models
{
	public class InfoResult
	{
		public bool Success { get; set; }
		public Dictionary<string, Money> Balances { get; set; }
		public Dictionary<string, string> Addresses { get; set; }
		public double Fee { get; set; }
	}
}
