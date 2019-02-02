using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;


namespace EcloLogistic
{
    public partial class MainForm : Form
    {
        DBManager db_manager = new DBManager();

        MenuItem[] tray_menu1;
        MenuItem[] tray_menu2;
        MenuItem[] lot_menu;
        MenuItem[] task_menu;
    
        /// <summary>
        /// Constructor
        /// Initialize Tray, Tasl, Lot, TODO ListViews
        /// Add form closing event handler
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            initialize_listViewTODO();
            InitializeListViewTray();
            InitializeListViewTask();
            InitializeListViewLot();

            listViewLot.FullRowSelect = true;
            listViewTask.FullRowSelect = true;

            FormClosing += new FormClosingEventHandler(OnFormClosing);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            db_manager.ProcessPath = Helpers.GetServerLocation();
            bool response;

            // If the file proccess doesn't exist: ask for a path
            while (!File.Exists(db_manager.ProcessPath + db_manager.ProcessFileName))
            {
                response = ChangeServerLocation(); // Show FolderBrowserDialog
                if (!response)
                {
                    // The user want to cancel.
                    Environment.Exit(0); // Exit the app.
                }
            }

            StartServer(); // StartServer(false): will show the console server (if not hidden)
            db_manager.Connect();
            // Wait for the server to start and gets ready before UpdateTODO()
            // (Avoid sending command to the database before its is ready)
            bool server_ok = false;
            int i = 0;
            while (!server_ok && i <= 10) // Leave the loop after 30 secondes (timeout)
            {
                System.Threading.Thread.Sleep(300);
                server_ok = db_manager.PingServer();
                i++;
            }
            // TODO: handle error

            // DEVELOP: TODO: update in async mode
            UpdateTODO();
        }

        // Methods
        /// <summary>
        /// Open the folder chooser window
        /// </summary>
        /// <returns>
        /// Returns true if a folder path if choosen, false else.
        /// </returns>
        private bool ChangeServerLocation()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Please select MongoDB server process file location:",
                ShowNewFolderButton = false
            };
            DialogResult = fbd.ShowDialog(this);
            fbd.Dispose();
            if (DialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string new_path = fbd.SelectedPath;
                db_manager.ProcessPath = new_path;
                Helpers.SaveServerLocation(new_path);
                StatusText = "Change server location: done";
                return true;
            }
            else
            {
                StatusText = "Change server location: aborted";
                return false;
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="hide_console"></param>
        public void StartServer(bool hide_console = true)
        {
            string message = db_manager.StartServer(hide_console);
            if (message != "" && !message.Contains("Error"))
            {
                MessageBox.Show(message, "Database server connection ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (message.Contains("Error"))
            {
                MessageBox.Show(message, "Database server connection ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DBConnected = "Database: not connected";
                StatusText = "Start server: failed";
                return;
            }
            DBConnected = "Database: connected";
            StatusText = "Start server: done";
        }


        /// <summary>
        /// Calculate all the tasks, check database.
        /// Clear the todo listview.
        /// Gets all the trays in the database and and it each tray in the tray list view.
        /// Get all the lots in the database and and it each tray in the lot list view.
        /// Get all the realized tasks in the database and and it each task in the lot list view.
        /// For each lot, calculate if it needs to be collected. If it does, add Collect task in todo listview; 
        /// if it doesn't than add the feed task in the todo listview.
        /// </summary>
        public void UpdateTODO()
        {
            if (!db_manager.IsConnected)
            {
                StatusText = "Update: aborted";
                return;
            }
            listViewTray.Items.Clear();
            listViewLot.Items.Clear();
            listViewTask.Items.Clear();
            listViewTODO.Items.Clear();

            // Gets all the items from the database
            List<Tray> trays = db_manager.GetTrays(); // Get trays from db
            List<Task> all_tasks = db_manager.GetTasks(); // Get all tasks from db
            List<Lot> lots = db_manager.GetLots(); // Get all lots from db

            // Display trays in listview
            bool tray_is_used;
            foreach (Tray tray in trays)
            {
                tray_is_used = false;
                // Create a fill task if the tray is empty:
                foreach(Lot lot in lots)
                {
                    if(lot.ProductivityUnit == -1 && lot.TrayId == tray.Id)
                    {
                        tray_is_used = true;
                    }
                }
                string state;
                if (!tray_is_used)
                {
                    AddTODOListView(new Task("Fill"), new Lot(tray.Id), -1);
                    state = "Empty";
                }
                else
                {
                    state = "Active";
                }

                AddTrayListView(tray, state); // Display the tray
            }

            // Display finished tasks in ListViewTask
            foreach (Task task in all_tasks)
            {
                AddTaskListView(task);
            }

            // Display lots in listview
            foreach (Lot lot in lots)
            {
                AddLotListView(lot);
                
                if (lot.BeginingDate != lot.CollectDate)
                    continue; // The lot is harvested, no need to calculate neither feed task nor collect task.

                // Add tasks in listview
                Task feed = new Task("Feed", lot.Id, lot.TrayId);
                int when_feed = lot.WhenFeed(DateTime.Today);

                Task collect = new Task("Collect", lot.Id, lot.TrayId);
                int when_collect = lot.WhenCollect(DateTime.Today);
                AddTODOListView(collect, lot, when_collect);

                // Get task related to this lot
                List<Task> tasks = new List<Task> { };
                foreach (Task task in all_tasks)
                {
                    if (task.LotId == lot.Id)
                        tasks.Add(task);
                }
                // No need to add feed task if the lot must be collected before feed, or if the feed is already done.
                //if (when_collect > when_feed && !feed.IsAlreadyDone(DateTime.Today, db_manager.GetTasksByLotId(lot.Id)))
                if (when_collect > when_feed && !feed.IsAlreadyDone(DateTime.Today, tasks))
                    AddTODOListView(feed, lot, when_feed);
            }
            StatusText = "Update: done";
        } // END UpdateTODO
        


        // Tray
        private void AddTrayListView(Tray tray, string state="Active")
        {
            ListViewItem item_new = new ListViewItem(new string[] { tray.Id, state, tray.Date.ToString() });
            listViewTray.Items.Add(item_new);
        }

        
        private void UpdateTrayState(string tray_id, string state)
        {
            foreach (ListViewItem item in listViewTray.Items)
            {
                if (item.SubItems[0].Text == tray_id)
                {
                    item.SubItems[1].Text = state;
                    break;
                }
            }
        }


        // Lot
        /// <summary>
        /// Add a lot in the listview
        /// </summary>
        /// <remarks>
        /// The lot can be harvested or not.
        /// </remarks>
        /// <param name="lot"></param>
        private void AddLotListView(Lot lot)
        {
            ListViewItem item_new = listViewLot.Items.Add(lot.Id.ToString());
            item_new.SubItems.Add("Larvae"); // TODO: state = f(beginning date)
            TimeSpan age = DateTime.Now - lot.BeginingDate;
            item_new.SubItems.Add(age.ToString("dd"));
            item_new.SubItems.Add(lot.TrayId.ToString());
            item_new.SubItems.Add(lot.FeedFrequency.ToString());
            item_new.SubItems.Add(lot.FoodQuantity.ToString());
            item_new.SubItems.Add(lot.Clutch.ToString());
            item_new.SubItems.Add(lot.BeginingDate.ToString());
            item_new.SubItems.Add(lot.BreedingTime.ToString());
            string collect_date = (lot.BeginingDate == lot.CollectDate)? "" : lot.CollectDate.ToString();
            item_new.SubItems.Add(collect_date);
            string p_unit = (lot.ProductivityUnit==-1) ? "": lot.ProductivityUnit.ToString();
            item_new.SubItems.Add(p_unit);
            string p_weight = (lot.ProductivityWeight == -1) ? "" : lot.ProductivityWeight.ToString();
            item_new.SubItems.Add(p_weight);
            double total_food = 0; // TODO: get total given food with all Feed Task of this lot.
            item_new.SubItems.Add(total_food.ToString());
        }


        // TODOTask
        /// <summary>
        /// Add a task to the TODOListView
        /// </summary>
        /// <remarks>
        /// If the task's type is Fill, the given lot is build with the signle
        /// argument constructor: <c>public Lot(string tray_id)</c>
        /// </remarks>
        /// <param name="task">The task to display</param>
        /// <param name="lot">The lot associed to the task</param>
        /// <param name="n_when">Number of day to wait before doing the task.</param>
        private void AddTODOListView(Task task, Lot lot, int n_when)
        {
            bool can = true;
            foreach (ListViewItem item in listViewTODO.Items)
            {
                if (lot.TrayId == item.SubItems[1].Text && task.Type == "Fill")
                {
                    can = false;
                    break; // It's useless to check other 'if'
                }
                if (item.SubItems[3].Text == "") // Fill tasks hasn't defined lot id
                {
                    continue;
                }
                if (lot.Id == Int32.Parse(item.SubItems[3].Text) && task.Type == "Feed" && item.SubItems[0].Text == "Feed")
                {
                    can = false;
                    break; // It's useless to check other 'if'
                }
                if (task.LotId == Int32.Parse(item.SubItems[3].Text) && task.Type == item.SubItems[0].Text)
                {
                    can = false;
                }
            }
            if (can)
            {
                // Calculate When
                string when;
                switch (n_when)
                {
                    case -1: // Fill task
                        when = "Whenever";
                        break;
                    case 0: // Execute the task today
                        when = "Today";
                        break;
                    default:
                        when = n_when.ToString();
                        break;
                }
                
                // Display item
                ListViewItem item_new = listViewTODO.Items.Add(task.Type); // Button text: Fill, Feed or Collect
                item_new.SubItems.Add(lot.TrayId);
                item_new.SubItems.Add(when); // When
                string lot_id = (lot.Id == 0) ? "": lot.Id.ToString(); // id the lot id is 0 -> default value
                item_new.SubItems.Add(lot_id);
            }
            listViewTODO.Sort();
        }

        // Tasks (finished)
        private void AddTaskListView(Task task)
        {
            bool can = true;
            // This might be useless:
            //foreach (ListViewItem item in listViewTask.Items)
            //{
            //    if (Int32.Parse(item.SubItems[4].Text) == task.LotId && item.SubItems[0].Text == task.Type &&
            //        DateTime.Today - task.Date == DateTime.Today - DateTime.Parse(item.SubItems[3].Text))
            //        can = false;
            //}
            if (can)
            {
                ListViewItem item = listViewTask.Items.Add(task.Type);
                item.SubItems.Add(task.FeedBack);
                // Display given food quantity only if the task is Feed:
                item.SubItems.Add((task.Type != "Feed") ? "" : task.QuantityGiven.ToString());
                item.SubItems.Add(task.Date.ToString());
                item.SubItems.Add(task.LotId.ToString());
                item.SubItems.Add(task.TrayId);
                item.SubItems.Add(task.Id.ToString());
            }
        }

        // Action (task)
        /// <summary>
        /// New tray task.
        /// </summary>
        private void NewTray()
        {
            FormNewTray form_new_tray = new FormNewTray();
            if (form_new_tray.ShowDialog() == DialogResult.OK && form_new_tray.id != "")
            {
                if (db_manager.IsConnected)
                {
                    foreach (ListViewItem itme_tray in listViewTray.Items)
                    {
                        if (form_new_tray.id == itme_tray.SubItems[0].Text)
                        {
                            MessageBox.Show(this, "This tray id is already use.", "Tray id error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            StatusText = "New tray: id error";
                            return;
                        }
                    }
                    Tray tray = new Tray(form_new_tray.id);
                    AddTrayListView(tray, "Empty");
                    db_manager.AddTray(tray);
                    StatusText = "New tray: done";

                    // Create a Fill task and display it
                    Task fill = new Task("Fill", tray.Id);
                    AddTODOListView(fill, new Lot(tray.Id), -1);

                    // Create NewTray Task and save it, display it
                    Task new_tray = new Task("NewTray", form_new_tray.Feedback, DateTime.Now, -1, 0, tray.Id);
                    db_manager.AddTask(new_tray);
                    AddTaskListView(new_tray);
                }
                else
                {
                    MessageBox.Show("Not connected to database.");
                    StatusText = "New tray: failed";
                }
            }
            else
            {
                StatusText = "New tray: aborted";
            }
        }

        /// <summary>
        /// New lot task. Called by the button in the ListView of tasks to do.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLot(object sender, ListViewColumnMouseEventArgs e)
        {
            ListViewItem item = e.Item; // Exactly the same than listViewTODO.SelectedItems[0]
            FormFill form = new FormFill();
            if (form.ShowDialog() == DialogResult.OK)
            {
                string tray_id = item.SubItems[1].Text; // Find the id of the tray relative to the new lot

                double food_quantity = form.Quantity;
                
                // Create new lot object:
                int id = db_manager.GetNewLotId(); // Find the last lot + 1: this will be the id of the new lot
                Lot lot = new Lot(id, food_quantity, form.Clutch, form.FeedFrequency, form.BreedingTime, tray_id);
                // Show the lot in ListViewLot:
                AddLotListView(lot);

                // Save the lot in database:
                db_manager.AddLot(lot);

                // Update tray state
                UpdateTrayState(tray_id, "Active"); // Update the label in the listview

                // Remove Fill task
                listViewTODO.Items[item.Index].Remove();

                // Create Fill task, save it
                Task fill = new Task("Fill", form.Feedback, DateTime.Now, -1, id, tray_id);
                db_manager.AddTask(fill);
                // Display finished Fill task:
                AddTaskListView(fill);

                // Create a Feed task
                Task feed = new Task("Feed", lot.Id, tray_id);
                int when_feed = lot.WhenFeed(DateTime.Today);
                AddTODOListView(feed, lot, when_feed);

                // Create a Collect task
                Task collect = new Task("Collect", lot.Id, tray_id);
                int when_collect = lot.WhenCollect(DateTime.Today);
                AddTODOListView(collect, lot, when_collect);
            }
        }

        /// <summary>
        /// Call by the button in the ListView of tasks to do
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Feed(object sender, ListViewColumnMouseEventArgs e)
        {
            ListViewItem item = e.Item; // Exactly the same than listViewTODO.SelectedItems[0]
            string when = item.SubItems[2].Text;
            if (when == "Today")
            {
                int lot_id = Int32.Parse(item.SubItems[3].Text);
                FormFeed form = new FormFeed();
                Lot lot = db_manager.GetLotById(lot_id);
                double f_quantity = lot.FoodQuantity;

                form.Quantity = f_quantity.ToString();
                form.SetTrayId(lot.TrayId.ToString());
                if (form.ShowDialog() == DialogResult.OK)
                {
                    double real_quantity = form.RealQantity == 0 ? lot.FoodQuantity : form.RealQantity;
                    Task feed = new Task("Feed", form.FeedBack, DateTime.Now, real_quantity, lot_id, lot.TrayId);
                    db_manager.AddTask(feed);
                    
                    UpdateTODO();
                }
            }
            else
            {
                int n_when = Int32.Parse(item.SubItems[2].Text);
                MessageBox.Show($"This task sould be done in {n_when.ToString()} day{((n_when == 1) ?  "": "s")}.");
            }
        }

        /// <summary>
        /// Collect task. Called by the button in the ListView of tasks to do.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Collect(object sender, ListViewColumnMouseEventArgs e)
        {
            string when = e.Item.SubItems[2].Text;
            if (when == "Today")
            {
                FormCollect form = new FormCollect();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int lot_id = Int32.Parse(e.Item.SubItems[3].Text);
                    Lot lot = db_manager.GetLotById(lot_id);
                    lot.CollectDate = DateTime.Now;
                    lot.ProductivityUnit = form.ProductivityUnit;
                    lot.ProductivityWeight = form.ProductivityWeight;

                    listViewTODO.Items[e.Item.Index].Remove(); // Remove task from ListViewTODO


                    // Update the harvested lot in the listview
                    foreach (ListViewItem lot_item in listViewLot.Items)
                    {
                        if (Int32.Parse(lot_item.SubItems[0].Text) == lot.Id)
                        {
                            listViewLot.Items[lot_item.Index].Remove(); // Remove the lot in the listview
                        }
                    }
                    // readd the lot in the listview:
                    AddLotListView(lot);
                    // Update the harvested lot in the bd:
                    db_manager.UpdateLot(lot);

                    // Create a collect task, save and display
                    Task collect = new Task("Collect", form.Feedback, DateTime.Now, -1, lot.Id, lot.TrayId);
                    db_manager.AddTask(collect);
                    AddTaskListView(collect);

                    // The tray is now empty, so: update tray state
                    UpdateTrayState(lot.TrayId, "Empty"); // Update the label in the listview

                    // Add Fill task
                    Task fill = new Task("Fill");
                    AddTODOListView(fill, new Lot(lot.TrayId), -1);
                }
            }
            else
            {
                int n_when = Int32.Parse(e.Item.SubItems[2].Text);
                MessageBox.Show($"This task sould be done in {n_when.ToString()} day{((n_when == 1) ? "" : "s")}.");
            }
        }

        // Properties
        /// <summary>
        /// Status label: connection to database.
        /// </summary>
        public string DBConnected
        {
            set { toolStripStatusLabel2.Text = value; }
        }
        /// <summary>
        /// Status label: action.
        /// </summary>
        public string StatusText
        {
            set { toolStripStatusLabel1.Text = value; }
        }
    }
}
