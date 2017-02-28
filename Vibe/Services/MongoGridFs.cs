using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Vibe.Services
{
    public class MongoGridFs
    {
        private readonly MongoDatabase _db;
        private readonly MongoGridFS _gridFs;

        public MongoGridFs(MongoDatabase db)
        {
            _db = db;
            _gridFs = _db.GridFS;
        }

        public ObjectId AddFile(Stream fileStream, string fileName)
        {
            var fileInfo = _gridFs.Upload(fileStream, fileName);
            return (ObjectId)fileInfo.Id;
        }

        public Stream GetFile(ObjectId id)
        {
            var file = _gridFs.FindOneById(id);
            return file.OpenRead();
        }
    }
}