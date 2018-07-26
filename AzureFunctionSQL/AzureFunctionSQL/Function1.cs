
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureFunctionSQL
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var config = new ConfigurationBuilder()
                 .SetBasePath(context.FunctionAppDirectory)
                 .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();

            var cstr = config.GetConnectionString("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                var text = $"SELECT * FROM dbo.Car";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected. 
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info($"{reader[0]} - {reader[1]}");
                    }
                    reader.Close();
                }
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
