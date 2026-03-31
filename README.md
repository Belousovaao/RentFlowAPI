# RentFlowAPI

RentFlow — это backend-система для управления процессом аренды активов (например, автомобилей, техники и т.д.).

Система решает ключевые задачи:\
	•	управление активами (Assets);\
	•	управление клиентами (Customers);\
	•	создание и контроль бронирований (Bookings);\
	•	предотвращение конфликтов бронирований (overlapping);\
	•	фиксация состояния данных на момент бронирования (snapshot).

Основные бизнес-правила:\
	•	Актив не может быть забронирован на пересекающиеся периоды;\
	•	Для физических лиц (Individual) и ИП (Entrepreneur) — водитель может быть автоматически создан;\
	•	Для организаций (Organization) — водитель обязателен;\
	•	Бронирование фиксирует состояние клиента и актива на момент создания.

⸻

### Архитектурное решение

Проект реализован в стиле Clean Architecture с разделением на слои:

RentFlow\
├── Api                → HTTP слой (Controllers, Middleware)\
├── Application        → use-cases, команды, обработчики (MediatR)\
├── Domain             → бизнес-логика, агрегаты, value objects\
├── Persistance        → EF Core, репозитории, инфраструктура\
├── Test               → интеграционные тесты

Основные принципы:\
	•	Инверсия зависимостей (Domain не зависит от инфраструктуры);\
	•	Явные use-cases (через MediatR);\
	•	Изоляция бизнес-логики.

⸻

В проекте используется **DDD** (Domain-Driven Design), потому что:\
	•	Сложные бизнес-правила:\
	  - логика создания водителя;\
	  - ограничения на бронирование.\
	•	Явные доменные модели:\
	  - Booking;\
	  - Customer (с типами);\
	  - Driver.\
	•	Value Objects:\
	  - PersonName;\
	  - DriverLicense;\
	  - RentalPeriod.

DDD позволяет:\
	•	держать бизнес-логику в одном месте;\
	•	избегать анемичных моделей;\
	•	делать систему расширяемой.

⸻

В Booking используются **snapshot’ы**:

BookingCustomerSnapshot;\
BookingAssetSnapshot.

Это необходимо, поскольку данные могут измениться со временем, но бронирование должно отражать состояние на момент создания.

⸻

Используется паттерн **Unit of Work**, потому что:

Создание бронирования включает:\
	•	проверку конфликтов;\
	•	загрузку сущностей;\
	•	создание агрегата;\
	•	сохранение.

```
await _uow.BeginTransactionAsync();\
...\
await _uow.SaveChangesAsync();\
await _uow.CommitAsync();
```

Это дает:\
	•	защиту от частичных изменений;\
	•	контроль транзакций;\
	•	единый commit.

⸻

ER-диаграмма:

![RentFlow_ER](https://github.com/user-attachments/assets/b1674bd9-4510-44a9-9a2c-a9736024dc85)


⸻

### Инструкция запуска

**1. Локальный запуск через Docker**
```
docker compose -f docker-compose.prod.yml up -d
```
После запуска:\
	•	API: http://localhost:8080 \
	•	Swagger: http://localhost:8080/swagger

⸻

**2. Переменные окружения**

Создать .env файл:
```
POSTGRES_PASSWORD=yourpassword
```
**3. Миграции**

Миграции применяются автоматически при старте приложения (кроме тестового окружения).

⸻

**4. Запуск тестов**
```
dotnet test
```
Тесты используют:\
	•	Testcontainers (PostgreSQL);\
	•	Respawn (очистка БД).

⸻

**5. CI**

Pipeline включает:\
	•	сборку проекта;\
	•	запуск тестов;\
	•	fail при ошибках.

### GitHub Actions:

.github/workflows/ci.yml

### Тестирование

Используются интеграционные тесты:\
	•	реальная БД (PostgreSQL);\
	•	контейнеры;\
	•	изоляция через Respawn.

Проверяются:\
	•	бизнес-правила;\
	•	ограничения БД;\
	•	корректность транзакций.

⸻

### Технологии:
	•	.NET 9.0
	•	ASP.NET Core
	•	Entity Framework Core
	•	PostgreSQL
	•	MediatR
	•	FluentValidation
	•	Serilog
	•	Testcontainers
	•	Respawn
	•	Docker / Docker Compose
	•	GitHub Actions


