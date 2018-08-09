using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using Microsoft.AspNetCore.Mvc;

namespace Ttl.CircuitBreakerManagementService.Dashboard.Controllers
{
    public class LogsController : Controller
    {
        private Dictionary<string, string> GetLogFileNames()
        {
            var logFiles = new Dictionary<string, string>();
            var repositories = LogManager.GetAllRepositories();
            foreach (var repository in repositories)
            {
                var appenders = repository.GetAppenders();
                foreach (var appender in appenders)
                {
                    if (appender is RollingFileAppender)
                    {
                        var fileName = ((RollingFileAppender) appender).File;
                        logFiles.Add(Path.GetFileName(fileName), fileName);
                    }
                }
            }
            return logFiles;
        }

        [AcceptVerbs("GET")]
        public ActionResult Index()
        {
            var logFiles = GetLogFileNames();
            return View(logFiles.Keys.ToList());
        }

        [AcceptVerbs("GET")]
        public FileResult Download(string fileName)
        {
            var logFiles = GetLogFileNames();
            var logFileName = logFiles[fileName];
            var tempName = Path.GetTempFileName();
            System.IO.File.Copy(logFileName, tempName, true);
            var downloadName = Path.GetFileName(logFileName);
            return File(tempName, "text/plain", downloadName);
        }
    }
}
