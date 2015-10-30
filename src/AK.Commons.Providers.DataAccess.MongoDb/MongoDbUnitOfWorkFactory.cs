/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.MongoDb.MongoDbUnitOfWorkFactory
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

using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.DataAccess;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;

#endregion

namespace AK.Commons.Providers.DataAccess.MongoDb
{
    /// <summary>
    /// Unit-of-work factory based on MongoDB.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IUnitOfWorkFactory)), PartCreationPolicy(CreationPolicy.Shared), ProviderMetadata("MongoDb")]
    public class MongoDbUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Constants

        private const string ConfigKeyConnectionString = "connectionstring";
        private const string ConfigKeyDatabase = "database";
        private const string ConfigKeyEntityKeyMapperType = "entitykeymappertype";
        private const string ConfigKeyAppSettingKey = "appsettingkey";
        private const string ConfigKeyEnumAsStringKey = "enumasstring";

        #endregion

        #region Fields

        private MongoClient client;
        private MongoServer server;
        private MongoDatabase database;
        private MongoDbEntityKeyMap entityKeyMap;

        #endregion

        #region Methods (IUnitOfWorkFactory)

        public void Configure(IAppConfig config, string name)
        {
            var configKeyConnectionString = name + "." + ConfigKeyConnectionString;
            var configKeyDatabase = name + "." + ConfigKeyDatabase;
            var configKeyEntityKeyMapperType = name + "." + ConfigKeyEntityKeyMapperType;
            var configKeyAppSettingKey = name + "." + ConfigKeyAppSettingKey;
            var configKeyEnumAsStringKey = name + "." + ConfigKeyEnumAsStringKey;

            var connectionString = config.Get<string>(configKeyConnectionString);
            var databaseName = config.Get<string>(configKeyDatabase);
            var entityKeyMapperTypeName = config.Get<string>(configKeyEntityKeyMapperType);
            var appSettingsKey = config.Get(configKeyAppSettingKey, string.Empty);
            var enumAsString = config.Get(configKeyEnumAsStringKey, false);

            if (!string.IsNullOrWhiteSpace(appSettingsKey))
            {
                connectionString = ConfigurationManager.AppSettings[appSettingsKey];
                var url = new MongoUrl(connectionString);
                databaseName = url.DatabaseName;
            }

            this.client = new MongoClient(connectionString);

#pragma warning disable 612,618
            this.server = this.client.GetServer();
#pragma warning restore 612,618
            
            this.database = this.server.GetDatabase(databaseName);

            var entityKeyMapperType = Type.GetType(entityKeyMapperTypeName);
            Debug.Assert(entityKeyMapperType != null);

            var entityKeyMapper = (IEntityKeyMapper) Activator.CreateInstance(entityKeyMapperType);
            this.entityKeyMap = new MongoDbEntityKeyMap();
            entityKeyMapper.Map(this.entityKeyMap);

            if (!enumAsString) return;

            var conventionPack = new ConventionPack();
            conventionPack.AddMemberMapConvention("EnumAsString", x =>
                {
                    if (x.MemberType.IsEnum) x.SetSerializer(new EnumAsStringBsonSerializer(x.MemberType));
                    if (x.MemberType.IsArray && x.MemberType.GetElementType().IsEnum)
                        x.SetSerializer(new EnumAsStringBsonSerializer(x.MemberType));
                });
            ConventionRegistry.Register("EnumAsString", conventionPack, t => true);
        }

        public IUnitOfWork Create()
        {
            return new MongoDbUnitOfWork(this.database, this.entityKeyMap);
        }

        #endregion

        private class EnumAsStringBsonSerializer : IBsonSerializer
        {
            public EnumAsStringBsonSerializer(Type valueType)
            {
                this.ValueType = valueType;
            }

            public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                var currentBsonType = context.Reader.CurrentBsonType;

                return currentBsonType == BsonType.String || currentBsonType == BsonType.Array
                           ? Enum.Parse(args.NominalType, context.Reader.ReadString())
                           : null;
            }

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            {
                context.Writer.WriteString(value.ToString());
            }

            public Type ValueType { get; private set; }
        }
    }
}