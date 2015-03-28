using System;

namespace App2
{
	public class BusStop
	{
		public BusStop(BusLine busLine, TimeSpan time, PhysicalStop physicalStop)
			: this(time, physicalStop)
		{
			BusLine = busLine;
		}

		public BusStop(TimeSpan time, PhysicalStop physicalStop)
		{
			Time = time;
			PhysicalStop = physicalStop;
			if (!PhysicalStop.BusStops.Contains(this))
				PhysicalStop.BusStops.Add(this);
		}

		public BusLine BusLine { get; internal set; }
		public TimeSpan Time { get; private set; }
		public PhysicalStop PhysicalStop { get; private set; }
		public string Name { get { return PhysicalStop.Name; } }

		public override string ToString()
		{
			return string.Format("{0}, Ligne {1} à {2}",
				(PhysicalStop == null) ? "" : PhysicalStop.Name,
				(BusLine == null) ? "" : BusLine.Name,
				Time);
		}

		public BusStop Clone()
		{
			return new BusStop(BusLine, Time, PhysicalStop);
		}
	}
}