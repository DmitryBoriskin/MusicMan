﻿using System;
using System.Web;

public class FilterParams
{
    public int Page { get; set; }
    public int Size { get; set; }
    public bool? Disabled { get; set; }
    public string Type { get; set; }
    public string Categoty { get; set; }
    public string Group { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DateEnd { get; set; }
    public string SearchText { get; set; }
    public string Lang { get; set; }
    public string User { get; set; }
    public bool? Main { get; set; }
    public string Sort { get; set; }
}
