using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Tool.hbm2ddl;

using Environment = NHibernate.Cfg.Environment;

namespace SC2LiquipediaStatistics.Utilities.DataBase
{
    public static class NHibernateAndSQLiteConfiguration
    {
        private static ISession currentSession;

        private static ISessionFactory SessionFactory { get; set; }

        private static Configuration Configuration { get; set; }

        private static readonly ConcurrentDictionary<string, Lazy<ISessionFactory>> Factories = new ConcurrentDictionary<string, Lazy<ISessionFactory>>();

        private static Configuration BuildConfig(Assembly[] assembliesContainingMappings, string dbname = ":memory:")
        {
            Configuration = new Configuration()
                .SetProperty(Environment.ReleaseConnections, "on_close")
                .SetProperty(Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName)
                .SetProperty(Environment.ConnectionDriver, typeof(SQLite20Driver).AssemblyQualifiedName)
                .SetProperty(Environment.ConnectionString, "data source=" + dbname + ";Pooling=True;Max Pool Size=10;cache=shared")
                .SetProperty(Environment.CurrentSessionContextClass, "thread_static")
                .CurrentSessionContext<CallSessionContext>()
                .SetProperty(Environment.ProxyFactoryFactoryClass,
                typeof(NHibernate.Bytecode.DefaultProxyFactoryFactory).AssemblyQualifiedName);


            var mapper = new ModelMapper();
            var addedAssemblies = new List<string>();

            foreach (var assembly in assembliesContainingMappings)
            {
                if (addedAssemblies.Contains(assembly.FullName))
                    continue;

                Configuration.AddAssembly(assembly);
                // Note: using the classmappings namespace, same way as in the sessionconfiguration
                var mappingTypes = assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                    (t.BaseType != null && t.BaseType.Namespace == typeof(ClassMapping<>).Namespace));
                mapper.AddMappings(mappingTypes);
                addedAssemblies.Add(assembly.FullName);
            }
            var hbmMappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
            Configuration.AddMapping(hbmMappings);
            return Configuration;
        }

        private static void CreateDataBase()
        {
            using (CurrentSession)
            {
                new SchemaExport(Configuration).Execute(false, true, false, CurrentSession.Connection, null);
                CurrentSession.Flush();
            }
        }

        public static void SetupDatabase(Assembly assemblyContainingMappings, string dbFilePath)
        {
            var shouldCreateDataBase = !File.Exists(dbFilePath);
            var configuration = BuildConfig(new Assembly[] { assemblyContainingMappings }, dbFilePath);

            SessionFactory = configuration.BuildSessionFactory();

            if (shouldCreateDataBase)
                CreateDataBase();
        }

        public static ISession CurrentSession
        {
            get
            {
                if (currentSession == null)
                    currentSession = SessionFactory.OpenSession();

                if (!currentSession.IsOpen)
                {
                    currentSession.Dispose();
                    currentSession = SessionFactory.OpenSession();
                }

                return currentSession;
            }
        }

        public static void DisposeSession()
        {
            if (SessionFactory != null && CurrentSessionContext.HasBind(SessionFactory))
            {
                try
                {
                    var session = SessionFactory.GetCurrentSession();
                    if (session.IsOpen)
                    {
                        session.Dispose();
                    }
                }
                finally
                {
                    CurrentSessionContext.Unbind(SessionFactory);
                }
            }
        }
    }
}
