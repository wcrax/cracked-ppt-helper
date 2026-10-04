using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Cvte.Windows;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x02000049 RID: 73
	public partial class MainWindow : Window, IComponentConnector
	{
		// Restored fields
		private Gesture _gesture;

		private DispatcherTimer _releaseTimer;

		private int _currentPageIndex;

		private bool _isAnalyzing;

		private string _prePptFilePath;

		// --- Minimize-to-bar feature ---
		private bool _isMinimizedToBar;
		private MinimizedBarWindow _minimizedBarWindow;

		private bool _isDraggingToolbar;

		private bool _toolbarDragPending;

		private Point _toolbarDragStartPoint;

		private Thickness _toolbarDragStartMargin;


		public bool IsMinimizedToBar
		{
			get
			{
				return this._isMinimizedToBar;
			}
		}

		public static readonly DependencyProperty SlideItemsProperty = DependencyProperty.Register("SlideItems", typeof(ObservableCollection<SlideItem>), typeof(MainWindow), new PropertyMetadata(null));

		public const int TimerSpan = 500;

		public const int ReleaseTimerSpan = 10000;

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000C07F File Offset: 0x0000A27F
		private bool IsTransparent
		{
			get
			{
				return base.Opacity < 0.1;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0000C092 File Offset: 0x0000A292
		// (set) Token: 0x060002EC RID: 748 RVA: 0x0000C09A File Offset: 0x0000A29A
		public bool IsExiting { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0000C0A3 File Offset: 0x0000A2A3
		// (set) Token: 0x060002EE RID: 750 RVA: 0x0000C0AB File Offset: 0x0000A2AB
		public DispatcherTimer UpdateTimer { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000C0B4 File Offset: 0x0000A2B4
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x0000C0BC File Offset: 0x0000A2BC
		public bool IsNeedUpdateTimer { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000C0C5 File Offset: 0x0000A2C5
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x0000C0CD File Offset: 0x0000A2CD
		public bool IsLeftSlideManagerViewShowed { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000C0D6 File Offset: 0x0000A2D6
		// (set) Token: 0x060002F4 RID: 756 RVA: 0x0000C0DE File Offset: 0x0000A2DE
		public bool IsRightSlideManagerViewShowed { get; set; }

		// Token: 0x060002F5 RID: 757 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
		public MainWindow()
		{
			this.InitializeComponent();
			base.DataContext = this;
			this.UpdateTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds((double)MainWindow.TimerSpan)
			};
			this.UpdateTimer.Tick += this.UpdateTimerTick;
			this._releaseTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds((double)MainWindow.ReleaseTimerSpan)
			};
			this._releaseTimer.Tick += this.ReleaseTimerTick;
			base.Loaded += this.MainWindow_Loaded;
			base.PreviewKeyDown += this.MainWindow_KeyDown;
			base.SizeChanged += this.MainWindow_SizeChanged;
			base.LocationChanged += this.MainWindow_LocationChanged;
			base.ManipulationBoundaryFeedback += delegate(object s, ManipulationBoundaryFeedbackEventArgs e)
			{
				e.Handled = true;
			};
			this.LeftSlideListBox.ManipulationBoundaryFeedback += delegate(object s, ManipulationBoundaryFeedbackEventArgs e)
			{
				e.Handled = true;
			};
			this.RightSlideListBox.ManipulationBoundaryFeedback += delegate(object s, ManipulationBoundaryFeedbackEventArgs e)
			{
				e.Handled = true;
			};
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000C238 File Offset: 0x0000A438
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (PptVersionManager.IsOffice2016OrLater)
			{
				this.LeftBottomButtonGrid.Children.Add(ViewHelper.LeftBottomArea);
				this.RightBottomButtonGrid.Children.Add(ViewHelper.RightBottomArea);
				this.LeftSlideManagerViewGrid.RenderTransformOrigin = new Point(0.5, 1.0);
				this.RightSlideManagerViewGrid.RenderTransformOrigin = new Point(0.5, 1.0);
				this.LeftSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Bottom;
				this.RightSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Bottom;
			}
			else
			{
				this.LeftCenterButtonGrid.Children.Add(ViewHelper.LeftArea);
				this.RightCenterButtonGrid.Children.Add(ViewHelper.RightArea);
				this.LeftSlideManagerViewGrid.RenderTransformOrigin = new Point(0.0, 0.5);
				this.RightSlideManagerViewGrid.RenderTransformOrigin = new Point(1.0, 0.5);
				this.LeftSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Center;
				this.RightSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Center;
			}
			this.BottomCenterButtonGrid.Children.Add(ViewHelper.BottomArea);
			base.Width = SystemParameters.PrimaryScreenWidth;
			base.Height = SystemParameters.PrimaryScreenHeight;
			this.SubscribeSelectionChangedEvent();
			this._gesture = new Gesture();
			this._gesture.ChangeGestureScope();
			WindowsApi.SetPopupParameter();
			PptOperator.Instance.ShowBegin += this._pptOperatorShowBegin;
			PptOperator.Instance.ShowEnd += this._pptOperatorShowEnd;
			PptOperator.Instance.NextSlide += this.PptOperatorNextSlide;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000C3F3 File Offset: 0x0000A5F3
		private void MainWindow_LocationChanged(object sender, EventArgs e)
		{
			this.ResetPreWindowRect();
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000C3FB File Offset: 0x0000A5FB
		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.AdjustFontSizeAndStyle();
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000C403 File Offset: 0x0000A603
		public void _pptOperatorShowBegin(object sender, EventArgs e)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				WindowStateHelper.UpdateWindowRect(this);
				Task.Factory.StartNew(delegate()
				{
					McuService.ForbiddenAndroidPen();
				});
				SyntacticSugar.CurrentDispatcherInvoke(delegate
				{
					DispatcherTimer releaseTimer = this._releaseTimer;
					if (releaseTimer != null)
					{
						releaseTimer.Stop();
					}
					base.Show();
					base.Activate();
					this.AdjustFontSizeAndStyle();
					BottomArea bottomArea = ViewHelper.BottomArea;
					if (bottomArea != null)
					{
						bottomArea.InitializeBottomArea();
					}
					this._currentPageIndex = PptOperator.Instance.CurrentShowPosition;
					this.SetPageIndex(this._currentPageIndex);
					this.SetLeftRightOpacity();
					this.IsExiting = false;
					this.ResetMinimizedBarState();
					base.Opacity = 1.0;
				});
				SyntacticSugar.CurrentDispatcherBeginInvoke(new Action(this.AnalyzePpt));
			}, null);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000C417 File Offset: 0x0000A617
		private void _pptOperatorShowEnd(object sender, EventArgs e)
		{
			this.ShowEnd();
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000C41F File Offset: 0x0000A61F
		private void PptOperatorNextSlide(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(delegate()
			{
				if (this.IsTransparent)
				{
					return;
				}
				this.UpdatePageIndex();
			});
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000C432 File Offset: 0x0000A632
		private void HighMaskButton_Click(object sender, RoutedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				this.HighMaskButton.Visibility = Visibility.Collapsed;
				this.PlayHiddenLeftSlideManagerViewStoryboard();
				this.PlayHiddenRightSlideManagerViewStoryboard();
				this.IsLeftSlideManagerViewShowed = false;
				this.IsRightSlideManagerViewShowed = false;
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				FridayReporter.ReportEventImmediately(EventID.CenterRegionHiddenThumbnailListBox);
			}, null);
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0000C446 File Offset: 0x0000A646
		private void LowMaskButton_Click(object sender, MouseButtonEventArgs e)
		{
			ViewHelper.InnerBoardWindow.HiddenWindow();
			FridayReporter.ReportEventImmediately(EventID.CenterRegionHiddenInnerBoard);
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000C45C File Offset: 0x0000A65C
		private void ShowLeftSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			this.IsLeftSlideManagerViewShowed = true;
			this.HighMaskButton.Visibility = Visibility.Visible;
			Storyboard storyboard = base.Resources["ShowLeftCenterButtonGridStoryBoard"] as Storyboard;
			if (storyboard != null)
			{
				storyboard.Begin();
			}
			SyntacticSugar.CurrentDispatcherBeginInvoke(DispatcherPriority.Background, new Action(this.UpdatePageIndex));
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.CurrentShowPosition = this._currentPageIndex;
			PptAnalyzer.StartAnalyzeAllAsync(AnalyzeRate.Highest);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000C4D0 File Offset: 0x0000A6D0
		private void ShowRightSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			this.IsRightSlideManagerViewShowed = true;
			this.HighMaskButton.Visibility = Visibility.Visible;
			Storyboard storyboard = base.Resources["ShowRightCenterButtonGridStoryBoard"] as Storyboard;
			if (storyboard != null)
			{
				storyboard.Begin();
			}
			SyntacticSugar.CurrentDispatcherBeginInvoke(DispatcherPriority.Background, new Action(this.UpdatePageIndex));
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.CurrentShowPosition = this._currentPageIndex;
			PptAnalyzer.StartAnalyzeAllAsync(AnalyzeRate.Highest);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000C543 File Offset: 0x0000A743
		private void HiddenLeftSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			this.IsLeftSlideManagerViewShowed = false;
			this.HighMaskButton.Visibility = Visibility.Collapsed;
			this.PlayHiddenLeftCenterButtonGridStoryboard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.StopAnalyzeAllAsyn();
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000C573 File Offset: 0x0000A773
		private void HiddenRightSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			this.IsRightSlideManagerViewShowed = false;
			this.HighMaskButton.Visibility = Visibility.Collapsed;
			this.PlayHiddenRightCenterButtonGridStoryboard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.StopAnalyzeAllAsyn();
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000C5A3 File Offset: 0x0000A7A3
		public void PlayHiddenLeftSlideManagerViewStoryboard()
		{
			if (!this.IsLeftSlideManagerViewShowed)
			{
				return;
			}
			this.PlayStoryboard(PptVersionManager.IsOffice2016OrLater ? "HiddenLeftBottomSlideManagerViewStoryboard" : "HiddenLeftSlideManagerViewStoryboard");
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000C5C7 File Offset: 0x0000A7C7
		public void PlayHiddenRightSlideManagerViewStoryboard()
		{
			if (!this.IsRightSlideManagerViewShowed)
			{
				return;
			}
			this.PlayStoryboard(PptVersionManager.IsOffice2016OrLater ? "HiddenRightBottomSlideManagerViewStoryboard" : "HiddenRightSlideManagerViewStoryboard");
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000C5EB File Offset: 0x0000A7EB
		public void PlayHiddenLeftCenterButtonGridStoryboard()
		{
			Storyboard storyboard = base.Resources["HiddenLeftCenterButtonGridStoryBoard"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0000C60C File Offset: 0x0000A80C
		public void PlayHiddenRightCenterButtonGridStoryboard()
		{
			Storyboard storyboard = base.Resources["HiddenRightCenterButtonGridStoryBoard"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0000C630 File Offset: 0x0000A830
		public void PlayStoryboard(string storyBoardName)
		{
			if (base.Width > 3800.0)
			{
				Storyboard storyboard = base.Resources[storyBoardName + "3800"] as Storyboard;
				if (storyboard != null)
				{
					storyboard.Begin();
				}
			}
			if (base.Width > 1440.0 && base.Width <= 3800.0)
			{
				Storyboard storyboard2 = base.Resources[storyBoardName + "1440"] as Storyboard;
				if (storyboard2 != null)
				{
					storyboard2.Begin();
				}
			}
			if (base.Width > 0.0 && base.Width <= 1440.0)
			{
				Storyboard storyboard3 = base.Resources[storyBoardName + "0"] as Storyboard;
				if (storyboard3 == null)
				{
					return;
				}
				storyboard3.Begin();
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0000C703 File Offset: 0x0000A903
		public void UpdatePageIndex()
		{
			if (this._isAnalyzing)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(delegate
			{
				this._currentPageIndex = PptOperator.Instance.CurrentShowPosition;
				if (this._currentPageIndex > PptOperator.Instance.SlidesCount)
				{
					return;
				}
				if (this._currentPageIndex <= 0)
				{
					this._currentPageIndex = 1;
				}
				this.SetPageIndex(this._currentPageIndex);
				this.UpdateSlideListBoxIndex(this._currentPageIndex);
			}, null);
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000C720 File Offset: 0x0000A920
		private void UpdateButtonState()
		{
			PptOperator.Instance.UpdatePointerColor();
			ViewHelper.BottomArea.IsInnerBoardWindowShowing = ViewHelper.InnerBoardWindow.IsShowing;
			if (ViewHelper.InnerBoardWindow.IsShowing)
			{
				return;
			}
			ViewHelper.BottomArea.UpdateButtonState(PptOperator.Instance.PointerType);
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0000C76C File Offset: 0x0000A96C
		private void UpdateWindowOpacity()
		{
			if (this._isAnalyzing)
			{
				return;
			}
			bool pptShowingState = WindowStateHelper.GetPptShowingState(this);
			if (this.IsTransparent && pptShowingState)
			{
				this.UpdatePageIndex();
			}
			base.Opacity = (double)(pptShowingState ? 1 : 0);
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0000C7A8 File Offset: 0x0000A9A8
		private void UpdateSlideListBoxIndex(int index)
		{
			this.CancelSubscribeSelectionChangedEvent();
			int num = index - 1;
			if (this.LeftSlideListBox.SelectedIndex != num)
			{
				this.LeftSlideListBox.SelectedIndex = num;
				this.LeftSlideListBox.ScrollIntoView(this.LeftSlideListBox.SelectedItem);
				PptAnalyzer.CurrentShowPosition = index;
			}
			if (this.RightSlideListBox.SelectedIndex != num)
			{
				this.RightSlideListBox.SelectedIndex = num;
				this.RightSlideListBox.ScrollIntoView(this.RightSlideListBox.SelectedItem);
			}
			this.SubscribeSelectionChangedEvent();
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000C82C File Offset: 0x0000AA2C
		private void UpdateGestureAndMouseLineTrackState()
		{
			if (this.IsTransparent || ViewHelper.MainWindow.IsLeftSlideManagerViewShowed || ViewHelper.MainWindow.IsRightSlideManagerViewShowed || ViewHelper.BottomArea.IsNeedStopMouseLineTrack)
			{
				MouseHelper.Instance.Stop();
				return;
			}
			MouseHelper.Instance.Start();
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0000C87C File Offset: 0x0000AA7C
		public void HiddenWindow()
		{
			base.Width = 1.0;
			base.Height = 1.0;
			WindowStateHelper.PreWindowRect.Bottom = 1;
			DispatcherTask.Start(100.0, new Action(base.Hide));
		}

		// Hide the main UI and show the small restore bar instead.
		public void MinimizeToBar()
		{
			if (this._isMinimizedToBar)
			{
				return;
			}
			this._isMinimizedToBar = true;
			this.PlayHiddenLeftSlideManagerViewStoryboard();
			this.PlayHiddenRightSlideManagerViewStoryboard();
			this.IsLeftSlideManagerViewShowed = false;
			this.IsRightSlideManagerViewShowed = false;
			this.RootGrid.Visibility = Visibility.Collapsed;
			base.Opacity = 0.0;
			MouseHelper.Instance.Stop();
			this.GetMinimizedBarWindow().ShowBar();
		}

		// Restore the main UI from the minimized bar.
		public void RestoreFromMinimizedBar()
		{
			if (!this._isMinimizedToBar)
			{
				return;
			}
			this._isMinimizedToBar = false;
			this.GetMinimizedBarWindow().HideBar();
			this.RootGrid.Visibility = Visibility.Visible;
			if (!base.IsVisible)
			{
				base.Show();
			}
			base.Opacity = 1.0;
			WindowStateHelper.MakeAppAbovePpt(this);
			base.Activate();
		}

		private MinimizedBarWindow GetMinimizedBarWindow()
		{
			if (this._minimizedBarWindow == null)
			{
				this._minimizedBarWindow = new MinimizedBarWindow
				{
					Owner = this
				};
			}
			return this._minimizedBarWindow;
		}

		// Called when a PPT show starts/ends so the feature resets to a clean state.
		public void ResetMinimizedBarState()
		{
			this._isMinimizedToBar = false;
			if (this._minimizedBarWindow != null)
			{
				this._minimizedBarWindow.HideBar();
			}
			this.RootGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000C8CD File Offset: 0x0000AACD
		private void AnalyzePpt()
		{
			if (this._isAnalyzing)
			{
				return;
			}
			this._isAnalyzing = true;
			SyntacticSugar.TryCatchAction(delegate
			{
				string pptFilePath = PptOperator.Instance.PptFilePath;
				List<string> listBoxItem;
				bool flag = PptAnalyzer.FindAlreadyAnalyzedFiles(pptFilePath, out listBoxItem);
				if ((this.SlideItems != null && pptFilePath == this._prePptFilePath && flag) || PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowNamedSlideShow)
				{
					this.DoAfterAnalyzePpt();
					return;
				}
				if (flag)
				{
					this.SetListBoxItem(listBoxItem);
					this.DoAfterAnalyzePpt();
				}
				else
				{
					LoadingScreen<LoadingView>.Current.Show(new Rect
					{
						X = base.Left,
						Y = base.Top,
						Width = base.Width,
						Height = base.Height
					}, true);
					LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
					{
						loadingView.Start();
						loadingView.CancelAction = delegate()
						{
							Restart.PPTService();
							Environment.Exit(0);
						};
					});
					LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
					{
						loadingView.Progress = 100;
					});
					this.InitListBoxItem();
					LoadingScreen<LoadingView>.Current.UsingLoadingThread(delegate(LoadingView loadingView)
					{
						loadingView.PushFrame();
						LoadingScreen<LoadingView>.Current.Hide();
						SyntacticSugar.CurrentDispatcherBeginInvoke(new Action(this.DoAfterAnalyzePpt));
					});
				}
				this._prePptFilePath = pptFilePath;
			}, null);
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000C8F1 File Offset: 0x0000AAF1
		private void ShowEnd()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				SyntacticSugar.CurrentDispatcherBeginInvoke(delegate()
				{
					if (!ConfigHelper.Instance.IsNeedStart() && this.IsTransparent)
					{
						return;
					}
					base.Opacity = 0.0;
					this.UpdateTimer.Stop();
					MouseHelper.Instance.Stop();
					ViewHelper.BottomArea.UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
					ViewHelper.InnerBoardWindow.HiddenAndClearInkCanvas();
					this.PlayHiddenLeftSlideManagerViewStoryboard();
					this.PlayHiddenRightSlideManagerViewStoryboard();
					this._releaseTimer.Stop();
					this._releaseTimer.Start();
					this.IsExiting = false;
					this.ResetMinimizedBarState();
					Task.Factory.StartNew(delegate()
					{
						McuService.OpenAndroidPen();
					});
					FridayReporter.ReportEventImmediately(EventID.PptShowEnd);
				});
			}, null);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000C908 File Offset: 0x0000AB08
		private void ResetPreWindowRect()
		{
			WindowStateHelper.PreWindowRect = new WindowRect
			{
				Left = (int)base.Left,
				Top = (int)base.Top,
				Right = (int)(base.Left + base.Width),
				Bottom = (int)(base.Top + base.Height)
			};
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000C96C File Offset: 0x0000AB6C
		private void AdjustFontSizeAndStyle()
		{
			double fontSize = 13.0;
			string key = "Style.ListBox1440";
			if (base.Width > 3800.0)
			{
				fontSize = (double)((PptOperator.Instance.SlidesCount >= 100) ? 20 : 30);
				key = "Style.ListBox3800";
			}
			if (base.Width > 1440.0 && base.Width <= 3800.0)
			{
				fontSize = (double)((PptOperator.Instance.SlidesCount >= 100) ? 11 : 13);
				key = "Style.ListBox1440";
			}
			if (base.Width > 0.0 && base.Width <= 1400.0)
			{
				fontSize = (double)((PptOperator.Instance.SlidesCount >= 100) ? 7 : 9);
				key = "Style.ListBox0";
			}
			ViewHelper.LeftArea.LeftSlideIndexTextBlock.FontSize = fontSize;
			ViewHelper.RightArea.RightSlideIndexTextBlock.FontSize = fontSize;
			ViewHelper.LeftBottomArea.LeftSlideIndexTextBlock.FontSize = fontSize;
			ViewHelper.RightBottomArea.RightSlideIndexTextBlock.FontSize = fontSize;
			this.LeftSlideListBox.Style = (base.Resources[key] as Style);
			this.RightSlideListBox.Style = (base.Resources[key] as Style);
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0000CAA9 File Offset: 0x0000ACA9
		private void DoAfterAnalyzePpt()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				this.UpdateTimer.Start();
				this._isAnalyzing = false;
				MouseHelper.Instance.Start();
				this.PlayHiddenLeftCenterButtonGridStoryboard();
				this.PlayHiddenRightCenterButtonGridStoryboard();
				FridayReporter.ReportEventImmediately(EventID.PptShowBegin);
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			}, null);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000CABD File Offset: 0x0000ACBD
		private void SubscribeSelectionChangedEvent()
		{
			this.LeftSlideListBox.SelectionChanged += this.SlideListBox_SelectionChanged;
			this.RightSlideListBox.SelectionChanged += this.SlideListBox_SelectionChanged;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000CAED File Offset: 0x0000ACED
		private void CancelSubscribeSelectionChangedEvent()
		{
			this.LeftSlideListBox.SelectionChanged -= this.SlideListBox_SelectionChanged;
			this.RightSlideListBox.SelectionChanged -= this.SlideListBox_SelectionChanged;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0000CB20 File Offset: 0x0000AD20
		private void SetLeftRightOpacity()
		{
			ViewHelper.LeftArea.Opacity = 1.0;
			ViewHelper.RightArea.Opacity = 1.0;
			ViewHelper.LeftArea.IsHitTestVisible = true;
			ViewHelper.RightArea.IsHitTestVisible = true;
			ViewHelper.LeftBottomArea.Opacity = 1.0;
			ViewHelper.RightBottomArea.Opacity = 1.0;
			ViewHelper.LeftBottomArea.IsHitTestVisible = true;
			ViewHelper.RightBottomArea.IsHitTestVisible = true;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000CBA8 File Offset: 0x0000ADA8
		private void SetPageIndex(int index)
		{
			if (index <= 0)
			{
				index = 1;
			}
			if (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowSlideRange)
			{
				index += PptOperator.Instance.StartingSlide - 1;
			}
			string text = (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowNamedSlideShow) ? string.Format("{0}", index) : string.Format("{0}/{1}", index, PptOperator.Instance.SlidesCount);
			ViewHelper.LeftArea.LeftSlideIndexTextBlock.Text = text;
			ViewHelper.RightArea.RightSlideIndexTextBlock.Text = text;
			ViewHelper.LeftBottomArea.LeftSlideIndexTextBlock.Text = text;
			ViewHelper.RightBottomArea.RightSlideIndexTextBlock.Text = text;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000CC58 File Offset: 0x0000AE58
		private void InitListBoxItem()
		{
			string[] array = new string[PptOperator.Instance.SlidesCount];
			foreach (KeyValuePair<int, string> keyValuePair in PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary)
			{
				array[keyValuePair.Key - 1] = keyValuePair.Value;
			}
			this.SetListBoxItem(array.ToList<string>());
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000CCD4 File Offset: 0x0000AED4
		private void SetListBoxItem(List<string> files)
		{
			if (files == null || !files.Any<string>())
			{
				return;
			}
			this.CancelSubscribeSelectionChangedEvent();
			this.SlideItems = new ObservableCollection<SlideItem>();
			if (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowAll)
			{
				int num = 1;
				foreach (string filePath in files)
				{
					this.CreateSlideItem(num++, filePath);
				}
			}
			if (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowSlideRange)
			{
				for (int i = PptOperator.Instance.StartingSlide; i <= PptOperator.Instance.EndingSlide; i++)
				{
					this.CreateSlideItem(i, files[i - 1]);
				}
			}
			this.SubscribeSelectionChangedEvent();
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000CD94 File Offset: 0x0000AF94
		public void UpdateListBoxItem(List<string> files)
		{
			if (this.SlideItems == null)
			{
				return;
			}
			for (int i = 0; i < this.SlideItems.Count; i++)
			{
				if (files[i] != null && !(this.SlideItems[i].IsPptThumbnail == "True"))
				{
					BitmapImage bitmapImage = new BitmapImage(new Uri(files[i]));
					this.SlideItems[i].BitmapImage = bitmapImage;
					this.SlideItems[i].IsPptThumbnail = "True";
				}
			}
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000CE20 File Offset: 0x0000B020
		private void CreateSlideItem(int index, string filePath)
		{
			bool flag = !string.IsNullOrEmpty(filePath);
			SlideItem item = new SlideItem
			{
				Index = index,
				IsPptThumbnail = flag.ToString(),
				BitmapImage = (flag ? new BitmapImage(new Uri(filePath)) : null)
			};
			this.SlideItems.Add(item);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000CE74 File Offset: 0x0000B074
		private void UpdateTimerTick(object sender, EventArgs e)
		{
			if (this.IsExiting || this._isMinimizedToBar || this._isDraggingToolbar || !this.IsNeedUpdateTimer || !PptOperator.Instance.IsShowing)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(delegate
			{
				if (!this.IsNeedUpdateTimer)
				{
					return;
				}
				this.UpdateButtonState();
				if (!this.IsNeedUpdateTimer)
				{
					return;
				}
				this.UpdateWindowOpacity();
				this.UpdateGestureAndMouseLineTrackState();
				WindowStateHelper.UpdateWindowRect(this);
				if (base.Opacity > 0.0)
				{
					WindowStateHelper.MakeAppAbovePpt(this);
				}
			}, null);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000CEA8 File Offset: 0x0000B0A8
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				PptOperator.Instance.Exit();
			}
			if (e.Key == Key.Prior || e.Key == Key.Left || e.Key == Key.Up)
			{
				PptOperator.Instance.PreviousPage();
			}
			if (e.Key == Key.Next || e.Key == Key.Right || e.Key == Key.Down)
			{
				PptOperator.Instance.NextPage();
			}
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000CF19 File Offset: 0x0000B119
		private void SlideListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				ListBox listBox = sender as ListBox;
				if (listBox == null)
				{
					return;
				}
				int num = listBox.SelectedIndex + 1;
				this.SetPageIndex(num);
				PptOperator.Instance.GotoSlide(num);
				FridayReporter.ReportEventImmediately((listBox.Name == "LeftSlideListBox") ? EventID.LeftRegionThumbnailListBoxItem : EventID.RightRegionThumbnailListBoxItem);
			}, null);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000CF3F File Offset: 0x0000B13F
		private void ReleaseTimerTick(object sender, EventArgs e)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				this._releaseTimer.Stop();
				BitmapImage bitmapImage = new BitmapImage(new Uri(ReleaseMemoryImageHelper.GetImageFilePath()));
				if (this.SlideItems != null && this.SlideItems.Any<SlideItem>())
				{
					foreach (SlideItem slideItem in this.SlideItems)
					{
						slideItem.BitmapImage = bitmapImage;
					}
				}
				this.CancelSubscribeSelectionChangedEvent();
				this.SlideItems = null;
				this.SubscribeSelectionChangedEvent();
				this.HiddenWindow();
				GarbageCollection.Collect();
			}, null);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000CF53 File Offset: 0x0000B153
		// --- Toolbar dragging (drag anywhere on the toolbar background; buttons still work) ---
				// Press anywhere in the toolbar arms a pending drag; it becomes a real drag only after the
		// pointer moves past the threshold. Below threshold + release = click; controls handle it
		// normally because we never capture the mouse for a stationary press.
		private const double ToolbarDragThreshold = 8.0;

		// Any mouse release anywhere (popup, button-captured, etc.) must clear the pending drag,
		// otherwise a stale pending + a later >threshold move teleports the toolbar off-screen.
		private void ToolbarGrid_LostMouseCapture(object sender, MouseEventArgs e)
		{
			this._toolbarDragPending = false;
			this._isDraggingToolbar = false;
		}

		private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this._toolbarDragPending = false;
			if (this._isDraggingToolbar)
			{
				this._isDraggingToolbar = false;
				Grid toolbarGrid = this.BottomCenterButtonGrid;
				if (toolbarGrid != null && toolbarGrid.IsMouseCaptured)
				{
					toolbarGrid.ReleaseMouseCapture();
				}
				this.ClampToolbarMargin();
				e.Handled = true;
			}
		}

		// Tunneling move: keep dragging alive even when the press was on a button that captured
		// the mouse (its Up would never reach the toolbar grid).
		private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (!this._toolbarDragPending || e.MouseDevice.LeftButton != MouseButtonState.Pressed)
			{
				return;
			}
			Grid toolbarGrid = this.BottomCenterButtonGrid;
			if (toolbarGrid == null)
			{
				return;
			}
			Point position = e.GetPosition(null);
			double offsetX = position.X - this._toolbarDragStartPoint.X;
			double offsetY = position.Y - this._toolbarDragStartPoint.Y;
			if (!this._isDraggingToolbar)
			{
				if (Math.Abs(offsetX) < ToolbarDragThreshold && Math.Abs(offsetY) < ToolbarDragThreshold)
				{
					return;
				}
				this._isDraggingToolbar = true;
				toolbarGrid.CaptureMouse();
			}
			toolbarGrid.Margin = new Thickness(this._toolbarDragStartMargin.Left + offsetX, this._toolbarDragStartMargin.Top + offsetY, this._toolbarDragStartMargin.Right - offsetX, this._toolbarDragStartMargin.Bottom - offsetY);
			e.Handled = true;
		}

		// Keep the toolbar at least partially on screen.
		private void ClampToolbarMargin()
		{
			Grid toolbarGrid = this.BottomCenterButtonGrid;
			if (toolbarGrid == null)
			{
				return;
			}
			Thickness margin = toolbarGrid.Margin;
			double maxOffset = base.ActualWidth * 0.45;
			double maxOffsetY = base.ActualHeight * 0.9;
			if (margin.Left > maxOffset)
			{
				margin.Left = maxOffset;
			}
			if (margin.Left < -maxOffset)
			{
				margin.Left = -maxOffset;
			}
			if (margin.Right > maxOffset)
			{
				margin.Right = maxOffset;
			}
			if (margin.Right < -maxOffset)
			{
				margin.Right = -maxOffset;
			}
			if (margin.Top > maxOffsetY)
			{
				margin.Top = maxOffsetY;
			}
			if (margin.Top < -maxOffsetY)
			{
				margin.Top = -maxOffsetY;
			}
			if (margin.Bottom > maxOffsetY)
			{
				margin.Bottom = maxOffsetY;
			}
			if (margin.Bottom < -maxOffsetY)
			{
				margin.Bottom = -maxOffsetY;
			}
			toolbarGrid.Margin = margin;
		}

		private void ToolbarGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this._isMinimizedToBar)
			{
				return;
			}
			Grid toolbarGrid = sender as Grid;
			if (toolbarGrid == null)
			{
				return;
			}
			// Arm the pending drag only for presses on the drag handle or the toolbar background.
			// Presses on buttons must be left completely alone: stealing the capture on a jittery
			// press eats their Click.
			DependencyObject current = e.OriginalSource as DependencyObject;
			bool onHandle = false;
			while (current != null && current != toolbarGrid)
			{
				if (current is ButtonBase || current is ToggleButton || current is Popup)
				{
					return;
				}
				FrameworkElement frameworkElement = current as FrameworkElement;
				if (frameworkElement != null)
				{
					string tag = frameworkElement.Tag as string;
					if (tag == "Interactive")
					{
						return;
					}
					if (tag == "DragHandle")
					{
						onHandle = true;
					}
				}
				current = (current is Visual || current is System.Windows.Media.Media3D.Visual3D) ? System.Windows.Media.VisualTreeHelper.GetParent(current) : LogicalTreeHelper.GetParent(current);
			}
			if (!onHandle)
			{
				return;
			}
			this._isDraggingToolbar = false;
			this._toolbarDragPending = true;
			this._toolbarDragStartPoint = e.GetPosition(null);
			this._toolbarDragStartMargin = toolbarGrid.Margin;
		}

		private void QrGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.QrImage.Visibility = Visibility.Collapsed;
			this.QrGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000CF6D File Offset: 0x0000B16D
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000CF7F File Offset: 0x0000B17F
		public ObservableCollection<SlideItem> SlideItems
		{
			get
			{
				return (ObservableCollection<SlideItem>)base.GetValue(MainWindow.SlideItemsProperty);
			}
			set
			{
				base.SetValue(MainWindow.SlideItemsProperty, value);
			}
		}
	}

}
