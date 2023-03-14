// <copyright file="SearchDescriptionWithoutPathsCommand.cs" company="Matt Lacey Limited">
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
    public sealed class SearchDescriptionWithoutPathsCommand : CommandBase
    {
        public const int CommandId = 4131;

        private SearchDescriptionWithoutPathsCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SearchDescriptionWithoutPathsCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SearchDescriptionWithoutPathsCommand(package, commandService);
        }

        public static string StripPaths(string descriptionContainingPaths)
        {
            var result = descriptionContainingPaths ?? string.Empty;

            string WithoutAbsolutePath(string original, string identifier)
            {
                var res = original;

                if (res.Contains(identifier))
                {
                    var pathStartPos = res.IndexOf(identifier);

                    if (char.IsLetter(res[pathStartPos - 1])
                     && !char.IsLetterOrDigit(res[pathStartPos - 2]))
                    {
                        pathStartPos -= 1;

                        var endPos = res.IndexOf(" ", pathStartPos);

                        if (res[pathStartPos - 1] == '\'')
                        {
                            endPos = res.IndexOf("'", pathStartPos) + 1;
                            pathStartPos -= 1;
                        }

                        var firstPart = res.Substring(0, pathStartPos);

                        res = firstPart + res.Substring(endPos);
                    }
                }

                return res;
            }

            result = WithoutAbsolutePath(result, ":/");
            result = WithoutAbsolutePath(result, ":\\");

            string WithoutRelativePath(string original, string identifier)
            {
                var res = original;

                if (res.Contains(identifier))
                {
                    var pathStartPos = res.IndexOf(identifier);

                    var endPos = res.IndexOf(" ", pathStartPos);

                    if (res[pathStartPos - 1] == '\'')
                    {
                        endPos = res.IndexOf("'", pathStartPos) + 1;
                        pathStartPos -= 1;
                    }

                    var firstPart = res.Substring(0, pathStartPos);

                    res = firstPart + res.Substring(endPos);
                }

                return res;
            }

            result = WithoutRelativePath(result, "../");
            result = WithoutRelativePath(result, "..\\");

            return result;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                var desc = await this.GetDescriptionAsync();

                if (!string.IsNullOrEmpty(desc))
                {
                    string url = this.GetSearchUrlBase();
                    string query = StripPaths(desc);

                    var ps = new ProcessStartInfo(url + WebUtility.UrlEncode(query))
                    {
                        UseShellExecute = true,
                        Verb = "open",
                    };

                    Process.Start(ps);
                }
            }
            catch (Exception exc)
            {
                GeneralOutputPane.Instance.WriteLine($"ErrorHelper: {exc}");
                await this.ShowStatusBarMessageAsync("Unable to search for error description.");
            }
        }
    }
}
