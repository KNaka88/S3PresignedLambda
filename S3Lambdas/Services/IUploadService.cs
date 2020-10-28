using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace S3Lambdas.Services
{
    public interface IUploadService
    {
        public Task<APIGatewayProxyResponse> StartMultipartUploadAsync(APIGatewayProxyRequest apiRequest, ILambdaContext context);
    }
}
