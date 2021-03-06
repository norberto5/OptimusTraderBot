﻿using System.Collections.Generic;
using System.Threading.Tasks;
using OptimusTraderBot.Enums;

namespace OptimusTraderBot.Connector
{
	public interface IApiConnector
	{
		Task<string> CallApiOperation(ApiMethod method, Dictionary<string, string> parameters = null);
	}
}
