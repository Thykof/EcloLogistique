using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcloLogistic
{
    public class Lot
    {
        // Properties:
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// Quantity of food to give at each nutrition.
        /// </summary>
        [BsonElement(elementName: "Food")]
        [BsonDefaultValue(defaultValue: 0)]
        public double FoodQuantity { get; set; }

        /// <summary>
        /// Number of clutch at the begining.
        /// </summary>
        /// <remarks>Enter the number of clutchs put in the tray.</remarks>
        [BsonElement(elementName: "Clutch")]
        [BsonDefaultValue(defaultValue: 0)]
        public int Clutch { get; set; }

        /// <summary>
        /// Number of insects at the end
        /// </summary>
        [BsonElement(elementName: "ProductivityUnit")]
        [BsonDefaultValue(defaultValue: -1)]
        public int ProductivityUnit { get; set; }

        /// <summary>
        /// total weight.
        /// </summary>
        [BsonElement(elementName: "ProductivityWeight")]
        [BsonDefaultValue(defaultValue: -1)]
        public double ProductivityWeight { get; set; }

        /// <summary>
        /// Number of day between two nutritions.
        /// </summary>
        /// <remarks>
        /// Enter the number of day between two nutritions.
        /// </remarks>
        [BsonElement(elementName: "FeedFrequency")]
        [BsonDefaultValue(defaultValue: -1)]
        public int FeedFrequency { get; set; }

        /// <summary>
        /// Insect rearing in day.
        /// </summary>
        /// <remarks>
        /// Number of rearing days before collecting
        /// </remarks>
        [BsonElement(elementName: "BreedingTime")]
        [BsonDefaultValue(defaultValue: 0)]
        public int BreedingTime { get; set; }


        [BsonElement(elementName: "BeginingDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BeginingDate { get; set; }

        [BsonElement(elementName: "CollectDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CollectDate { get; set; }

        [BsonElement(elementName: "TrayId")]
        [BsonDefaultValue(defaultValue: "")]
        public string TrayId { get; set; }

        // Constructors:
        /// <summary>
        /// Constructor with all attributes
        /// id: lot id
        /// state: egg, larvae, nymphs, adult
        /// food: food quantity
        /// clutch: number of clutch
        /// pu: productivity unit
        /// pw: prodictivity weight
        /// ff: feed frequency
        /// bd: begining date
        /// cd: collect date
        /// </summary>
        public Lot(int id, string state, double food, int clutch, int pu, double pw, int ff, int bt, DateTime bd, DateTime cd, string tray_id)
        {
            Id = id;
            FoodQuantity = food;
            Clutch = clutch;
            ProductivityUnit = pu;
            ProductivityWeight = pw;
            FeedFrequency = ff;
            BreedingTime = bt;
            BeginingDate = bd;
            CollectDate = cd;
            TrayId = tray_id;
        }

        /// <summary>
        /// Constructor with no attributes
        /// </summary>
        /// <remarks>Use when DBManager.GetLotById didn't match.</remarks>
        [BsonConstructor]
        public Lot()
        
        {
            Id = 0;
            FoodQuantity = 0;
            Clutch = 0;
            ProductivityUnit = -1;
            ProductivityWeight = -1;
            FeedFrequency = -1;
            BreedingTime = 0;
            DateTime now = DateTime.Now;
            BeginingDate = now;
            CollectDate = now; // will be updated when collecting the lot
            TrayId = "0";
        }

        /// <summary>
        /// Constructor with tray id attribute
        /// </summary>
        /// <remarks>Use to create Fill task.</remarks>
        public Lot(string tray_id)
        {
            Id = 0;
            FoodQuantity = 0;
            Clutch = 0;
            ProductivityUnit = -1;
            ProductivityWeight = -1;
            FeedFrequency = -1;
            BreedingTime = 0;
            DateTime now = DateTime.Now;
            BeginingDate = now;
            CollectDate = now; // will be updated when collecting the lot
            TrayId = tray_id;
        }

        /// <summary>
        /// Constructor use after new lot dialog window
        /// The only two specific value are id, food quantity, clutch, feed frequency, breeding time, tray id
        /// all other properties are set to default.
        /// </summary>
        public Lot(int id, double food, int clutch, int ff, int bt, string tray_id)
        
        {
            Id = id;
            FoodQuantity = food;
            Clutch = clutch;
            ProductivityUnit = -1;
            ProductivityWeight = -1;
            FeedFrequency = ff;
            BreedingTime = bt;
            DateTime now = DateTime.Now;
            BeginingDate = now;
            CollectDate = now; // will be updated when collecting the lot
            TrayId = tray_id;
        }

        // Methods
        /// <summary>
        /// Return the number of days to wait before doing the Feed task.
        /// </summary>
        /// <param name="today"></param>
        /// <returns>int</returns>
        public int WhenFeed(DateTime today)
        {
            // Default value of FeedFrequency
            if (FeedFrequency == -1)
                return -1;


            TimeSpan feed_frequency = new TimeSpan(FeedFrequency, 0, 0, 0, 0);
            TimeSpan age = today - BeginingDate;
            TimeSpan when;
            int n_when;
            if (age > feed_frequency)
            {
                if (FeedFrequency == 0) // DEVELOP ONLY: FeedFrequency will never be 0
                    return 0;
                int n = age.Days / FeedFrequency;
                int remainder = age.Days % FeedFrequency;
                TimeSpan FFxn = new TimeSpan(FeedFrequency * (n+1), 0, 0, 0);
                DateTime B = BeginingDate + FFxn;
                when = B - today;
                n_when = when.Days;
            }
            else
            {
                when = feed_frequency - age;
                n_when = when.Days;
            }

            return n_when;
        }

        /// <summary>
        /// Calculate the number of day we must wait to collect the lot
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public int WhenCollect(DateTime today)
        {
            TimeSpan age = today - BeginingDate;
            DateTime collect_date = BeginingDate + new TimeSpan(BreedingTime, 0, 0, 0);
            TimeSpan when = collect_date - today;
            return when.Days;
        }
    }
}
