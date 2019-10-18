// <copyright file="CommandBase.cs" company="Matt Lacey Limited">
// Copyright (c) Matt Lacey Limited. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ErrorHelper
{
    internal class CommandBase
    {
        public static readonly Guid CommandSet = new Guid("5fea58eb-df34-46e8-ab5d-4708b07cf330");

        private readonly AsyncPackage package;

        public CommandBase(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public async Task<string> GetDescriptionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var tasks = await this.package.GetServiceAsync(typeof(SVsErrorList)) as IVsTaskList2;

            if (tasks != null)
            {
                tasks.EnumSelectedItems(out IVsEnumTaskItems itemsEnum);

                var vsTaskItem = new IVsTaskItem[1];

                if (itemsEnum.Next(1, vsTaskItem, null) == 0)
                {
                    string description;

                    vsTaskItem[0].get_Text(out description);

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        return description;
                    }
                }
            }

            return string.Empty;
        }
    }
}
