using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PPTService
{
	// Token: 0x02000011 RID: 17
	public sealed class PropertyChangeNotifier : DependencyObject, IDisposable
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00003ADA File Offset: 0x00001CDA
		public PropertyChangeNotifier(DependencyObject propertySource, string path) : this(propertySource, new PropertyPath(path, new object[0]))
		{
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003AEF File Offset: 0x00001CEF
		public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property) : this(propertySource, new PropertyPath(property))
		{
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003B00 File Offset: 0x00001D00
		public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
		{
			if (propertySource == null)
			{
				throw new ArgumentNullException("propertySource");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this._propertySource = new WeakReference(propertySource);
			Binding binding = new Binding();
			binding.Path = property;
			binding.Mode = BindingMode.OneWay;
			binding.Source = propertySource;
			BindingOperations.SetBinding(this, PropertyChangeNotifier.ValueProperty, binding);
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00003B64 File Offset: 0x00001D64
		public DependencyObject PropertySource
		{
			get
			{
				DependencyObject result;
				try
				{
					result = (this._propertySource.IsAlive ? (this._propertySource.Target as DependencyObject) : null);
				}
				catch
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003BAC File Offset: 0x00001DAC
		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PropertyChangeNotifier propertyChangeNotifier = (PropertyChangeNotifier)d;
			if (propertyChangeNotifier.ValueChanged != null)
			{
				propertyChangeNotifier.ValueChanged(propertyChangeNotifier, EventArgs.Empty);
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00003BD9 File Offset: 0x00001DD9
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00003BE6 File Offset: 0x00001DE6
		[Description("Returns/sets the value of the property")]
		[Category("Behavior")]
		[Bindable(true)]
		public object Value
		{
			get
			{
				return base.GetValue(PropertyChangeNotifier.ValueProperty);
			}
			set
			{
				base.SetValue(PropertyChangeNotifier.ValueProperty, value);
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060000A9 RID: 169 RVA: 0x00003BF4 File Offset: 0x00001DF4
		// (remove) Token: 0x060000AA RID: 170 RVA: 0x00003C2C File Offset: 0x00001E2C
		public event EventHandler ValueChanged;

		// Token: 0x060000AB RID: 171 RVA: 0x00003C61 File Offset: 0x00001E61
		public void Dispose()
		{
			BindingOperations.ClearBinding(this, PropertyChangeNotifier.ValueProperty);
		}

		// Token: 0x0400002B RID: 43
		private WeakReference _propertySource;

		// Token: 0x0400002C RID: 44
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(PropertyChangeNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyChangeNotifier.OnPropertyChanged)));
	}
}
