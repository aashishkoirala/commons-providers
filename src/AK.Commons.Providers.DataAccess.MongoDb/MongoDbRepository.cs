/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.MongoDb.MongoDbRepository
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
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Linq;

#endregion

namespace AK.Commons.Providers.DataAccess.MongoDb
{
    internal class MongoDbRepository<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly MongoDatabase database;
        private readonly MongoDbEntityKeyMap entityKeyMap;

        #endregion

        #region Constructor

        public MongoDbRepository(MongoDatabase database, MongoDbEntityKeyMap entityKeyMap)
        {
            this.database = database;
            this.entityKeyMap = entityKeyMap;
        }

        #endregion

        #region Methods (IRepository)

        public IQueryable<T> Query
        {
            get { return this.GetCollection().AsQueryable(); }
        }

        public void Save(T thing)
        {
            this.GetCollection().Save(thing);
        }

        public void Delete(T thing)
        {
            var query = this.GetMongoQueryForDelete(thing);
            this.GetCollection().Remove(query);
        }

        #endregion

        #region Methods (Private)

        private MongoCollection<T> GetCollection()
        {
            var collectionName = typeof (T).Name;

            if (!this.database.CollectionExists(collectionName))
                this.database.CreateCollection(collectionName);

            return this.database.GetCollection<T>(collectionName);
        }

        private IMongoQuery GetMongoQueryForDelete(T thing)
        {
            var keyType = this.entityKeyMap.GetKeyType<T>();
            IMongoQuery query = null;
            if (keyType == typeof(int))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<T, int>();
                var key = idExpression.Compile()(thing);
                query = MongoDB.Driver.Builders.Query<T>.EQ(idExpression, key);
            }
            else if(keyType == typeof(long))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<T, long>();
                var key = idExpression.Compile()(thing);
                query = MongoDB.Driver.Builders.Query<T>.EQ(idExpression, key);                
            }
            else if (keyType == typeof(Guid))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<T, Guid>();
                var key = idExpression.Compile()(thing);
                query = MongoDB.Driver.Builders.Query<T>.EQ(idExpression, key);
            }
            else if (keyType == typeof(string))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<T, string>();
                var key = idExpression.Compile()(thing);
                query = MongoDB.Driver.Builders.Query<T>.EQ(idExpression, key);
            }

            return query;
        }

        #endregion
    }
}