/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.FluentNHibernate.FluentNHibernateUnitOfWork
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
using System;

#endregion

namespace AK.Commons.Providers.DataAccess.FluentNHibernate
{
    /// <summary>
    /// IUnitOfWork implementation that encapsulates NHibernate ISession and ITransaction objects.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class FluentNHibernateUnitOfWork : IUnitOfWork
    {
        private readonly ISession session;
        private readonly ITransaction transaction;

        public FluentNHibernateUnitOfWork(ISession session)
        {
            this.session = session;
            this.transaction = this.session.BeginTransaction();
            this.IsValid = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FluentNHibernateUnitOfWork()
        {
            this.Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.transaction.Dispose();
            this.session.Dispose();
            this.IsValid = false;
        }

        public bool IsValid { get; private set; }

        public IRepository<T> Repository<T>() where T : class
        {
            return new FluentNHibernateRepository<T>(this.session);
        }

        public T NextValueInSequence<T>(string sequenceContainerName, string sequenceName, T incrementBy)
        {
            throw new NotSupportedException("This operation is not yet supported by this provider.");
        }

        public void Commit()
        {
            this.transaction.Commit();
            this.IsValid = false;
        }
    }
}