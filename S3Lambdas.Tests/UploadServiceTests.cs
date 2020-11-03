using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using S3Lambdas.Models;
using S3Lambdas.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _s3.GetPreSignedURL(Arg.Any<GetPreSignedUrlRequest>()).Returns("presignedUrl.com");
            _uploadService = new UploadService(_s3, _environment);
            _context = new TestLambdaContext();
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_throw_when_no_fileName()
        {
            var request = PrepareRequest(fileName: null, folderName: "Folder1");

            Func<Task> act = async () => { await _uploadService.StartMultipartUploadAsync(request, _context); };

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_throw_when_fileName_is_empty()
        {
            var request = PrepareRequest(fileName: "", folderName: "Folder1");

            Func<Task> act = async () => { await _uploadService.StartMultipartUploadAsync(request, _context); };

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_return_UploadId()
        {
            var request = PrepareRequest(fileName: "TestFile.pdf", folderName: null);

            var response = await _uploadService.StartMultipartUploadAsync(request, _context);

            await _s3.Received().InitiateMultipartUploadAsync(Arg.Is<InitiateMultipartUploadRequest>(a =>
                a.BucketName == bucketName &&
                a.Key == request.FileName
            ));
            response.Should().BeOfType<InitiateMultipartUploadResponse>();
            response.UploadId.Should().Be("42");
        }

        [Test]
        public async Task StartMultipartUploadAsync_should_construct_key_with_folderName()
        {
            var request = PrepareRequest(fileName: "TestFile.pdf", folderName: "Folder1");

            var response = await _uploadService.StartMultipartUploadAsync(request, _context);

            await _s3.Received().InitiateMultipartUploadAsync(Arg.Is<InitiateMultipartUploadRequest>(a =>
                a.BucketName == bucketName &&
                a.Key == $"{request.FolderName}/{request.FileName}"
            ));
        }

        [Test]
        public void CreatePresignedUrl_should_call_S3_GetPresignedURL()
        {
            var request = new PresignedUrlRequest
            {
                ContentType = "application/pdf",
                FileName = "TestFile.pdf",
                FolderName = "Folder1",
                PartNumbers = new List<int> { 1, 3, 2, 4, 5 },
                UploadId = "42"
            };

            _uploadService.CreatePresignedUrl(request, _context);

            _s3.Received(5).GetPreSignedURL(Arg.Is<GetPreSignedUrlRequest>(p => 
                p.ContentType == "application/pdf" &&
                p.Key == $"{request.FolderName}/{request.FileName}" &&
                p.UploadId == request.UploadId
            ));
        }

        [Test]
        public void CreatePresignedUrl_should_return_list_of_PresignedUrlResponse()
        {
            var request = new PresignedUrlRequest
            {
                ContentType = "application/pdf",
                FileName = "TestFile.pdf",
                FolderName = "Folder1",
                PartNumbers = new List<int> { 1, 3, 2 },
                UploadId = "42"
            };

            var response = _uploadService.CreatePresignedUrl(request, _context);

            var index = 1;
            foreach (var createPresignedUrlResponse in response)
            {
                createPresignedUrlResponse.PartNumber.Should().Be(index);
                createPresignedUrlResponse.PresignedUrl.Should().Be("presignedUrl.com");
                index++;
            }
        }

        private StartMultipartUploadRequest PrepareRequest(string fileName, string folderName)
        {
            return new StartMultipartUploadRequest
            {
                FileName = fileName,
                FolderName = folderName
            };
        }
    }
}
