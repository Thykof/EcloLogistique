using System;


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcloLogistic
{
    class Tray
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement(elementName: "Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
        

        // Constructors:
        public Tray(string id) // used by Form1.new_tray
        {
            Id = id;
            Date = DateTime.Now;
        }
        public Tray(string id, DateTime date)
        {
            Id = id;
            Date = date;
        }

        // BSON document way:
        /*public Tray(BsonDocument document)
        {
            Id = document.GetValue("Id").ToString();
            State = document.GetValue("State").ToString();
            Date = document.GetValue("Date"); // need convert into DateTime
        }
        public Tray()
        {
            Id = "0";
            State = "disable";
        }

        // Methods:
        public BsonDocument ToBson()
        {
            BsonDocument document = new BsonDocument
            {
                { "Id", Id },
                { "State", State },
            };

            return document;
        }*/
        //Not used. Instead: mapped classe with [BsonElement] syntax.
    }
}
