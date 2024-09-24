using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.SelfHost.Services;
using Topshelf;
using ServiceStack.Authentication.MongoDb;
using Core.Contracts;
using Core.Repositories;
using MongoDB.Driver;
using static System.Net.WebRequestMethods;

namespace ServiceStack.SelfHost
{
    public class ServiceStackSelfHost
    {
        private Program.AppHost _selfHostRef;
        public void Start(string primaryUrl)
        {
            //Register ServiceStack license.
            Licensing.RegisterLicense(@"3228-e1JlZjozMjI4LE5hbWU6QXV0b21zb2Z0LFR5cGU6QnVzaW5lc3MsSGFzaDpvd0xCbE1PMG41dkl4MWxGTUN1TFQwbnFpb2E2UVJLZkd2ZHpaOXNYQUxGMjJjRW4wWW1NdFBUbFU4T0ZZQlVmVW9kQ1dudHo5T3JsVGl2eGxJQ0pDL0ViU1FvMXdNcTRpcWc1SlZnRU5tMUcreXF5c29LdDE3clo0S21RSlJlUGhubk1rY2svVXNzVi92RVNYT2M1d0hVRkRMUUxHRGd4UHExRVVUMW5OeTg9LEV4cGlyeToyMDE2LTEyLTEwfQ==");

            //Create instance.
            _selfHostRef = new Program.AppHost(primaryUrl);
            _selfHostRef.Init();

            _selfHostRef.Start();

            Console.WriteLine("REST Server now listening at: {0}", primaryUrl);
        }

        public void Stop()
        {
            _selfHostRef.Stop();
            Console.WriteLine("REST Server stopped.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var primaryUrl = "http://*:5001";

            // see if security is enabled
            if (ConfigurationManager.AppSettings["PrimaryUrlBase"] != null)
                primaryUrl = ConfigurationManager.AppSettings["PrimaryUrlBase"];
            
            HostFactory.Run(x =>
            {
                x.Service<ServiceStackSelfHost>(s =>
                {
                    s.ConstructUsing(name => new ServiceStackSelfHost());
                    s.WhenStarted(tc => tc.Start(primaryUrl));
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("DocumentHQ REST API Server");
                x.SetDisplayName("DocumentHQ REST API Server");
                x.SetServiceName("DHQRESTServer");
            });
        }

        //Define the Web Services AppHost
        public class AppHost : AppHostHttpListenerBase
        {
            public AppHost(string primaryUrl)
                : base("HttpListener Self-Host", typeof(DocumentHQService).Assembly)
            {
                _primaryUrl = primaryUrl;

                // ensure we have a trailing /
                if (!_primaryUrl.EndsWith("/"))
                    _primaryUrl += "/";
            }
            
            protected string _primaryUrl = "";

            public void Start()
            {
                Listener = new HttpListener();

                Start(_primaryUrl);
            }
            public override void Configure(Funq.Container container)
            {
                //register any dependencies your services use, e.g:
                //container.Register<ICacheClient>(new MemoryCacheClient());
                //Error handling rules
                SetConfig(new HostConfig
                {
                    MapExceptionToStatusCode = {
                        { typeof(KeyNotFoundException), (int)HttpStatusCode.NotFound }
                    }
                });
                //Plugins
                //Logging factory, so we can log using commands such as log.Debug("Debug Event Log Entry."); anywhere in the server.
                //LogManager.LogFactory = new Log4NetFactory(true);

                //Request logging, so we can access SERVER/requestlogs for debugging.
                Plugins.Add(new RequestLogsFeature { RequiredRoles = new string[] { } });
                //TODO: Look into how we will whitelist this call from all hosts if required.
                Plugins.Add(new CorsFeature(
                                allowCredentials: true,
                                allowedHeaders: "Content-Type, Authorization",
                                allowOriginWhitelist: new[] 
                                {
                                    "http://localhost:5000",
                                    "http://localhost:5001",
                                    "http://*:5001",
                                    "http://localhost:49361",
                                    "http://localhost:5002",
                                    "http://*:5002",
                                    "https://localhost:5003",
                                    "http://63.32.159.120:5001",
                                    "http://63.32.159.120:5002",
                                    "https://63.32.159.120:5003",
                                    "http://ec2-63-32-159-120.eu-west-1.compute.amazonaws.com:5002",
                                    "https://ec2-63-32-159-120.eu-west-1.compute.amazonaws.com:5003"
                                }));

                Plugins.Add(new ServerEventsFeature());

                container.Register<ICacheClient>(new MemoryCacheClient());
                // support authentication
                Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                            new IAuthProvider[]
                            {
                                new CredentialsAuthProvider(), //user/pass auth
                                // Remove this for now, just go for basic authentication
                              //  new CustomCredentialsAuthProvider(),
                                //new ApiKeyAuthProvider(), //Auth using an api key instead of a password
                                new JwtAuthProvider(AppSettings) {
                                    AuthKeyBase64 = "84BEC2C2CAB432D7A09789AADAA08ADE",
                                   // AuthKey = AesUtils.CreateKey(), //JWT with a new key each time the server starts
                                    RequireSecureConnection = false //Don't require HTTPS, until a cert is provided
                                }
                                
                            }
                           ));
                var userAuthRepos = new MongoDbAuthRepository(GetMongoDatabase(), true);
                container.Register<IUserAuthRepository>(userAuthRepos);
                
                if (userAuthRepos.GetUserAuthByUserName("admin") == null)
                {
                    (userAuthRepos as IAuthRepository).CreateUserAuth(new UserAuth { UserName = "admin", Id = 1, Roles = { "SuperAdmin" } }, "admin");
                }

                Plugins.Add(new RegistrationFeature());
                container.RegisterAutoWiredAs<MemoryChatHistory, IChatHistory>();
            }

            private IMongoDatabase GetMongoDatabase()
            {
                MongoClient _client = new MongoClient("mongodb://localhost:27019/?safe=true");
                return _client.GetDatabase("DocumentHQ");

            }
        }
    }

}

public class CustomCredentialsAuthProvider : CredentialsAuthProvider
{
    public override bool IsAuthorized(IAuthSession session, IAuthTokens tokens, Authenticate request = null)
    {
        return base.IsAuthorized(session, tokens, request);
    }
    public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
    {
        // see if security is enabled
        if (ConfigurationManager.AppSettings["DisableSecurity"] != null)
            return true;

        string strDomain = "";
        // has the domain name been included as part of the username
        if (userName.Contains("\\"))
        {
            // yes, split them apart
            strDomain = userName.Substring(0, userName.IndexOf('\\'));
            userName = userName.Substring(userName.IndexOf('\\') + 1);
        }
        else
        {
            // no, get the current domain name
            try
            {
                strDomain = Domain.GetCurrentDomain().Name;
            }
            catch (ActiveDirectoryOperationException ex)
            {
                // We may not be in a domain here, let this go. We authenticate against the machine now.
            }
        }

        // OOR TODO: Check for local login in Mongo, if that's not valid check AD.
        // Possibly do the AD check if the name contains a "\".

        bool isValid;
        if (!string.IsNullOrEmpty(strDomain))
        {
            using (var pc = new PrincipalContext(ContextType.Domain, strDomain))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(userName, password);
            }

        }
        else
        {
            using (var pc = new PrincipalContext(ContextType.Machine, System.Environment.MachineName))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(userName, password);
            }
        }
        return isValid;
    }
}

