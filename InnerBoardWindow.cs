using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x0200002D RID: 45
	public partial class InnerBoardWindow : Window, IComponentConnector
	{
		// Restored fields
		// Restored decompiled fields
		public const int MaxSlides = 20;

		public const double WindowHeight = 1080.0;

		private StrokeCollection[] _strokeCollection;

		private int _totalIndex;

		private int _currentIndex;

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00006A56 File Offset: 0x00004C56
		// (set) Token: 0x06000189 RID: 393 RVA: 0x00006A5E File Offset: 0x00004C5E
		public bool IsShowing { get; set; }

		// Token: 0x0600018A RID: 394 RVA: 0x00006A68 File Offset: 0x00004C68
		public InnerBoardWindow()
		{
			this.InitializeComponent();
			this._strokeCollection = new StrokeCollection[InnerBoardWindow.MaxSlides];
			base.Loaded += this.InnerBoardWindow_Loaded;
			this.CloseBorder.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
			{
				this.HiddenWindow();
				FridayReporter.ReportEventImmediately(EventID.InnerBoardExit);
			};
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00006AC8 File Offset: 0x00004CC8
		private void InnerBoardWindow_Loaded(object sender, RoutedEventArgs e)
		{
			base.Top = 0.0;
			this.InkCanvas.DefaultDrawingAttributes.Width = 3.0;
			this.InkCanvas.DefaultDrawingAttributes.Height = 3.0;
			this.InkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
			this._strokeCollection = new StrokeCollection[InnerBoardWindow.MaxSlides];
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00006B3B File Offset: 0x00004D3B
		public void ClearInkCanvas()
		{
			this.InkCanvas.Strokes.Clear();
			this._strokeCollection = new StrokeCollection[InnerBoardWindow.MaxSlides];
			this._totalIndex = 1;
			this._currentIndex = 1;
			this.UpdateIndex();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00006B71 File Offset: 0x00004D71
		public void SetPenToInkMode()
		{
			this.InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00006B7F File Offset: 0x00004D7F
		public void SetPen(Color color)
		{
			this.InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
			this.InkCanvas.DefaultDrawingAttributes.Color = color;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00006B9E File Offset: 0x00004D9E
		public void SetRubber()
		{
			this.InkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
			this.InkCanvas.EraserShape = new EllipseStylusShape(30.0, 30.0);
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00006BD0 File Offset: 0x00004DD0
		public void ShowWindow()
		{
			if (this.IsShowing)
			{
				return;
			}
			this.IsShowing = true;
			base.Show();
			this.ChangeWindowRect();
			this.InkCanvas.DefaultDrawingAttributes.Width = (double)((base.Height > (double)InnerBoardWindow.WindowHeight) ? 6 : 3);
			this.InkCanvas.DefaultDrawingAttributes.Height = (double)((base.Height > (double)InnerBoardWindow.WindowHeight) ? 6 : 3);
			Storyboard storyboard = base.Resources["ShowStoryBoard"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00006C60 File Offset: 0x00004E60
		public void HiddenWindow()
		{
			if (!this.IsShowing)
			{
				return;
			}
			this.IsShowing = false;
			ViewHelper.BottomArea.InnerBoardCheckBox.IsChecked = new bool?(false);
			ViewHelper.MainWindow.LowMaskButton.Visibility = Visibility.Collapsed;
			Storyboard storyboard = base.Resources["HiddenStoryBoard"] as Storyboard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Begin();
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00006CC1 File Offset: 0x00004EC1
		public void HiddenAndClearInkCanvas()
		{
			this.HiddenWindow();
			this.ClearInkCanvas();
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00006CD0 File Offset: 0x00004ED0
		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			if (this._currentIndex <= 1)
			{
				return;
			}
			this._strokeCollection[this._currentIndex] = this.InkCanvas.Strokes.Clone();
			InkCanvas inkCanvas = this.InkCanvas;
			StrokeCollection[] strokeCollection = this._strokeCollection;
			int num = this._currentIndex - 1;
			this._currentIndex = num;
			inkCanvas.Strokes = (strokeCollection[num] ?? new StrokeCollection());
			this.UpdateIndex();
			FridayReporter.ReportEventImmediately(EventID.InnerBoardPreviousPage);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00006D40 File Offset: 0x00004F40
		private void Next_OnClick(object sender, RoutedEventArgs e)
		{
			if (this._currentIndex + 1 >= InnerBoardWindow.MaxSlides)
			{
				return;
			}
			FridayReporter.ReportEventImmediately((this._currentIndex == this._totalIndex) ? EventID.InnerBoardAddPage : EventID.InnerBoardNextPage);
			this._strokeCollection[this._currentIndex] = this.InkCanvas.Strokes.Clone();
			InkCanvas inkCanvas = this.InkCanvas;
			StrokeCollection[] strokeCollection = this._strokeCollection;
			int num = this._currentIndex + 1;
			this._currentIndex = num;
			inkCanvas.Strokes = (strokeCollection[num] ?? new StrokeCollection());
			this._totalIndex = Math.Max(this._currentIndex, this._totalIndex);
			this.UpdateIndex();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00006DE4 File Offset: 0x00004FE4
		private void UpdateIndex()
		{
			this.Index.Text = this._currentIndex.ToString() + "/" + this._totalIndex.ToString();
			this.Add.Visibility = ((this._currentIndex == this._totalIndex) ? Visibility.Visible : Visibility.Collapsed);
			this.Next.Visibility = ((this._currentIndex == this._totalIndex) ? Visibility.Collapsed : Visibility.Visible);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00006E58 File Offset: 0x00005058
		private void UpdateState()
		{
			PpSlideShowPointerType pointerType = PptOperator.Instance.PointerType;
			if (pointerType == PpSlideShowPointerType.ppSlideShowPointerPen)
			{
				this.SetPen(PptOperator.Instance.PointerColor);
				return;
			}
			if (pointerType != PpSlideShowPointerType.ppSlideShowPointerEraser)
			{
				ViewHelper.BottomArea.PenClick();
				this.SetPen(PptOperator.Instance.PointerColor);
				return;
			}
			this.InkCanvas.DefaultDrawingAttributes.Color = PptOperator.Instance.PointerColor;
			this.SetRubber();
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00006EC6 File Offset: 0x000050C6
		public void SetNone()
		{
			this.InkCanvas.EditingMode = InkCanvasEditingMode.None;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00006ED4 File Offset: 0x000050D4
		private void ShowStoryBoard_OnCompleted(object sender, EventArgs e)
		{
			Transform renderTransform = this.RootGrid.RenderTransform;
			this.RootGrid.RenderTransform = null;
			this.RootGrid.RenderTransform = renderTransform;
			this.UpdateState();
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00006F0C File Offset: 0x0000510C
		private void HiddenStoryBoard_OnCompleted(object sender, EventArgs e)
		{
			if (ViewHelper.MainWindow.IsExiting)
			{
				PptOperator.Instance.Exit();
			}
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			base.Width = 1.0;
			base.Height = 1.0;
			DispatcherTask.Start(100.0, new Action(base.Hide));
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00006F78 File Offset: 0x00005178
		public void ChangeWindowRect()
		{
			base.Left = ViewHelper.MainWindow.Left;
			base.Top = ViewHelper.MainWindow.Top;
			base.Width = ViewHelper.MainWindow.Width;
			base.Height = ViewHelper.MainWindow.Height;
		}
	}

}
