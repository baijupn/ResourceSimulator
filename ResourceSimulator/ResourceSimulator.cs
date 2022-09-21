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
using System.Collections.Generic;
using System.Web.Http;
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
                if (cpuUtil >= 95.0)
                {
                    cpuUtil = 95.0;
                }
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
    // curl -X POST http://localhost:7143/api/get-resource -d '{}'
    // In Azure:
    // curl -X POST https://resourcesimulator20220918231716.azurewebsites.net/api/get-resource -d '{}'
    public static class GetResource
    {
        [FunctionName("get-resource")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Resource received.");
            var resourceCount = getIntValFromFile("resourceCount.txt");
            
            return (ActionResult)new OkObjectResult($"ResourceSimulator count:  {resourceCount}");

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
            log.LogInformation("Timer triggered");
            log.LogInformation($"current resource count: {resourceCount}");
            var cpuUtil = getFloatValFromFile("cpuUtil.txt");
            var operVal = getIntValFromFile("operVal.txt");
            log.LogInformation($"current operation: {operVal}");
            if (resourceCount == 0) // first time
            {
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
            Random rand = new Random();
            var memUtil = cpuUtil / 2 + rand.NextDouble() * 4;
            var sessEstRejectsOvld = 0;
            if (cpuUtil > 70.0)
            {
                sessEstRejectsOvld = (int)((cpuUtil - 70.0) * 100);
            }
            var sessionEstFailFpTimeout = 0;
            if (cpuUtil > 60.0)
            {
                sessionEstFailFpTimeout = (int)((cpuUtil - 60.0) * 33);
            }

            var ret = SendMetrics(cpuUtil, memUtil, sessEstRejectsOvld, sessionEstFailFpTimeout);

            if (ret.Result == 1)
            {
                log.LogInformation("sending create-slice request");
                var cRet = CreateTheSlice();
                
            }
            else if (ret.Result == 2)
            {
                log.LogInformation("sending create-slice request");
                var dRet = DeleteTheSlice();
            }

        }

        //HttpClient should be instancied once and not be disposed 
        private static readonly HttpClient client = new HttpClient();

        private static async Task<int> SendMetrics(double cpuUtil, double memUtil, int sessEstRejectsOvld, int sessionEstFailFpTimeout)
        {
            Console.WriteLine("Sending metric data");
            // send metrics to AI/ML component
            string metricsData = $"{{\r\n    \"data\": [\r\n        {{\r\n            \"Date\": \"{DateTime.Now}\",\r\n            \"UPF-CPU\": {cpuUtil},\r\n            \"UPF-Mem\": {memUtil},\r\n            \"UPF_SessionEstablishmentRejects_Overload\": {sessEstRejectsOvld},\r\n            \"UPF_SessionEstablishmentFailed_Fastpath_timeout\": {sessionEstFailFpTimeout}\r\n        }}\r\n    ]\r\n}}";
            Console.WriteLine(metricsData);
            string uri =  "http://localhost:7202/api/test-aiml";
            //string uri = "https://nssmf.azurewebsites.net/subscriptions/ecd77763-10fa-495b-963c-788721bde427/resourceGroups/TestingRG/nssmfs/myNssmf/ObjectManagement/v1/SliceProfiles";

            //POST the object to the specified URI 
            var response = await client.PostAsync(uri, new StringContent(metricsData));
            var ret = 0;
            if (response.IsSuccessStatusCode)
            {
                //Read back the answer from server
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Received suyccess response");

                Console.WriteLine(responseString);

                if (responseString.Contains("Create"))
                {
                    ret = 1;
                }
                else if (responseString.Contains("Delete"))
                {
                    ret = 2;
                }

            }
            else
            {
                Console.WriteLine("Received error response");
            }
            
            return ret;          
        }

        private static async Task<int> CreateTheSlice()
        {
            string uri = "http://localhost:7202/api/create-slice";
            //string uri = "https://nssmf.azurewebsites.net/subscriptions/ecd77763-10fa-495b-963c-788721bde427/resourceGroups/TestingRG/nssmfs/myNssmf/ObjectManagement/v1/SliceProfiles";
            //POST the object to the specified URI 
            var response = await client.PostAsync(uri, new StringContent("{}"));

            //Read back the answer from server
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
            return 1;
        }

        private static async Task<int> DeleteTheSlice()
        {
            string uri = "http://localhost:7202/api/delete-slice";
            //string uri = "https://nssmf.azurewebsites.net/subscriptions/ecd77763-10fa-495b-963c-788721bde427/resourceGroups/TestingRG/nssmfs/myNssmf/ObjectManagement/v1/SliceProfiles";
            //POST the object to the specified URI 
            var response = await client.DeleteAsync(uri);

            //Read back the answer from server
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
            return 1;
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
    // curl -X POST http://localhost:7143/api/test-aiml -d '{}'
    // In Azure:
    // curl -X POST https://resourcesimulator20220918231716.azurewebsites.net/api/test-aiml -d '{}'
    public static class TestAiMl
    {
        [FunctionName("test-aiml")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject(requestBody);
            log.LogInformation($"Test AI/ML processed. {input}");
            return (ActionResult)new OkObjectResult("Create");
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
            log.LogInformation($"Create Slice: Current Slice Count is {sliceCount.Result}");
            return (ActionResult)new OkObjectResult("New Slice Created.");
        }

        //HttpClient should be instancied once and not be disposed 
        private static readonly HttpClient client = new HttpClient();

        private static async Task<int> CreateTheSlice(string fileName)
        {
            var folder = Environment.ExpandEnvironmentVariables(@"%HOME%\data\MyFunctionAppData");
            var fullPath = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder); // noop if it already exists
            int sliceCount = 0;
            if (File.Exists(fullPath))
            {
                sliceCount = int.Parse(File.ReadAllText(fullPath));

            }
            ++sliceCount;
            string sliceString = "Slice-" + sliceCount.ToString();
            string content = $"{{\"serviceProfileId\":\"{sliceString}\"," +
                $"\"plmnInfoList\":[" +
                    $"{{\"plmnId\":{{\"mcc\":\"321\",\"mnc\":\"123\"}},\"snssai\":{{\"sst\":1,\"sd\":\"first\"}}}}," +
                    $"{{\"plmnId\":{{\"mcc\":\"321\",\"mnc\":\"123\"}},\"snssai\":{{\"sst\":1,\"sd\":\"second\"}}}}," +
                    $"{{\"plmnId\":{{\"mcc\":\"999\",\"mnc\":\"123\"}},\"snssai\":{{\"sst\":1,\"sd\":\"first\"}}}}]," +
               $"\"cNSliceSubnetProfile\":{{\"dLThptPerSliceSubnet\":{{\"servAttrCom\":{{}}}}," +
               $"\"dLThptPerUE\":{{\"servAttrCom\":{{}}}},\"uLThptPerSliceSubnet\":{{\"servAttrCom\":{{}}}}," +
               $"\"uLThptPerUE\":{{\"servAttrCom\":{{}}}},\"maxNumberOfPDUSessions\":500000,\"delayTolerance\":{{\"servAttrCom\":{{}}}}," +
               $"\"synchronicity\":{{}},\"dLDeterministicComm\":{{\"servAttrCom\":{{}}}},\"uLDeterministicComm\":{{\"servAttrCom\":{{}}}}," +
               $"\"nssaaSupport\":{{\"servAttrCom\":{{}}}},\"n6Protection\":{{\"servAttrCom\":{{}}}}}}," +
               $"\"rANSliceSubnetProfile\":{{\"dLThptPerSliceSubnet\":{{\"servAttrCom\":{{}}}}," +
               $"\"dLThptPerUE\":{{\"servAttrCom\":{{}}}},\"uLThptPerSliceSubnet\":{{\"servAttrCom\":{{}}}}," +
               $"\"uLThptPerUE\":{{\"servAttrCom\":{{}}}},\"delayTolerance\":{{\"servAttrCom\":{{}}}},\"positioning\":{{}}," +
               $"\"termDensity\":{{\"servAttrCom\":{{}}}},\"synchronicity\":{{}},\"dLDeterministicComm\":{{\"servAttrCom\":{{}}}}," +
               $"\"uLDeterministicComm\":{{\"servAttrCom\":{{}}}}}}," +
               $"\"topSliceSubnetProfile\":{{\"dLThptPerSliceSubnet\":{{\"servAttrCom\":{{}}}},\"dLThptPerUE\":{{\"servAttrCom\":{{}}}}," +
               $"\"uLThptPerSliceSubnet\":{{\"servAttrCom\":{{}}}},\"uLThptPerUE\":{{\"servAttrCom\":{{}}}}," +
               $"\"energyEfficiency\":{{\"servAttrCom\":{{}},\"performance\":{{}}}},\"synchronicity\":{{\"servAttrCom\":{{}}}}," +
               $"\"delayTolerance\":{{\"servAttrCom\":{{}}}},\"positioning\":{{\"servAttrCom\":{{}}}},\"termDensity\":{{\"servAttrCom\":{{}}}}," +
               $"\"dLDeterministicComm\":{{\"servAttrCom\":{{}}}},\"uLDeterministicComm\":{{\"servAttrCom\":{{}}}}}}}}";

            //string uri =  "http://localhost:7143/api/test-create-slice";
            string uri = "https://nssmf.azurewebsites.net/subscriptions/ecd77763-10fa-495b-963c-788721bde427/resourceGroups/TestingRG/nssmfs/myNssmf/ObjectManagement/v1/SliceProfiles";

            //POST the object to the specified URI 
            var response = await client.PostAsync(uri, new StringContent(content));

            //Read back the answer from server
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);

            File.WriteAllText(fullPath, (sliceCount).ToString());
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
            if (deleted.Result)
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

        //HttpClient should be instancied once and not be disposed 
        private static readonly HttpClient client = new HttpClient();

        private static async Task<bool> DeleteTheSlice(string fileName)
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
                    string sliceString = "Slice-" + sliceCount.ToString();
                    //string uri = "http://localhost:7143/api/test-create-slice";
                    string uri = "https://nssmf.azurewebsites.net/subscriptions/ecd77763-10fa-495b-963c-788721bde427/resourceGroups/TestingRG/nssmfs/myNssmf/ObjectManagement/v1/SliceProfiles/";

                    //Delete the object with specified URI 
                    var response = await client.DeleteAsync(uri + sliceString);

                    //Read back the answer from server
                    var responseString = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseString);
                    File.WriteAllText(fullPath, (--sliceCount).ToString());
                    return true;
                }
            }
            return false;
        }
    }

    public static class TestCreateSlice
    {
        [FunctionName("test-create-slice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject(requestBody);
            log.LogInformation($"Test Create Slice processed. {input}");
            return (ActionResult)new OkObjectResult("Test Create Slice processed.");
        }
    }
    public static class TestDeleteSlice
    {
        [FunctionName("test-delete-slice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "delete", Route = "test-delete-slice/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Test Delete Slice {id} processed.");
            return (ActionResult)new OkObjectResult($"Test Delete Slice {id} processed.");
        }
    }
}