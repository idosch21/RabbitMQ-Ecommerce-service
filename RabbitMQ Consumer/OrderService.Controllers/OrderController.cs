using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
	private static readonly Dictionary<string, Order> _orders = new Dictionary<string, Order>();

	[HttpGet("order-details")]
	public IActionResult GetOrderDetails(string orderId)
	{
		if (_orders.TryGetValue(orderId, out Order order))
		{
			return Ok(order);
		}
		return NotFound(new
		{
			Message = "Order not found"
		});
	}

	public static void StoreOrder(Order order)
	{
		_orders[order.OrderId] = order;
	}
}
