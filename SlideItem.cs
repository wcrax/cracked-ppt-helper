using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PPTService
{
	// Token: 0x02000039 RID: 57
	public class SlideItem : UserControl
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000252 RID: 594 RVA: 0x0000AE01 File Offset: 0x00009001
		// (set) Token: 0x06000253 RID: 595 RVA: 0x0000AE13 File Offset: 0x00009013
		public string IsPptThumbnail
		{
			get
			{
				return (string)base.GetValue(SlideItem.IsPptThumbnailProperty);
			}
			set
			{
				base.SetValue(SlideItem.IsPptThumbnailProperty, value);
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000AE21 File Offset: 0x00009021
		// (set) Token: 0x06000255 RID: 597 RVA: 0x0000AE33 File Offset: 0x00009033
		public int Index
		{
			get
			{
				return (int)base.GetValue(SlideItem.IndexProperty);
			}
			set
			{
				base.SetValue(SlideItem.IndexProperty, value);
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000AE46 File Offset: 0x00009046
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000AE58 File Offset: 0x00009058
		public BitmapImage BitmapImage
		{
			get
			{
				return (BitmapImage)base.GetValue(SlideItem.BitmapImageProperty);
			}
			set
			{
				base.SetValue(SlideItem.BitmapImageProperty, value);
			}
		}

		// Token: 0x04000122 RID: 290
		public static readonly DependencyProperty IsPptThumbnailProperty = DependencyProperty.Register("IsPptThumbnail", typeof(string), typeof(SlideItem), new PropertyMetadata(null));

		// Token: 0x04000123 RID: 291
		public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(SlideItem), new PropertyMetadata(0));

		// Token: 0x04000124 RID: 292
		public static readonly DependencyProperty BitmapImageProperty = DependencyProperty.Register("BitmapImage", typeof(BitmapImage), typeof(SlideItem), new PropertyMetadata(null));
	}
}
