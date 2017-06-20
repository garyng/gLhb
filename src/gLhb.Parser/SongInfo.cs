namespace gLhb.Parser
{
	public class SongInfo
	{
		public string Title { get; set; }
		public string Artist { get; set; }

		public override string ToString()
		{
			return $"{Title}\t({Artist})";
		}
	}
}