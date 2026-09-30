# ТЕСТЫ

Скриншоты прогона curl-проверок против `https://localhost:5001` (реальные снимки окна терминала, не рендер).

| Скриншот | Что проверяет | Результат |
|---|---|---|
| `тест-1-2.png` | `GET /api/books` (без версии — дефолт v1) и `GET /api/v1/books` | 200; у v1 заголовки `Deprecation: true`, `Sunset` |
| `тест-3-4.png` | `GET /api/v2/books` и `HEAD /api/v1/books` (`curl -I`) | 200; v2 — envelope с пагинацией; у v1 Deprecation/Sunset |
| `тест-5-6.png` | `POST /api/v1/books` и `POST /api/v2/books` | 201 Created, `Location`, созданы записи id 6 и 7 |
| `тест-7-8.png` | версия через `?api-version=2.0` и через заголовок `api-version: 2.0` | 200; отдана v2, `total: 7` (после двух POST) |
