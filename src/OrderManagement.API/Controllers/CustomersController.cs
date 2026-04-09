using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Application.Queries;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{customerId:guid}/orders")]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId)
    {
        var orders = await _mediator.Send(new GetCustomerOrdersQuery(customerId));
        return Ok(orders);
    }
}
