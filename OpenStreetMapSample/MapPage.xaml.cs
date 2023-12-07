using MapSampleCommon.CustomWidget;
using Mapsui.Styles;
using Mapsui.UI.Maui;
using Mapsui.UI;

namespace OpenStreetMapSample;

public partial class MapPage : ContentPage
{
    private CancellationTokenSource? gpsCancelation;
    public Func<MapView?, MapClickedEventArgs, bool>? Clicker { get; set; }

    public MapPage()
    {
        InitializeComponent();

        // nullable warning workaround
        var test = this.mapView ?? throw new InvalidOperationException();
        var test1 = this.info ?? throw new InvalidOperationException();
    }

    public MapPage(Action<IMapControl> setup, Func<MapView?, MapClickedEventArgs, bool>? c = null)
    {
        InitializeComponent();

        // nullable warning workaround
        var test = this.mapView ?? throw new InvalidOperationException();
        var test1 = this.info ?? throw new InvalidOperationException();

        mapView!.RotationLock = false;
        mapView.UnSnapRotationDegrees = 30;
        mapView.ReSnapRotationDegrees = 5;

        mapView.PinClicked += OnPinClicked;
        mapView.MapClicked += OnMapClicked;

        Compass.ReadingChanged += Compass_ReadingChanged;

        mapView.MyLocationLayer.UpdateMyLocation(new Position());

        mapView.Info += MapView_Info;
        mapView.Renderer.WidgetRenders[typeof(CustomWidget)] = new CustomWidgetSkiaRenderer();

        Task.Run(StartGPS);

        try
        {
            if (!Compass.IsMonitoring)
                Compass.Start(SensorSpeed.Default);
        }
        catch (Exception) { }

        setup(mapView);

        Clicker = c;
    }

    protected override void OnAppearing()
    {
        mapView.IsVisible = true;
        mapView.Refresh();
    }

    private void MapView_Info(object? sender,Mapsui.MapInfoEventArgs? e)
    {
        if (e?.MapInfo?.Feature != null)
        {
            foreach (var style in e.MapInfo.Feature.Styles)
            {
                if (style is CalloutStyle)
                {
                    style.Enabled = !style.Enabled;
                    e.Handled = true;
                }
            }

            mapView.Refresh();
        }
    }

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        e.Handled = Clicker?.Invoke(sender as MapView, e) ?? false;
        //Samples.SetPins(mapView, e);
        //Samples.DrawPolylines(mapView, e);
    }

    private void OnPinClicked(object? sender, PinClickedEventArgs e)
    {
        if (e.Pin != null)
        {
            if (e.NumOfTaps == 2)
            {
                // Hide Pin when double click
                //DisplayAlert($"Pin {e.Pin.Label}", $"Is at position {e.Pin.Position}", "Ok");
                e.Pin.IsVisible = false;
            }
            if (e.NumOfTaps == 1)
                if (e.Pin.Callout.IsVisible)
                    e.Pin.HideCallout();
                else
                    e.Pin.ShowCallout();
        }

        e.Handled = true;
    }

    public async void StartGPS()
    {
        this.gpsCancelation = new CancellationTokenSource();

        await Task.Run(async () => {
            while (!gpsCancelation.IsCancellationRequested)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                await Dispatcher.DispatchAsync(async () => {
                    var location = await Geolocation.GetLocationAsync(request, this.gpsCancelation.Token).ConfigureAwait(false);
                    if (location != null)
                    {
                        MyLocationPositionChanged(location);
                    }
                }).ConfigureAwait(false);

                await Task.Delay(200).ConfigureAwait(false);
            }
        }, gpsCancelation.Token).ConfigureAwait(false);
    }

    public void StopGPS()
    {
        this.gpsCancelation?.Cancel();
    }

    /// <summary>
    /// New informations from Geolocator arrived
    /// </summary>
    /// <param name="sender">Geolocator</param>
    /// <param name="e">Event arguments for new position</param>
    private void MyLocationPositionChanged(Location e)
    {
        Dispatcher.Dispatch(() => {
            mapView.MyLocationLayer.UpdateMyLocation(new Position(e.Latitude, e.Longitude));
            if (e.Course != null)
            {
                mapView.MyLocationLayer.UpdateMyDirection(e.Course.Value, mapView.Rotation);
            }

            if (e.Speed != null)
            {
                mapView.MyLocationLayer.UpdateMySpeed(e.Speed.Value);
            }
        });
    }

    private void Compass_ReadingChanged(object? sender, CompassChangedEventArgs e)
    {
        mapView.MyLocationLayer.UpdateMyViewDirection(e.Reading.HeadingMagneticNorth, mapView.Rotation, false);
    }
}