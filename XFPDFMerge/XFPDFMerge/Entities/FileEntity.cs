﻿using System;
using System.Threading.Tasks;
using XFPDFMerge.Common;

namespace XFPDFMerge.Entities
{
    public class FileEntity
    {
        public event EventHandler Deleted;

        public FileEntity()
        {
            DeleteCommand = new AsyncRelayCommand(DeleteCommandHandler);
        }

        private async Task DeleteCommandHandler() => Deleted?.Invoke(this, null);

        public string FileName { get; set; }

        public string DisplaySize => Helpers.ReadableFileLength(Size);

        public byte[] DataArray { get; set; }

        public int Size => DataArray?.Length ?? 0;

        public AsyncRelayCommand DeleteCommand { get; }
    }
}