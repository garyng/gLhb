namespace gLhb.Parser
{
	public class RankInfo
	{
		public string ThisWeekRank { get; set; }
		public string LastWeekRank { get; set; }

		public override string ToString()
		{
			return $"{ThisWeekRank}\t{LastWeekRank}";
		}
	}
}