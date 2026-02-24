using System;
using System.Collections.Generic;

namespace ProducerService_Exercise1.Models;

public class Order
{
	public string OrderId { get; set; }

	public string CustomerId { get; set; }

	public string Currency { get; set; }

	public string Status { get; set; }

	public DateTime CreatedAt { get; set; }

	public decimal TotalAmount { get; set; }

	public List<OrderItem> Items { get; set; }
}
