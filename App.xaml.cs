using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x02000048 RID: 72
	public partial class App : Application
	{
		// Token: 0x060002DF RID: 735 RVA: 0x0000BE90 File Offset: 0x0000A090
		protected override void OnStartup(StartupEventArgs args)
		{
			base.OnStartup(args);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			this.StartProcess();
			stopwatch.Stop();
			DispatcherTask.Start(6000.0, delegate
			{
				SyntacticSugar.TryCatchAction(delegate
				{
					string eventValue = stopwatch.ElapsedMilliseconds.ConvertMilliseconds();
					FridayReporter.ReportEventDelayed(EventID.StartTime, eventValue);
				}, null);
			});
			bool flag;
			App._mutex = new Mutex(true, App.AppMutex, out flag);
			if (flag)
			{
				return;
			}
			if (!args.Args.Any<string>() || args.Args.First<string>() != App.AppRestart)
			{
				Environment.Exit(0);
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000BF2C File Offset: 0x0000A12C
		private void StartProcess()
		{
			AppPath.ClearTempFolder();
			ConfigHelper.Instance = new ConfigHelper();
			this._pptOperator = new PptOperator();
			PptOperator.Instance = this._pptOperator;
			this._pptOperator.Initialize();
			MouseHelper.Instance = new MouseHelper();
			new ViewHelper().Initialize();
			App.Version = App.GetAttribute<AssemblyFileVersionAttribute>().Version;
			FridayReporter.Initialize(App.AppName, App.Version);
			this.CatchUnhandledException();
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000BFA1 File Offset: 0x0000A1A1
		private void CatchUnhandledException()
		{
			base.DispatcherUnhandledException += delegate(object s, DispatcherUnhandledExceptionEventArgs e)
			{
				this.PostErrorAndShutdown(new UnhandledExceptionEventArgs(e, false));
			};
			AppDomain.CurrentDomain.UnhandledException += delegate(object s, UnhandledExceptionEventArgs e)
			{
				this.PostErrorAndShutdown(e);
			};
			Dispatcher.CurrentDispatcher.UnhandledException += delegate(object s, DispatcherUnhandledExceptionEventArgs e)
			{
				this.PostErrorAndShutdown(new UnhandledExceptionEventArgs(e, false));
			};
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000BFE1 File Offset: 0x0000A1E1
		private void PostErrorAndShutdown(UnhandledExceptionEventArgs args)
		{
			Log.WriteError(args.ExceptionObject.ToString());
			if (ConfigHelper.Instance.IsComOk)
			{
				Restart.PPTService();
			}
			Environment.Exit(0);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000C00A File Offset: 0x0000A20A
		private static T GetAttribute<T>() where T : Attribute
		{
			return (T)((object)Assembly.GetAssembly(Application.Current.GetType()).GetCustomAttributes(typeof(T), false)[0]);
		}

		// Token: 0x04000174 RID: 372
		public static string Version;

		// Token: 0x04000175 RID: 373
		private static string AppRestart = "Restart";

		// Token: 0x04000176 RID: 374
		public static readonly string AppName = "PPTService";

		// Token: 0x04000177 RID: 375
		private static string AppMutex = "#P_#P_#T_&S_&e_&r_*v_*i_*c_$e";

		// Token: 0x04000178 RID: 376
		private PptOperator _pptOperator;

		// Token: 0x04000179 RID: 377
		private static Mutex _mutex;
	}
}
