# FIAP Cloud Games - Games API

Microsserviço responsável pelo gerenciamento do catálogo de jogos, manutenção da biblioteca do usuário e orquestração inicial do processo de compras.

## Arquitetura e Lógica

Este serviço opera de forma híbrida (Síncrona e Assíncrona):
* **Catálogo e Biblioteca**: Operações de leitura e escrita diretas no banco de dados (SQL Server).
* **Fluxo de Compra (Producer)**: Ao iniciar uma compra (`/buy`), o serviço não processa o pagamento imediatamente. Ele registra o pedido com status "Pendente" e publica uma mensagem na fila do **Azure Service Bus**.
* **Processamento de Resposta (Consumer)**: O serviço fica escutando a fila de respostas. Quando o microsserviço de Payments processa a transação, este serviço recebe o evento de volta para atualizar o status do pedido (Aprovado/Rejeitado) na biblioteca do usuário.

## Tecnologias

* .NET 8
* Entity Framework Core
* SQL Server / Azure SQL
* MassTransit (Abstração de Mensageria)
* Azure Service Bus
* Docker

## Configuração (Variáveis de Ambiente)

Para a correta execução, as seguintes configurações são mandatórias no `appsettings.json` ou variáveis de ambiente:

* `ConnectionStrings:DefaultConnection`: Conexão com o banco de dados dos Jogos.
* `ConnectionStrings:ServiceBusConnection`: Endpoint de conexão com o Azure Service Bus (necessário permissão de Send/Listen).
* `Jwt:Key`: A mesma chave utilizada no serviço de Users para validar o token Bearer.

## Execução Local (Docker)

É necessário que o SQL Server esteja rodando e que a connection string do Service Bus seja válida.

1. Construir a imagem:
   docker build -t fcg-games .

2. Rodar o container:
   docker run -d -p 8081:80 \
     -e "ConnectionStrings:DefaultConnection=SUA_SQL_CONN_STRING" \
     -e "ConnectionStrings:ServiceBusConnection=SUA_SB_CONN_STRING" \
     --name fcg-games \
     fcg-games

## Deploy na Azure (Kubernetes)

No cluster AKS:
1. O serviço utiliza **ConfigMaps** e **Secrets** para receber as credenciais do Service Bus e do Banco de Dados de forma segura.
2. O **MassTransit** configura automaticamente os tópicos e filas no Service Bus ao iniciar, garantindo que a infraestrutura de mensageria esteja pronta.
3. O serviço escala horizontalmente (HPA) baseado no uso de CPU/Memória, garantindo disponibilidade do catálogo mesmo em picos de acesso.

## Endpoints

A documentação completa está disponível no Swagger.

### VideoGames
* `GET /GetVideoGames`: Retorna a lista de jogos disponíveis para compra.
* `POST /buy`: Inicia o fluxo de compra.
  * **Input**: ID do Jogo.
  * **Header**: Token JWT (Obrigatório para identificar o comprador).
  * **Lógica**: Retorna `202 Accepted` indicando que o pedido foi enfileirado para processamento.

### Library
* `GET /GetUserLibrary/{id}`: Retorna os jogos que o usuário já possui e o status das compras recentes.

## Testes

dotnet test