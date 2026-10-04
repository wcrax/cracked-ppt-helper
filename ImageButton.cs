using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PPTService
{
	// Token: 0x0200000B RID: 11
	public class ImageButton : Button
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00002C8C File Offset: 0x00000E8C
		static ImageButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002D10 File Offset: 0x00000F10
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			Grid grid = base.GetTemplateChild("BackgroundImageGrid") as Grid;
			if (grid == null)
			{
				return;
			}
			this._storyboard = (grid.Resources["ButtonPressedStoryBoard"] as Storyboard);
			if (this._storyboard == null)
			{
				return;
			}
			this.UpdateTemplateImageSource();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002D64 File Offset: 0x00000F64
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ImageButton.NormalSourceProperty || e.Property == ImageButton.PressedSourceProperty || e.Property == UIElement.IsEnabledProperty || e.Property == UIElement.IsMouseOverProperty || e.Property == UIElement.IsStylusOverProperty || e.Property == ButtonBase.IsPressedProperty)
			{
				this.UpdateTemplateImageSource();
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002DD2 File Offset: 0x00000FD2
		private void UpdateTemplateImageSource()
		{
			if (this._storyboard != null && base.IsPressed)
			{
				this._storyboard.Begin();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002DEF File Offset: 0x00000FEF
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002E01 File Offset: 0x00001001
		public ImageSource PressedSource
		{
			get
			{
				return (ImageSource)base.GetValue(ImageButton.PressedSourceProperty);
			}
			set
			{
				base.SetValue(ImageButton.PressedSourceProperty, value);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002E0F File Offset: 0x0000100F
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002E21 File Offset: 0x00001021
		public ImageSource NormalSource
		{
			get
			{
				return (ImageSource)base.GetValue(ImageButton.NormalSourceProperty);
			}
			set
			{
				base.SetValue(ImageButton.NormalSourceProperty, value);
			}
		}

		// Token: 0x0400000E RID: 14
		private Storyboard _storyboard;

		// Token: 0x0400000F RID: 15
		public static readonly DependencyProperty PressedSourceProperty = DependencyProperty.Register("PressedSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

		// Token: 0x04000010 RID: 16
		public static readonly DependencyProperty NormalSourceProperty = DependencyProperty.Register("NormalSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));
	}
}
