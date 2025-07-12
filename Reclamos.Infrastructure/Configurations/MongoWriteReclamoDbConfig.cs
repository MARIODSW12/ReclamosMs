using MongoDB.Driver;
using Reclamos.Domain.Exceptions;

namespace Reclamos.Infrastructure.Configurations
{
    public class MongoWriteReclamoDbConfig
    {
        public MongoClient client;
        public IMongoDatabase db;

        public MongoWriteReclamoDbConfig()
        {
            try
            {
                string connectionUri = Environment.GetEnvironmentVariable("MONGODB_CNN_WRITE");

                if (string.IsNullOrWhiteSpace(connectionUri))
                {
                    throw new MongoDBConnectionException();
                }

                var settings = MongoClientSettings.FromConnectionString(connectionUri);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                client = new MongoClient(settings);

                string databaseName = Environment.GetEnvironmentVariable("MONGODB_NAME_WRITE");
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
