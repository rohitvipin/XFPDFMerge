﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.FilePicker;
using XFPDFMerge.Common;
using XFPDFMerge.Entities;

namespace XFPDFMerge.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Files = new ObservableCollection<FileEntity>();

            PickFilesCommand = new AsyncRelayCommand(PickFilesCommandHandler);
            MergeFilesCommand = new AsyncRelayCommand(MergeFilesCommandHandler);

            Files.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(IsFilesAvailableToMerge));
        }

        private Task MergeFilesCommandHandler()
        {
            throw new System.NotImplementedException();
        }

        private async Task PickFilesCommandHandler()
        {
            var file = await CrossFilePicker.Current.PickFile();
            var fileEntity = new FileEntity
            {
                FileName = file.FileName,
                DataArray = file.DataArray
            };
            Files.Add(fileEntity);
            fileEntity.Deleted += FileEntity_OnDeleted;
        }

        private void FileEntity_OnDeleted(object sender, EventArgs eventArgs)
        {
            var deletedEntity = sender as FileEntity;
            if (deletedEntity == null)
            {
                return;
            }
            deletedEntity.Deleted -= FileEntity_OnDeleted;
            Files.Remove(deletedEntity);
        }

        public ObservableCollection<FileEntity> Files { get; set; }

        public bool IsFilesAvailableToMerge => Files?.Count > 0;

        public AsyncRelayCommand PickFilesCommand { get; }

        public AsyncRelayCommand MergeFilesCommand { get; }
    }
}