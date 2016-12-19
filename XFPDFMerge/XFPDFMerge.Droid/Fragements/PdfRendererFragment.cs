using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace XFPDFMerge.Droid.Fragements
{
    public class PdfRendererFragment : Fragment
    {
        private const string StateCurrentPageIndex = "current_page_index";
        private ParcelFileDescriptor _mFileDescriptor;
        private PdfRenderer _mPdfRenderer;
        private PdfRenderer.Page _mCurrentPage;

        /**
         * {@link android.widget.ImageView} that shows a PDF page as a {@link android.graphics.Bitmap}
         */
        private ImageView _mImageView;

        /**
         * {@link android.widget.Button} to move to the previous page.
         */
        private Button _mButtonPrevious;

        /**
         * {@link android.widget.Button} to move to the next page.
         */
        private Button _mButtonNext;

        public string FilePath { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_pdf_renderer, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            // Retain view references.
            _mImageView = (ImageView)view.FindViewById(Resource.Id.image);
            _mButtonPrevious = (Button)view.FindViewById(Resource.Id.previous);
            _mButtonNext = (Button)view.FindViewById(Resource.Id.next);
            // Bind events.
            _mButtonPrevious.Click += (sender, args) => { ShowPage(_mCurrentPage.Index - 1); };
            _mButtonNext.Click += (sender, args) => { ShowPage(_mCurrentPage.Index + 1); };
            // Show the first page by default.
            int index = 0;
            // If there is a savedInstanceState (screen orientations, etc.), we restore the page index.
            if (null != savedInstanceState)
            {
                index = savedInstanceState.GetInt(StateCurrentPageIndex, 0);
            }
            ShowPage(index);
        }

        [Obsolete("deprecated")]
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            try
            {
                OpenRenderer(activity);
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                Toast.MakeText(activity, $"Error! {e.Message}", ToastLength.Short).Show();
                activity.Finish();
            }
        }


        public override void OnDetach()
        {
            try
            {
                CloseRenderer();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            base.OnDetach();
        }


        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (null != _mCurrentPage)
            {
                outState.PutInt(StateCurrentPageIndex, _mCurrentPage.Index);
            }
        }

        /**
         * Sets up a {@link android.graphics.pdf.PdfRenderer} and related resources.
         */
        private void OpenRenderer(Context context)
        {
            // In this sample, we read a PDF from the assets directory.
            using (var i = context.Assets.Open(FilePath))
            {
                using (var o = context.OpenFileOutput("_sample.pdf", FileCreationMode.Private))
                {
                    i.CopyTo(o);
                }
            }
            var f = context.GetFileStreamPath("_sample.pdf");
            _mFileDescriptor = ParcelFileDescriptor.Open(f, ParcelFileMode.ReadOnly);


            //AssetFileDescriptor desc = context.Assets.OpenFd("presentation.pdf");
            //mFileDescriptor = desc.ParcelFileDescriptor;
            // This is the PdfRenderer we use to render the PDF.
            _mPdfRenderer = new PdfRenderer(_mFileDescriptor);
        }

        /**
         * Closes the {@link android.graphics.pdf.PdfRenderer} and related resources.
         *
         * @throws java.io.IOException When the PDF file cannot be closed.
         */
        private void CloseRenderer()
        {
            _mCurrentPage?.Close();
            _mPdfRenderer.Close();
            _mFileDescriptor.Close();
        }

        /**
         * Shows the specified page of PDF to the screen.
         *
         * @param index The page index.
         */
        private void ShowPage(int index)
        {
            if (_mPdfRenderer.PageCount <= index)
            {
                return;
            }
            // Make sure to close the current page before opening another one.
            _mCurrentPage?.Close();
            // Use `openPage` to open a specific page in PDF.
            _mCurrentPage = _mPdfRenderer.OpenPage(index);
            // Important: the destination bitmap must be ARGB (not RGB).
            using (Bitmap bitmap = Bitmap.CreateBitmap(_mCurrentPage.Width, _mCurrentPage.Height, Bitmap.Config.Argb8888))
            {
                _mCurrentPage.Render(bitmap, null, null, PdfRenderMode.ForDisplay);
                // We are ready to show the Bitmap to user.
                _mImageView.SetImageBitmap(bitmap);
            }
            UpdateUi();
        }

        /**
         * Updates the state of 2 control buttons in response to the current page index.
         */
        private void UpdateUi()
        {
            _mButtonPrevious.Enabled = 0 != _mCurrentPage.Index;
            _mButtonNext.Enabled = _mCurrentPage.Index + 1 < _mPdfRenderer.PageCount;
            //string[] strparams = { (index + 1).ToString(), pageCount.ToString()};
            //string appname = GetString(Resource.String.app_name_with_index, strparams);
            //this.Activity.Title = appname;
        }

        /**
         * Gets the number of pages in the PDF. This method is marked as public for testing.
         *
         * @return The number of pages.
         */
        public int GetPageCount() => _mPdfRenderer.PageCount;
    }
}