using System;

namespace S3Lambdas
{
    public class EnvironmentWrapper : IEnvironment
    {
        public string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }

        public void SetEnvironmentVariable(string variableName, string value)
        {
            Environment.SetEnvironmentVariable(variableName, value);
        }
    }
}
