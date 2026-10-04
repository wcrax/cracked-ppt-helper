using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PPTService
{
	// Token: 0x0200000E RID: 14
	public class PiePiece : Canvas
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000086 RID: 134 RVA: 0x00003360 File Offset: 0x00001560
		// (remove) Token: 0x06000087 RID: 135 RVA: 0x00003398 File Offset: 0x00001598
		public event PiePiece.PiePieceClicked PiePieceClickedEvent;

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000033CD File Offset: 0x000015CD
		// (set) Token: 0x06000089 RID: 137 RVA: 0x000033DF File Offset: 0x000015DF
		public double Radius
		{
			get
			{
				return (double)base.GetValue(PiePiece.RadiusProperty);
			}
			set
			{
				base.SetValue(PiePiece.RadiusProperty, value);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600008A RID: 138 RVA: 0x000033F2 File Offset: 0x000015F2
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00003404 File Offset: 0x00001604
		public Brush Fill
		{
			get
			{
				return (Brush)base.GetValue(PiePiece.FillProperty);
			}
			set
			{
				base.SetValue(PiePiece.FillProperty, value);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003412 File Offset: 0x00001612
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00003424 File Offset: 0x00001624
		public Brush Stroke
		{
			get
			{
				return (Brush)base.GetValue(PiePiece.StrokeProperty);
			}
			set
			{
				base.SetValue(PiePiece.StrokeProperty, value);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00003432 File Offset: 0x00001632
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00003444 File Offset: 0x00001644
		public double InnerRadius
		{
			get
			{
				return (double)base.GetValue(PiePiece.InnerRadiusProperty);
			}
			set
			{
				base.SetValue(PiePiece.InnerRadiusProperty, value);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00003457 File Offset: 0x00001657
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00003469 File Offset: 0x00001669
		public double WedgeAngle
		{
			get
			{
				return (double)base.GetValue(PiePiece.WedgeAngleProperty);
			}
			set
			{
				base.SetValue(PiePiece.WedgeAngleProperty, value);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000092 RID: 146 RVA: 0x0000347C File Offset: 0x0000167C
		// (set) Token: 0x06000093 RID: 147 RVA: 0x0000348E File Offset: 0x0000168E
		public double RotationAngle
		{
			get
			{
				return (double)base.GetValue(PiePiece.RotationAngleProperty);
			}
			set
			{
				base.SetValue(PiePiece.RotationAngleProperty, value);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000034A1 File Offset: 0x000016A1
		// (set) Token: 0x06000095 RID: 149 RVA: 0x000034B3 File Offset: 0x000016B3
		public double CentreY
		{
			get
			{
				return (double)base.GetValue(PiePiece.CentreYProperty);
			}
			set
			{
				base.SetValue(PiePiece.CentreYProperty, value);
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000034C6 File Offset: 0x000016C6
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000034D8 File Offset: 0x000016D8
		public double CentreX
		{
			get
			{
				return (double)base.GetValue(PiePiece.CentreXProperty);
			}
			set
			{
				base.SetValue(PiePiece.CentreXProperty, value);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000034EB File Offset: 0x000016EB
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000034F3 File Offset: 0x000016F3
		public double PieceValue { get; set; }

		// Token: 0x0600009A RID: 154 RVA: 0x000034FC File Offset: 0x000016FC
		private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PiePiece).AddPathToCanvas();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003509 File Offset: 0x00001709
		public PiePiece()
		{
			this.AddPathToCanvas();
			base.MouseLeftButtonUp += this.PiePiece_MouseLeftButtonUp;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000352C File Offset: 0x0000172C
		private void PiePiece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(null));
			if (hitTestResult != null && hitTestResult.VisualHit is Path && hitTestResult.VisualHit is Path && this.PiePieceClickedEvent != null)
			{
				this.PiePieceClickedEvent(this);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003578 File Offset: 0x00001778
		private void AddPathToCanvas()
		{
			base.Children.Clear();
			Path element = this.ConstructPath();
			base.Children.Add(element);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000035A4 File Offset: 0x000017A4
		private Path ConstructPath()
		{
			if (this.WedgeAngle >= 360.0)
			{
				return new Path
				{
					Fill = this.Fill,
					Stroke = this.Stroke,
					StrokeThickness = 1.0,
					Data = new GeometryGroup
					{
						FillRule = FillRule.EvenOdd,
						Children = new GeometryCollection
						{
							new EllipseGeometry
							{
								Center = new Point(this.CentreX, this.CentreY),
								RadiusX = this.Radius,
								RadiusY = this.Radius
							},
							new EllipseGeometry
							{
								Center = new Point(this.CentreX, this.CentreY),
								RadiusX = this.InnerRadius,
								RadiusY = this.InnerRadius
							}
						}
					}
				};
			}
			new Point(this.CentreX, this.CentreY);
			Point point = PointUtils.ComputeCartesianCoordinate(this.RotationAngle, this.InnerRadius).OffsetEx(this.CentreX, this.CentreY);
			Point point2 = PointUtils.ComputeCartesianCoordinate(this.RotationAngle + this.WedgeAngle, this.InnerRadius).OffsetEx(this.CentreX, this.CentreY);
			Point point3 = PointUtils.ComputeCartesianCoordinate(this.RotationAngle, this.Radius).OffsetEx(this.CentreX, this.CentreY);
			Point point4 = PointUtils.ComputeCartesianCoordinate(this.RotationAngle + this.WedgeAngle, this.Radius).OffsetEx(this.CentreX, this.CentreY);
			bool isLargeArc = this.WedgeAngle > 180.0;
			Size size = new Size(this.Radius, this.Radius);
			Size size2 = new Size(this.InnerRadius, this.InnerRadius);
			PathFigure value = new PathFigure
			{
				StartPoint = point,
				Segments = new PathSegmentCollection
				{
					new ArcSegment
					{
						Point = point3,
						Size = new Size(2.0, 2.0),
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Clockwise,
						RotationAngle = 0.0
					},
					new ArcSegment
					{
						Point = point4,
						Size = size,
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Clockwise,
						RotationAngle = 0.0
					},
					new ArcSegment
					{
						Point = point2,
						Size = new Size(2.0, 2.0),
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Clockwise,
						RotationAngle = 0.0
					},
					new ArcSegment
					{
						Point = point,
						Size = size2,
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Counterclockwise,
						RotationAngle = 0.0
					}
				}
			};
			return new Path
			{
				Fill = this.Fill,
				Stroke = this.Stroke,
				StrokeThickness = 1.0,
				Data = new PathGeometry
				{
					Figures = new PathFigureCollection
					{
						value
					}
				}
			};
		}

		// Token: 0x04000022 RID: 34
		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000023 RID: 35
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000024 RID: 36
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000025 RID: 37
		public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register("InnerRadius", typeof(double), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000026 RID: 38
		public static readonly DependencyProperty WedgeAngleProperty = DependencyProperty.Register("WedgeAngle", typeof(double), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000027 RID: 39
		public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof(double), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000028 RID: 40
		public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register("CentreY", typeof(double), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x04000029 RID: 41
		public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register("CentreX", typeof(double), typeof(PiePiece), new PropertyMetadata(new PropertyChangedCallback(PiePiece.OnDependencyPropertyChanged)));

		// Token: 0x0200008A RID: 138
		// (Invoke) Token: 0x0600043C RID: 1084
		public delegate void PiePieceClicked(PiePiece sender);
	}
}
