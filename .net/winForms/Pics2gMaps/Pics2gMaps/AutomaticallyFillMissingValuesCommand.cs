﻿using System.Data;

namespace Pics2gMaps;

public class AutomaticallyFillMissingValuesCommand
{
    public DataRow DataRow { get; set; }
    public DataColumnCollection Columns { get; set; }
    public string BaseUrl { get; set; }
    public string JqueryVersion { get; set; }
}