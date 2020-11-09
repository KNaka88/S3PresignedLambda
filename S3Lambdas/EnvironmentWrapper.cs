using System;

namespace S3Lambdas
{
    /// <inheritdoc/>
    public class EnvironmentWrapper : IEnvironment
    {
        /// <inheritdoc/>
        public string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }

        /// <inheritdoc/>
        public void SetEnvironmentVariable(string variableName, string value)
        {
            Environment.SetEnvironmentVariable(variableName, value);
        }
    }
}
