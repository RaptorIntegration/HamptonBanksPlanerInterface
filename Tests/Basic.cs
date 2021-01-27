using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebSort;
using WebSort.Model;

namespace Tests
{
    [TestClass]
    public class Basic
    {
        [TestMethod, TestCategory("Version")]
        public void Version()
        {
            User user = Global.GetSecurity("Home", "Raptor");
            Assert.IsNotNull(user);
            Assert.AreEqual(user.UserID, 4);
        }
    }
}