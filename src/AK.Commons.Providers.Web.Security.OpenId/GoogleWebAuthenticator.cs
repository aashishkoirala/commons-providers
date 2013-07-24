/*******************************************************************************************************************************
 * AK.Commons.Providers.Web.Security.OpenId.GoogleWebAuthenticator
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
using System.Web.Mvc;
using System.Web.Security;
using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

#endregion

namespace AK.Commons.Providers.Web.Security.OpenId
{
    /// <summary>
    /// Implementation of IWebAuthenticator that uses DotNetOpenAuth to perform OpenId based SSO.
    /// This particular implementation uses Google as the identity provider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IWebAuthenticator)), PartCreationPolicy(CreationPolicy.Shared), ProviderMetadata("Google")]
    public class GoogleWebAuthenticator : IWebAuthenticator
    {
        #region Constants/Fields/Properties

        private const string GoogleIdRequestUrlConfigKey = "ak.commons.providers.web.security.openid.googleRequestUrl";
        private const string DefaultGoogleIdRequestUrl = "https://www.google.com/accounts/o8/id";

        private static readonly OpenIdRelyingParty OpenIdRp = new OpenIdRelyingParty();

        [Import] private Lazy<IAppConfig> appConfig;

        private IAppConfig AppConfig { get { return this.appConfig.Value; } }

        private string GoogleIdRequestUrl
        {
            get { return this.AppConfig.Get(GoogleIdRequestUrlConfigKey, DefaultGoogleIdRequestUrl); }
        }

        #endregion

        #region Methods (Public - IWebAuthenticator)

        public void Authenticate(AuthorizationContext authorizationContext, 
            Action<WebAuthenticationResult> authenticationCallback, Action alreadyLoggedInCallback)
        {
            if (authorizationContext.HttpContext.User.Identity.IsAuthenticated)
            {
                alreadyLoggedInCallback();
                return;
            }

            var response = OpenIdRp.GetResponse();
            if (response == null)
            {
                try
                {
                    var authRequest = OpenIdRp.CreateRequest(this.GoogleIdRequestUrl);
                    var fetchRequest = new FetchRequest();
                    fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                    authRequest.AddExtension(fetchRequest);
                    authorizationContext.Result = authRequest.RedirectingResponse.AsActionResult();
                    return;
                }
                catch (Exception ex)
                {
                    authenticationCallback(new WebAuthenticationResult
                    {
                        ResultType = WebAuthenticationResultType.Error,
                        Exception = ex,
                        ErrorMessage = ex.Message
                    });
                    return;
                }
            }

            var result = new WebAuthenticationResult();
            if (response.Status != AuthenticationStatus.Authenticated)
            {
                result.ResultType = WebAuthenticationResultType.Denied;
                result.Exception = response.Exception;
                result.ErrorMessage = response.Exception == null ? string.Empty : response.Exception.Message;
            }
            else
            {
                result.ResultType = WebAuthenticationResultType.Success;

                var fetchResponse = response.GetExtension<FetchResponse>();
                if (fetchResponse != null)
                    result.UserName = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);

                FormsAuthentication.SetAuthCookie(response.ClaimedIdentifier.ToString(), true);
            }
            result.UserAttributes["Response"] = response;
            authenticationCallback(result);
        }

        #endregion
    }
}