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
        }

        public LocationModel? SaveTemporaryMarker()
        {
            if (tempMarker == null) return null;

            var savedMarker = new GMapMarker(tempMarker.Position)
            {
                Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Fill = Brushes.Orange
                }
            };

            gmap.Markers.Add(savedMarker);
            gmap.Markers.Remove(tempMarker);

            var model = new LocationModel
            {
                Latitude = tempMarker.Position.Lat,
                Longitude = tempMarker.Position.Lng,
                Timestamp = System.DateTime.Now
            };

            tempMarker = null;
            return model;
        }

        public void AddSavedMarkers(List<LocationModel> locations)
        {
            foreach (var loc in locations)
            {
                var marker = new GMapMarker(new PointLatLng(loc.Latitude, loc.Longitude))
                {
                    Shape = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        Fill = Brushes.LightBlue
                    }
                };

                gmap.Markers.Add(marker);
            }
        }
        
    }
}