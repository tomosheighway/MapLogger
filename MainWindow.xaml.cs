using System;
using System.Windows;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapLogger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Setup GMap.NET
            gmap.MapProvider = GMapProviders.OpenStreetMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly; // or ServerAndCache for limited offline support

            gmap.Position = new PointLatLng(51.4545, -2.5879); // set to bristol on loadup 
            gmap.MinZoom = 2;
            gmap.MaxZoom = 18;
            gmap.Zoom = 10;
            gmap.ShowCenter = false;
        }

        private void LogLocation_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(latBox.Text, out double lat) &&
                double.TryParse(lonBox.Text, out double lng))
            {
                var marker = new GMapMarker(new PointLatLng(lat, lng))
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

                gmap.Markers.Add(marker);
                gmap.Position = new PointLatLng(lat, lng);
            }
            else
            {
                MessageBox.Show("Invalid coordinates. Please enter valid decimal numbers.");
            }
        }
    }
}
