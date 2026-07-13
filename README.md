# Payment Service

Serviço de pagamentos. Consome `order.created` do RabbitMQ, registra pagamentos no SQL Server e permite aprová-los ou recusá-los. Ao finalizar, publica `payment.processed` para os demais serviços.

## Componentes

| Projeto | Responsabilidade |
| --- | --- |
| `br.com.fiap.cloudgames.Payment.Domain` | Regras de pagamento, objetos de valor e contratos. |
| `br.com.fiap.cloudgames.Payment.Application` | Casos de uso e manipulador de pedidos criados. |
| `br.com.fiap.cloudgames.Payment.Infrastructure` | SQL Server/EF Core, JWT e RabbitMQ. |
| `br.com.fiap.cloudgames.Payment.WebAPI` | API REST, Swagger e worker consumidor de eventos. |

## Pré-requisitos

- .NET SDK 10;
- SQL Server na porta `1433`;
- RabbitMQ na porta `5672` (painel: `15672`).

Para iniciar a plataforma completa, consulte o [README da orquestração](https://github.com/andersonvnieves/orchestration/blob/main/README.md).

## Configuração local

O perfil `Development` carrega `br.com.fiap.cloudgames.Payment.WebAPI/appsettings.Development.json`. Para usar credenciais ou serviços diferentes, sobrescreva a configuração com variáveis de ambiente:

```powershell
$env:ConnectionStrings__Default = "Server=localhost,1433;Database=FGC_Payment;User Id=sa;Password=<SENHA>;TrustServerCertificate=True;"
$env:Jwt__Issuer = "fgcapi"
$env:Jwt__Audience = "fgcapi"
$env:Jwt__Key = "<CHAVE_JWT>"
$env:RabbitMQ__URI = "amqp://<USUARIO>:<SENHA>@localhost:5672/"
$env:RabbitMQ__OrderCreatedEvent__Exchange = "fgc"
$env:RabbitMQ__OrderCreatedEvent__RoutingKey = "order.created"
$env:RabbitMQ__PaymentProcessedEvent__Exchange = "fgc"
$env:RabbitMQ__PaymentProcessedEvent__RoutingKey = "payment.processed"
```

## Executar localmente

Na pasta `payment-service`:

```powershell
dotnet restore .\br.com.fiap.cloudgames.PaymentAPI.sln
dotnet run --project .\br.com.fiap.cloudgames.Payment.WebAPI\br.com.fiap.cloudgames.Payment.WebAPI.csproj --launch-profile http
```

A API atende em `http://localhost:5151`; o Swagger está em `http://localhost:5151/swagger`. As migrations são aplicadas automaticamente na inicialização.

## Endpoints principais

| Método | Rota | Acesso |
| --- | --- | --- |
| `POST` | `/api/Payment/approve?orderId={guid}` | JWT válido |
| `POST` | `/api/Payment/decline?orderId={guid}` | JWT válido |

Envie `Authorization: Bearer <token>` com um JWT compatível com a configuração do serviço.

## Testes

```powershell
dotnet test .\br.com.fiap.cloudgames.PaymentAPI.sln
```

## Contêiner

```powershell
docker build -t fgc-payment-service:latest .
docker run --rm -p 8082:8080 fgc-payment-service:latest
```

Forneça as configurações de banco, JWT e RabbitMQ por variáveis de ambiente ao executar a imagem isoladamente.
