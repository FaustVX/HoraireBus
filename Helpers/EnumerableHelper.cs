using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace App2.Helpers
{
	public static class EnumerableHelper
	{
		private static readonly Random Rnd;

		static EnumerableHelper()
		{
			Rnd = new Random();
		}

		public static IEnumerable<TElement> Shuffle<TElement>(this IEnumerable<TElement> source)
		{
			return source.OrderBy(e => Rnd.Next());
		}

		public static TElement Random<TElement>(this IEnumerable<TElement> source)
		{
			return source.Shuffle().FirstOrDefault();
		}

		public static TResult To<TElement, TResult>(this IEnumerable<TElement> source,
			Func<IEnumerable<TElement>, TResult> converter)
			where TResult : IEnumerable<TElement>
		{
			return converter(source);
		}

		public static IEnumerable<TElement> ForEach<TElement>(this IEnumerable<TElement> source, Action<TElement> action)
		{
			foreach (var element in source)
			{
				action(element);
				yield return element;
			}
		}

		public static IEnumerable<TResult> ForEach<TElement, TResult>(this IEnumerable<TElement> source,
			Func<TElement, TResult> func)
		{
			return source.Select(func);
		}

		public static IEnumerable<TElement> Add<TElement>(this IEnumerable<TElement> source, TElement element)
		{
			foreach (var el in source)
				yield return el;
			yield return element;
		}

		public static IEnumerable<TElement> TakeWhileThenAdd<TElement>(this IEnumerable<TElement> source, Predicate<TElement> predicate, int count)
		{
			var s = source.ReadOnce();
			var pred = new Func<TElement, bool>(predicate);
			return s.TakeWhile(pred).Concat(s.SkipWhile(pred).Take(count));
		}

		public static ReadOnceEnumerable<TElement> ReadOnce<TElement>(this IEnumerable<TElement> source)
		{
			return new ReadOnceEnumerable<TElement>(source);
		}
	}

	public class ReadOnceEnumerable<TElement> : IEnumerable<TElement>
	{
		private readonly List<TElement> _list;
		private readonly IEnumerator<TElement> _enumList;

		public ReadOnceEnumerable(IEnumerable<TElement> list)
		{
			_list = new List<TElement>();
			_enumList = list.GetEnumerator();
		}

		private bool Next(out TElement element, ref int count, ref IEnumerator<TElement> enumerator)
		{
			var res1 = enumerator.MoveNext();
			var res2 = res1 || _enumList.MoveNext();

				count++;

			if (res1)
			{
				element = enumerator.Current;
			}
			else if (res2)
			{
				_list.Add(element = _enumList.Current);
				enumerator = ((IEnumerable<TElement>) _list).GetEnumerator();
				for (int i = 0; i < count; i++)
					enumerator.MoveNext();
			}
			else
				element = default(TElement);

			return res2;
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			int count = 0;
			TElement element;
			var enumerator = ((IEnumerable<TElement>) _list).GetEnumerator();
             while(Next(out element, ref count, ref enumerator))
				yield return element;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
