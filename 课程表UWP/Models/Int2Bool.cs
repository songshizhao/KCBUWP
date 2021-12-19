using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CommonShared_Core.Converters
{
	public class Int2BoolConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{

			//return true;

			var dv = (int)value;

			return (dv==0) ? false : true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
		
			if ((bool)value)
			{
				return 1;
			}
			else
			{
				return 0;
			}



		}
	}
}
