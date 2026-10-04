using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace PPTService
{
	// Token: 0x02000021 RID: 33
	public static class ImageConvertExtensions
	{
		// Token: 0x0600010E RID: 270 RVA: 0x00004D58 File Offset: 0x00002F58
		public static void SaveToFile(this BitmapSource bitmap, ImageFormat format, string fileName, bool overwrite = false)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			if (File.Exists(fileName) && !overwrite)
			{
				throw new IOException("File " + fileName + " already Exists.");
			}
			BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
			bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
			using (Stream stream = File.Create(fileName))
			{
				bitmapEncoder.Save(stream);
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00004DD8 File Offset: 0x00002FD8
		public static void SaveToFile(this BitmapSource bitmap, string fileName, bool overwrite = false)
		{
			ImageFormat png = ImageFormat.Png;
			bitmap.SaveToFile(png, fileName, overwrite);
		}
	}
}
