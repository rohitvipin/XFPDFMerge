using System.IO;
using System.Threading.Tasks;
using Android.OS;
using XFPDFMerge.Entities;

namespace XFPDFMerge.Droid.Helpers
{
    public static class FileHelper
    {
        public static async Task<string> WriteToFileInStorage(FileEntity fileEntity)
        {
            var filePath = Path.Combine(Environment.ExternalStorageDirectory.Path, fileEntity.FileName);
            using (var memorystream = new MemoryStream())
            {
                await memorystream.WriteAsync(fileEntity.DataArray, 0, fileEntity.Size);
                memorystream.Position = 0;

                using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    memorystream.WriteTo(file);
                }
            }
            return filePath;
        }
    }
}