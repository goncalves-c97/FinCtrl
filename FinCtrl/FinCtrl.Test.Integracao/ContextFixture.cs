using FinCtrlApi.Databases.MongoDb.FinCtrl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinCtrl.Test.Integracao
{
    public class ContextFixture
    {
        public FinCtrlAppDbContext Context { get; }

        public ContextFixture() 
        {
            string path = Directory.GetCurrentDirectory();
            path = path[..path.IndexOf("\\bin")];

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(path) // Set the base path for the configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string? mongoDbConnectionString = configuration.GetConnectionString("MongoDbString");

            if (string.IsNullOrEmpty(mongoDbConnectionString))
                throw new Exception("Não foi possível obter a string de conexão ao MongoDb");

            var options = new DbContextOptionsBuilder<FinCtrlAppDbContext>()
                .UseMongoDB(mongoDbConnectionString, "FinCtrl")
                .Options;

            Context = new FinCtrlAppDbContext(options);
        }
    }
}
