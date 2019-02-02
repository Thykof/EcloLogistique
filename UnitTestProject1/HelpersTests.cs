using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcloLogistic.Tests
{
    [TestClass()]
    public class HelpersTests
    {
        [TestMethod()]
        public void CompareTest()
        {
            // TODO
        }

        [TestMethod()]
        public void ValidateCollectFormTest1()
        {
            bool result = Helpers.ValidateCollectForm("", "", out double ProductivityWeight, out int ProductivityUnit);
            Assert.AreEqual(false, result);
            Assert.AreEqual(0, ProductivityWeight);
            Assert.AreEqual(0, ProductivityUnit);
        }
        [TestMethod()]
        public void ValidateCollectFormTest2()
        {
            bool result = Helpers.ValidateCollectForm("1.1", "100", out double ProductivityWeight, out int ProductivityUnit);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.1, ProductivityWeight);
            Assert.AreEqual(100, ProductivityUnit);
        }
        [TestMethod()]
        public void ValidateCollectFormTest3()
        {
            bool result = Helpers.ValidateCollectForm("3.6", "100.", out double ProductivityWeight, out int ProductivityUnit);
            Assert.AreEqual(false, result);
            Assert.AreEqual(3.6, ProductivityWeight);
            Assert.AreEqual(0, ProductivityUnit);
        }
        [TestMethod()]
        public void ValidateCollectFormTest4()
        {
            bool result = Helpers.ValidateCollectForm("3.6.", "100", out double ProductivityWeight, out int ProductivityUnit);
            Assert.AreEqual(false, result);
            Assert.AreEqual(0, ProductivityWeight);
            Assert.AreEqual(100, ProductivityUnit);
        }
        [TestMethod()]
        public void ValidateCollectFormTest5()
        {
            bool result = Helpers.ValidateCollectForm("", "100", out double ProductivityWeight, out int ProductivityUnit);
            Assert.AreEqual(false, result);
            Assert.AreEqual(0, ProductivityWeight);
            Assert.AreEqual(100, ProductivityUnit);
        }
    }
}