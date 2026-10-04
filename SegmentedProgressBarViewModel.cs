using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PPTService
{
	// Token: 0x02000012 RID: 18
	public class SegmentedProgressBarViewModel : CircularProgressBarViewModel
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00003CA5 File Offset: 0x00001EA5
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00003CAD File Offset: 0x00001EAD
		public int SegmentCount
		{
			get
			{
				return this._segmentCount;
			}
			set
			{
				this._segmentCount = value;
				this.BuildSegments();
				this.ComputeViewModelProperties();
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00003CC2 File Offset: 0x00001EC2
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00003CCA File Offset: 0x00001ECA
		public List<SegmentedProgressBarViewModel.SegmentData> Segments
		{
			get
			{
				return this._segments;
			}
			set
			{
				this._segments = value;
				base.OnPropertyChanged("Segments");
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003CDE File Offset: 0x00001EDE
		public SegmentedProgressBarViewModel()
		{
			this.BuildSegments();
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00003CF4 File Offset: 0x00001EF4
		private void BuildSegments()
		{
			List<SegmentedProgressBarViewModel.SegmentData> list = new List<SegmentedProgressBarViewModel.SegmentData>();
			double wedge = 360.0 / (double)this.SegmentCount;
			for (int i = 0; i < this.SegmentCount; i++)
			{
				double start = (double)i * 360.0 / (double)this.SegmentCount;
				list.Add(new SegmentedProgressBarViewModel.SegmentData(start, wedge, this));
			}
			this.Segments = list;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003D54 File Offset: 0x00001F54
		public override void ComputeViewModelProperties()
		{
			if (this._progressBar == null)
			{
				return;
			}
			double num = this._progressBar.Value / (this._progressBar.Maximum - this._progressBar.Minimum);
			for (int i = 0; i < this.SegmentCount; i++)
			{
				double num2 = (double)i / (double)this.SegmentCount;
				double num3 = (double)(i + 1) / (double)this.SegmentCount;
				double num4 = (num - num2) / (num3 - num2);
				num4 = Math.Min(1.0, Math.Max(0.0, num4));
				this._segments[i].Opacity = num4;
			}
			base.ComputeViewModelProperties();
		}

		// Token: 0x0400002E RID: 46
		private int _segmentCount = 8;

		// Token: 0x0400002F RID: 47
		private List<SegmentedProgressBarViewModel.SegmentData> _segments;

		// Token: 0x0200008B RID: 139
		public class SegmentData : INotifyPropertyChanged
		{
			// Token: 0x1700009D RID: 157
			// (get) Token: 0x0600043F RID: 1087 RVA: 0x0000ED05 File Offset: 0x0000CF05
			// (set) Token: 0x06000440 RID: 1088 RVA: 0x0000ED0D File Offset: 0x0000CF0D
			public double StartAngle { get; private set; }

			// Token: 0x1700009E RID: 158
			// (get) Token: 0x06000441 RID: 1089 RVA: 0x0000ED16 File Offset: 0x0000CF16
			// (set) Token: 0x06000442 RID: 1090 RVA: 0x0000ED1E File Offset: 0x0000CF1E
			public double WedgeAngle { get; private set; }

			// Token: 0x1700009F RID: 159
			// (get) Token: 0x06000443 RID: 1091 RVA: 0x0000ED27 File Offset: 0x0000CF27
			// (set) Token: 0x06000444 RID: 1092 RVA: 0x0000ED2F File Offset: 0x0000CF2F
			public double Opacity
			{
				get
				{
					return this._opacity;
				}
				set
				{
					this._opacity = value;
					this.OnPropertyChanged("Opacity");
				}
			}

			// Token: 0x170000A0 RID: 160
			// (get) Token: 0x06000445 RID: 1093 RVA: 0x0000ED43 File Offset: 0x0000CF43
			// (set) Token: 0x06000446 RID: 1094 RVA: 0x0000ED4B File Offset: 0x0000CF4B
			public SegmentedProgressBarViewModel Parent { get; private set; }

			// Token: 0x06000447 RID: 1095 RVA: 0x0000ED54 File Offset: 0x0000CF54
			public SegmentData(double start, double wedge, SegmentedProgressBarViewModel parent)
			{
				this.StartAngle = start;
				this.WedgeAngle = wedge;
				this.Parent = parent;
				this.Opacity = 0.0;
			}

			// Token: 0x14000010 RID: 16
			// (add) Token: 0x06000448 RID: 1096 RVA: 0x0000ED80 File Offset: 0x0000CF80
			// (remove) Token: 0x06000449 RID: 1097 RVA: 0x0000EDB8 File Offset: 0x0000CFB8
			public event PropertyChangedEventHandler PropertyChanged;

			// Token: 0x0600044A RID: 1098 RVA: 0x0000EDED File Offset: 0x0000CFED
			protected void OnPropertyChanged(string property)
			{
				if (this.PropertyChanged != null)
				{
					this.PropertyChanged(this, new PropertyChangedEventArgs(property));
				}
			}

			// Token: 0x04000229 RID: 553
			private double _opacity;
		}
	}
}
