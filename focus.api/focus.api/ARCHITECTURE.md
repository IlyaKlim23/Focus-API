# Архитектура Focus API

## Умный персональный планировщик задач с прогнозом продуктивности

> ВКР: Система на основе ASP.NET Core с ML-моделью для предсказания продуктивных периодов и формирования оптимального расписания.

---

## 1. Обзор архитектуры

Применяется **Clean Architecture** (Чистая архитектура) с разделением на слои по принципу зависимостей: внутренние слои не зависят от внешних.

```
┌─────────────────────────────────────────────────────────────────┐
│                      Focus.API (Presentation)                     │
│  Controllers, Middleware, DTOs, Swagger                           │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Focus.Application (Use Cases)                   │
│  Services, CQRS handlers, Feature Engineering, Schedule Logic     │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Focus.Domain (Core)                            │
│  Entities, Value Objects, Domain Events, Interfaces (Ports)       │
└─────────────────────────────────────────────────────────────────┘
                                ▲
                                │
┌─────────────────────────────────────────────────────────────────┐
│                 Focus.Infrastructure (Adapters)                   │
│  EF Core, ML Service, NLP, Repositories, External APIs            │
└─────────────────────────────────────────────────────────────────┘
```

---

## 2. Слои и ответственность

### 2.1 Focus.Domain
**Ядро системы.** Не зависит ни от каких внешних слоёв.

| Компонент | Назначение |
|-----------|------------|
| **Entities** | `User`, `Task`, `DailyNote`, `TaskCategory`, `ScheduleSlot` |
| **Value Objects** | `TimeSlot`, `Priority`, `TaskStatus`, `ProductivityScore` |
| **Interfaces (Ports)** | `ITaskRepository`, `IUserRepository`, `IProductivityPredictor`, `IScheduleOptimizer` |
| **Domain Events** | `TaskCompleted`, `DailyNoteCreated` (опционально) |

**Ключевые сущности:**

```
User
├── Id, Email, CreatedAt
└── Tasks[], DailyNotes[]

Task
├── Id, UserId, Title, Description
├── Status (Todo, InProgress, Done, Cancelled)
├── Priority (Low, Medium, High, Critical)
├── CategoryId, EstimatedMinutes, ActualMinutes
├── DueDate, StartedAt, CompletedAt, CreatedAt
└── Interruptions (счётчик прерываний)

DailyNote
├── Id, UserId, Date
├── Content (текст заметки для NLP)
├── MoodScore, EnergyLevel (опционально)
└── ExtractedFactors (ключевые причины из NLP)

ProductivityPrediction
├── UserId, DateTime (slot)
├── Score (0–1 вероятность высокой продуктивности)
└── Factors (объяснимость: время суток, день недели, и т.д.)
```

---

### 2.2 Focus.Application
**Бизнес-логика и сценарии использования.**

| Компонент | Назначение |
|-----------|------------|
| **Services** | `TaskService`, `ScheduleService`, `ProductivityService`, `DailyNoteService`, `FeatureEngineeringService` |
| **DTOs** | Request/Response модели для API |
| **Interfaces** | Контракты для внешних сервисов (ML, NLP) |
| **Mappings** | AutoMapper или ручной маппинг |

**Основные сценарии:**

1. **Управление задачами** — CRUD, смена статуса, учёт времени и прерываний.
2. **Генерация признаков** — агрегаты по 7/14/30 дням, среднее время выполнения, % незавершённых, частота прерываний.
3. **Предсказание продуктивности** — вызов ML-модели для слотов (почасово на день/неделю).
4. **Формирование расписания** — жадный алгоритм + приоритеты + баланс нагрузки.
5. **Обработка ежедневных заметок** — сохранение, NLP-анализ, обновление фич для модели.

---

### 2.3 Focus.Infrastructure
**Реализация портов: БД, ML, внешние сервисы.**

| Компонент | Назначение |
|-----------|------------|
| **Persistence** | EF Core, DbContext, миграции, репозитории |
| **ML Service** | Обёртка над ML.NET или HTTP-клиент к Python-сервису (LightGBM/scikit-learn) |
| **NLP Service** | Анализ текста заметок (ключевые слова, тональность, причины отвлечений) |
| **Caching** | Redis (опционально) для кэширования предсказаний |

**Рекомендация по ML:**
- **Вариант A:** ML.NET в том же процессе — проще деплой, подходит для табличных моделей (бустинг).
- **Вариант B:** Отдельный Python-сервис (FastAPI) — гибче для LSTM/Transformer, требует Docker/orchestration.

---

### 2.4 Focus.API
**HTTP API, аутентификация, Swagger.**

| Компонент | Назначение |
|-----------|------------|
| **Controllers** | `TasksController`, `ScheduleController`, `ProductivityController`, `DailyNotesController`, `UsersController` |
| **Middleware** | Auth (JWT), глобальная обработка ошибок, валидация |
| **Configuration** | DI, CORS, настройки ML/NLP endpoints |

**Эндпоинты (черновик):**

```
/api/v1/tasks           GET, POST, PUT, DELETE
/api/v1/tasks/{id}      GET, PATCH (status, time)
/api/v1/schedule        POST (generate for date)
/api/v1/productivity    GET (predictions for date range)
/api/v1/daily-notes     GET, POST
/api/v1/users           GET (profile), POST (register)
/api/v1/auth            POST (login, refresh)
```

---

## 3. Модули и потоки данных

### 3.1 Поток «Создание и завершение задачи»
```
Client → TasksController → TaskService → ITaskRepository (Infrastructure)
                                    ↓
                            FeatureEngineeringService (обновление агрегатов)
                                    ↓
                            (при необходимости) ProductivityModel retrain/update
```

### 3.2 Поток «Генерация расписания»
```
Client → ScheduleController → ScheduleService
                                    ├→ FeatureEngineeringService (признаки)
                                    ├→ IProductivityPredictor (предсказания по слотам)
                                    └→ IScheduleOptimizer (распределение задач)
                                    ↓
                            ScheduleDto (slots + tasks)
```

### 3.3 Поток «Ежедневная заметка»
```
Client → DailyNotesController → DailyNoteService
                                    ├→ INlpAnalyzer (ключевые факторы)
                                    └→ FeatureEngineeringService (новые фичи)
                                    ↓
                            DailyNoteDto + ExtractedFactors
```

---

## 4. ML и Feature Engineering

### 4.1 Признаки (из описания ВКР)
- `hour_of_day`, `day_of_week`
- `avg_task_duration_7d`, `avg_task_duration_14d`, `avg_task_duration_30d`
- `completion_rate_7d`, `completion_rate_14d`, `completion_rate_30d`
- `interruption_frequency_7d`, `interruption_frequency_14d`
- `priority_distribution`, `category_distribution`
- Фичи из NLP: `has_stress`, `had_sleep_issues`, `felt_energetic`, и т.д.

### 4.2 Модель
- **Табличные признаки:** Gradient Boosting (LightGBM / ML.NET)
- **Последовательные паттерны (опционально):** LSTM/Transformer для истории по дням
- **Выход:** вероятность высокой продуктивности (0–1) для каждого часового слота

### 4.3 Алгоритм планирования
- Вход: список задач (приоритет, оценка времени, дедлайн).
- Для каждого слота — предсказание продуктивности.
- Жадное распределение: высокоприоритетные задачи → слоты с максимальной продуктивностью.
- Ограничения: баланс нагрузки, не превышать 8–10 часов в день.

---

## 5. Технологический стек

| Категория | Технология |
|-----------|------------|
| Runtime | .NET 10 |
| Web API | ASP.NET Core Minimal API или MVC |
| ORM | Entity Framework Core |
| БД | PostgreSQL или SQL Server |
| Auth | JWT (ASP.NET Identity или реализация своими силами) |
| ML | ML.NET и/или Python-микросервис |
| NLP | ML.NET Text или вызов Python (spaCy, transformers) |
| Документация | Swagger/OpenAPI |
| Тесты | xUnit, NSubstitute, FluentAssertions |

---

## 6. Структура решения

```
Focus.sln
├── src/
│   ├── Focus.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Interfaces/
│   │   └── Exceptions/
│   ├── Focus.Application/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Common/
│   ├── Focus.Infrastructure/
│   │   ├── Persistence/
│   │   ├── ML/
│   │   ├── NLP/
│   │   └── Repositories/
│   └── Focus.API/
│       ├── Controllers/
│       ├── Middleware/
│       └── Configuration/
└── tests/
    ├── Focus.Domain.Tests/
    ├── Focus.Application.Tests/
    └── Focus.API.Tests/
```

---

## 7. Дальнейшие шаги

1. Создать проекты Domain, Application, Infrastructure.
2. Реализовать сущности и репозитории.
3. Подключить EF Core, создать миграции.
4. Реализовать Feature Engineering и заглушку ML-сервиса.
5. Добавить контроллеры и JWT.
6. Интегрировать реальную ML-модель (ML.NET или Python).
7. Добавить модуль NLP для ежедневных заметок.
8. Написать тесты и документацию.
