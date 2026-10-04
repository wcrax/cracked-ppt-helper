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
	// Token: 0x0200003F RID: 63
	public partial class RightArea : UserControl, IComponentConnector
	{
		// Token: 0x0600028D RID: 653 RVA: 0x0000B94A File Offset: 0x00009B4A
		public RightArea()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000B958 File Offset: 0x00009B58
		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.PreviousPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.RightRegionPreviousPage);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000B984 File Offset: 0x00009B84
		private void Thumbnail_Click(object sender, RoutedEventArgs e)
		{
			if (PptVersionManager.IsWps)
			{
				return;
			}
			FridayReporter.ReportEventImmediately(EventID.RightRegionThumbnail);
			int count = ViewHelper.MainWindow.RightSlideListBox.Items.Count;
			if (count <= 0 || count != PptOperator.Instance.SlidesCount)
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				Log.WriteError(string.Format("ItemsCount  {0};  SlidesCount  {1}", count, PptOperator.Instance.SlidesCount));
				return;
			}
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightCenterButtonGridStoryboard();
			if (!ViewHelper.MainWindow.IsRightSlideManagerViewShowed)
			{
				ViewHelper.MainWindow.PlayStoryboard("ShowRightSlideManagerViewStoryboard");
				return;
			}
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000BA38 File Offset: 0x00009C38
		public void Next_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.NextPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.RightRegionNextPage);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000B770 File Offset: 0x00009970
		private void PlayHiddenStoryBoard()
		{
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightCenterButtonGridStoryboard();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000BA64 File Offset: 0x00009C64
		public void PlayHiddenOrShowLeftAreaStoryBoard(bool isHidden)
		{
			double value = base.ActualWidth + ViewHelper.MainWindow.RootGrid.Margin.Left + 3.0;
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
