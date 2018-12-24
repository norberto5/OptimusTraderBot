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

			Console.WriteLine(GetMonitoringMessage());
		}

		private static string GetMonitoringMessage()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Actual orders:");
			List<Order> orders = apiManager.GetOrders();
			foreach(Order order in orders)
				sb.AppendLine(order.ToString());

			sb.AppendLine(new string('-', 30));

			sb.AppendLine("Actual OrderBook for LSK/PLN:");
			OrderBookResult orderbookResult = apiManager.GetOrderBook("LSK", "PLN");
			sb.AppendLine(orderbookResult.GetStringWithMarkedOrders(orders, 5, 5));

			sb.AppendLine(new string('-', 30));

			sb.Append(apiManager.GetInfo());

			return sb.ToString();
		}

		private static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			while(true)
			{
				var worker = sender as BackgroundWorker;
				string msg = GetMonitoringMessage();
				for(int i = 0; i < 104; i++)
				{
					if(worker.CancellationPending)
					{
						e.Cancel = true;
						return;
					}
					if(i % 5 == 0)
					{
						Console.Clear();
						Console.WriteLine($"{new string('-', Math.Min(i / 5, 10))}{new string('.', Math.Max(0, Math.Min(10 - i / 5, 10)))}" +
							$"AUTOUPDATE" +
							$"{(i < 50 ? new string('.', 10) : new string('-', Math.Min((i-50) / 5, 10)) + new string('.', Math.Min(10 - (i - 50) / 5, 10)))}");
						Console.WriteLine(msg);
					}
					Thread.Sleep(100);
				}
			}
		}
	}
}
