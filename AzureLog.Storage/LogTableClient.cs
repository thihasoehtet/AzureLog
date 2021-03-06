﻿using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLog.Storage
{
    public class LogTableClient
    {
        private readonly CloudTable _cloudTable;
        private readonly string _connectionStringKey;

        private LogTableClient(string connectionStringKey, CloudTable cloudTable)
        {
            _connectionStringKey = connectionStringKey;
            _cloudTable = cloudTable;
        }


        public static LogTableClient Create(string connectionStringKey, string tableName)
        {
            var storageAccount = GetStorageAccount(connectionStringKey);
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            var cloudTable = tableClient.GetTableReference(tableName);
            cloudTable.CreateIfNotExists();

            return new LogTableClient(connectionStringKey, cloudTable);
        }

        private static string GetStorageAccountConnectionString(string connectionStringKey)
        {
            // try get connection string from app settings or could service config
            var connectionStringValue = CloudConfigurationManager.GetSetting(connectionStringKey);
            if (!string.IsNullOrEmpty(connectionStringValue)) return connectionStringValue;

            // try get connection string from ConfigurationManager.ConnectionStrings
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey];
            if (connectionString != null)
            {
                connectionStringValue = connectionString.ConnectionString;
            }
            return connectionStringValue;
        }

        private static CloudStorageAccount GetStorageAccount(string connectionStringKey)
        {
            var connectionString = GetStorageAccountConnectionString(connectionStringKey);
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount;
        }

        public void Insert(LogEntity entity)
        {
            var insertOperation = TableOperation.Insert(entity);
            _cloudTable.Execute(insertOperation);
        }
    }
}
