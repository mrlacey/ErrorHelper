// <copyright file="OpenUrlCommand.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ErrorHelper
{
    public sealed class OpenUrlCommand : CommandBase
    {
        public const int CommandId = 4130;

        private OpenUrlCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static OpenUrlCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new OpenUrlCommand(package, commandService);
        }

        public static List<string> ParseUrls(string input)
        {
            var result = new List<string>();

            result.AddRange(ParseUrlsWithPrefix(input, "https://"));
            result.AddRange(ParseUrlsWithPrefix(input, "http://"));

            return result;
        }

        public static List<string> ParseUrlsWithPrefix(string input, string prefix)
        {
            var urls = new List<string>();

            var nextStart = input.IndexOf(prefix);

            var trimEndChars = new[] { '.', ',', '"', ':', ';', '\'' };

            while (nextStart > -1)
            {
                var end = input.IndexOfAny(new[] { ' ', "\r"[0], "\n"[0] }, nextStart);

                int nextSearchStart;

                if (end == -1)
                {
                    // Will be true if domain ends at the end of the string
                    urls.Add(input.Substring(nextStart).TrimEnd(trimEndChars));
                    nextSearchStart = nextStart + 1;  // Need to set this to a valid value that will break the while loop
                }
                else
                {
                    urls.Add(input.Substring(nextStart, end - nextStart).TrimEnd(trimEndChars));
                    nextSearchStart = end;
                }

                nextStart = input.IndexOf(prefix, nextSearchStart);
            }

            return urls;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var desc = await this.GetDescriptionAsync();

            if (!string.IsNullOrEmpty(desc))
            {
                var urls = ParseUrls(desc);

                var urlCount = urls.Count;

                if (urlCount > 0)
                {
                    if (urlCount > 1)
                    {
                        await this.ShowStatusBarMessageAsync("Multiple URLS found in description, opening first");
                    }

                    var ps = new ProcessStartInfo(urls.First())
                    {
                        UseShellExecute = true,
                        Verb = "open",
                    };

                    Process.Start(ps);
                }
                else
                {
                    await this.ShowStatusBarMessageAsync("No URL found in description");
                }
            }
        }
    }
}
