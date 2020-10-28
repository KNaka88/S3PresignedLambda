using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using S3Lambdas.Models;
using S3Lambdas.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace S3Lambdas.Tests
{
    [TestFixture]
    public class UploadServiceTests
    {
        private IEnvironment _environment;
        private IAmazonS3 _s3;
        private IUploadService _uploadService;
        private readonly string bucketName = "bucketName";

        [SetUp]
        public void Setup()
        {
            _environment = Substitute.For<IEnvironment>();
            _environment.GetEnvironmentVariable("BUCKET_NAME").Returns(bucketName);
            _s3 = Substitute.For<IAmazonS3>();
            _s3.InitiateMultipartUploadAsync(Arg.Any<InitiateMultipartUploadRequest>()).Returns(new InitiateMultipartUploadResponse { UploadId = "42" });
            _uploadService = new UploadService(_s3, _environment);
        }

        [Test]
        public async Task Foo()
        {
            var body = new StartMultipartUploadRequest
            {
                FileName = "TestFile.pdf",
            };

            var apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Body = JsonSerializer.Serialize(body)
            };

            var context = new TestLambdaContext();

            var response = await _uploadService.StartMultipartUploadAsync(apiGatewayProxyRequest, context);

            await _s3.Received().InitiateMultipartUploadAsync(Arg.Is<InitiateMultipartUploadRequest>(a =>
                a.BucketName == bucketName &&
                a.Key == "TestFile.pdf"
            ));
            response.StatusCode.Should().Be(200);
            response.Body.Should().Be("42");
        }
    }
}
