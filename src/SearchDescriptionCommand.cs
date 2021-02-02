// <copyright file="SearchDescriptionCommand.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ErrorHelper
{
    public sealed class SearchDescriptionCommand : CommandBase
    {
        public const int CommandId = 4129;

        private SearchDescriptionCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SearchDescriptionCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SearchDescriptionCommand(package, commandService);
        }

        public static string StripPaths(string descriptionContainingPaths)
        {
            var result = descriptionContainingPaths ?? string.Empty;

            if (result.Contains(":/"))
            {
                var pathStartPos = result.IndexOf(":/");

                if (char.IsLetter(result[pathStartPos - 1])
                 && !char.IsLetterOrDigit(result[pathStartPos - 2]))
                {
                    pathStartPos -= 1;

                    var endPos = result.IndexOf(" ", pathStartPos);

                    if (result[pathStartPos - 1] == '\'')
                    {
                        endPos = result.IndexOf("'", pathStartPos) + 1;
                        pathStartPos -= 1;
                    }

                    var firstPart = result.Substring(0, pathStartPos);

                    result = firstPart + result.Substring(endPos);
                }
            }

            if (result.Contains("../"))
            {
                var pathStartPos = result.IndexOf("../");

                var endPos = result.IndexOf(" ", pathStartPos);

                if (result[pathStartPos - 1] == '\'')
                {
                    endPos = result.IndexOf("'", pathStartPos) + 1;
                    pathStartPos -= 1;
                }

                var firstPart = result.Substring(0, pathStartPos);

                result = firstPart + result.Substring(endPos);
            }

            return result;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var desc = await this.GetDescriptionAsync();

            if (!string.IsNullOrEmpty(desc))
            {
                var searchEngine = ErrorHelperPackage.Instance?.Options?.SearchEngine;

                string url;
                switch (searchEngine)
                {
                    case SearchEngine.Google:
                        url = "https://www.google.com/search?q=";
                        break;
                    case SearchEngine.StackOverflow:
                        url = "https://stackoverflow.com/search?q=";
                        break;
                    case SearchEngine.Bing:
                    default:
                        url = "https://www.bing.com/search?q=";
                        break;
                }

                string query = desc;

                if (ErrorHelperPackage.Instance?.Options?.StripPaths ?? false)
                {
                    query = StripPaths(desc);
                }

                var ps = new ProcessStartInfo(url + WebUtility.UrlEncode(query))
                {
                    UseShellExecute = true,
                    Verb = "open",
                };
                Process.Start(ps);
            }
        }
    }
}
