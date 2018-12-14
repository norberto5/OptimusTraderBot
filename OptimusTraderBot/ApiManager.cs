using Newtonsoft.Json.Linq;
using OptimusTraderBot.Connector;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	public class ApiManager
	{
		private readonly IApiConnector apiConnector;

		public ApiManager(ApiSettings apiSettings)
		{
			apiConnector = new ApiConnector(apiSettings);
		}

		public InfoResult GetInfo()
		{
			string result = apiConnector.CallApiOperation(ApiMethod.Info).Result;
			var json = JToken.Parse(result);
			//Console.WriteLine(json.ToString(Formatting.Indented));
			InfoResult info = json.ToObject<InfoResult>();
			return info;
		}
	}
}
