# 🐇 Microservices Order System (RabbitMQ + .NET 8)

A distributed order management system demonstrating asynchronous communication between microservices using **RabbitMQ** and **Docker Compose**.

---

## 🏗 System Architecture
* **Producer (ProducerEX1rabbitMQ):** Entry point for order creation. It persists data locally and publishes messages to a RabbitMQ exchange.
* **Consumer (ConsumerEx1RabbitMQ):** Subscribes to the RabbitMQ queue, processes incoming orders, and maintains a global order repository.
* **Message Broker:** RabbitMQ (Management-enabled).

---

## 🚀 Getting Started

### Prerequisites
* Docker Desktop
* Postman (for testing)

### Deployment
Run the following command in the root directory to build and start the infrastructure:

```bash
docker-compose up --build
```

### 🛠 API Reference & Testing (Postman)
1. Create a New Order
Producer (ProducerEX1rabbitMQ) - Port 5000

Method: POST

URL: http://localhost:5000/api/Cart/create-order

Body (JSON):

JSON
{
    "orderId": "ORD-123",
    "itemsNum": 10
}
Constraints: orderId must be a non-empty string; itemsNum must be an integer greater than 0.

2. Get Order Details
Consumer (ConsumerEx1RabbitMQ) - Port 5001

Method: GET

URL: http://localhost:5001/api/Order/order-details?orderId=ORD-123

Description: Retrieves the processed order details from the consumer's in-memory dictionary. Use the orderId sent in the original POST request.

### 📡 Infrastructure Declaration & Rationale
Exchange Type: Direct
Rationale: I chose a Direct Exchange because it routes messages based on an exact match between the message's routing key and the binding key of the queue.

Benefit: In this e-commerce application, it ensures that specific order tasks are handled only by the intended consumer services, providing high efficiency and clear routing logic.

Consumer Binding Key
Binding Key: "MyKey"

Purpose: The consumer uses this key to subscribe to the specific "Order Creation" message stream from the exchange. This prevents the consumer from processing unrelated messages within the system.

Shared Responsibility
Responsible Service: Both the Producer and Consumer.

Rationale: While the Producer declares the exchange and queue to ensure the "plumbing" exists before publishing, the Consumer also declares them upon startup. This prevents data loss (black holes) regardless of which service starts first and ensures the system is resilient even if RabbitMQ is restarted.

### 🛡️ Error Handling & Resilience
1. Connection Errors / Broker Unavailability
Retry Mechanism: Both Producer and Consumer implement a retry mechanism with exponential backoff to ensure transient connection issues do not result in message loss.

Startup Delays: Services are configured with health checks and staggered startup times to ensure RabbitMQ is fully operational before processing messages.

2. Health Checks & Readiness
Infrastructure Checks: Health checks for RabbitMQ, ProducerEX1rabbitMQ, and ConsumerEx1RabbitMQ ensure that dependent services do not start until their dependencies are fully functional.

Service Verification: Both services use HTTP-based checks (via curl to their /health endpoints) to confirm operational readiness before accepting traffic.

### 📄 Program Names
Producer Project: ProducerEX1rabbitMQ

Consumer Project: ConsumerEx1RabbitMQ
