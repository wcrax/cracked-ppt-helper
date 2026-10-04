using System;
using System.Windows;
using System.Windows.Input;

namespace PPTService
{
	// Minimized state bar: shown while the main window is user-hidden.
	// - Press on the pill background and drag to move it.
	// - Press on the inner button restores the main UI (button handles its own click;
	//   the drag handlers ignore presses that originate inside the button).
	public partial class MinimizedBarWindow : Window
	{
		private bool _isDragging;

		private Point _dragStartPoint;

		public MinimizedBarWindow()
		{
			this.InitializeComponent();
		}

		public void ShowBar()
		{
			this.PositionAtBottomCenter();
			base.Opacity = 0.55;
			base.Show();
			base.Topmost = true;
		}

		public void HideBar()
		{
			base.Hide();
		}

		private void PositionAtBottomCenter()
		{
			double workWidth = SystemParameters.WorkArea.Width;
			double workHeight = SystemParameters.WorkArea.Height;
			base.Left = workWidth / 2.0 - base.Width / 2.0;
			base.Top = workHeight - base.Height - 6.0;
		}

		private const double DragThreshold = 8.0;

		private bool _dragPending;

		private void BarBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this._isDragging = false;
			this._dragPending = true;
			this._dragStartPoint = e.GetPosition(null);
		}

		private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (!this._dragPending || e.MouseDevice.LeftButton != MouseButtonState.Pressed)
			{
				return;
			}
			Point position = e.GetPosition(null);
			double offsetX = position.X - this._dragStartPoint.X;
			double offsetY = position.Y - this._dragStartPoint.Y;
			if (!this._isDragging)
			{
				if (Math.Abs(offsetX) < MinimizedBarWindow.DragThreshold && Math.Abs(offsetY) < MinimizedBarWindow.DragThreshold)
				{
					return;
				}
				this._isDragging = true;
				this.CaptureMouse();
			}
			base.Left += offsetX;
			base.Top += offsetY;
			e.Handled = true;
		}

		private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this._dragPending = false;
			if (this._isDragging)
			{
				this._isDragging = false;
				if (this.IsMouseCaptured)
				{
					this.ReleaseMouseCapture();
					// Swallow the release so the button doesn't fire Click after a drag.
					e.Handled = true;
				}
			}
		}

		private void RestoreButton_Click(object sender, RoutedEventArgs e)
		{
			ViewHelper.MainWindow.RestoreFromMinimizedBar();
		}

		private void Bar_OnMouseEnter(object sender, MouseEventArgs e)
		{
			base.Opacity = 0.9;
		}

		private void Bar_OnMouseLeave(object sender, MouseEventArgs e)
		{
			base.Opacity = 0.55;
		}
	}
}
