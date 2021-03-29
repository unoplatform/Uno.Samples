// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using WCTDataTreeTabSample;
using WCTDataTreeTabSample.Helpers;

namespace Microsoft.Toolkit.Uwp.SampleApp.Data
{
    [Bindable]
    public class LocationDataSource
    {
        private static ObservableCollection<LocationDataItem> _items;
        private static List<string> _locations;
        private string _cachedSortedColumn = string.Empty;

        // Loading data
        public async Task<IEnumerable<LocationDataItem>> GetDataAsync()
        {
            using (var stream = await StreamHelperEx.GetEmbeddedFileStreamAsync(GetType(), "LocationSampleData.csv"))
            {
                var list = new List<LocationDataItem>();

                using (var sr = new StreamReader(stream))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(',');

                        list.Add(
                            new LocationDataItem()
                            {
                                Location = values[0],
                                Coordinates = $"{values[1]} , {values[2]}"
                            });
                    }
                }

                _items = new ObservableCollection<LocationDataItem>(
                    list
#if __WASM__        // Uncomment this line to load the sample faster in WASM interpreted mode
                    .Take(10)
#endif
                );
                return _items;
            }
        }

        // Load locations into separate collection for use in combobox column
        public async Task<IEnumerable<string>> GetLocations()
        {
            if (_items == null || !_items.Any())
            {
                await GetDataAsync();
            }

            _locations = _items?.OrderBy(x => x.Location).Select(x => x.Location).Distinct().ToList();

            return _locations;
        }

        // Sorting implementation using LINQ
        public string CachedSortedColumn
        {
            get
            {
                return _cachedSortedColumn;
            }

            set
            {
                _cachedSortedColumn = value;
            }
        }

        public ObservableCollection<LocationDataItem> SortData(string sortBy, bool ascending)
        {
            _cachedSortedColumn = sortBy;
            switch (sortBy)
            {
                case "Location":
                    if (ascending)
                    {
                        return new ObservableCollection<LocationDataItem>(from item in _items
                                                                          orderby item.Location ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<LocationDataItem>(from item in _items
                                                                          orderby item.Location descending
                                                                          select item);
                    }
            }

            return _items;
        }
    }
}
