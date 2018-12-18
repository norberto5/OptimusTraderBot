using System;
using System.Collections.Generic;
using System.Linq;
using OptimusTraderBot.Models;

namespace OptimusTraderBot
{
	public class ConsoleController
	{
		private readonly ApiManager apiManager;

		public ConsoleController(ApiManager apiManager)
		{
			this.apiManager = apiManager;
		}

		public void ParseCommand(string cmdToParse)
		{
			string[] blocks = cmdToParse.Split(' ');
			string cmd = blocks[0];

			switch(cmd)
			{
				case "info":
					InfoCommand();
					break;
			}
		}

		private void InfoCommand()
		{
			InfoResult info = apiManager.GetInfo();
			Console.WriteLine(new string('-', 30));
			if(info.Success)
			{
				IEnumerable<KeyValuePair<string, Money>> balancesNotZero = info.Balances.Where(b => b.Value.Available != 0 || b.Value.Locked != 0);
				if(balancesNotZero.Count() == 0)
				{
					Console.WriteLine("Your balances are empty!");
				}
				else
				{
					foreach(KeyValuePair<string, Money> balance in balancesNotZero)
					{
						Console.WriteLine($"{balance.Key} : {balance.Value}");
					}
				}
				Console.WriteLine(new string('-', 30));
				Console.WriteLine($"Fee: {info.Fee}");
			}
			else
			{
				Console.WriteLine("Command failed!");
			}
		}
	}
}
