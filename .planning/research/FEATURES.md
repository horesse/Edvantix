# Feature Research

**Domain:** Online school management SaaS — Organizations/RBAC, Scheduling, Payments
**Researched:** 2026-03-18
**Confidence:** HIGH (RBAC and scheduling patterns), MEDIUM (payment tracking patterns)

## Feature Landscape

This document covers three bounded contexts for the Edvantix milestone: Organizations service
(custom RBAC), Scheduling service (lesson slots + attendance), and Payments service (package
tracking, no payment processing).

---

## Organizations Service — Custom RBAC

### Table Stakes (Users Expect These)

Features a school owner assumes the system can do. Missing these makes the system unusable.

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Predefined system roles (Owner, Manager, Teacher, Student) | Every school needs a baseline; starting from zero is friction | LOW | Ship as seed data; owner cannot delete these |
| Create custom role with a name | Owner needs to model school-specific positions (e.g., "Curator", "Metodist") | LOW | Scoped to tenant; names unique per tenant |
| Assign permissions to a role | Permissions are the unit of access; roles without them are useless | MEDIUM | Permission = CQRS command identifier (e.g., `scheduling.create-slot`) |
| Assign a role to a user within a school | Users without role assignment cannot act | LOW | User can hold one role per school in v1 |
| View current role assignments for a school | Manager needs to audit who has what access | LOW | Read-only list view per tenant |
| Remove/revoke a role from a user | Staff leave; permissions must be revocable | LOW | Soft: unassign role, user loses all derived permissions |
| Enforce permissions at API boundary | Authorization is meaningless without enforcement | HIGH | Each service queries Organizations or uses cached permission set; see Architecture |
| Tenant isolation of roles | School A's roles must never bleed into School B | MEDIUM | All role/permission lookups scoped by `tenantId` |

### Differentiators (Competitive Advantage)

| Feature | Value Proposition | Complexity | Notes |
|---------|-------------------|------------|-------|
| Owner can define permission sets (role templates) | Large schools clone roles instead of rebuilding permissions from scratch | MEDIUM | "Clone role" operation; useful when school has multiple teachers with slight variations |
| Permission discovery UI (list all available permissions per service) | Onboarding friction drops when owner can browse what permissions exist | MEDIUM | Requires services to register/expose their permission catalogue |
| Audit log of role/permission changes | Compliance and debugging: "who granted X to Y and when?" | MEDIUM | Event-sourced: emit `RoleAssigned`, `PermissionGranted` domain events |
| User can hold multiple roles | Some users (e.g., teacher who also manages schedule) need combined access | MEDIUM | Defer to v1.x — increases permission evaluation complexity |

### Anti-Features

| Feature | Why Requested | Why Problematic | Alternative |
|---------|---------------|-----------------|-------------|
| Sync roles to Keycloak | Seems natural since Keycloak already exists | Keycloak AuthZ is rigid; defeats the point of custom RBAC; creates two sources of truth | Keep Keycloak as AuthN only; Organizations service is the single authority for AuthZ |
| Field-level permissions (e.g., "can see salary but not phone") | Power users always ask for this | Exponential complexity; not needed for scheduling/payments domain | Use role-level permissions with feature flag approach; defer until clear demand |
| Hierarchical roles (role inheriting another role) | Feels natural for org charts | Inheritance chains are hard to reason about and audit | Flat roles with explicit permission assignment; clone a role if starting point is needed |
| Per-resource permissions (e.g., "manage only Group A") | Fine-grained control for large schools | Massive complexity increase; requires resource ownership model across all services | Use role-based access at service level; resource-level filtering is a v2 concern |

---

## Scheduling Service

### Table Stakes (Users Expect These)

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Create a lesson slot (group + teacher + date/time + duration) | Core manager action; no slot = no lessons | MEDIUM | Slot must reference a group and a teacher; duration required for attendance deduction |
| View all school slots in a calendar (manager view) | Manager needs oversight of the entire schedule | MEDIUM | Weekly/list view; filterable by group, teacher, date range |
| View own upcoming slots (teacher view) | Teacher plans their day by seeing only their lessons | LOW | Filter on `teacherId`; same calendar component, different filter |
| View own upcoming lessons (student view) | Student needs to know when and where to show up | LOW | Filter on `studentId` via group membership; no raw slot access |
| Mark attendance per student per slot | Attendance is the trigger for lesson deduction; useless system without it | MEDIUM | Manager or teacher marks: Present / Absent / Excused |
| View attendance for a slot (who was present) | Teacher and manager need post-lesson record | LOW | Read-only list; filterable |
| Conflict detection: teacher double-booking | Two slots same teacher same time = scheduling error | MEDIUM | Server-side check on slot creation/update; return domain error |
| Edit a slot (reschedule, change teacher) | Mistakes happen; manager needs correction ability | MEDIUM | Must re-validate conflicts on edit; notify affected parties in v1.x |
| Cancel / delete a slot | Lessons get cancelled; slot must be removable | LOW | Soft delete preferred; attendance records preserved |
| Group membership (student belongs to group) | Slot is assigned to group, not individual students | MEDIUM | Groups managed in Organizations service or Scheduling service; decision needed (see deps) |

### Differentiators

| Feature | Value Proposition | Complexity | Notes |
|---------|-------------------|------------|-------|
| Recurring lesson slots (weekly pattern) | Most lessons are recurring; entering each one manually is painful | HIGH | Weekly recurrence with end date or count; complex because editing one vs all instances |
| Color-coded calendar by group or teacher | Visual overview dramatically reduces scheduling errors | LOW | Pure frontend; groups/teachers assigned colors |
| Drag-and-drop rescheduling | Faster manager UX for moving slots around | HIGH | Frontend complexity; requires optimistic update with server conflict check |
| Attendance statistics per student | "Student X missed 30% of lessons" surfaces retention risk | MEDIUM | Aggregate query; useful for manager and parent views |
| iCal/Google Calendar export | Teachers want their work schedule in personal calendar | MEDIUM | Standard iCal feed; one-way read-only export |

### Anti-Features

| Feature | Why Requested | Why Problematic | Alternative |
|---------|---------------|-----------------|-------------|
| Real-time slot notifications (push/WebSocket) | Users want instant updates when schedule changes | Out of scope for this milestone; adds infrastructure complexity (SignalR/SSE) | Polling with reasonable interval (30s) for calendar refresh; push in v2 |
| Student self-booking | Online tutoring platforms offer this | Edvantix model is manager-controlled scheduling, not marketplace; conflicts with group model | Manager creates slots; student only views |
| Room/location management | Physical schools need rooms | Online school context; rooms are not relevant to v1 | Slot has optional `meetingUrl` field for online link |
| Automatic lesson deduction on attendance mark | Seems logical to trigger payment deduction immediately | Cross-service transaction is complex; better to have Payments service subscribe to attendance events | Publish `AttendanceMarked` domain event; Payments service reacts asynchronously |

---

## Payments Service

### Table Stakes (Users Expect These)

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Create a lesson package for a student (N lessons purchased) | Core data model; without this there is nothing to track | LOW | Package has: studentId, total lessons, start date, optional expiry date |
| View student balance (purchased / used / remaining) | Every stakeholder needs this; it is the core value of the service | LOW | Computed from package + attendance events; display in student profile |
| Mark a lesson as paid / unpaid (manual) | v1 has no payment processor; manager manually records payment received | LOW | Per-lesson payment status; simple boolean + timestamp + recorded-by |
| View payment status per lesson (for a student) | Manager needs to know which lessons have been settled | LOW | List view: lesson date, status, package credited from |
| Deduct lesson from package when attendance marked | The "ledger" update; attendance = consumption | MEDIUM | Subscribe to `AttendanceMarked` event from Scheduling; apply deduction to active package |
| View all students with low/zero balance (manager alert) | Manager needs to follow up on students about to run out | MEDIUM | Query: remaining lessons <= threshold (e.g., 2); manager dashboard widget |
| Multiple packages per student (sequential) | Students buy new packages before old one runs out | MEDIUM | Packages consumed in order of purchase date; active package = oldest with remaining lessons |
| Package history for a student | Manager reviews purchase history during billing conversation | LOW | Chronological list of all packages with status (active / exhausted / expired) |

### Differentiators

| Feature | Value Proposition | Complexity | Notes |
|---------|-------------------|------------|-------|
| Package expiry date with warning | Schools often sell packages with validity period (e.g., 3 months) | LOW | Expiry date on package; warning shown when near expiry; expired lessons not deducted |
| Discounts / adjustments on packages (manual) | School gives a student 2 bonus lessons; manager needs to record this | LOW | Manual adjustment entry with reason; affects remaining balance |
| Per-student payment status overview (all packages, all lessons) | Single screen shows full picture; reduces manager context switching | MEDIUM | Aggregated view across packages and attendance |
| Export payment data to CSV | Manager sends report to accountant | LOW | Simple CSV export; no external integration needed |

### Anti-Features

| Feature | Why Requested | Why Problematic | Alternative |
|---------|---------------|-----------------|-------------|
| Payment gateway integration (Stripe, YooKassa) | Natural evolution; saves manual work | Out of scope for v1 per PROJECT.md; integration adds compliance, webhook handling, refund logic | Manual recording now; design package model so payment reference field can be added later |
| Invoice generation and sending | Professional schools want PDF invoices | Adds PDF generation, email sending infrastructure | Export data to CSV; manager uses accounting tool |
| Automatic payment reminders | Nice automation | Requires notification infrastructure (email/SMS) not in this milestone | Manager views low-balance list and contacts manually |
| Subscription / recurring billing model | Some schools use monthly fees | Different model from lesson packages; mixing both in v1 adds confusion | Package model only in v1; subscription is a separate billing model to add in v2 |

---

## Feature Dependencies

```
[Create Slot]
    └──requires──> [Group Membership Model]
                       └──requires──> [User Profiles] (existing)

[Mark Attendance]
    └──requires──> [Create Slot]
    └──publishes──> [AttendanceMarked event]

[AttendanceMarked event]
    └──consumed by──> [Deduct Lesson from Package]
                          └──requires──> [Create Package]

[View Student Balance]
    └──requires──> [Create Package]
    └──requires──> [Deduct Lesson from Package]

[Enforce Permissions at API Boundary]
    └──requires──> [Assign Role to User]
    └──requires──> [Assign Permissions to Role]

[View Teacher Schedule]
    └──requires──> [Create Slot]
    └──requires──> [Assign Role to User] (to identify who is a teacher)

[View Student Lessons]
    └──requires──> [Group Membership Model]
    └──requires──> [Create Slot]
```

### Dependency Notes

- **Mark Attendance requires Create Slot:** Attendance is per-slot; no slot = no attendance record.
- **Deduct Lesson requires AttendanceMarked event:** Cross-service communication must be asynchronous via domain event; synchronous call creates tight coupling.
- **Group Membership requires decision:** Groups can live in Organizations service (org structure) or Scheduling service (scheduling concern). Scheduling service is the natural owner since groups are scheduling constructs; Organizations owns users and roles.
- **Enforce Permissions requires populated permission catalogue:** Services must register their permissions before owners can assign them. Ship a seed of all `scheduling.*`, `payments.*`, `organizations.*` permissions on deploy.

---

## MVP Definition

### Launch With (v1 — this milestone)

**Organizations / RBAC:**
- [ ] Predefined system roles with seed permissions — establishes baseline without configuration burden
- [ ] Create / update / delete custom roles — owner can model their school
- [ ] Assign permissions to role from a fixed catalogue — enables access control
- [ ] Assign / revoke role for a user within a school — completes the RBAC loop
- [ ] Permission enforcement middleware in each service — security is non-negotiable

**Scheduling:**
- [ ] Create, edit, cancel lesson slots (group + teacher + time + duration) — core manager action
- [ ] Conflict detection on teacher double-booking — prevents obvious scheduling errors
- [ ] Manager calendar view (all school slots, filterable) — manager oversight
- [ ] Teacher view (own slots) — teacher daily workflow
- [ ] Student view (own lessons via group membership) — student self-service
- [ ] Mark attendance per student per slot (Present / Absent / Excused) — triggers payments

**Payments:**
- [ ] Create lesson package for a student (N lessons) — core ledger entry
- [ ] Deduct lesson on attendance marked event — automatic consumption tracking
- [ ] View student balance (purchased / used / remaining) — core display requirement
- [ ] Manual paid/unpaid flag per lesson — v1 payment confirmation workflow
- [ ] Low-balance student list for manager — proactive follow-up tool

### Add After Validation (v1.x)

- [ ] Recurring lesson slots — triggered when managers report manual entry fatigue
- [ ] Package expiry dates with warnings — triggered when schools report time-limited packages
- [ ] Attendance statistics per student (absence rate) — triggered when retention data is needed
- [ ] Multiple packages sequential consumption — triggered when first students exhaust initial package
- [ ] Audit log for role/permission changes — triggered when first compliance question arrives
- [ ] iCal export for teachers — triggered by teacher requests

### Future Consideration (v2+)

- [ ] Payment gateway integration — separate milestone; requires compliance and webhook infra
- [ ] Real-time schedule change notifications — requires SignalR or SSE infrastructure
- [ ] Per-resource permissions (manage only Group A) — requires resource ownership model
- [ ] Drag-and-drop calendar scheduling — pure UX enhancement, not blocking workflow
- [ ] Subscription / recurring billing model — second billing model, adds confusion before PMF

---

## Feature Prioritization Matrix

| Feature | User Value | Implementation Cost | Priority |
|---------|------------|---------------------|----------|
| Permission enforcement at API boundary | HIGH | HIGH | P1 |
| Create / assign roles and permissions | HIGH | MEDIUM | P1 |
| Create lesson slot with conflict detection | HIGH | MEDIUM | P1 |
| Mark attendance | HIGH | MEDIUM | P1 |
| Student balance display | HIGH | LOW | P1 |
| Create lesson package | HIGH | LOW | P1 |
| Manager calendar view | HIGH | MEDIUM | P1 |
| Teacher / student filtered views | HIGH | LOW | P1 |
| Manual paid/unpaid flag | MEDIUM | LOW | P1 |
| Low-balance student list | MEDIUM | MEDIUM | P2 |
| Recurring lesson slots | HIGH | HIGH | P2 |
| Package expiry with warning | MEDIUM | LOW | P2 |
| Attendance statistics | MEDIUM | MEDIUM | P2 |
| Role clone / template | LOW | MEDIUM | P3 |
| iCal export | LOW | MEDIUM | P3 |
| CSV payment export | LOW | LOW | P3 |

**Priority key:**
- P1: Must have for launch (this milestone)
- P2: Should have, add after v1 core is validated
- P3: Nice to have, future consideration

---

## Competitor Feature Analysis

| Feature | TutorCruncher | Teachworks | DreamClass | Edvantix Approach |
|---------|---------------|------------|------------|-------------------|
| Custom roles | System roles only | System roles only | Fixed roles | Owner-defined roles with permission assignment — differentiator |
| Conflict detection | YES (teacher) | YES (basic) | YES (teacher + room) | Teacher conflict; room not applicable (online) |
| Group scheduling | YES | YES | YES | YES — group is the primary scheduling unit |
| Attendance tracking | YES | YES | YES | YES — with domain event for payment deduction |
| Package billing | YES (credit system) | YES (packages) | YES | Manual-only in v1; no gateway |
| Student balance view | YES | YES | YES | YES — purchased / used / remaining |
| Recurring slots | YES | YES | YES | v1.x — after core validated |
| Self-booking by student | YES | Optional | NO | Deliberately excluded; manager-controlled model |
| Payment gateway | YES (multiple) | YES | YES | Out of scope v1 |
| Notification system | YES (email/SMS) | YES (email) | YES | Out of scope v1 |

---

## Sources

- [Role-Based Access in School Management Systems — Edcrib](https://web.edcrib.com/updates/the-power-of-role-based-access-in-school-management-systems/)
- [Multi-tenant RBAC design for SaaS — WorkOS](https://workos.com/blog/how-to-design-multi-tenant-rbac-saas)
- [RBAC Design in SaaS Applications — Agnite Studio](https://agnitestudio.com/blog/rbac-design-saas/)
- [Best Practices for Multi-Tenant Authorization — Permit.io](https://www.permit.io/blog/best-practices-for-multi-tenant-authorization)
- [TutorCruncher scheduling features](https://tutorcruncher.com)
- [Teachworks calendar and conflict checker](https://teachworks.com/features/calendar)
- [DreamClass scheduling and billing](https://www.dreamclass.io/feature/scheduling-timetables/)
- [Teach 'n Go scheduling features](https://www.teachngo.com/features-new/powerful-teacher-scheduling)
- [Leading Platforms for School Finances and Billing — DreamClass](https://www.dreamclass.io/2025/leading-platforms-for-managing-school-finances-and-student-billing/)
- [TutorCruncher vs TutorBird comparison 2025](https://tutorcruncher.com/blog/tutorcruncher-vs-tutorbird)
- [Best Tutor Scheduling Software 2025 — withnoto](https://www.withnono.com/blog/best-tutor-scheduling-software)

---
*Feature research for: Online school management SaaS — Organizations, Scheduling, Payments*
*Researched: 2026-03-18*
