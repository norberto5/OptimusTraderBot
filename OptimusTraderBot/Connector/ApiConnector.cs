using System.Collections.Generic;
using System.Threading.Tasks;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot.Connector
{
	public class ApiConnector : IApiConnector
	{
		private readonly ApiSettings apiSettings;

		public ApiConnector(ApiSettings apiSettings)
		{
			this.apiSettings = apiSettings;
		}

		public async Task<string> CallApiOperation(ApiMethod method, Dictionary<string, string> parameters)
		{
			switch(method)
			{
				case ApiMethod.Info:
					break;
			}

			return "";
		}
	}
}
