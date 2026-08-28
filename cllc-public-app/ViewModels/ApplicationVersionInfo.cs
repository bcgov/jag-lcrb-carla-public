namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class ApplicationVersionInfo
    {
        /// <summary>
        /// Base Path of the application
        /// </summary>
        public string BasePath { get; set; }
        /// <summary>
        /// Base URI for the application
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Dotnet Environment (Development, Staging, Production...) — this is the
        /// ASP.NET Core hosting environment, kept fixed at "Production" in every
        /// deployed tier to avoid a DI-validation crash; it does NOT indicate which
        /// deployment tier (dev/test/prod) is actually running. Use DeploymentTier for that.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Which deployment tier this build is running in (Development, Test, Production),
        /// set explicitly by the deploy/promote pipeline based on the target environment.
        /// </summary>
        public string DeploymentTier { get; set; }

        /// <summary>
        /// File creation time for the running assembly
        /// </summary>
        public string FileCreationTime { get; set; }

        /// <summary>
        /// File version for the running assembly
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// Git commit used to build the application
        /// </summary>
        public string SourceCommit { get; set; }

        /// <summary>
        /// Git reference used to build the application
        /// </summary>
        public string SourceReference { get; set; }

        /// <summary>
        /// Git repository used to build the application
        /// </summary>
        public string SourceRepository { get; set; }
    }
}
