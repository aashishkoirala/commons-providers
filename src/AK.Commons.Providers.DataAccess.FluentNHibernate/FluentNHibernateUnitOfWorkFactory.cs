/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.FluentNHibernate.FluentNHibernateUnitOfWorkFactory
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
using System.ComponentModel.Composition;
using System.Linq;
using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.DataAccess;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using NHibernate;

#endregion

namespace AK.Commons.Providers.DataAccess.FluentNHibernate
{
    /// <summary>
    /// Unit-of-work factory provider based on Fluent NHibernate.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IUnitOfWorkFactory)), PartCreationPolicy(CreationPolicy.Shared),
     ProviderMetadata("FluentNHibernate")]
    public class FluentNHibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private ISessionFactory sessionFactory;

        public void Configure(IAppConfig config, string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetTypes().Any(y => y.GetInterfaces().Contains(typeof(IMappingProvider))))
                .Where(x => !x.FullName.StartsWith("FluentNHibernate"))
                .ToList();
            var nhConfig = new NHibernate.Cfg.Configuration();
            
            BuildConfiguration(config, name, nhConfig);

            this.sessionFactory = Fluently
                .Configure(nhConfig)
                .Mappings(mc => assemblies.Aggregate(mc.FluentMappings, (current, assembly) => current.AddFromAssembly(assembly)))
                .BuildSessionFactory();
        }

        public IUnitOfWork Create()
        {
            return new FluentNHibernateUnitOfWork(this.sessionFactory.OpenSession());
        }

        private static void BuildConfiguration(IAppConfig config, string name, NHibernate.Cfg.Configuration nhConfig)
        {
            AddToConfiguration(config, name, nhConfig, "connection.provider");
            AddToConfiguration(config, name, nhConfig, "connection.driver_class");
            AddToConfiguration(config, name, nhConfig, "connection.connection_string");
            AddToConfiguration(config, name, nhConfig, "show_sql");
            AddToConfiguration(config, name, nhConfig, "generate_statistics");
            AddToConfiguration(config, name, nhConfig, "dialect");
            AddToConfiguration(config, name, nhConfig, "current_session_context_class");
            AddToConfiguration(config, name, nhConfig, "use_outer_join");
            AddToConfiguration(config, name, nhConfig, "adonet.batch_size");
            AddToConfiguration(config, name, nhConfig, "query.substitutions");            
        }

        private static void AddToConfiguration(IAppConfig config, string name, NHibernate.Cfg.Configuration nhConfig, string paramName)
        {
            string value;
            if (!config.TryGet(name + "." + paramName, out value))
                value = null;

            if (value == null) return;

            nhConfig.SetProperty(paramName, value);
        }
    }
}