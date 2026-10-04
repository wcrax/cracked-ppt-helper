using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Office.Interop.PowerPoint;

namespace PPTService
{
	// Token: 0x02000036 RID: 54
	public static class PptAnalyzer
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001FC RID: 508 RVA: 0x000095C0 File Offset: 0x000077C0
		public static bool IsAnalyzeAllAsyncRunning
		{
			get
			{
				return PptAnalyzer._thread != null && PptAnalyzer._thread.IsAlive;
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000095D8 File Offset: 0x000077D8
		public static void StartAnalyzeAllAsync(AnalyzeRate rate)
		{
			if (PptAnalyzer.IsAnalyzeOver() || PptAnalyzer._isAnalyzingAll)
			{
				return;
			}
			if (!PptAnalyzer.IsAnalyzeAllAsyncRunning && PptOperator.Instance.IsShowing)
			{
				PptAnalyzer._thread = new Thread(new ThreadStart(PptAnalyzer.AnalyzeAsync))
				{
					IsBackground = true
				};
				PptAnalyzer._isBreakExportAsync = false;
				PptAnalyzer._thread.Start();
			}
			PptAnalyzer.CurrentAnalyzeRate = rate;
			switch (PptAnalyzer.CurrentAnalyzeRate)
			{
			case AnalyzeRate.Lowest:
				PptAnalyzer._waitTime = 999;
				return;
			case AnalyzeRate.Middle:
				PptAnalyzer._waitTime = 333;
				return;
			case AnalyzeRate.Highest:
				PptAnalyzer._waitTime = 0;
				return;
			default:
				return;
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000966F File Offset: 0x0000786F
		public static void StopAnalyzeAllAsyn()
		{
			if (!PptAnalyzer.IsAnalyzeAllAsyncRunning)
			{
				return;
			}
			PptAnalyzer._isBreakExportAsync = true;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000967F File Offset: 0x0000787F
		public static bool SeewoLinkRequest(int startIndex, int range = 4)
		{
			if (startIndex + range < 1 || startIndex - range > PptOperator.Instance.SlidesCount)
			{
				return false;
			}
			PptAnalyzer.StopAnalyzeAllAsyn();
			bool result = PptAnalyzer.PptExportRangeImage(PptOperator.Instance.PptFilePath, null, startIndex, range);
			PptAnalyzer.CurrentShowPosition = startIndex;
			return result;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x000096B8 File Offset: 0x000078B8
		private static void AnalyzeAsync()
		{
			if (PptAnalyzer.IsAnalyzeOver())
			{
				return;
			}
			while (!PptAnalyzer.IsAnalyzeOver() && !PptAnalyzer._isBreakExportAsync && PptOperator.Instance.IsShowing)
			{
				bool flag = true;
				if (PptAnalyzer._turn)
				{
					for (int i = PptAnalyzer.CurrentShowPosition; i >= 1; i--)
					{
						if (!PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(i))
						{
							flag = PptAnalyzer.PptExportRangeImage(PptOperator.Instance.PptFilePath, null, i, 0);
							break;
						}
					}
				}
				else
				{
					for (int j = PptAnalyzer.CurrentShowPosition; j <= PptOperator.Instance.SlidesCount; j++)
					{
						if (!PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(j))
						{
							flag = PptAnalyzer.PptExportRangeImage(PptOperator.Instance.PptFilePath, null, j, 0);
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				PptAnalyzer._count++;
				if (PptAnalyzer._count > 5)
				{
					PptAnalyzer._count = 0;
					PptAnalyzer.UpdateListBoxItem();
				}
				Thread.Sleep(PptAnalyzer._waitTime);
				PptAnalyzer._turn = !PptAnalyzer._turn;
			}
			PptAnalyzer.UpdateListBoxItem();
		}

		// Token: 0x06000201 RID: 513 RVA: 0x000097AC File Offset: 0x000079AC
		public static void InitPptAnalyzer(string pptFilePath)
		{
			List<string> list;
			PptAnalyzer.FindAlreadyAnalyzedFiles(pptFilePath, out list);
			PptAnalyzer.CreateOrGetAnalyzedIndexList();
			PptAnalyzer.CreateOrGetThumbnailFilePath();
			PptAnalyzer.SetThumbnailSize(PptOperator.Instance.Slides.FirstOrDefault<Slide>());
		}

		// Token: 0x06000202 RID: 514 RVA: 0x000097E0 File Offset: 0x000079E0
		private static void UpdateListBoxItem()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				SyntacticSugar.CurrentDispatcherBeginInvoke(delegate()
				{
					string[] array = new string[PptOperator.Instance.SlidesCount];
					foreach (KeyValuePair<int, string> keyValuePair in PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary)
					{
						int num = keyValuePair.Key - 1;
						if (num >= 0 && num <= array.Length - 1)
						{
							array[num] = keyValuePair.Value;
						}
					}
					ViewHelper.MainWindow.UpdateListBoxItem(array.ToList<string>());
				});
			}, null);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00009808 File Offset: 0x00007A08
		public static void PptExportAllImages(string pptFilePath, Action<double> exportProgress, out List<string> imageFiles)
		{
			object analyzeLocker = PptAnalyzer.AnalyzeLocker;
			lock (analyzeLocker)
			{
				PptAnalyzer._isAnalyzingAll = true;
				PptAnalyzer.IsBreakExport = false;
				imageFiles = new List<string>();
				PptAnalyzer.StopAnalyzeAllAsyn();
				try
				{
					foreach (Slide slide in PptOperator.Instance.Slides)
					{
						if (PptAnalyzer.IsBreakExport)
						{
							break;
						}
						int slideIndex = slide.SlideIndex;
						if (PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(slideIndex))
						{
							imageFiles.Add(PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.FirstOrDefault((KeyValuePair<int, string> i) => i.Key == slideIndex).Value);
						}
						else
						{
							string text = Path.Combine(PptAnalyzer.ThumbnailFilePath, slideIndex.ToString() + PptAnalyzer.JpgSuffix);
							slide.Export(text, PptAnalyzer.JpgSuffix, PptAnalyzer._thumbnailWidth, PptAnalyzer._thumbnailHeight);
							imageFiles.Add(text);
							PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.Add(slideIndex, text);
							if (exportProgress != null)
							{
								exportProgress((double)slide.SlideIndex / ((double)PptOperator.Instance.SlidesCount * 1.0));
							}
						}
					}
					PptAnalyzer.UpdateListBoxItem();
					if (PptAnalyzer.IsBreakExport)
					{
						imageFiles = new List<string>();
						PptAnalyzer._isAnalyzingAll = false;
						PptAnalyzer.IsBreakExport = false;
					}
					else
					{
						PptAnalyzer.AddInfoToDictionary(pptFilePath);
						imageFiles = (from x in imageFiles
						orderby Convert.ToInt32(Path.GetFileNameWithoutExtension(x))
						select x).ToList<string>();
						if (exportProgress != null)
						{
							exportProgress(1.0);
						}
					}
				}
				catch (Exception)
				{
					imageFiles = new List<string>();
				}
				finally
				{
					PptAnalyzer.IsBreakExport = false;
					PptAnalyzer._isAnalyzingAll = false;
					GarbageCollection.Collect();
				}
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00009A40 File Offset: 0x00007C40
		public static bool PptExportRangeImage(string pptFilePath, Action<double> exportProgress, int startIndex, int range = 4)
		{
			object analyzeLocker = PptAnalyzer.AnalyzeLocker;
			bool result;
			lock (analyzeLocker)
			{
				int slidesCount = PptOperator.Instance.SlidesCount;
				int num = startIndex - range;
				int num2 = startIndex + range;
				if (num <= 0)
				{
					num2 -= num - 1;
				}
				if (num2 > slidesCount)
				{
					num -= num2 - slidesCount;
				}
				if (num2 < num)
				{
					result = false;
				}
				else
				{
					if (num < 1)
					{
						num = 1;
					}
					if (num2 > slidesCount)
					{
						num2 = slidesCount;
					}
					try
					{
						for (int i = num - 1; i < num2; i++)
						{
							Slide slide = PptOperator.Instance.Slides[i];
							int slideIndex = slide.SlideIndex;
							if (!PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(slideIndex))
							{
								string text = Path.Combine(PptAnalyzer.ThumbnailFilePath, slideIndex.ToString() + PptAnalyzer.JpgSuffix);
								slide.Export(text, PptAnalyzer.JpgSuffix, PptAnalyzer._thumbnailWidth, PptAnalyzer._thumbnailHeight);
								PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.Add(slideIndex, text);
								if (exportProgress != null)
								{
									exportProgress((double)(i - num + 1) / ((double)(num2 - num + 1) * 1.0));
								}
							}
						}
						PptAnalyzer.AddInfoToDictionary(pptFilePath);
						if (exportProgress != null)
						{
							exportProgress(1.0);
						}
					}
					catch (Exception)
					{
						return false;
					}
					finally
					{
						GarbageCollection.Collect();
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00009BD0 File Offset: 0x00007DD0
		private static void SetThumbnailSize(Slide slide)
		{
			if (slide == null)
			{
				return;
			}
			PptAnalyzer.ThumbnailRate = (double)(slide.Master.Width / slide.Master.Height);
			PptAnalyzer._thumbnailWidth = Convert.ToInt32((double)PptAnalyzer._thumbnailHeight * PptAnalyzer.ThumbnailRate);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00009C09 File Offset: 0x00007E09
		private static bool IsAnalyzeOver()
		{
			return PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary.Count == PptOperator.Instance.SlidesCount;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00009C24 File Offset: 0x00007E24
		private static void AddInfoToDictionary(string pptFilePath)
		{
			if (string.IsNullOrEmpty(PptAnalyzer._md5) || !PptAnalyzer.IsAnalyzeOver())
			{
				return;
			}
			if (!PptAnalyzer.AnalyzeFinishedFilePathDictionary.ContainsKey(PptAnalyzer._md5))
			{
				PptAnalyzer.AnalyzeFinishedFilePathDictionary.Add(PptAnalyzer._md5, pptFilePath);
			}
			if (!PptAnalyzer.AnalyzeFinishedThumbnailPathDictionary.ContainsKey(PptAnalyzer._md5))
			{
				PptAnalyzer.AnalyzeFinishedThumbnailPathDictionary.Add(PptAnalyzer._md5, PptAnalyzer.ThumbnailFilePath);
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00009C8C File Offset: 0x00007E8C
		private static void CreateOrGetThumbnailFilePath()
		{
			if (string.IsNullOrEmpty(PptAnalyzer._md5))
			{
				PptAnalyzer.ThumbnailFilePath = Path.Combine(AppPath.TemPath(), Guid.NewGuid().ToString());
				if (!Directory.Exists(PptAnalyzer.ThumbnailFilePath))
				{
					Directory.CreateDirectory(PptAnalyzer.ThumbnailFilePath);
				}
				return;
			}
			if (!PptAnalyzer.ThumbnailFilePathDictionary.ContainsKey(PptAnalyzer._md5))
			{
				PptAnalyzer.ThumbnailFilePath = Path.Combine(AppPath.TemPath(), Guid.NewGuid().ToString());
				if (!Directory.Exists(PptAnalyzer.ThumbnailFilePath))
				{
					Directory.CreateDirectory(PptAnalyzer.ThumbnailFilePath);
				}
				PptAnalyzer.ThumbnailFilePathDictionary.Add(PptAnalyzer._md5, PptAnalyzer.ThumbnailFilePath);
				return;
			}
			string thumbnailFilePath;
			PptAnalyzer.ThumbnailFilePathDictionary.TryGetValue(PptAnalyzer._md5, out thumbnailFilePath);
			PptAnalyzer.ThumbnailFilePath = thumbnailFilePath;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00009D58 File Offset: 0x00007F58
		private static void CreateOrGetAnalyzedIndexList()
		{
			if (string.IsNullOrEmpty(PptAnalyzer._md5))
			{
				PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary = new Dictionary<int, string>();
				return;
			}
			if (!PptAnalyzer.PptMd5AndAnalyzedIndexDictionary.ContainsKey(PptAnalyzer._md5))
			{
				PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary = new Dictionary<int, string>();
				PptAnalyzer.PptMd5AndAnalyzedIndexDictionary.Add(PptAnalyzer._md5, PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary);
				return;
			}
			Dictionary<int, string> analyzedIndexAndThumbnailFilePathDictionary;
			PptAnalyzer.PptMd5AndAnalyzedIndexDictionary.TryGetValue(PptAnalyzer._md5, out analyzedIndexAndThumbnailFilePathDictionary);
			PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary = analyzedIndexAndThumbnailFilePathDictionary;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00009DC4 File Offset: 0x00007FC4
		public static bool FindAlreadyAnalyzedFiles(string filePath, out List<string> imageFiles)
		{
			bool result;
			try
			{
				imageFiles = new List<string>();
				if (!RegexHelper.IsLocalPath(filePath))
				{
					PptAnalyzer._md5 = null;
					result = false;
				}
				else
				{
					if (!File.Exists(filePath) && PptAnalyzer.AnalyzeFinishedFilePathDictionary.ContainsValue(filePath))
					{
						PptAnalyzer._md5 = PptAnalyzer.AnalyzeFinishedFilePathDictionary.FirstOrDefault((KeyValuePair<string, string> d) => d.Value == filePath).Key;
					}
					else if (File.Exists(filePath))
					{
						PptAnalyzer._md5 = Md5SecurityUtility.GetShaFromFile(filePath);
					}
					if (string.IsNullOrEmpty(PptAnalyzer._md5))
					{
						result = false;
					}
					else
					{
						string text;
						if (!PptAnalyzer.AnalyzeFinishedThumbnailPathDictionary.ContainsKey(PptAnalyzer._md5))
						{
							text = null;
						}
						else
						{
							text = PptAnalyzer.AnalyzeFinishedThumbnailPathDictionary.FirstOrDefault((KeyValuePair<string, string> d) => d.Key == PptAnalyzer._md5).Value;
						}
						string text2 = text;
						if (text2 == null)
						{
							result = false;
						}
						else
						{
							DirectoryInfo directoryInfo = new DirectoryInfo(text2);
							if (directoryInfo.GetFiles(PptAnalyzer.FilterName).Any<FileInfo>())
							{
								imageFiles.AddRange((from x in directoryInfo.GetFiles(PptAnalyzer.FilterName)
								orderby Convert.ToInt32(Path.GetFileNameWithoutExtension(x.Name))
								select x into imageFile
								select imageFile.FullName).ToList<string>());
								PptAnalyzer.ThumbnailFilePath = text2;
								result = true;
							}
							else
							{
								result = false;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				imageFiles = new List<string>();
				Log.WriteError(ex.ToString());
				result = false;
			}
			return result;
		}

		// Token: 0x040000F2 RID: 242
		private static bool _turn;

		// Token: 0x040000F3 RID: 243
		private static int _count;

		// Token: 0x040000F4 RID: 244
		private static string _md5;

		// Token: 0x040000F5 RID: 245
		private static Thread _thread;

		// Token: 0x040000F6 RID: 246
		public static bool IsBreakExport;

		// Token: 0x040000F7 RID: 247
		private static int _waitTime = 999;

		// Token: 0x040000F8 RID: 248
		private static bool _isAnalyzingAll;

		// Token: 0x040000F9 RID: 249
		private static bool _isBreakExportAsync;

		// Token: 0x040000FA RID: 250
		public static string ThumbnailFilePath;

		// Token: 0x040000FB RID: 251
		public static double ThumbnailRate = 1.7;

		// Token: 0x040000FC RID: 252
		private static int _thumbnailWidth = 960;

		// Token: 0x040000FD RID: 253
		private static int _thumbnailHeight = 540;

		// Token: 0x040000FE RID: 254
		public static int CurrentShowPosition = 1;

		// Token: 0x040000FF RID: 255
		public static AnalyzeRate CurrentAnalyzeRate;

		// Token: 0x04000100 RID: 256
		public static readonly string JpgSuffix = ".jpg";

		// Token: 0x04000101 RID: 257
		public static readonly string FilterName = "*.jpg";

		// Token: 0x04000102 RID: 258
		private static readonly object AnalyzeLocker = new object();

		// Token: 0x04000103 RID: 259
		public static Dictionary<int, string> AnalyzedIndexAndThumbnailFilePathDictionary = new Dictionary<int, string>();

		// Token: 0x04000104 RID: 260
		private static readonly Dictionary<string, string> ThumbnailFilePathDictionary = new Dictionary<string, string>();

		// Token: 0x04000105 RID: 261
		private static readonly Dictionary<string, string> AnalyzeFinishedFilePathDictionary = new Dictionary<string, string>();

		// Token: 0x04000106 RID: 262
		private static readonly Dictionary<string, string> AnalyzeFinishedThumbnailPathDictionary = new Dictionary<string, string>();

		// Token: 0x04000107 RID: 263
		private static readonly Dictionary<string, Dictionary<int, string>> PptMd5AndAnalyzedIndexDictionary = new Dictionary<string, Dictionary<int, string>>();
	}
}
