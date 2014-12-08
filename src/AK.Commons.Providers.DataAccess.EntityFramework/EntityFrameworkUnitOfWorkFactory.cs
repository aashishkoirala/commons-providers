/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.EntityFramework.EntityFrameworkUnitOfWorkFactory
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
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

using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.DataAccess;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data.EntityClient;

#endregion

namespace AK.Commons.Providers.DataAccess.EntityFramework
{
    /// <summary>
    /// Unit-of-work factory provider based on Entity Framework.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IUnitOfWorkFactory)), PartCreationPolicy(CreationPolicy.Shared),
     ProviderMetadata("EntityFramework")]
    public class EntityFrameworkUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private const string EntityConnectionStringConfigKey = "EntityConnectionString";
        private const string EntityConnectionStringNameConfigKey = "EntityConnectionStringName";
        private const string MetadataConfigKey = "Metadata";
        private const string ProviderConfigKey = "Provider";
        private const string ProviderConnectionStringConfigKey = "ProviderConnectionString";
        private const string ProviderConnectionStringNameConfigKey = "ProviderConnectionStringName";

        private string entityConnectionStringName;
        private string entityConnectionString;

        public void Configure(IAppConfig config, string name)
        {
            this.entityConnectionStringName = null;
            this.entityConnectionString = null;

            this.entityConnectionStringName = GetConfigValue(config, name, EntityConnectionStringNameConfigKey);
            if (entityConnectionStringName != null) return;

            this.entityConnectionString = GetConfigValue(config, name, EntityConnectionStringConfigKey);
            if (this.entityConnectionString != null) return;

            var metadata = GetConfigValue(config, name, MetadataConfigKey);
            var provider = GetConfigValue(config, name, ProviderConfigKey);

            var providerConnectionStringName = GetConfigValue(config, name, ProviderConnectionStringNameConfigKey);
            var providerConnectionString =
                providerConnectionStringName != null
                    ? ConfigurationManager.ConnectionStrings[providerConnectionStringName].ConnectionString
                    : GetConfigValue(config, name, ProviderConnectionStringConfigKey);

            var builder = new EntityConnectionStringBuilder
            {
                Metadata = metadata,
                Provider = provider,
                ProviderConnectionString = providerConnectionString
            };

            this.entityConnectionString = builder.ToString();
        }

        public IUnitOfWork Create()
        {
            throw new NotImplementedException();
        }

        private static string GetConfigValue(IAppConfig config, string name, string key)
        {
            string value;
            return config.TryGet(key + "." + name, out value) ? null : value;
        }
    }
}

/*
                Metadata =
                    "res://BareCoveEntities.csdl|res://BareCoveEntities.ssdl|res://BareCoveEntities.msl",
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = "server=(local);database=barecove;integrated security=true;"
*/