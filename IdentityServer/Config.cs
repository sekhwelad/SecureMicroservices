using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId="movieClient",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes={ "movieAPI" }
                },
                new Client
                {
                    ClientId = "movies_mvc_client",
                    ClientName = "Movies MVC Web App",
                    AllowedGrantTypes=GrantTypes.Code,
                    AllowRememberConsent=false,
                    RedirectUris=new List<string>()
                    {
                        "https://localhost:5002/signin-oidc" // this is client app port
                    },
                    PostLogoutRedirectUris=new List<string>()
                    {
                        "https://localhost:5002/signout-callback-oidc"
                    },
                    ClientSecrets=new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes=new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                }

            };

        public static IEnumerable<ApiScope> ApiScopes =>
           new ApiScope[]
           {
               new ApiScope("movieAPI","Movies API")
           };

        public static IEnumerable<ApiResource> ApiResources =>
         new ApiResource[]
         {

         };

        public static IEnumerable<IdentityResource> IdentityResources =>
          new IdentityResource[]
          {
              new IdentityResources.OpenId(),
              new IdentityResources.Profile()
          };

        public static List<TestUser> TestUsers =>
         new List<TestUser>
         {
              new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "delight",
                    Password = "delight",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "delight"),
                        new Claim(JwtClaimTypes.FamilyName, "sekhwela")
                    }
                }
         };


    }
}
