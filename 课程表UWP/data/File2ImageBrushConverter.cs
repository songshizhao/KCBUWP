using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace 课程表UWP.data
{
	public class File2ImageBrushConverter:IValueConverter
	{
		public  object Convert(object value, Type targetType, object parameter, string language)
		{
			var SkinImageBrush = new ImageBrush();
			StorageFile file = value as StorageFile;

			using (IRandomAccessStream ir = file.OpenAsync(FileAccessMode.Read).GetResults())
			{
				BitmapImage bi = new BitmapImage();
				bi.SetSourceAsync(ir).GetResults();
				SkinImageBrush.ImageSource = bi;
				return SkinImageBrush;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}

	}
}
