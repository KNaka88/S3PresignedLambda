using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using S3Lambdas.Models;
using System;
using System.Collections.Generic;
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

        public async Task<APIGatewayProxyResponse> StartMultipartUploadAsync(APIGatewayProxyRequest apiRequest, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var startMultipartUploadRequest = JsonSerializer.Deserialize<StartMultipartUploadRequest>(apiRequest.Body);

            var uploadRequest = new InitiateMultipartUploadRequest
            {
                BucketName = _environment.GetEnvironmentVariable("BUCKET_NAME"),
                Key = ConstructFileKey(startMultipartUploadRequest),
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };
            var uploadResponse = await _s3.InitiateMultipartUploadAsync(uploadRequest);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = uploadResponse.UploadId,
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }

        private string ConstructFileKey(StartMultipartUploadRequest request)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(request.FileName))
            {
                throw new ArgumentNullException("Filename cannot be empty");
            }

            if (request.FolderName != null)
            {
                sb.Append($"{request.FolderName}/");
            }

            sb.Append(request.FileName);

            return sb.ToString();
        }
    }
}
