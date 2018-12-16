using Newtonsoft.Json;

namespace OptimusTraderBot.Models
{
	public class CancelResult
	{
		public bool Success { get; set; }
		[JsonProperty("order_id")]
		public double OrderId { get; set; }
	}
}
