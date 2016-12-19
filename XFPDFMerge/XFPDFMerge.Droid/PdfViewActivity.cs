﻿using Android.App;
using Android.OS;

namespace XFPDFMerge.Droid
{
    public class PdfViewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main_real);
            if (savedInstanceState == null)
            {
                FragmentManager.BeginTransaction()
                        .Add(Resource.Id.container, new PdfRendererFragment { FilePath = Intent.GetStringExtra("filePath") }, "FRAGMENT_PDF_RENDERER")
                        .Commit();
            }
        }
    }
}