using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KontinuityCRM.Models;
using System.Collections.Generic;

namespace KontinuityCRM.Tests.Models
{
    [TestClass]
    public class BalancerTests
    {
        [TestMethod]
        public void TestProcessorSelectionByAllocation()
        {
            // arrange
            var balancer = new Balancer
            {
                
                AllocationBalance = true,
                BalancerProcessors = new List<BalancerProcessor>()
                {
                    new BalancerProcessor
                    {
                        ProcessorId = 1,
                        Allocation = 500,
                        AllocationPercent = 30,
                        ProcessedAllocation = 10,
                        Initials = 2,
                        InitialLimit = 5,
                        
                    },

                    new BalancerProcessor
                    {
                        ProcessorId = 2,
                        Allocation = 200,
                        AllocationPercent = 70,
                        ProcessedAllocation = 30,
                        Initials = 3,
                        InitialLimit = 6,
                    },

                }
            };
            var amount = 20;

            // act
            var bp = balancer.SelectProcessor(amount);

            // assert
            Assert.AreEqual(bp.ProcessorId, 1);
            
        }

        [TestMethod]
        public void TestProcessorSelectionByPercent()
        {
            // arrange
            var balancer = new Balancer
            {
                AllocationBalance = false,
                BalancerProcessors = new List<BalancerProcessor>()
                {
                    new BalancerProcessor
                    {
                        ProcessorId = 1,
                        Allocation = 500,
                        AllocationPercent = 30,
                        ProcessedAllocation = 10,
                        Initials = 2,
                        InitialLimit = 5,
                    },
                    new BalancerProcessor
                    {
                        ProcessorId = 2,
                        Allocation = 200,
                        AllocationPercent = 70,
                        ProcessedAllocation = 30,
                        Initials = 3,
                        InitialLimit = 6,
                    },
                }
            };
            var amount = 20;

            // act
            var bp = balancer.SelectProcessor(amount);

            // assert
            Assert.AreEqual(bp.ProcessorId, 2);
        }
    }
}
