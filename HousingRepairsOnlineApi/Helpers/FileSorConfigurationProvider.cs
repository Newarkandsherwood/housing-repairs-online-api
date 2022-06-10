using System;
using System.IO;
using System.IO.Abstractions;
using Ardalis.GuardClauses;

namespace HousingRepairsOnlineApi.Helpers
{
    public class FileSorConfigurationProvider : ISorConfigurationProvider
    {
        private string sorConfigPath;
        private IFileSystem fileSystem;

        public FileSorConfigurationProvider(string sorConfigPath, IFileSystem fileSystem = null)
        {
            Guard.Against.NullOrWhiteSpace(sorConfigPath, nameof(sorConfigPath));

            this.fileSystem = fileSystem ?? new FileSystem();
            this.sorConfigPath = sorConfigPath;
        }

        public string ConfigurationValue()
        {
            string sorConfigurationValue;
            try
            {
                sorConfigurationValue = fileSystem.File.ReadAllText(sorConfigPath);
            }
            catch (FileNotFoundException e)
            {
                throw new InvalidOperationException($"Required configuration file '{sorConfigPath}' not found.", e);
            }

            return sorConfigurationValue;
        }
    }
}
