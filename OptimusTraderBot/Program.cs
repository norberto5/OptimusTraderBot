using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	internal class Program
	{
		public static readonly CultureInfo DateCulture = CultureInfo.GetCultureInfo("PL");

		private static ApiManager apiManager;

		private static void Main(string[] args)
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

			IConfigurationRoot config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var apiSettings = new ApiSettings();
			config.GetSection(nameof(ApiSettings)).Bind(apiSettings);
			var userSettings = new UserSettings();
			config.GetSection(nameof(UserSettings)).Bind(userSettings);

			apiManager = new ApiManager(apiSettings);

			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
			backgroundWorker.WorkerSupportsCancellation = true;

			WelcomeMessage();

			var consoleController = new ConsoleController(apiManager, userSettings);
			string cmd = string.Empty;
			while(cmd != "exit")
			{
				if(Console.KeyAvailable)
				{
					if(backgroundWorker.IsBusy)
					{
						backgroundWorker.CancelAsync();
						Console.WriteLine("Auto update cancelled");
					}

					cmd = Console.ReadLine();

					if(cmd == "auto" || string.IsNullOrEmpty(cmd))
					{
						if(!backgroundWorker.IsBusy)
							backgroundWorker.RunWorkerAsync();
					}
					else
					{
						consoleController.ParseCommand(cmd);
					}
				}

			}

			Console.Read();
		}

		private static void WelcomeMessage()
		{
			InfoResult info = apiManager.GetInfo();
			Console.WriteLine($"Connection to API is {(info.Success ? "OK" : "NOT OK! Something went wrong!")}");

			Console.WriteLine("Actual orders:");
			List<Order> orders = apiManager.GetOrders();
			foreach(Order order in orders)
				Console.WriteLine(order);

			Console.WriteLine(new string('-', 30));

			Console.WriteLine("Actual OrderBook for LSK/PLN:");
			OrderBookResult orderbookResult = apiManager.GetOrderBook("LSK", "PLN");
			Console.WriteLine(orderbookResult.GetStringWithMarkedOrders(orders, 5, 5));

			Console.WriteLine(new string('-', 30));
		}

		private static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			List<Order> orders = apiManager.GetOrders();
			while(true)
			{
				var worker = sender as BackgroundWorker;
				if(worker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}

				var sb = new StringBuilder();
				sb.AppendLine($"------- AUTO UPDATE ------- ({DateTime.Now.ToString(DateCulture)})");
				sb.AppendLine("Actual orders:");
				List<Order> newOrders = apiManager.GetOrders();
				foreach(Order order in newOrders)
					sb.AppendLine(order.ToString());

				sb.AppendLine(new string('-', 30));

				sb.AppendLine("Actual OrderBook for LSK/PLN:");
				OrderBookResult orderbookResult = apiManager.GetOrderBook("LSK", "PLN");
				sb.AppendLine(orderbookResult.GetStringWithMarkedOrders(newOrders, 5, 5));

				Console.Clear();
				Console.WriteLine(sb);

				if(orders.Count != newOrders.Count)
				{
					orders = newOrders;
					Console.Beep(500, 500);
					Console.Beep(500, 500);
					Console.Beep(500, 500);
					Console.WriteLine("Orders changed!");
					break;
				}

				for(int i = 0; i < 100; i++)
				{
					if(worker.CancellationPending)
					{
						e.Cancel = true;
						return;
					}
					Thread.Sleep(100);
				}
			}
		}
	}
}
