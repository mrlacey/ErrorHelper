// <copyright file="OptionPageGrid.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace ErrorHelper
{
    public class OptionPageGrid : DialogPage
    {
        [Category("Error Helper")]
        [DisplayName("Search Engine")]
        [Description("Where to do the search.")]
        public SearchEngine SearchEngine { get; set; }

        ////[Category("Error Helper")]
        ////[DisplayName("Strip Paths Before Search")]
        ////[Description("Remove anything that looks like a file path from the value sent to the search engine.")]
        ////public bool StripPaths { get; set; }
    }
}
