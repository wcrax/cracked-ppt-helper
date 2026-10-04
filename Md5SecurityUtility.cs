using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PPTService
{
	// Token: 0x02000033 RID: 51
	public class Md5SecurityUtility
	{
		// Token: 0x060001CF RID: 463 RVA: 0x00007D7C File Offset: 0x00005F7C
		public static string GetShaFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
			string result;
			try
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (SHA256Cng sha256Cng = new SHA256Cng())
					{
						byte[] array = sha256Cng.ComputeHash(fileStream);
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < array.Length; i++)
						{
							stringBuilder.Append(array[i].ToString("x2"));
						}
						result = stringBuilder.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.ToString());
				result = null;
			}
			return result;
		}
	}
}
