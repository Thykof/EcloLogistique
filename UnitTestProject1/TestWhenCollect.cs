using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EcloLogistic;

namespace UnitTestProject2
{
    [TestClass]
    public class TestWhenCollect
    {
        /// <summary>
        /// Today = beginning day
        /// </summary>
        [TestMethod]
        public void TestWhenCollect1()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 5, new DateTime(2018, 5, 2), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 2);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(5, when);
        }
        /// <summary>
        /// Today = beginning day, Breeding day the following month
        /// </summary>
        [TestMethod]
        public void TestWhenCollect2()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 30, new DateTime(2018, 5, 6), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 6);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(30, when);
        }
        /// <summary>
        /// Beginning date < today < breeding date
        /// </summary>
        [TestMethod]
        public void TestWhenCollect3()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 11, 30, new DateTime(2018, 5, 1), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 11);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(20, when);
        }
        /// <summary>
        /// Today = Breeding day
        /// </summary>
        [TestMethod]
        public void TestWhenCollect4()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 5, 10, new DateTime(2018, 5, 22), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 6, 1);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(0, when);
        }

        [TestMethod]
        public void TestWhenCollect5()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 30, new DateTime(2018, 4, 16), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 13);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(3, when);
        }

        [TestMethod]
        public void TestWhenCollect6()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 5, new DateTime(2018, 5, 2), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 7);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(0, when);
        }

        [TestMethod]
        public void TestWhenCollect7()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 10, new DateTime(2018, 5, 26), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 5, 31);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(5, when);
        }

        [TestMethod]
        public void TestWhenCollect8()
        {
            Lot lot = new Lot(1, "Eggs", 0.9F, 3, 0, 0, 10, 10, new DateTime(2018, 5, 26), DateTime.MinValue, "0");
            DateTime today = new DateTime(2018, 6, 2);
            int when = lot.WhenCollect(today);
            Assert.AreEqual(3, when);
        }
    }
}
