/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.FluentNHibernate.FluentNHibernateRepository
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
using NHibernate;
using NHibernate.Linq;
using System.Linq;

#endregion

namespace AK.Commons.Providers.DataAccess.FluentNHibernate
{
    /// <summary>
    /// IRepository implementation that encapsulates an NHibernate ISession object.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class FluentNHibernateRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ISession session;

        public FluentNHibernateRepository(ISession session)
        {
            this.session = session;
        }

        public IQueryable<TEntity> Query
        {
            get { return this.session.Query<TEntity>(); }
        }

        public void Save(TEntity entity)
        {
            this.session.SaveOrUpdate(entity);
        }

        public void Delete(TEntity entity)
        {
            this.session.Delete(entity);
        }
    }
}