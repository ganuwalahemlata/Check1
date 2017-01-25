using KontinuityCRM.Models.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;

namespace KontinuityCRM.Tests.Helpers
{
    [TestClass]
    public class KontinuityCRMHelperTests
    {
        [TestMethod]
        public void TestGetNextDate()
        { 
            // arrange
            var date = new DateTime(2015, 8, 12);

            // act 

            //First = 1,
            //Second,
            //Third,
            //Fourth,
            //Last = 5,
      
            //Sunday = 0,
            //Monday = 1,
            //Tuesday = 2,
            //Wednesday = 3,
            //Thursday = 4,
            //Friday = 5,
            //Saturday = 6,

            var date1 = KontinuityCRMHelper.GetNextDate(date, BillType.ByCycle, 3);
            var date2 = KontinuityCRMHelper.GetNextDate(date, BillType.ByDate, 30);
            var date3 = KontinuityCRMHelper.GetNextDate(date, BillType.ByDate, 3);
            var date4 = KontinuityCRMHelper.GetNextDate(date, BillType.ByDay, 33); // third wednesday
            var date5 = KontinuityCRMHelper.GetNextDate(date, BillType.ByDay, 14); // first thurday
            var date6 = KontinuityCRMHelper.GetNextDate(date, BillType.ByDay, 55); // last friday

            // assert 
            Assert.AreEqual(new DateTime(2015, 8, 15), date1);
            Assert.AreEqual(new DateTime(2015, 8, 30), date2);
            Assert.AreEqual(new DateTime(2015, 9, 3), date3);
            Assert.AreEqual(new DateTime(2015, 8, 19), date4);
            Assert.AreEqual(new DateTime(2015, 9, 3), date5);
            Assert.AreEqual(new DateTime(2015, 8, 28), date6);
        }
    }
}
