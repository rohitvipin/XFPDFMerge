using System.Collections.Generic;
using System.Threading.Tasks;
using XFPDFMerge.Entities;

namespace XFPDFMerge.DependencyServices
{
    public interface IPdfService
    {
        Task DisplayFile(FileEntity fileEntity);

        Task<FileEntity> MergeFiles(IList<FileEntity> pdfFiles);
    }
}