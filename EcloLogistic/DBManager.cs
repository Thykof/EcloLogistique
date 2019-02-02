using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;


using MongoDB.Driver;
using MongoDB.Bson;


namespace EcloLogistic
{
    /// <summary>
    /// Manager of all operations relative to the database.
    /// </summary>
    class DBManager
    {
        MongoClient m_client;
        IMongoDatabase m_db;
        IMongoCollection<Tray> m_tray_collection;
        IMongoCollection<Lot> m_lot_collection;
        IMongoCollection<Task> m_task_collection;
        Process m_mongod;
        bool m_connect = false;
        const string m_process_filename = @"\mongod.exe";

        /// <summary>
        /// Default constructor: do nothing.
        /// </summary>
        public DBManager() {}


        // Properties:
        /// <summary>
        /// True is the app is connected to the database.
        /// </summary>
        public bool IsConnected
        {
            get { return m_connect; }
        }

        public string ProcessPath { get; set; }
        public string ProcessFileName { get { return m_process_filename; } }


        /// <summary>
        /// Start the server.
        /// Start the process: mongod.exe
        /// </summary>
        /// <param name="hide_console">true will hide the console of the process.</param>
        /// <returns>A result string.</returns>
        public string StartServer(bool hide_console=true)
        {
            hide_console = false; // DEVELOP
            string response = "";
            if (!m_connect)
            {
                string file_name = ProcessPath + ProcessFileName;
                //starting the mongod server (when app starts)
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = ProcessPath + ProcessFileName
                };
                
                if (!File.Exists(file_name))
                {
                    m_connect = false;
                    return "Error: file not found.";
                }

                    if (hide_console)
                {
                    start.CreateNoWindow = true;  // No console window
                    start.UseShellExecute = false;  // No console window
                }

                // Get c:/Users/Name/AppData/Roaming:
                string user_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                string dbPath = user_path + @"\EcloLogistic\ecloDB";
                string dbArg = @"--dbpath " + dbPath;
                
                if (!Directory.Exists(dbPath))
                {
                    Directory.CreateDirectory(dbPath);
                    response =  "Directory " + dbPath + " created.";
                }
                
                start.Arguments = dbArg;

                m_mongod = Process.Start(start);
                
                return response;
            }
            else
                return "";
        }

        /// <summary>
        /// Connect to the localhost database in port 27017.
        /// Gets the eclo database and the collections.
        /// </summary>
        public void Connect()
        {
            // Call by StartServer
            m_client = new MongoClient(
                 new MongoClientSettings
                 {
                     Server = new MongoServerAddress("localhost", 27017),
                     // Giving 3 seconds for a MongoDB server to be up before we throw:
                     ServerSelectionTimeout = TimeSpan.FromSeconds(3)
                 }
            );

            //Uncomment these two lines to connect to remote server:
            //string connectionStrinUncomment this line to connect to remote server.g = "mongodb://EcloLogistic:EcloLogistic@clustereclo-shard-00-00-oslrt.mongodb.net:27017,clustereclo-shard-00-01-oslrt.mongodb.net:27017,clustereclo-shard-00-02-oslrt.mongodb.net:27017/test?ssl=true&replicaSet=ClusterEclo-shard-0&authSource=admin&retryWrites=true";
            //m_client = new MongoClient(connectionString);

            m_db = m_client.GetDatabase("eclo"); // create or get
            m_tray_collection = m_db.GetCollection<Tray>("Tray");
            m_lot_collection = m_db.GetCollection<Lot>("Lot");
            m_task_collection = m_db.GetCollection<Task>("Task");

            m_connect = true;
        }

        /// <summary>
        /// Gets a collection from the <c>m_db</c> database.
        /// </summary>
        /// <param name="name">The collestion's name.</param>
        /// <remarks>Not use yet.</remarks>
        /// <returns>
        /// <c>IMongoCollection</c>: the collection.
        /// </returns>
        private IMongoCollection<BsonDocument> GetCollection(string name)
        // Return the collection by given name
        {
            IMongoCollection<BsonDocument> collection = m_db.GetCollection<BsonDocument>(name);
            return collection;
        }

        /// <summary>
        /// Send shutdown command to server.
        /// </summary>
        /// <remarks>
        /// This should close the process.
        /// Shutdown is an admin command
        /// </remarks>
        /// <returns>
        /// A string error response or an empty string if successful.
        /// </returns>
        public string StopServer()
        {
            string response = "";
            if (m_connect)
            {
                response = RunAdminCommand("{shutdown: 1}"); // don't do this on remote server! :)

                // Stopping the mongod server (when app is closing)
                //m_mongod.Kill(); // Disable because shutdown command should close the process
                m_connect = false;
            }
            return response;
        }

        /// <summary>
        /// Ping the server.
        /// </summary>
        /// <returns>Bool: whether the server responds or not.</returns>
        public bool PingServer()
        {
            bool response = true;
            try
            {
                RunAdminCommand("{ping: 1}");
            }
            catch (System.TimeoutException)
            {
                response = false;
            }
            return response;
        }

        /// <summary>
        /// Send an admin command to the admin database.
        /// Gets the admin database and call <c>RunCommand</c>.
        /// </summary>
        /// <param name="cmd">The command</param>
        /// <returns>The repsonse string.</returns>
        public string RunAdminCommand(string cmd)
        {
            var adminDatabase = m_client.GetDatabase("admin"); // Gets the admin database
            string response;
            try
            {
                response = adminDatabase.RunCommand<BsonDocument>(cmd).ToString();
            }
            catch (MongoConnectionException e)
            {
                response = e.ToString();
            }
            catch(MongoCommandException e)
            {
                response = e.ToString();
            }
            return response;
        }


        // Tray
        /// <summary>
        /// Insert a <c>Tray</c> in the tray collection.
        /// </summary>
        /// <param name="tray">The tray object</param>
        public void AddTray(Tray tray)
        {
            m_tray_collection.InsertOne(tray);
        }

        /// <summary>
        /// Gets all the trays in the tray collection.
        /// </summary>
        /// <returns>A list of trays.</returns>
        public List<Tray> GetTrays()
        {
            IAsyncCursor<Tray> documents = m_tray_collection.FindSync(filter=>true);
            List<Tray> trays = new List<Tray> { };
            trays = documents.ToList();
            return trays;
        }

        /// <summary>
        /// Delete a tray by the Id.
        /// </summary>
        /// <param name="tray_id">The id of the deleted tray.</param>
        public void DeleteTrayById(string tray_id)
        {
            FilterDefinition<Tray> filter = Builders<Tray>.Filter.Eq("Id", tray_id);
            m_tray_collection.DeleteOne(filter);
        }

        // Lot
        /// <summary>
        /// Insert a <c>Lot</c> in the lot collection.        
        /// </summary>
        /// <param name="lot">The lot object</param>
        public void AddLot(Lot lot)
        {
            m_lot_collection.InsertOne(lot);
        }

        /// <summary>
        /// Gets all the lots in the lot collection.
        /// </summary>
        /// <returns>A List of lots.</returns>
        public List<Lot> GetLots()
        {
            IAsyncCursor<Lot> documents = m_lot_collection.FindSync(filter => true);
            List<Lot> lots = new List<Lot> { };
            lots = documents.ToList();
            return lots;
        }

        /// <summary>
        /// Gets a lot by its id.
        /// </summary>
        /// <param name="lot_id">The lot id.</param>
        /// <returns></returns>
        public Lot GetLotById(int lot_id)
        {
            FilterDefinition<Lot> filter = Builders<Lot>.Filter.Eq("Id", lot_id);
            IAsyncCursor<Lot> documents = m_lot_collection.FindSync(filter);
            List<Lot> list_lot = documents.ToList();

            if (list_lot.Count == 0)
                return new Lot(); // This constructor set Id to 0, which is impossible -> default value -> not found
            else
                return list_lot[0];
        }
        /// <summary>
        /// Calculate a new lot Id.
        /// Id lot number start to 1 and increase for each lot.
        /// When creating a new lot, its id is the lasted created + 1
        /// </summary>
        /// <returns>int: the id of the next lot.</returns>
        public int GetNewLotId()
        
        {
            int id = 0;
            var lots = m_lot_collection.AsQueryable();
            foreach (Lot lot in lots)
            {
                if(lot.Id > id)
                {
                    id = lot.Id;
                }
            }
            return id +1;
            // TODO: optimize with mongodb query
        }

        /// <summary>
        /// Search a unfinished lot that use and specific tray.
        /// </summary>
        /// <param name="tray_id">The tray's Id.</param>
        /// <returns>True is the tray is free.</returns>
        public bool CanCreateNewLot(string tray_id)
        {
            FilterDefinition<Lot> filter = Builders<Lot>.Filter.Eq("TrayId", tray_id);
            filter = filter & Builders<Lot>.Filter.Eq("ProductivityUnit", -1); // Harvested lot
            IAsyncCursor<Lot> result = m_lot_collection.FindSync(filter);
            List<Lot> r = result.ToList();
            return (r.Count == 0) ? true: false;
        }

        /// <summary>
        /// Delete a lot by its Id.
        /// </summary>
        /// <param name="lot_id">The lot's Id.</param>
        public void DeleteLotById(string lot_id)
        {
            FilterDefinition<Lot> filter = Builders<Lot>.Filter.Eq("Id", lot_id);
            m_lot_collection.DeleteOne(filter);
        }

        /// <summary>
        /// Update a lot.
        /// </summary>
        /// <param name="lot">The new lot.</param>
        public void UpdateLot(Lot lot)
        {
            FilterDefinition<Lot> filter = Builders<Lot>.Filter.Eq("Id", lot.Id);
            m_lot_collection.ReplaceOne(filter, lot);
        }

        // Task
        /// <summary>
        /// Insert a <c>Task</c> in the task collection.
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(Task task)
        {
            m_task_collection.InsertOne(task);
        }

        /// <summary>
        /// Gets all the tasks in the task collection.
        /// </summary>
        /// <returns>Task list.</returns>
        public List<Task> GetTasks()
        {
            return m_task_collection.AsQueryable().ToList();  // Another way to find all elements, maybe here this is not a good idea :/
        }

        /// <summary>
        /// Return all the tasks relative to the given lot id.
        /// </summary>
        /// <remarks>
        /// Not used.
        /// </remarks>
        /// <param name="lot_id"></param>
        /// <returns></returns>
        public List<Task> GetTasksByLotId(int lot_id)
        {
            FilterDefinition<Task> filter = Builders<Task>.Filter.Eq("LotId", lot_id);
            IAsyncCursor<Task> result = m_task_collection.FindSync(filter);
            return result.ToList();
        }

        /// <summary>
        /// Delete a <c>Task</c> in the task collection.
        /// </summary>
        /// <remarks>Without the task id, find a unique task by tray id, type and date.</remarks>
        /// <param name="tray_id">The tray id.</param>
        /// <param name="type">The type.</param>
        /// <param name="date">The date.</param>
        public void DeleteTask(string tray_id, string type, DateTime date)
        {
            FilterDefinition<Task> filter = Builders<Task>.Filter.Eq("TrayId", tray_id);
            filter = filter & Builders<Task>.Filter.Eq("Type", type);
            //filter = filter & Builders<Task>.Filter.Eq("Date", date);
            ObjectId task_id = new ObjectId();
            foreach(Task task in m_task_collection.FindSync<Task>(filter).ToList())
            {
                if(task.Date == date)
                {
                    task_id = task.Id;
                }
            }
            if(task_id != null)
            {
                m_task_collection.DeleteOne(filter);
            }
        }
    }
}
