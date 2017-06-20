using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using gLhb.Parser;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;

namespace gLhbViewer
{
	public class MainViewModel : ViewModelBase
	{
		private const string _filename = "Y.E.S. 93.3 醉心龙虎榜.xlsx";
		private readonly IDialogCoordinator _dialogCoordinator;

		public RelayCommand RefreshCommand { get; set; }

		private ObservableCollection<Lhb> _lhbs;

		public ObservableCollection<Lhb> Lhbs
		{
			get => _lhbs;
			set => Set(ref _lhbs, value);
		}

		private Lhb _selectedLhb;

		public Lhb SelectedLhb
		{
			get { return _selectedLhb; }
			set { Set(ref _selectedLhb, value); }
		}

		
		public MainViewModel(IDialogCoordinator dialogCoordinator)
		{
			_dialogCoordinator = dialogCoordinator;
			RefreshCommand = new RelayCommand(async () =>
			{
				if (await FetchFromWeb())
				{
					await Parse();
				}
			});

#if DEBUG
			if (IsInDesignMode)
			{
				Lhbs = new ObservableCollection<Lhb>
				{
					new Lhb()
					{
						Info = new LhbInfo()
						{
							Date = DateTime.Now,
							Week = 10
						},
						Songs = new List<Song>()
						{
							new Song()
							{
								Info = new SongInfo()
								{
									Artist = "Testing Artist",
									Title = "A pretty long title for a song. Yeah. Pretty long."
								},
								Rank = new RankInfo()
								{
									LastWeekRank = "新歌",
									ThisWeekRank = "1",
								}
							}
						}
					},
					new Lhb()
					{
						Info = new LhbInfo()
						{
							Date = DateTime.Now.AddDays(1),
							Week = 11
						},
						Songs = new List<Song>()
						{
							new Song()
							{
								Info = new SongInfo()
								{
									Artist = "Testing Artist 2",
									Title = "A pretty long title for a song. Yeah. Pretty long. 2"
								},
								Rank = new RankInfo()
								{
									LastWeekRank = "新歌",
									ThisWeekRank = "2",
								}
							}
						}
					}
				};
				SelectedLhb = Lhbs.First();
			}
#endif
		}

		private async Task<bool> FetchFromWeb()
		{
			WebClient wc = new WebClient();
			ProgressDialogController controller =
				await _dialogCoordinator.ShowProgressAsync(this, "Fetching...", "Fetching latest data... Please wait...");
			controller.SetIndeterminate();

			try
			{
				await wc.DownloadFileTaskAsync(
						"https://docs.google.com/spreadsheets/d/1_62S7NTMbkJFOaj9nhzMTCgSkwX9hFRrruxroNdzlAs/pub?authkey=CJ7p6&output=xls",
						_filename)
					.ConfigureAwait(false);
			}
			catch (WebException e)
			{
				await _dialogCoordinator.ShowMessageAsync(this, "Error!",
					"Error occurred while fetching data from the web. Are you connected to the internet?");
				return false;
			}
			finally
			{
				await controller.CloseAsync();
			}
			return true;
		}

		private async Task Parse()
		{
			var lhbParser = new LhbParser();
			var controller = await _dialogCoordinator.ShowProgressAsync(this, "Parsing...", "Crunching data... Please wait...");
			controller.SetIndeterminate();
			if (!File.Exists(_filename))
			{
				await _dialogCoordinator.ShowMessageAsync(this, "Error!", "Data file does not exist... Please retry");
				await controller.CloseAsync();
				return;
			}
			List<Lhb> lhbs = await Task.Run(() => lhbParser.Parse(_filename));
			await controller.CloseAsync();
			Lhbs = new ObservableCollection<Lhb>(lhbs);
		}
	}
}