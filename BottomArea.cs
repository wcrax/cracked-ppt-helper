using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x0200002C RID: 44
	public partial class BottomArea : UserControl, IComponentConnector
	{
		// Restored fields
		private bool _suppressExclusiveReentry;

		public static readonly DependencyProperty PenBrushProperty = DependencyProperty.Register("PenBrush", typeof(Brush), typeof(BottomArea), new PropertyMetadata(Brushes.Red));

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00005E72 File Offset: 0x00004072
		// (set) Token: 0x0600016B RID: 363 RVA: 0x00005E7A File Offset: 0x0000407A
		public bool IsInnerBoardWindowShowing { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00005E84 File Offset: 0x00004084
		public bool IsNeedStopMouseLineTrack
		{
			get
			{
				bool? isChecked = this.RubberCheckBox.IsChecked;
				bool flag = true;
				if (!(isChecked.GetValueOrDefault() == flag & isChecked != null))
				{
					isChecked = this.InnerBoardCheckBox.IsChecked;
					flag = true;
					if (!(isChecked.GetValueOrDefault() == flag & isChecked != null))
					{
						isChecked = this.SelectCheckBox.IsChecked;
						flag = false;
						return isChecked.GetValueOrDefault() == flag & isChecked != null;
					}
				}
				return true;
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00005EF7 File Offset: 0x000040F7
		public BottomArea()
		{
			this.InitializeComponent();
			base.DataContext = this;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00005F0C File Offset: 0x0000410C
		public void InitializeBottomArea()
		{
			this.RubberCheckBox.IsChecked = new bool?(false);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00005F1F File Offset: 0x0000411F
		private void Select_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetNone();
				this.UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
			}
			this.ApplyExclusiveMode(PpSlideShowPointerType.ppSlideShowPointerNone);
			this.SetPptPointerType(PpSlideShowPointerType.ppSlideShowPointerNone);
			FridayReporter.ReportEventImmediately(EventID.BottomRegionSelect);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00005F5C File Offset: 0x0000415C
		private void Pen_OnClick(object sender, RoutedEventArgs e)
		{
			this.PenClick();
			if (this.IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetPenToInkMode();
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00005F78 File Offset: 0x00004178
		private void Rubber_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetRubber();
				this.SetPptPointerType(PpSlideShowPointerType.ppSlideShowPointerEraser);
				this.UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerEraser);
			}
			else
			{
				this.ApplyExclusiveMode(PpSlideShowPointerType.ppSlideShowPointerEraser);
				this.SetPptPointerType(PpSlideShowPointerType.ppSlideShowPointerEraser);
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionRubber);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00005FEA File Offset: 0x000041EA
		private void Magnifier_Click(object sender, RoutedEventArgs e)
		{
			this.PlayHiddenAllAreaStoryBoard();
			FridayReporter.ReportEventImmediately(EventID.BottomRegionMagnifier);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00005FFC File Offset: 0x000041FC
		private void InnerBoard_Click(object sender, RoutedEventArgs e)
		{
			bool? isChecked = this.InnerBoardCheckBox.IsChecked;
			bool flag = true;
			if (isChecked.GetValueOrDefault() == flag & isChecked != null)
			{
				this.SelectCheckBox.IsChecked = new bool?(false);
				ViewHelper.InnerBoardWindow.Left = ViewHelper.MainWindow.Left;
				ViewHelper.InnerBoardWindow.Top = ViewHelper.MainWindow.Top;
				ViewHelper.InnerBoardWindow.Width = ViewHelper.MainWindow.Width;
				ViewHelper.InnerBoardWindow.Height = ViewHelper.MainWindow.Height;
				ViewHelper.InnerBoardWindow.ShowWindow();
				ViewHelper.MainWindow.LowMaskButton.Visibility = Visibility.Visible;
				FridayReporter.ReportEventImmediately(EventID.BottomRegionOpenInnerBoard);
				return;
			}
			ViewHelper.InnerBoardWindow.HiddenWindow();
			ViewHelper.MainWindow.LowMaskButton.Visibility = Visibility.Collapsed;
			FridayReporter.ReportEventImmediately(EventID.BottomRegionCloseInnerBoard);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000060D8 File Offset: 0x000042D8
		// Toolbar hide button: minimize the main UI to the small restore bar.
		private void Hide_OnClick(object sender, RoutedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				ViewHelper.MainWindow.MinimizeToBar();
			}, null);
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			ViewHelper.MainWindow.IsExiting = true;
			bool isShowing = ViewHelper.InnerBoardWindow.IsShowing;
			ViewHelper.InnerBoardWindow.HiddenAndClearInkCanvas();
			if (!isShowing)
			{
				PptOperator.Instance.Exit();
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionExit);
			DispatcherTask.Start(3000.0, new Action(AutoUpdater.Check));
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00006138 File Offset: 0x00004338
		private void RadioButton_OnClick(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = e.OriginalSource as RadioButton;
			if (radioButton == null)
			{
				return;
			}
			this.SetPenColor(radioButton.Background.ToString());
			this.ColorDiskPopup.IsOpen = false;
			if (!this.IsInnerBoardWindowShowing)
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000618C File Offset: 0x0000438C
		private void ColorDiskPopup_OnOpened(object sender, EventArgs e)
		{
			this.RedRadioButton.IsChecked = new bool?(false);
			this.BlueRadioButton.IsChecked = new bool?(false);
			this.BlackRadioButton.IsChecked = new bool?(false);
			Color pointerColor = PptOperator.Instance.PointerColor;
			if (pointerColor == Colors.Red)
			{
				this.RedRadioButton.IsChecked = new bool?(true);
			}
			if (pointerColor == Colors.Black)
			{
				this.BlackRadioButton.IsChecked = new bool?(true);
			}
			if (pointerColor.R == 0 && pointerColor.G == 133 && pointerColor.B == 205)
			{
				this.BlueRadioButton.IsChecked = new bool?(true);
			}
			Storyboard storyboard = base.Resources["ShowColorDiskStoryBoard"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00006268 File Offset: 0x00004468
		public void PenClick()
		{
			if (this.SelectedPenImage.Visibility == Visibility.Collapsed)
			{
				PptOperator.Instance.PointerType = PpSlideShowPointerType.ppSlideShowPointerPen;
				this.UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerPen);
				this.ApplyExclusiveMode(PpSlideShowPointerType.ppSlideShowPointerPen);
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				FridayReporter.ReportEventImmediately(EventID.BottomRegionPen);
				return;
			}
			this.ColorDiskPopup.IsOpen = true;
			FridayReporter.ReportEventImmediately(EventID.BottomRegionColorDisk);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000062E8 File Offset: 0x000044E8
		private void SetPenColor(string color)
		{
			byte r = (byte)Convert.ToInt32(color.Substring(3, 2), 16);
			byte g = (byte)Convert.ToInt32(color.Substring(5, 2), 16);
			byte b = (byte)Convert.ToInt32(color.Substring(7, 2), 16);
			int pointerColor = Convert.ToInt32(color.Substring(7, 2) + color.Substring(5, 2) + color.Substring(3, 2), 16);
			PptOperator.Instance.SetPointerColor(pointerColor);
			PptOperator.Instance.SetPointerColor(pointerColor);
			ViewHelper.InnerBoardWindow.SetPen(Color.FromRgb(r, g, b));
			this.PenBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
			BottomArea.PostPenColorEvent(r, g, b);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00006390 File Offset: 0x00004590
		private static void PostPenColorEvent(byte r, byte g, byte b)
		{
			if (r == 255 && g == 0 && b == 0)
			{
				FridayReporter.ReportEventImmediately(EventID.BottomRegionRedPen);
			}
			if (r == 0 && g == 133 && b == 205)
			{
				FridayReporter.ReportEventImmediately(EventID.BottomRegionBluePen);
			}
			if (r == 0 && g == 0 && b == 0)
			{
				FridayReporter.ReportEventImmediately(EventID.BottomRegionBlackPen);
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000063E5 File Offset: 0x000045E5
		private void SetPptPointerType(PpSlideShowPointerType type)
		{
			PptOperator.Instance.PointerType = type;
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00006404 File Offset: 0x00004604
		// Single source of truth for the select/pen/rubber mutual exclusion.
		// WPF CheckBoxes self-toggle on click, so every click path must re-assert the
		// exclusive state here instead of relying on ad-hoc IsChecked assignments.
		private void ApplyExclusiveMode(PpSlideShowPointerType type)
		{
			this.UpdateButtonState(type);
			this._suppressExclusiveReentry = true;
			try
			{
				if (type == PpSlideShowPointerType.ppSlideShowPointerPen)
				{
					this.SelectCheckBox.IsChecked = new bool?(false);
					this.RubberCheckBox.IsChecked = new bool?(false);
					return;
				}
				if (type == PpSlideShowPointerType.ppSlideShowPointerEraser)
				{
					this.SelectCheckBox.IsChecked = new bool?(false);
					this.RubberCheckBox.IsChecked = new bool?(true);
					return;
				}
				this.SelectCheckBox.IsChecked = new bool?(true);
				this.RubberCheckBox.IsChecked = new bool?(false);
			}
			finally
			{
				this._suppressExclusiveReentry = false;
			}
		}

		public void UpdateButtonState(PpSlideShowPointerType type)
		{
			if (type != PpSlideShowPointerType.ppSlideShowPointerPen)
			{
				if (type != PpSlideShowPointerType.ppSlideShowPointerEraser)
				{
					this.SelectCheckBox.IsChecked = new bool?(true);
					this.RubberCheckBox.IsChecked = new bool?(false);
					this.PenImage.Visibility = Visibility.Visible;
					this.SelectedPenImage.Visibility = Visibility.Collapsed;
				}
				else
				{
					this.SelectCheckBox.IsChecked = new bool?(false);
					this.RubberCheckBox.IsChecked = new bool?(true);
					this.PenImage.Visibility = Visibility.Visible;
					this.SelectedPenImage.Visibility = Visibility.Collapsed;
					if (PptAnalyzer.CurrentAnalyzeRate != AnalyzeRate.Highest)
					{
						PptAnalyzer.StopAnalyzeAllAsyn();
						return;
					}
				}
			}
			else
			{
				this.SelectCheckBox.IsChecked = new bool?(false);
				this.RubberCheckBox.IsChecked = new bool?(false);
				this.PenImage.Visibility = Visibility.Collapsed;
				this.SelectedPenImage.Visibility = Visibility.Visible;
				this.PenBrush = new SolidColorBrush(PptOperator.Instance.PointerColor);
				if (PptAnalyzer.CurrentAnalyzeRate != AnalyzeRate.Highest)
				{
					PptAnalyzer.StopAnalyzeAllAsyn();
					return;
				}
			}
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00006500 File Offset: 0x00004700
		public void PlayHiddenAllAreaStoryBoard()
		{
			ViewHelper.LeftArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			ViewHelper.RightArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			ViewHelper.LeftBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			ViewHelper.RightBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			double value = base.ActualHeight + ViewHelper.MainWindow.RootGrid.Margin.Bottom + 3.0;
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrame keyFrame = new EasingDoubleKeyFrame(value, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300.0)));
			doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
			Storyboard.SetTarget(doubleAnimationUsingKeyFrames, this.RootGrid);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)", new object[0]));
			Storyboard storyboard = new Storyboard();
			storyboard.Children = new TimelineCollection
			{
				doubleAnimationUsingKeyFrames
			};
			storyboard.Completed += delegate(object s, EventArgs e)
			{
				SyntacticSugar.TryCatchAction(delegate
				{
					base.Visibility = Visibility.Collapsed;
					ViewHelper.Magnifier = new Magnifier
					{
						Owner = ViewHelper.MainWindow
					};
					ViewHelper.Magnifier.ShowDialog();
					ViewHelper.Magnifier = null;
					this.PlayShowLeftAreaStoryBoard();
					ViewHelper.MainWindow.UpdatePageIndex();
					ViewHelper.LeftArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
					ViewHelper.RightArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
					ViewHelper.LeftBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
					ViewHelper.RightBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
				}, null);
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			};
			storyboard.Begin();
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000065E0 File Offset: 0x000047E0
		public void PlayShowLeftAreaStoryBoard()
		{
			base.Visibility = Visibility.Visible;
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrame keyFrame = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300.0)));
			doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
			Storyboard.SetTarget(doubleAnimationUsingKeyFrames, this.RootGrid);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)", new object[0]));
			new Storyboard
			{
				Children = new TimelineCollection
				{
					doubleAnimationUsingKeyFrames
				}
			}.Begin();
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00006667 File Offset: 0x00004867
		// (set) Token: 0x0600017F RID: 383 RVA: 0x00006679 File Offset: 0x00004879
		public Brush PenBrush
		{
			get
			{
				return (Brush)base.GetValue(BottomArea.PenBrushProperty);
			}
			set
			{
				base.SetValue(BottomArea.PenBrushProperty, value);
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00006687 File Offset: 0x00004887
		private void Share_OnClick(object sender, RoutedEventArgs e)
		{
			PptSharer.Share(PptOperator.Instance.PptFilePath);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00006698 File Offset: 0x00004898
		private void Recommendation_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.BottomRegionRecommendation);
			RecommendationWindow recommendationWindow = new RecommendationWindow();
			recommendationWindow.Owner = ViewHelper.MainWindow;
			recommendationWindow.Width = ViewHelper.MainWindow.Width;
			recommendationWindow.Height = ViewHelper.MainWindow.Height;
			ViewHelper.StopMainWindowUpdateTimerAndMouseGesture();
			recommendationWindow.ShowDialog();
			ViewHelper.BeginMainWindowUpdateTimerAndMouseGesture();
		}
	}

}
