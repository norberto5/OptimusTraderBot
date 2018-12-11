using System.Collections.Generic;
using System.Threading.Tasks;
using OptimusTraderBot.Settings;
using System.Net.Http;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using OptimusTraderBot.Tools;

namespace OptimusTraderBot.Connector
{
	public class ApiConnector : IApiConnector
	{
		private const string apiUrl = "https://bitbay.net/API/Trading/tradingApi.php";
		private readonly ApiSettings apiSettings;

		private readonly HttpClient client = new HttpClient();

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
				default:
					return null;
			}

			var postValues = new Dictionary<string, string>
			{
				{ "method", method.ToString().ToLower() },
				{ "moment" , DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
			};
			foreach(KeyValuePair<string, string> element in parameters)
			{
				postValues.Add(element.Key, element.Value);
			}

			string hash = GetHash(postValues);

			var content = new FormUrlEncodedContent(postValues);
			content.Headers.Add("API-Key", apiSettings.PublicApiKey);
			content.Headers.Add("API-Hash", hash);

			HttpResponseMessage response = await client.PostAsync(apiUrl, content);

			if(response.StatusCode >= (HttpStatusCode)400)
			{
				return response.StatusCode.ToString();
			}

			return await response.Content.ReadAsStringAsync();
		}

		private string GetHash(Dictionary<string, string> postValues)
		{
			string post = string.Join('&', postValues.Select(x => $"{x.Key}={x.Value}"));
			byte[] key = Encoding.ASCII.GetBytes(apiSettings.PrivateApiKey);
			using(var sha512 = new HMACSHA512(key))
			{
				return sha512.ComputeHash(Encoding.ASCII.GetBytes(post)).ToHexString();
			}
		}

		
	}
}
