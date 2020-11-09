namespace S3Lambdas
{
    /// <summary>
    /// The wrapper of Environment class.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Get the variable of environment.
        /// </summary>
        /// <param name="variableName">The variable name to get.</param>
        /// <returns>The environment variable value.</returns>
        public string GetEnvironmentVariable(string variableName);

        /// <summary>
        /// Set the Environment variable.
        /// </summary>
        /// <param name="variableName">The variable name to set.</param>
        /// <param name="value">The variable value to set.</param>
        public void SetEnvironmentVariable(string variableName, string value);
    }
}
