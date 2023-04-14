﻿using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Movies.Client.ApiServices;
using Movies.Client.HttpHandler;
using System;

namespace Movies.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IMovieApiService, MovieApiService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
             {
                 options.Authority = "https://localhost:5005";

                 options.ClientId = "movies_mvc_client";
                 options.ClientSecret = "secret";
                 options.ResponseType = "code id_token";

                 //options.Scope.Add("openid");
                 //options.Scope.Add("profile");
                 options.Scope.Add("address");
                 options.Scope.Add("email");
                 options.Scope.Add("movieAPI");
                 options.Scope.Add("roles");

                 options.ClaimActions.MapUniqueJsonKey("role", "role");

                 options.SaveTokens = true;
                 options.GetClaimsFromUserInfoEndpoint = true;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     NameClaimType=JwtClaimTypes.GivenName,
                     RoleClaimType= JwtClaimTypes.Role
                 };



             });

            // 1. Create a HttpClient used for accessing MovieAPI
            // 
            services.AddTransient<AuthenticationDelegationHandler>();

            services.AddHttpClient("MovieAPIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5010");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticationDelegationHandler>();

            // 2. Create a HttpClient for Accesing the Identity Server
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5005");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddHttpContextAccessor();

            //services.AddSingleton(new ClientCredentialsTokenRequest
            //{
            //    Address="https://localhost:5005/connect/token",
            //    ClientId="movieClient",
            //    ClientSecret="secret",
            //    Scope="movieAPI"
            //});



            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
