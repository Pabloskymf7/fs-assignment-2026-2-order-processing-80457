using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.API.Application.Queries;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;
using Xunit;

namespace SportsStore.Tests
{
    public class OrderManagementQueryTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // --- GetOrdersQuery ---

        [Fact]
        public async Task GetOrders_Returns_All_Orders()
        {
            var db = CreateInMemoryDb();
            db.Orders.AddRange(
                new Order { Status = OrderStatus.Submitted, TotalAmount = 100 },
                new Order { Status = OrderStatus.Completed, TotalAmount = 200 },
                new Order { Status = OrderStatus.PaymentFailed, TotalAmount = 50 }
            );
            await db.SaveChangesAsync();

            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetOrdersQueryHandler(db, mapper);

            var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetOrders_Returns_Empty_When_No_Orders()
        {
            var db = CreateInMemoryDb();
            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetOrdersQueryHandler(db, mapper);

            var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

            Assert.Empty(result);
        }

        // --- GetCustomerOrdersQuery ---

        [Fact]
        public async Task GetCustomerOrders_Returns_Only_Customer_Orders()
        {
            var db = CreateInMemoryDb();
            var customerId = Guid.NewGuid();
            var otherCustomerId = Guid.NewGuid();
            db.Orders.AddRange(
                new Order { CustomerId = customerId, Status = OrderStatus.Submitted, TotalAmount = 100 },
                new Order { CustomerId = customerId, Status = OrderStatus.Completed, TotalAmount = 200 },
                new Order { CustomerId = otherCustomerId, Status = OrderStatus.Submitted, TotalAmount = 50 }
            );
            await db.SaveChangesAsync();

            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetCustomerOrdersQueryHandler(db, mapper);

            var result = await handler.Handle(
                new GetCustomerOrdersQuery(customerId), CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.All(result, o => Assert.Equal(customerId, o.CustomerId));
        }

        [Fact]
        public async Task GetCustomerOrders_Returns_Empty_For_Unknown_Customer()
        {
            var db = CreateInMemoryDb();
            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetCustomerOrdersQueryHandler(db, mapper);

            var result = await handler.Handle(
                new GetCustomerOrdersQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Empty(result);
        }

        // --- GetOrdersByStatusQuery ---

        [Fact]
        public async Task GetOrdersByStatus_Returns_Only_Matching_Orders()
        {
            var db = CreateInMemoryDb();
            db.Orders.AddRange(
                new Order { Status = OrderStatus.Completed, TotalAmount = 100 },
                new Order { Status = OrderStatus.Completed, TotalAmount = 200 },
                new Order { Status = OrderStatus.PaymentFailed, TotalAmount = 50 }
            );
            await db.SaveChangesAsync();

            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetOrdersByStatusQueryHandler(db, mapper);

            var result = await handler.Handle(
                new GetOrdersByStatusQuery(OrderStatus.Completed), CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.All(result, o => Assert.Equal("Completed", o.Status));
        }

        [Fact]
        public async Task GetOrdersByStatus_Returns_Empty_When_No_Match()
        {
            var db = CreateInMemoryDb();
            db.Orders.Add(new Order { Status = OrderStatus.Submitted, TotalAmount = 100 });
            await db.SaveChangesAsync();

            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetOrdersByStatusQueryHandler(db, mapper);

            var result = await handler.Handle(
                new GetOrdersByStatusQuery(OrderStatus.Completed), CancellationToken.None);

            Assert.Empty(result);
        }
    }
}
