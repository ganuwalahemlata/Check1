using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using AutoMapper;
using FizzWare.NBuilder;
using KontinuityCRM.Controllers.API;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.APIModels;
using KontinuityCRM.Models.Enums;
using Moq;
using NUnit.Framework;
using OrderProduct = KontinuityCRM.Models.APIModels.OrderProduct;


namespace KontinuityCRM.Tests.ApiControllers
{
    [TestFixture]
    public class PartialsControllerTest : IApiControllerTest<partialsController>
    {
        public class PartialsApiTestController : partialsController
        {
            public static OrderStatus OrderStatus = OrderStatus.All;
            public PartialsApiTestController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, INLogger logger)
                : base(uow, wsw, mapper, logger)
            {
            }

            protected override async Task<Order> CreateOrder(Partial partial, IUnitOfWork uow, PartialToOrderModel model, IMappingEngine mapper,
                IWebSecurityWrapper wsw)
            {
                var order = Builder<Order>.CreateNew()
                    .With(x => x.Status = OrderStatus)
                    .Build();
                return order;
            }
        }

        public class UrlTestHelper : UrlHelper
        {
            public override string Link(string routeName, object routeValues)
            {
                return "http://localhost/api/product";
            }
        } 

        public partialsController Controller { get; set; }
        public Mock<IUnitOfWork> UowMock { get; set; }
        public Mock<IWebSecurityWrapper> WswMock { get; set; }
        public IMappingEngine MapperEngine { get; set; }
        public Mock<INLogger> LoggerMock { get; set; }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            MapperEngine = TestHelpper.CreateMapperEngine();
        }
            
        [SetUp]
        public void SetUp()
        {
            UowMock = new Mock<IUnitOfWork>();
            WswMock = new Mock<IWebSecurityWrapper>();
            LoggerMock = new Mock<INLogger>();
        }

        [TearDown]
        public void TearDown()
        {
            Controller = null;
            UowMock = null;
            WswMock = null;
            LoggerMock = null;
        }

        public void InitializeController()
        {
            Controller = new PartialsApiTestController(UowMock.Object, WswMock.Object, MapperEngine, LoggerMock.Object);
            Controller.Request = new HttpRequestMessage();              //for Request.CreateResponse
            Controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            Controller.Url = new UrlTestHelper();
        }

        [Test]
        public void Post_Partial_InternalServerError_Returns_Error()
        {
            var model = Builder<PartialCreateModel>.CreateNew().Build();
            InitializeController();

            HttpResponseMessage response = Controller.post(model).Result;
            Assert.AreEqual(500, (int)response.StatusCode);  //HttpStatusCode.InternalServerError
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.AreEqual("Partial generation failed.", error.Message);
        }

        [Test]
        public void Post_Partial_Success_Returns_Entity()
        {
            var partialRepoMock = new Mock<IRepository<Partial>>();
            partialRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<Partial, bool>>>(), It.IsAny<Func<IQueryable<Partial>, IOrderedQueryable<Partial>>>(), It.IsAny<string>())).Returns(new List<Partial>());
            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns((Product) null);
            UowMock.Setup(x => x.PartialRepository).Returns(partialRepoMock.Object);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);

            var model = Builder<PartialCreateModel>.CreateNew().Build();
            InitializeController();

            HttpResponseMessage response = Controller.post(model).Result;
            Assert.AreEqual(201, (int)response.StatusCode);   //HttpStatusCode.Created
            ObjectContent<PartialAPIModel> content = response.Content as ObjectContent<PartialAPIModel>;
            PartialAPIModel entity = content.Value as PartialAPIModel;
            Assert.AreEqual(model.FirstName, entity.FirstName);
            Assert.AreEqual(model.LastName, entity.LastName);
        }

        [Test]
        public void Post_Order_InternalServerError_Returns_Error()
        {
            int partialId = 1;
            var model = Builder<PartialToOrderModel>.CreateNew().Build();
            InitializeController();

            HttpResponseMessage response = Controller.order(partialId, model).Result;
            Assert.AreEqual(400, (int)response.StatusCode);  //HttpStatusCode.BadRequest
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.IsNotEmpty(error.Message);   //default error message
        }

        [Test]
        public void Post_Order_PatialNotFound_By_PartialId_Returns_Error()
        {
            var partialRepoMock = new Mock<IRepository<Partial>>();
            partialRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns((Partial)null);
            UowMock.Setup(x => x.PartialRepository).Returns(partialRepoMock.Object);
            InitializeController();

            int partialId = 1;
            var model = Builder<PartialToOrderModel>.CreateNew().Build();
            HttpResponseMessage response = Controller.order(partialId, model).Result;

            Assert.AreEqual(404, (int)response.StatusCode);  //HttpStatusCode.NotFound
            ObjectContent<HttpException> content = response.Content as ObjectContent<HttpException>;
            HttpException error = content.Value as HttpException;
            Assert.AreEqual("Partial not found", error.Message);
        }

        [Test]
        public void Post_Order_Has_SinglePurchaseLimitErrors_Returns_Error()
        {
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = true)
                .Build();
            var partial = Builder<Partial>.CreateNew().Build();

            var partialRepoMock = new Mock<IRepository<Partial>>();
            partialRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(partial);
            UowMock.Setup(x => x.PartialRepository).Returns(partialRepoMock.Object);

            var testCardNumberRepoMock = new Mock<IRepository<TestCardNumber>>();
            testCardNumberRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<TestCardNumber, bool>>>(), It.IsAny<Func<IQueryable<TestCardNumber>, IOrderedQueryable<TestCardNumber>>>(), It.IsAny<string>())).Returns(new List<TestCardNumber>());
            UowMock.Setup(x => x.TestCardNumberRepository).Returns(testCardNumberRepoMock.Object);

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);

            var order = Builder<Order>.CreateNew().Build();
            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(), It.IsAny<string>())).Returns(new List<Order>(){ order });
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            InitializeController();

            var orderProduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            var model = Builder<PartialToOrderModel>.CreateNew()
                .With(x => x.OrderProducts = new List<OrderProduct>() { orderProduct })
                .Build();

            HttpResponseMessage response = Controller.order(partial.PartialId, model).Result;
            Assert.AreEqual(403, (int)response.StatusCode);  //HttpStatusCode.Forbidden
        }

        [Test]
        public void Post_Order_Success_Where_Order_Approved()
        {
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = false)
                .Build();
            var partial = Builder<Partial>.CreateNew().Build();

            var partialRepoMock = new Mock<IRepository<Partial>>();
            partialRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(partial);
            UowMock.Setup(x => x.PartialRepository).Returns(partialRepoMock.Object);

            var testCardNumberRepoMock = new Mock<IRepository<TestCardNumber>>();
            testCardNumberRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<TestCardNumber, bool>>>(), It.IsAny<Func<IQueryable<TestCardNumber>, IOrderedQueryable<TestCardNumber>>>(), It.IsAny<string>())).Returns(new List<TestCardNumber>());
            UowMock.Setup(x => x.TestCardNumberRepository).Returns(testCardNumberRepoMock.Object);

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);

            var order = Builder<Order>.CreateNew().Build();
            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(), It.IsAny<string>())).Returns(new List<Order>() { order });
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            PartialsApiTestController.OrderStatus = OrderStatus.Approved;
            InitializeController();

            var orderProduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            var model = Builder<PartialToOrderModel>.CreateNew()
                .With(x => x.OrderProducts = new List<OrderProduct>() { orderProduct })
                .Build();

            HttpResponseMessage response = Controller.order(partial.PartialId, model).Result;
            Assert.AreEqual(201, (int)response.StatusCode);  //HttpStatusCode.Created
            ObjectContent<OrderCreationResponse> content = response.Content as ObjectContent<OrderCreationResponse>;
            OrderCreationResponse orderCreationResponse = content.Value as OrderCreationResponse;
            Assert.IsNotNull(orderCreationResponse);

        }

        [Test]
        public void Post_Order_Success_Where_Order_NotApproved()
        {
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = false)
                .Build();
            var partial = Builder<Partial>.CreateNew().Build();

            var partialRepoMock = new Mock<IRepository<Partial>>();
            partialRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(partial);
            UowMock.Setup(x => x.PartialRepository).Returns(partialRepoMock.Object);

            var testCardNumberRepoMock = new Mock<IRepository<TestCardNumber>>();
            testCardNumberRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<TestCardNumber, bool>>>(), It.IsAny<Func<IQueryable<TestCardNumber>, IOrderedQueryable<TestCardNumber>>>(), It.IsAny<string>())).Returns(new List<TestCardNumber>());
            UowMock.Setup(x => x.TestCardNumberRepository).Returns(testCardNumberRepoMock.Object);

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);

            var order = Builder<Order>.CreateNew().Build();
            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(), It.IsAny<string>())).Returns(new List<Order>() { order });
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            PartialsApiTestController.OrderStatus = OrderStatus.All;
            InitializeController();

            var orderProduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            var model = Builder<PartialToOrderModel>.CreateNew()
                .With(x => x.OrderProducts = new List<OrderProduct>() { orderProduct })
                .Build();

            HttpResponseMessage response = Controller.order(partial.PartialId, model).Result;
            Assert.AreEqual(402, (int)response.StatusCode);  //HttpStatusCode.PaymentRequired
            ObjectContent<OrderCreationResponse> content = response.Content as ObjectContent<OrderCreationResponse>;
            OrderCreationResponse orderCreationResponse = content.Value as OrderCreationResponse;
            Assert.IsNotNull(orderCreationResponse);
        }
    }
}
