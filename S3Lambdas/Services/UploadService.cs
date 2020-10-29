using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using S3Lambdas.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace S3Lambdas.Services
{
    public class UploadService : IUploadService
    {
        private readonly IAmazonS3 _s3;
        private readonly IEnvironment _environment;

        public UploadService(IAmazonS3 s3, IEnvironment environment)
        {
            _s3 = s3;
            _environment = environment;
        }

        public async Task<InitiateMultipartUploadResponse> StartMultipartUploadAsync(StartMultipartUploadRequest request, ILambdaContext context)
        {
            var uploadRequest = new InitiateMultipartUploadRequest
            {
                BucketName = _environment.GetEnvironmentVariable("BUCKET_NAME"),
                Key = ConstructFileKey(request.FileName, request.FolderName),
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };
            
            return await _s3.InitiateMultipartUploadAsync(uploadRequest);
        }

        public IList<CreatePresignedUrlResponse> CreatePresignedUrl(PresignedUrlRequest request, ILambdaContext context)
        {
            var presignedUrlRequests = request.PartNumbers.Select(partNumber => new GetPreSignedUrlRequest
            {
                BucketName = _environment.GetEnvironmentVariable("BUCKET_NAME"),
                Key = ConstructFileKey(request.FileName, request.FolderName),
                Verb = HttpVerb.PUT,
                UploadId = request.UploadId,
                PartNumber = partNumber,
                ContentType = request.ContentType,
                Expires = DateTime.UtcNow.AddMinutes(30),
            });

            var responseCollection = new BlockingCollection<CreatePresignedUrlResponse>();

            Parallel.ForEach(presignedUrlRequests, (request) =>
            {
                responseCollection.Add(new CreatePresignedUrlResponse
                {
                    PartNumber = request.PartNumber,
                    PresignedUrl = _s3.GetPreSignedURL(request)
                });
            });

            return responseCollection.OrderBy(x => x.PartNumber).ToList();
        }

        private string ConstructFileKey(string fileName, string folderName)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("Filename cannot be empty");
            }

            if (folderName != null)
            {
                sb.Append($"{folderName}/");
            }

            sb.Append(fileName);

            return sb.ToString();
        }
    }
}
