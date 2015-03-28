using System;
using System.Collections.Generic;
using System.Linq;
using App2.Helpers;

namespace App2
{
	public class City
	{
		public string Name { get; private set; }

		public City(string name, IEnumerable<BusLine> busLine)
		{
			Name = name;
			BusLines = new List<BusLine>(busLine);
			GenerateCityProperties(this);
		}

		private static void GenerateCityProperties(City city)
		{
			city.BusStops = (
				from line in city.BusLines
				from stop in line.BusStops
				select stop)
				.Distinct()
				.ToArray();

			city.PhysicalStops = (
				from stop in city.BusStops
				select stop.PhysicalStop)
				.Distinct()
				.ToArray();

			foreach (var stop in city.PhysicalStops)
				stop.City = city;
		}

		public IList<BusLine> BusLines { get; private set; }
		public IEnumerable<BusStop> BusStops { get; private set; }
		public IEnumerable<PhysicalStop> PhysicalStops { get; private set; }

		public IEnumerable<BusStop> TakeBusAt(PhysicalStop stop)
		{
			return
				from line in BusLines
				from s in line.BusStops
				where s.PhysicalStop == stop
				orderby s.Time
				select s;
		}

		public Tuple<IEnumerable<BusPath>, IEnumerable<BusPath>> TakeBusFromToAtAndBack(PhysicalStop start, TimeSpan go, PhysicalStop stop, TimeSpan back)
		{
			return Tuple.Create(TakeBusFromToAt(start, go, stop), TakeBusFromToAt(stop, back, start));
		}

		public IEnumerable<BusPath> TakeBusFromToAt(PhysicalStop start, TimeSpan time, PhysicalStop stop)
		{
			var stops = TakeBusAt(start)
				.Where(s => s.BusLine.BusStops
					.SkipWhile(s2 => s2.PhysicalStop != start)
					.Select(s2 => s2.PhysicalStop)
					.Contains(stop));

			var busStops = stops.ToArray();
			return busStops.Where(n => n.Time < time).Reverse().Take(2).Reverse()
				.Concat(busStops.Where(n => n.Time >= time).Take(3))
				.Select(s => new BusPath(
					s.BusLine.BusStops
						.SkipWhile(s2 => s2.PhysicalStop != start)
						.TakeWhileThenAdd(s2 => s2.PhysicalStop != stop, 1)
					));
		}

		public override string ToString()
		{
			return string.Format("{0}, {1} lignes", Name, BusLines.Count);
		}
	}
}