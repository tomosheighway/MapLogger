/* 
TODO TASKS 
Improve error handling to prevent saving of a location that has already been saved. 
Add key for colours of presaved / searching points 
Zoom in on point when searching
Add manual zoom buttons 
*/

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapLogger
{
    public partial class MainWindow : Window
    {
        private readonly string csvFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, @"..\..\..\Savedlocations.csv");
        private GMapMarker? tempMarker = null;

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
            gmap.CanDragMap = true;
            gmap.DragButton = MouseButton.Left;

            LoadSavedLocations();
        }

        private void SaveLocation_Click(object sender, RoutedEventArgs e)
        {
            if (tempMarker != null)
            {
                var position = tempMarker.Position;

                // create new permanant marker to match temp
                var marker = new GMapMarker(position)
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
                AppendToCsv(position.Lat, position.Lng, DateTime.Now);

                // Remove the temp marker now it has been saved
                gmap.Markers.Remove(tempMarker);
                tempMarker = null;

                MessageBox.Show("Location saved.");
            }
            else
            {
                MessageBox.Show("Please search for a location first.");
            }
        }

        private void SearchLocation_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(latBox.Text, out double lat) &&
                double.TryParse(lonBox.Text, out double lng))
            {
                // Remove current temp if there is one 
                if (tempMarker != null)
                {
                    gmap.Markers.Remove(tempMarker);
                }

                // Create the new temporary marker
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
            else
            {
                MessageBox.Show("Invalid coordinates.");
            }
        }

        private void AppendToCsv(double lat, double lng, DateTime timestamp)
        {
            bool fileExists = File.Exists(csvFilePath);

            using (var writer = new StreamWriter(csvFilePath, append: true, Encoding.UTF8))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Latitude,Longitude,Timestamp");
                }

                writer.WriteLine($"{lat},{lng},{timestamp:O}");  // writes long lat and time 
            }
        }

        private void LoadSavedLocations()
        {
            if (!File.Exists(csvFilePath)) return;

            using (var reader = new StreamReader(csvFilePath))
            {
                string? line;
                bool isFirstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false; // Skip head in csv 
                        continue;
                    }

                    var parts = line.Split(',');
                    if (parts.Length >= 2 &&
                        double.TryParse(parts[0], out double lat) &&
                        double.TryParse(parts[1], out double lng))
                    {
                        var marker = new GMapMarker(new PointLatLng(lat, lng))
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


    }
}
