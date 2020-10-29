namespace S3Lambdas.Models
{
    public class CreatePresignedUrlResponse
    {
        public int PartNumber { get; set; }
        public string PresignedUrl { get; set; }
    }
}
