using System;

namespace gLhb.Parser
{
	public class LhbInfo
	{
		public int Week { get; set; }
		public DateTime Date { get; set; }

		public override string ToString()
		{
			return $"{Date:dd/MM/yyyy} #{Week}";
		}
	}
}