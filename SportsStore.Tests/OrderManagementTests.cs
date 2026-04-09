using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.API.Application.Commands;
using OrderManagement.API.Application.Queries;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.DTOs;
using Shared.Enums;
using Xunit;

namespace SportsStore.Tests
{
    public class OrderManagementTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // --- CheckoutOrderCommand ---

        [Fact]
        public async Task CheckoutOrder_Creates_Order_And_Returns_Id()
        {
            var db = CreateInMemoryDb();
            var bus = new Mock<IPublishEndpoint>();
            var logger = new Mock<ILogger<CheckoutOrderCommandHandler>>();
            var handler = new CheckoutOrderCommandHandler(db, bus.Object, logger.Object);

            var items = new List<CartItemDto>
            {
                new CartItemDto { ProductId = 1, ProductName = "Kayak", Quantity = 2, UnitPrice = 275 }
            };
            var command = new CheckoutOrderCommand(Guid.NewGuid(), items);

            var orderId = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, orderId);
            var order = await db.Orders.FindAsync(orderId);
            Assert.NotNull(order);
            Assert.Equal(OrderStatus.Submitted, order.Status);
        }

        [Fact]
        public async Task CheckoutOrder_Publishes_OrderSubmitted_Event()
        {
            var db = CreateInMemoryDb();
            var bus = new Mock<IPublishEndpoint>();
            var logger = new Mock<ILogger<CheckoutOrderCommandHandler>>();
            var handler = new CheckoutOrderCommandHandler(db, bus.Object, logger.Object);

            var items = new List<CartItemDto>
            {
                new CartItemDto { ProductId = 1, ProductName = "Kayak", Quantity = 1, UnitPrice = 275 }
            };
            var command = new CheckoutOrderCommand(Guid.NewGuid(), items);

            await handler.Handle(command, CancellationToken.None);

            bus.Verify(b => b.Publish(
                It.IsAny<Shared.Messages.OrderSubmitted>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- CancelOrderCommand ---

        [Fact]
        public async Task CancelOrder_Returns_False_When_Order_Not_Found()
        {
            var db = CreateInMemoryDb();
            var logger = new Mock<ILogger<CancelOrderCommandHandler>>();
            var handler = new CancelOrderCommandHandler(db, logger.Object);

            var result = await handler.Handle(
                new CancelOrderCommand(Guid.NewGuid()), CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task CancelOrder_Sets_Status_To_Failed()
        {
            var db = CreateInMemoryDb();
            var order = new Order { Status = OrderStatus.Submitted };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var logger = new Mock<ILogger<CancelOrderCommandHandler>>();
            var handler = new CancelOrderCommandHandler(db, logger.Object);

            var result = await handler.Handle(
                new CancelOrderCommand(order.Id), CancellationToken.None);

            Assert.True(result);
            var updated = await db.Orders.FindAsync(order.Id);
            Assert.Equal(OrderStatus.Failed, updated!.Status);
        }

        [Fact]
        public async Task CancelOrder_Cannot_Cancel_Completed_Order()
        {
            var db = CreateInMemoryDb();
            var order = new Order { Status = OrderStatus.Completed };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var logger = new Mock<ILogger<CancelOrderCommandHandler>>();
            var handler = new CancelOrderCommandHandler(db, logger.Object);

            var result = await handler.Handle(
                new CancelOrderCommand(order.Id), CancellationToken.None);

            Assert.False(result);
        }

        // --- GetOrderByIdQuery ---

        [Fact]
        public async Task GetOrderById_Returns_Null_When_Not_Found()
        {
            var db = CreateInMemoryDb();
            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetOrderByIdQueryHandler(db, mapper);

            var result = await handler.Handle(
                new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderById_Returns_Order_When_Found()
        {
            var db = CreateInMemoryDb();
            var order = new Order
            {
                Status = OrderStatus.Submitted,
                TotalAmount = 100,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, ProductName = "Kayak", Quantity = 1, UnitPrice = 100 }
                }
            };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var mapper = AutoMapperHelper.CreateMapper();
            var handler = new GetOrderByIdQueryHandler(db, mapper);

            var result = await handler.Handle(
                new GetOrderByIdQuery(order.Id), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result!.Id);
        }

        // --- GetDashboardSummaryQuery ---

        [Fact]
        public async Task GetDashboardSummary_Returns_Correct_Counts()
        {
            var db = CreateInMemoryDb();
            db.Orders.AddRange(
                new Order { Status = OrderStatus.Completed, TotalAmount = 100 },
                new Order { Status = OrderStatus.Completed, TotalAmount = 200 },
                new Order { Status = OrderStatus.PaymentFailed, TotalAmount = 50 },
                new Order { Status = OrderStatus.Submitted, TotalAmount = 75 }
            );
            await db.SaveChangesAsync();

            var logger = new Mock<ILogger<GetDashboardSummaryQueryHandler>>();
            var handler = new GetDashboardSummaryQueryHandler(db);

            var result = await handler.Handle(
                new GetDashboardSummaryQuery(), CancellationToken.None);

            Assert.Equal(4, result.TotalOrders);
            Assert.Equal(2, result.CompletedOrders);
            Assert.Equal(1, result.FailedOrders);
            Assert.Equal(1, result.PendingOrders);
            Assert.Equal(300, result.TotalRevenue);
        }

        // --- UpdateOrderStatusCommand ---

        [Fact]
        public async Task UpdateOrderStatus_Changes_Status_Correctly()
        {
            var db = CreateInMemoryDb();
            var order = new Order { Status = OrderStatus.Submitted };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var logger = new Mock<ILogger<UpdateOrderStatusCommandHandler>>();
            var handler = new UpdateOrderStatusCommandHandler(db, logger.Object);

            await handler.Handle(
                new UpdateOrderStatusCommand(order.Id, OrderStatus.InventoryConfirmed),
                CancellationToken.None);

            var updated = await db.Orders.FindAsync(order.Id);
            Assert.Equal(OrderStatus.InventoryConfirmed, updated!.Status);
        }
    }

    public static class AutoMapperHelper
    {
        public static AutoMapper.IMapper CreateMapper()
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrderManagement.API.Application.Mappings.OrderMappingProfile>();
            });
            return config.CreateMapper();
        }
    }
}


