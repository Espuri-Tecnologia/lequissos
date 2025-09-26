# Fluxo dos Event Handlers

Este documento descreve como cada `EventHandler` do projeto **LexosHub ERP VarejOnline** reage aos eventos recebidos, quais dependências utiliza e quais ações são disparadas em seguida. O objetivo é facilitar a compreensão do fluxo de sincronização entre o Hub e o ERP VarejOnline.

## Visão geral dos encadeamentos

- **Criação de integração**
  - `IntegrationCreatedEventHandler` registra/atualiza a integração e dispara o evento `InitialSync`.
  - `InitialSyncEventHandler` inicia o bootstrap disparando `StoresRequested` e `RegisterDefaultWebhooks`.
  - `StoresRequestedEventHandler` sincroniza lojas e `RegisterDefaultWebhooksEventHandler` registra webhooks padrão.
- **Carga inicial e cadência diária**
  - `CompaniesRequestedEventHandler` prepara o contexto disparando `ProductsRequested` após consultar empresas.
  - `ProductsRequestedEventHandler` pagina produtos, distribuindo o processamento para `CriarProdutosSimples`, `CriarProdutosKits` e `CriarProdutosConfiguraveis`.
  - `PriceTablesRequestedEventHandler` pagina tabelas de preço e aciona `PriceTablePageProcessed` para cada lote.
  - `StocksRequestedEventHandler` pagina estoques enquanto registra o progresso do processo de sincronização.
- **Eventos de pedidos e fiscais**
  - Handlers de pedidos (`OrderCreated`, `OrderShipped`, `OrderDelivered`, `OrderCancelled`) enviam atualizações para a fila **SyncIn**.
  - `InvoicesRequestedEventHandler` obtém XMLs de NF-e e, em fallback, publica uma versão mock para **SyncIn**.

As seções a seguir detalham cada handler.

## Handlers de integração e bootstrap

### IntegrationCreatedEventHandler
- **Evento de entrada:** `IntegrationCreated`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IEventDispatcher`.
- **Fluxo:**
  1. Loga a criação da integração e monta um `HubIntegracaoDto` com os dados do evento.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Integracao/IntegrationCreatedEventHandler.cs†L22-L33】
  2. Persiste/atualiza a integração via `IIntegrationService` e dispara um `InitialSync` para o mesmo `HubKey` usando o `IEventDispatcher`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Integracao/IntegrationCreatedEventHandler.cs†L35-L38】
- **Saídas:** Evento `InitialSync` agendado para o mesmo hub.

### InitialSyncEventHandler
- **Evento de entrada:** `InitialSync`.
- **Dependências:** `ILogger`, `IEventDispatcher`.
- **Fluxo:**
  1. Registra o início da sincronização inicial.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/InitialSyncEventHandler.cs†L18-L21】
  2. Dispara sequencialmente os eventos `StoresRequested` e `RegisterDefaultWebhooks` para o hub.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/InitialSyncEventHandler.cs†L22-L26】
- **Saídas:** Eventos de sincronização de lojas e registro de webhooks.

### StoresRequestedEventHandler
- **Evento de entrada:** `StoresRequested`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IVarejOnlineApiService`, `ISyncOutApiService`.
- **Fluxo:**
  1. Busca a integração para obter o token do ERP e consulta as entidades via API do VarejOnline.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Loja/StoresRequestedEventHandler.cs†L35-L41】
  2. Filtra entidades habilitadas para e-commerce e monta a lista de lojas (`LojaDto`).【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Loja/StoresRequestedEventHandler.cs†L41-L44】
  3. Envia as lojas para o Hub através do `ISyncOutApiService` (`IntegrarLojas`).【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Loja/StoresRequestedEventHandler.cs†L45-L50】
- **Saídas:** Integração das lojas no Hub via API externa.

### RegisterDefaultWebhooksEventHandler
- **Evento de entrada:** `RegisterDefaultWebhooks`.
- **Dependências:** `ILogger`, `IWebhookService`.
- **Fluxo:**
  1. Itera sobre os eventos padrão (`PRODUTOS`, `TABELAPRECOPRODUTO`, `NOTAFISCAL`) e os métodos `POST`/`PUT`.
  2. Para cada combinação, registra o webhook com URL padronizada `https://api-varejoonline.lexoshub.com/{hub}/{evento}` via `IWebhookService`.
  3. O loop garante que cada evento está habilitado para envio e atualização.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Webhook/RegisterDefaultWebhooksEventHandler.cs†L21-L41】
- **Saídas:** Webhooks padrão criados para o hub no serviço de webhooks.

## Handlers de catálogo (empresas, produtos, preços)

### CompaniesRequestedEventHandler
- **Evento de entrada:** `CompaniesRequested`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IVarejOnlineApiService`, `IEventDispatcher`.
- **Fluxo:**
  1. Recupera o token da integração e monta `EmpresaRequest` com os filtros informados.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Loja/CompaniesRequestedEventHandler.cs†L32-L44】
  2. Consulta empresas no ERP Varejo Online e, em seguida, dispara `ProductsRequested` para continuar a sincronização do catálogo.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Loja/CompaniesRequestedEventHandler.cs†L46-L49】
- **Saídas:** Evento `ProductsRequested` encadeado após leitura de empresas.

### ProductsRequestedEventHandler
- **Evento de entrada:** `ProductsRequested`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IVarejOnlineApiService`, `IEventDispatcher`, `IConfiguration`.
- **Fluxo:**
  1. Resolve o token e define paginação usando o tamanho padrão configurado quando necessário.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/ProductsRequestedEventHandler.cs†L33-L45】
  2. Pagina produtos aplicando os filtros do evento; em cada página, separa itens simples, configuráveis e kits.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/ProductsRequestedEventHandler.cs†L53-L96】
  3. Para cada lote de simples, dispara `CriarProdutosSimples`; acumula configuráveis e kits para disparo posterior.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/ProductsRequestedEventHandler.cs†L97-L142】
  4. Ao final, registra log consolidado com totais processados.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/ProductsRequestedEventHandler.cs†L144-L148】
- **Saídas:** Eventos `CriarProdutosSimples`, `CriarProdutosKits` e `CriarProdutosConfiguraveis` conforme a classificação dos produtos.

### CriarProdutosSimplesEventHandler
- **Evento de entrada:** `CriarProdutosSimples`.
- **Dependências:** `ILogger`, `ISqsRepository` (SyncOut), `IOptions<SyncOutConfig>`, `ProdutoViewMapper`.
- **Fluxo:**
  1. Loga a página recebida e mapeia os produtos simples para o modelo de saída do Hub.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosSimplesEventHandler.cs†L30-L44】
  2. Serializa o payload com regras de omissão de valores nulos e envia notificação para a fila FIFO `notificacao-syncout-{hub}` usando `ISqsRepository`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosSimplesEventHandler.cs†L46-L65】
- **Saídas:** Mensagens na fila **SyncOut** contendo produtos simples preparados para o Hub.

### CriarProdutosKitsEventHandler
- **Evento de entrada:** `CriarProdutosKits`.
- **Dependências:** `ILogger`, `ISqsRepository` (SyncOut), `IOptions<SyncOutConfig>`, `ProdutoViewMapper`.
- **Fluxo:**
  1. Loga o lote e mapeia kits com `ProdutoViewMapper.MapKits`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosKitsEventHandler.cs†L33-L46】
  2. Publica o JSON resultante na fila FIFO `notificacao-syncout-{hub}` indicando processo de produto.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosKitsEventHandler.cs†L47-L67】
- **Saídas:** Mensagens **SyncOut** com kits prontos para sincronização.

### CriarProdutosConfiguraveisEventHandler
- **Evento de entrada:** `CriarProdutosConfiguraveis`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IVarejOnlineApiService`, `ISqsRepository` (SyncOut).
- **Fluxo:**
  1. Recupera token do ERP e, para cada produto base, busca variações via `GetProdutosAsync` para compor um `ProdutoView` configurável.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosConfiguraveisEventHandler.cs†L40-L62】
  2. Serializa a coleção mapeada e envia notificação FIFO `notificacao-syncout-{hub}` para o tipo de processo Produto.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosConfiguraveisEventHandler.cs†L65-L87】
- **Saídas:** Mensagens **SyncOut** com produtos configuráveis (base + variações).

### PriceTablesRequestedEventHandler
- **Evento de entrada:** `PriceTablesRequested`.
- **Dependências:** `ILogger`, `ISqsRepository` (SyncOut), `IVarejOnlineApiService`, `IIntegrationService`, `IConfiguration`, `IEventDispatcher`.
- **Fluxo:**
  1. Garante inicialização da fila SyncOut e obtém token da integração.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Preco/PriceTablesRequestedEventHandler.cs†L22-L44】
  2. Pagina tabelas de preço no ERP usando o tamanho padrão configurado; para cada lote, dispara `PriceTablePageProcessed` com o conteúdo retornado.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Preco/PriceTablesRequestedEventHandler.cs†L59-L88】
- **Saídas:** Eventos `PriceTablePageProcessed` para processamento e publicação assíncrona de cada página.

### PriceTablesPageProcessedEventHandler
- **Evento de entrada:** `PriceTablePageProcessed`.
- **Dependências:** `ILogger`, `ISqsRepository` (SyncOut), `IOptions<SyncOutConfig>`.
- **Fluxo:**
  1. Loga a página recebida e converte as tabelas para o modelo consumido pelo Hub com `Map()`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Preco/PriceTablesPageProcessedEventHandler.cs†L29-L43】
  2. Publica o resultado serializado na fila FIFO `notificacao-syncout-{hub}` marcando o processo como preço.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Preco/PriceTablesPageProcessedEventHandler.cs†L45-L53】
- **Saídas:** Mensagens **SyncOut** contendo tabelas de preço normalizadas.

## Handler de estoque

### StocksRequestedEventHandler
- **Evento de entrada:** `StocksRequested`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IVarejOnlineApiService`, `ISyncProcessService`, `ISqsRepository` (SyncOut), `IOptions<SyncOutConfig>`, `IConfiguration`.
- **Fluxo:**
  1. Busca a integração, configura paginação padrão e abre um registro de processo de sincronização (`CreateSyncProcessDto`).【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L49-L88】
  2. Para cada página:
     - Registra um item de processo com descrição e metadados da página.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L103-L121】
     - Consulta estoques no ERP com filtros do evento e mapeia o resultado para o modelo do Hub.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L125-L159】
     - Publica a notificação na fila FIFO `notificacao-syncout-{hub}` com tipo de processo Estoque.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L146-L158】
     - Atualiza o status do item de processo e o progresso geral.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L165-L188】
  3. Finaliza o processo com status correspondente (Finished/Cancelled/Failed) conforme o fluxo ou exceções capturadas.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L189-L207】
- **Saídas:** Mensagens **SyncOut** com saldos e atualizações de status do processo de sincronização.

## Handlers de pedidos e faturamento

### PedidoEventHandlerBase (comum aos handlers de pedido)
- **Responsabilidade:** inicializar a fila SyncIn e disponibilizar helpers para publicar `PedidoRetornoView`.
- **Detalhes:**
  - Inicializa a fila FIFO `notificacao-syncin-{hub}` com base na configuração `SyncIn` logo no construtor.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/PedidoEventHandlerBase.cs†L17-L26】
  - Fornece métodos utilitários `PublishPedidoRetorno` e `BuildPedidoRetornoView` para montar o retorno consolidado do pedido e publicar na fila.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/PedidoEventHandlerBase.cs†L28-L76】

### OrderCreatedEventHandler
- **Evento de entrada:** `OrderCreated`.
- **Fluxo específico:**
  1. Registra log e busca a integração para obter token do ERP.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderCreatedEventHandler.cs†L38-L46】
  2. Quando há CPF/CNPJ do cliente, tenta localizar o terceiro; se inexistente, cria-o via API antes de montar o pedido.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderCreatedEventHandler.cs†L48-L71】
  3. Mapeia o pedido, injeta referência do terceiro e envia ao ERP. Em sucesso, publica `PedidoRetornoView` com flag `PedidoIncluido` na fila SyncIn.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderCreatedEventHandler.cs†L74-L85】
- **Saídas:** Mensagem **SyncIn** confirmando inclusão do pedido.

### OrderShippedEventHandler
- **Evento de entrada:** `OrderShipped`.
- **Fluxo específico:**
  1. Valida o pedido recebido, recupera token e status configurado `StatusShipped` da integração.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderShippedEventHandler.cs†L33-L48】
  2. Envia `AlterarStatusPedidoRequest` ao ERP; em caso de sucesso, atualiza `PedidoStatusERPId` e publica retorno com `PedidoAlterado=true`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderShippedEventHandler.cs†L49-L72】
- **Saídas:** Mensagem **SyncIn** indicando alteração de status para enviado.

### OrderDeliveredEventHandler
- **Evento de entrada:** `OrderDelivered`.
- **Fluxo específico:**
  1. Recupera token e status `StatusDelivered`, envia atualização de status ao ERP.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderDeliveredEventHandler.cs†L33-L58】
  2. Atualiza o `PedidoStatusERPId` local e publica retorno de pedido com `PedidoAlterado=true`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderDeliveredEventHandler.cs†L66-L72】
- **Saídas:** Mensagem **SyncIn** indicando status entregue.

### OrderCancelledEventHandler
- **Evento de entrada:** `OrderCancelled`.
- **Fluxo específico:**
  1. Valida existência de pedido, chama `CancelarPedidoAsync` no ERP e trata falhas com logs.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderCancelledEventHandler.cs†L32-L53】
  2. Marca o pedido como cancelado, garante `PedidoERPId` e publica retorno com `PedidoCancelado=true`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/OrderCancelledEventHandler.cs†L55-L62】
- **Saídas:** Mensagem **SyncIn** confirmando cancelamento do pedido.

### InvoicesRequestedEventHandler
- **Evento de entrada:** `InvoicesRequested`.
- **Dependências:** `ILogger`, `IIntegrationService`, `IVarejOnlineApiService`, `ISyncInApiService`.
- **Fluxo:**
  1. Busca a integração e tenta obter o XML da nota via API do ERP.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/InvoicesRequestedEventHandler.cs†L36-L55】
  2. Se a API falhar ou retornar vazio, gera XML mock padronizado para garantir continuidade.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/InvoicesRequestedEventHandler.cs†L56-L75】
  3. Envia o XML (real ou mock) ao serviço SyncIn via `InserirNotaFiscalExterna`, disponibilizando-o para o hub.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/InvoicesRequestedEventHandler.cs†L65-L68】
- **Saídas:** Nota fiscal externa registrada no SyncIn, assegurando retorno mesmo em caso de falha no ERP.

## Considerações finais

- Handlers de **SyncOut** (produtos, preços, estoques) sempre inicializam a fila `notificacao-syncout-{hub}` antes de enviar mensagens, garantindo idempotência na configuração de filas SQS.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Produto/CriarProdutosSimplesEventHandler.cs†L21-L28】【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Preco/PriceTablesPageProcessedEventHandler.cs†L18-L27】
- Handlers de **SyncIn** (pedidos, notas fiscais) utilizam o helper do `PedidoEventHandlerBase` ou serviços específicos para publicar notificações para consumo interno do Hub.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Pedido/PedidoEventHandlerBase.cs†L28-L76】
- O `IEventDispatcher` mantém o fluxo orquestrado, encadeando eventos que quebram cargas grandes em etapas menores e paralelizáveis, especialmente para produtos e tabelas de preço.

