# SportsStore — Distributed Order Processing

This started as the standard SportsStore MVC app from the Pro ASP.NET Core textbook. The task was to extend it into something that actually resembles how real e-commerce platforms handle orders — separate services, async messaging, the whole thing.

## What it does

When a customer checks out on the SportsStore front-end, instead of just saving an order to a database, the system kicks off a chain of events:

1. The checkout hits the Order API, which saves the order and fires an event to RabbitMQ
2. The Inventory Service picks that up, checks if stock is available, and responds
3. If inventory is confirmed, the Payment Service processes the payment
4. If payment goes through, the Shipping Service creates a shipment with a tracking reference
5. The order ends up as Completed — or Failed at whatever step went wrong

The whole thing is observable through Seq (structured logs from every service) and manageable through a React admin dashboard.

## Services

| Service | What it does | Port |
|---------|-------------|------|
| SportsStore | Customer-facing shop (Razor Pages) | 5000 |
| OrderManagement.API | Central API, owns the order state | 5250 |
| InventoryService | Validates stock availability | — |
| PaymentService | Simulates payment processing | — |
| ShippingService | Creates shipments, generates tracking refs | — |
| Admin Dashboard | React UI for order management | 3000 |
| RabbitMQ | Message broker | 5672 / 15672 |
| Seq | Log aggregation | 5341 |

## Running it

The easy way (Docker):

    docker-compose up --build

For development (Visual Studio + separate React terminal), make sure RabbitMQ is running first:

    docker start rabbitmq

Then F5 in Visual Studio with multiple startup projects: OrderManagement.API, InventoryService, PaymentService, ShippingService, SportsStore.

And in a separate terminal:

    cd src/admin-dashboard
    npm start

## API

    POST   /api/orders/checkout          Place a new order
    GET    /api/orders                   List all orders
    GET    /api/orders/{id}              Get order by ID
    GET    /api/orders/{id}/status       Get just the status
    GET    /api/orders/filter/{status}   Filter by status
    GET    /api/orders/dashboard/summary Stats for the dashboard
    GET    /api/customers/{id}/orders    Orders for a specific customer
    DELETE /api/orders/{id}              Cancel an order

## Tech decisions worth noting

SQLite instead of SQL Server — keeps Docker setup simple, no separate DB container needed.

MassTransit 8.2.3 — newer versions require a commercial license, this one does not.

AutoMapper 12.0.1 — pinned because 13.x broke the Extensions package compatibility.

Separate consumer classes per message type — MassTransit only creates a queue for the first message type when you implement multiple interfaces in one class. Learned that the hard way.

EnsureDeleted + EnsureCreated on startup — avoids migration conflicts during development. Not something you would do in production.

## Payment simulation

The PaymentService rejects about 20% of payments randomly, plus a hardcoded list of test card numbers that always fail. This is just to demonstrate the failure path works end-to-end.

## Logs

Every service writes structured logs to Seq at http://localhost:5341. Useful filters:

    OrderId = 'your-order-id-here'    -- Follow a specific order
    EventType = 'PaymentRejected'     -- See all payment failures
    @Level = 'Error'                  -- See what is breaking

## Tests

37 tests covering the CQRS handlers, query handlers, and RabbitMQ consumers. Run with:

    dotnet test
