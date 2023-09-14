using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace readAssembly
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            bool ret = true;
            try
            {
                // convert dllfile to byte Type
                string fileFullPath = Environment.CurrentDirectory + "\\" + "Azure.Core.dll";
                byte[] bt = null;
                using (var fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
                {
                    bt = new byte[fs.Length];
                    fs.Read(bt, 0, bt.Length);
                }
                //convert byte to Base64 type
                string base64str = Convert.ToBase64String(bt);
                log.LogInformation("first 10 character from base64-string : " + base64str.Substring(0, 10));
                var dllFileBin = Convert.FromBase64String(base64str);

                var assembly = Assembly.Load(dllFileBin);
                log.LogInformation("read assembly info : " + assembly.ToString());
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                ret = false;
            }

            return new OkObjectResult(ret);
        }
    }
}
