namespace OptimusTraderBot.Models
{
	public class Money
	{
		public double Available { get; set; }
		public double Locked { get; set; }

		public override string ToString() => $"Available: {Available} - Locked: {Locked}";
	}
}
