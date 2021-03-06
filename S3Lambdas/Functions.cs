using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using S3Lambdas.Services;
using S3Lambdas.Models;
using System.Collections.Generic;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3Lambdas
{
    public class Functions
    {
        private readonly ServiceProvider _serviceProvider;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// A Lambda function to initiating the multipart upload.
        /// </summary>
        /// <param name="request">The <see cref="APIGatewayProxyRequest"/>.</param>
        /// <returns>The <see cref="Task"/> of the <see cref="APIGatewayProxyResponse"/>.</returns>
        public async Task<APIGatewayProxyResponse> StartMultipartUpload(APIGatewayProxyRequest apiRequest, ILambdaContext context)
        {
            var request = JsonConverter.Deserialize<StartMultipartUploadRequest>(apiRequest.Body);
            var uploadResponse = await _serviceProvider.GetService<IUploadService>().StartMultipartUploadAsync(request, context);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = uploadResponse.UploadId,
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Headers", "*" },
                    { "Access-Control-Allow-Methods", "POST" }
                }
            };
        }

        /// <summary>
        /// A Lambda function to create presigned urls. It has to be called after the StartMultipartUpload.
        /// </summary>
        /// <param name="request">The <see cref="APIGatewayProxyRequest"/>.</param>
        /// <returns>The <see cref="APIGatewayProxyResponse"/>.</returns>
        public APIGatewayProxyResponse CreatePresignedUrl(APIGatewayProxyRequest apiRequest, ILambdaContext context)
        {
            var request = JsonConverter.Deserialize<PresignedUrlRequest>(apiRequest.Body);
            var responseList = _serviceProvider.GetService<IUploadService>().CreatePresignedUrl(request, context);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConverter.SerializeWithCamelCase(responseList),
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Headers", "*" },
                    { "Access-Control-Allow-Methods", "POST" }
                }
            };
        }

        /// <summary>
        /// A Lambda function to complete multi part upload after file uploading is done.
        /// </summary>
        /// <param name="request">The <see cref="APIGatewayProxyRequest"/>.</param>
        /// <returns>The <see cref="Task"/> of the <see cref="APIGatewayProxyResponse"/>.</returns>
        public async Task<APIGatewayProxyResponse> CompleteMultiPartUpload(APIGatewayProxyRequest apiRequest, ILambdaContext context)
        {
            var request = JsonConverter.Deserialize<CompleteMultipartRequest>(apiRequest.Body);
            await _serviceProvider.GetService<IUploadService>().CompleteMultiPartUploadAsync(request);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, string> { 
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Headers", "*" },
                    { "Access-Control-Allow-Methods", "POST" }
                }
            };
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironment, EnvironmentWrapper>();
            serviceCollection.AddTransient<IUploadService, UploadService>();

            var region = Environment.GetEnvironmentVariable("AWS_REGION");
            serviceCollection.AddSingleton<IAmazonS3, AmazonS3Client>(sp => new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region)));
        }
    }
}
