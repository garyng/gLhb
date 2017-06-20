using System;
using System.Collections.Generic;
using System.Linq;

namespace gLhb.Parser
{
	/// <summary>
	/// Lhb means "Áú»¢°ñ"
	/// </summary>
	public class Lhb
	{
		public LhbInfo Info { get; set; }
		public List<Song> Songs { get; set; }

		public override string ToString()
		{
			return $"{Info}\n{Songs.Select(song => song.ToString()).Join(Environment.NewLine)}";
		}
	}
}