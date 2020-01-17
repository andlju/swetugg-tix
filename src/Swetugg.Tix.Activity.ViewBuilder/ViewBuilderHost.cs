using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using NEventStore;
using NEventStore.Persistence;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ViewBuilderHost
    {
        private IPersistStreams _store;
        private readonly string _connectionString;
        private IDictionary<Type, IList<Func<object, Task>>> _eventHandlers = new Dictionary<Type, IList<Func<object, Task>>>();

        public ViewBuilderHost(IPersistStreams store, string connectionString)
        {
            _store = store;
            _connectionString = connectionString;

            using (var conn = new SqlConnection(_connectionString))
            {
                var hasCheckpoint = conn.ExecuteScalar<int>("SELECT COUNT(Id) FROM Checkpoints WHERE Name=@name", new {name = "ViewBuilder"});
                if (hasCheckpoint <= 0)
                {
                    conn.Execute("INSERT INTO Checkpoints (Name, LastCheckpoint) VALUES(@Name,0)", new { name = "ViewBuilder" });
                }
            }
        }

        public void RegisterHandler<TEvent>(IHandleEvent<TEvent> eventHandler)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handlerList))
            {
                _eventHandlers.Add(typeof(TEvent), handlerList = new List<Func<object, Task>>());
            };
            handlerList.Add(evt => eventHandler.Handle((TEvent)evt));
        }

        public static ViewBuilderHost Build(Wireup eventStoreWireup, ILoggerFactory loggerFactory, string viewsConnectionString)
        {
            var eventStore =
                eventStoreWireup
                    .Build();

            return new ViewBuilderHost(eventStore.Advanced, viewsConnectionString);
        }

        public async Task HandleCommits()
        {
            long checkpoint = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                checkpoint = await conn.ExecuteScalarAsync<long>("SELECT LastCheckpoint FROM Checkpoints WHERE Name=@name", new {name = "ViewBuilder"});
            }

            var commits = _store.GetFrom(checkpoint);
            foreach (var commit in commits)
            {
                checkpoint = commit.CheckpointToken;
                foreach (var evt in commit.Events)
                {
                    if (!_eventHandlers.TryGetValue(evt.Body.GetType(), out var handlers))
                        continue;
                    
                    foreach (var handler in handlers)
                    {
                        await handler(evt.Body);
                    }
                }
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                var result = await conn.ExecuteAsync("UPDATE Checkpoints SET LastCheckpoint = @checkpoint WHERE Name=@name", new { name = "ViewBuilder", checkpoint });
            }
        }
    }
}