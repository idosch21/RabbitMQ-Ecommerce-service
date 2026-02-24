API Endpoints
Producer Service
URL: http://localhost:5000/api/Cart/create-order

Method: POST

Request Body Format:

JSON
{
    "orderId": "string",
    "itemsNum": 10
}
Constraints: orderId must be a non-empty string; itemsNum must be an integer greater than 0.

Consumer Service
URL: http://localhost:5001/api/Order/order-details?orderId=YOUR_ID

Method: GET

Description: Retrieves the order details from the in-memory dictionary. Use the orderId sent in the original POST request.

Exchange Type and Rationale
Exchange Type: Direct

Rationale: I chose a Direct Exchange because it routes messages based on an exact match between the message's routing key and the binding key of the queue. In this e-commerce application, it ensures that specific order tasks are handled only by the intended consumer services, providing high efficiency and clear routing logic.

Consumer Binding Key
Binding Key: "MyKey"

Purpose: The consumer uses this key to subscribe to the specific "Order Creation" message stream from the exchange. This prevents the consumer from processing unrelated messages within the system.

Infrastructure Declaration
Responsible Service: Both the Producer and Consumer

Rationale: While the Producer declares the exchange and queue to ensure the "plumbing" exists before publishing, the Consumer also declares them upon startup. This shared responsibility prevents data loss (black holes) regardless of which service starts first and ensures the system is resilient even if RabbitMQ is restarted.