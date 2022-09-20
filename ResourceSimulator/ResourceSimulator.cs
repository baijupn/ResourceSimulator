using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
//using System.Runtime.Caching;


namespace ResourceSimulator
{
    // Local:
    // curl -X POST http://localhost:7143/api/create-resource -d '{}'
    // In Azure:
    // curl -X POST https://resourcesimulator20220918231716.azurewebsites.net/api/create-resource -d '{}'
    public static class CreateResource
    {
        [FunctionName("create-resource")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Create Resource processed.");
            return (ActionResult)new OkObjectResult("Create Resource processed.");
        }
    }

    // Local:
    // curl -X DELETE  http://localhost:7143/api/delete-resource
    // In Azure:
    // curl -X DELETE  https://resourcesimulator20220918231716.azurewebsites.net/api/delete-resource
    public static class DeleteResource
    {
        [FunctionName("delete-resource")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Delete Resource processed.");
            return (ActionResult)new OkObjectResult("Delete Resource processed.");
        }
    }
    public class TimeTrigger
    {
        [FunctionName("time-trigger")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var persistedCount = IncrementInvocationCountFile("invocations.txt");
            log.LogInformation($"Webhook triggered {persistedCount}");
        }
        private static int IncrementInvocationCountFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var persistedCount = 0;
            if (File.Exists(fullPath))
            {
                persistedCount = int.Parse(File.ReadAllText(fullPath));
            }
            File.WriteAllText(fullPath, (++persistedCount).ToString());
            return persistedCount;
        }
    }

    // Local:
    // curl -X POST http://localhost:7143/api/create-slice -d '{}'
    // In Azure:
    // curl -X POST https://resourcesimulator20220918231716.azurewebsites.net/api/create-slice -d '{}'
    public static class CreateSlice
    {
        [FunctionName("create-slice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var sliceCount = CreateTheSlice("slice.txt");
            log.LogInformation($"Create Slice: Current Slice Count is {sliceCount}");
            return (ActionResult)new OkObjectResult("New Slice Created.");
        }
        private static int CreateTheSlice(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            int sliceCount = 0;
            if (File.Exists(fullPath))
            {
                sliceCount = int.Parse(File.ReadAllText(fullPath));
            }
            File.WriteAllText(fullPath, (++sliceCount).ToString());

            return sliceCount;
        }
    }

    // Local:
    // curl -X DELETE  http://localhost:7143/api/delete-slice
    // In Azure:
    // curl -X DELETE  https://resourcesimulator20220918231716.azurewebsites.net/api/delete-slice
    public static class DeleteSlice
    {
        [FunctionName("delete-slice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            var deleted = DeleteTheSlice("slice.txt");
            if (deleted)
            {
                log.LogInformation("Delete Slice: Slice Deleted.");
                return (ActionResult)new OkObjectResult("Slice Deleted.");
            }
            else
            {
                log.LogInformation("Delete Slice: No Slice Deleted.");
                return (ActionResult)new OkObjectResult("No Slice Deleted.");
            }
        }
        private static bool DeleteTheSlice(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            int sliceCount = 0;
            if (File.Exists(fullPath))
            {
                sliceCount = int.Parse(File.ReadAllText(fullPath));
                if (sliceCount > 0)
                {
                    File.WriteAllText(fullPath, (--sliceCount).ToString());
                    return true;
                }
            }
            return false;
        }
    }
}
