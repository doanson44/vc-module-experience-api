# X-Purchase Order

X-Purchase-Order provides high performance API for order data.

## Key features

- Getting and searching orders;
- Basic order workflow operations;

## QueryRoot
### Queries

|#|Endpoint|Arguments|Returns|
|------|---------|---------|---------|
|1|[order](#order-query)|`id` `number` `userId`|Order|
|2|[orders](#orders-connection)|`filter` `sort` `language` `userId`|Paginated order list|

### Mutations

|# |Endpoint|Arguments|Description|
|--|-----------------------|---------------------|---------|
|1 |[createOrderFromCart](#createOrderFromCart)|`!cartId`|Create order from an existing cart.|
|2 |[changeOrderStatus](#changeOrderStatus)|`!orderId` `!status`|Changes order status.|
|3 |[confirmOrderPayment](#confirmOrderPayment)|`payment { id sum caurrency …}`|Confirms order payment.|
|4 |[cancelOrderPayment](#cancelOrderPayment)|`payment { id sum caurrency …}`|Cancels order payment.|
|5 |[updateOrderDynamicProperties](#updateOrderDynamicProperties)|`!dynamicProperties`|Updates dynamic properties in order.|
|6 |[updateOrderItemDynamicProperties](#updateOrderItemDynamicProperties)|`!lineItemId` `!dynamicProperties`|Updates dynamic properties in order items.|
|7 |[updateOrderShipmentDynamicProperties](#updateOrderShipmentDynamicProperties)|`!shipmentId` `!dynamicProperties`|Updates dynamic properties in order shipment.|
|8 |[updateOrderPaymentDynamicProperties](#updateOrderPaymentDynamicProperties)|`!paymentId` `!dynamicProperties`|Updates dynamic properties in order payment.|

> [!NOTE]
> In arguments column we show additional arguments. if they are marked with an exclamation mark, they are required.

## Queriable objects
### Order main types schema

![OrderType schema structure](./media/OrderMainTypes.png)

## Examples

In this block you can find some examples of queries and mutations.

### Order query

```
{
  order(
    number: "CU1508131823002"
    userId: "0cda0396-43fe-4034-a20e-d0bab4c88c93"
  ) {
    id
    customerId
    customerName
    createdDate
    dynamicProperties { name value valueType }
    addresses {
      postalCode
    }
    currency {
      code
    }
    items {
      sku
      name
      quantity
    }
    total {
      amount
    }
    cancelledDate
  }
}
```

<details>
<summary>Example result (click to expand)</summary>

```
{
  "data": {
    "order": {
      "id": "9d27c868-2e31-4ab4-861b-909bc3f86657",
      "customerId": "0cda0396-43fe-4034-a20e-d0bab4c88c93",
      "customerName": "George Basker",
      "createdDate": "2019-01-06",
      "addresses": [
        {
          "postalCode": "77462"
        }
      ],
      "currency": {
        "code": "EUR"
      },
      "items": [
        {
          "sku": "PTO-38363811",
          "name": "Laced In Love White Floral Prom Dress",
          "quantity": 1
        },
        {
          "sku": "QRY-61202734",
          "name": "Dark Blue Floral Print Twist Cut Out Back Dress",
          "quantity": 1
        }
      ],
      "total": {
        "amount": 62.99
      },
      "cancelledDate": null
    }
  },
  "extensions": {}
}
```
</details>

</p>
> [!TIP]
> See OrderType schema for better understanding of possible fields in request.

### Orders connection
With this connection you can get all user's orders.
```
{
  orders(
    after: "0"
    first: 10
    sort: "createdDate:desc"
    language: "en-US"
    userId: "0cda0396-43fe-4034-a20e-d0bab4c88c93"
  ) {
    totalCount
    items {
      id
      customerId
      customerName
      createdDate
      addresses {
        postalCode
      }
      currency {
        code
      }
      items {
        sku
        name
        quantity
      }
      total {
        amount
      }
      cancelledDate
    }
  }
}

```
<details>
<summary>Example result (click to expand)</summary>

```
{
  "data": {
    "orders": {
      "totalCount": 3,
      "items": [
        {
          "id": "11a6d4a0-284f-46b1-8e17-add55983353f",
          "customerId": "0cda0396-43fe-4034-a20e-d0bab4c88c93",
          "customerName": "George Basker",
          "createdDate": "2019-01-06",
          "addresses": [
            {
              "postalCode": "77462"
            }
          ],
          "currency": {
            "code": "EUR"
          },
          "items": [
            {
              "sku": "PTO-38363811",
              "name": "Laced In Love White Floral Prom Dress",
              "quantity": 1
            },
            {
              "sku": "EIQ-20582301",
              "name": "Burgundy Baroque Lace Waist Dress",
              "quantity": 2
            },
            {
              "sku": "334713255",
              "name": "Wide Fit Lilac Ankle Strap Straw Wedges",
              "quantity": 1
            }
          ],
          "total": {
            "amount": 106.98
          },
          "cancelledDate": null
        }
      ]
    }
  },
  "extensions": {}
}
```
</details>

</p>

### CreateOrderFromCart

This mutation creates an order from the cart with given id.

#### Query

```
mutation {
  createOrderFromCart(
    command: { cartId: "05479fa6-9b6f-4028-94b1-cda21447e268" }
  )
  {
    id
    items {
      id
      sku
      name
    }
    total {
      amount
    }
  }
}
```
### ChangeOrderStatus

This mutation changes order status.

#### Query

```
mutation {
  changeOrderStatus(
    command: { orderId: "1672428e-52fe-4092-8380-7604c3637f91" status: "Approved"}
  )
}

```

### ConfirmOrderPayment

This mutation confirms order payment.

#### Query

```
mutation {
  confirmOrderPayment(
    command: {
      payment: {
        orderId: "9d27c868-2e31-4ab4-861b-909bc3f86657"
        operationType: "PaymentIn"
        number: "PA1508131823002"
        isApproved: false
        status: "Authorized"
        comment: null
        isCancelled: false
        customerId: "0cda0396-43fe-4034-a20e-d0bab4c88c93"
        sum: 100
        currency: "USD"
        taxDetails: { name: "State tax", amount: 10, rate: 0.1 }
        taxTotal: 10
        discounts: { discountAmount: 11, discountAmountWithTax: 11, currency: "USD" }
      }
    }
  )
}

```

### CancelOrderPayment

This mutation cancels order payment.

#### Query

```
mutation {
  cancelOrderPayment(
    command: {
      payment: {
        orderId: "9d27c868-2e31-4ab4-861b-909bc3f86657"
        operationType: "PaymentIn"
        number: "PA1508131823002"
        isApproved: false
        status: "Authorized"
        comment: null
        isCancelled: false
        customerId: "0cda0396-43fe-4034-a20e-d0bab4c88c93"
        sum: 100
        currency: "USD"
        taxDetails: { name: "State tax", amount: 10, rate: 0.1 }
        taxTotal: 10
        discounts: { discountAmount: 11, discountAmountWithTax: 11, currency: "USD" }
      }
    }
  )
}

```

### updateOrderDynamicProperties

This mutation updates dynamic properties in order

#### Query

```
mutation ($command: InputUpdateOrderDynamicPropertiesType!)
{
    updateOrderDynamicProperties(command: $command)
    {
        dynamicProperties
        {
            name
            value
            valueType
            dictionaryItem
            {
                label
                name
                id
            }
        }
    }
}
```

#### Variables

```
"command": {
    "orderId": "d548c750-5a74-4e54-b72b-f5209f44caa6",
    "dynamicProperties": [
        {
            "name": "Example string property",
            "value": "12345678"
        },
        {
            "name": "Example multilanguage property",
            "locale":"de-DE",
            "value": "hallo welt"
        },
        {
            "name": "Example dictionary property",
            "value": "578fadeb1d2a40b3b08b1daf8db09463"
        }
  	]
  }
}
```

### updateOrderItemDynamicProperties

This mutation updates dynamic properties in order item

#### Query

```
mutation ($command: InputUpdateOrderItemDynamicPropertiesType!)
{
    updateOrderItemDynamicProperties(command: $command)
    {
        items
        {
            id
            dynamicProperties
            {
                name
                value
                valueType
                dictionaryItem
                {
                    label
                    name
                    id
                }
            }
        }
    }
}
```

#### Variables

```
"command": {
    "orderId": "d548c750-5a74-4e54-b72b-f5209f44caa6",
    "lineItemId": "dab09410-aa1a-4daf-8a32-4e41abee77b8",
    "dynamicProperties": [
        {
            "name": "Example string property",
            "value": "12345678"
        },
        {
            "name": "Example multilanguage property",
            "locale":"de-DE",
            "value": "hallo welt"
        },
        {
            "name": "Example dictionary property",
            "value": "578fadeb1d2a40b3b08b1daf8db09463"
        }
  	]
  }
}
```

### updateOrderShipmentDynamicProperties

This mutation updates dynamic properties in order shipment

#### Query

```
mutation ($command: InputUpdateOrderShipmentDynamicPropertiesType!)
{
    updateOrderShipmentDynamicProperties(command: $command)
    {
        shipments
        {
            id
            dynamicProperties
            {
                name
                value
                valueType
                dictionaryItem
                {
                    label
                    name
                    id
                }
            }
        }
    }
}
```

#### Variables:

```
"command": {
    "orderId": "d548c750-5a74-4e54-b72b-f5209f44caa6",
    "shipmentId": "79b8f095-9740-4353-998b-e1c4dd577ee6",
    "dynamicProperties": [
        {
            "name": "Example string property",
            "value": "12345678"
        },
        {
            "name": "Example multilanguage property",
            "locale":"de-DE",
            "value": "hallo welt"
        },
        {
            "name": "Example dictionary property",
            "value": "578fadeb1d2a40b3b08b1daf8db09463"
        }
  	]
  }
}
```

### updateOrderPaymentDynamicProperties

This mutation updates dynamic properties in order payment

#### Query

```
mutation ($command: InputUpdateOrderPaymentDynamicPropertiesType!)
{
    updateOrderPaymentDynamicProperties(command: $command)
    {
        inPayments
        {
            id
            dynamicProperties
            {
                name
                value
                valueType
                dictionaryItem
                {
                    label
                    name
                    id
                }
            }
        }
    }
}
```

#### Variables

```
"command": {
    "orderId": "d548c750-5a74-4e54-b72b-f5209f44caa6",
    "paymentId": "0859f1e8-16e8-4924-808b-47e03560085d",
    "dynamicProperties": [
        {
            "name": "Example string property",
            "value": "12345678"
        },
        {
            "name": "Example multilanguage property",
            "locale":"de-DE",
            "value": "hallo welt"
        },
        {
            "name": "Example dictionary property",
            "value": "578fadeb1d2a40b3b08b1daf8db09463"
        }
  	]
  }
}
```
