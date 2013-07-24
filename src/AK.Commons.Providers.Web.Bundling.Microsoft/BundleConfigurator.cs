/*******************************************************************************************************************************
 * AK.Commons.Providers.Web.Bundling.Microsoft.BundleConfigurator
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

using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Optimization;
using System.Web.Script.Serialization;
using AK.Commons.Composition;
using AK.Commons.Web.Bundling;

#endregion

namespace AK.Commons.Providers.Web.Bundling.Microsoft
{
    /// <summary>
    /// Implementation of IBundleConfigurator based on Microsoft's bundling library.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IBundleConfigurator)), PartCreationPolicy(CreationPolicy.Shared),
     ProviderMetadata("Microsoft")]
    public class BundleConfigurator : IBundleConfigurator
    {
        public void Configure(string bundleConfigurationJson)
        {
            var configuration = new JavaScriptSerializer().Deserialize<BundleConfiguration>(bundleConfigurationJson);
            BundleTable.EnableOptimizations = true;
            BundleTable.Bundles.Clear();

            configuration.BundleItems
                .Select(CreateBundleFromConfigItem)
                .Where(bundle => bundle != null)
                .ForEach(bundle =>
                {
                    if (!configuration.Minify) RemoveMinifiersFromBundle(bundle);

                    BundleTable.Bundles.Add(bundle);
                });
        }

        private static Bundle CreateBundleFromConfigItem(BundleItem bundleItem)
        {
            Bundle bundle = null;
            switch (bundleItem.Type)
            {
                case BundleItemType.JavaScript:
                    bundle = new ScriptBundle(bundleItem.Path);
                    break;
                case BundleItemType.Css:
                    bundle = new StyleBundle(bundleItem.Path);
                    break;
            }

            if (bundle == null) return null;

            bundle.Include(bundleItem.IncludedFiles);

            return bundle;
        }

        private static void RemoveMinifiersFromBundle(Bundle bundle)
        {
            IBundleTransform transform;
            do
            {
                transform = bundle.Transforms
                    .FirstOrDefault(p => (p is CssMinify) || (p is JsMinify));

                if (transform != null)
                    bundle.Transforms.Remove(transform);

            } while (transform != null);
        }
    }
}