using System;
using System.Collections.Generic;
using System.Linq;

namespace App2
{
	public class PhysicalStop
	{
		public PhysicalStop(string name, City city, IEnumerable<BusStop> busStops)
		{
			Name = name;
			City = city;
			BusStops = new List<BusStop>(busStops);

			Times = (
				from stop in BusStops
				orderby stop.Time
				select stop.Time)
				.Distinct();
		}

		public PhysicalStop(string name)
			: this(name, null, new BusStop[0])
		{ }

		public IEnumerable<TimeSpan> Times { get; private set; }

		public IList<BusStop> BusStops { get; private set; }
		public string Name { get; private set; }
		public City City { get; internal set; }

		public override string ToString()
		{
			return string.Format("{0} à {1}", Name, (City == null) ? "" : City.Name);
		}
	}
}