using System.Collections.Generic;

namespace S3Lambdas.Models
{
    public class PresignedUrlRequest
    {
        public string UploadId { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public IList<int> PartNumbers { get; set; }
        public string ContentType { get; set; }
    }
}
