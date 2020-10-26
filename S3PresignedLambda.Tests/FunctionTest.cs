
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using S3Lambdas;

namespace S3PresignedLambda.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
        }

        [Fact]
        public void TetGetMethod()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions();


            request = new APIGatewayProxyRequest();
            context = new TestLambdaContext();
            response = functions.StartMultipartUpload(request, context);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("Hello AWS Serverless", response.Body);
        }
    }
}
