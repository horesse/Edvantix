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
| `PUT /profile` (with avatar in body) | **Changed** | Split: `PUT /profile` (JSON) + `PUT /profile/avatar` (form-data) |

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

> **Breaking:** Avatar is **no longer part of this request**. Update avatar separately via `PUT /profile/avatar`.

**Request** `Content-Type: application/json`:
```json
{
  "firstName": "Иван",
  "lastName": "Иванов",
  "middleName": "Сергеевич",
  "gender": 1,
  "birthDate": "1990-01-15",
  "contacts": [
    { "type": 1, "value": "+7 999 000 00 00", "description": null }
  ],
  "educations": [
    {
      "dateStart": "2007-09-01",
      "dateEnd": "2012-06-30",
      "institution": "МГУ",
      "specialty": "Прикладная математика",
      "level": 3
    }
  ],
  "employmentHistories": [
    {
      "workplace": "ООО Рога и Копыта",
      "position": "Senior Developer",
      "startDate": "2020-03-01T00:00:00Z",
      "endDate": null,
      "description": null
    }
  ]
}
```

> **Important:** `contacts`, `educations`, `employmentHistories` are **full-replace** lists.
> Sending an empty array `[]` deletes all existing records of that type.

**Response** `200 OK`: `ProfileViewModel` (same as `GET /profile`).

---

### `PUT /profiles/{id}` — Update profile by ID (admin only)

Same request body as `PUT /profile`. Returns `200 OK` with `ProfileViewModel`.

> **Breaking:** Was `NoContent` (204), now returns `200 OK`.

---

### `PUT /profile/avatar` — Upload avatar **(New)**

Uploads or replaces the avatar for the current user. Previously this was part of the `PUT /profile` form-data request.

**Request** `Content-Type: multipart/form-data`:
- `avatar` — image file (JPEG or PNG, max 5 MB)

**Response** `200 OK`: `ProfileViewModel` with the new `avatarUrl` (SAS URL).

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
| 2 | `PUT /profile` no longer accepts `multipart/form-data`; now JSON-only | Update request serialization |
| 3 | `PUT /profile` returns `200 ProfileViewModel` instead of `204 NoContent` | Handle new response body |
| 4 | `PUT /profiles/{id}` returns `200 ProfileViewModel` instead of `204 NoContent` | Handle new response body |
| 5 | Avatar upload moved to `PUT /profile/avatar` | Separate API call for avatar changes |
| 6 | `GET /profile/own` → `GET /profile` (route rename) | Update all references |
| 7 | `POST /profile/registration` response body is `number`, not `string` | Parse as number |
| 8 | Contacts/Educations/EmploymentHistories are **full-replace** on every update | Do not send partial lists |
