﻿// <copyright file="ErrorHelperPackage.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ErrorHelper
{
	[ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
	[ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(ErrorHelperPackage.PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)] // Info on this package for Help/About
	[ProvideOptionPage(typeof(OptionPageGrid), Vsix.Name, "General", 0, 0, true)]
	[ProvideProfileAttribute(typeof(OptionPageGrid), Vsix.Name, "General", 106, 107, isToolsOptionPage: true)]
	public sealed class ErrorHelperPackage : AsyncPackage
	{
		public const string PackageGuidString = "3f9fa707-35b8-42f5-a623-8fa0ab37a05c";

		public static ErrorHelperPackage Instance;

		public OptionPageGrid Options
		{
			get
			{
				return (OptionPageGrid)this.GetDialogPage(typeof(OptionPageGrid));
			}
		}

		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

			ErrorHelperPackage.Instance = this;

			await CopyDescriptionCommand.InitializeAsync(this);
			await SearchDescriptionCommand.InitializeAsync(this);
			await SearchDescriptionWithoutPathsCommand.InitializeAsync(this);
			await OpenUrlCommand.InitializeAsync(this);

			await SponsorRequestHelper.CheckIfNeedToShowAsync();

			TrackBasicUsageAnalytics();
		}

		private static void TrackBasicUsageAnalytics()
		{
#if !DEBUG
			try
			{
				if (string.IsNullOrWhiteSpace(AnalyticsConfig.TelemetryConnectionString))
				{
					return;
				}

				var config = new TelemetryConfiguration
				{
					ConnectionString = AnalyticsConfig.TelemetryConnectionString,
				};

				var client = new TelemetryClient(config);

				var properties = new Dictionary<string, string>
				{
					{ "VsixVersion", Vsix.Version },
					{ "VsVersion", Microsoft.VisualStudio.Telemetry.TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ExeVersion") },
					{ "Architecture", RuntimeInformation.ProcessArchitecture.ToString() },
					{ "MsInternal", Microsoft.VisualStudio.Telemetry.TelemetryService.DefaultSession?.IsUserMicrosoftInternal.ToString() },
				};

				client.TrackEvent(Vsix.Name, properties);
			}
			catch (Exception exc)
			{
				System.Diagnostics.Debug.WriteLine(exc);
				OutputPane.Instance.WriteLine("Error tracking usage analytics: " + exc.Message);
			}
#endif
		}
	}
}
