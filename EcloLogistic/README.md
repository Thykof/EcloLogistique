# EcloLogistic

Look for 'c:\MongoDB\Server\3.4.5\bin',
and put database in c:/Users/Username/AppData/Roaming/EcloLogistic/ecloDB

## Developer's notes
MongoDB handle double number, why not use double in c# for productivity weight? OK

I've decided to Update just after a Feed Task. Then, No need to worry about removing old feed task, adding the next, etc.
Why? because here we need when_collect to know if we need to add feed task.

## TODO
 - Handle connection error.
 - Calculate total food, update value in listview.
 - Select all item from all listview that is related to the same IDs that the user selected item is.
 - Sort realized tasks.
 - Wrappe all database method with a function that ping the database server, and update status texts.
 - Make the listviews resizable, remove flow layout.
 - Delete all the selected items in listviews, and ask for deleting related other item (deleting a lot will delete all related tasks).

### Sync with remote server
It could be possible by droping all collections in the remote server and add all elements from local database, when clicking on a Push button.
But, maybe split the startserver method by a StartLocalServer and Connect.

## Change log
### 0.0.4
 - Improve validation of Collect form (unit tests).
 - Fix various bugs.
 - Use double precision instead of float.
 - Add Task.IsIsAlreadyDone (with unit tests) and DBManager.GetTasksByLotId.
 - Improve form fill validation.

 - Remove prevent adding dublon in AddTrayListView, and in AddTaskListView for NewTray, and in AddLotListView.


## Doc
PxtlCa.XmlCommentMarkDownGenerator

## Test
MSTest.TestFramework
MSTest.TestAdapter
System.Runtime
