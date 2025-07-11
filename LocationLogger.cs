using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapLogger
{
    public class LocationLogger
    {
        private readonly string csvPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Savedlocations.csv");

        public void AppendLocation(LocationModel location)
        {
            bool fileExists = File.Exists(csvPath);

            using var writer = new StreamWriter(csvPath, append: true, Encoding.UTF8);
            if (!fileExists)
            {
                writer.WriteLine("Latitude,Longitude,Timestamp");
            }

            writer.WriteLine($"{location.Latitude},{location.Longitude},{location.Timestamp:O}");
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
    }
}