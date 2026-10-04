using System;
using System.Windows;
using System.Windows.Input;

namespace PPTService
{
	// Token: 0x02000013 RID: 19
	public static class DeviceInput
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00003DF9 File Offset: 0x00001FF9
		public static void CaptureDevice(this UIElement element, TouchDevice device = null)
		{
			element.CaptureMouse();
			element.CaptureStylus();
			if (device != null)
			{
				element.CaptureTouch(device);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003E14 File Offset: 0x00002014
		public static void ReleaseDeviceCapture(this UIElement element, TouchDevice device = null)
		{
			element.ReleaseMouseCapture();
			element.ReleaseStylusCapture();
			if (device != null)
			{
				element.ReleaseTouchCapture(device);
			}
		}
	}
}
