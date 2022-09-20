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
            var resourceCount = getIntValFromFile("resourceCount.txt");
            
            log.LogInformation($"current resource count: {resourceCount}");
            if (resourceCount >= 15)
            {
                log.LogError($"resource count limit reached. No new resources added");
                return (ActionResult)new InternalServerErrorResult();
            }
            var cpuUtil = 0.0;
            
            if (resourceCount == 0) // first time
            {
                log.LogInformation($"first time invocation. setting resourceCount to 5.");
                resourceCount = 5;
                cpuUtil = 30.0;
                int operVal = 1;
                writeIntValToFile("operVal.txt", operVal);
            }
            else
            {
                var oldResourceCount = resourceCount;
                resourceCount += 5;
                cpuUtil = getFloatValFromFile("cpuUtil.txt");
                log.LogInformation($"current cpu util: {cpuUtil}");
                cpuUtil = cpuUtil / ((float)resourceCount / oldResourceCount);
            }
                        
            writeIntValToFile("resourceCount.txt", resourceCount);
            writeFloatValToFile("cpuUtil.txt", cpuUtil);
            log.LogInformation($"new resource count: {resourceCount}");
            log.LogInformation($"New cpu util: {cpuUtil}");
            return (ActionResult)new OkObjectResult("Create Resource processed.");

            
        }

        private static int getIntValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var intVal = 0;
            if (File.Exists(fullPath))
            {
                intVal = int.Parse(File.ReadAllText(fullPath));
            }

            return intVal;
        }

        private static void writeIntValToFile(string fileName, int intVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (intVal).ToString());
        }

        private static double getFloatValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var floatVal = 0.0;
            if (File.Exists(fullPath))
            {
                floatVal = float.Parse(File.ReadAllText(fullPath));
            }

            return floatVal;
        }

        private static void writeFloatValToFile(string fileName, double floatVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (floatVal).ToString());
        }
    }

    // Local:
    // curl -X POST http://localhost:7143/api/reset-resource -d '{}'
    // In Azure:
    // curl -X POST https://resourcesimulator20220918231716.azurewebsites.net/api/reset-resource -d '{}'
    public static class ResetResource
    {
        [FunctionName("reset-resource")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Reset Resource received.");
            var resourceCount = 0;
            var cpuUtil = 0.0;
            var operVal = 1;
            

            writeIntValToFile("resourceCount.txt", resourceCount);
            writeFloatValToFile("cpuUtil.txt", cpuUtil);
            writeIntValToFile("operVal.txt", operVal);

            return (ActionResult)new OkObjectResult("Reset Resource processed.");

        }

        private static int getIntValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var intVal = 0;
            if (File.Exists(fullPath))
            {
                intVal = int.Parse(File.ReadAllText(fullPath));
            }

            return intVal;
        }

        private static void writeIntValToFile(string fileName, int intVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (intVal).ToString());
        }

        private static double getFloatValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var floatVal = 0.0;
            if (File.Exists(fullPath))
            {
                floatVal = float.Parse(File.ReadAllText(fullPath));
            }

            return floatVal;
        }

        private static void writeFloatValToFile(string fileName, double floatVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (floatVal).ToString());
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
            var resourceCount = getIntValFromFile("resourceCount.txt");
            log.LogInformation($"current resource count: {resourceCount}");
            if (resourceCount <= 5)
            {
                log.LogError($"resource count limit reached. No resources deleted");
                return (ActionResult)new OkObjectResult("Delete Resource processed.");
            }
            var oldResourceCount = resourceCount;
            resourceCount -= 5;
            log.LogInformation($"new resource count: {resourceCount}");
            writeIntValToFile("resourceCount.txt", resourceCount);
            var cpuUtil = getFloatValFromFile("cpuUtil.txt");
            log.LogInformation($"current cpu util: {cpuUtil}");
            cpuUtil = cpuUtil * ((float)oldResourceCount / resourceCount);
            if (cpuUtil >= 95.0)
            {
                cpuUtil = 95.0;
            }
            writeFloatValToFile("cpuUtil.txt", cpuUtil);
            log.LogInformation($"New cpu util: {cpuUtil}");
            return (ActionResult)new OkObjectResult("Delete Resource processed.");
        }

        private static int getIntValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var intVal = 0;
            if (File.Exists(fullPath))
            {
                intVal = int.Parse(File.ReadAllText(fullPath));
            }

            return intVal;
        }

        private static void writeIntValToFile(string fileName, int intVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (intVal).ToString());
        }

        private static double getFloatValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var floatVal = 0.0;
            if (File.Exists(fullPath))
            {
                floatVal = float.Parse(File.ReadAllText(fullPath));
            }

            return floatVal;
        }

        private static void writeFloatValToFile(string fileName, double floatVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (floatVal).ToString());
        }
    }

    public class TimeTrigger
    {
        [FunctionName("time-trigger")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        //public void Run([TimerTrigger("10 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var resourceCount = getIntValFromFile("resourceCount.txt");
            log.LogInformation($"Webhook triggered");
            log.LogInformation($"current resource count: {resourceCount}");
            var cpuUtil = getFloatValFromFile("cpuUtil.txt");
            var operVal = getIntValFromFile("operVal.txt");
            log.LogInformation($"current operation: {operVal}");
            if (resourceCount == 0) // first time
            {
                //log.LogInformation($"first time invocation. setting resourceCount to 5.");
                //resourceCount = 5;
                //cpuUtil = 30.0;
                //writeIntValToFile("resourceCount.txt", resourceCount);
                //log.LogInformation($"New resource count: {resourceCount}");
                //writeFloatValToFile("cpuUtil.txt", cpuUtil);
                //log.LogInformation($"New cpu util: {cpuUtil}");
                operVal = 1;
                writeIntValToFile("operVal.txt", operVal);
                return;

            }
            
            log.LogInformation($"current cpu util: {cpuUtil}");
           
            if (resourceCount >= 15 && operVal == 1 && 
                (cpuUtil > 80.0))
            {
                operVal = 0;
                writeIntValToFile("operVal.txt", operVal);
            }
            if (resourceCount <= 5 && operVal == 0 && 
                (cpuUtil <= 30.0))
            {
                operVal = 1;
                writeIntValToFile("operVal.txt", operVal);
            }
            
            if (operVal == 1 &&
                cpuUtil < 95.0)
            {
                cpuUtil += 5.0;
            }
            else if (operVal == 0 &&
                    cpuUtil > 10.0)
            {
                cpuUtil -= 5.0;
            }
            writeFloatValToFile("cpuUtil.txt", cpuUtil);
            log.LogInformation($"New cpu util: {cpuUtil}");
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

        private static int getIntValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var intVal = 0;
            if (File.Exists(fullPath))
            {
                intVal = int.Parse(File.ReadAllText(fullPath));
            }
            
            return intVal;
        }

        private static void writeIntValToFile(string fileName, int intVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (intVal).ToString());
        }

        private static double getFloatValFromFile(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            var floatVal = 0.0;
            if (File.Exists(fullPath))
            {
                floatVal = float.Parse(File.ReadAllText(fullPath));
            }

            return floatVal;
        }

        private static void writeFloatValToFile(string fileName, double floatVal)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists

            File.WriteAllText(fullPath, (floatVal).ToString());
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
