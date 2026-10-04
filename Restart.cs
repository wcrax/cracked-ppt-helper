using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace PPTService
{
	// Token: 0x0200001D RID: 29
	public class Restart
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x00004830 File Offset: 0x00002A30
		public static void PPTService()
		{
			string pptservicePath = Restart.GetPPTServicePath();
			if (string.IsNullOrEmpty(pptservicePath))
			{
				return;
			}
			if (!File.Exists(pptservicePath))
			{
				return;
			}
			Process.Start(pptservicePath, Restart.AppRestart);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00004864 File Offset: 0x00002A64
		private static string GetPPTServicePath()
		{
			string text = null;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Seewo\\PPTService", false);
			if (registryKey != null)
			{
				text = registryKey.GetValue("ExePath").ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Seewo\\PPTService", false);
				if (registryKey2 != null)
				{
					text = registryKey2.GetValue("ExePath").ToString();
				}
			}
			return text;
		}

		// Token: 0x0400004C RID: 76
		private static string AppRestart = "Restart";
	}
}
