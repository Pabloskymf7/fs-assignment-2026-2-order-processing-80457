using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Application.Commands;
using OrderManagement.API.Application.Queries;
using Shared.DTOs;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var orderId = await _mediator.Send(
            new CheckoutOrderCommand(request.CustomerId, request.Items));

        return CreatedAtAction(nameof(GetById), new { id = orderId }, new { orderId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _mediator.Send(new GetOrdersQuery());
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order is null) return NotFound();
        return Ok(new { order.Id, order.Status });
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var orders = await _mediator.Send(new GetCustomerOrdersQuery(customerId));
        return Ok(orders);
    }
}