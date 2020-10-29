using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3.Model;
using S3Lambdas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3Lambdas.Services
{
    public interface IUploadService
    {
        public Task<InitiateMultipartUploadResponse> StartMultipartUploadAsync(StartMultipartUploadRequest apiRequest, ILambdaContext context);
        public IList<CreatePresignedUrlResponse> CreatePresignedUrl(PresignedUrlRequest request, ILambdaContext context);

    }
}
