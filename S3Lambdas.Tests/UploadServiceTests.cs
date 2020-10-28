using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using S3Lambdas.Models;
using S3Lambdas.Services;
using System;
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
        private TestLambdaContext _context;

        [SetUp]
        public void Setup()
        {
            _environment = Substitute.For<IEnvironment>();
            _environment.GetEnvironmentVariable("BUCKET_NAME").Returns(bucketName);
            _s3 = Substitute.For<IAmazonS3>();
            _s3.InitiateMultipartUploadAsync(Arg.Any<InitiateMultipartUploadRequest>()).Returns(new InitiateMultipartUploadResponse { UploadId = "42" });
            _uploadService = new UploadService(_s3, _environment);
            _context = new TestLambdaContext();
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_throw_when_no_fileName()
        {
            (StartMultipartUploadRequest request, APIGatewayProxyRequest apiGatewayProxyRequest) = PrepareRequest(fileName: null, folderName: "Folder1");

            Func<Task> act = async () => { await _uploadService.StartMultipartUploadAsync(apiGatewayProxyRequest, _context); };

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_throw_when_fileName_is_empty()
        {
            (StartMultipartUploadRequest request, APIGatewayProxyRequest apiGatewayProxyRequest) = PrepareRequest(fileName: "", folderName: "Folder1");

            Func<Task> act = async () => { await _uploadService.StartMultipartUploadAsync(apiGatewayProxyRequest, _context); };

            await act.Should().ThrowAsync<ArgumentNullException>();
        }


        [Test]
        public async Task StartMultipartUploadAsync_should_return_UploadId()
        {
            (StartMultipartUploadRequest request, APIGatewayProxyRequest apiGatewayProxyRequest) = PrepareRequest(fileName: "TestFile.pdf", folderName: null);

            var response = await _uploadService.StartMultipartUploadAsync(apiGatewayProxyRequest, _context);

            await _s3.Received().InitiateMultipartUploadAsync(Arg.Is<InitiateMultipartUploadRequest>(a =>
                a.BucketName == bucketName &&
                a.Key == request.FileName
            ));
            response.StatusCode.Should().Be(200);
            response.Body.Should().Be("42");
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_construct_key_with_folderName()
        {
            (StartMultipartUploadRequest request, APIGatewayProxyRequest apiGatewayProxyRequest) = PrepareRequest(fileName: "TestFile.pdf", folderName: "Folder1");

            var response = await _uploadService.StartMultipartUploadAsync(apiGatewayProxyRequest, _context);

            await _s3.Received().InitiateMultipartUploadAsync(Arg.Is<InitiateMultipartUploadRequest>(a =>
                a.BucketName == bucketName &&
                a.Key == $"{request.FolderName}/{request.FileName}"
            ));
        }

        private (StartMultipartUploadRequest request, APIGatewayProxyRequest proxyRequest) PrepareRequest(string fileName, string folderName)
        {
            var startMultipartUploadRequest = new StartMultipartUploadRequest
            {
                FileName = fileName,
                FolderName = folderName
            };

            var apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Body = JsonSerializer.Serialize(startMultipartUploadRequest)
            };

            return (startMultipartUploadRequest, apiGatewayProxyRequest);
        }
    }
}
