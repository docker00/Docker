// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Host.Configuration;
using Host.MySqlData.InDataBase;
using IdentityServer4;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Serilog;
using System;
using System.IO;

namespace Host
{
    public class Startup
    {
        public static string ConnectionString = "Server=mysql;Database=identityserver4_release;Uid=root;Pwd=qwe123!@#;Port=3306";
        public IConfigurationRoot Configuration { get; }

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File(@"identityserver4_log.txt");

            if (environment.IsDevelopment())
            {
                serilog.WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}");
            }

            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    { "IdentityServer4", LogLevel.Debug },
                    { "Microsoft", LogLevel.Information },
                    { "System", LogLevel.Error },
                })
                .AddSerilog(serilog.CreateLogger());
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //new MySql.AspNet.Identity.Repositories.UserTestRepository(ConnectionString).InsertTestUsers();

            //System.Collections.Generic.List<MySql.AspNet.Identity.User> users = new System.Collections.Generic.List<MySql.AspNet.Identity.User>();
            //users.AddRange(new MySql.AspNet.Identity.Repositories.UserRepository<MySql.AspNet.Identity.User>(ConnectionString).GetAll());

            bool ServerOld = false;

            bool fromDataBase = false;
            bool fromMemory = true;
            if (!ServerOld)
            {
                fromDataBase = true;
                fromMemory = false;
            }

            IIdentityServerBuilder builder = services.AddIdentityServer(options =>
             {
                 options.Authentication.FederatedSignOutPaths.Add("/signout-callback-aad");
                 options.Authentication.FederatedSignOutPaths.Add("/signout-callback-idsrv");
                 options.Authentication.FederatedSignOutPaths.Add("/signout-callback-adfs");

                 options.Events.RaiseSuccessEvents = true;
                 options.Events.RaiseFailureEvents = true;
                 options.Events.RaiseErrorEvents = true;
             });

            if (!fromMemory)
            {
                builder.AddInDataBaseIdentityResources(ConnectionString);
                builder.AddInDataBaseApiResources(ConnectionString);
                builder.AddInDataBaseClients(ConnectionString);
                builder.AddUsers(ConnectionString);
            }
            else
            {
                builder.AddInMemoryIdentityResources(Host.Configuration.Resources.GetIdentityResources(ConnectionString, fromDataBase));
                builder.AddInMemoryApiResources(Host.Configuration.Resources.GetApiResources(ConnectionString, fromDataBase));
                builder.AddInMemoryClients(Clients.Get(ConnectionString, fromDataBase));
            }

            builder.AddDeveloperSigningCredential();
            builder.AddExtensionGrantValidator<Extensions.ExtensionGrantValidator>();
            builder.AddExtensionGrantValidator<Extensions.NoSubjectExtensionGrantValidator>();
            builder.AddSecretParser<ClientAssertionSecretParser>();
            builder.AddSecretValidator<PrivateKeyJwtSecretValidator>();

            //.AddTestUsers(TestUsers.Users);

            // AppAuth enabled redirect URI validator
            builder.Services.AddTransient<IRedirectUriValidator, StrictRedirectUriValidatorAppAuth>();

            builder.Services.AddMvc();

            builder.Services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = "6LfUZC8UAAAAAGvvJzJvj_Oh7-PdNw20_F5mIPUV",
                SecretKey = "6LfUZC8UAAAAAEys8pOiE0Y50OBd5wZVTvIzeceW",
                ValidationMessage = "Я не робот"
            });

            // only use for development until this bug is fixed
            // https://github.com/aspnet/DependencyInjection/pull/470
            return builder.Services.BuildServiceProvider(validateScopes: true);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            env.EnvironmentName = EnvironmentName.Development;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer();

            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com",
                ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh"
            });

            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //    ClientId = "907633161782-hrt7q1hpk95f9nabfrgccogg9m6uhcp3.apps.googleusercontent.com",
            //    ClientSecret = "Tzwf2tBtJkuftydR69C3tccz"
            //});

            app.UseFacebookAuthentication(new FacebookOptions
            {
                //AppId = "2336144006611795",
                AuthenticationScheme = "Facebook",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "2336144006611795",
                ClientSecret = "645f18ba3d5354beaac5652abad2a960"
            });

            //TODO: 
            //app.UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions
            //{
            //    //AppId = "36394710-8d0b-42da-a9db-d6be20f561b6",
            //    //AuthenticationScheme = "Microsoft",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //    ClientId = "36394710-8d0b-42da-a9db-d6be20f561b6",
            //    ClientSecret = "RY4mySiUjksieFZMdYCzkGf"
            //});
            //
            //app.UseTwitterAuthentication(new TwitterOptions
            //{
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //    ConsumerKey = "36394710-8d0b-42da-a9db-d6be20f561b6",
            //    ConsumerSecret = "10qMC1KyTL1xmMyVNKIAXLfB0bg6kThuKmfHGSGp5E1OQpZBRk"
            //});


            /*
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "demoidsrv",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                SignOutScheme = IdentityServerConstants.SignoutScheme,
                AutomaticChallenge = false,
                DisplayName = "IdentityServer",
                Authority = "https://demo.identityserver.io/",
                ClientId = "implicit",
                ResponseType = "id_token",
                Scope = { "openid profile" },
                SaveTokens = true,
                CallbackPath = new PathString("/signin-idsrv"),
                SignedOutCallbackPath = new PathString("/signout-callback-idsrv"),
                RemoteSignOutPath = new PathString("/signout-idsrv"),
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                }
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "Microsoft",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                SignOutScheme = IdentityServerConstants.SignoutScheme,
                AutomaticChallenge = false,
                DisplayName = "Microsoft",
                Authority = "https://login.windows.net/4ca9cb4c-5e5f-4be9-b700-c532992a3705",
                ClientId = "96e3c53e-01cb-4244-b658-a42164cb67a9",
                ResponseType = "id_token",
                Scope = { "openid profile" },
                CallbackPath = new PathString("/signin-aad"),
                SignedOutCallbackPath = new PathString("/signout-callback-aad"),
                RemoteSignOutPath = new PathString("/signout-aad"),
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                }
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "adfs",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                SignOutScheme = IdentityServerConstants.SignoutScheme,
                AutomaticChallenge = false,
                DisplayName = "ADFS",
                Authority = "https://adfs.leastprivilege.vm/adfs",
                ClientId = "c0ea8d99-f1e7-43b0-a100-7dee3f2e5c3c",
                ResponseType = "id_token",
                Scope = { "openid profile" },
                CallbackPath = new PathString("/signin-adfs"),
                SignedOutCallbackPath = new PathString("/signout-callback-adfs"),
                RemoteSignOutPath = new PathString("/signout-adfs"),
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                }
            });
            */

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public static string GetLocalConf()
        {
            return File.ReadAllText(@"local.conf");
        }

        public static void SaveClientLocalConf(string localConf, string fileText)
        {
            try
            {
                if (File.Exists(localConf))
                {
                    File.Delete(@"/etc/nginx/apps_conf/" + localConf + ".conf");
                }
            }
            catch
            {
                throw new Exception();
            }

            File.WriteAllText(@"/etc/nginx/apps_conf/" + localConf + ".conf", fileText);
        }

        public static void DeleteClientLocalConf(string localConf)
        {
            try
            {
                if (File.Exists(localConf))
                {
                    File.Delete(@"/etc/nginx/apps_conf/" + localConf + ".conf");
                }
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}