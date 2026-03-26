using AutoMapper;
using OrderManagement.API.Domain.Entities;
using Shared.DTOs;

namespace OrderManagement.API.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<PaymentRecord, PaymentDto>();

        CreateMap<ShipmentRecord, ShipmentDto>();
    }
}