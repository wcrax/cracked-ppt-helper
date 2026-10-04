using System;
using System.ComponentModel;

namespace PPTService
{
	// Token: 0x0200000D RID: 13
	public class CircularProgressBarViewModel : AttachedViewModelBase
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00003197 File Offset: 0x00001397
		// (set) Token: 0x06000074 RID: 116 RVA: 0x0000319F File Offset: 0x0000139F
		public double Percent
		{
			get
			{
				return this._percent;
			}
			set
			{
				this._percent = value;
				base.OnPropertyChanged("Percent");
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000075 RID: 117 RVA: 0x000031B3 File Offset: 0x000013B3
		// (set) Token: 0x06000076 RID: 118 RVA: 0x000031BB File Offset: 0x000013BB
		public double Diameter
		{
			get
			{
				return this._diameter;
			}
			set
			{
				this._diameter = value;
				base.OnPropertyChanged("Diameter");
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000031CF File Offset: 0x000013CF
		// (set) Token: 0x06000078 RID: 120 RVA: 0x000031D7 File Offset: 0x000013D7
		public double Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
				base.OnPropertyChanged("Radius");
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000031EB File Offset: 0x000013EB
		// (set) Token: 0x0600007A RID: 122 RVA: 0x000031F3 File Offset: 0x000013F3
		public double InnerRadius
		{
			get
			{
				return this._innerRadius;
			}
			set
			{
				this._innerRadius = value;
				base.OnPropertyChanged("InnerRadius");
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00003207 File Offset: 0x00001407
		// (set) Token: 0x0600007C RID: 124 RVA: 0x0000320F File Offset: 0x0000140F
		public double CentreX
		{
			get
			{
				return this._centreX;
			}
			set
			{
				this._centreX = value;
				base.OnPropertyChanged("CentreX");
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003223 File Offset: 0x00001423
		// (set) Token: 0x0600007E RID: 126 RVA: 0x0000322B File Offset: 0x0000142B
		public double CentreY
		{
			get
			{
				return this._centreY;
			}
			set
			{
				this._centreY = value;
				base.OnPropertyChanged("CentreY");
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600007F RID: 127 RVA: 0x0000323F File Offset: 0x0000143F
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00003247 File Offset: 0x00001447
		public double Angle
		{
			get
			{
				return this._angle;
			}
			set
			{
				this._angle = value;
				base.OnPropertyChanged("Angle");
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000325B File Offset: 0x0000145B
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00003263 File Offset: 0x00001463
		public double HoleSizeFactor
		{
			get
			{
				return this._holeSizeFactor;
			}
			set
			{
				this._holeSizeFactor = value;
				this.ComputeViewModelProperties();
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003274 File Offset: 0x00001474
		public override void ComputeViewModelProperties()
		{
			if (this._progressBar == null)
			{
				return;
			}
			this.Angle = (this._progressBar.Value - this._progressBar.Minimum) * 360.0 / (this._progressBar.Maximum - this._progressBar.Minimum);
			this.CentreX = this._progressBar.ActualWidth / 2.0;
			this.CentreY = this._progressBar.ActualHeight / 2.0;
			this.Radius = Math.Min(this.CentreX, this.CentreY);
			this.Diameter = this.Radius * 2.0;
			this.InnerRadius = this.Radius * this.HoleSizeFactor;
			this.Percent = this.Angle / 360.0;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002F72 File Offset: 0x00001172
		protected override void AdaptedDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		// Token: 0x04000019 RID: 25
		private double _angle;

		// Token: 0x0400001A RID: 26
		private double _centreX;

		// Token: 0x0400001B RID: 27
		private double _centreY;

		// Token: 0x0400001C RID: 28
		private double _radius;

		// Token: 0x0400001D RID: 29
		private double _innerRadius;

		// Token: 0x0400001E RID: 30
		private double _diameter;

		// Token: 0x0400001F RID: 31
		private double _percent;

		// Token: 0x04000020 RID: 32
		private double _holeSizeFactor;
	}
}
