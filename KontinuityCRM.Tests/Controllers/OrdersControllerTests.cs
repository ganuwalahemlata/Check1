using ArtisanCode.SimpleAesEncryption;
using AutoMapper;
using KontinuityCRM.Controllers.API;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace KontinuityCRM.Tests.Controllers
{
    [TestClass]
    public class OrdersControllerTests
    {
        [TestMethod]
        public void MustReturn201AndLinkForPost() 
        {
            // Arrange            
            var order = new KontinuityCRM.Models.APIModels.OrderCreateModel
            {
                CustomerId = 0,
                ShippingMethodId = 1,  
                CreditCardNumber = "371449635398431",
                
                OrderProducts = new List<KontinuityCRM.Models.APIModels.OrderProduct>
                {
                    new KontinuityCRM.Models.APIModels.OrderProduct
                    {
                        ProductId = 1,
                        Quantity = 1,
                    },
                },
            };

            string requestUri = "http://localhost:62806/api/orders/";
            //Uri uriForNewOrder = new Uri(new Uri(requestUri), id.ToString());

            #region mocking repositories

            var mockShippingMethodRepo = new Mock<IRepository<ShippingMethod>>();
            mockShippingMethodRepo.Setup(m => m.Find(1)).Returns(new ShippingMethod() { });

            var mockProductRepo = new Mock<IRepository<Product>>();
            mockProductRepo.Setup(m => m.Find(It.IsAny<object>())).Returns(new Product() { });

            var mockCustomerRepo = new Mock<IRepository<Customer>>();
            var mockTestCardNumberRepo = new Mock<IRepository<TestCardNumber>>();
            var mockPrepaidInfoRepo = new Mock<IRepository<PrepaidInfo>>();
            //mockCustomerRepo.Setup(m => m.Find(It.IsAny<object>())).Returns(new Customer() { });

            var mockOrderRepo = new Mock<IRepository<Order>>();
            
            
            #endregion
            
            // mock unitofwork
            var mockuow = new Mock<IUnitOfWork>();
            mockuow.Setup(x => x.Save()).Returns(1);
            mockuow.Setup(x => x.ShippingMethodRepository).Returns(mockShippingMethodRepo.Object);
            mockuow.Setup(x => x.ProductRepository).Returns(mockProductRepo.Object);
            mockuow.Setup(x => x.OrderRepository).Returns(mockOrderRepo.Object);
            mockuow.Setup(x => x.CustomerRepository).Returns(mockCustomerRepo.Object);
            mockuow.Setup(x => x.TestCardNumberRepository).Returns(mockTestCardNumberRepo.Object);
            mockuow.Setup(x => x.PrepaidInfoRepository).Returns(mockPrepaidInfoRepo.Object);

            mockPrepaidInfoRepo.Setup(x => x.GetSet()).Returns(new List<PrepaidInfo>().AsQueryable());

            // mock websecuritywrapper
            var mockwsw = new Mock<IWebSecurityWrapper>();
            mockwsw.Setup(x => x.CurrentUserId).Returns(It.IsAny<int>());

            // mock messageEncryptor
            //var mockme = new Mock<IMessageEncryptor>();
            //mockme.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(It.IsAny<string>());

            // mock messageDecritor
            //var mockmd = new Mock<IMessageDecryptor>();
            //mockmd.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(It.IsAny<string>());

            DtoMapperConfig.CreateMaps();
            
            var controller = new ordersController(mockuow.Object, mockwsw.Object/*, mockme.Object*/, Mapper.Engine/*, mockmd.Object*/, new Mock<INLogger>().Object);

            controller.SetRequest("orders", HttpMethod.Post, requestUri);

            // Act
            HttpResponseMessage response = controller.Post(order).Result;

            // Assert
            mockOrderRepo.Verify(v => v.Add(It.IsAny<Order>()), Times.Once());
            mockuow.Verify(u => u.Save(It.IsAny<object>()), Times.Exactly(2));
            //mockuow.VerifyAll();
            

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            //Assert.AreEqual(uriForNewOrder, response.Headers.Location);
            
        }
    }
}
