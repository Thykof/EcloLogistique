using System;
using System.Windows.Forms;


namespace EcloLogistic
{
    partial class MainForm
    {
        // Initialization
        private void InitializeListViewTray()
        {
            listViewTray.FullRowSelect = true;

            // Menu (right click)
            tray_menu1 = new MenuItem[] { new MenuItem("New", OnNewTray), };
            tray_menu2 = new MenuItem[] { new MenuItem("Delete", OnDeleteTray), };

            listViewTray.ContextMenu = new ContextMenu();
            listViewTray.ContextMenu.MenuItems.AddRange(tray_menu1); // begin with standard items (tray_menu1)
            listViewTray.ContextMenu.Popup += OnMenuPopupListViewTray; // add the popup handler
        }

        private void InitializeListViewLot()
        {
            listViewLot.FullRowSelect = true;
            lot_menu = new MenuItem[] { new MenuItem("Delete", OnDeleteLot), };

            // Menu (right click)
            listViewLot.ContextMenu = new ContextMenu(); ;
            listViewLot.ContextMenu.MenuItems.AddRange(lot_menu);
            listViewLot.ContextMenu.Popup += OnMenuPopupListViewLot; // add the popup handler
        }
        private void InitializeListViewTask()
        {
            listViewTask.FullRowSelect = true;
            task_menu = new MenuItem[] { new MenuItem("Delete", OnDeleteTask), };

            // Menu (right click)
            listViewTask.ContextMenu = new ContextMenu(); ;
            listViewTask.ContextMenu.MenuItems.AddRange(task_menu);
            listViewTask.ContextMenu.Popup += OnMenuPopupListViewTask; // add the popup handler

            //HiddenColumnTaskId.Width = 0;
            HiddenColumnTaskId.Dispose();
        }
        /// <summary>
        /// Initialize todo ListView with the ListViewExtender
        /// </summary>
        /// <remarks>
        /// The first column contains a button widget.
        /// </remarks>
        public void initialize_listViewTODO()
        {
            // listView with buttons
            listViewTODO.FullRowSelect = true;
            ListViewExtender extender = new ListViewExtender(listViewTODO);
            // extend 2nd column
            ListViewButtonColumn buttonAction = new ListViewButtonColumn(0);
            buttonAction.Click += ExecuteTask;
            buttonAction.FixedWidth = true;
            extender.AddColumn(buttonAction);

            listViewTODO.ListViewItemSorter = new ListViewItemComparer(2);

            // example:
            /*ListViewItem item = listViewTODO.Items.Add("Feed");
            item.SubItems.Add("0");
            item.SubItems.Add("Today");
            item.SubItems.Add("0");*/
        }

        // Events:
        private void OnMenuPopupListViewTray(object sender, EventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            if (menu == null)
                return;

            menu.MenuItems.Clear();

            // If an item was selected, display Delete option. TODO: only in admin mode.
            if (listViewTray.SelectedItems.Count > 0)
            {
                menu.MenuItems.AddRange(tray_menu2);
            }
            else // display only the New option
            {
                menu.MenuItems.AddRange(tray_menu1);
            }
        }
        private void OnMenuPopupListViewLot(object sender, EventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            ListView list_view = menu.SourceControl as ListView;
            menu.MenuItems.Clear();

            if (menu == null || list_view.SelectedItems.Count == 0)
                return;
            else
                menu.MenuItems.AddRange(lot_menu);
        }
        private void OnMenuPopupListViewTask(object sender, EventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            ListView list_view = menu.SourceControl as ListView;
            menu.MenuItems.Clear();

            if (menu == null || list_view.SelectedItems.Count == 0)
                return;
            else
                menu.MenuItems.AddRange(task_menu);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: in async mode; if not remote server
            string response = db_manager.StopServer();
            //MessageBox.Show(response);
        }

        /// <summary>
        /// Call by a click on the button in the todo Listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteTask(object sender, ListViewColumnMouseEventArgs e)
        {
            // ListViewExtender
            ListViewItem item = e.Item;
            switch (e.Item.SubItems[0].Text)
            {
                case "Fill":
                    // Show Fill Task dialog window:
                    NewLot(sender, e);
                    break;
                case "Feed":
                    // Show Feed Task dialog window:
                    Feed(sender, e);
                    break;
                case "Collect":
                    // Show Collect Task dialog window:
                    Collect(sender, e);
                    break;
            }
        }

        /// <summary>
        /// New tray (from menu).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTray();
        }

        /// <summary>
        /// From Listview menu (right click)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewTray(Object sender, System.EventArgs e)
        {
            NewTray();
        }

        private void buttonNewTray_Click(object sender, EventArgs e)
        {
            NewTray();
        }

        /// <summary>
        /// Tray Listview menu (right click)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteTray(object sender, EventArgs e)
        {
            string id = listViewTray.SelectedItems[0].SubItems[0].Text;
            string messsage = "Delete this tray : " + id + "?";
            DialogResult can_delete = MessageBox.Show(messsage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (can_delete == DialogResult.Yes)
            {
                db_manager.DeleteTrayById(id);
                listViewTray.SelectedItems[0].Remove();
            }
            StatusText = "Delete tray: done";
        }

        /// <summary>
        /// Lot Listview menu (right click)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteLot(object sender, EventArgs e)
        {
            string id = listViewLot.SelectedItems[0].SubItems[0].Text;
            string messsage = "Delete this lot : " + id + "?";
            DialogResult can_delete = MessageBox.Show(messsage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (can_delete == DialogResult.Yes)
            {
                db_manager.DeleteLotById(id);
                listViewLot.SelectedItems[0].Remove();
            }
            StatusText = "Delete lot: done";
        }

        /// <summary>
        /// Task Listview menu (right click): delete task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteTask(object sender, EventArgs e)
        {
            string type = listViewTask.SelectedItems[0].SubItems[0].Text; // Type
            string tray_id = listViewTask.SelectedItems[0].SubItems[5].Text; // TrayId
            string s_date = listViewTask.SelectedItems[0].SubItems[3].Text; // Date
            DateTime date = DateTime.Parse(s_date);
            string messsage = $"Delete this task : \nType: {type}\nTray id: {tray_id}\nDate: {date.ToString()}\n?";
            DialogResult can_delete = MessageBox.Show(messsage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (can_delete == DialogResult.Yes)
            {
                db_manager.DeleteTask(tray_id, type, date);
                listViewTask.SelectedItems[0].Remove();
            }
            StatusText = "Delete task: done";
        }


        /// <summary>
        /// Update todo list. From button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateTODO();
        }

        /// <summary>
        /// Update todo list. From menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateTODO();
        }

        /// <summary>
        /// Change server location, called by Menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeServerLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeServerLocation();
        }


        // Informations dialogs
        /// <summary>
        /// Server informations dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = "Server informations";
            string message = "EcloLogistic work with a local MongnDB server.\n";
            message += "You must instal mongoDB server on the machine.\n";
            message += "You can specify the location of the installation by clicking on Change Server Location.";
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Display About dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About form_about = new About();
            form_about.ShowDialog(this);
        }
    }
}
