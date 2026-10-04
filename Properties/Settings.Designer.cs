using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace PPTService.Properties
{
	// Token: 0x0200004B RID: 75
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000335 RID: 821 RVA: 0x0000D953 File Offset: 0x0000BB53
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400019C RID: 412
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
