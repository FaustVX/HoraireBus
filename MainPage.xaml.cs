using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using App2.Annotations;
using App2.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
	{
		private City _selectedCity;
		private ObservableCollection<PhysicalStop> _physicalStops;
		private PhysicalStop _physicalStopStart, _physicalStopStop;
		private TimeSpan _selectedTime;
	    private BusPath _selectedBusPath;

	    public ObservableCollection<City> Cities
		{ get; private set; }

		public City SelectedCity
		{
			get
			{ return _selectedCity; }
			set
			{
				if(Equals(value, _selectedCity))
					return;
				_selectedCity = value;
				OnPropertyChanged();

				PhysicalStops = new ObservableCollection<PhysicalStop>(SelectedCity.PhysicalStops);
			}
		}

		public ObservableCollection<PhysicalStop> PhysicalStops
		{
			get
			{ return _physicalStops; }
			set
			{
				if(Equals(value, _physicalStops))
					return;
				_physicalStops = value;
				OnPropertyChanged();

				PhysicalStopStart = PhysicalStops.FirstOrDefault();
				PhysicalStopStop = PhysicalStops.LastOrDefault();
			}
		}

		public PhysicalStop PhysicalStopStart
		{
			get
			{ return _physicalStopStart; }
			set
			{

				if(Equals(value, _physicalStopStart))
					return;
				_physicalStopStart = value;
				OnPropertyChanged();

				TimesStart = PhysicalStopStart.Times;
				OnPropertyChanged("TimesStart");

				var timesStart = TimesStart.ReadOnce();
				var t = timesStart.Select(ts => new TimeSpan?(ts))
					.FirstOrDefault(ts => ts.Value >= (DateTime.Now - DateTime.Today)) ??
				        timesStart.First();

				SelectedTime = t;
			}
		}

		public PhysicalStop PhysicalStopStop
		{
			get
			{ return _physicalStopStop; }
			set
			{
				if(Equals(value, _physicalStopStop))
					return;
				_physicalStopStop = value;
				OnPropertyChanged();
				GenerateBusPath();
			}
		}

		public TimeSpan SelectedTime
		{
			get
			{ return _selectedTime; }
			set
			{
				if(value.Equals(_selectedTime))
					return;
				_selectedTime = value;
				OnPropertyChanged();
				GenerateBusPath();
			}
		}

		public IEnumerable<TimeSpan> TimesStart { get; set; }

	    public BusPath SelectedBusPath
	    {
		    get { return _selectedBusPath; }
		    set
		    {
			    if (Equals(value, _selectedBusPath)) return;
			    _selectedBusPath = value;
			    OnPropertyChanged();
			    OnPropertyChanged("BusPathInfos");
		    }
	    }

	    public IEnumerable<BusPath> BusPaths { get; set; } 

		public string BusPathInfos
		{
			get { return ((object) SelectedBusPath ?? "").ToString(); }
		}

		private void GenerateBusPath()
		{
			if (PhysicalStopStart != null && PhysicalStopStop != null && PhysicalStopStart != PhysicalStopStop)
			{
				var buses = SelectedCity
					.TakeBusFromToAt(PhysicalStopStart, SelectedTime, PhysicalStopStop)
					.ReadOnce();
				BusPaths = buses;
				SelectedBusPath = BusPaths.FirstOrDefault(bp => bp.FirstOrDefault().Time >= SelectedTime) ?? buses.Last();
			}
			else
			{
				BusPaths = new BusPath[0];
				SelectedBusPath = null;
			}

			OnPropertyChanged("BusPaths");
		}

		public MainPage()
		{
			Cities = new ObservableCollection<City>(GenerateCities());

			DataContext = this;
			this.InitializeComponent();

			SelectedCity = Cities.FirstOrDefault();

			this.NavigationCacheMode = NavigationCacheMode.Required;
		}

		private static City[] GenerateCities()
		{
			var xdoc = XElement.Load(@"XML\Horaires.xml");

			var cities =
				from city in xdoc.Elements("City")
				let physics = (
					from physic in city.Element("PhysicalStops").Elements("PhysicalStop")
					select new { Physic = new PhysicalStop(physic.Attribute("name").Value), Id = physic.Attribute("id").Value })
					.ToArray()
				let lines =
					from line in city.Element("BusLines").Elements("BusLine")
					let stops =
						from stop in line.Elements("BusStop")
						select new BusStop(TimeSpan.Parse(stop.Attribute("time").Value),
							physics.First(p => p.Id == stop.Attribute("stop").Value).Physic)
					select new BusLine(line.Attribute("name").Value, stops)
				select new City(city.Attribute("name").Value, lines);

			return cities.ToArray();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// TODO: Prepare page for display here.

			// TODO: If your application contains multiple pages, ensure that you are
			// handling the hardware Back button by registering for the
			// Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
			// If you are using the NavigationHelper provided by some templates,
			// this event is handled for you.
		}

	    public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    var handler = PropertyChanged;
		    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
	    }

	    private void SwitchDestination(object sender, TappedRoutedEventArgs e)
	    {
		    var temp = PhysicalStopStart;
		    PhysicalStopStart = PhysicalStopStop;
		    PhysicalStopStop = temp;
	    }
	}
}
