using System;

namespace XFPDFMerge.Common
{
    public static class Helpers
    {
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const int DecimalPlaces = 2;
        private const string SizeDisplayFormat = "{0} {1}";
        private const string GbSizeUnit = "GB";
        private const string MbSizeUnit = "MB";
        private const string KbSizeUnit = "KB";
        private const string ByteSizeUnit = "Bytes";

        public static string ReadableFileLength(int numberOfBytes)
        {
            if (numberOfBytes < OneKb)
            {
                return string.Format(SizeDisplayFormat, numberOfBytes, ByteSizeUnit);
            }

            if (numberOfBytes < OneMb)
            {
                return string.Format(SizeDisplayFormat, Math.Round((double)numberOfBytes / OneKb, DecimalPlaces), KbSizeUnit);
            }

            return numberOfBytes < OneGb ? string.Format(SizeDisplayFormat, Math.Round((double)numberOfBytes / OneMb, DecimalPlaces), MbSizeUnit)
                : string.Format(SizeDisplayFormat, Math.Round((double)numberOfBytes / OneGb, DecimalPlaces), GbSizeUnit);
        }
    }
}