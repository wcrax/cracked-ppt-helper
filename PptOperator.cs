using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x02000038 RID: 56
	public class PptOperator
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000A00F File Offset: 0x0000820F
		// (set) Token: 0x0600020D RID: 525 RVA: 0x0000A017 File Offset: 0x00008217
		public bool IsShowing { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000A020 File Offset: 0x00008220
		// (set) Token: 0x0600020F RID: 527 RVA: 0x0000A028 File Offset: 0x00008228
		public Color PointerColor { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000A031 File Offset: 0x00008231
		// (set) Token: 0x06000211 RID: 529 RVA: 0x0000A039 File Offset: 0x00008239
		public int SlidesCount { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000A042 File Offset: 0x00008242
		// (set) Token: 0x06000213 RID: 531 RVA: 0x0000A04A File Offset: 0x0000824A
		public string PptFilePath { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000A053 File Offset: 0x00008253
		// (set) Token: 0x06000215 RID: 533 RVA: 0x0000A05B File Offset: 0x0000825B
		public List<Slide> Slides { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000216 RID: 534 RVA: 0x0000A064 File Offset: 0x00008264
		// (set) Token: 0x06000217 RID: 535 RVA: 0x0000A06B File Offset: 0x0000826B
		public static PptOperator Instance { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000218 RID: 536 RVA: 0x0000A073 File Offset: 0x00008273
		// (set) Token: 0x06000219 RID: 537 RVA: 0x0000A07B File Offset: 0x0000827B
		public Presentation Presentation { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000A084 File Offset: 0x00008284
		// (set) Token: 0x0600021B RID: 539 RVA: 0x0000A08C File Offset: 0x0000828C
		public IntPtr SlideShowWindowIntPtr { get; private set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000A095 File Offset: 0x00008295
		// (set) Token: 0x0600021D RID: 541 RVA: 0x0000A09D File Offset: 0x0000829D
		public PpSlideShowRangeType ShowRangeType { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0000A0A6 File Offset: 0x000082A6
		// (set) Token: 0x0600021F RID: 543 RVA: 0x0000A0D2 File Offset: 0x000082D2
		public PpSlideShowPointerType PointerType
		{
			get
			{
				PpSlideShowPointerType type = PpSlideShowPointerType.ppSlideShowPointerNone;
				SyntacticSugar.TryCatchAction(delegate
				{
					SlideShowWindow slideShowWindow = this._slideShowWindow;
					type = ((slideShowWindow != null) ? slideShowWindow.View.PointerType : PpSlideShowPointerType.ppSlideShowPointerNone);
				}, null);
				return type;
			}
			set
			{
				SyntacticSugar.TryCatchAction(delegate
				{
					if (this._slideShowWindow != null)
					{
						this._slideShowWindow.View.PointerType = value;
					}
				}, null);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000A0F8 File Offset: 0x000082F8
		public int CurrentShowPosition
		{
			get
			{
				int result;
				try
				{
					result = this._slideShowWindow.View.CurrentShowPosition;
				}
				catch
				{
					result = 1;
				}
				return result;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000A130 File Offset: 0x00008330
		public int EndingSlide
		{
			get
			{
				return this.Presentation.SlideShowSettings.EndingSlide;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0000A142 File Offset: 0x00008342
		public int StartingSlide
		{
			get
			{
				return this.Presentation.SlideShowSettings.StartingSlide;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000223 RID: 547 RVA: 0x0000A154 File Offset: 0x00008354
		public PpSlideShowType? PptShowType
		{
			get
			{
				return new PpSlideShowType?(this.Presentation.SlideShowSettings.ShowType);
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000A16B File Offset: 0x0000836B
		private void SetSlideAdvanceOnClickProperty(MsoTriState value)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				if (this.Slides == null || !this.Slides.Any<Slide>())
				{
					return;
				}
				foreach (Slide slide in this.Slides)
				{
					slide.SlideShowTransition.AdvanceOnClick = value;
				}
			}, null);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000A191 File Offset: 0x00008391
		public void PreviousPage()
		{
			if (this._stopwatch.ElapsedMilliseconds < (long)PptOperator.MinPageTurningTimerSpan)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(delegate
			{
				Task.Run(delegate()
				{
					this._slideShowWindow.View.Previous();
				});
			}, null);
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000A1B9 File Offset: 0x000083B9
		public void NextPage()
		{
			if (this._stopwatch.ElapsedMilliseconds < (long)PptOperator.MinPageTurningTimerSpan)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(delegate
			{
				Task.Run(delegate()
				{
					this._slideShowWindow.View.Next();
				});
			}, null);
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000A1E1 File Offset: 0x000083E1
		public void Exit()
		{
			PptOperator.SetActive((IntPtr)this._app.HWND);
			SyntacticSugar.TryCatchAction(delegate
			{
				Task.Run(delegate()
				{
					PptAnalyzer.StopAnalyzeAllAsyn();
					this._slideShowWindow.View.Exit();
				});
			}, null);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000A20C File Offset: 0x0000840C
		public void GotoSlide(int index)
		{
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowNextSlideEventHandler), this, "OnSlideShowNextSlide"));
			this._slideShowWindow.View.GotoSlide(index, MsoTriState.msoFalse);
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowNextSlideEventHandler), this, "OnSlideShowNextSlide"));
			this._slideShowWindow.Activate();
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000A28C File Offset: 0x0000848C
		public void UpdatePointerColor()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				Color red = Colors.Red;
				SlideShowWindow slideShowWindow = this._slideShowWindow;
				string text = ((slideShowWindow != null) ? slideShowWindow.View.PointerColor.RGB : 0).ToString("X2").PadLeft(6, '0');
				red.R = Convert.ToByte(text.Substring(4, 2), 16);
				red.G = Convert.ToByte(text.Substring(2, 2), 16);
				red.B = Convert.ToByte(text.Substring(0, 2), 16);
				this.PointerColor = red;
			}, null);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000A2A0 File Offset: 0x000084A0
		public void SetPointerColor(int color)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				this.PointerType = PpSlideShowPointerType.ppSlideShowPointerPen;
				this._slideShowWindow.View.PointerColor.RGB = color;
			}, null);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000A2C8 File Offset: 0x000084C8
		private bool CheckMainObject()
		{
			if (this._slideShowWindow == null)
			{
				Log.WriteError("SlideShowWindowIsNull");
				return false;
			}
			SlideShowWindow slideShowWindow = this._slideShowWindow;
			this.SlideShowWindowIntPtr = (IntPtr)((slideShowWindow != null) ? new int?(slideShowWindow.HWND) : null).Value;
			SlideShowWindow slideShowWindow2 = this._slideShowWindow;
			this.Presentation = ((slideShowWindow2 != null) ? slideShowWindow2.Presentation : null);
			if (this.Presentation == null)
			{
				Log.WriteError("PresentationIsNull");
				return false;
			}
			return true;
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x0600022C RID: 556 RVA: 0x0000A348 File Offset: 0x00008548
		// (remove) Token: 0x0600022D RID: 557 RVA: 0x0000A380 File Offset: 0x00008580
		public event EventHandler ShowBegin;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x0600022E RID: 558 RVA: 0x0000A3B8 File Offset: 0x000085B8
		// (remove) Token: 0x0600022F RID: 559 RVA: 0x0000A3F0 File Offset: 0x000085F0
		public event EventHandler ShowEnd;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000230 RID: 560 RVA: 0x0000A428 File Offset: 0x00008628
		// (remove) Token: 0x06000231 RID: 561 RVA: 0x0000A460 File Offset: 0x00008660
		public event EventHandler NextSlide;

		// Token: 0x06000232 RID: 562 RVA: 0x0000A495 File Offset: 0x00008695
		private void OnShowBegin()
		{
			EventHandler showBegin = this.ShowBegin;
			if (showBegin == null)
			{
				return;
			}
			showBegin(null, EventArgs.Empty);
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000A4AD File Offset: 0x000086AD
		private void OnShowEnd()
		{
			EventHandler showEnd = this.ShowEnd;
			if (showEnd == null)
			{
				return;
			}
			showEnd(null, EventArgs.Empty);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000A4C5 File Offset: 0x000086C5
		private void OnNextSlide()
		{
			EventHandler nextSlide = this.NextSlide;
			if (nextSlide == null)
			{
				return;
			}
			nextSlide(this, EventArgs.Empty);
		}

		// Token: 0x06000235 RID: 565
		[DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
		private static extern bool SetActive(IntPtr hWnd);

		// Token: 0x06000236 RID: 566 RVA: 0x0000A4E0 File Offset: 0x000086E0
		public void Initialize()
		{
			this._stopwatch = new Stopwatch();
			this._checkPowerPointTimer.Interval = TimeSpan.FromMilliseconds(2500.0);
			this._checkPowerPointTimer.Tick += this.CheckPowerPointProcess;
			this._checkPowerPointTimer.Start();
			this._clearEmptyPowerPointTimer.Interval = TimeSpan.FromMilliseconds(2500.0);
			this._clearEmptyPowerPointTimer.Tick += this.ClearEmptyPowerPointProcess;
			this.CheckPowerPointProcess();
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000A56C File Offset: 0x0000876C
		private void CheckPowerPointProcess(object sender, EventArgs e)
		{
			this.CheckPowerPointProcess();
			int times = this._times;
			this._times = times + 1;
			if (times > 10)
			{
				this._times = 0;
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000A5A5 File Offset: 0x000087A5
		private void ClearEmptyPowerPointProcess(object sender, EventArgs e)
		{
			this.ClearEmptyPowerPointProcess();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000A5B0 File Offset: 0x000087B0
		private void CheckPowerPointProcess()
		{
			if (ProcessHelper.HasPowerPointProcess())
			{
				try
				{
					this.CreatePowerPointApplication();
					this.InitializeControl();
					this.SetTimerEnable(true);
				}
				catch (Exception)
				{
					this._app = null;
					ConfigHelper.Instance.IsInitApplicationSuccessful = false;
				}
			}
			this._isCheckPptStateFirstly = false;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000A608 File Offset: 0x00008808
		private void ClearEmptyPowerPointProcess()
		{
			try
			{
				if (!ProcessHelper.HasPowerPointProcess())
				{
					this.CleanAndListen();
				}
				else if (this._app != null && this._app.Presentations.Count == 0)
				{
					this.CleanAndListen();
					if (this._app.Caption.ToLower().Contains("wps") || this._app.Path.ToLower().Contains("wps") || this._app.Path.ToLower().Contains("wpp"))
					{
						ProcessHelper.TryKillWppProcess();
					}
				}
			}
			catch (COMException ex)
			{
				if (ex.HResult == -2147023174)
				{
					this.CleanAndListen();
				}
			}
			catch (Exception ex2)
			{
				Log.WriteError(ex2.Message);
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000A6E4 File Offset: 0x000088E4
		private void SetTimerEnable(bool isPowerPointProcessStart)
		{
			if (isPowerPointProcessStart)
			{
				this._checkPowerPointTimer.Stop();
				this._clearEmptyPowerPointTimer.Start();
				return;
			}
			this._clearEmptyPowerPointTimer.Stop();
			this._checkPowerPointTimer.Start();
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000A718 File Offset: 0x00008918
		private void InitializeControl()
		{
			if (this._app == null || this.IsShowing)
			{
				return;
			}
			SlideShowWindows slideShowWindows = this._app.SlideShowWindows;
			if (slideShowWindows.Count > 0)
			{
				if (this._isCheckPptStateFirstly)
				{
					try
					{
						if (this._app.ActivePresentation.SlideShowSettings.ShowType == PpSlideShowType.ppShowTypeSpeaker)
						{
							return;
						}
						this.AppSlideShowBegin(slideShowWindows[1]);
						return;
					}
					catch (Exception arg)
					{
						Log.WriteError(string.Format("InitializeControl：{0}", arg));
						return;
					}
				}
				this.AppSlideShowBegin(slideShowWindows[1]);
			}
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000A7AC File Offset: 0x000089AC
		private void CreatePowerPointApplication()
		{
			if (this._app != null)
			{
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowBeginEventHandler), this, "AppSlideShowBegin"));
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowEndEventHandler), this, "OnAppSlideShowEnd"));
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowNextSlideEventHandler), this, "OnSlideShowNextSlide"));
			}
			this._app = (Microsoft.Office.Interop.PowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
			PptVersionManager.UpdatePptApplicationInfo(this._app.Path, this._app.Version);
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").AddEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowBeginEventHandler), this, "AppSlideShowBegin"));
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").AddEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowEndEventHandler), this, "OnAppSlideShowEnd"));
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowNextSlideEventHandler), this, "OnSlideShowNextSlide"));
			new ComAwareEventInfo(typeof(EApplication_Event), "PresentationCloseFinal").AddEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_PresentationCloseFinalEventHandler), this, "AppOnPresentationCloseFinal"));
			ConfigHelper.Instance.IsInitApplicationSuccessful = true;
			Log.WriteDebug("PPTApplicationInitialized");
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000A938 File Offset: 0x00008B38
		private void AppOnPresentationCloseFinal(Presentation pres)
		{
			if (this._app.Presentations.Count == 0 || this._app.Presentations.Count == 1)
			{
				try
				{
					if (this._app.ActivePresentation == pres)
					{
						System.Windows.Application application = System.Windows.Application.Current;
						if (application != null)
						{
							application.Dispatcher.InvokeAsync(new Action(this.CleanAndListen));
						}
					}
				}
				catch (Exception ex)
				{
					Log.WriteDebug(ex.Message);
				}
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000A9BC File Offset: 0x00008BBC
		private void CleanAndListen()
		{
			this.TryCleanComObject();
			this.SetTimerEnable(false);
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000A9CC File Offset: 0x00008BCC
		private void TryCleanComObject()
		{
			try
			{
				if (this._app != null)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowBeginEventHandler), this, "AppSlideShowBegin"));
					new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowEndEventHandler), this, "OnAppSlideShowEnd"));
					new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(this._app, Delegate.CreateDelegate(typeof(EApplication_SlideShowNextSlideEventHandler), this, "OnSlideShowNextSlide"));
					Marshal.ReleaseComObject(this._app);
					Marshal.ReleaseComObject(this._app);
					this._app = null;
				}
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.Message);
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000AAAC File Offset: 0x00008CAC
		private void AppSlideShowBegin(SlideShowWindow wn)
		{
			try
			{
				if (ConfigHelper.Instance.IsNeedStart())
				{
					this._slideShowWindow = wn;
					if (this.CheckMainObject())
					{
						PpSlideShowType? pptShowType = this.PptShowType;
						PpSlideShowType ppSlideShowType = PpSlideShowType.ppShowTypeSpeaker;
						this._isShowTypeSpeaker = (pptShowType.GetValueOrDefault() == ppSlideShowType & pptShowType != null);
						ConfigHelper.Instance.IsPptShowTypeSpeaker = this._isShowTypeSpeaker;
						if (this._isShowTypeSpeaker)
						{
							this.ShowRangeType = this.Presentation.SlideShowSettings.RangeType;
							this.SlidesCount = this.Presentation.Slides.Count;
							this.PptFilePath = this.Presentation.FullName;
							this.Slides = this.Presentation.Slides.Cast<Slide>().ToList<Slide>();
							PptAnalyzer.InitPptAnalyzer(this.PptFilePath);
							this.IsShowing = true;
							this.OnShowBegin();
							this.SetSlideAdvanceOnClickProperty(MsoTriState.msoFalse);
							ConfigHelper.Instance.IsShowing = this.IsShowing;
							SyntacticSugar.CurrentDispatcherBeginInvoke(delegate()
							{
								this._stopwatch.Restart();
							});
							ConfigHelper.Instance.IsComOk = true;
							Log.WriteDebug("PPTShowBegin");
						}
					}
				}
			}
			catch (Exception arg)
			{
				ConfigHelper.Instance.IsComOk = false;
				Log.WriteError(string.Format("ShowBeginError：{0}", arg));
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000AC08 File Offset: 0x00008E08
		private void OnAppSlideShowEnd(Presentation pres)
		{
			this.SetSlideAdvanceOnClickProperty(MsoTriState.msoTrue);
			SyntacticSugar.TryCatchAction(delegate
			{
				if (!this._isShowTypeSpeaker)
				{
					return;
				}
				this.IsShowing = false;
				PptAnalyzer.StopAnalyzeAllAsyn();
				SyntacticSugar.CurrentDispatcherBeginInvoke(delegate()
				{
					this._stopwatch.Stop();
				});
				ConfigHelper.Instance.IsShowing = this.IsShowing;
				this._isShowTypeSpeaker = false;
				this.OnShowEnd();
				Log.WriteDebug("PPTShowEnd");
			}, null);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000AC23 File Offset: 0x00008E23
		private void OnSlideShowNextSlide(SlideShowWindow wn)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				if (!this._isShowTypeSpeaker)
				{
					return;
				}
				SyntacticSugar.CurrentDispatcherBeginInvoke(delegate()
				{
					this._stopwatch.Restart();
				});
				this.OnNextSlide();
			}, null);
		}

		// Token: 0x0400010C RID: 268
		private int _times;

		// Token: 0x0400010D RID: 269
		private Microsoft.Office.Interop.PowerPoint.Application _app;

		// Token: 0x0400010E RID: 270
		private Stopwatch _stopwatch;

		// Token: 0x0400010F RID: 271
		private bool _isShowTypeSpeaker;

		// Token: 0x04000110 RID: 272
		private SlideShowWindow _slideShowWindow;

		// Token: 0x04000111 RID: 273
		private const int PptProcessCheckInterval = 2500;

		// Token: 0x04000112 RID: 274
		private static readonly int MinPageTurningTimerSpan = 30;

		// Token: 0x0400011F RID: 287
		private bool _isCheckPptStateFirstly = true;

		// Token: 0x04000120 RID: 288
		private readonly DispatcherTimer _checkPowerPointTimer = new DispatcherTimer();

		// Token: 0x04000121 RID: 289
		private readonly DispatcherTimer _clearEmptyPowerPointTimer = new DispatcherTimer();
	}
}
