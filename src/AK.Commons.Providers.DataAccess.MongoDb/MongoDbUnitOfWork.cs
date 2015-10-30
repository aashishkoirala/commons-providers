/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.MongoDb.MongoDbUnitOfWork
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

using AK.Commons.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Linq;

#endregion

namespace AK.Commons.Providers.DataAccess.MongoDb
{
    /// <summary>
    /// IUnitOfWork based on MongoDB.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class MongoDbUnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly MongoDatabase database;
        private readonly MongoDbEntityKeyMap entityKeyMap;

        #endregion

        #region Constructor

        public MongoDbUnitOfWork(MongoDatabase database, MongoDbEntityKeyMap entityKeyMap)
        {
            this.database = database;
            this.entityKeyMap = entityKeyMap;
            this.IsValid = true;
        }

        #endregion

        #region Properties/Methods (IUnitOfWork)

        public bool IsValid { get; private set; }

        public void Dispose()
        {
            this.IsValid = false;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            return new MongoDbRepository<T>(this.database, this.entityKeyMap);
        }

        public T NextValueInSequence<T>(string sequenceContainerName, string sequenceName, T incrementBy)
        {
            var incrementByAsLong = (long) Convert.ChangeType(incrementBy, typeof (long));

            if (!this.database.CollectionExists(sequenceContainerName))
                this.database.CreateCollection(sequenceContainerName);

            var collection = this.database.GetCollection(sequenceContainerName);

#pragma warning disable 612,618

            var result = collection.FindAndModify(
                Query.EQ("Name", new BsonString(sequenceName)),
                SortBy.Null,
                Update.Inc("Value", incrementByAsLong),
                false, true);

#pragma warning restore 612,618

            long value = 0;
            var item = result.Response.Values.First();
            if (item.IsBsonNull) return (T) Convert.ChangeType(value, typeof (T));

            value = item["Value"].ToInt64();

            return (T) Convert.ChangeType(value, typeof (T));
        }

        public void Commit()
        {
            this.IsValid = false;
        }

        #endregion        
    }
}