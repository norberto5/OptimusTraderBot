using System;
using Newtonsoft.Json;

namespace OptimusTraderBot.Models
{
	public class HistoryEntry
	{
		public long Id { get; set; }
		public decimal Amount { get; set; }
		[JsonProperty("balance_after")]
		public decimal BalanceAfter { get; set; }
		public string Currency { get; set; }
		[JsonProperty("operation_type")]
		public string OperationType { get; set; }
		public DateTime Time { get; set; }
		public string Comment { get; set; }

		public override string ToString() => $"{OperationType} : (amount: {Amount} {Currency}) (balance after: {BalanceAfter} {Currency}) ({Time.ToString(Program.DateCulture)})";
	}
}
