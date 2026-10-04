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
	// Token: 0x0200003D RID: 61
	public partial class RecommendationWindow : Window, IComponentConnector
	{
		// Restored fields
		private const string MoreInfoWebsite = "https://e.seewo.com";

		private const string EducationInfoWebsite = "https://e.seewo.com";

		private const string DownloadWebsiteEasiNote5 = "https://easinote.seewo.com";

		// Token: 0x0600027A RID: 634 RVA: 0x0000B48B File Offset: 0x0000968B
		public RecommendationWindow()
		{
			this.InitializeComponent();
			this.VersionTextBlock.Text = App.Version;
			PptOperator.Instance.ShowEnd += this.Instance_ShowEnd;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00007FBF File Offset: 0x000061BF
		private void Instance_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(new Action(base.Close));
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00009414 File Offset: 0x00007614
		private void CloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000B4BF File Offset: 0x000096BF
		private void MoreInfoButton_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.RecommendationWindowMoreInfo);
			ExploreHelper.OpenLink(RecommendationWindow.MoreInfoWebsite);
			base.Close();
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000B4DB File Offset: 0x000096DB
		private void EducationInfoButton_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.RecommendationWindowEducationInfo);
			ExploreHelper.OpenLink(RecommendationWindow.EducationInfoWebsite);
			base.Close();
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000B4F7 File Offset: 0x000096F7
		private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.RecommendationWindowDownload);
			ExploreHelper.OpenLink(RecommendationWindow.DownloadWebsiteEasiNote5);
			base.Close();
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000B513 File Offset: 0x00009713
		private void BackgroundButton_OnClick(object sender, RoutedEventArgs e)
		{
			Storyboard storyboard = base.Resources["Storyboard.Shake"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}
	}

}
