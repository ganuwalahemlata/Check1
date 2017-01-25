using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KontinuityCRM.Tests.Helpers
{
    [TestFixture]
    public class TimezoneUtiliyTest
    {
        class TestDateTimeObject
        {
            public DateTime DateTime { get; set; }
            public DateTime SetDateTime
            {
                set
                {
                    DateTime = value;
                }
            }
            public DateTime GetDateTime
            {
                get
                {
                    return DateTime;
                }
            }
            public DateTime? NullDateTime { get; set; }
            public DateTime? AnotherNullDateTime { get; set; }
        }

        /*
         * returns .NETFramework timezone order by DisplayName
         */
        [Test]
        public void StandTimeZones()
        {
            var list = TimeZoneUtility.TimeZones().ToList();

            /*total item*/
            Assert.AreEqual(113, list.Count);       //113 entity added to db, timezones tbl

            /*order by displayName*/
            var timezone = list[0]; //1st item
            Assert.AreEqual("(UTC) Casablanca", timezone.DisplayName);
            Assert.AreEqual("Morocco Standard Time", timezone.Id);
            timezone = list.ToList()[43];   //44th item
            Assert.AreEqual("Bangladesh Standard Time", timezone.Id);
            Assert.AreEqual("(UTC+06:00) Dhaka", timezone.DisplayName);
            timezone = list.ToList()[112];  //last item
            Assert.AreEqual("Dateline Standard Time", timezone.Id);
            Assert.AreEqual("(UTC-12:00) International Date Line West", timezone.DisplayName);
        }

        [Test]
        public void UtcTimezone()
        {
            TimeZoneInfo info = TimeZoneInfo.Utc;
            Assert.AreEqual("UTC", info.Id);                                        //not (UTC) Casablanca
            Assert.AreEqual("(UTC) Coordinated Universal Time", info.DisplayName);  //not Morocco Standard Time
        }

        [Test]
        [TestCase("Morocco Standard Time")]
        [TestCase("morocco standard time")]
        public void GetTimeZone_Found(string timeZoneName)
        {
            TimeZoneInfo timezone = TimeZoneUtility.GetTimeZone(timeZoneName);
            Assert.IsNotNull(timezone);
            Assert.AreEqual("(UTC) Casablanca", timezone.DisplayName);
        }

        [Test]
        [TestCase("My timezone")]
        [TestCase("")]
        [TestCase(null)]
        public void GetTimeZone_Not_Found(string timeZoneName)
        {
            Assert.Catch<Exception>(() => TimeZoneUtility.GetTimeZone(timeZoneName));
        }

        [Test]
        public void ToUtc()
        {
            DateTime utcDateTime = DateTime.ParseExact("03-25-2015, 10:30 PM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture);           //UTC   timezone-"(UTC) Coordinated Universal Time"
            DateTime centralAsiaDateTime = DateTime.ParseExact("03-26-2015, 04:30 AM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture);   //UTC+6 timezone-"Central Asia Standard Time"            
            Assert.AreEqual(utcDateTime, TimeZoneUtility.ToUtc(centralAsiaDateTime, TimeZoneUtility.GetTimeZone("Central Asia Standard Time")));
            Assert.AreEqual(DateTime.ParseExact("03-26-2015, 04:30 AM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture), centralAsiaDateTime);
        }

        [Test]
        public void FromUtc()
        {
            DateTime utcDateTime = DateTime.ParseExact("03-25-2015, 10:30 PM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture);           //UTC   timezone-"(UTC) Coordinated Universal Time"
            DateTime centralAsiaDateTime = DateTime.ParseExact("03-26-2015, 04:30 AM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture);   //UTC+6 timezone-"Central Asia Standard Time"
            Assert.AreEqual(centralAsiaDateTime, TimeZoneUtility.FromUtc(utcDateTime, TimeZoneUtility.GetTimeZone("Central Asia Standard Time")));
            Assert.AreEqual(DateTime.ParseExact("03-25-2015, 10:30 PM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture), utcDateTime);
        }

        [Test]
        public void ConvertDateTime_List()
        {
            Func<DateTime> dateTimeFunc = () => DateTime.ParseExact("03-25-2015, 10:30 PM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture);  //UTC   timezone-"(UTC) Coordinated Universal Time"
            DateTime centralAsiaDateTime = DateTime.ParseExact("03-26-2015, 04:30 AM", "MM-dd-yyyy, hh:mm tt", CultureInfo.InvariantCulture);       //UTC+6 timezone-"Central Asia Standard Time"

            var sourceItem = new TestDateTimeObject()
            {
                DateTime = dateTimeFunc(),
                NullDateTime = dateTimeFunc(),
                AnotherNullDateTime = null
            };
            var sourceList = new List<TestDateTimeObject>() { sourceItem };
            var convertedItem = sourceList.ConvertUtcTime(TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time")).ToList()[0];

            //source also changed
            Assert.AreEqual(centralAsiaDateTime, sourceItem.DateTime);
            Assert.AreEqual(centralAsiaDateTime, sourceItem.NullDateTime);
            Assert.AreEqual(null, sourceItem.AnotherNullDateTime);
            //converted
            Assert.AreEqual(centralAsiaDateTime, convertedItem.DateTime);      //convered datetime object
            Assert.AreEqual(centralAsiaDateTime, convertedItem.NullDateTime);  //convered null able datetime
            Assert.AreEqual(null, convertedItem.AnotherNullDateTime);          //manage null datetime
        }
    }
}
