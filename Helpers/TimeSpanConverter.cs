using System;
using Windows.UI.Xaml.Data;

namespace App2.Helpers
{
	public class TimeSpanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			ValueType ts = TimeSpan.FromSeconds((double) value);

			return ts;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return null;
		}
	}
}