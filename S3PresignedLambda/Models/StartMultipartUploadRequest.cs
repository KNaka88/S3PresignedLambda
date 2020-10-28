using System;
using System.ComponentModel.DataAnnotations;

namespace S3Lambdas.Models
{
    public class StartMultipartUploadRequest
    {
        [Required]
        public string FileName { get; set; }
        public Guid UserGuid { get; set; }
        public string FolderName { get; set; }
    }
}
