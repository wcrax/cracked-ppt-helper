using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;

namespace PPTService
{
	// Token: 0x02000007 RID: 7
	public class AdaptiveTrigger : TriggerBase<FrameworkElement>
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000024FC File Offset: 0x000006FC
		protected override void OnAttached()
		{
			if (DesignerProperties.GetIsInDesignMode(base.AssociatedObject))
			{
				return;
			}
			Window window = Window.GetWindow(base.AssociatedObject);
			if (window == null)
			{
				base.AssociatedObject.Loaded += this.OnLoaded;
				return;
			}
			if (window.ActualWidth <= 0.0 && window.ActualHeight <= 0.0)
			{
				base.AssociatedObject.Loaded += this.OnLoaded;
				return;
			}
			this.StartWatch();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002580 File Offset: 0x00000780
		protected override void OnDetaching()
		{
			if (DesignerProperties.GetIsInDesignMode(base.AssociatedObject))
			{
				return;
			}
			base.AssociatedObject.Loaded -= this.OnLoaded;
			Window.GetWindow(base.AssociatedObject).SizeChanged += this.OnWindowSizeChanged;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000025CE File Offset: 0x000007CE
		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			base.AssociatedObject.Loaded -= this.OnLoaded;
			this.StartWatch();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000025ED File Offset: 0x000007ED
		private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.Adapt(e.NewSize);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000025FC File Offset: 0x000007FC
		private void StartWatch()
		{
			Window window = Window.GetWindow(base.AssociatedObject);
			window.SizeChanged += this.OnWindowSizeChanged;
			if (base.AssociatedObject.IsLoaded)
			{
				this.Adapt(new Size(window.ActualWidth, window.ActualHeight));
				return;
			}
			base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.Adapt(new Size(window.ActualWidth, window.ActualHeight));
			}), new object[0]);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000268C File Offset: 0x0000088C
		private void Adapt(Size size)
		{
			bool flag = false;
			if (!double.IsNaN(this.MinWindowWidth))
			{
				flag = (size.Width >= this.MinWindowWidth);
			}
			if (!flag && !double.IsNaN(this.MinWindowHeight))
			{
				flag = (size.Height >= this.MinWindowHeight);
			}
			if (flag)
			{
				base.InvokeActions(null);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000026E8 File Offset: 0x000008E8
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000026FA File Offset: 0x000008FA
		public double MinWindowWidth
		{
			get
			{
				return (double)base.GetValue(AdaptiveTrigger.MinWindowWidthProperty);
			}
			set
			{
				base.SetValue(AdaptiveTrigger.MinWindowWidthProperty, value);
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000032 RID: 50 RVA: 0x0000270D File Offset: 0x0000090D
		// (set) Token: 0x06000033 RID: 51 RVA: 0x0000271F File Offset: 0x0000091F
		public double MinWindowHeight
		{
			get
			{
				return (double)base.GetValue(AdaptiveTrigger.MinWindowHeightProperty);
			}
			set
			{
				base.SetValue(AdaptiveTrigger.MinWindowHeightProperty, value);
			}
		}

		// Token: 0x04000002 RID: 2
		public static readonly DependencyProperty MinWindowWidthProperty = DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(double.NaN));

		// Token: 0x04000003 RID: 3
		public static readonly DependencyProperty MinWindowHeightProperty = DependencyProperty.Register("MinWindowHeight", typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(double.NaN));
	}
}
