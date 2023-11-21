// <copyright file="CopyDescriptionCommand.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ErrorHelper
{
    internal sealed class CopyDescriptionCommand : CommandBase
    {
        public const int CommandId = 0x0100;

        private CopyDescriptionCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static CopyDescriptionCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CopyDescriptionCommand(package, commandService);
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
                    System.Windows.Forms.Clipboard.SetText(desc);
                }
            }
            catch (Exception exc)
            {
                OutputPane.Instance.WriteLine($"ErrorHelper: {exc}");
                await this.ShowStatusBarMessageAsync("Unable to copy error description.");
            }
        }
    }
}
