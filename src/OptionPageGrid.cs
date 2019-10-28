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
    }
}
