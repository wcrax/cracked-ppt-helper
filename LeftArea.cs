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
	// Token: 0x0200002F RID: 47
	public partial class LeftArea : UserControl, IComponentConnector
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000745A File Offset: 0x0000565A
		public LeftArea()
		{
			this.InitializeComponent();
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00007468 File Offset: 0x00005668
		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.PreviousPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.LeftRegionPreviousPage);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00007494 File Offset: 0x00005694
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
				ViewHelper.MainWindow.PlayStoryboard("ShowLeftSlideManagerViewStoryboard");
				return;
			}
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00007548 File Offset: 0x00005748
		private void Next_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.NextPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.LeftRegionNextPage);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00007280 File Offset: 0x00005480
		private void PlayHiddenStoryBoard()
		{
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenLeftCenterButtonGridStoryboard();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00007574 File Offset: 0x00005774
		public void PlayHiddenOrShowLeftAreaStoryBoard(bool isHidden)
		{
			double value = -(base.ActualWidth + ViewHelper.MainWindow.RootGrid.Margin.Left + 3.0);
			if (!isHidden)
			{
				base.Visibility = Visibility.Visible;
				value = 0.0;
			}
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrame keyFrame = new EasingDoubleKeyFrame(value, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300.0)));
			doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
			Storyboard.SetTarget(doubleAnimationUsingKeyFrames, this.RootGrid);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)", new object[0]));
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
