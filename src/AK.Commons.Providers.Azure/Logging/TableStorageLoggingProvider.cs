/*******************************************************************************************************************************
 * AK.Commons.Providers.Azure.Logging.TableStorageLoggingProvider
 * Copyright © 2013-2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library Provider Set (AKCLPS).
 *  
 * AKCLPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCLPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCLPS.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using AK.Commons.Logging;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ComponentModel.Composition;
using System.Reflection;

#endregion

namespace AK.Commons.Providers.Azure.Logging
{
    /// <summary>
    /// Logging provider that logs to Azure Table storage.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof(ILoggingProvider))]
    public class TableStorageLoggingProvider : LoggingProviderBase
    {
        #region Constants

        private const string ConfigKeyFormatUrl = "{0}.url";
        private const string ConfigKeyFormatAccountName = "{0}.accountname";
        private const string ConfigKeyFormatAccessKey = "{0}.accesskey";
        private const string ConfigKeyFormatTableName = "{0}.tablename";
        private const string ConfigKeyFormatPartitionKeyPropertyName = "{0}.partitionkeypropertyname";

        #endregion

        #region Fields

        private CloudTableClient client;
        private CloudTable table;
        private PropertyInfo partitionKeyProperty;

        #endregion

        #region Properties (Private)

        private string ConfigKeyUrl { get { return string.Format(ConfigKeyFormatUrl, this.ConfigKeyRoot); } }
        private string ConfigKeyAccountName { get { return string.Format(ConfigKeyFormatAccountName, this.ConfigKeyRoot); } }
        private string ConfigKeyAccessKey { get { return string.Format(ConfigKeyFormatAccessKey, this.ConfigKeyRoot); } }
        private string ConfigKeyTableName { get { return string.Format(ConfigKeyFormatTableName, this.ConfigKeyRoot); } }
        private string ConfigKeyPartitionKeyPropertyName { get { return string.Format(ConfigKeyFormatPartitionKeyPropertyName, this.ConfigKeyRoot); } }

        private string Url { get { return this.AppConfig.Get<string>(this.ConfigKeyUrl); } }
        private string AccountName { get { return this.AppConfig.Get<string>(this.ConfigKeyAccountName); } }
        private string AccessKey { get { return this.AppConfig.Get<string>(this.ConfigKeyAccessKey); } }
        private string TableName { get { return this.AppConfig.Get<string>(this.ConfigKeyTableName); } }
        private string PartitionKeyPropertyName { get { return this.AppConfig.Get<string>(this.ConfigKeyPartitionKeyPropertyName); } }

        #endregion

        #region Methods (LoggingProviderBase)

        protected override void LogEntry(LogEntry logEntry)
        {
            this.InitializeIfNeeded();

            var partitionKey = this.partitionKeyProperty.GetValue(logEntry).ToString();
            var logEntryEntity = new LogEntryEntity(logEntry, partitionKey);
            var operation = TableOperation.Insert(logEntryEntity);

            this.table.Execute(operation);
        }

        #endregion

        #region Methods (Private)

        private void InitializeIfNeeded()
        {
            if (this.client == null)
            {
                this.client = new CloudTableClient(new Uri(this.Url),
                    new StorageCredentials(this.AccountName, this.AccessKey));
            }

            if (this.table == null)
            {
                this.table = client.GetTableReference(this.TableName);
                this.table.CreateIfNotExists();
            }

            if (partitionKeyProperty == null)
                this.partitionKeyProperty = typeof (LogEntry).GetProperty(this.PartitionKeyPropertyName);
        }

        #endregion
    }
}