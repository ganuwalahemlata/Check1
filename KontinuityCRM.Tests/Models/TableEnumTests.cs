using System.Collections.Generic;
using KontinuityCRM.Models.Enums;
using NUnit.Framework;

namespace KontinuityCRM.Tests.Models
{
    [TestFixture]
    public class TableEnumTests : EnumTests
    {
        [Test]
        public void BillTypeEnum()
        {
            List<string> names = Names<BillType>();

            Assert.AreEqual(3, names.Count);

            Assert.IsTrue(names.Contains("ByCycle"));
            Assert.AreEqual(1, (int)BillType.ByCycle);

            Assert.IsTrue(names.Contains("ByDate"));
            Assert.AreEqual(2, (int)BillType.ByDate);

            Assert.IsTrue(names.Contains("ByDay"));
            Assert.AreEqual(3, (int)BillType.ByDay);
        }

        [Test]
        public void OrderStatusEnum()
        {
            List<string> names = Names<OrderStatus>();

            Assert.AreEqual(14, names.Count);

            Assert.IsTrue(names.Contains("All"));
            Assert.AreEqual(0, (int)OrderStatus.All);

            Assert.IsTrue(names.Contains("Approved"));
            Assert.AreEqual(1, (int)OrderStatus.Approved);

            Assert.IsTrue(names.Contains("Declined"));
            Assert.AreEqual(2, (int)OrderStatus.Declined);

            Assert.IsTrue(names.Contains("Void"));
            Assert.AreEqual(3, (int)OrderStatus.Void);

            Assert.IsTrue(names.Contains("Refunded"));
            Assert.AreEqual(4, (int)OrderStatus.Refunded);

            Assert.IsTrue(names.Contains("Deleted"));
            Assert.AreEqual(5, (int)OrderStatus.Deleted);

            Assert.IsTrue(names.Contains("RMAIssued"));
            Assert.AreEqual(6, (int)OrderStatus.RMAIssued);

            Assert.IsTrue(names.Contains("ShippedException"));
            Assert.AreEqual(7, (int)OrderStatus.ShippedException);

            Assert.IsTrue(names.Contains("Unpaid"));
            Assert.AreEqual(8, (int)OrderStatus.Unpaid);

            Assert.IsTrue(names.Contains("FailedCapture"));
            Assert.AreEqual(9, (int)OrderStatus.FailedCapture);

            Assert.IsTrue(names.Contains("IsTest"));
            Assert.AreEqual(10, (int)OrderStatus.IsTest);

            Assert.IsTrue(names.Contains("Fraud"));
            Assert.AreEqual(11, (int)OrderStatus.Fraud);

            Assert.IsTrue(names.Contains("OrderReturned"));
            Assert.AreEqual(12, (int)OrderStatus.OrderReturned);

            Assert.IsTrue(names.Contains("Shipped"));
            Assert.AreEqual(13, (int)OrderStatus.Shipped);
        }
    }
}
