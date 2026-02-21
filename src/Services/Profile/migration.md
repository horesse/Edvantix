---
title: Persona API — Migration Guide (v1)
description: Breaking changes in the Persona (Profile) service API. Required for frontend migration.
---

## Overview

The Profile service has been renamed from **ProfileService** to **Persona** and significantly reworked.
All endpoints are under the versioned prefix `/api/v1/`.

---

## Removed Endpoints

| Old endpoint | Status | Replacement |
|---|---|---|
| `GET /profile/own` | **Removed** | `GET /profile` |
| `GET /profiles/{id}/own` | **Removed** | `GET /profiles/{id}` |
| `GET /profile/details/own` | **Removed** | `GET /profile/details` |
| `PUT /profile` (JSON) | **Changed** | `PUT /profile` (multipart/form-data, avatar included) |
| `PUT /profile/avatar` | **Removed** | Avatar now sent in `PUT /profile` as `avatar` field |

---

## Endpoint Changes

### `GET /profile` — Own short profile

Returns the short profile of the currently authenticated user.

**Response** `200 OK`:
```json
{
  "id": 42,
  "name": "Иван Иванов",
  "userName": "ivan.ivanov",
  "avatarUrl": "https://..."
}
```

> **Breaking:** `id` type changed from `string` (formerly `long`) to `number` (`ulong`).

---

### `GET /profiles/{id}` — Profile by ID (admin only)

Returns the short profile for any user. Requires `Admin` role.

**Path params:**
- `id` — profile ID (`ulong`)

**Response** `200 OK`: same shape as `GET /profile`.

---

### `GET /profile/details` — Own full profile

Returns the full profile of the currently authenticated user, including contacts, employment history and education.

**Response** `200 OK`:
```json
{
  "id": 42,
  "accountId": "550e8400-e29b-41d4-a716-446655440000",
  "login": "ivan.ivanov",
  "gender": 1,
  "birthDate": "1990-01-15",
  "firstName": "Иван",
  "lastName": "Иванов",
  "middleName": "Сергеевич",
  "avatarUrl": "https://...",
  "contacts": [
    { "type": 1, "value": "+7 999 000 00 00", "description": null }
  ],
  "employmentHistories": [
    {
      "workplace": "ООО Рога и Копыта",
      "position": "Senior Developer",
      "startDate": "2020-03-01T00:00:00Z",
      "endDate": null,
      "description": null
    }
  ],
  "educations": [
    {
      "dateStart": "2007-09-01",
      "dateEnd": "2012-06-30",
      "institution": "МГУ",
      "specialty": "Прикладная математика",
      "educationLevel": 3
    }
  ]
}
```

> **New:** This endpoint replaces the previous separate `GetOwnProfileDetails` call.

---

### `GET /profiles/{id}/details` — Full profile by ID (admin only)

Same shape as `GET /profile/details`. Requires `Admin` role.

---

### `PUT /profile` — Update own profile

> **Breaking:** Was `NoContent` (204), now returns `200 OK` with the updated `ProfileViewModel`.

> **Breaking:** Was JSON (`application/json`), now **`multipart/form-data`**. Avatar is included directly in this request.

**Request** `Content-Type: multipart/form-data`:
| Field | Type | Notes |
|---|---|---|
| `firstName` | `string` | Required |
| `lastName` | `string` | Required |
| `middleName` | `string` | Optional |
| `birthDate` | `string` (`YYYY-MM-DD`) | Required |
| `contacts[n].type` | `number` | ContactType enum value |
| `contacts[n].value` | `string` | |
| `contacts[n].description` | `string` | Optional |
| `educations[n].dateStart` | `string` (`YYYY-MM-DD`) | |
| `educations[n].dateEnd` | `string` (`YYYY-MM-DD`) | Optional |
| `educations[n].institution` | `string` | |
| `educations[n].specialty` | `string` | Optional |
| `educations[n].level` | `number` | EducationLevel enum value |
| `employmentHistories[n].workplace` | `string` | |
| `employmentHistories[n].position` | `string` | |
| `employmentHistories[n].startDate` | `string` (ISO 8601) | |
| `employmentHistories[n].endDate` | `string` (ISO 8601) | Optional |
| `employmentHistories[n].description` | `string` | Optional |
| `avatar` | `file` | Optional, JPEG/PNG, max 1 MB |

> **Breaking:** `gender` is **removed** from the update request. Gender is set only at registration and cannot be changed.

> **Important:** `contacts`, `educations`, `employmentHistories` are **full-replace** lists.
> Sending an empty array deletes all existing records of that type.

**Response** `200 OK`: `ProfileViewModel` (same as `GET /profile`).

---

### `PUT /profiles/{id}` — Update profile by ID (admin only)

Same request body as `PUT /profile`. Returns `200 OK` with `ProfileViewModel`.

> **Breaking:** Was `NoContent` (204), now returns `200 OK`.

---

### `POST /profile/registration` — Registration

> **Breaking:** The `201 Created` response body type changed from `string` to `number` (`ulong` profile ID).

> **Breaking:** The `Location` header now points to `/api/v1/profiles/{id}` (previously `/api/v1/profile/{id}`).

**Request** `Content-Type: multipart/form-data`:
- `firstName` (required)
- `lastName` (required)
- `middleName` (optional)
- `birthDate` (required, `YYYY-MM-DD`)
- `gender` (required, integer)
- `avatar` (optional, image file)

---

## Type Reference

### `ProfileViewModel`

| Field | Type | Notes |
|---|---|---|
| `id` | `number` | **Breaking:** was `string` |
| `name` | `string` | Full name (formatted) |
| `userName` | `string` | Keycloak preferred_username / login |
| `avatarUrl` | `string \| null` | Temporary SAS URL; may expire |

### `Gender` enum (integer)

Use the numeric value in requests/responses. Exact values are defined in the OpenAPI spec.

### `ContactType` enum (integer)

Use the numeric value. Exact values are defined in the OpenAPI spec.

### `EducationLevel` enum (integer)

Use the numeric value. Exact values are defined in the OpenAPI spec.

---

## Summary of Breaking Changes

| # | Change | Impact |
|---|---|---|
| 1 | `ProfileViewModel.id` type: `string` → `number` (`ulong`) | All places rendering/storing profile ID |
| 2 | `PUT /profile` changed from JSON to `multipart/form-data`; avatar included as `avatar` field | Update request format and add avatar field |
| 3 | `PUT /profile` returns `200 ProfileViewModel` instead of `204 NoContent` | Handle new response body |
| 4 | `PUT /profiles/{id}` returns `200 ProfileViewModel` instead of `204 NoContent` | Handle new response body |
| 5 | `PUT /profile/avatar` removed; avatar upload merged into `PUT /profile` | Remove separate avatar call |
| 6 | `gender` removed from `PUT /profile` and `PUT /profiles/{id}` request body | Remove gender from update payloads |
| 7 | `GET /profile/own` → `GET /profile` (route rename) | Update all references |
| 8 | `POST /profile/registration` response body is `number`, not `string` | Parse as number |
| 9 | Contacts/Educations/EmploymentHistories are **full-replace** on every update | Do not send partial lists |
| 10 | Avatar max size reduced from 5 MB to **1 MB**; allowed types limited to **JPEG and PNG** | Validate before upload |
