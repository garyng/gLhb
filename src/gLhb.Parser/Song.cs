namespace gLhb.Parser
{
	public class Song
	{
		public RankInfo Rank { get; set; }
		public SongInfo Info { get; set; }

		public override string ToString()
		{
			return $"{Rank}\t{Info}";
		}
	}
}