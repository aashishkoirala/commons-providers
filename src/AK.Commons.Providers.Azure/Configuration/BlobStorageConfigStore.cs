/*******************************************************************************************************************************
 * AK.Commons.Providers.Azure.Configuration.BlobStorageConfigStore
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

using AK.Commons.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Text;

#endregion

namespace AK.Commons.Providers.Azure.Configuration
{
    /// <summary>
    /// Configuration store that fetches the configuration XML from Azure Blob storage.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class BlobStorageConfigStore : IConfigStore
    {
        #region Properties

        /// <summary>
        /// Azure BLOB storage URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Azure service account name.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Azure storage primary access key.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Configuration store container name.
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Configuration store BLOB name.
        /// </summary>
        public string BlobName { get; set; }

        /// <summary>
        /// Encoding to use to convert BLOB to string (UTF-8 if none specified).
        /// </summary>
        public Encoding Encoding { get; set; }

        #endregion

        #region Methods (IConfigStore)

        /// <summary>
        /// Gets the configuration XML.
        /// </summary>
        /// <returns>Configuration XML.</returns>
        public string GetConfigurationXml()
        {            
            var client = new CloudBlobClient(new Uri(this.Url), new StorageCredentials(this.AccountName, this.AccessKey));
            var configContainer = client.GetContainerReference(this.ContainerName);
            var configBlob = configContainer.GetBlobReferenceFromServer(this.BlobName);
            
            string configXml;
            using (var stream = new MemoryStream())
            {
                configBlob.DownloadToStream(stream);
                stream.Position = 0;
                configXml = (this.Encoding ?? Encoding.UTF8).GetString(stream.ToArray());
            }
            
            return configXml;
        }

        #endregion
    }
}