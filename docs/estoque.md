# Sincronização de Estoques

Este documento explica como o **LexosHub ERP VarejOnline** consulta os saldos de mercadorias no ERP e publica as variações de estoque para o Hub. O fluxo é orquestrado pelo evento `StocksRequested` e percorre desde a abertura do processo de sincronização até o envio das mensagens na fila **SyncOut**.

## Visão geral do fluxo

1. O evento `StocksRequested` é recebido pelo `StocksRequestedEventHandler`, que resolve dependências como integração, serviço de API do ERP, controle de processos e fila SQS de saída.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L28-L46】
2. O handler busca a integração do hub para recuperar o token, define a paginação e abre um processo de sincronização (`SyncProcess`) com os filtros recebidos.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L53-L90】
3. Para cada página, registra um item no processo, monta a requisição de estoque e consulta o endpoint `apps/api/saldos-mercadorias` do Varejo Online.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L103-L139】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L224-L255】
4. Os registros retornados são transformados pelo `EstoqueViewMapper` e publicados como `NotificacaoAtualizacaoModel` na fila FIFO `notificacao-syncout-{hub}` com tipo de processo **Estoque**.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L142-L158】【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L10-L39】
5. Ao final de cada página, o item do processo é atualizado. Quando termina a paginação (ou ao capturar erros/cancelamento) o processo principal é finalizado com o status adequado.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L165-L207】

## Parâmetros aceitos e paginação

Os filtros aceitos pelo evento são propagados diretamente para a consulta do ERP via `EstoqueRequest`. É possível restringir o escopo por produtos, entidades, recortes de data e flags de canais. Também é definido o deslocamento inicial (`Inicio`) e a quantidade de registros por página (`Quantidade`). Quando o evento não informa `Quantidade`, aplica-se o tamanho padrão configurado em `VarejOnlineApiSettings:DefaultPageSize`.

| Campo do evento | Query string enviada | Descrição |
| --------------- | -------------------- | --------- |
| `Produtos` | `produtos` | Lista de SKUs/IDs separados por vírgula para restringir a consulta.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L125-L128】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L230-L234】|
| `Entidades` | `entidades` | Filtra pelos identificadores das entidades (lojas) desejadas.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L125-L129】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L230-L235】|
| `Inicio` | `inicio` | Offset inicial da paginação. Default: `0` quando ausente.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L63-L64】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L236-L238】|
| `Quantidade` | `quantidade` | Tamanho da página. Se não informado ou <= 0, usa o valor padrão da configuração do serviço.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L63-L66】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L239-L240】|
| `AlteradoApos` | `alteradoApos` | Consulta registros alterados após a data/hora informada.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L131-L132】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L242-L244】|
| `Data` | `data` | Permite fixar a leitura em uma data-base específica (saldo histórico).【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L132-L133】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L245-L246】|
| `SomenteEcommerce` | `somenteEcommerce` | Limita o resultado a produtos marcados para e-commerce.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L133-L134】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L248-L250】|
| `SomenteMarketplace` | `somenteMarketplace` | Retorna apenas itens habilitados para marketplace.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L134-L135】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Services/VarejoOnlineApiService.cs†L251-L252】|

Os campos do request são declarados no `EstoqueRequest`, garantindo a aderência entre o evento do Hub e os parâmetros da API do ERP.【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Responses/Estoque/EstoqueRequest.cs†L3-L13】

## Estrutura dos dados publicados

Cada item retornado pelo ERP contém informações sobre produto, entidade e quantidade disponível.【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Responses/Estoque/EstoqueResponse.cs†L3-L10】【F:src/LexosHub.ERP.VarejOnline.Infra.ErpApi/Responses/Estoque/ProdutoEstoqueResponse.cs†L3-L10】 Esses dados são transformados para o modelo `ProdutoEstoqueView`, que é serializado no corpo da notificação enviada à fila:

- **LojaIdGlobal**: ID da entidade convertida para `int` (identificador da loja no Hub).【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L23-L33】
- **Quantidade**: saldo disponível retornado pelo ERP.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L32-L33】
- **QuantidadeReservado**: sempre `null`, pois o ERP não fornece esse campo no endpoint consultado.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L32-L34】
- **Tipo**: constante `SIMPLES`, sinalizando que o saldo é consolidado por SKU simples.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L33-L35】
- **Sku**: prioriza `CodigoSistema`, caindo para `CodigoInterno` ou `CodigoBarras` quando necessário.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L35-L38】
- **DateVersion**: data utilizada para controle de versionamento; tenta primeiro `DataAlteracao`, depois `Data` e, na falta de ambas, usa `DateTime.UtcNow`.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Mappers/Estoque/EstoqueViewMapper.cs†L39-L57】

O payload final é embalado em `NotificacaoAtualizacaoModel`, junto com metadados como `Chave` (hub), `TipoProcesso = Estoque`, `DataHora` da captura e `PlataformaId = 41`. A mensagem é enviada para a fila FIFO `notificacao-syncout-{hub}`, mantendo a ordenação das atualizações de estoque por hub.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L148-L158】

## Monitoramento e tratamento de erros

- Cada página processada registra um item no `SyncProcess`, permitindo acompanhar progresso e métricas de registros recuperados.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L114-L171】
- Em caso de falha na leitura da página, o item é marcado como `Failed` e a exceção é propagada para que o processo principal seja encerrado com erro.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L172-L207】
- Cancelamentos por `CancellationToken` atualizam o processo com status `Cancelled`; sucesso completo encerra com `Finished`, armazenando o total de páginas e registros tratados.【F:src/LexosHub.ERP.VarejOnline.Infra.Messaging/Handlers/Estoque/StocksRequestedEventHandler.cs†L189-L207】

Com essas etapas, o Hub garante rastreabilidade completa da sincronização de estoques e disponibiliza os saldos atualizados para consumo pelos canais integrados.

## Agendamento automático via Hangfire

Ambientes de desenvolvimento podem acionar a sincronização de estoques de forma contínua por meio de um job recorrente do **Hangfire**. Quando o serviço é executado com um depurador anexado, o `Program` registra o `StockSyncJobService` com o `IRecurringJobManager`, configurando-o para rodar a cada minuto (`"* * * * *"`).【F:src/LexosHub.ERP.VarejOnline.Api/Program.cs†L134-L144】

O job instancia o `StockSyncJobService`, recupera todas as integrações ativas e despacha um evento `StocksRequested` para cada uma, reaproveitando o fluxo descrito nas seções anteriores.【F:src/LexosHub.ERP.VarejOnline.Api/Jobs/StockSyncJobService.cs†L8-L85】 Dessa forma, é possível testar localmente a publicação de estoques sem depender de um disparo manual do evento.
