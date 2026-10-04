using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace PPTService
{
	// Token: 0x0200003C RID: 60
	public partial class QrWindow : Window, IComponentConnector
	{
		// Token: 0x0600026E RID: 622 RVA: 0x0000B0F0 File Offset: 0x000092F0
		public QrWindow(string qrFilePath)
		{
			this.InitializeComponent();
			base.Loaded += this.QrWindow_Loaded;
			base.Unloaded += this.QrWindow_Unloaded;
			base.Top = ViewHelper.MainWindow.Top;
			base.Left = ViewHelper.MainWindow.Left;
			base.Width = ViewHelper.MainWindow.Width;
			base.Height = ViewHelper.MainWindow.Height;
			this.CloseButton.Click += delegate(object s, RoutedEventArgs e)
			{
				base.Close();
			};
			this.QrImage.Source = new BitmapImage(new Uri(qrFilePath));
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000B19A File Offset: 0x0000939A
		private void QrWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.AdjustImageSize();
			PptOperator.Instance.ShowEnd += this.PPT_ShowEnd;
			Storyboard storyboard = base.Resources["StoryBoard.ShowQrImage"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000B1D7 File Offset: 0x000093D7
		private void QrWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			}, null);
			PptOperator.Instance.ShowEnd -= this.PPT_ShowEnd;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00007FBF File Offset: 0x000061BF
		private void PPT_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(new Action(base.Close));
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000B214 File Offset: 0x00009414
		private void QrBorder_OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
		{
			e.Mode = ManipulationModes.Scale;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000B220 File Offset: 0x00009420
		private void QrBorder_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			this.Enlarge(e.DeltaManipulation.Scale.X);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000B248 File Offset: 0x00009448
		private void QrBorder_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scaleX = 1.05;
			if (e.Delta < 0)
			{
				scaleX = 0.95;
			}
			this.Enlarge(scaleX);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000B27C File Offset: 0x0000947C
		private void Enlarge(double scaleX)
		{
			double width = this.QrBorder.Width * scaleX;
			double num = this.QrBorder.Height * scaleX;
			if (num <= 300.0 || num > ViewHelper.MainWindow.Height)
			{
				return;
			}
			this.QrBorder.Width = width;
			this.QrBorder.Height = num;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000B2D8 File Offset: 0x000094D8
		private void AdjustImageSize()
		{
			if (base.Width > 3800.0)
			{
				this.QrBorder.Width = 1200.0;
				this.QrBorder.Height = 1200.0;
			}
			if (base.Width > 1440.0 && base.Width <= 3800.0)
			{
				this.QrBorder.Width = 600.0;
				this.QrBorder.Height = 600.0;
			}
			if (base.Width > 0.0 && base.Width <= 1400.0)
			{
				this.QrBorder.Width = 400.0;
				this.QrBorder.Height = 400.0;
			}
		}
	}

}
