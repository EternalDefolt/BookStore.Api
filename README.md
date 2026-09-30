# BookStore.Api

ASP.NET Core Web API: версионирование (URL / заголовок / query), Swagger по версиям, deprecation-заголовки.

- SDK: .NET 9 (`net9.0`)
- Запуск: `dotnet run` → https://localhost:5001 (и http://localhost:5000)
- Swagger UI: https://localhost:5001/swagger (Development)

## Версии API

| Версия | Ресурс | Отличия |
|---|---|---|
| v1 — **deprecated** (Sunset: 31.12.2026) | `/api/v1/books` | `price` — число, есть `isbn` |
| v2 | `/api/v2/books` | `price` — объект `{amount, currency}`, ответ — envelope с пагинацией |

Без версии в URL (`/api/books`) используется дефолтная v1. Версию можно задать заголовком `api-version`
или query-параметром `?api-version=`. Для устаревшей версии отдаются заголовки `Deprecation`, `Sunset`,
`api-deprecated-versions`.

## Состав

| Путь | Что |
|---|---|
| `Program.cs` | конфигурация сервисов и pipeline |
| `Swagger/ConfigureSwaggerOptions.cs` | отдельный Swagger-документ на каждую версию |
| `Middleware/DeprecationMiddleware.cs` | заголовки `Deprecation` / `Sunset` для устаревших версий |
| `Controllers/BooksController.cs` | v1 (deprecated) |
| `Controllers/BooksV2Controller.cs` | v2 |
| `Services/InMemoryBookRepository.cs` | данные в памяти |

## Проверка работы (curl)

Все запросы выполнены против `https://localhost:5001`.

### 1. Дефолтная версия

```bash
curl -k https://localhost:5001/api/books
```

```json
[{"id":1,"title":"Мастер и Маргарита","author":"Михаил Булгаков","price":890,"isbn":"978-5-389-01686-3"},{"id":2,"title":"Преступление и наказание","author":"Фёдор Достоевский","price":720,"isbn":"978-5-04-116345-3"},{"id":3,"title":"Война и мир","author":"Лев Толстой","price":1490,"isbn":"978-5-17-090326-5"},{"id":4,"title":"Евгений Онегин","author":"Александр Пушкин","price":460,"isbn":"978-5-699-92580-6"},{"id":5,"title":"Мёртвые души","author":"Николай Гоголь","price":610,"isbn":"978-5-04-119117-5"}]
```

HTTP 200

### 2. Явная v1

```bash
curl -k https://localhost:5001/api/v1/books
```

HTTP 200 — тело совпадает с п.1 (v1-модель).

### 3. Явная v2

```bash
curl -k https://localhost:5001/api/v2/books
```

```json
{"items":[{"id":1,"title":"Мастер и Маргарита","author":"Михаил Булгаков","price":{"amount":890,"currency":"RUB"}},{"id":2,"title":"Преступление и наказание","author":"Фёдор Достоевский","price":{"amount":720,"currency":"RUB"}},{"id":3,"title":"Война и мир","author":"Лев Толстой","price":{"amount":1490,"currency":"RUB"}},{"id":4,"title":"Евгений Онегин","author":"Александр Пушкин","price":{"amount":460,"currency":"RUB"}},{"id":5,"title":"Мёртвые души","author":"Николай Гоголь","price":{"amount":610,"currency":"RUB"}}],"total":5,"page":1,"pageSize":10}
```

HTTP 200

### 4. Заголовки устаревшей версии

```bash
curl -k -I https://localhost:5001/api/v1/books
```

```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
api-deprecated-versions: 1.0
Deprecation: true
Sunset: Thu, 31 Dec 2026 23:59:59 GMT
```

### 5. Создание в v1

```bash
curl -k -X POST https://localhost:5001/api/v1/books \
  -H "Content-Type: application/json" \
  -d '{"title":"Refactoring","author":"Martin Fowler","price":2500,"isbn":"978-0134757599"}'
```

```json
{"id":6,"title":"Refactoring","author":"Martin Fowler","price":2500,"isbn":"978-0134757599"}
```

HTTP 201

### 6. Создание в v2

```bash
curl -k -X POST https://localhost:5001/api/v2/books \
  -H "Content-Type: application/json" \
  -d '{"title":"Refactoring","author":"Martin Fowler","price":{"amount":2500,"currency":"RUB"}}'
```

```json
{"id":7,"title":"Refactoring","author":"Martin Fowler","price":{"amount":2500,"currency":"RUB"}}
```

HTTP 201

### 7. Версия через query

```bash
curl -k "https://localhost:5001/api/books?api-version=2.0"
```

HTTP 200 — v2-ответ, `total: 7` (после двух POST).

### 8. Версия через заголовок

```bash
curl -k -H "api-version: 2.0" https://localhost:5001/api/books
```

HTTP 200 — v2-ответ, `total: 7`.

---

> Примечание: в PowerShell 5.1 кавычки в JSON для `-d` съедаются — запускать команды из cmd / git-bash
> или экранировать (`--%` + `\"`). В cmd, git-bash и PowerShell 7 команды выше работают как есть.
