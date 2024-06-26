﻿// <copyright file="SearchDescriptionCommand.cs" company="Matt Lacey Limited">
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
					string query = desc;

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
				await this.ShowStatusBarMessageAsync("Unable to search for error description.");
				OutputPane.Instance.WriteLine($"ErrorHelper: {exc}");
				OutputPane.Instance.Activate();
			}
		}
	}
}
