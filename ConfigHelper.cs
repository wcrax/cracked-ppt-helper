using System;
using System.IO;

namespace PPTService
{
	// Token: 0x0200003A RID: 58
	public class ConfigHelper
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000AEFD File Offset: 0x000090FD
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000AF04 File Offset: 0x00009104
		public static ConfigHelper Instance { get; set; }

		// Token: 0x0600025C RID: 604 RVA: 0x0000AF0C File Offset: 0x0000910C
		public ConfigHelper()
		{
			this.CheckSeewoDevice();
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000AF1C File Offset: 0x0000911C
		private void CheckSeewoDevice()
		{
			if (ConfigHelper.ConfigController.IsSeewoDevice != "")
			{
				return;
			}
			bool flag = McuService.FindMcu();
			ConfigHelper.ConfigController.IsNeedStart = true;
			ConfigHelper.ConfigController.IsSeewoDevice = (flag ? ConfigHelper.CheckedTrue : ConfigHelper.CheckedFalse);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000AF6A File Offset: 0x0000916A
		public bool IsNeedStart()
		{
			return ConfigHelper.ConfigController.IsNeedStart;
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600025F RID: 607 RVA: 0x0000AF76 File Offset: 0x00009176
		// (set) Token: 0x06000260 RID: 608 RVA: 0x0000AF82 File Offset: 0x00009182
		public bool IsInitApplicationSuccessful
		{
			get
			{
				return ConfigHelper.ConfigController.IsInitApplicationSuccessful;
			}
			set
			{
				ConfigHelper.ConfigController.IsInitApplicationSuccessful = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000261 RID: 609 RVA: 0x0000AF8F File Offset: 0x0000918F
		// (set) Token: 0x06000262 RID: 610 RVA: 0x0000AF9B File Offset: 0x0000919B
		public bool IsComOk
		{
			get
			{
				return ConfigHelper.ConfigController.IsComOk;
			}
			set
			{
				ConfigHelper.ConfigController.IsComOk = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000263 RID: 611 RVA: 0x0000AFA8 File Offset: 0x000091A8
		// (set) Token: 0x06000264 RID: 612 RVA: 0x0000AFB4 File Offset: 0x000091B4
		public string OfficeVersion
		{
			get
			{
				return ConfigHelper.ConfigController.OfficeVersion;
			}
			set
			{
				ConfigHelper.ConfigController.OfficeVersion = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0000AFC1 File Offset: 0x000091C1
		// (set) Token: 0x06000266 RID: 614 RVA: 0x0000AFCD File Offset: 0x000091CD
		public bool IsShowing
		{
			get
			{
				return ConfigHelper.ConfigController.IsShowing;
			}
			set
			{
				ConfigHelper.ConfigController.IsShowing = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0000AFDA File Offset: 0x000091DA
		// (set) Token: 0x06000268 RID: 616 RVA: 0x0000AFE6 File Offset: 0x000091E6
		public bool IsPptShowTypeSpeaker
		{
			get
			{
				return ConfigHelper.ConfigController.IsPptShowTypeSpeaker;
			}
			set
			{
				ConfigHelper.ConfigController.IsPptShowTypeSpeaker = value;
			}
		}

		// Token: 0x04000126 RID: 294
		private static readonly string Section = "StartConfig";

		// Token: 0x04000127 RID: 295
		private static readonly string CheckedTrue = "ChceckedTrue";

		// Token: 0x04000128 RID: 296
		private static readonly string CheckedFalse = "ChceckedFalse";

		// Token: 0x04000129 RID: 297
		private static readonly string AppConfigFileName = "app.config";

		// Token: 0x0400012A RID: 298
		private static readonly ConfigController ConfigController = new ConfigController(Path.Combine(AppPath.RootPath(), ConfigHelper.AppConfigFileName), ConfigHelper.Section);
	}
}
