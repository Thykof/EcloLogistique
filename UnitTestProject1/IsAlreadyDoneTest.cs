using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EcloLogistic;

namespace UnitTestProject1
{
    [TestClass]
    public class IsAlreadyDoneTest
    {
        [TestMethod]
        public void IsAlreadyDoneTest1()
        {
            Task task = new Task("Feed", 1, "A");
            List<Task> tasks = new List<Task> {
                new Task("Fill", "", new DateTime(2018, 6, 24, 20, 45, 10), -1, 1, "A"),
                new Task("Feed", "", new DateTime(2018, 6, 25, 20, 45, 10), 0.1, 1, "A"),
            };
            bool result = task.IsAlreadyDone(new DateTime(2018, 6, 25, 20, 45, 12), tasks);
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void IsAlreadyDoneTest2()
        {
            Task task = new Task("Feed", "", new DateTime(), 0.1, 1, "A");
            List<Task> tasks = new List<Task> {
                new Task("Fill", "", new DateTime(2018, 6, 25, 20, 45, 9), -1, 1, "A"),
            };
            bool result = task.IsAlreadyDone(new DateTime(2018, 6, 25, 20, 45, 12), tasks);
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void IsAlreadyDoneTest3()
        {
            // Lot has FeedFrquency to 2.
            Task task = new Task("Feed", "", new DateTime(), 0.1, 1, "A");
            List<Task> tasks = new List<Task> {
                new Task("Fill", "", new DateTime(2018, 6, 20, 20, 45, 10), -1, 1, "A"),
                new Task("Feed", "", new DateTime(2018, 6, 22, 20, 45, 10), 0.1, 1, "A"),
                new Task("Feed", "", new DateTime(2018, 6, 24, 20, 45, 10), 0.1, 1, "A"),
            };
            bool result = task.IsAlreadyDone(new DateTime(2018, 6, 24, 20, 45, 12), tasks);
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void IsAlreadyDoneTest3B()
        {
            // Lot has FeedFrquency to 2.
            Task task = new Task("Feed", "", new DateTime(), 0.1, 1, "A");
            List<Task> tasks = new List<Task> {
                new Task("Fill", "", new DateTime(2018, 6, 20, 20, 45, 10), -1, 1, "A"),
                new Task("Feed", "", new DateTime(2018, 6, 22, 20, 45, 10), 0.1, 1, "A"),
                new Task("Feed", "", new DateTime(2018, 6, 24, 20, 45, 10), 0.1, 1, "A"),
            };
            bool result = task.IsAlreadyDone(new DateTime(2018, 6, 26, 20, 45, 12), tasks);
            Assert.AreEqual(false, result);
        }

    }
}
