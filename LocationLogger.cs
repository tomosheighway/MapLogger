using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapLogger
{
    public class LocationLogger
    {
        private readonly string csvPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Savedlocations.csv");
        private const double Tolerance = 0.00005; // 5 meters

        public bool AppendLocation(LocationModel location)
        {
            bool fileExists = File.Exists(csvPath);
            var existingLocations = LoadLocations();

            bool isDuplicate = existingLocations.Any(l =>
                Math.Abs(l.Latitude - location.Latitude) < Tolerance &&
                Math.Abs(l.Longitude - location.Longitude) < Tolerance);

            if (isDuplicate)
            {
                return false; // Location already exists
            }

            using var writer = new StreamWriter(csvPath, append: true, Encoding.UTF8);
            if (!fileExists)
            {
                writer.WriteLine("Latitude,Longitude,Timestamp");
            }

            writer.WriteLine($"{location.Latitude},{location.Longitude},{location.Timestamp:O}");
            return true; //location added  
        }

        public List<LocationModel> LoadLocations()
        {
            var list = new List<LocationModel>();

            if (!File.Exists(csvPath)) return list;

            using var reader = new StreamReader(csvPath);
            bool isFirstLine = true;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false; // skip head in csv 
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length >= 3 &&
                    double.TryParse(parts[0], out double lat) &&
                    double.TryParse(parts[1], out double lng) &&
                    DateTime.TryParse(parts[2], out DateTime timestamp))
                {
                    list.Add(new LocationModel
                    {
                        Latitude = lat,
                        Longitude = lng,
                        Timestamp = timestamp
                    });
                }
            }

            return list;
        }

        public bool DeleteLocation(LocationModel location)
        {
            if (!File.Exists(csvPath))
                return false;

            var locations = LoadLocations();

            locations.RemoveAll(l => // check tolerance 
                Math.Abs(l.Latitude - location.Latitude) < Tolerance &&
                Math.Abs(l.Longitude - location.Longitude) < Tolerance);

            try
            {
                using var writer = new StreamWriter(csvPath, false, Encoding.UTF8);

                //rewrite entire csv file. Use of proper DB in future instead 
                writer.WriteLine("Latitude,Longitude,Timestamp");
                foreach (var loc in locations)
                {
                    writer.WriteLine($"{loc.Latitude},{loc.Longitude},{loc.Timestamp:O}");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}