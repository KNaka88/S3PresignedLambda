namespace S3Lambdas.Models
{
    public class StartMultipartUploadRequest
    {
        public string FileName { get; set; }
        public string FolderName { get; set; }
    }
}
