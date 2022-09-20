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

            //POST the object to the specified URI 
            var response = await client.PostAsync("http://localhost:7143/api/test-create-slice", new StringContent(content));

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

                    //Delete the object with specified URI 
                    var response = await client.DeleteAsync("http://localhost:7143/api/test-delete-slice/" + sliceString);

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
