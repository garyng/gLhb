using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace gLhb.Parser
{
	public class LhbParser
	{
		//|----------------|----------------|------------|--------|
		//|							Episode						  |
		//|----------------|----------------|------------|--------|
		//| This Week Rank | Last Week Rank | Song Title | Artist | x20
		//|----------------|----------------|------------|--------|

		public List<Lhb> Parse(string filename)
		{
			var workbook = new XLWorkbook(filename, XLEventTracking.Disabled);
			var worksheet = workbook.Worksheet("醉心龙虎榜");

			var allLhbRange = worksheet.RangeUsed();
			//allLhbRange.Column(1).Delete();	// Column A
			allLhbRange.Row(1).Delete(); // Header
			allLhbRange.Row(1).Delete(); // Blank line


			int totalRows = allLhbRange.RowCount();
			int totalColumns = allLhbRange.ColumnCount();

			var lhbs = new List<Lhb>();

			for (int row = 1; row < totalRows; row += 22)
			{
				for (int column = 1; column < totalColumns; column += 5)
				{
					var firstCell = allLhbRange.Cell(row, column);
					var lastCell = allLhbRange.Cell(row + 21 - 1, column + 4 - 1);
					var range = allLhbRange.Range(firstCell, lastCell);
					var lhb = ExtractFromRange(range);
					if (lhb != null)
					{
						lhbs.Add(lhb);
					}
				}
			}
			return lhbs
				.OrderByDescending(lhb => lhb.Info.Date)
				.ToList();
		}

		public List<SongInfo> GetAllSongs(string filename)
		{
			return Parse(filename)
				.SelectMany(lhb => lhb.Songs)
				.Select(song => song.Info)
				.Distinct()
				.ToList();
		}

		private Lhb ExtractFromRange(IXLRange range)
		{
			if (range.Rows()
				.Select(row => row.Cells()
					.Select(cell => cell.Value.ToString())
					.Where(text => !string.IsNullOrEmpty(text))
					.Join(""))
				.All(string.IsNullOrEmpty))
			{
				// empty space, nothing in this range
				return null;
			}

			var titleText = range.Rows()
				.First()
				.Cells()
				.Select(cell => cell.Value.ToString())
				.Where(text => !string.IsNullOrEmpty(text))
				.Join("");

			var songsRows = range.Rows().Skip(1);
			var songEntries = songsRows.Select(song => new Song()
				{
					Rank = new RankInfo()
					{
						ThisWeekRank = song.Cell(1).Value.ToString(),
						LastWeekRank = song.Cell(2).Value.ToString()
					},
					Info = new SongInfo()
					{
						Title = song.Cell(3).Value.ToString(),
						Artist = song.Cell(4).Value.ToString()
					}
				})
				.ToList();

			return new Lhb()
			{
				Info = new LhbInfo()
				{
					Date = ExtractDate(titleText),
					Week = ExtractWeekNumber(titleText)
				},
				Songs = songEntries
			};
		}

		private DateTime ExtractDate(string title)
		{
			var match = Regex.Match(title, "(\\d{4})年(\\d{1,2})月(\\d{1,2})日");
			int year = int.Parse(match.Groups[1].Value);
			int month = int.Parse(match.Groups[2].Value);
			int date = int.Parse(match.Groups[3].Value);
			return new DateTime(year, month, date);
		}

		private int ExtractWeekNumber(string title)
		{
			return int.Parse(Regex.Match(title, "第(\\d+)期").Groups[1].Value);
		}
	}
}