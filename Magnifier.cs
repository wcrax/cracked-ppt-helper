using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PPTService.Collection;

namespace PPTService
{
	// Token: 0x02000034 RID: 52
	public partial class Magnifier : Window, IComponentConnector
	{
		// Restored fields
		private System.Windows.Point _mouseStartPoint;

		private System.Windows.Point _rectangleStartPoint;

		private bool _isMagnifierRectangleMouseLeftButtonDown;

		private bool _isMagnifierBorderMouseLeftButtonDown;

		private double _magnifierBorderWidth;

		private double _magnifierBorderHeight;

		private double _enlargementFactor = 1.0;

		private static double _minEnlargementFactor = 1.0;

		private static double _maxEnlargementFactor = 5.0;

		private static int _buttonBorderVerticalAlignmentMargin = 60;

		private static int _buttonBorderHorizontalAlignmentMargin = 60;

		// Token: 0x060001D1 RID: 465 RVA: 0x00007E34 File Offset: 0x00006034
		public Magnifier()
		{
			this.InitializeComponent();
			base.DataContext = this;
			base.Loaded += this.Magnifier_Loaded;
			base.Unloaded += this.Magnifier_Unloaded;
			base.SizeChanged += delegate(object s, SizeChangedEventArgs e)
			{
				this.SetWindowRect();
			};
			base.LocationChanged += delegate(object s, EventArgs e)
			{
				this.SetWindowRect();
			};
			base.PreviewKeyDown += this.Magnifier_PreviewKeyDown;
			this.CloseButton.Click += delegate(object s, RoutedEventArgs e)
			{
				base.Close();
				FridayReporter.ReportEventImmediately(EventID.MagnifierExit);
			};
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00007EF4 File Offset: 0x000060F4
		private void Magnifier_Loaded(object sender, RoutedEventArgs e)
		{
			this.SetMagnifierShapeToRectangle();
			Rect screenBoundsByWindow = ScreenHelper.GetScreenBoundsByWindow(System.Windows.Application.Current.MainWindow);
			Canvas.SetLeft(this.RootGrid, screenBoundsByWindow.Left - base.Left + (screenBoundsByWindow.Width - this.RootGrid.ActualWidth) / 2.0);
			Canvas.SetTop(this.RootGrid, screenBoundsByWindow.Top - base.Top + (screenBoundsByWindow.Height - this.RootGrid.ActualHeight) / 2.0);
			this.UpdateMagnifierImageViewBox();
			this.InitializeMagnifierWindow();
			PptOperator.Instance.ShowEnd += this.Instance_ShowEnd;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00007FA7 File Offset: 0x000061A7
		private void Magnifier_Unloaded(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.ShowEnd -= this.Instance_ShowEnd;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00007FBF File Offset: 0x000061BF
		private void Instance_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(new Action(base.Close));
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00007FD4 File Offset: 0x000061D4
		private void InitializeMagnifierWindow()
		{
			System.Drawing.Rectangle virtualScreen = SystemInformation.VirtualScreen;
			this.RootCanvasBackgroundImage.ImageSource = Snapshot.Take(virtualScreen);
			this.MagnifierImage.ImageSource = this.RootCanvasBackgroundImage.ImageSource;
			this.SetWindowRect();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00008014 File Offset: 0x00006214
		private void SetWindowRect()
		{
			System.Drawing.Rectangle virtualScreen = SystemInformation.VirtualScreen;
			base.WindowStartupLocation = WindowStartupLocation.Manual;
			base.Left = (double)virtualScreen.Left;
			base.Top = (double)virtualScreen.Top;
			base.Width = (double)virtualScreen.Width / ScreenHelper.DpiFactor;
			base.Height = (double)virtualScreen.Height / ScreenHelper.DpiFactor;
			this.AdjustButtonBorderMargin();
			this.AdjustMagnifierBorder();
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000807E File Offset: 0x0000627E
		private void SetMagnifierShapeToRectangle()
		{
			this.MagnifierRectangle.RadiusX = 0.0;
			this.MagnifierRectangle.RadiusY = 0.0;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x000080A8 File Offset: 0x000062A8
		private void Magnifier_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key != Key.Escape)
			{
				return;
			}
			base.Close();
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x000080BC File Offset: 0x000062BC
		private void ChangeBackgroundImageCheckBox_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.MagnifierChangeTransparency);
			bool? isChecked = this.ChangeBackgroundImageCheckBox.IsChecked;
			bool flag = true;
			if (isChecked.GetValueOrDefault() == flag & isChecked != null)
			{
				Storyboard storyboard = base.Resources["HiddenBackgroundImageStoryboard"] as Storyboard;
				if (storyboard != null)
				{
					storyboard.Begin();
				}
				FridayReporter.ReportEventImmediately(EventID.MagnifierNonTransparentState);
				return;
			}
			Storyboard storyboard2 = base.Resources["ShowBackgroundImageStoryboard"] as Storyboard;
			if (storyboard2 != null)
			{
				storyboard2.Begin();
			}
			FridayReporter.ReportEventImmediately(EventID.MagnifierTransparentState);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000814C File Offset: 0x0000634C
		private void MagnifierRectangle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.MagnifierRectangle.CaptureDevice(null);
			this._mouseStartPoint = e.GetPosition(this);
			this._rectangleStartPoint.X = Canvas.GetLeft(this.RootGrid);
			this._rectangleStartPoint.Y = Canvas.GetTop(this.RootGrid);
			this._isMagnifierRectangleMouseLeftButtonDown = true;
			base.LayoutUpdated += this.Magnifier_LayoutUpdated;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x000081C0 File Offset: 0x000063C0
		private void MagnifierRectangle_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			e.Handled = true;
			if (!this._isMagnifierRectangleMouseLeftButtonDown)
			{
				return;
			}
			Vector vector = e.GetPosition(this) - this._mouseStartPoint;
			double length = this._rectangleStartPoint.X + vector.X;
			double length2 = this._rectangleStartPoint.Y + vector.Y;
			this.AdjustMagnifierPosition(ref length, ref length2);
			Canvas.SetLeft(this.RootGrid, length);
			Canvas.SetTop(this.RootGrid, length2);
			this.UpdateMagnifierImageViewBox();
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00008240 File Offset: 0x00006440
		private void MagnifierRectangle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this._isMagnifierRectangleMouseLeftButtonDown = false;
			base.LayoutUpdated -= this.Magnifier_LayoutUpdated;
			this.MagnifierRectangle.ReleaseDeviceCapture(null);
			FridayReporter.ReportEventImmediately(EventID.MagnifierMove);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00008278 File Offset: 0x00006478
		private void MagnifierBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			Border border = sender as Border;
			if (border != null)
			{
				border.CaptureDevice(null);
			}
			this._magnifierBorderWidth = this.MagnifierBorder.ActualWidth;
			this._magnifierBorderHeight = this.MagnifierBorder.ActualHeight;
			this._rectangleStartPoint.X = Canvas.GetLeft(this.RootGrid);
			this._rectangleStartPoint.Y = Canvas.GetTop(this.RootGrid);
			this._mouseStartPoint = e.GetPosition(this);
			this._isMagnifierBorderMouseLeftButtonDown = true;
			base.LayoutUpdated += this.Magnifier_LayoutUpdated;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00008314 File Offset: 0x00006514
		private void MagnifierBorder_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			e.Handled = true;
			if (!this._isMagnifierBorderMouseLeftButtonDown)
			{
				return;
			}
			Border border = e.OriginalSource as Border;
			if (border == null)
			{
				return;
			}
			Vector vector = e.GetPosition(this) - this._mouseStartPoint;
			string name = border.Name;
			if (!(name == "LeftTopThumbBorder"))
			{
				if (!(name == "RightTopThumbBorder"))
				{
					if (!(name == "LeftBottomThumbBorder"))
					{
						if (name == "RightBottomThumbBorder")
						{
							if (this._magnifierBorderWidth + vector.X * 2.0 > this.MagnifierBorder.MinWidth && this._magnifierBorderWidth + vector.X * 2.0 < this.MagnifierBorder.MaxWidth)
							{
								this.MagnifierBorder.Width = this._magnifierBorderWidth + vector.X * 2.0;
								Canvas.SetLeft(this.RootGrid, this._rectangleStartPoint.X - vector.X);
							}
							if (this._magnifierBorderHeight + vector.Y * 2.0 > this.MagnifierBorder.MinHeight && this._magnifierBorderHeight + vector.Y * 2.0 < this.MagnifierBorder.MaxHeight)
							{
								this.MagnifierBorder.Height = this._magnifierBorderHeight + vector.Y * 2.0;
								Canvas.SetTop(this.RootGrid, this._rectangleStartPoint.Y - vector.Y);
							}
						}
					}
					else
					{
						if (this._magnifierBorderWidth - vector.X * 2.0 > this.MagnifierBorder.MinWidth && this._magnifierBorderWidth - vector.X * 2.0 < this.MagnifierBorder.MaxWidth)
						{
							this.MagnifierBorder.Width = this._magnifierBorderWidth - vector.X * 2.0;
							Canvas.SetLeft(this.RootGrid, this._rectangleStartPoint.X + vector.X);
						}
						if (this._magnifierBorderHeight + vector.Y * 2.0 > this.MagnifierBorder.MinHeight && this._magnifierBorderHeight + vector.Y * 2.0 < this.MagnifierBorder.MaxHeight)
						{
							this.MagnifierBorder.Height = this._magnifierBorderHeight + vector.Y * 2.0;
							Canvas.SetTop(this.RootGrid, this._rectangleStartPoint.Y - vector.Y);
						}
					}
				}
				else
				{
					if (this._magnifierBorderWidth + vector.X * 2.0 > this.MagnifierBorder.MinWidth && this._magnifierBorderWidth + vector.X * 2.0 < this.MagnifierBorder.MaxWidth)
					{
						this.MagnifierBorder.Width = this._magnifierBorderWidth + vector.X * 2.0;
						Canvas.SetLeft(this.RootGrid, this._rectangleStartPoint.X - vector.X);
					}
					if (this._magnifierBorderHeight - vector.Y * 2.0 > this.MagnifierBorder.MinHeight && this._magnifierBorderHeight - vector.Y * 2.0 < this.MagnifierBorder.MaxHeight)
					{
						this.MagnifierBorder.Height = this._magnifierBorderHeight - vector.Y * 2.0;
						Canvas.SetTop(this.RootGrid, this._rectangleStartPoint.Y + vector.Y);
					}
				}
			}
			else
			{
				if (this._magnifierBorderWidth - vector.X * 2.0 > this.MagnifierBorder.MinWidth && this._magnifierBorderWidth - vector.X * 2.0 < this.MagnifierBorder.MaxWidth)
				{
					this.MagnifierBorder.Width = this._magnifierBorderWidth - vector.X * 2.0;
					Canvas.SetLeft(this.RootGrid, this._rectangleStartPoint.X + vector.X);
				}
				if (this._magnifierBorderHeight - vector.Y * 2.0 > this.MagnifierBorder.MinHeight && this._magnifierBorderHeight - vector.Y * 2.0 < this.MagnifierBorder.MaxHeight)
				{
					this.MagnifierBorder.Height = this._magnifierBorderHeight - vector.Y * 2.0;
					Canvas.SetTop(this.RootGrid, this._rectangleStartPoint.Y + vector.Y);
				}
			}
			this.AdjustButtonPosition(Canvas.GetLeft(this.RootGrid), Canvas.GetTop(this.RootGrid));
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00008832 File Offset: 0x00006A32
		private void MagnifierBorder_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this._isMagnifierBorderMouseLeftButtonDown = false;
			base.LayoutUpdated -= this.Magnifier_LayoutUpdated;
			Border border = sender as Border;
			if (border != null)
			{
				border.ReleaseDeviceCapture(null);
			}
			FridayReporter.ReportEventImmediately(EventID.MagnifierScaleFrame);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00008870 File Offset: 0x00006A70
		private void MagnifierRectangle_OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
		{
			e.ManipulationContainer = this.RootCanvas;
			e.Mode = (ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.Scale);
			base.LayoutUpdated += this.Magnifier_LayoutUpdated;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00008898 File Offset: 0x00006A98
		private void MagnifierRectangle_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			this.Enlarge(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x000088E4 File Offset: 0x00006AE4
		private void MagnifierRectangle_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			base.LayoutUpdated -= this.Magnifier_LayoutUpdated;
			FridayReporter.ReportEventImmediately(EventID.MagnifierScaleContent);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00008904 File Offset: 0x00006B04
		private void MagnifierRectangle_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scaleX = 1.05;
			if (e.Delta < 0)
			{
				scaleX = 0.95;
			}
			this.Enlarge(scaleX, 0.0, 0.0);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00008948 File Offset: 0x00006B48
		private void Enlarge(double scaleX, double translationX, double translationY)
		{
			double num = this._enlargementFactor * scaleX;
			if (num < Magnifier._minEnlargementFactor || num > Magnifier._maxEnlargementFactor)
			{
				return;
			}
			this._enlargementFactor = num;
			this.UpdateMagnifierImageViewBox();
			if (Math.Abs(scaleX - 1.0) > 1E-07)
			{
				return;
			}
			double length = Canvas.GetLeft(this.RootGrid) + translationX;
			double length2 = Canvas.GetTop(this.RootGrid) + translationY;
			this.AdjustMagnifierPosition(ref length, ref length2);
			Canvas.SetLeft(this.RootGrid, length);
			Canvas.SetTop(this.RootGrid, length2);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x000089D8 File Offset: 0x00006BD8
		private void Magnifier_LayoutUpdated(object sender, EventArgs e)
		{
			this.UpdateMagnifierImageViewBox();
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x000089E0 File Offset: 0x00006BE0
		private void UpdateMagnifierImageViewBox()
		{
			double num = Canvas.GetLeft(this.RootGrid) + this.InnerMagnifierBorder.BorderThickness.Left;
			double num2 = Canvas.GetTop(this.RootGrid) + this.InnerMagnifierBorder.BorderThickness.Top;
			double num3 = this.MagnifierRectangle.ActualWidth - this.MagnifierRectangle.ActualWidth / this._enlargementFactor;
			double num4 = this.MagnifierRectangle.ActualHeight - this.MagnifierRectangle.ActualHeight / this._enlargementFactor;
			num += num3 / 2.0;
			num2 += num4 / 2.0;
			double num5 = num + this.MagnifierRectangle.ActualWidth / this._enlargementFactor;
			double num6 = num2 + this.MagnifierRectangle.ActualHeight / this._enlargementFactor;
			num *= ScreenHelper.DpiFactor;
			num2 *= ScreenHelper.DpiFactor;
			num5 *= ScreenHelper.DpiFactor;
			num6 *= ScreenHelper.DpiFactor;
			this.MagnifierImage.Viewbox = new Rect(new System.Windows.Point(num, num2), new System.Windows.Point(num5, num6));
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00008AFC File Offset: 0x00006CFC
		private void AdjustMagnifierPosition(ref double left, ref double top)
		{
			double left2 = this.MagnifierBorder.Margin.Left;
			if (left < -this.MagnifierBorder.ActualWidth / 2.0 - left2)
			{
				left = -this.MagnifierBorder.ActualWidth / 2.0 - left2;
			}
			if (top < -this.MagnifierBorder.ActualHeight / 2.0 - left2)
			{
				top = -this.MagnifierBorder.ActualHeight / 2.0 - left2;
			}
			if (left > base.ActualWidth - this.MagnifierBorder.ActualWidth / 2.0 - left2)
			{
				left = base.ActualWidth - this.MagnifierBorder.ActualWidth / 2.0 - left2;
			}
			if (top > base.ActualHeight - this.MagnifierBorder.ActualHeight / 2.0 - left2)
			{
				top = base.ActualHeight - this.MagnifierBorder.ActualHeight / 2.0 - left2;
			}
			this.AdjustButtonPosition(left, top);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00008C18 File Offset: 0x00006E18
		private void AdjustButtonPosition(double left, double top)
		{
			double top2 = 0.0;
			double left2 = 0.0;
			double bottom = (double)Magnifier._buttonBorderVerticalAlignmentMargin;
			double right = (double)(-(double)Magnifier._buttonBorderHorizontalAlignmentMargin);
			double left3 = this.MagnifierBorder.Margin.Left;
			VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
			System.Windows.HorizontalAlignment horizontalAlignment = System.Windows.HorizontalAlignment.Right;
			VerticalAlignment verticalAlignment2 = this.ButtonBorder.VerticalAlignment;
			System.Windows.HorizontalAlignment horizontalAlignment2 = this.ButtonBorder.HorizontalAlignment;
			if (left < 0.0)
			{
				horizontalAlignment = System.Windows.HorizontalAlignment.Right;
				right = (double)(-(double)Magnifier._buttonBorderHorizontalAlignmentMargin);
			}
			if (top < 0.0)
			{
				verticalAlignment = VerticalAlignment.Bottom;
				bottom = (double)Magnifier._buttonBorderVerticalAlignmentMargin;
			}
			if (left > base.ActualWidth - this.MagnifierBorder.ActualWidth)
			{
				horizontalAlignment = System.Windows.HorizontalAlignment.Left;
				left2 = (double)(-(double)Magnifier._buttonBorderHorizontalAlignmentMargin);
			}
			if (top > base.ActualHeight - this.MagnifierBorder.ActualHeight - left3 * 2.0)
			{
				verticalAlignment = VerticalAlignment.Top;
				top2 = (double)Magnifier._buttonBorderVerticalAlignmentMargin;
			}
			this.ButtonBorder.Margin = new Thickness(left2, top2, right, bottom);
			this.ButtonBorder.HorizontalAlignment = horizontalAlignment;
			this.ButtonBorder.VerticalAlignment = verticalAlignment;
			if (horizontalAlignment != horizontalAlignment2 || verticalAlignment != verticalAlignment2)
			{
				Storyboard storyboard = base.Resources["ShowButtonStoryboard"] as Storyboard;
				if (storyboard == null)
				{
					return;
				}
				storyboard.Begin();
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00008D50 File Offset: 0x00006F50
		private void AdjustButtonBorderMargin()
		{
			if (base.Height > 2100.0)
			{
				Magnifier._buttonBorderHorizontalAlignmentMargin = 110;
				Magnifier._buttonBorderVerticalAlignmentMargin = 80;
			}
			if (base.Height > 770.0 && base.Height <= 2100.0)
			{
				Magnifier._buttonBorderHorizontalAlignmentMargin = 55;
				Magnifier._buttonBorderVerticalAlignmentMargin = 40;
			}
			if (base.Height > 0.0 && base.Height <= 770.0)
			{
				Magnifier._buttonBorderHorizontalAlignmentMargin = 35;
				Magnifier._buttonBorderVerticalAlignmentMargin = 25;
			}
			this.ButtonBorder.Margin = new Thickness(0.0, 0.0, (double)(-(double)Magnifier._buttonBorderHorizontalAlignmentMargin), (double)Magnifier._buttonBorderVerticalAlignmentMargin);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00008E0C File Offset: 0x0000700C
		private void AdjustMagnifierBorder()
		{
			double num = 450.0;
			if (base.Height > 2100.0)
			{
				num = 900.0;
			}
			if (base.Height > 770.0 && base.Height <= 2100.0)
			{
				num = 450.0;
			}
			if (base.Height > 0.0 && base.Height <= 770.0)
			{
				num = 300.0;
			}
			this.MagnifierBorder.Width = num;
			this.MagnifierBorder.Height = num;
			this.MagnifierBorder.MaxWidth = base.Width;
			this.MagnifierBorder.MaxHeight = base.Height;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00008ED0 File Offset: 0x000070D0
		public void Operate(System.Windows.Point point, double length)
		{
			double x = point.X;
			double y = point.Y;
			this.AdjustMagnifierPosition(ref x, ref y);
			this.RootGrid.Width = length;
			this.RootGrid.Height = length;
			Canvas.SetLeft(this.RootGrid, x);
			Canvas.SetTop(this.RootGrid, y);
			this.UpdateMagnifierImageViewBox();
		}
	}

}
