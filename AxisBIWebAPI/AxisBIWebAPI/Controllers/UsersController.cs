using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserWebAPI.Models;


namespace AxisBIWebAPI.Controllers
{

    public class UsersController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Usuarios> Get()
        {
            return this.queryAzureStorage();
        }
        // GET api/<controller>/5
        public Usuarios Get(string id)
        {
            return this.queryAzureStorage("usuarios", id).FirstOrDefault();
        }

        // POST api/<controller>
        public Usuarios Post([FromBody]Usuarios value)
        {
            CloudTable table = getTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(value);
            TableResult insertResult = table.Execute(insertOperation);
            return insertResult.Result as Usuarios;
        }

        // PUT api/<controller>/5
        public Usuarios Put(string id, [FromBody]Usuarios value)
        {
            CloudTable table = getTable();
            TableOperation retrieveOperation = TableOperation.Retrieve<Usuarios>("usuarios", id);
            TableResult retrieveResult = table.Execute(retrieveOperation);
            Usuarios usuario = retrieveResult.Result as Usuarios;

            if (usuario != null)
            {
                usuario.Merge(value);

                TableOperation updateOperation = TableOperation.InsertOrMerge(usuario);
                TableResult updateResult = table.Execute(updateOperation);
                return updateResult.Result as Usuarios;
            }

            return null;
        }

        // DELETE api/<controller>/5
        public void Delete(string id)
        {
            CloudTable table = getTable();
            TableOperation retrieveOperation = TableOperation.Retrieve<Usuarios>("usuarios", id);
            TableResult retrieveResult = table.Execute(retrieveOperation);

            Usuarios usuario = retrieveResult.Result as Usuarios;
            if (usuario != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(usuario);
                TableResult deleteResult = table.Execute(deleteOperation);
            }
        }

        #region Setup Usuarios
        [HttpGet]
        [Route("Setup/CreateTableStorage")]
        public IEnumerable<Usuarios> GetCreataTableStorage()
        {
            CloudTable table = getTable();
            table.CreateIfNotExists();

            TableBatchOperation batch = new TableBatchOperation();
            batch.InsertOrMerge(new Usuarios("usuarios", "759990") { Nome = "Christiano Donke", Email = "chrido@microsoft.com" });
            batch.InsertOrMerge(new Usuarios("usuarios", "677040") { Nome = "Alexandre Campos", Email = "alexde@microsoft.com" });

            table.ExecuteBatch(batch);

            return this.queryAzureStorage();
        }

        [HttpPost]
        [Route("Setup/CreateTableStorage")]
        public IEnumerable<Usuarios> PutCreataTableStorage([FromBody]IEnumerable<Usuarios> usuarios)
        {
            CloudTable table = getTable();
            table.CreateIfNotExists();

            TableBatchOperation batch = new TableBatchOperation();
            foreach (var usuario in usuarios)
                batch.InsertOrMerge(usuario);

            table.ExecuteBatch(batch);

            return this.queryAzureStorage();
        }
        #endregion

        #region Azure Table Storage functions
        IEnumerable<Usuarios> queryAzureStorage()
        {
            return this.queryAzureStorage(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "usuarios"));
        }
        IEnumerable<Usuarios> queryAzureStorage(string partitionKey, string rowKey)
        {
            return this.queryAzureStorage(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)
                )
            );
        }
        IEnumerable<Usuarios> queryAzureStorage(string filterCondition)
        {
            CloudTable table = getTable();
            TableQuery<Usuarios> query = new TableQuery<Usuarios>().Where(filterCondition);

            return table.ExecuteQuery(query);
        }

        private CloudTable getTable()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureTableStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 5);
            CloudTable table = tableClient.GetTableReference("Usuarios");

            return table;
        }

        #endregion
    }

}