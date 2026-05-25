// Group Enrollment — Step 3 of group setup. Pick students from the pool,
// invite by email, set recruitment policy, prepare welcome notification.

const { useState: useStateEN, useMemo: useMemoEN } = React;

// The group we're enrolling into — taken from window.GROUPS in real flow
const ENROLL_GROUP = {
  code: 'EN-B1-12',
  name: 'English Intermediate · вечерняя',
  level: 'B1',
  course: 'General English',
  teacher: 'Петров А. Н.',
  format: 'offline',
  room: 'Каб. 204',
  capacity: 12,
  schedule: 'Пн / Ср · 18:00–19:30',
  starts: '04.05.2026',
  ends: '21.12.2026',
};

// ── Helpers ──────────────────────────────────────────────────────────
function declensionEN(n, forms) {
  const abs = Math.abs(n);
  const m10 = abs % 10, m100 = abs % 100;
  if (m10 === 1 && m100 !== 11) return forms[0];
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return forms[1];
  return forms[2];
}
function isValidEmailEN(s) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(s.trim());
}

// ── Initial config ───────────────────────────────────────────────────
const INITIAL_ENROLL = {
  enrolledIds: [],          // student ids picked into the roster
  waitlistIds: [],          // overflow goes here
  invites: [],              // {email, name?} invitations queued
  openRecruitment: true,    // visible in school's public catalog
  recruitDeadline: '2026-05-25',
  requirePayment: true,
  requireTest: false,
  notifyOnSave: true,
  sendWelcome: true,
  welcomeSubject: 'Вы зачислены в группу English Intermediate',
  welcomeBody:
    'Здравствуйте, {{firstName}}!\n\n' +
    'Вас зачислили в группу {{groupName}}. Первое занятие — {{startsAt}}, кабинет {{room}}.\n' +
    'Преподаватель: {{teacher}}. Расписание занятий уже в вашем личном кабинете.\n\n' +
    'До встречи!\nКоманда Edvantix',
};

// ─────────────────────────────────────────────────────────────────────
function GroupEnrollmentApp() {
  const [cfg, setCfg] = useStateEN(INITIAL_ENROLL);
  const update = (patch) => setCfg(c => ({ ...c, ...patch }));

  const enrolled = useMemoEN(
    () => cfg.enrolledIds.map(id => STUDENT_POOL.find(s => s.id === id)).filter(Boolean),
    [cfg.enrolledIds]
  );
  const waitlisted = useMemoEN(
    () => cfg.waitlistIds.map(id => STUDENT_POOL.find(s => s.id === id)).filter(Boolean),
    [cfg.waitlistIds]
  );

  const seatsTaken = enrolled.length;
  const seatsLeft = Math.max(0, ENROLL_GROUP.capacity - seatsTaken);
  const overCap = seatsTaken > ENROLL_GROUP.capacity;

  const isInRoster = (id) => cfg.enrolledIds.includes(id) || cfg.waitlistIds.includes(id);
  const toggleStudent = (id) => {
    if (cfg.enrolledIds.includes(id)) {
      update({ enrolledIds: cfg.enrolledIds.filter(x => x !== id) });
    } else if (cfg.waitlistIds.includes(id)) {
      update({ waitlistIds: cfg.waitlistIds.filter(x => x !== id) });
    } else if (seatsLeft > 0) {
      update({ enrolledIds: [...cfg.enrolledIds, id] });
    } else {
      update({ waitlistIds: [...cfg.waitlistIds, id] });
    }
  };
  const removeFromRoster = (id) => {
    if (cfg.enrolledIds.includes(id)) {
      // If there's someone on waitlist — promote them
      if (cfg.waitlistIds.length > 0) {
        const [promoted, ...rest] = cfg.waitlistIds;
        update({
          enrolledIds: [...cfg.enrolledIds.filter(x => x !== id), promoted],
          waitlistIds: rest,
        });
      } else {
        update({ enrolledIds: cfg.enrolledIds.filter(x => x !== id) });
      }
    } else {
      update({ waitlistIds: cfg.waitlistIds.filter(x => x !== id) });
    }
  };
  const moveToWaitlist = (id) => {
    update({
      enrolledIds: cfg.enrolledIds.filter(x => x !== id),
      waitlistIds: [...cfg.waitlistIds, id],
    });
  };
  const promoteFromWaitlist = (id) => {
    if (seatsLeft <= 0) return;
    update({
      waitlistIds: cfg.waitlistIds.filter(x => x !== id),
      enrolledIds: [...cfg.enrolledIds, id],
    });
  };

  // ── Suggestion: auto-pick students matching the group ──
  const autoPick = () => {
    // Prefer: waitlist of this level, then "free" tested students, then any free
    const candidates = STUDENT_POOL
      .filter(s => s.level === ENROLL_GROUP.level && !isInRoster(s.id))
      .filter(s => s.status === 'free' || s.status === 'waitlist' || s.status === 'new')
      .sort((a, b) => {
        const score = (s) => (s.status === 'waitlist' ? 0 : s.status === 'free' ? 1 : 2)
          + (s.tags.includes('paid') ? -0.5 : 0)
          + (s.tags.includes('tested') ? -0.3 : 0);
        return score(a) - score(b);
      });
    const room = seatsLeft;
    const fill = candidates.slice(0, room).map(s => s.id);
    update({ enrolledIds: [...cfg.enrolledIds, ...fill] });
  };

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="groups" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0, position: 'relative' }}>
        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <a href="Groups.html" style={{ color: '#64748b' }}>Школа</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Groups.html" style={{ color: '#64748b' }}>Группы</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Group Create.html" style={{ color: '#64748b' }}>{ENROLL_GROUP.name}</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Студенты</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '20px 32px 18px', borderBottom: '1px solid #e2e8f0',
          background: '#fff',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 20, marginBottom: 14 }}>
            <a href="Group Schedule Setup.html" style={{
              width: 36, height: 36, borderRadius: 10, border: '1px solid #e2e8f0',
              background: '#fff', display: 'inline-flex', alignItems: 'center',
              justifyContent: 'center', color: '#64748b', flexShrink: 0,
            }}><Icon.ArrowLeft size={16} /></a>
            <div style={{ flex: 1, minWidth: 0 }}>
              <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
                Зачисление студентов
              </h1>
              <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
                Шаг 3 из 3 — соберите состав группы. Можно зачислить вручную из базы, пригласить новых или открыть набор.
              </div>
            </div>
            <ProgressIndicatorEN current={3} steps={[
              { id: 1, label: 'Основное' },
              { id: 2, label: 'Расписание' },
              { id: 3, label: 'Студенты' },
            ]} />
          </div>

          {/* Group context strip */}
          <div style={{
            display: 'flex', alignItems: 'center', gap: 14, padding: '10px 14px',
            background: '#f8fafc', border: '1px solid #e2e8f0', borderRadius: 12,
          }}>
            <div style={{
              width: 36, height: 36, borderRadius: 10, flexShrink: 0,
              background: 'rgba(14,165,233,0.12)', color: '#0369a1',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontSize: 12, fontWeight: 700, fontFamily: 'var(--edv-font-mono)',
            }}>{ENROLL_GROUP.level}</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <span style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>
                  {ENROLL_GROUP.name}
                </span>
                <span style={{
                  fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b',
                  padding: '2px 7px', borderRadius: 6, background: '#fff', border: '1px solid #e2e8f0',
                }}>{ENROLL_GROUP.code}</span>
              </div>
              <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2,
                display: 'flex', alignItems: 'center', gap: 12, flexWrap: 'wrap' }}>
                <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5 }}>
                  <Icon.Calendar size={12} stroke="#94a3b8" />{ENROLL_GROUP.schedule}
                </span>
                <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5 }}>
                  <Icon.School size={12} stroke="#94a3b8" />{ENROLL_GROUP.room}
                </span>
                <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                <span>{ENROLL_GROUP.teacher}</span>
                <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                <span>{ENROLL_GROUP.starts} → {ENROLL_GROUP.ends}</span>
              </div>
            </div>
            <a href="Group Schedule Setup.html" style={{
              fontSize: 12.5, color: '#4338ca', fontWeight: 500,
              padding: '6px 10px', borderRadius: 8,
              border: '1px solid rgba(79,70,229,0.2)',
              background: 'rgba(79,70,229,0.04)',
            }}>Изменить расписание</a>
          </div>
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 120px' }}>
          <div style={{ maxWidth: 1240, margin: '0 auto', display: 'grid',
            gridTemplateColumns: 'minmax(0, 1fr) 360px', gap: 24, alignItems: 'start' }}>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 20, minWidth: 0 }}>

              {/* 3.1 Capacity overview */}
              <CapacityOverview
                capacity={ENROLL_GROUP.capacity}
                enrolled={enrolled.length}
                waitlist={waitlisted.length}
                invites={cfg.invites.length}
                openRecruitment={cfg.openRecruitment}
                autoPick={autoPick}
              />

              {/* 3.2 Student pool selector */}
              <ENSection icon="UserPlus" title="Подбор из базы школы"
                subtitle="Найдите подходящих студентов и зачислите в один клик"
                step="3.1">
                <StudentPool
                  groupLevel={ENROLL_GROUP.level}
                  groupCategory="adult"
                  isInRoster={isInRoster}
                  inEnrolled={(id) => cfg.enrolledIds.includes(id)}
                  inWaitlist={(id) => cfg.waitlistIds.includes(id)}
                  onToggle={toggleStudent}
                  seatsLeft={seatsLeft}
                />
              </ENSection>

              {/* 3.3 Invite by email */}
              <ENSection icon="Mail" title="Пригласить новых студентов"
                subtitle="Если студентов ещё нет в базе — отправьте им приглашения"
                step="3.2">
                <InvitePicker
                  invites={cfg.invites}
                  onAdd={(inv) => update({ invites: [...cfg.invites, inv] })}
                  onRemove={(idx) => update({ invites: cfg.invites.filter((_, i) => i !== idx) })}
                />
              </ENSection>

              {/* 3.4 Open recruitment */}
              <ENSection icon="Megaphone" title="Открытый набор"
                subtitle="Сделайте группу видимой в публичном каталоге школы"
                step="3.3">
                <RecruitmentPanel
                  cfg={cfg} update={update}
                  seatsLeft={seatsLeft}
                  group={ENROLL_GROUP}
                />
              </ENSection>

              {/* 3.5 Welcome message */}
              <ENSection icon="Sparkles" title="Приветствие и уведомления"
                subtitle="Что произойдёт после нажатия «Завершить»"
                step="3.4">
                <WelcomePanel cfg={cfg} update={update} enrolled={enrolled.length} invites={cfg.invites.length} />
              </ENSection>
            </div>

            {/* Sticky preview */}
            <div style={{ position: 'sticky', top: 0, alignSelf: 'start',
              display: 'flex', flexDirection: 'column', gap: 14 }}>
              <RosterCard
                capacity={ENROLL_GROUP.capacity}
                enrolled={enrolled}
                waitlisted={waitlisted}
                invites={cfg.invites}
                onRemove={removeFromRoster}
                onPromote={promoteFromWaitlist}
                onWaitlist={moveToWaitlist}
                seatsLeft={seatsLeft}
              />
              <ReadyHint enrolled={enrolled.length} invites={cfg.invites.length}
                openRecruitment={cfg.openRecruitment} />
            </div>
          </div>
        </div>

        {/* Sticky finish bar */}
        <FinishBar
          enrolled={enrolled.length}
          waitlist={waitlisted.length}
          invites={cfg.invites.length}
          capacity={ENROLL_GROUP.capacity}
          openRecruitment={cfg.openRecruitment}
        />
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Section card (matches steps 1 & 2)
// ═══════════════════════════════════════════════════════════════════
function ENSection({ icon, title, subtitle, children, step }) {
  const IC = Icon[icon];
  return (
    <section style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
      overflow: 'hidden',
    }}>
      <header style={{
        padding: '16px 24px', borderBottom: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', gap: 14,
      }}>
        <div style={{
          width: 36, height: 36, borderRadius: 10, flexShrink: 0,
          background: 'rgba(79,70,229,0.08)', color: '#4f46e5',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <IC size={18} stroke="#4f46e5" />
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <h2 style={{ margin: 0, fontSize: 15, fontWeight: 600, color: '#0f172a',
            letterSpacing: '-0.01em' }}>{title}</h2>
          {subtitle && <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>{subtitle}</div>}
        </div>
        {step && (
          <span style={{
            fontFamily: 'var(--edv-font-mono)', fontSize: 11, color: '#94a3b8',
            padding: '3px 8px', borderRadius: 9999, background: '#f1f5f9',
          }}>Шаг {step}</span>
        )}
      </header>
      <div style={{ padding: '22px 24px' }}>{children}</div>
    </section>
  );
}

window.GroupEnrollmentApp = GroupEnrollmentApp;
