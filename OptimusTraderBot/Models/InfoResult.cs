using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusTraderBot.Models
{
	public class InfoResult
	{
		public bool Success { get; set; }
		public Dictionary<string, Money> Balances { get; set; }
		public Dictionary<string, string> Addresses { get; set; }
		public double Fee { get; set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			IEnumerable<KeyValuePair<string, Money>> balancesNotZero = Balances.Where(b => b.Value.Available != 0 || b.Value.Locked != 0);
			if(balancesNotZero.Count() == 0)
			{
				sb.AppendLine("Your balances are empty!");
			}
			else
			{
				foreach(KeyValuePair<string, Money> balance in balancesNotZero)
				{
					sb.AppendLine($"{balance.Key} : {balance.Value}");
				}
			}
			sb.AppendLine(new string('-', 30));
			sb.AppendLine($"Fee: {Fee}");

			return sb.ToString();
		}
	}
}
