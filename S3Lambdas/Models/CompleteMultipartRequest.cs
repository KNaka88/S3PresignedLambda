using Amazon.S3.Model;
using System.Collections.Generic;

namespace S3Lambdas.Models
{
    public class CompleteMultipartRequest
    {
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public string UploadId { get; set; }
        public List<PartETag> PartETags { get; set; } 
    }
}
