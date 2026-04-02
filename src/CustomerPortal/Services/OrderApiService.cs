using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.DTOs;

namespace CustomerPortal.Services;

public class OrderApiService
{
    private readonly HttpClient _http;

    public OrderApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await _http.GetFromJsonAsync<List<OrderDto>>("api/Orders") ?? new();
    }

    public async Task<OrderDto?> GetOrderAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<OrderDto>($"api/Orders/{id}");
    }

    public async Task<List<OrderDto>> GetCustomerOrdersAsync(Guid customerId)
    {
        return await _http.GetFromJsonAsync<List<OrderDto>>(
            $"api/Orders/customer/{customerId}") ?? new();
    }

    public async Task<Guid?> CheckoutAsync(CheckoutRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Orders/checkout", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
        return result?.OrderId;
    }
}

public class CheckoutResponse
{
    public Guid OrderId { get; set; }
}