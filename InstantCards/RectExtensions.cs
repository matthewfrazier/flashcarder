using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protomeme
{
	public static class RectExtensions
	{
		public static System.Windows.Int32Rect ToInt32Rect(this System.Windows.Rect rect)
		{
			return new System.Windows.Int32Rect(
				(int)Math.Floor(rect.Left),
				(int)Math.Floor(rect.Top),
				(int)Math.Floor(rect.Width),
				(int)Math.Floor(rect.Height)
				);
		}
	}
}
