using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PPTService.Properties
{
	// Token: 0x0200004A RID: 74
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000331 RID: 817 RVA: 0x000048C6 File Offset: 0x00002AC6
		internal Resources()
		{
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000332 RID: 818 RVA: 0x0000D918 File Offset: 0x0000BB18
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("PPTService.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000333 RID: 819 RVA: 0x0000D944 File Offset: 0x0000BB44
		// (set) Token: 0x06000334 RID: 820 RVA: 0x0000D94B File Offset: 0x0000BB4B
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x0400019A RID: 410
		private static ResourceManager resourceMan;

		// Token: 0x0400019B RID: 411
		private static CultureInfo resourceCulture;
	}
}
