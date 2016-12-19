using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics.Pdf;
using Android.OS;
using XFPDFMerge.DependencyServices;
using XFPDFMerge.Entities;

namespace XFPDFMerge.Droid.ServiceImplementations
{
    public class PdfService : IPdfService
    {
        public void DisplayFile(FileEntity fileEntity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                //Since API Level 21 (Lollipop) Android provides a PdfRenderer class

                var filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileEntity.FileName);

                using (var memorystream = new MemoryStream())
                {
                    memorystream.Write(fileEntity.DataArray, 0, fileEntity.Size);
                    using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        memorystream.WriteTo(file);
                    }
                }


                var activity2 = new Intent(Application.Context, typeof(PdfViewActivity));
                activity2.PutExtra("filePath", filePath);
                Application.Context.StartActivity(activity2);


            }
        }

        public FileEntity MergeFiles(IList<FileEntity> pdfFiles)
        {
            throw new NotImplementedException();
        }
    }
}