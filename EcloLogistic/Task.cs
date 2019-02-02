using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace EcloLogistic
{
    /// <summary>
    /// 4 different types: NewTray, Fill, Feed, Collect
    /// </summary>
    public class Task
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement(elementName: "Type")]
        [BsonDefaultValue(defaultValue: "")]
        public string Type { get; set; } // Feed or Fill (new lot) or Collect

        [BsonElement(elementName: "FeedBack")]
        [BsonDefaultValue(defaultValue: "")]
        public string FeedBack { get; set; } // Optional message write by the user

        [BsonElement(elementName: "Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        [BsonElement(elementName: "QuantityGiven")]
        [BsonDefaultValue(defaultValue: -1)]
        public double QuantityGiven { get; set; } // Quantity in kg of food given by the user if the type id Feed

        [BsonElement(elementName: "LotId")]
        [BsonDefaultValue(defaultValue: 0)]
        public int LotId { get; set; }

        [BsonElement(elementName: "TrayId")]
        [BsonDefaultValue(defaultValue: "")]
        public string TrayId { get; set; }

        public Task(string type, string feedback, DateTime date, double quantity, int lot_id, string tray_id)
        {
            Type = type;
            FeedBack = feedback;
            Date = date;
            QuantityGiven = quantity;
            LotId = lot_id;
            TrayId = tray_id;
        }


        public Task(string type)
        {
            Type = type;
            FeedBack = "";
            Date = DateTime.MinValue; // default DateTime value
            QuantityGiven = -1;
            LotId = 0; // 0 means that there is no lot assiciated with this task because lot id starts to 1
            TrayId = "";
        }

        public Task(string type, int lot_id)
        {
            Type = type;
            FeedBack = "";
            Date = DateTime.MinValue; // default DateTime value
            QuantityGiven = -1;
            LotId = lot_id;
            TrayId = "";
        }
        public Task(string type, int lot_id, string tray_id)
        {
            Type = type;
            FeedBack = "";
            Date = DateTime.MinValue; // default DateTime value
            QuantityGiven = -1;
            LotId = lot_id;
            TrayId = tray_id;
        }
        public Task(string type, string tray_id)
        {
            Type = type;
            FeedBack = "";
            Date = DateTime.MinValue; // default DateTime value
            QuantityGiven = -1;
            LotId = 0;
            TrayId = tray_id;
        }

        /// <summary>
        /// Return true if the given task has already been realized.
        /// </summary>
        /// <param name="today"></param>
        /// <param name="tasks">All the tasks that apply on the same lot than the given task lot.</param>
        /// <returns></returns>
        public bool IsAlreadyDone(DateTime today, List<Task> tasks)
        {
            bool result = false;
            foreach(Task task in tasks)
            {
                if (task.Type == Type && (today - task.Date).Days == 0)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
