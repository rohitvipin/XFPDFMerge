using System.Collections.Generic;
using XFPDFMerge.Entities;

namespace XFPDFMerge.DependencyServices
{
    public interface IPdfService
    {
        void DisplayFile(FileEntity fileEntity);

        FileEntity MergeFiles(IList<FileEntity> pdfFiles);
    }
}