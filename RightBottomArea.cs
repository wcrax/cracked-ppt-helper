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
	// Token: 0x0200003E RID: 62
	public partial class RightBottomArea : UserControl, IComponentConnector
	{
		// Token: 0x06000284 RID: 644 RVA: 0x0000B653 File Offset: 0x00009853
		public RightBottomArea()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000B661 File Offset: 0x00009861
		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.PreviousPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.RightRegionPreviousPage);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000B690 File Offset: 0x00009890
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
				ViewHelper.MainWindow.PlayStoryboard("ShowRightBottomSlideManagerViewStoryboard");
				return;
			}
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000B744 File Offset: 0x00009944
		public void Next_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.NextPage();
			this.PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.RightRegionNextPage);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000B770 File Offset: 0x00009970
		private void PlayHiddenStoryBoard()
		{
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightCenterButtonGridStoryboard();
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000B790 File Offset: 0x00009990
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
