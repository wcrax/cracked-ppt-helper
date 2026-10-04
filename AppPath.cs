using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PPTService
{
	// Token: 0x02000032 RID: 50
	public class AppPath
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x00007B68 File Offset: 0x00005D68
		public static string DocumentPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppPath.Company, AppPath.AppName, AppPath.Document);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00007BA4 File Offset: 0x00005DA4
		public static string LogPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppPath.Company, AppPath.AppName, AppPath.Log);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00007BE0 File Offset: 0x00005DE0
		public static string TemPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppPath.Company, AppPath.AppName, AppPath.Temporary);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00007C1C File Offset: 0x00005E1C
		public static string RootPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppPath.Company, AppPath.AppName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00007C50 File Offset: 0x00005E50
		public static void ClearTempFolder()
		{
			try
			{
				Task.Factory.StartNew(delegate()
				{
					AppPath.ClearDirectory(AppPath.TemPath());
				});
			}
			catch
			{
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00007C9C File Offset: 0x00005E9C
		private static void ClearDirectory(string directory)
		{
			try
			{
				Stack<string> stack = new Stack<string>();
				stack.Push(directory);
				while (stack.Any<string>())
				{
					string path = stack.Pop();
					foreach (string path2 in Directory.GetFiles(path))
					{
						File.SetAttributes(path2, FileAttributes.Normal);
						File.Delete(path2);
					}
					foreach (string item in Directory.GetDirectories(path))
					{
						stack.Push(item);
					}
				}
			}
			catch
			{
			}
			try
			{
				Directory.Delete(directory, true);
				Directory.CreateDirectory(directory);
			}
			catch
			{
			}
		}

		// Token: 0x040000C4 RID: 196
		private static readonly string Company = "Seewo";

		// Token: 0x040000C5 RID: 197
		private static readonly string AppName = "PPTService";

		// Token: 0x040000C6 RID: 198
		private static readonly string Log = "Log";

		// Token: 0x040000C7 RID: 199
		private static readonly string Temporary = "Temp";

		// Token: 0x040000C8 RID: 200
		private static readonly string Document = "Document";
	}
}
