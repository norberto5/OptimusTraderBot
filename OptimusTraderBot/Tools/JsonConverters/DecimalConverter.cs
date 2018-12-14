using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OptimusTraderBot.Tools.JsonConverters
{
	public class DecimalConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(decimal) || objectType == typeof(decimal?);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var token = JToken.Load(reader);
			if(token.Type == JTokenType.Boolean)
			{
				return bool.Parse(token.ToString()) ? 1m : 0m;
			}
			else if(token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
			{
				return token.ToObject<decimal>();
			}
			else if(token.Type == JTokenType.String)
			{
				// customize this to suit your needs
				return decimal.Parse(token.ToString());
			}
			else if(token.Type == JTokenType.Null && objectType == typeof(decimal?))
			{
				return null;
			}
			throw new JsonSerializationException($"Unexpected token type: {token.Type.ToString()}");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
	}
}
