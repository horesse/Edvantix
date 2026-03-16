# Backend Requirements

## Profile Service

### Avatar Upload Endpoint

The profile settings page supports uploading a custom photo.
The backend needs to accept this file and return a stored URL.

**Required endpoint:** `POST /api/profiles/avatar`

```
Request:
  Content-Type: multipart/form-data
  Body: { file: <image file> }

Response 200:
  { avatarUrl: "https://cdn.edvantix.com/avatars/{id}.webp" }
```

- Maximum file size: 5 MB
- Accepted MIME types: `image/jpeg`, `image/png`, `image/webp`, `image/gif`
- Images should be resized/compressed server-side to max 256×256 px
- Returned URL should be publicly accessible (CDN preferred)

**Also required:** `DELETE /api/profiles/avatar`

```
Response 204: (no content)
```

Removes the avatar and falls back to initials. Currently the "Удалить фото" button
in the profile settings UI shows a toast; it will be wired once this endpoint exists.

---

### Update Personal Info — Add Gender Field

`PATCH /api/profiles/me/personal` currently accepts `firstName`, `lastName`,
`middleName`, `birthDate`. The profile edit form now also sends `gender`.

**Updated request payload:**

```json
{
  "firstName": "Иван",
  "lastName": "Иванов",
  "middleName": "Иванович",
  "birthDate": "1990-05-20",
  "gender": 1
}
```

Gender enum values (integer):
- `1` — Male
- `2` — Female
- `3` — None / not specified

**Updated response** should include `gender` in `OwnProfileDetails`:

```json
{
  "id": "...",
  "accountId": "...",
  "login": "ivan@example.com",
  "firstName": "Иван",
  "lastName": "Иванов",
  "middleName": "Иванович",
  "birthDate": "1990-05-20T00:00:00Z",
  "gender": 1,
  "avatarUrl": null,
  "contacts": [],
  "educations": [],
  "employmentHistories": []
}
```

**Current workaround:** `gender` is present in `OwnProfileDetails` (read) but was missing
from the update request. Frontend type `UpdatePersonalInfoRequest` has been updated
to include `gender: Gender`.

---

### Profile Last Updated Timestamp

The profile settings bottom bar displays the date and time the profile was last updated.

**Required:** `OwnProfileDetails` response should include `updatedAt`.

**Updated response:**

```json
{
  "id": "...",
  "updatedAt": "2026-03-15T14:32:00Z",
  ...
}
```

Frontend type `OwnProfileDetails` has been updated with `updatedAt?: string | null`.

**Current workaround:** If `updatedAt` is missing from the response, the UI shows
"Профиль ещё не обновлялся".

---

### Profile Bio

The profile settings page includes a free-text "О себе" field (max 600 characters).

**Required endpoint:** `PATCH /api/profiles/me/bio`

```json
Request:
  { "bio": "Преподаватель Python с 8-летним опытом…" }

  // or null to clear:
  { "bio": null }

Response 200: OwnProfileDetails (full profile with updated bio)
```

**Updated response** should include `bio` in `OwnProfileDetails`:

```json
{
  "bio": "Преподаватель Python с 8-летним опытом…",
  ...
}
```

Validation rules:
- Maximum 600 characters
- `null` or empty string clears the bio

**Current workaround:** `TabBio` component stores bio in local state and calls
`PATCH /api/profiles/me/bio` on save. Until the endpoint exists, saving bio will
return a 404 and show an error toast.

---

### Profile Subjects / Expertise Tags

The profile settings left panel shows a subjects/tags card where users can add their
areas of expertise (e.g., "Python", "Data Science", "ML").

**Required endpoints:**

`GET /api/profiles/me/subjects`

```json
Response 200:
  { "subjects": ["Python", "Data Science", "ML"] }
```

`PUT /api/profiles/me/subjects`

```json
Request:
  { "subjects": ["Python", "Data Science", "SQL"] }

Response 200:
  { "subjects": ["Python", "Data Science", "SQL"] }
```

Rules:
- Maximum 20 subjects per profile
- Each subject: max 50 characters, trimmed whitespace
- Case-sensitive (server should not deduplicate)

**Current workaround:** `SubjectsCard` uses local React state only; changes are not
persisted. Wire `useUpdateSubjects` hook once this endpoint is available.

---

### Profile Members Count

The sidebar `OrganizationSelector` component displays member count badges in the navigation
(Участники, Ученики, Учителя). These counts need to come from the API.

**Required endpoint:** `GET /api/organizations/{id}/stats`

```json
Response 200:
  {
    "totalMembers": 312,
    "studentCount": 248,
    "teacherCount": 32,
    "activeCount": 298,
    "pendingCount": 14
  }
```

**Current workaround:** Counts are not displayed in the sidebar (badges removed pending this endpoint).

---

### Members Sub-routes

The sidebar navigation includes `/organization/members/students` and
`/organization/members/teachers` as dedicated filtered views.

**Required:** The members list API (`GET /api/organizations/{id}/members`) should support
a `role` query parameter to filter by `Student` or `Teacher`:

```
GET /api/organizations/{id}/members?role=Student
GET /api/organizations/{id}/members?role=Teacher
```

---

### Attendance API

The attendance page (`/school/attendance`) is implemented with mock data.
Real data must come from the following endpoints.

**GET /api/organizations/{orgId}/attendance**

```
Query params:
  groupId:   string   (UUID of the group)
  weekStart: string   (ISO date, Monday of the week, e.g. "2026-03-13")

Response 200:
{
  "weekStart": "2026-03-13",
  "group": { "id": "...", "name": "Python для начинающих" },
  "days": ["2026-03-13", "2026-03-14", "2026-03-15", "2026-03-16", "2026-03-17"],
  "records": [
    {
      "studentId": "...",
      "studentName": "Александров Д.А.",
      "attendance": {
        "2026-03-13": "present",   // "present" | "late" | "absent" | "excused" | null
        "2026-03-14": "present",
        ...
      }
    }
  ]
}
```

**PATCH /api/organizations/{orgId}/attendance**

```json
Request:
{
  "groupId": "...",
  "date": "2026-03-15",
  "records": [
    { "studentId": "...", "status": "present" },
    { "studentId": "...", "status": "absent"  }
  ]
}

Response 200:
{ "updated": 24 }
```

**GET /api/organizations/{orgId}/attendance/stats**

```json
Response 200:
{
  "weekStart": "2026-03-13",
  "presentPct": 87,
  "latePct": 7,
  "absentPct": 6,
  "trendVsPreviousWeek": -3
}
```

**Current workaround:** Frontend uses fully mocked data. To wire real data,
replace `MOCK_STUDENTS`, `MOCK_RECORDS` in `attendance-page.tsx` with
`useAttendance(orgId, groupId, weekStart)` hook (to be created).
