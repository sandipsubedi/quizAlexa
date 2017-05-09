using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alexa.Controllers
{
    public class AlexaUser
    {
        public ObjectId _id { get; set; }
        public string personId { get; set; }
        public string Name { get; set; }
      }

    public class Developer
    {
        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("company_name")]
        public string CompanyName { get; set; }
        [BsonElement("knowledge_base")]
        public List<Knowledge> KnowledgeBase { get; set; }
    }

    public class Knowledge
    {
        [BsonElement("langu_name")]
        public string Language { get; set; }
        [BsonElement("technology")]
        public string Technology { get; set; }
        [BsonElement("rating")]
        public ushort Rating { get; set; }
    }
}