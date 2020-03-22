# EcloLogistique
Desktop software program for eclofood project.

This software aims to manage insect rearing.
The insects are placed in trays and must be feed frequently. 
The user can see which tray needs to be feed.

The software can be used offline, which is the main constraint.

## Features
 - use a local mondoDB database
 - handle multiple trays

## Future development
synchronize with remote MongoDB server


# How to use
The operating system must be Windows. You need to have a MongoDB server installed on the computer.

At the start of the program, you need to select the installation folder of MongoDB server. `For example: C:\Program Files\MongoDB\Server\4.2\bin`.

You can create a tray. Then you can fill the tray in the TODO tab. You can see the new lot in the Lot tab. After the specified amount of days, you would be able to feed the tray and collect it.
All the TODO tasks are saved in the Tasks tab.
