using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LinqToVisualTree;

namespace PPTService
{
	// Token: 0x0200000C RID: 12
	public abstract class AttachedViewModelBase : FrameworkElement, INotifyPropertyChanged
	{
		// Token: 0x0600005A RID: 90 RVA: 0x00002E37 File Offset: 0x00001037
		public static AttachedViewModelBase GetAttach(DependencyObject d)
		{
			return (AttachedViewModelBase)d.GetValue(AttachedViewModelBase.AttachProperty);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002E49 File Offset: 0x00001049
		public static void SetAttach(DependencyObject d, AttachedViewModelBase value)
		{
			d.SetValue(AttachedViewModelBase.AttachProperty, value);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002E58 File Offset: 0x00001058
		private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement frameworkElement = d as FrameworkElement;
			(e.NewValue as AttachedViewModelBase).AttachedElement = frameworkElement;
			frameworkElement.Loaded += AttachedViewModelBase.Element_Loaded;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002E90 File Offset: 0x00001090
		private static void Element_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			FrameworkElement source = frameworkElement.Parent as FrameworkElement;
			AttachedViewModelBase attach = AttachedViewModelBase.GetAttach(frameworkElement);
			frameworkElement.DataContext = attach;
			ProgressBar progressBar = frameworkElement.Ancestors<ProgressBar>().Single<DependencyObject>() as ProgressBar;
			attach.SetProgressBar(progressBar);
			attach.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext")
			{
				Source = source
			});
			attach.SetBinding(AttachedViewModelBase.DataContextPiggyBackProperty, new Binding("DataContext")
			{
				Source = source
			});
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002F0E File Offset: 0x0000110E
		public static object GetDataContextPiggyBack(DependencyObject d)
		{
			return d.GetValue(AttachedViewModelBase.DataContextPiggyBackProperty);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00002F1B File Offset: 0x0000111B
		public static void SetDataContextPiggyBack(DependencyObject d, object value)
		{
			d.SetValue(AttachedViewModelBase.DataContextPiggyBackProperty, value);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002F29 File Offset: 0x00001129
		private static void OnDataContextPiggyBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as AttachedViewModelBase).OnDataContextChanged();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002F38 File Offset: 0x00001138
		private void OnDataContextChanged()
		{
			INotifyPropertyChanged notifyPropertyChanged = base.DataContext as INotifyPropertyChanged;
			if (notifyPropertyChanged != null)
			{
				notifyPropertyChanged.PropertyChanged += this.AdaptedDataContextPropertyChanged;
			}
			this.OnPropertyChanged("");
		}

		// Token: 0x06000062 RID: 98
		protected abstract void AdaptedDataContextPropertyChanged(object sender, PropertyChangedEventArgs e);

		// Token: 0x06000063 RID: 99 RVA: 0x00002F72 File Offset: 0x00001172
		public virtual void ComputeViewModelProperties()
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002F74 File Offset: 0x00001174
		private void SetProgressBar(ProgressBar progressBar)
		{
			this._progressBar = progressBar;
			this._progressBar.SizeChanged += delegate(object s, SizeChangedEventArgs e)
			{
				this.ComputeViewModelProperties();
			};
			this.valueNotifier = new PropertyChangeNotifier(this._progressBar, "Value");
			this.valueNotifier.ValueChanged += this.valueNotifier_ValueChanged;
			this.maximumNotifier = new PropertyChangeNotifier(this._progressBar, "Maximum");
			this.maximumNotifier.ValueChanged += this.maximumNotifier_ValueChanged;
			this.minimumNotifier = new PropertyChangeNotifier(this._progressBar, "Minimum");
			this.minimumNotifier.ValueChanged += this.minimumNotifier_ValueChanged;
			this.ComputeViewModelProperties();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000302C File Offset: 0x0000122C
		private void minimumNotifier_ValueChanged(object sender, EventArgs e)
		{
			this.ComputeViewModelProperties();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000302C File Offset: 0x0000122C
		private void maximumNotifier_ValueChanged(object sender, EventArgs e)
		{
			this.ComputeViewModelProperties();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000302C File Offset: 0x0000122C
		private void valueNotifier_ValueChanged(object sender, EventArgs e)
		{
			this.ComputeViewModelProperties();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003034 File Offset: 0x00001234
		public AttachedViewModelBase()
		{
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000069 RID: 105 RVA: 0x0000303C File Offset: 0x0000123C
		// (remove) Token: 0x0600006A RID: 106 RVA: 0x00003074 File Offset: 0x00001274
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x0600006B RID: 107 RVA: 0x000030A9 File Offset: 0x000012A9
		protected void OnPropertyChanged(string property)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000030C5 File Offset: 0x000012C5
		// (set) Token: 0x0600006D RID: 109 RVA: 0x000030CD File Offset: 0x000012CD
		public FrameworkElement AttachedElement
		{
			get
			{
				return this._element;
			}
			set
			{
				this._element = value;
				this._element.SizeChanged += this.AttachedElement_SizeChanged;
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000030EE File Offset: 0x000012EE
		protected virtual void AttachedElement_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.OnPropertyChanged("ElementHeight");
			this.OnPropertyChanged("ElementWidth");
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00003106 File Offset: 0x00001306
		public double ElementWidth
		{
			get
			{
				return this._element.ActualWidth;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00003113 File Offset: 0x00001313
		public double ElementHeight
		{
			get
			{
				return this._element.ActualHeight;
			}
		}

		// Token: 0x04000011 RID: 17
		public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(object), typeof(AttachedViewModelBase), new PropertyMetadata(null, new PropertyChangedCallback(AttachedViewModelBase.OnAttachChanged)));

		// Token: 0x04000012 RID: 18
		public static readonly DependencyProperty DataContextPiggyBackProperty = DependencyProperty.RegisterAttached("DataContextPiggyBack", typeof(object), typeof(AttachedViewModelBase), new PropertyMetadata(null, new PropertyChangedCallback(AttachedViewModelBase.OnDataContextPiggyBackChanged)));

		// Token: 0x04000013 RID: 19
		protected ProgressBar _progressBar;

		// Token: 0x04000014 RID: 20
		private PropertyChangeNotifier valueNotifier;

		// Token: 0x04000015 RID: 21
		private PropertyChangeNotifier maximumNotifier;

		// Token: 0x04000016 RID: 22
		private PropertyChangeNotifier minimumNotifier;

		// Token: 0x04000018 RID: 24
		private FrameworkElement _element;
	}
}
