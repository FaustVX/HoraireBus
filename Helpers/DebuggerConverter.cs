using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace App2.Helpers
{
	public class DebuggerConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			Debugger.Break();

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			Debugger.Break();

			return value;
		}
	}
}