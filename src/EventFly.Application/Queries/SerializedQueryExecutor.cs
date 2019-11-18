using EventFly.Definitions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventFly.Queries
{
    public class SerializedQueryExecutor : ISerializedQueryExecutor
    {
        private readonly IApplicationDefinition _applicationDefinition;
        private readonly IQueryProcessor _queryProcessor;

        public SerializedQueryExecutor(IApplicationDefinition applicationDefinition, IQueryProcessor queryProcessor)
        {
            _applicationDefinition = applicationDefinition;
            _queryProcessor = queryProcessor;
        }

        public Task<Object> ExecuteQueryAsync(String name, String json, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof(json));

            var def = _applicationDefinition.Queries.FirstOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (def == null)
                throw new ArgumentException($"No query definition found for query '{name}'");
            IQuery query;

            try
            {
                query = (IQuery)JsonConvert.DeserializeObject(json, def.Type);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to deserialize query '{name}': {ex.Message}", ex);
            }
            return _queryProcessor.Process(query);
        }

    }
}