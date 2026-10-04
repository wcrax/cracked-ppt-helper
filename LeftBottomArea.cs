using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x0200002E RID: 46
	public partial class LeftBottomArea : UserControl, IComponentConnector
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x00007164 File Offset: 0x00005364
		public LeftBottomArea()
		{
			this.InitializeComponent();
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00007172 File Offset: 0x00005372
		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.PreviousPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.LeftRegionPreviousPage);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x000071A0 File Offset: 0x000053A0
		private void Thumbnail_Click(object sender, RoutedEventArgs e)
		{
			if (PptVersionManager.IsWps)
			{
				return;
			}
			FridayReporter.ReportEventImmediately(EventID.LeftRegionThumbnail);
			int count = ViewHelper.MainWindow.LeftSlideListBox.Items.Count;
			if (count <= 0 || count != PptOperator.Instance.SlidesCount)
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				Log.WriteError(string.Format("ItemsCount  {0};  SlidesCount  {1}", count, PptOperator.Instance.SlidesCount));
				return;
			}
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenLeftCenterButtonGridStoryboard();
			if (!ViewHelper.MainWindow.IsLeftSlideManagerViewShowed)
			{
				ViewHelper.MainWindow.PlayStoryboard("ShowLeftBottomSlideManagerViewStoryboard");
				return;
			}
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00007254 File Offset: 0x00005454
		private void Next_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.NextPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.LeftRegionNextPage);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00007280 File Offset: 0x00005480
		private void PlayHiddenStoryBoard()
		{
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenLeftCenterButtonGridStoryboard();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000072A0 File Offset: 0x000054A0
		public void PlayHiddenOrShowLeftAreaStoryBoard(bool isHidden)
		{
			double value = base.ActualHeight + ViewHelper.MainWindow.RootGrid.Margin.Bottom + 3.0;
			if (!isHidden)
			{
				base.Visibility = Visibility.Visible;
				value = 0.0;
			}
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
				if (isHidden)
				{
					this.Visibility = Visibility.Collapsed;
				}
			};
			storyboard.Begin();
		}
	}

}
