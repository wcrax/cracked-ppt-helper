using System;
using System.IO;

namespace PPTService
{
	// Token: 0x02000027 RID: 39
	internal class ConfigController : IniSetting
	{
		// Token: 0x0600014C RID: 332 RVA: 0x0000564F File Offset: 0x0000384F
		public ConfigController(string settingPath, string section) : base(settingPath, section)
		{
			if (!File.Exists(settingPath))
			{
				base.SetSettingValue(ConfigController.IsNeedStartKey, true, section);
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600014D RID: 333 RVA: 0x0000566E File Offset: 0x0000386E
		// (set) Token: 0x0600014E RID: 334 RVA: 0x0000567C File Offset: 0x0000387C
		public bool IsComOk
		{
			get
			{
				return base.GetSettingValue(ConfigController.IsComOkKey, true);
			}
			set
			{
				base.SetSettingValue(ConfigController.IsComOkKey, value);
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600014F RID: 335 RVA: 0x0000568A File Offset: 0x0000388A
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00005698 File Offset: 0x00003898
		public bool IsInitApplicationSuccessful
		{
			get
			{
				return base.GetSettingValue(ConfigController.IsInitApplicationSuccessfulKey, true);
			}
			set
			{
				base.SetSettingValue(ConfigController.IsInitApplicationSuccessfulKey, value);
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000151 RID: 337 RVA: 0x000056A6 File Offset: 0x000038A6
		// (set) Token: 0x06000152 RID: 338 RVA: 0x000056B4 File Offset: 0x000038B4
		public bool IsNeedStart
		{
			get
			{
				return base.GetSettingValue(ConfigController.IsNeedStartKey, true);
			}
			set
			{
				base.SetSettingValue(ConfigController.IsNeedStartKey, value);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000153 RID: 339 RVA: 0x000056C2 File Offset: 0x000038C2
		// (set) Token: 0x06000154 RID: 340 RVA: 0x000056D4 File Offset: 0x000038D4
		public string OfficeVersion
		{
			get
			{
				return base.GetSettingValue(ConfigController.OfficeVersionKey, "");
			}
			set
			{
				base.SetSettingValue(ConfigController.OfficeVersionKey, value);
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000056E2 File Offset: 0x000038E2
		// (set) Token: 0x06000156 RID: 342 RVA: 0x000056F0 File Offset: 0x000038F0
		public bool IsShowing
		{
			get
			{
				return base.GetSettingValue(ConfigController.IsShowingKey, false);
			}
			set
			{
				base.SetSettingValue(ConfigController.IsShowingKey, value);
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000157 RID: 343 RVA: 0x000056FE File Offset: 0x000038FE
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00005710 File Offset: 0x00003910
		public string IsSeewoDevice
		{
			get
			{
				return base.GetSettingValue(ConfigController.IsSeewoDeviceKey, "");
			}
			set
			{
				base.SetSettingValue(ConfigController.IsSeewoDeviceKey, value);
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000159 RID: 345 RVA: 0x0000571E File Offset: 0x0000391E
		// (set) Token: 0x0600015A RID: 346 RVA: 0x0000572C File Offset: 0x0000392C
		public bool IsPptShowTypeSpeaker
		{
			get
			{
				return base.GetSettingValue(ConfigController.IsPptShowTypeSpeakerKey, true);
			}
			set
			{
				base.SetSettingValue(ConfigController.IsPptShowTypeSpeakerKey, value);
			}
		}

		// Token: 0x04000064 RID: 100
		private static readonly string IsComOkKey = "IsComOk";

		// Token: 0x04000065 RID: 101
		private static readonly string IsShowingKey = "IsShowing";

		// Token: 0x04000066 RID: 102
		private static readonly string IsNeedStartKey = "IsNeedStart";

		// Token: 0x04000067 RID: 103
		private static readonly string OfficeVersionKey = "OfficeVersion";

		// Token: 0x04000068 RID: 104
		private static readonly string IsSeewoDeviceKey = "IsSeewoDevice";

		// Token: 0x04000069 RID: 105
		private static readonly string IsPptShowTypeSpeakerKey = "IsPptShowTypeSpeaker";

		// Token: 0x0400006A RID: 106
		private static readonly string IsInitApplicationSuccessfulKey = "IsInitApplicationSuccessful";
	}
}
