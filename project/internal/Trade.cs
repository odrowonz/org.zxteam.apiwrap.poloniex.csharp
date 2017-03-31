using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Globalization;

namespace org.zxteam.apiwrap.poloniex._internal
{
	//	{
	//"globalTradeID":2036467,
	//"tradeID":21387,
	//"date":"2014-09-12 05:21:26",
	//"type":"buy",
	//"rate":"0.00008943",
	//"amount":"1.27241180",
	//"total":"0.00011379"
	//}
	[DataContract]
	internal class Trade : data.ITrade
	{
		private const string DatePattern = "yyyy-MM-dd HH:mm:ss";

		[DataMember]
		private long globalTradeID;

		[DataMember]
		private long tradeID;

		[DataMember]
		private string date;

		[DataMember]
		private string type;

		[DataMember]
		private decimal rate;

		[DataMember]
		private decimal amount;

		[DataMember]
		private decimal total;

#if DEBUG
		public override string ToString()
		{
			return string.Format("Type:{0} Rate:{1} Amount:{2}", type, rate, amount);
		}
#endif

		long data.ITrade.GlobalTradeID { get { return this.globalTradeID; } }
		long data.ITrade.TradeID { get { return this.tradeID; } }
		DateTime data.ITrade.UtcDate { get { return DateTime.ParseExact(this.date, DatePattern, CultureInfo.InvariantCulture, DateTimeStyles.None); } }
		data.TradeType data.ITrade.Type
		{
			get
			{
				switch (this.type)
				{
					case "buy": return data.TradeType.BUY;
					case "sell": return data.TradeType.SELL;
					default: throw new NotSupportedException(this.type);
				}
			}
		}
		decimal data.ITrade.Rate { get { return this.rate; } }
		decimal data.ITrade.Amount { get { return this.amount; } }
		decimal data.ITrade.Total { get { return this.total; } }
	}
}
