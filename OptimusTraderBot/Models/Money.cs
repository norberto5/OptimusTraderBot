namespace OptimusTraderBot.Models
{
	public class Money
	{
		public decimal Available { get; set; }
		public decimal Locked { get; set; }

		public override string ToString() => $"Available: {Available} - Locked: {Locked}";
	}
}
