using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
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
    public class OrdersControllerTest : IApiControllerTest<ordersController>
    {
        public class OrdersApiTestController : ordersController
        {
            public static OrderStatus OrderStatus = OrderStatus.All;
            public OrdersApiTestController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, INLogger logger) : base(uow, wsw, mapper, logger)
            {
            }

            protected override async Task CreateOrder(Order order, IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            {
                order.Status = OrderStatus;
            }
        }

        public class UrlTestHelper : UrlHelper
        {
            public override string Link(string routeName, object routeValues)
            {
                return "http://localhost/api/product";
            }
        }      

        public ordersController Controller { get; set; }
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
            /*http://stackoverflow.com/questions/15022627/url-link-not-working-in-webapi*/
            Controller = new OrdersApiTestController(UowMock.Object, WswMock.Object, MapperEngine, LoggerMock.Object);
            Controller.Request = new HttpRequestMessage();              //for Request.CreateResponse
            Controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            Controller.Url = new UrlTestHelper();
        }

        [Test]
        public void Post_Upsell_InternalServerError_Returns_Error()
        {
            InitializeController();
            var upsellproducts = Builder<OrderProduct>.CreateListOfSize(2).Build();
            HttpResponseMessage response = Controller.upsell(1, upsellproducts).Result;

            Assert.AreEqual(500, (int)response.StatusCode);  //HttpStatusCode.InternalServerError
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.IsTrue(error.Message.StartsWith("Failed at upsell, possible reason could be"));
        }

        public static object[] NullOrEmptyUpSellProducts =
        {
            new object[] { null },
            new object[] { new List<OrderProduct>() }
        };

        [Test]
        [TestCaseSource("NullOrEmptyUpSellProducts")]
        public void Post_Upsell_UpsellProducts_NullOrEmpty_Returns_Error(List<OrderProduct> upsellproducts)
        {
            InitializeController();
            HttpResponseMessage response = Controller.upsell(1, upsellproducts).Result;

            Assert.AreEqual(404, (int)response.StatusCode);  //HttpStatusCode.NotFound
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.IsTrue(error.Message.StartsWith("Upsell products are not available or invalid. Try again with valid data"));
        }

        [Test]
        public void Post_Upsell_OrderNotFound_Returns_Error()
        {
            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns((Order)null);
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            InitializeController();

            int orderId = 1;
            var upsellproducts = Builder<OrderProduct>.CreateListOfSize(2).Build();
            HttpResponseMessage response = Controller.upsell(orderId, upsellproducts).Result;

            Assert.AreEqual(404, (int)response.StatusCode);  //HttpStatusCode.NotFound
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.AreEqual("Previous order not found.", error.Message);
        }

        [Test]
        public void Post_Upsell_ProductNotFound_Returns_Error()
        {
            var order = Builder<Order>.CreateNew().Build();

            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(order);
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            InitializeController();

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns((Product)null);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);
            InitializeController();

            var upsellproduct = Builder<OrderProduct>.CreateNew().Build();
            HttpResponseMessage response = Controller.upsell(order.OrderId, new List<OrderProduct>() { upsellproduct }).Result;

            Assert.AreEqual(404, (int)response.StatusCode);  //HttpStatusCode.NotFound
            ObjectContent<string> content = response.Content as ObjectContent<string>;
            string errorMessage = content.Value as string;
            Assert.AreEqual(string.Format("Product in upsell product not found. Prodcut id:{0}", upsellproduct.ProductId), errorMessage);
        }

        [Test]
        public void Post_Upsell_SinglePurchaseLimitErrors_Returns_Error()
        {
            var order = Builder<Order>.CreateNew().Build();
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = true)
                .Build();

            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(order);
            orderRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(), It.IsAny<string>())).Returns(Builder<Order>.CreateListOfSize(2).Build());
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            InitializeController();

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);
            InitializeController();

            var upsellproduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            HttpResponseMessage response = Controller.upsell(order.OrderId, new List<OrderProduct>() { upsellproduct }).Result;

            Assert.AreEqual(404, (int)response.StatusCode);  //HttpStatusCode.NotFound
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.AreEqual("No order found against this id. Try with different order id.", error.Message); /*Todo: error message should be like '"Single Purchase Limit Error"'*/
        }

        [Test]
        public void Post_Upsell_Success()
        {
            var order = Builder<Order>.CreateNew().Build();
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = false)
                .Build();

            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(order);
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            InitializeController();

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);
            InitializeController();

            var upsellproduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            HttpResponseMessage response = Controller.upsell(order.OrderId, new List<OrderProduct>() { upsellproduct }).Result;

            Assert.AreEqual(201, (int)response.StatusCode);  //HttpStatusCode.Created
            ObjectContent<OrderAPIModel> content = response.Content as ObjectContent<OrderAPIModel>;
            OrderAPIModel apiModel = content.Value as OrderAPIModel;
            Assert.IsNotNull(apiModel);
        }

        [Test]
        public void Post_Order_SinglePurchaseLimitErrors_Returns_Error()
        {
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = true)
                .Build();

            var orderRepoMock = new Mock<IRepository<Order>>();
            orderRepoMock.Setup(x => x.Get(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(), It.IsAny<string>())).Returns(Builder<Order>.CreateListOfSize(2).Build());
            UowMock.Setup(x => x.OrderRepository).Returns(orderRepoMock.Object);
            InitializeController();

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);
            InitializeController();

            var orderProduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            var orderCreateModel = Builder<OrderCreateModel>.CreateNew()
                .With(x => x.OrderProducts = new List<OrderProduct>() { orderProduct })
                .Build();
            HttpResponseMessage response = Controller.Post(orderCreateModel).Result;

            Assert.AreEqual(404, (int)response.StatusCode);  //HttpStatusCode.NotFound
            ObjectContent<HttpError> content = response.Content as ObjectContent<HttpError>;
            HttpError error = content.Value as HttpError;
            Assert.AreEqual(string.Format("Signle purchase limit stops further processing. Product id: {0}", orderProduct.ProductId), error.Message);
        }

        [Test]
        public void Post_Order_Success_Where_Order_Approved()
        {
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = false)
                .Build();

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);
            OrdersApiTestController.OrderStatus = OrderStatus.Approved;
            InitializeController();

            var orderProduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            var orderCreateModel = Builder<OrderCreateModel>.CreateNew()
                .With(x => x.OrderProducts = new List<OrderProduct>() { orderProduct })
                .Build();
            HttpResponseMessage response = Controller.Post(orderCreateModel).Result;

            Assert.AreEqual(201, (int)response.StatusCode);  //HttpStatusCode.Created
            ObjectContent<OrderCreationResponse> content = response.Content as ObjectContent<OrderCreationResponse>;
            OrderCreationResponse orderCreationResponse = content.Value as OrderCreationResponse;
            Assert.IsNotNull(orderCreationResponse);
        }

        [Test]
        public void Post_Order_Success_Where_Order_Not_Approved()
        {
            var product = Builder<Product>.CreateNew()
                .With(x => x.IsSinglePurchaseLimit = false)
                .Build();

            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(x => x.Find(It.IsAny<object>())).Returns(product);
            UowMock.Setup(x => x.ProductRepository).Returns(productRepoMock.Object);
            OrdersApiTestController.OrderStatus = OrderStatus.All;
            InitializeController();

            var orderProduct = Builder<OrderProduct>.CreateNew()
                .With(x => x.ProductId = product.ProductId)
                .Build();
            var orderCreateModel = Builder<OrderCreateModel>.CreateNew()
                .With(x => x.OrderProducts = new List<OrderProduct>() { orderProduct })
                .Build();
            HttpResponseMessage response = Controller.Post(orderCreateModel).Result;

            Assert.AreEqual(402, (int)response.StatusCode);  //HttpStatusCode.PaymentRequired
            ObjectContent<OrderCreationResponse> content = response.Content as ObjectContent<OrderCreationResponse>;
            OrderCreationResponse orderCreationResponse = content.Value as OrderCreationResponse;
            Assert.IsNotNull(orderCreationResponse);
        }

    }
}
