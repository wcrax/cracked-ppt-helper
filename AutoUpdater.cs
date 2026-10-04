using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace PPTService
{
	// Token: 0x02000008 RID: 8
	internal static class AutoUpdater
	{
		// Token: 0x06000036 RID: 54 RVA: 0x000027B5 File Offset: 0x000009B5
		public static void Check()
		{
			if (AutoUpdater._checkTimes >= AutoUpdater.MaxCheckTimes)
			{
				return;
			}
			AutoUpdater._checkTimes++;
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				AutoUpdater.InnerCheck();
			});
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000027F5 File Offset: 0x000009F5
		public static void Update()
		{
			AutoUpdater.InnerUpdate();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000027FC File Offset: 0x000009FC
		private static void InnerCheck()
		{
			Version newVersion = new Version();
			string text = AutoUpdater.TryToGetUpdater();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			Process process = new Process();
			process.StartInfo.Arguments = "-task check -type userstart -lang zh_chs";
			process.StartInfo.WorkingDirectory = Path.GetDirectoryName(text);
			process.StartInfo.FileName = text;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				Version parsedVersion;
				if (Version.TryParse(e.Data, out parsedVersion))
				{
					newVersion = parsedVersion;
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();
			if (newVersion != new Version())
			{
				Application.Current.Dispatcher.Invoke(delegate()
				{
					new UpdateWindow
					{
						NewVersion = newVersion.ToString()
					}.ShowDialog();
				});
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000028C4 File Offset: 0x00000AC4
		private static void InnerUpdate()
		{
			string text = AutoUpdater.TryToGetUpdater();
			new Process
			{
				StartInfo = 
				{
					Arguments = "-task update -type userstart -lang zh_chs",
					WorkingDirectory = Path.GetDirectoryName(text),
					FileName = text
				}
			}.Start();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002910 File Offset: 0x00000B10
		private static string TryToGetUpdater()
		{
			string directoryName = Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ApplicationBase));
			if (!string.IsNullOrEmpty(directoryName))
			{
				string text = Path.Combine(directoryName, "Updater.exe");
				if (File.Exists(text))
				{
					return text;
				}
			}
			Log.WriteError("Updater missing.");
			return string.Empty;
		}

		// Token: 0x04000004 RID: 4
		private static int _checkTimes;

		// Token: 0x04000005 RID: 5
		private static int MaxCheckTimes = 2;

		// Token: 0x04000006 RID: 6
		private const string UpdaterName = "Updater.exe";
	}
}
