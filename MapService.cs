using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace MapLogger
{
    public class MapService
    {
        private readonly GMapControl gmap;
        private GMapMarker? tempMarker;
        public event Action<LocationModel>? SavedMarkerClicked;

        public MapService(GMapControl gmapControl)
        {
            gmap = gmapControl;
        }

            public void InitializeMap()     // setup Gmap
        {
            gmap.MapProvider = GMapProviders.OpenStreetMap;
                GMaps.Instance.Mode = AccessMode.ServerOnly;        // or ServerAndCache for limited offline support

                gmap.Position = new PointLatLng(51.4545, -2.5879);     // bristol starting point 
            gmap.MinZoom = 2;
            gmap.MaxZoom = 18;
            gmap.Zoom = 10;
            gmap.ShowCenter = false;
            gmap.CanDragMap = true;
            gmap.DragButton = MouseButton.Left;
        }

            public void ZoomIn()
            {
                if (gmap.Zoom < gmap.MaxZoom)
                {
                    gmap.Zoom += 1;
                }
            }

            public void ZoomOut()
            {
                if (gmap.Zoom > gmap.MinZoom)
                {
                    gmap.Zoom -= 1;
                }
            }

        public void SetTemporaryMarker(double lat, double lng)
        {
            if (tempMarker != null)
                {
                gmap.Markers.Remove(tempMarker);
                }

            tempMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    Fill = Brushes.Blue
                }
            };

            gmap.Markers.Add(tempMarker);
            gmap.Position = new PointLatLng(lat, lng);
                gmap.Zoom += 1;             // Increase zoom when searching point
        }

        public LocationModel? SaveTemporaryMarker()
        {
             if (tempMarker == null) return null;

            var model = new LocationModel
            {
                Latitude = tempMarker.Position.Lat,
                Longitude = tempMarker.Position.Lng,
                Timestamp = System.DateTime.Now
            };

            var savedMarker = CreateSavedMarker(model);
            gmap.Markers.Add(savedMarker);
            gmap.Markers.Remove(tempMarker);
            tempMarker = null;

            return model;
        }

        public void AddSavedMarkers(List<LocationModel> locations)
        {
            foreach (var loc in locations)
                gmap.Markers.Add(CreateSavedMarker(loc));
        }

        private GMapMarker CreateSavedMarker(LocationModel location)
        {
            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
                Cursor = Cursors.Hand
            };

            ellipse.MouseLeftButtonUp += (s, e) =>
            {
                e.Handled = true;
                SavedMarkerClicked?.Invoke(location);
            };

            return new GMapMarker(new PointLatLng(location.Latitude, location.Longitude))
            {
                Shape = ellipse
            };
        }

        public void RemoveMarker(LocationModel location)
        {
            var marker = gmap.Markers.FirstOrDefault(m =>
                Math.Abs(m.Position.Lat - location.Latitude) < 0.000001 &&
                Math.Abs(m.Position.Lng - location.Longitude) < 0.000001);

            if (marker != null)
                gmap.Markers.Remove(marker);
        }
    }
}
    