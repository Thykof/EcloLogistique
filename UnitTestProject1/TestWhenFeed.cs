using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EcloLogistic;

namespace UnitTestProject1
{
    [TestClass]
    public class TestWhenFeed
    {
        [TestMethod]
        public void TestWhenFeed1()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 30, new DateTime(2018, 5, 2), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 6);
            int when = lot.WhenFeed(today);
            // when(day) = FeedFrequency - (Today - BeginningDate) = 10 - (6 - 2) = 10 - 4 = 6
            Assert.AreEqual(6, when);
        }
        [TestMethod]
        public void TestWhenFeed2()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 30, new DateTime(2018, 5, 6), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 6);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(10, when);
        }
        [TestMethod]
        public void TestWhenFeed3()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 11, 30, new DateTime(2018, 5, 1), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 11);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(1, when);
        }
        [TestMethod]
        public void TestWhenFeed4()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 5, 30, new DateTime(2018, 5, 1), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 13);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(3, when);
        }
        [TestMethod]
        public void TestWhenFeed5()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 30, new DateTime(2018, 4, 16), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 13);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(3, when);
        }
        // FeedFrequency = 0: prevent division by 0
        [TestMethod]
        public void TestWhenFeed6()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 0, 30, new DateTime(2018, 4, 16), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 13);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(0, when);
        }
        
        /// <summary>
        /// FeedFrequency = -1: default Lot object
        /// If the task's type is Fill, the given lot is build with the single
        /// argument constructor: <c>public Lot(string tray_id)</c> 
        /// </summary>
        [TestMethod]
        public void TestWhenFeed7()
        {
            Lot lot = new Lot("0");
            DateTime today = new DateTime(2018, 5, 13);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(-1, when);
        }
        [TestMethod]
        public void TestWhenFeed8()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 5, 30, new DateTime(2018, 5, 1), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 6);
            int when = lot.WhenFeed(today);
            // when(day) = FeedFrequency - (Today - BeginningDate) = 10 - (6 - 2) = 10 - 4 = 6
            Assert.AreEqual(0, when);
        }
        [TestMethod]
        public void TestWhenFeed9()
        {
            Lot lot = new Lot(1, "Eggs", 1, 3, 0, 0, 15, 30, new DateTime(2018, 5, 1, 12, 25, 12), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 6, 12, 28, 45);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(9, when);
        }
        [TestMethod]
        public void TestWhenFeed10()
        {
            Lot lot = new Lot(1, "Eggs", 1, 3, 0, 0, 1, 30, new DateTime(2018, 6, 25, 12, 25, 10), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 6, 25, 12, 25, 11);
            int when = lot.WhenFeed(today);
            Assert.AreEqual(0, when);
        }
        [TestMethod]
        public void TestWhenFeed11()
        {
            Lot lot = new Lot(1, "Eggs", 1, 3, 0, 0, 1, 30, new DateTime(2018, 6, 25, 12, 25, 10), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 6, 25, 12, 25, 9); // impossible
            int when = lot.WhenFeed(today);
            Assert.AreEqual(1, when);
        }
        [TestMethod]
        public void TestWhenFeed12()
        {
            Lot lot = new Lot(1, "Eggs", 1, 3, 0, 0, 1, 30, new DateTime(2018, 6, 25, 12, 25, 10), DateTime.MinValue, "0");
            int when = lot.WhenFeed(new DateTime(2018, 6, 26, 12, 25, 11));
            Assert.AreEqual(0, when);
        }
        [TestMethod]
        public void TestWhenFeed12B()
        {
            Lot lot = new Lot(1, "Eggs", 1, 3, 0, 0, 1, 30, new DateTime(2018, 6, 25, 12, 25, 10), DateTime.MinValue, "0");
            int when = lot.WhenFeed(new DateTime(2018, 6, 26, 12, 25, 9));
            Assert.AreEqual(0, when);
        }
        [TestMethod]
        public void TestWhenFeed13()
        {
            Lot lot = new Lot(5, "Eggs", 1, 3, 0, 0, 1, 30, new DateTime(2018, 6, 23, 20, 18, 30), DateTime.MinValue, "0");
            int when = lot.WhenFeed(new DateTime(2018, 6, 25, 20, 36, 10));
            Assert.AreEqual(0, when);
        }
    }
}
