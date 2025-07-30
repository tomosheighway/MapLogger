/* 
TODO TASKS 
Add key for colours of presaved / searching points 
Add the logic to location Logger to actually remove the point from csv 
Allow user to add text notes about a given location 
Update to use real DB not CSV file 
Login / user then store points for that given user. 
Add overlay lines similar to - https://www.independent-software.com/gmap-net-tutorial-maps-markers-and-polygons.html/
Offline functionality to store localy etc 
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
        private readonly MapService mapService;
        private readonly LocationLogger logger;

        public MainWindow()
        {
            InitializeComponent();
            ShowInstructions();

            mapService = new MapService(gmap);
            logger = new LocationLogger();

            mapService.InitializeMap();             //setup map 
            var savedLocations = logger.LoadLocations();       // get saved locations from csv 
            mapService.AddSavedMarkers(savedLocations);        // add to map 
            mapService.SavedMarkerClicked += SavedMarker_Click;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mapService.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mapService.ZoomOut();
        }

        private void Zoom_MouseDoubleClick(object sender, MouseButtonEventArgs e)  // zoom into cursor location on double click 
        {
            if (gmap.Zoom < gmap.MaxZoom)
            {
                gmap.Position = gmap.FromLocalToLatLng((int)e.GetPosition(gmap).X, (int)e.GetPosition(gmap).Y);
                gmap.Zoom += 1;
            }
        }


        private void SearchLocation_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(latBox.Text, out double lat) &&
                double.TryParse(lonBox.Text, out double lng))
            {
                mapService.SetTemporaryMarker(lat, lng);
            }
            else
            {
                MessageBox.Show("Invalid coordinates.");
            }
        }

        private void SaveLocation_Click(object sender, RoutedEventArgs e)
        {
            var location = mapService.SaveTemporaryMarker();

            if (location != null)
            {
                bool saved = logger.AppendLocation(location);

                if (saved)
                    {
                    MessageBox.Show("Location saved.");
                    }
                else
                    {
                    MessageBox.Show("Duplicate location has not been saved");
                    }
            }
            else
            {
                MessageBox.Show("Please search for a location first.");
            }
        }

        private void Save_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(gmap);
            var latLng = gmap.FromLocalToLatLng((int)point.X, (int)point.Y);

            mapService.SetTemporaryMarker(latLng.Lat, latLng.Lng);
        }

            private void Help_Click(object sender, RoutedEventArgs e)
            {
                ShowInstructions();
            }

        private void ShowInstructions()
        {
            MessageBox.Show(
                "Welcome to the MapLogger Windows Form App\n\n" +
                "Right click on the map to select a location.\n" +
                "Click 'Save Location' to store it.\n" +
                "Use 'Search Location' to jump to specific coordinates.\n" +
                "Use the zoom buttons, trackpad or double-click to zoom in.\n\n" +
                "Developed by Tomos Heighway",
                "MapLogger - Getting Started",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // 🔹 Event handler for saved marker clicks
        private void SavedMarker_Click(LocationModel location)
        {
            var result = MessageBox.Show(
                $"Latitude: {location.Latitude}\n" +
                $"Longitude: {location.Longitude}\n\n" +
                "Do you want to delete this marker?",
                "Marker Details",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                mapService.RemoveMarker(location);
                logger.DeleteLocation(location);
            }
        }
    }
}