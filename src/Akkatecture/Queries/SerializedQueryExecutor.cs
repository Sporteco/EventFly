using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Definitions;
using Newtonsoft.Json;

namespace Akkatecture.Queries
{
    public class SerializedQueryExecutor : ISerializedQueryExecutor
    {
        private readonly IApplicationDefinition _applicationDefinition;

        public SerializedQueryExecutor(ActorSystem system)
        {
            _applicationDefinition = system.GetApplicationDefinition();
        }

        public Task<object> ExecuteQueryAsync(string name, string json, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof (name));
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof (json));

            var def = _applicationDefinition.Queries.FirstOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (def == null)
                throw new ArgumentException($"No query definition found for query '{name}'");
            IQuery query;

            try
            {
                query = (IQuery) JsonConvert.DeserializeObject(json, def.Type);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to deserialize query '{name}': {ex.Message}", ex);
            }
            return _applicationDefinition.QueryAsync(query);
        }

    }
}