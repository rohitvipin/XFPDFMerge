using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Widget;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Xamarin.Forms;
using XFPDFMerge.DependencyServices;
using XFPDFMerge.Droid.Activities;
using XFPDFMerge.Droid.Helpers;
using XFPDFMerge.Droid.ServiceImplementations;
using XFPDFMerge.Entities;
using Application = Android.App.Application;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(PdfService))]
namespace XFPDFMerge.Droid.ServiceImplementations
{
    public class PdfService : IPdfService
    {
        #region Private Methods

        private static void OpenInPdfRenderer(string filePath)
        {
            var intent = new Intent(Application.Context, typeof(PdfViewActivity));
            intent.PutExtra("filePath", filePath);
            intent.AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }

        private static void OpenFileExternalApp(string filePath)
        {
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Uri.Parse($"file:///{filePath}"), GetFileType(Path.GetExtension(filePath)));
            intent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }

        private static void OpenInWebView(string filePath)
        {
            try
            {
                var intent = new Intent(Application.Context, typeof(PdfWebViewActivity));
                intent.PutExtra("filePath", filePath);
                intent.AddFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(intent);
            }
            catch (Exception)
            {
                Toast.MakeText(Application.Context, "Can't open file", ToastLength.Long).Show();
            }
        }

        private static string GetFileType(string extension)
        {
            if (extension.EndsWith(".doc", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".docx", StringComparison.InvariantCultureIgnoreCase))
            {
                // Word document
                return "application/msword";
            }
            if (extension.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                // PDF file
                return "application/pdf";
            }
            if (extension.EndsWith(".ppt", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".pptx", StringComparison.InvariantCultureIgnoreCase))
            {
                // Powerpoint file
                return "application/vnd.ms-powerpoint";
            }
            if (extension.EndsWith(".xls", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
            {
                // Excel file
                return "application/vnd.ms-excel";
            }
            if (extension.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".rar", StringComparison.InvariantCultureIgnoreCase))
            {
                // WAV audio file
                return "application/x-wav";
            }
            if (extension.EndsWith(".rtf", StringComparison.InvariantCultureIgnoreCase))
            {
                // RTF file
                return "application/rtf";
            }
            if (extension.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
            {
                // WAV audio file
                return "audio/x-wav";
            }
            if (extension.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase))
            {
                // GIF file
                return "image/gif";
            }
            if (extension.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                // JPG file
                return "image/jpeg";
            }
            if (extension.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                // Text file
                return "text/plain";
            }
            if (extension.EndsWith(".3gp", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".mpg", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".mpeg", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".mpe", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase) || extension.EndsWith(".avi", StringComparison.InvariantCultureIgnoreCase))
            {
                // Video files
                return "video/*";
            }
            //if you want you can also define the intent type for any other file

            //additionally use else clause below, to manage other unknown extensions
            //in this case, Android will show all applications installed on the device
            //so you can choose which application to use
            return "*/*";
        }


        #endregion

        public async Task DisplayFile(FileEntity fileEntity)
        {
            string filePath = null;

            try
            {
                filePath = await FileHelper.CopyFileToStorage(fileEntity);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    OpenInPdfRenderer(filePath);
                }
                else
                {
                    OpenFileExternalApp(filePath);
                }
            }
            catch (Exception)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Toast.MakeText(Application.Context, "Unable to generate file", ToastLength.Long).Show();
                }
                else
                {
                    OpenInWebView(filePath);
                }
            }
        }

        public async Task<FileEntity> MergeFiles(IList<FileEntity> pdfFiles)
        {
            if (pdfFiles?.Count > 0 == false)
            {
                return null;
            }

            var mergedFileName = string.Join("_", pdfFiles.Select(x => Path.GetFileNameWithoutExtension(x.FileName))) + ".pdf";
            var mergedFilePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, mergedFileName);

            using (var stream = new FileStream(mergedFilePath, FileMode.Create, FileAccess.Write))
            {
                var document = new Document();
                var pdf = new PdfCopy(document, stream);
                PdfReader reader = null;

                try
                {
                    document.Open();

                    foreach (var file in pdfFiles)
                    {
                        await FileHelper.CopyFileToStorage(file);

                        reader = new PdfReader(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, file.FileName));

                        for (int pageIndex = 1; pageIndex <= reader.NumberOfPages; pageIndex++)
                        {
                            pdf.AddPage(pdf.GetImportedPage(reader, pageIndex));
                        }

                        pdf.FreeReader(reader);

                        reader.Close();
                    }
                }
                catch (Exception)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                }
            }

            var mergedFile = new FileEntity
            {
                FileName = mergedFileName
            };

            using (FileStream file = new FileStream(mergedFilePath, FileMode.Open, FileAccess.Read))
            {
                mergedFile.DataArray = new byte[file.Length];
                await file.ReadAsync(mergedFile.DataArray, 0, mergedFile.Size);
            }

            return mergedFile;
        }
    }
}