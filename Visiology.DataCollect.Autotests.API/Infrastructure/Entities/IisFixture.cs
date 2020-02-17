using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Xunit;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// IIS
    /// </summary>
    public class IisFixture : IDisposable
    {
        private Process iisProcess = new Process();

        public IisFixture()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            iisProcess.StartInfo.FileName = programFiles + @"\IIS Express\iisexpress.exe";
            iisProcess.StartInfo.Arguments = $"/config:\"{GetSolutionFolder()}\\.vs\\config\\applicationhost.config\" /site:\"Visiology.DataCollect.Web-Site\" /apppool:\"Clr4IntegratedAppPool\"";

            iisProcess.Start();
        }

        /// <summary>
        /// Получение расположения текущей директории приложения
        /// </summary>
        /// <returns></returns>
        private string GetApplicationPath()
        {
            throw new Exception();
           // return Path.GetFullPath(Path.Combine(this.GetCurrentDirectory(), this.config.GetValue("CurrentDirectory")));
        }

        private string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        }

        private string GetSolutionFolder()
        {
            var tmpDirName = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');

            return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(tmpDirName)));
        }

        public void Dispose()
        {
            if (!iisProcess.HasExited)
            {
                iisProcess.Kill();
            }
        }
    }

    [CollectionDefinition("Iis collection")]
    public class IisCollection : ICollectionFixture<IisFixture>
    {
    }
}
