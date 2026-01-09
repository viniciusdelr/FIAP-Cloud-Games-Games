# FIAP-Cloud-Games-Games
```markdown
# FCG Games Microservice

Microsserviço responsável pelo catálogo de jogos, biblioteca do usuário e inicialização do fluxo de compras.

## Funcionalidades

* **Catálogo**: CRUD de jogos (Listagem, Cadastro, Edição).
* **Biblioteca**: Consulta de jogos adquiridos pelo usuário.
* **Compras**: Processamento assíncrono de aquisição de jogos.

## Endpoints Principais

* `GET /GetVideoGames`: Lista todos os jogos disponíveis.
* `POST /buy`: Inicia o processo de compra. Retorna 202 (Accepted) e envia uma mensagem para a fila.
* `GET /GetUserLibrary/{id}`: Consulta a biblioteca do usuário.

## Lógica de Mensageria (MassTransit)

Este serviço atua como **Producer** e **Consumer**:

1. Ao receber um `POST /buy`, cria um registro com status "Pendente" e publica o evento `GamePurchased` no Azure Service Bus.
2. Possui um Consumer (`PaymentResultConsumer`) que escuta a fila de respostas do serviço de Pagamentos.
3. Ao receber `PaymentResult`, atualiza o status da compra no banco de dados para "Aprovado" ou "Rejeitado".

## Execução Local (Docker)

Certifique-se de fornecer a ConnectionString do Service Bus e do SQL Server.

```bash
docker build -t fcg-games .
docker run -p 8080:80 -e "ConnectionStrings:ServiceBusConnection=SUA_STRING" -e "ConnectionStrings:DefaultConnection=SUA_DB_STRING" fcg-games