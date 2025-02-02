# Entity_Resolver_SubField_Nullable_EntryField_Nullable_Second_Service_Errors_SubField

## Result

```json
{
  "errors": [
    {
      "message": "Unexpected Execution Error",
      "locations": [
        {
          "line": 6,
          "column": 5
        }
      ],
      "path": [
        "productById",
        "score"
      ]
    }
  ],
  "data": {
    "productById": {
      "id": "1",
      "name": "string",
      "price": 123.456,
      "score": null
    }
  }
}
```

## Request

```graphql
{
  productById(id: "1") {
    id
    name
    price
    score
  }
}
```

## QueryPlan Hash

```text
1187C75DB20A2D54D1EDC1F31D46DA85C597E294
```

## QueryPlan

```json
{
  "document": "{ productById(id: \u00221\u0022) { id name price score } }",
  "rootNode": {
    "type": "Sequence",
    "nodes": [
      {
        "type": "Parallel",
        "nodes": [
          {
            "type": "Resolve",
            "subgraph": "Subgraph_1",
            "document": "query fetch_productById_1 { productById(id: \u00221\u0022) { id name price } }",
            "selectionSetId": 0
          },
          {
            "type": "Resolve",
            "subgraph": "Subgraph_2",
            "document": "query fetch_productById_2 { productById(id: \u00221\u0022) { score } }",
            "selectionSetId": 0
          }
        ]
      },
      {
        "type": "Compose",
        "selectionSetIds": [
          0
        ]
      }
    ]
  }
}
```

