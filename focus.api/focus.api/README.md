# Focus API

**Умный персональный планировщик задач с прогнозом продуктивности (ML)**

Серверная часть ВКР: REST API на ASP.NET Core с архитектурой Clean Architecture.

## Требования

- .NET 10
- PostgreSQL 14+

## Настройка PostgreSQL

Создайте базу и пользователя:

```sql
CREATE DATABASE focus;
CREATE USER focus_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE focus TO focus_user;
```

Строка подключения в `appsettings.json` (или переменная окружения `ConnectionStrings__DefaultConnection`):

```
Host=localhost;Port=5432;Database=focus;Username=postgres;Password=postgres
```

При запуске приложения миграции применяются автоматически.

Создание новых миграций:
```bash
dotnet ef migrations add MigrationName --project src/Focus.Database --startup-project focus.api
```

## Запуск

```bash
cd focus.api
dotnet run --project focus.api
```

API будет доступен по адресу из `launchSettings.json` (обычно `https://localhost:5xxx`). Swagger UI: `/swagger`.

## Аутентификация

1. **Регистрация**: `POST /api/v1/auth/register`
   ```json
   {"email": "user@example.com", "password": "pass123", "displayName": "Имя"}
   ```

2. **Вход**: `POST /api/v1/auth/login`
   ```json
   {"email": "user@example.com", "password": "pass123"}
   ```

3. В ответе приходит `accessToken`. Для защищённых эндпоинтов добавляйте заголовок:
   ```
   Authorization: Bearer <accessToken>
   ```

## Эндпоинты

| Метод | Путь | Описание |
|-------|------|----------|
| POST | /api/v1/auth/register | Регистрация |
| POST | /api/v1/auth/login | Вход |
| GET | /api/v1/tasks | Список задач |
| GET | /api/v1/tasks/{id} | Задача по ID |
| POST | /api/v1/tasks | Создать задачу |
| PUT | /api/v1/tasks/{id} | Обновить задачу |
| DELETE | /api/v1/tasks/{id} | Удалить задачу |
| POST | /api/v1/schedule | Сгенерировать расписание на дату |
| GET | /api/v1/daily-notes/{date} | Заметка за день (yyyy-MM-dd) |
| POST | /api/v1/daily-notes/{date} | Создать/обновить заметку |

## Дальнейшие шаги

1. **ML-модель** — заменить StubProductivityPredictor на ML.NET или Python-сервис
2. **NLP** — заменить StubNlpAnalyzer на реальный анализ текста
3. **Feature Engineering** — модуль генерации признаков для модели
4. **Тесты** — xUnit, NSubstitute
