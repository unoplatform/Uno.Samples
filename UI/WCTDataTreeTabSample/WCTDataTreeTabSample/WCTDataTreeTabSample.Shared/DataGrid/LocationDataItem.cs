// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.Uwp.SampleApp.Data
{
    public class LocationDataItem
    {
        //private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        //private string _location;
        //private string _coordinates;

        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public string Location { get; set; }

        public string Coordinates { get; set; }
    }
}
