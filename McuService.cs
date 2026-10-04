using System;
using Cvte.Mcu;

namespace PPTService
{
	// Token: 0x02000009 RID: 9
	public static class McuService
	{
		// Token: 0x0600003C RID: 60 RVA: 0x0000296C File Offset: 0x00000B6C
		public static bool FindMcu()
		{
			return Mcu.FindMcu();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002973 File Offset: 0x00000B73
		public static bool ForbiddenAndroidPen()
		{
			return Mcu.ForbiddenAndroidPen();
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000297A File Offset: 0x00000B7A
		public static bool OpenAndroidPen()
		{
			return Mcu.OpenAndroidPen();
		}
	}
}
