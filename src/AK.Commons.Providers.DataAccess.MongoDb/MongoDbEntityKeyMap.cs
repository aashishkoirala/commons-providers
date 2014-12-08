/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.MongoDb.MongoDbEntityKeyMap
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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#endregion

namespace AK.Commons.Providers.DataAccess.MongoDb
{
    /// <summary>
    /// Entity-key map for MongoDB provider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class MongoDbEntityKeyMap : IEntityKeyMap
    {
        private readonly IDictionary<Type, Expression> keyExpressionByEntityTypeHash = new Dictionary<Type, Expression>();
        private readonly IDictionary<Type, Type> keyTypeByEntityTypeHash = new Dictionary<Type, Type>();

        public void Map<TEntity, TKey>(Expression<Func<TEntity, TKey>> memberExpression)
        {
            this.keyExpressionByEntityTypeHash[typeof (TEntity)] = memberExpression;
            this.keyTypeByEntityTypeHash[typeof (TEntity)] = typeof (TKey);
        }

        public Expression<Func<TEntity, TKey>> GetKeyExpression<TEntity, TKey>()
        {
            return (Expression<Func<TEntity, TKey>>) this.keyExpressionByEntityTypeHash[typeof (TEntity)];
        }

        public Type GetKeyType<TEntity>()
        {
            return this.keyTypeByEntityTypeHash[typeof (TEntity)];
        }
    }
}