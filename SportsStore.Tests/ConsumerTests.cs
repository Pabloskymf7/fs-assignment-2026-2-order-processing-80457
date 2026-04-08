using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using InventoryService.Consumers;
using PaymentService.Consumers;
using Shared.Messages;
using Xunit;

namespace SportsStore.Tests
{
    public class InventoryConsumerTests
    {
        [Fact]
        public async Task OrderSubmitted_Consumer_Processes_Message()
        {
            var logger = new Mock<ILogger<OrderSubmittedConsumer>>();
            var bus = new Mock<IPublishEndpoint>();
            var consumer = new OrderSubmittedConsumer(logger.Object, bus.Object);

            var message = new OrderSubmitted(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<OrderItemMessage>
                {
                    new OrderItemMessage(1, "Kayak", 1, 275)
                },
                275,
                DateTime.UtcNow
            );

            var context = new Mock<ConsumeContext<OrderSubmitted>>();
            context.Setup(c => c.Message).Returns(message);

            await consumer.Consume(context.Object);

            Assert.True(bus.Invocations.Count >= 1);
        }

        [Fact]
        public async Task OrderSubmitted_Publishes_InventoryConfirmed_Or_Failed()
        {
            var logger = new Mock<ILogger<OrderSubmittedConsumer>>();
            var bus = new Mock<IPublishEndpoint>();
            var consumer = new OrderSubmittedConsumer(logger.Object, bus.Object);

            var message = new OrderSubmitted(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<OrderItemMessage>
                {
                    new OrderItemMessage(2, "Soccer Ball", 3, 19.50m)
                },
                58.50m,
                DateTime.UtcNow
            );

            var context = new Mock<ConsumeContext<OrderSubmitted>>();
            context.Setup(c => c.Message).Returns(message);

            await consumer.Consume(context.Object);

            var confirmedCalls = bus.Invocations
                .Count(i => i.Arguments.Any(a => a is InventoryConfirmed));
            var failedCalls = bus.Invocations
                .Count(i => i.Arguments.Any(a => a is InventoryFailed));

            Assert.True(confirmedCalls + failedCalls >= 1);
        }
    }

    public class PaymentConsumerTests
    {
        [Fact]
        public async Task InventoryConfirmed_Consumer_Processes_Message()
        {
            var logger = new Mock<ILogger<InventoryConfirmedConsumer>>();
            var bus = new Mock<IPublishEndpoint>();
            var consumer = new InventoryConfirmedConsumer(logger.Object, bus.Object);

            var message = new InventoryConfirmed(Guid.NewGuid());

            var context = new Mock<ConsumeContext<InventoryConfirmed>>();
            context.Setup(c => c.Message).Returns(message);

            await consumer.Consume(context.Object);

            Assert.True(bus.Invocations.Count >= 1);
        }

        [Fact]
        public async Task InventoryConfirmed_Publishes_PaymentApproved_Or_Rejected()
        {
            var logger = new Mock<ILogger<InventoryConfirmedConsumer>>();
            var bus = new Mock<IPublishEndpoint>();
            var consumer = new InventoryConfirmedConsumer(logger.Object, bus.Object);

            var message = new InventoryConfirmed(Guid.NewGuid());

            var context = new Mock<ConsumeContext<InventoryConfirmed>>();
            context.Setup(c => c.Message).Returns(message);

            await consumer.Consume(context.Object);

            var approvedCalls = bus.Invocations
                .Count(i => i.Arguments.Any(a => a is PaymentApproved));
            var rejectedCalls = bus.Invocations
                .Count(i => i.Arguments.Any(a => a is PaymentRejected));

            Assert.True(approvedCalls + rejectedCalls >= 1);
        }
    }
}
