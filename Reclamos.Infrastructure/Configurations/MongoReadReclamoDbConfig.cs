using MongoDB.Driver;
using Reclamos.Domain.Exceptions;

namespace Reclamos.Infrastructure.Configurations
{
    public class MongoReadReclamoDbConfig
    {
        public MongoClient client;
        public IMongoDatabase db;

        public MongoReadReclamoDbConfig()
        {
            try
            {
                string connectionUri = Environment.GetEnvironmentVariable("MONGODB_CNN_READ");

                if (string.IsNullOrWhiteSpace(connectionUri))
                {
                    throw new MongoDBConnectionException();
                }

                var settings = MongoClientSettings.FromConnectionString(connectionUri);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                client = new MongoClient(settings);

                string databaseName = Environment.GetEnvironmentVariable("MONGODB_NAME_READ");
                if (string.IsNullOrWhiteSpace(databaseName))
                {
                    throw new MongoDBConnectionException();
                }

                db = client.GetDatabase(databaseName);
            }
            catch (MongoException ex)
            {
                throw new MongoDBUnnexpectedException();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
