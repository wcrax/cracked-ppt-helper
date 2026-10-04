using System;
using Cvte.Windows;

namespace PPTService
{
	// Token: 0x0200001F RID: 31
	public class ViewHelper
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000493F File Offset: 0x00002B3F
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00004946 File Offset: 0x00002B46
		public static Magnifier Magnifier { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000494E File Offset: 0x00002B4E
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00004955 File Offset: 0x00002B55
		public static LeftArea LeftArea { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000495D File Offset: 0x00002B5D
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00004964 File Offset: 0x00002B64
		public static RightArea RightArea { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000496C File Offset: 0x00002B6C
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00004973 File Offset: 0x00002B73
		public static BottomArea BottomArea { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000FE RID: 254 RVA: 0x0000497B File Offset: 0x00002B7B
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00004982 File Offset: 0x00002B82
		public static LeftBottomArea LeftBottomArea { get; private set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000100 RID: 256 RVA: 0x0000498A File Offset: 0x00002B8A
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00004991 File Offset: 0x00002B91
		public static RightBottomArea RightBottomArea { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00004999 File Offset: 0x00002B99
		// (set) Token: 0x06000103 RID: 259 RVA: 0x000049A0 File Offset: 0x00002BA0
		public static MainWindow MainWindow { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000104 RID: 260 RVA: 0x000049A8 File Offset: 0x00002BA8
		public static InnerBoardWindow InnerBoardWindow
		{
			get
			{
				if (ViewHelper._innerBoardWindow != null)
				{
					return ViewHelper._innerBoardWindow;
				}
				ViewHelper._innerBoardWindow = new InnerBoardWindow
				{
					Owner = ViewHelper.MainWindow
				};
				ControlHelper.SetAltF4Enabled(ViewHelper.InnerBoardWindow, false);
				return ViewHelper._innerBoardWindow;
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000049DC File Offset: 0x00002BDC
		public void Initialize()
		{
			PptOperator.Instance.ShowBegin += this.Instance_ShowBegin;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000049F4 File Offset: 0x00002BF4
		private void Instance_ShowBegin(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherInvoke(delegate
			{
				LoadingScreen<LoadingView>.Current = new LoadingScreen<LoadingView>();
				ViewHelper.LeftArea = new LeftArea();
				ViewHelper.RightArea = new RightArea();
				ViewHelper.LeftBottomArea = new LeftBottomArea();
				ViewHelper.RightBottomArea = new RightBottomArea();
				ViewHelper.BottomArea = new BottomArea();
				ViewHelper.MainWindow = new MainWindow();
				ControlHelper.SetAltF4Enabled(ViewHelper.MainWindow, false);
			});
			ViewHelper.MainWindow._pptOperatorShowBegin(null, null);
			PptOperator.Instance.ShowBegin -= this.Instance_ShowBegin;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004A47 File Offset: 0x00002C47
		public static void StopMainWindowUpdateTimerAndMouseGesture()
		{
			ViewHelper.MainWindow.IsNeedUpdateTimer = false;
			ViewHelper.MainWindow.UpdateTimer.Stop();
			MouseHelper.Instance.Stop();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00004A6D File Offset: 0x00002C6D
		public static void BeginMainWindowUpdateTimerAndMouseGesture()
		{
			ViewHelper.MainWindow.IsNeedUpdateTimer = true;
			ViewHelper.MainWindow.UpdateTimer.Start();
			MouseHelper.Instance.Start();
		}

		// Token: 0x0400004E RID: 78
		private static InnerBoardWindow _innerBoardWindow;
	}
}
