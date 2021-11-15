// <copyright file="ErrorHelperPackage.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ErrorHelper
{
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(ErrorHelperPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("#110", "#112", "1.9")] // Info on this package for Help/About
    [ProvideOptionPage(typeof(OptionPageGrid), "Error Helper", "General", 0, 0, true)]
    [ProvideProfileAttribute(typeof(OptionPageGrid), "Error Helper", "General", 106, 107, isToolsOptionPage: true)]
    public sealed class ErrorHelperPackage : AsyncPackage
    {
        public const string PackageGuidString = "3f9fa707-35b8-42f5-a623-8fa0ab37a05c";

#pragma warning disable SA1401 // Fields should be private
        public static ErrorHelperPackage Instance;
#pragma warning restore SA1401 // Fields should be private

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
            await OpenUrlCommand.InitializeAsync(this);

            await SponsorRequestHelper.CheckIfNeedToShowAsync();
        }
    }
}
