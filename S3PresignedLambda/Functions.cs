using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using S3Lambdas.Services;

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
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> StartMultipartUpload(APIGatewayProxyRequest apiRequest, ILambdaContext context)
        {
            return await _serviceProvider.GetService<IUploadService>().StartMultipartUploadAsync(apiRequest, context);
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
