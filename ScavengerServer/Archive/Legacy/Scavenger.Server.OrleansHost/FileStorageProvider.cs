using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Serialization;
using Orleans.Storage;

namespace Scavenger.Server.OrleansHost
{
    public class FileStorageProvider : IStorageProvider
    {
        private JsonSerializerSettings _jsonSettings;

        public Logger Log { get; set; }

        public string Name { get; set; }

        public string RootDirectory { get; set; }

        public Task Init(string name,
                  Orleans.Providers.IProviderRuntime providerRuntime,
                  Orleans.Providers.IProviderConfiguration config)
        {
            _jsonSettings = SerializationManager.UpdateSerializerSettings(SerializationManager.GetDefaultJsonSerializerSettings(), config);

            this.Name = name;
            if (string.IsNullOrWhiteSpace(config.Properties["RootDirectory"]))
                throw new ArgumentException("RootDirectory property not set");

            var directory = new System.IO.DirectoryInfo(config.Properties["RootDirectory"]);
            if (!directory.Exists)
                directory.Create();

            this.RootDirectory = directory.FullName;

            return TaskDone.Done;
        }

        public Task Close()
        {
            return TaskDone.Done;
        }

        public async Task ReadStateAsync(string grainType,
                                   GrainReference grainRef,
                                   IGrainState grainState)
        {
            var collectionName = grainState.GetType().Name;
            var key = grainRef.ToKeyString();

            var fName = key + "." + collectionName;
            var path = System.IO.Path.Combine(RootDirectory, fName);

            var fileInfo = new System.IO.FileInfo(path);
            if (!fileInfo.Exists)
                return;

            using (var stream = fileInfo.OpenText())
            {
                var storedData = await stream.ReadToEndAsync();

                grainState.State = JsonConvert.DeserializeObject(storedData, grainState.State.GetType(), _jsonSettings);
            }

        }

        public async Task WriteStateAsync(string grainType,
                                    GrainReference grainRef,
                                    IGrainState grainState)
        {
            var storedData = JsonConvert.SerializeObject(grainState.State, _jsonSettings);

            var collectionName = grainState.GetType().Name;
            var key = grainRef.ToKeyString();

            var fName = key + "." + collectionName;
            var path = System.IO.Path.Combine(RootDirectory, fName);

            var fileInfo = new System.IO.FileInfo(path);

            using (var stream = new System.IO.StreamWriter(
                       fileInfo.Open(System.IO.FileMode.Create,
                                     System.IO.FileAccess.Write)))
            {
                await stream.WriteAsync(storedData);
            }
        }

        public Task ClearStateAsync(string grainType,
                                        GrainReference grainRef,
                                        IGrainState grainState)
        {
            var collectionName = grainState.GetType().Name;
            var key = grainRef.ToKeyString();
            var fName = key + "." + collectionName;
            var path = System.IO.Path.Combine(RootDirectory, fName);

            var fileInfo = new System.IO.FileInfo(path);
            fileInfo.Delete();

            return TaskDone.Done;
        }
    }
}

