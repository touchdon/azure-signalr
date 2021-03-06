﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Azure.SignalR;
using Microsoft.Azure.SignalR.AspNet;
using Microsoft.Azure.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Owin.Infrastructure;

namespace Owin
{
    public static partial class OwinExtensions
    {
        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <returns>The app builder</returns>
        /// <remarks>
        /// The connection string is read from the ConnectionString section of application config (web.config or app.config), with name "Azure:SignalR:ConnectionString"
        /// </remarks>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string applicationName)
        {
            return builder.MapAzureSignalR(applicationName, new HubConfiguration());
        }

        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="connectionString">The connection string of an Azure SignalR Service instance.</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string applicationName, string connectionString)
        {
            return builder.MapAzureSignalR(applicationName, options => options.ConnectionString = connectionString);
        }

        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="optionsConfigure">A callback to configure the <see cref="ServiceOptions"/>.</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string applicationName, Action<ServiceOptions> optionsConfigure)
        {
            return builder.MapAzureSignalR(applicationName, new HubConfiguration(), optionsConfigure);
        }

        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="configuration">The hub configuration <see cref="HubConfiguration"/>.</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string applicationName, HubConfiguration configuration)
        {
            return builder.MapAzureSignalR("/signalr", applicationName, configuration);
        }

        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="configuration">The hub configuration <see cref="HubConfiguration"/>.</param>
        /// <param name="optionsConfigure">A callback to configure the <see cref="ServiceOptions"/>.</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string applicationName, HubConfiguration configuration, Action<ServiceOptions> optionsConfigure)
        {
            return builder.MapAzureSignalR("/signalr", applicationName, configuration, optionsConfigure);
        }

        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at the specified path.
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path to map signalr hubs.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="configuration">The hub configuration <see cref="HubConfiguration"/>.</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string path, string applicationName, HubConfiguration configuration)
        {
            return builder.Map(path, subApp => subApp.RunAzureSignalR(applicationName, configuration));
        }

        /// <summary>
        /// Maps Azure SignalR hubs to the app builder pipeline at the specified path.
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path to map signalr hubs.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="configuration">The hub configuration <see cref="HubConfiguration"/>.</param>
        /// <param name="optionsConfigure">A callback to configure the <see cref="ServiceOptions"/>.</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder MapAzureSignalR(this IAppBuilder builder, string path, string applicationName, HubConfiguration configuration, Action<ServiceOptions> optionsConfigure)
        {
            return builder.Map(path, subApp => subApp.RunAzureSignalR(applicationName, configuration, optionsConfigure));
        }

        /// <summary>
        /// Adds Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <remarks>
        /// The connection string is read from the ConnectionString section of application config (web.config or app.config), with name "Azure:SignalR:ConnectionString"
        /// </remarks>
        public static void RunAzureSignalR(this IAppBuilder builder, string applicationName)
        {
            builder.RunAzureSignalR(applicationName, new HubConfiguration());
        }

        /// <summary>
        /// Adds Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="connectionString">The connection string of an Azure SignalR Service instance.</param>
        public static void RunAzureSignalR(this IAppBuilder builder, string applicationName, string connectionString)
        {
            RunAzureSignalR(builder, applicationName, connectionString, new HubConfiguration());
        }

        /// <summary>
        /// Adds Azure SignalR hubs to the app builder pipeline at "/signalr" using the connection string specified in web.config 
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="configuration">The hub configuration <see cref="HubConfiguration"/>.</param>
        public static void RunAzureSignalR(this IAppBuilder builder, string applicationName, HubConfiguration configuration)
        {
            RunAzureSignalR(builder, applicationName, configuration, null);
        }

        /// <summary>
        /// Adds Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder <see cref="IAppBuilder"/>.</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive.</param>
        /// <param name="connectionString">The connection string of an Azure SignalR Service instance.</param>
        /// <param name="configuration">The hub configuration <see cref="HubConfiguration"/>.</param>
        public static void RunAzureSignalR(this IAppBuilder builder, string applicationName, string connectionString, HubConfiguration configuration)
        {
            RunAzureSignalR(builder, applicationName, configuration, s => s.ConnectionString = connectionString);
        }


        /// <summary>
        /// Adds Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive</param>
        /// <param name="optionsConfigure">A callback to configure the <see cref="ServiceOptions"/>.</param>
        public static void RunAzureSignalR(this IAppBuilder builder, string applicationName, Action<ServiceOptions> optionsConfigure)
        {
            RunAzureSignalR(builder, applicationName, new HubConfiguration(), optionsConfigure);
        }

        /// <summary>
        /// Adds Azure SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="applicationName">The name of your app, it is case-insensitive</param>
        /// <param name="configuration">The hub configuration</param>
        /// <param name="optionsConfigure">A callback to configure the <see cref="ServiceOptions"/>.</param>
        public static void RunAzureSignalR(this IAppBuilder builder, string applicationName, HubConfiguration configuration, Action<ServiceOptions> optionsConfigure)
        {
            var serviceOptions = new ServiceOptions();
            optionsConfigure?.Invoke(serviceOptions);
            RunAzureSignalRCore(builder, applicationName, configuration, serviceOptions);
        }

        private static void RunAzureSignalRCore(IAppBuilder builder, string applicationName, HubConfiguration configuration, ServiceOptions options)
        {
            // applicationName is case insensitive, it will be lower cased in the service side
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentException(nameof(applicationName), "Empty application name is not allowed.");
            }

            if (configuration == null)
            {
                // Keep the same as SignalR's exception
                throw new ArgumentException("A configuration object must be specified.");
            }

            var resolver = configuration.Resolver ?? throw new ArgumentException("A dependency resolver must be specified.");

            // Ensure we have the conversions for MS.Owin so that
            // the app builder respects the OwinMiddleware base class
            SignatureConversions.AddConversions(builder);

            // ServiceEndpointManager needs the logger
            var loggerFactory = new LoggerFactory();

            var hubs = GetAvailableHubNames(configuration);

            var endpoint = new ServiceEndpointManager(options, loggerFactory);
            configuration.Resolver.Register(typeof(IServiceEndpointManager), () => endpoint);

            // Get the one from DI or new a default one
            var router = configuration.Resolver.Resolve<IEndpointRouter>() ?? new DefaultEndpointRouter();

            builder.Use<NegotiateMiddleware>(configuration, applicationName, endpoint, router, options, loggerFactory);

            builder.RunSignalR(configuration);

            // Fetch the trace manager from DI and add logger provider
            var traceManager = configuration.Resolver.Resolve<ITraceManager>();
            if (traceManager != null)
            {
                loggerFactory.AddProvider(new TraceManagerLoggerProvider(traceManager));
            }

            var dispatcher = PrepareAndGetDispatcher(configuration, options, endpoint, router, applicationName, hubs, loggerFactory);
            if (dispatcher != null)
            {
                // Start the server->service connection asynchronously 
                _ = dispatcher.StartAsync();
            }
        }

        private static ServiceHubDispatcher PrepareAndGetDispatcher(HubConfiguration configuration, ServiceOptions options, IServiceEndpointManager endpoint, IEndpointRouter router, string applicationName, IReadOnlyList<string> hubs, ILoggerFactory loggerFactory)
        {
            // TODO: Using IOptions looks wierd, thinking of a way removing it
            // share the same object all through
            var serviceOptions = Options.Create(options);

            // For safety, ALWAYS register abstract classes or interfaces
            // Some third-party DI frameworks such as Ninject, implicit self-binding concrete types:
            // https://github.com/ninject/ninject/wiki/dependency-injection-with-ninject#skipping-the-type-binding-bit--implicit-self-binding-of-concrete-types
            configuration.Resolver.Register(typeof(IOptions<ServiceOptions>), () => serviceOptions);

            var serviceProtocol = new ServiceProtocol();
            configuration.Resolver.Register(typeof(IServiceProtocol), () => serviceProtocol);

            var scm = new ServiceConnectionManager(applicationName, hubs);
            configuration.Resolver.Register(typeof(Microsoft.Azure.SignalR.AspNet.IServiceConnectionManager), () => scm);

            var ccm = new ClientConnectionManager(configuration);
            configuration.Resolver.Register(typeof(IClientConnectionManager), () => ccm);

            var atm = new AzureTransportManager(configuration.Resolver);
            configuration.Resolver.Register(typeof(ITransportManager), () => atm);

            var parser = new SignalRMessageParser(hubs, configuration.Resolver);
            configuration.Resolver.Register(typeof(IMessageParser), () => parser);

            var smb = new ServiceMessageBus(configuration.Resolver);
            configuration.Resolver.Register(typeof(IMessageBus), () => smb);

            if (hubs?.Count > 0)
            {
                return new ServiceHubDispatcher(hubs, serviceProtocol, scm, ccm, endpoint, router, serviceOptions, loggerFactory);
            }
            else
            {
                loggerFactory.CreateLogger<IAppBuilder>().Log(LogLevel.Warning, "No hubs found.");
                return null;
            }
        }

        private static IReadOnlyList<string> GetAvailableHubNames(HubConfiguration configuration)
        {
            var hubManager = configuration.Resolver.Resolve<IHubManager>();
            return hubManager?.GetHubs().Select(s => s.Name).ToList();
        }
    }
}
