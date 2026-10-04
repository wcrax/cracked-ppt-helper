using System;
using System.Windows;

namespace PPTService
{
	// Token: 0x02000010 RID: 16
	public static class PointUtils
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public static Point ComputeCartesianCoordinate(double angle, double radius)
		{
			double num = 0.017453292519943295 * (angle - 90.0);
			double x = radius * Math.Cos(num);
			double y = radius * Math.Sin(num);
			return new Point(x, y);
		}
	}
}
