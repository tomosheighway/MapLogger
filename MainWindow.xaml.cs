/* 
TODO TASKS 
Improve error handling to prevent saving of a location that has already been saved. 
Add key for colours of presaved / searching points 
Zoom in on point when searching
Add manual zoom buttons 
Click on a point to load its x and y location 
Option to then delete pin
Allow user to add text notes about a given location 
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

            mapService = new MapService(gmap);
            logger = new LocationLogger();

            mapService.InitializeMap();             //setup map 
            var savedLocations = logger.LoadLocations();       // get saved locations from csv 
            mapService.AddSavedMarkers(savedLocations);        // add to map 
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
                logger.AppendLocation(location);
                MessageBox.Show("Location saved.");
            }
            else
            {
                MessageBox.Show("Please search for a location first.");
            }
        }
    }
}