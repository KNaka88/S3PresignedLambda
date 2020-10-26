using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text.Json;
using S3Lambdas.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3Lambdas
{
    public class Functions
    {
        private readonly IAmazonS3 _s3;
        private readonly string BUCKET_NAME;
        private readonly string REGION;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            REGION = Environment.GetEnvironmentVariable("AWS_REGION");
            BUCKET_NAME = Environment.GetEnvironmentVariable("BUCKET_NAME");
            _s3 = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(REGION));
        }


        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> StartMultipartUpload(APIGatewayProxyRequest apiRequest, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var startMultipartUploadRequest = JsonSerializer.Deserialize<StartMultipartUploadRequest>(apiRequest.Body);

            var uploadRequest = new InitiateMultipartUploadRequest
            {
                BucketName = BUCKET_NAME,
                Key = $"{startMultipartUploadRequest.FileName}",
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };
            var uploadResponse = await _s3.InitiateMultipartUploadAsync(uploadRequest);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(uploadResponse.UploadId),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}
