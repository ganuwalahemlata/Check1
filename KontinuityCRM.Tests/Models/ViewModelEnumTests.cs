using System;
using System.Collections.Generic;
using System.Linq;
using KontinuityCRM.Models.ViewModels.Enum;
using NUnit.Framework;

namespace KontinuityCRM.Tests.Models
{
    [TestFixture]
    public class ViewModelEnumTests : EnumTests
    {
        [Test]
        public void RebillSearchOption()
        {
            List<string> names = Names<RebillSearchOptionEnum>();

            Assert.IsTrue(names.Contains("RebillDate"));
            Assert.AreEqual(1, (int)RebillSearchOptionEnum.RebillDate);

            Assert.IsTrue(names.Contains("SignUpDate"));
            Assert.AreEqual(2, (int)RebillSearchOptionEnum.SignUpDate);
        }
    }
}
