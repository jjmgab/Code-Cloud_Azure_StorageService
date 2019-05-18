using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;

namespace StorageService
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string email = req.Query["email"];

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    config["name"], config["key"]), true);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get a reference to a table named "peopleTable"
            CloudTable peopleTable = tableClient.GetTableReference("peopleTable");

            await peopleTable.CreateIfNotExistsAsync();

            CustomerEntity customer1 = new CustomerEntity(email, email);

            TableOperation insertOperation = TableOperation.Insert(customer1);
            await peopleTable.ExecuteAsync(insertOperation);

            return email != null
                ? (ActionResult)new OkObjectResult($"Hello, {email}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
