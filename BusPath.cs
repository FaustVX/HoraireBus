using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace App2
{
	public class BusPath : IEnumerable<BusStop>
	{
		public BusPath(IEnumerable<BusStop> busStops)
		{
			BusStops = new List<BusStop>(busStops);
		}

		public IList<BusStop> BusStops { get; private set; }

		public IEnumerator<BusStop> GetEnumerator()
		{
			return BusStops.GetEnumerator();
		}

		public string Infos
		{
			get
			{ return ToString(); }
		}

		public TimeSpan Start
		{
			get
			{ return BusStops.FirstOrDefault().Time; }
		}

		public string Summary
		{
			get
			{
				var start = BusStops.FirstOrDefault();
				var last = BusStops.LastOrDefault();
				return string.Format("à partir de {0} en {1} ({2} étapes)",
					start.Time,
					last.Time - start.Time,
					BusStops.Count);
			}
		}

		public override string ToString()
		{
			var start = BusStops.FirstOrDefault();
			var last = BusStops.LastOrDefault();
			return string.Format("De {0} à {1} à partir de {2} en {3} ({4} étapes)",
				start.Name,
				last.Name,
				start.Time,
				last.Time - start.Time,
				BusStops.Count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}