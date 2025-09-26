# Webhooks do LexosHub VarejOnline

Este documento descreve como o LexosHub integra-se com o ERP VarejOnline através de webhooks: quais eventos são assinados automaticamente, como a API do hub expõe os endpoints públicos e qual o formato esperado das notificações.

## Registro automático de webhooks

Durante o processo de sincronização inicial do hub, o evento `RegisterDefaultWebhooks` é disparado. O *handler* `RegisterDefaultWebhooksEventHandler` registra automaticamente, para cada hub, três eventos padrão do ERP VarejOnline (`PRODUTOS`, `TABELAPRECOPRODUTO` e `NOTAFISCAL`). Cada evento é cadastrado aceitando os métodos `POST` e `PUT`, com a URL padronizada `https://api-varejoonline.lexoshub.com/{hubKey}/{evento}`.【F:docs/event-handlers-flow.md†L49-L55】【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Webhook/RegisterDefaultWebhooksEventHandler.cs†L21-L37】

O registro usa o serviço de domínio `WebhookService`. Ele consulta o token da integração correspondente ao `hubKey`, monta o `WebhookRequest` com o evento, URL e métodos configurados e chama o serviço remoto do ERP via `RegisterWebhookAsync`. Quando o ERP confirma a criação (retornando o `IdRecurso`), o LexosHub persiste o *webhook* em sua base de dados.【F:src/LexosHub.ERP.VarejOnline.Domain/Services/WebhookService.cs†L19-L70】

## Endpoints expostos pelo LexosHub

A API pública do LexosHub expõe os seguintes endpoints relacionados a webhooks, todos sob o prefixo `/api/webhook`:

| Método | Rota | Descrição |
| ------ | ---- | --------- |
| `POST` | `/api/webhook` | Permite registrar manualmente um webhook informando `hubKey`, `event`, `types` e `url`. O payload é encaminhado ao `WebhookService.RegisterAsync`. |
| `POST` | `/api/webhook/{hubKey}` | Endpoint utilitário para solicitar a sincronização completa de produtos (`ProductsRequested`). |
| `POST` | `/api/webhook/{hubKey}/produto` | Recebe notificações de produtos. Extrai o `productId` do campo `object` e dispara `ProductsRequested` com o identificador informado. |
| `POST` | `/api/webhook/{hubKey}/tabela-preco` | Recebe notificações de tabela de preço. Extrai `tabelaPrecoId` do campo `object` e dispara `PriceTablesRequested`. |
| `POST` | `/api/webhook/{hubKey}/nota-fiscal` | Recebe notificações de notas fiscais. Extrai `erpNotaFiscalId` do campo `object` e dispara `InvoicesRequested`. |

Todos os endpoints validam se o payload foi enviado e registram logs de diagnóstico contextualizando o `hubKey` recebido. Erros de validação retornam `400 Bad Request` com a mensagem específica, enquanto notificações processadas retornam `200 OK` com uma mensagem de sucesso.【F:src/LexosHub.ERP.VarejOnline.Api/Controllers/Webhook/WebhookController.cs†L13-L124】

## Estrutura esperada das notificações

O payload das notificações segue o contrato `WebhookNotificationDto`, contendo:

```json
{
  "object": "<URL do recurso no ERP>",
  "webhookEvent": "<identificador do evento>",
  "eventType": "<tipo do evento>",
  "contractId": "<identificador opcional do contrato>"
}
```

O campo `object` deve terminar com o identificador numérico do recurso (ex.: `.../produtos/123`). Esse identificador é usado pelos handlers para emitir os eventos internos correspondentes. Campos ausentes ou valores inválidos resultam em respostas de erro conforme descrito acima.【F:src/LexosHub.ERP.VarejOnline.Domain/DTOs/Produto/WebhookNotificationDto.cs†L6-L12】【F:src/LexosHub.ERP.VarejOnline.Api/Controllers/Webhook/WebhookController.cs†L52-L123】

## Persistência dos webhooks

Ao concluir o registro junto ao ERP, o LexosHub salva um `WebhookRecordDto` contendo `IntegrationId`, `Uuid` retornado pelo ERP, `Event`, `Types` e `Url`. Esse registro é persistido pelo `WebhookRepository`, que insere os dados na tabela `[Webhook]`. A entidade inclui colunas de auditoria (`CreatedDate`, `UpdatedDate`).【F:src/LexosHub.ERP.VarejOnline.Domain/DTOs/Webhook/WebhookRecordDto.cs†L5-L16】【F:src/LexosHub.ERP.VarejOnline.Infra.Data/Repositories/Webhook/WebhookRepository.cs†L9-L41】【F:src/LexosHub.ERP.VarejOnline.Infra.Data.Migrations/Migrations/20250626120000_WebhookTable.cs†L12-L31】

## Fluxo resumido

1. A sincronização inicial dispara `RegisterDefaultWebhooks` para o `hubKey` configurado.
2. O `RegisterDefaultWebhooksEventHandler` registra os webhooks padrão e chama `WebhookService.RegisterAsync` para cada evento.
3. O `WebhookService` registra o webhook no ERP, valida a resposta e persiste os dados locais.
4. O ERP passa a chamar os endpoints públicos do LexosHub quando os eventos configurados ocorrerem.
5. Cada endpoint valida a notificação, emite os eventos internos (`ProductsRequested`, `PriceTablesRequested`, `InvoicesRequested`) e confirma o processamento via HTTP 200.

Esse fluxo garante que o LexosHub esteja sempre apto a receber notificações do ERP VarejOnline e transformar essas notificações em eventos internos para processamento assíncrono.
