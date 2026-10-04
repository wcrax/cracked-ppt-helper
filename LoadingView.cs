using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Cvte.Windows;

namespace PPTService
{
	// Token: 0x02000030 RID: 48
	public partial class LoadingView : UserControl, IComponentConnector
	{
		// Restored fields
		private DispatcherTimer _timer;

		private int _progress;

		private DispatcherFrame _frame;

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000772E File Offset: 0x0000592E
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00007736 File Offset: 0x00005936
		public Action CancelAction { get; set; }

		// Token: 0x17000041 RID: 65
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x0000773F File Offset: 0x0000593F
		public string Tip
		{
			set
			{
				this.TipTextBlock.Text = value;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000774D File Offset: 0x0000594D
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x00007755 File Offset: 0x00005955
		public int Progress { get; set; }

		// Token: 0x060001B7 RID: 439 RVA: 0x00007760 File Offset: 0x00005960
		public LoadingView()
		{
			this.InitializeComponent();
			this._timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(3.0)
			};
			this._timer.Tick += this._timer_Tick;
			base.Loaded += this.LoadingView_Loaded;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000077C4 File Offset: 0x000059C4
		private void LoadingView_Loaded(object sender, RoutedEventArgs e)
		{
			base.Opacity = 1.0;
			this.ClearData();
			this.CancelButton.IsEnabled = true;
			Storyboard storyboard = this.RootGrid.Resources["StoryBoard.ShowRootGrid"] as Storyboard;
			if (storyboard != null)
			{
				storyboard.Begin();
			}
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				WindowStateHelper.LoadingWindowIntPtr = new WindowInteropHelper(window).Handle;
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00007834 File Offset: 0x00005A34
		private void _timer_Tick(object sender, EventArgs e)
		{
			if (this._progress < this.Progress)
			{
				this._progress++;
				this.LoadProgressBar.Value = (double)this._progress;
				this.LoadProgressTextBlock.Text = this._progress.ToString() + "%";
			}
			this._progress=100;
			if (this._progress >= 100)
			{
				this._timer.Stop();
				this.Progress = 0;
				this.ClearData();
				Storyboard storyboard = this.RootGrid.Resources["StoryBoard.HiddenRootGridImmediatly"] as Storyboard;
				if (storyboard != null)
				{
					storyboard.Begin();
				}
				DispatcherTask.Start(30.0, delegate()
				{
					this._frame.Continue = false;
				});
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x000078F1 File Offset: 0x00005AF1
		public void Start()
		{
			this._timer.Start();
			this._frame = new DispatcherFrame();
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00007909 File Offset: 0x00005B09
		public void PushFrame()
		{
			Dispatcher.PushFrame(this._frame);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00007916 File Offset: 0x00005B16
		public void Continue()
		{
			Storyboard storyboard = this.RootGrid.Resources["StoryBoard.HiddenRootGridImmediatly"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000793C File Offset: 0x00005B3C
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			Action cancelAction = this.CancelAction;
			if (cancelAction != null)
			{
				cancelAction();
			}
			this.Tip = "正在取消...";
			this.CancelButton.IsEnabled = false;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00007968 File Offset: 0x00005B68
		public void Cancel()
		{
			base.Opacity = 0.0;
			this.ClearData();
			Storyboard storyboard = this.RootGrid.Resources["StoryBoard.HiddenRootGridImmediatly"] as Storyboard;
			if (storyboard != null)
			{
				storyboard.Begin();
			}
			DispatcherTask.Start(500.0, delegate()
			{
				this._frame.Continue = false;
				LoadingScreen<LoadingView>.Current.Hide();
			});
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000079CA File Offset: 0x00005BCA
		private void StoryBoard_HiddenRootGridImmediately_OnCompleted(object sender, EventArgs e)
		{
			this._frame.Continue = false;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000079D8 File Offset: 0x00005BD8
		private void ClearData()
		{
			this._progress = 0;
			this.LoadProgressBar.Value = 0.0;
			this.LoadProgressTextBlock.Text = "0%";
		}
	}

}
