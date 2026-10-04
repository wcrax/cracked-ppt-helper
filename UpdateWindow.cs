using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace PPTService
{
	// Token: 0x02000040 RID: 64
	public partial class UpdateWindow : Window, IComponentConnector
	{
		// Token: 0x1700005C RID: 92
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000BC1E File Offset: 0x00009E1E
		public string NewVersion
		{
			set
			{
				this.VersionRun.Text = value;
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000BC2C File Offset: 0x00009E2C
		public UpdateWindow()
		{
			this.InitializeComponent();
			base.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
			{
				if (e.ButtonState == MouseButtonState.Pressed)
				{
					base.DragMove();
				}
			};
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00009414 File Offset: 0x00007614
		private void CloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00009414 File Offset: 0x00007614
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000BC4C File Offset: 0x00009E4C
		private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
		{
			AutoUpdater.Update();
			base.Close();
		}
	}

}
