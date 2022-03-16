using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MedRoom.ServerFramework.WebConfigHelpers.XSRF
{
    public static class XSRFExtension
    {
        #region Constants

        private const string COOKIE_NAME = "XSRF-TOKEN-INTERNAL";
        private const string COOKIE_NAME_CLIENT = "XSRF-TOKEN";
        private const string HEADER_NAME = "X-XSRF-TOKEN";

        #endregion

        #region Public Methods

        public static void ConfigureAntiforgery(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = COOKIE_NAME;
                options.HeaderName = HEADER_NAME;
                options.SuppressXFrameOptionsHeader = false;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Path = "/";
            });
        }

        public static void SetCookieResponse(this IAntiforgery antiforgery, HttpContext httpContext, bool httpsEnabled)
        {
            AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(httpContext);

            if (!string.IsNullOrWhiteSpace(tokens.CookieToken))
            {
                httpContext.Response.Cookies.Append(COOKIE_NAME, tokens.CookieToken,
                    new CookieOptions()
                    {
                        Path = "/",
                        Secure = httpsEnabled,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict
                    });
            }

            if (!string.IsNullOrWhiteSpace(tokens.RequestToken))
            {
                httpContext.Response.Cookies.Append(COOKIE_NAME_CLIENT, tokens.RequestToken,
                    new CookieOptions()
                    {
                        Path = "/",
                        Secure = httpsEnabled,
                        HttpOnly = false,
                        SameSite = SameSiteMode.Strict
                    });
            }
        }

        #endregion
    }
}
