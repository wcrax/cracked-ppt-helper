using System;
using System.Runtime.InteropServices;

namespace PPTService
{
	// Token: 0x02000045 RID: 69
	public struct HidpCaps
	{
		// Token: 0x0400015F RID: 351
		public ushort Usage;

		// Token: 0x04000160 RID: 352
		public ushort UsagePage;

		// Token: 0x04000161 RID: 353
		public ushort InputReportByteLength;

		// Token: 0x04000162 RID: 354
		public ushort OutputReportByteLength;

		// Token: 0x04000163 RID: 355
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
		public ushort[] Reserved;

		// Token: 0x04000164 RID: 356
		public ushort NumberLinkCollectionNodes;

		// Token: 0x04000165 RID: 357
		public ushort NumberInputButtonCaps;

		// Token: 0x04000166 RID: 358
		public ushort NumberInputValueCaps;

		// Token: 0x04000167 RID: 359
		public ushort NumberInputDataIndices;

		// Token: 0x04000168 RID: 360
		public ushort NumberOutputButtonCaps;

		// Token: 0x04000169 RID: 361
		public ushort NumberOutputValueCaps;

		// Token: 0x0400016A RID: 362
		public ushort NumberOutputDataIndices;

		// Token: 0x0400016B RID: 363
		public ushort NumberFeatureButtonCaps;

		// Token: 0x0400016C RID: 364
		public ushort NumberFeatureValueCaps;

		// Token: 0x0400016D RID: 365
		public ushort NumberFeatureDataIndices;
	}
}
