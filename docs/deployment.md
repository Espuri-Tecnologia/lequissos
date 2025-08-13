# Deployment Guide

## Kit Product Queue Configuration

`SqsEventDispatcher` resolves queue URLs from the `AWS:SQSQueues` configuration section. To enable processing of kit product creation events, define the `ProdutosKits` queue in your configuration or environment variables.

### appsettings.json example

```json
"AWS": {
  "SQSQueues": {
    "ProdutosKits": "queue/produtokit-sync-varejoonline-dev.fifo"
  }
}
```

### Environment variable example

```
AWS__SQSQueues__ProdutosKits=queue/produtokit-sync-varejoonline-dev.fifo
```

Ensure this queue exists and is accessible by the application at deployment time.
