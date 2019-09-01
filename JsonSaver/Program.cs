using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileDataCollection.Survey.Models;

namespace JsonSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            var memoryProvider = new MemoryStorageAccessProvider();
            var jsonProvider = new JsonStorageProvider(memoryProvider);
            jsonProvider.Save(new MockQuestionProvider().LoadQuestions());
        }
    }
    class MemoryStorageAccessProvider : IStorageAccessProvider
    {
        public Stream OpenAsset(string path)
        {
            throw new NotImplementedException();
        }

        public Stream OpenFileRead(string path)
        {
            throw new NotImplementedException();
        }

        public Stream OpenFileWrite(string path)
        {
            return File.OpenWrite("1.txt");
        }
    }
}
