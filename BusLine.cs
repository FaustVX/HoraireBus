using System.Collections.Generic;
using System.Linq;

namespace App2
{
	public class BusLine
	{
		private readonly string _nameTemplate;

		public BusLine(string name, IEnumerable<BusStop> busStop)
		{
			_nameTemplate = name + " ({0} > {1})";

			BusStops = new List<BusStop>(busStop.OrderBy(s => s.Time));

			for (int i = 0; i < BusStops.Count; i++)
			{
				var stop = BusStops[i];

				if (stop.BusLine != null)
					BusStops[i] = stop.Clone();

				stop.BusLine = this;
			}

			GenerateName(this);
		}

		private static void GenerateName(BusLine line)
		{
			var stopsname = line.BusStops.Select(stop => stop.PhysicalStop.Name).ToArray();

			line.Name = string.Format(line._nameTemplate, stopsname.FirstOrDefault(), stopsname.LastOrDefault());
		}

		public string Name { get; private set; }
		public IList<BusStop> BusStops { get; private set; }

		public override string ToString()
		{
			return string.Format("Ligne {0}, {1} arrets, Durée de {2}",
				Name,
				BusStops.Count,
				BusStops.Last().Time - BusStops.First().Time);
		}
	}
}