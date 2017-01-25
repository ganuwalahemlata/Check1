using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace KontinuityCRM.Tests.Models
{
    [TestClass]
    public class OrderTests
    {
        [TestMethod]
        public void ApplyDeclineRuleOnFaildRebill()
        { 
            // arrange
            var id = 12345;
            var order = new Order
            {
                OrderId = id,
                CustomerId = 0,
                ShippingMethodId = 1,
                ShippingMethod = new ShippingMethod() { RecurringPrice = 1 },
                CreditCardNumber = "4111111111111111",
                CreditCardExpirationMonth = Month.December,
                CreditCardExpirationYear = DateTime.Now.Year + 1,
                Notes = new List<OrderNote>(),
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct
                    {
                        ProductId = 1,
                        Quantity = 1,
                        NextDate = DateTime.Today,
                        NextProductId = 1,
                        RebillDiscount = 0,
                        ReAttempts = 1, //
                        NextProduct = new Product()
                        {
                            Price = 10,   
                            ProductSalvages = new List<ProductSalvage>()
                            {
                                new ProductSalvage
                                {
                                    SalvageProfile = new SalvageProfile
                                    {
                                        BillType = BillType.ByCycle,
                                        BillValue = 3,
                                        LowerPrice = true,
                                        LowerAmount = 2,
                                        CancelAfter = 3,
                                        LowerPriceAfter = 1,
                                        DeclineType = new DeclineType 
                                        {
                                            WildCard = ".*",
                                        },
                                    },
                                    ProductId = 1,
                                },
                            },
                        },
                        Product = new Product()
                        {
                        },
                        
                    },
                },
            };

            foreach (var op in order.OrderProducts)
                op.Order = order;


            #region mocking repositories

            var mockShippingMethodRepo = new Mock<IRepository<ShippingMethod>>();
            mockShippingMethodRepo.Setup(m => m.Find(1)).Returns(new ShippingMethod() { });

            var mockProductRepo = new Mock<IRepository<Product>>();
            mockProductRepo.Setup(m => m.Find(It.IsAny<object>())).Returns(new Product() { });

            var mockCustomerRepo = new Mock<IRepository<Customer>>();           
            var mockOrderRepo = new Mock<IRepository<Order>>();
            var mockOrderProductRepo = new Mock<IRepository<OrderProduct>>();
            var mockOrderTimeEventRepo = new Mock<IRepository<OrderTimeEvent>>();
            var mockPrepaidInfoRepo = new Mock<IRepository<PrepaidInfo>>();           

            #endregion

            // mock unitofwork
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x.ShippingMethodRepository).Returns(mockShippingMethodRepo.Object);
            uow.Setup(x => x.ProductRepository).Returns(mockProductRepo.Object);
            uow.Setup(x => x.OrderRepository).Returns(mockOrderRepo.Object);
            uow.Setup(x => x.CustomerRepository).Returns(mockCustomerRepo.Object);
            uow.Setup(x => x.OrderProductRepository).Returns(mockOrderProductRepo.Object);
            uow.Setup(x => x.OrderTimeEventRepository).Returns(mockOrderTimeEventRepo.Object);
            uow.Setup(x => x.PrepaidInfoRepository).Returns(mockPrepaidInfoRepo.Object);

            var wsw = new Mock<IWebSecurityWrapper>();
            wsw.Setup(x => x.CurrentUserId).Returns(It.IsAny<int>());

            var mapper = new Mock<IMappingEngine>();

            // act
            order.Rebill(uow.Object, wsw.Object, mapper.Object).Wait();

            // assert
            Assert.AreEqual(DateTime.Today.AddDays(3), order.OrderProducts.First().NextDate);
            Assert.AreEqual(2, order.OrderProducts.First().RebillDiscount);
            Assert.AreEqual(2, order.OrderProducts.First().ReAttempts);
        }

        [TestMethod]
        public void PrepaidDeclineRebill()
        {
            // arrange
            var id = 12345;

            var salvageProfile = new SalvageProfile
            {
                BillType = BillType.ByCycle,
                BillValue = 3,
                LowerPrice = true,
                LowerAmount = 2,
                CancelAfter = 3,
                PrepaidIncrement = 10,
                LowerPriceAfter = 1,
                DeclineType = new DeclineType
                {
                    WildCard = ".*",
                },
            };

            var order = new Order
            {
                OrderId = id,
                CustomerId = 0,
                ShippingMethodId = 1,
                IsPrepaid = true,
                ShippingMethod = new ShippingMethod() { RecurringPrice = 1 },
                CreditCardNumber = "4111111111111111",
                CreditCardExpirationMonth = Month.December,
                CreditCardExpirationYear = DateTime.Now.Year + 1,
                Notes = new List<OrderNote>(),
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct
                    {
                        ProductId = 1,
                        Quantity = 1,
                        NextDate = DateTime.Today,
                        NextProductId = 1,
                        RebillDiscount = 0,
                        ReAttempts = 1, //
                        SalvageProfile = salvageProfile,
                        NextProduct = new Product()
                        {
                            Price = 10,   
                            ProductSalvages = new List<ProductSalvage>()
                            {
                                new ProductSalvage
                                {
                                    SalvageProfile = salvageProfile,
                                    ProductId = 1,
                                },
                            },
                        },
                    },
                },
            };

            foreach (var op in order.OrderProducts)
                op.Order = order;


            #region mocking repositories

            var mockShippingMethodRepo = new Mock<IRepository<ShippingMethod>>();
            mockShippingMethodRepo.Setup(m => m.Find(1)).Returns(new ShippingMethod() { });

            var mockProductRepo = new Mock<IRepository<Product>>();
            mockProductRepo.Setup(m => m.Find(It.IsAny<object>())).Returns(new Product() { });

            var mockCustomerRepo = new Mock<IRepository<Customer>>();
            var mockOrderRepo = new Mock<IRepository<Order>>();
            var mockOrderProductRepo = new Mock<IRepository<OrderProduct>>();
            var mockOrderTimeEventRepo = new Mock<IRepository<OrderTimeEvent>>();
            var mockPrepaidInfoRepo = new Mock<IRepository<PrepaidInfo>>();

            #endregion

            // mock unitofwork
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x.ShippingMethodRepository).Returns(mockShippingMethodRepo.Object);
            uow.Setup(x => x.ProductRepository).Returns(mockProductRepo.Object);
            uow.Setup(x => x.OrderRepository).Returns(mockOrderRepo.Object);
            uow.Setup(x => x.CustomerRepository).Returns(mockCustomerRepo.Object);
            uow.Setup(x => x.OrderProductRepository).Returns(mockOrderProductRepo.Object);
            uow.Setup(x => x.OrderTimeEventRepository).Returns(mockOrderTimeEventRepo.Object);
            uow.Setup(x => x.PrepaidInfoRepository).Returns(mockPrepaidInfoRepo.Object);

            var wsw = new Mock<IWebSecurityWrapper>();
            wsw.Setup(x => x.CurrentUserId).Returns(It.IsAny<int>());

            var mapper = new Mock<IMappingEngine>();

            // act
            order.Rebill(uow.Object, wsw.Object, mapper.Object).Wait();

            // assert
            Assert.AreEqual(null, order.OrderProducts.First().NextDate);
            Assert.AreEqual(0, order.OrderProducts.First().RebillDiscount);
            Assert.AreEqual(0, order.OrderProducts.First().ReAttempts);
        }

        [TestMethod]
        public void PrepaidIncrementRebill()
        {
            // arrange
            var id = 12345;

            var salvageProfile = new SalvageProfile
            {
                BillType = BillType.ByCycle,
                BillValue = 3,
                LowerPrice = true,
                LowerAmount = 2,
                CancelAfter = 3,
                PrepaidIncrement = 10,
                LowerPriceAfter = 1,
                DeclineType = new DeclineType
                {
                    WildCard = ".*",
                },
            };

            var order = new Order
            {
                OrderId = id,
                CustomerId = 0,
                ShippingMethodId = 1,
                IsPrepaid = true,
                ShippingMethod = new ShippingMethod() { RecurringPrice = 1 },
                CreditCardNumber = "4111111111111111",
                CreditCardExpirationMonth = Month.December,
                CreditCardExpirationYear = DateTime.Now.Year + 1,
                Notes = new List<OrderNote>(),
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct
                    {
                        ProductId = 1,
                        Quantity = 1,
                        NextDate = DateTime.Today,
                        NextProductId = 1,
                        RebillDiscount = 0,
                        ReAttempts = 0, //
                        SalvageProfile = null,
                        NextProduct = new Product()
                        {
                            Price = 10,   
                            ProductSalvages = new List<ProductSalvage>()
                            {
                                new ProductSalvage
                                {
                                    SalvageProfile = salvageProfile,
                                    ProductId = 1,
                                },
                            },
                        },
                    },
                },
            };

            foreach (var op in order.OrderProducts)
                op.Order = order;


            #region mocking repositories

            var mockShippingMethodRepo = new Mock<IRepository<ShippingMethod>>();
            mockShippingMethodRepo.Setup(m => m.Find(1)).Returns(new ShippingMethod() { });

            var mockProductRepo = new Mock<IRepository<Product>>();
            mockProductRepo.Setup(m => m.Find(It.IsAny<object>())).Returns(new Product() { });

            var mockCustomerRepo = new Mock<IRepository<Customer>>();
            var mockOrderRepo = new Mock<IRepository<Order>>();
            var mockOrderProductRepo = new Mock<IRepository<OrderProduct>>();
            var mockOrderTimeEventRepo = new Mock<IRepository<OrderTimeEvent>>();
            var mockPrepaidInfoRepo = new Mock<IRepository<PrepaidInfo>>();

            #endregion

            // mock unitofwork
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x.ShippingMethodRepository).Returns(mockShippingMethodRepo.Object);
            uow.Setup(x => x.ProductRepository).Returns(mockProductRepo.Object);
            uow.Setup(x => x.OrderRepository).Returns(mockOrderRepo.Object);
            uow.Setup(x => x.CustomerRepository).Returns(mockCustomerRepo.Object);
            uow.Setup(x => x.OrderProductRepository).Returns(mockOrderProductRepo.Object);
            uow.Setup(x => x.OrderTimeEventRepository).Returns(mockOrderTimeEventRepo.Object);
            uow.Setup(x => x.PrepaidInfoRepository).Returns(mockPrepaidInfoRepo.Object);

            var wsw = new Mock<IWebSecurityWrapper>();
            wsw.Setup(x => x.CurrentUserId).Returns(It.IsAny<int>());

            var mapper = new Mock<IMappingEngine>();

            // act
            order.Rebill(uow.Object, wsw.Object, mapper.Object).Wait();

            // assert
            Assert.AreEqual(DateTime.Today.AddDays(3), order.OrderProducts.First().NextDate);
            Assert.AreEqual(0, order.OrderProducts.First().RebillDiscount);
            Assert.AreEqual(1, order.OrderProducts.First().ReAttempts);
            Assert.AreEqual(salvageProfile, order.OrderProducts.First().SalvageProfile);
        }
       
    }
}
