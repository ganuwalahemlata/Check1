using AutoMapper;
using FizzWare.NBuilder;
using KontinuityCRM.Models;
using KontinuityCRM.Models.APIModels;
using Moq;
using NUnit.Framework;

namespace KontinuityCRM.Tests.DtoMappers
{
    [TestFixture]
    public class DtoMapperTest
    {
        public IMappingEngine MapperEngine { get; set; }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            MapperEngine = TestHelpper.CreateMapperEngine();
        }

        [Test]
        public void PartialCreateModel_To_Partial()
        {
            PartialCreateModel model = Builder<PartialCreateModel>.CreateNew().Build();
            Partial entity = MapperEngine.Map<Partial>(model);

            Assert.AreEqual(model.FirstName, entity.FirstName);
            Assert.AreEqual(model.LastName, entity.LastName);
            Assert.AreEqual(model.Address1, entity.Address1);
            Assert.AreEqual(model.Address2, entity.Address2);
            Assert.AreEqual(model.City, entity.City);
            Assert.AreEqual(model.Province, entity.Province);
            Assert.AreEqual(model.PostalCode, entity.PostalCode);
            Assert.AreEqual(model.Country, entity.Country);
            Assert.AreEqual(model.Phone, entity.Phone);
            Assert.AreEqual(model.Email, entity.Email);
            Assert.AreEqual(model.AffiliateId, entity.AffiliateId);
            Assert.AreEqual(model.SubId, entity.SubId);
            Assert.AreEqual(model.ProductId, entity.ProductId);
            Assert.AreEqual(model.IPAddress, entity.IPAddress);
        }
    }
}
