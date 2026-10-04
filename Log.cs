using System;
using System.IO;
using System.Text;

namespace PPTService
{
	// Token: 0x02000028 RID: 40
	public static class Log
	{
		// Token: 0x0600015C RID: 348 RVA: 0x0000578F File Offset: 0x0000398F
		public static void WriteError(string error)
		{
			Log.Write(Log.ErrorFilePath, error);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00002F72 File Offset: 0x00001172
		public static void WriteDebug(string info)
		{
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000579C File Offset: 0x0000399C
		private static void Write(string filePath, string str)
		{
			object locker = Log.Locker;
			lock (locker)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Format("{0:G}  {1}", DateTime.Now, str));
				File.AppendAllText(filePath, stringBuilder.ToString());
			}
		}

		// Token: 0x0400006B RID: 107
		public static readonly string ErrorFilePath = Path.Combine(AppPath.LogPath(), "error.txt");

		// Token: 0x0400006C RID: 108
		public static readonly string DebugFilePath = Path.Combine(AppPath.LogPath(), "debug.txt");

		// Token: 0x0400006D RID: 109
		private static readonly object Locker = new object();
	}
}
