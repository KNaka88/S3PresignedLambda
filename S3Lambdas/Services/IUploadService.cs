using Amazon.Lambda.Core;
using Amazon.S3.Model;
using S3Lambdas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3Lambdas.Services
{
    /// <summary>
    /// Resnposible for uploading files to S3.
    /// </summary>
    public interface IUploadService
    {
        /// <summary>
        /// Sending to request to S3 to start multi part upload.
        /// </summary>
        /// <param name="apiRequest">The request of <see cref="StartMultipartUploadRequest"/>.</param>
        /// <param name="context">The <see cref="ILambdaContext"/>.</param>
        /// <returns></returns>
        public Task<InitiateMultipartUploadResponse> StartMultipartUploadAsync(StartMultipartUploadRequest apiRequest, ILambdaContext context);

        /// <summary>
        /// Create presigned urls so that frontend cant upload the file directly to S3 bucket.
        /// </summary>
        /// <param name="request">The <see cref="PresignedUrlRequest"/>.</param>
        /// <param name="context">The <see cref="ILambdaContext"/>.</param>
        /// <returns>THe list of <see cref="CreatePresignedUrlResponse"/>.</returns>
        public IList<CreatePresignedUrlResponse> CreatePresignedUrl(PresignedUrlRequest request, ILambdaContext context);

        /// <summary>
        /// Sends request to complete the multi part upload.
        /// </summary>
        /// <param name="request">The request of <see cref="CompleteMultipartRequest"/>.</param>
        /// <returns>The void of <see cref="Task"/>.</returns>
        public Task CompleteMultiPartUploadAsync(CompleteMultipartRequest request);
    }
}
