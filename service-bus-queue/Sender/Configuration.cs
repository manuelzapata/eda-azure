using System.IO;
using Microsoft.Extensions.Configuration;

namespace Sender
{

    class Configuration {

        private static Configuration instance;

        public string ConnectionString { get; private set; }
        public string QueueName { get; private set; }

        private Configuration () {}

        public static Configuration Instance {

            get {
                //Singleton
                if(instance == null) {

                    var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");

                    var configuration = builder.Build();

                    instance = new Configuration();
                    instance.ConnectionString = configuration["connectionString"];
                    instance.QueueName = configuration["queueName"];
                }
                return instance;
            }


        }

    }


}