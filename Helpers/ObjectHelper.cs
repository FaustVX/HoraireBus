using System;
using System.Collections.Generic;
using System.Linq;

namespace App2.Helpers
{
	public static class ObjectHelper
	{
		public static readonly char[] LowerLetters, UpperLetters, Letters, Numbers, Alphabet;

		static ObjectHelper()
		{
			LowerLetters = ToCharArray(Enumerable.Range('a', 26));
			UpperLetters = ToCharArray(Enumerable.Range('A', 26));
			Letters = LowerLetters.Concat(UpperLetters).ToArray();
			Numbers = ToCharArray(Enumerable.Range('0', 10));
			Alphabet = Letters.Concat(Numbers).Concat(new[] {' '}).ToArray();
		}

		public static char[] ToCharArray(this IEnumerable<int> range)
		{
			return range
				.Select(i => (char) i)
				.ToArray();
		}

		public static TElement Do<TElement>(this TElement element, Action<TElement> action)
		{
			action(element);
			return element;
		}

		public static TReturn To<TElement, TReturn>(this TElement element, Func<TElement, TReturn> converter)
		{
			return converter(element);
		}
	}
}
