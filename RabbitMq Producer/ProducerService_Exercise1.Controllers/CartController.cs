using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProducerService_Exercise1.Connection;
using ProducerService_Exercise1.Models;

namespace ProducerService_Exercise1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
	private readonly IMessageProducer _rabbitMQProducer;

	public CartController(IMessageProducer rabbitMQProducer)
	{
		_rabbitMQProducer = rabbitMQProducer;
	}

	[HttpPost("create-order")]
	public IActionResult CreateOrder([FromBody] OrderRequest request)
	{
		if (!base.ModelState.IsValid)
		{
			return BadRequest(base.ModelState);
		}
		if (string.IsNullOrEmpty(request.OrderId) || request.ItemsNum <= 0)
		{
			return BadRequest("Invalid order details");
		}
		Order order = GenerateOrder(request.OrderId, request.ItemsNum);
		_rabbitMQProducer.PublishMessage(order);
		return Ok(new
		{
			Message = "Order created successfully",
			OrderId = order.OrderId
		});
	}

	private Order GenerateOrder(string orderId, int itemsNum)
	{
		Random random = new Random();
		Order order = new Order
		{
			OrderId = orderId,
			CustomerId = $"Customer{random.Next(1000, 9999)}",
			Currency = "USD",
			Status = "new",
			CreatedAt = DateTime.UtcNow,
			Items = new List<OrderItem>()
		};
		decimal totalAmount = default(decimal);
		for (int i = 0; i < itemsNum; i++)
		{
			OrderItem item = new OrderItem
			{
				ProductId = $"PROD-{random.Next(100, 999)}",
				Quantity = random.Next(1, 5),
				Price = (decimal)random.Next(10, 1000) / 10m
			};
			order.Items.Add(item);
			totalAmount += (decimal)item.Quantity * item.Price;
		}
		order.TotalAmount = totalAmount;
		return order;
	}
}
