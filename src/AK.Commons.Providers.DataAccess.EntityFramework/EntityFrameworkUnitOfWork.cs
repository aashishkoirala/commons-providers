/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.EntityFramework.EntityFrameworkUnitOfWork
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

using System;
using System.Data.Entity;
using System.Linq;
using AK.Commons.DataAccess;

#endregion

namespace AK.Commons.Providers.DataAccess.EntityFramework
{
    /// <summary>
    /// IUnitOfWork implementation that encapsulates EntityFramework context.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        public EntityFrameworkUnitOfWork(string nameOrConnectionString)
        {
            this.context = new DbContext(nameOrConnectionString);
            this.IsValid = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EntityFrameworkUnitOfWork()
        {
            this.Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.context.Dispose();
            this.IsValid = false;
        }

        public bool IsValid { get; private set; }

        public TEntity Get<TEntity, TKey>(TKey key)
        {
            return default(TEntity);
        }

        public IQueryable<TEntity> Query<TEntity>()
        {
            return (IQueryable<TEntity>) this.context.Set(typeof (TEntity));
        }

        public TResult Query<TEntity, TResult>(Func<IQueryable<TEntity>, TResult> queryBuilder)
        {
            return queryBuilder(this.Query<TEntity>());
        }

        public void Save<TEntity>(TEntity entity)
        {
            var a = this.context.Set(null);
            a.Find()
        }

        public void Delete<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            this.context.SaveChanges();
            this.IsValid = false;
        }

        public void Rollback()
        {
            this.context.Dispose();
            this.IsValid = false;
        }
    }
}