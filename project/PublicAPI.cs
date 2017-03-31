namespace org.zxteam.apiwrap.poloniex
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Runtime.Serialization.Json;
	using org.zxteam.apiwrap.poloniex._internal;
	using org.zxteam.apiwrap.poloniex.data;

	public class PublicAPI
	{
		private static readonly DateTime UNIXBASETIME = new DateTime(1970, 1, 1);
		private const string DEFAULT_URL = "https://poloniex.com/public";

		private readonly Uri _url;

		public PublicAPI(string url = DEFAULT_URL)
		{
			this._url = new Uri(url);
		}

		public IEnumerable<ITrade> GetTradeHistory(string currencyPair, DateTime from, DateTime to)
		{
			long unixFrom = TranslateToUnixTime(from.ToUniversalTime());
			long unixTo = TranslateToUnixTime(to.ToUniversalTime());

			IDictionary<string, string> args = new Dictionary<string, string>() {
				{ "currencyPair", currencyPair},
				{ "start", unixFrom.ToString() },
				{ "end", unixTo.ToString() }
			};
			string response = this.DownloadJSON("returnTradeHistory", args);

			DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Trade[]));
			object objResponse = jsonSerializer.ReadObject(
				new MemoryStream(System.Text.Encoding.UTF8.GetBytes(response))
				);
			Trade[] friendlyResponse = (Trade[])objResponse;
			if (friendlyResponse.Length >= 50000)
			{
				throw new InvalidOperationException("Response records are overflow. Regarding Poloniex contract: up to 50,000 trades between a range specified in UNIX timestamps by the \"start\" and \"end\". Please use smaller range.");
			}
			return friendlyResponse.AsEnumerable();
		}

		[Pure]
		private string DownloadJSON(string method, IDictionary<string, string> args)
		{
			using (WebClient wc = new WebClient())
			{
				wc.Proxy = this.AutodetectWebProxy();
				wc.QueryString.Add("command", method);
				foreach (var arg in args)
				{
					wc.QueryString.Add(arg.Key, arg.Value);
				}
				string url = this._url.AbsoluteUri;
				var qq = wc.QueryString;
				return wc.DownloadString(url);
			}
		}

		[Pure]
		private WebProxy AutodetectWebProxy()
		{
			if (!WebRequest.DefaultWebProxy.IsBypassed(this._url))
			{
				String resolvedAddress = WebRequest.DefaultWebProxy.GetProxy(this._url).ToString();
				WebProxy wp = new WebProxy();
				ICredentials credentials = CredentialCache.DefaultNetworkCredentials;
				NetworkCredential credential = credentials.GetCredential(this._url, "Basic");
				wp.Credentials = credential;
				wp.Address = new Uri(resolvedAddress + @"proxy.pac");
				return wp;
			}
			return null;
		}

		[Pure]
		private static long TranslateToUnixTime(DateTime date) { return (long)date.Subtract(UNIXBASETIME).TotalSeconds; }
	}
}
