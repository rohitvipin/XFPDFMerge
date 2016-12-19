﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Util;
using Xamarin.Forms;
using XFPDFMerge.DependencyServices;
using XFPDFMerge.Droid.Activities;
using XFPDFMerge.Droid.ServiceImplementations;
using XFPDFMerge.Entities;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using WebView = Android.Webkit.WebView;

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
            Intent intent = new Intent(Intent.ActionView);
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
                Toast.MakeText(Application.Context, "Can't open file", ToastLength.Long);
            }
        }

        private static string GetFileType(string extension)
        {
            if (extension.EndsWith(".doc") || extension.EndsWith(".docx"))
            {
                // Word document
                return "application/msword";
            }
            if (extension.EndsWith(".pdf"))
            {
                // PDF file
                return "application/pdf";
            }
            if (extension.EndsWith(".ppt") || extension.EndsWith(".pptx"))
            {
                // Powerpoint file
                return "application/vnd.ms-powerpoint";
            }
            if (extension.EndsWith(".xls") || extension.EndsWith(".xlsx"))
            {
                // Excel file
                return "application/vnd.ms-excel";
            }
            if (extension.EndsWith(".zip") || extension.EndsWith(".rar"))
            {
                // WAV audio file
                return "application/x-wav";
            }
            if (extension.EndsWith(".rtf"))
            {
                // RTF file
                return "application/rtf";
            }
            if (extension.EndsWith(".wav") || extension.EndsWith(".mp3"))
            {
                // WAV audio file
                return "audio/x-wav";
            }
            if (extension.EndsWith(".gif"))
            {
                // GIF file
                return "image/gif";
            }
            if (extension.EndsWith(".jpg") || extension.EndsWith(".jpeg") || extension.EndsWith(".png"))
            {
                // JPG file
                return "image/jpeg";
            }
            if (extension.EndsWith(".txt"))
            {
                // Text file
                return "text/plain";
            }
            if (extension.EndsWith(".3gp") || extension.EndsWith(".mpg") || extension.EndsWith(".mpeg") || extension.EndsWith(".mpe") || extension.EndsWith(".mp4") || extension.EndsWith(".avi"))
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

        private static async Task<string> WriteToFileInStorage(FileEntity fileEntity)
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
        #endregion

        public async Task DisplayFile(FileEntity fileEntity)
        {
            string filePath = null;

            try
            {
                filePath = await WriteToFileInStorage(fileEntity);

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
                    Toast.MakeText(Application.Context, "Unable to generate file", ToastLength.Long);
                }
                else
                {
                    OpenInWebView(filePath);
                }
            }
        }

        public FileEntity MergeFiles(IList<FileEntity> pdfFiles)
        {
            throw new NotImplementedException();
        }
    }
}