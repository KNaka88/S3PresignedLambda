namespace S3Lambdas
{
    public interface IEnvironment
    {
        public string GetEnvironmentVariable(string variableName);
        public void SetEnvironmentVariable(string variableName, string value);
    }
}
