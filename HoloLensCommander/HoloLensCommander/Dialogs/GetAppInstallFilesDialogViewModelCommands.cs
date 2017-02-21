// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace HoloLensCommander
{
    /// <summary>
    /// The view model for the GetAppInstallFilesDialog fsobject.
    /// </summary>
    partial class GetAppInstallFilesDialogViewModel
    {
        /// <summary>
        /// Command used to allow the user to browse for the application package file.
        /// </summary>
        public ICommand BrowseForAppPackageCommand
        { get; private set; }    

        /// <summary>
        /// Implementation of the browse for app package command.
        /// </summary>
        /// <returns>Task object used for tracking method completion.</returns>
        private async Task BrowseForAppPackageAsync()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null)
            {
                return;
            }

            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("AppxFolderToken", folder);

            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            StorageFile file = files.FirstOrDefault(f => f.Name.EndsWith(".appxbundle"));

            if (file == null)
            {
                return;
            }

            this.AppPackageFile = file.Path;

            StorageFolder dependenciesFolder = await folder.GetFolderAsync("Dependencies\\x86");

            IReadOnlyList<StorageFile> dependencies = await dependenciesFolder.GetFilesAsync();

            foreach (StorageFile dependency in dependencies)
            {
                if (dependency.Name.EndsWith(".appx"))
                {
                    this.AppDependencyFileNames.Add(dependency.Path);
                }
            }
        }
    }
}
