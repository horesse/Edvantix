# Requirements: Edvantix

**Defined:** 2026-03-18
**Core Value:** Менеджер школы может создать расписание группы, студент видит свои уроки и баланс, учитель видит свои занятия — всё в рамках одной школы с изолированными данными.

## v1 Requirements

### Organizations

- [x] **ORG-01**: Владелец школы может создать кастомную роль с именем в рамках своей школы
- [x] **ORG-02**: Владелец может просматривать, редактировать и удалять роли своей школы
- [x] **ORG-03**: Владелец может назначить набор permissions на роль (permission = строка-идентификатор CQRS-команды)
- [x] **ORG-04**: Сервисы автоматически регистрируют свои permission-строки в каталоге при старте
- [x] **ORG-05**: Владелец может назначить роль пользователю в рамках своей школы
- [x] **ORG-06**: Владелец может отозвать роль у пользователя
- [x] **ORG-07**: Organizations service предоставляет gRPC-эндпоинт CheckPermission(userId, schoolId, permission) → bool
- [x] **ORG-08**: Результат CheckPermission кешируется в HybridCache с инвалидацией по событию смены роли
- [x] **ORG-09**: При изменении роли/назначения публикуется интеграционное событие для инвалидации кеша в других сервисах
- [x] **ORG-10**: EF Core global query filter по schoolId во всех агрегатах (tenant isolation)

### Scheduling

- [ ] **SCH-01**: Менеджер может создать слот расписания (группа + дата/время + учитель) с проверкой конфликтов учителя
- [ ] **SCH-02**: Менеджер может редактировать и удалять слоты расписания
- [ ] **SCH-03**: Менеджер видит расписание всех групп своей школы
- [ ] **SCH-04**: Учитель видит только своё расписание
- [ ] **SCH-05**: Студент видит только свои уроки (группы, в которых состоит)
- [ ] **SCH-06**: Менеджер может добавить студента в группу
- [ ] **SCH-07**: Менеджер может удалить студента из группы
- [ ] **SCH-08**: На уроке можно отметить присутствие/отсутствие каждого студента
- [ ] **SCH-09**: При фиксации посещаемости публикуется событие AttendanceRecorded через Kafka (с outbox)
- [ ] **SCH-10**: Все даты/время хранятся как DateTimeOffset

### Payments

- [ ] **PAY-01**: Менеджер может вручную добавить кредиты (уроки) на баланс студента в рамках школы
- [ ] **PAY-02**: При получении AttendanceRecorded автоматически списывается 1 урок с баланса студента (idempotent)
- [ ] **PAY-03**: Студент и менеджер видят баланс студента: куплено / использовано / остаток
- [ ] **PAY-04**: На конкретном слоте расписания отображается статус оплаты студента (оплачен / не оплачен / долг)
- [ ] **PAY-05**: Баланс хранится как append-only ledger (LessonTransaction с Credit/Debit)
- [ ] **PAY-06**: Менеджер может вручную скорректировать баланс (ручное списание/добавление)

## v2 Requirements

### Scheduling

- **SCH-V2-01**: Учебная программа (curriculum) учителя — план тем по урокам
- **SCH-V2-02**: Повторяющиеся слоты (weekly recurrence)
- **SCH-V2-03**: Уведомления об изменениях расписания

### Payments

- **PAY-V2-01**: Интеграция с платёжными системами (Stripe, ЮKassa)
- **PAY-V2-02**: Автоматические напоминания о низком балансе
- **PAY-V2-03**: История транзакций для студента

### Organizations

- **ORG-V2-01**: Пользователь в нескольких школах одновременно (multi-school token)
- **ORG-V2-02**: Шаблоны ролей (predefined role sets)

## Out of Scope

| Feature | Reason |
|---------|--------|
| Интеграция с платёжными системами | v1 — только отображение и ручное управление |
| Keycloak Authorization Services | Кастомная RBAC даёт гибкость; Keycloak AuthZ слишком жёсткий |
| Real-time уведомления (WebSocket/SSE) | Не нужны в v1, добавить после |
| Мобильное приложение | Веб-первично |
| PostgreSQL Row-Level Security | Несовместимо с PgBouncer transaction mode; используем EF Core global filters |
| Программа/учебный план учителя | Опциональная фича, defer в v2 |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| ORG-01 | Phase 1 | Complete |
| ORG-02 | Phase 1 | Complete |
| ORG-03 | Phase 1 | Complete |
| ORG-04 | Phase 1 | Complete |
| ORG-05 | Phase 1 | Complete |
| ORG-06 | Phase 1 | Complete |
| ORG-07 | Phase 1 | Complete |
| ORG-08 | Phase 1 | Complete |
| ORG-09 | Phase 1 | Complete |
| ORG-10 | Phase 1 | Complete |
| SCH-01 | Phase 2 | Pending |
| SCH-02 | Phase 2 | Pending |
| SCH-03 | Phase 2 | Pending |
| SCH-04 | Phase 2 | Pending |
| SCH-05 | Phase 2 | Pending |
| SCH-06 | Phase 2 | Pending |
| SCH-07 | Phase 2 | Pending |
| SCH-08 | Phase 2 | Pending |
| SCH-09 | Phase 2 | Pending |
| SCH-10 | Phase 2 | Pending |
| PAY-01 | Phase 3 | Pending |
| PAY-02 | Phase 3 | Pending |
| PAY-03 | Phase 3 | Pending |
| PAY-04 | Phase 3 | Pending |
| PAY-05 | Phase 3 | Pending |
| PAY-06 | Phase 3 | Pending |

**Coverage:**
- v1 requirements: 26 total
- Mapped to phases: 26
- Unmapped: 0 ✓

---
*Requirements defined: 2026-03-18*
*Last updated: 2026-03-18 after initial definition*
