/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.MongoDb.MongoDbUnitOfWork
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

using AK.Commons.DataAccess;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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

        public TEntity Get<TEntity, TKey>(TKey key)
        {
            var query = MongoDB.Driver.Builders.Query<TEntity>.EQ(this.entityKeyMap.GetKeyExpression<TEntity, TKey>(), key);
            return this.GetCollection<TEntity>().FindOne(query);
        }

        public IQueryable<TEntity> Query<TEntity>()
        {
            return this.GetCollection<TEntity>().AsQueryable();
        }

        public TResult Query<TEntity, TResult>(Func<IQueryable<TEntity>, TResult> queryBuilder)
        {
            return queryBuilder(this.Query<TEntity>());
        }

        public void Save<TEntity>(TEntity entity)
        {
            this.GetCollection<TEntity>().Save(entity);
        }

        public void Delete<TEntity>(TEntity entity)
        {
            var query = this.GetMongoQueryForDelete(entity);
            this.GetCollection<TEntity>().Remove(query);
        }

        public void Commit() {}

        public void Rollback() {}

        #endregion

        #region Methods (Private)

        private MongoCollection<TEntity> GetCollection<TEntity>()
        {
            var collectionName = typeof (TEntity).Name;

            if (!this.database.CollectionExists(collectionName))
                this.database.CreateCollection(collectionName);

            return this.database.GetCollection<TEntity>(collectionName);
        }

        private IMongoQuery GetMongoQueryForDelete<TEntity>(TEntity entity)
        {
            var keyType = this.entityKeyMap.GetKeyType<TEntity>();
            IMongoQuery query = null;
            if (keyType == typeof(int))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<TEntity, int>();
                var key = idExpression.Compile()(entity);
                query = MongoDB.Driver.Builders.Query<TEntity>.EQ(idExpression, key);
            }
            else if(keyType == typeof(long))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<TEntity, long>();
                var key = idExpression.Compile()(entity);
                query = MongoDB.Driver.Builders.Query<TEntity>.EQ(idExpression, key);                
            }
            else if (keyType == typeof(Guid))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<TEntity, Guid>();
                var key = idExpression.Compile()(entity);
                query = MongoDB.Driver.Builders.Query<TEntity>.EQ(idExpression, key);
            }
            else if (keyType == typeof(string))
            {
                var idExpression = this.entityKeyMap.GetKeyExpression<TEntity, string>();
                var key = idExpression.Compile()(entity);
                query = MongoDB.Driver.Builders.Query<TEntity>.EQ(idExpression, key);
            }

            return query;
        }

        #endregion
    }
}