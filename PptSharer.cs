using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Cvte.WebService.Lychee;
using Cvte.Windows;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x0200002A RID: 42
	public static class PptSharer
	{
		// Token: 0x06000161 RID: 353 RVA: 0x00005A03 File Offset: 0x00003C03
		public static void Share(string pptFilePath)
		{
			PptSharer.IsCancelShare = false;
			PptSharer._isAddExportTime = false;
			SyntacticSugar.TryCatchAction(delegate
			{
				ViewHelper.StopMainWindowUpdateTimerAndMouseGesture();
				if (RegexHelper.IsLocalPath(pptFilePath) && File.Exists(pptFilePath))
				{
					string md5 = Md5SecurityUtility.GetShaFromFile(pptFilePath);
					if (PptSharer.PptMd5AndQrFilePathDictionary.ContainsKey(md5))
					{
						string value = PptSharer.PptMd5AndQrFilePathDictionary.FirstOrDefault((KeyValuePair<string, string> d) => d.Key == md5).Value;
						if (File.Exists(value))
						{
							FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "ShowExistentQrImage");
							new QrWindow(value)
							{
								Owner = ViewHelper.MainWindow
							}.ShowDialog();
							return;
						}
					}
				}
				if (!NetworkHelper.IsNetworkConnect)
				{
					FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "NetwrokIsNotConnect");
					new MessageWindow("无网络连接，无法创建二维码")
					{
						Owner = ViewHelper.MainWindow
					}.ShowDialog();
					return;
				}
				PptSharer.LycheeShare(pptFilePath);
			}, new Action(ViewHelper.BeginMainWindowUpdateTimerAndMouseGesture));
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00005A3C File Offset: 0x00003C3C
		private static void LycheeShare(string pptFilePath)
		{
			LoadingScreen<LoadingView>.Current.Show(new Rect
			{
				X = ViewHelper.MainWindow.Left,
				Y = ViewHelper.MainWindow.Top,
				Width = ViewHelper.MainWindow.Width,
				Height = ViewHelper.MainWindow.Height
			}, true);
			LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
			{
				loadingView.Start();
				loadingView.Tip = "创建二维码中...";
				loadingView.CancelAction = new Action(PptSharer.Cancel);
			});
			List<string> list;
			if (!PptAnalyzer.FindAlreadyAnalyzedFiles(pptFilePath, out list))
			{
				PptAnalyzer.PptExportAllImages(pptFilePath, delegate(double progress)
				{
					LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
					{
						PptSharer._isAddExportTime = true;
						loadingView.Progress = Convert.ToInt32(progress * 30.0);
					});
				}, out list);
			}
			if (list.Any<string>())
			{
				PptSharer.CancellationTokenSource = new CancellationTokenSource();
				try
				{
					LycheeService lycheeService = new LycheeService("http://lizhi.seewo.com", "pptservice", "0f6bb4dc70e901e21eba5dfdec6c68f6");
					IList<string> files = list;
					PublicInfo publicInfo = new PublicInfo();
					publicInfo.Author = "";
					publicInfo.Title = Path.GetFileName(pptFilePath);
					publicInfo.Scale = PptAnalyzer.ThumbnailRate;
					string url;
					if (lycheeService.Publish(files, publicInfo, PptSharer.CancellationTokenSource.Token, delegate(double progress)
					{
						LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
						{
							loadingView.Progress = Convert.ToInt32(PptSharer._isAddExportTime ? (30.0 + progress * 70.0) : (progress * 100.0));
						});
					}, out url))
					{
						PptSharer.AfterShareSuccessfullyDo(pptFilePath, url);
					}
					else
					{
						LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
						{
							loadingView.Cancel();
						});
						PptSharer.AfterShareFailedDo();
					}
				}
				catch (Exception)
				{
					PptSharer.AfterShareFailedDo();
				}
				return;
			}
			LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
			{
				loadingView.Continue();
				LoadingScreen<LoadingView>.Current.Hide();
			});
			if (PptSharer.IsCancelShare)
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				return;
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "NoPicture");
			new MessageWindow("图片解析出错，无法创建二维码")
			{
				Owner = ViewHelper.MainWindow
			}.ShowDialog();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00005C48 File Offset: 0x00003E48
		private static void AfterShareSuccessfullyDo(string pptFilePath, string url)
		{
			string qrFilePath = Path.Combine(AppPath.TemPath(), Guid.NewGuid().ToString() + ".png");
			if (!string.IsNullOrEmpty(url))
			{
				new WebClient().DownloadFile(url, qrFilePath);
			}
			LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
			{
				loadingView.PushFrame();
				LoadingScreen<LoadingView>.Current.Hide();
				SyntacticSugar.CurrentDispatcherInvoke(delegate
				{
					if (string.IsNullOrEmpty(qrFilePath) || !File.Exists(qrFilePath))
					{
						return;
					}
					FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "Success");
					SyntacticSugar.TryCatchAction(delegate
					{
						ViewHelper.StopMainWindowUpdateTimerAndMouseGesture();
						QrWindow qrWindow = new QrWindow(qrFilePath);
						qrWindow.Owner = ViewHelper.MainWindow;
						if (RegexHelper.IsLocalPath(pptFilePath) && File.Exists(pptFilePath))
						{
							string shaFromFile = Md5SecurityUtility.GetShaFromFile(pptFilePath);
							PptSharer.PptMd5AndQrFilePathDictionary.Add(shaFromFile, qrFilePath);
						}
						qrWindow.ShowDialog();
					}, new Action(ViewHelper.BeginMainWindowUpdateTimerAndMouseGesture));
				});
			});
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00005CC0 File Offset: 0x00003EC0
		private static void AfterShareFailedDo()
		{
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			if (PptSharer.IsCancelShare)
			{
				return;
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "Failed");
			LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
			{
				loadingView.Continue();
				LoadingScreen<LoadingView>.Current.Hide();
				SyntacticSugar.CurrentDispatcherInvoke(delegate
				{
					new MessageWindow("网络开小差，无法创建二维码")
					{
						Owner = ViewHelper.MainWindow
					}.ShowDialog();
				});
			});
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00005D1D File Offset: 0x00003F1D
		private static void Cancel()
		{
			PptSharer.IsCancelShare = true;
			PptAnalyzer.IsBreakExport = true;
			CancellationTokenSource cancellationTokenSource = PptSharer.CancellationTokenSource;
			if (cancellationTokenSource == null)
			{
				return;
			}
			cancellationTokenSource.Cancel();
		}

		// Token: 0x0400006E RID: 110
		public static bool IsCancelShare;

		// Token: 0x0400006F RID: 111
		private static bool _isAddExportTime;

		// Token: 0x04000070 RID: 112
		public static CancellationTokenSource CancellationTokenSource;

		// Token: 0x04000071 RID: 113
		private static readonly Dictionary<string, string> PptMd5AndQrFilePathDictionary = new Dictionary<string, string>();
	}
}
