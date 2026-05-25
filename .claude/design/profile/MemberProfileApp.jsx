// Organization member profile — work-related view (not account credentials).
const { useState: useStateP, useMemo: useMemoP } = React;

const TABS = [
  { id: 'overview',   label: 'Обзор',          icon: 'LayoutDashboard' },
  { id: 'groups',     label: 'Группы и курсы', icon: 'Users',           count: PROFILE.groups.length },
  { id: 'schedule',   label: 'Расписание',     icon: 'CalendarDays' },
  { id: 'documents',  label: 'Документы',      icon: 'FileText',        count: PROFILE.documents.length },
  { id: 'history',    label: 'История',        icon: 'BarChart2' },
];

function MemberProfileApp() {
  const [tab, setTab] = useStateP('overview');
  const p = PROFILE;
  const status = MEMBER_STATUSES[p.status];
  const primaryRoleData = MEMBER_ROLES.find(r => r.value === p.primaryRole);

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="profiles" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Пользователи</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Organization Members.html" style={{ color: '#4f46e5' }}>Участники</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>{p.name}</span>
          <div style={{ marginLeft: 'auto', fontSize: 12, color: '#94a3b8',
            fontFamily: 'var(--edv-font-mono)' }}>
            {p.staffNumber}
          </div>
        </div>

        {/* Profile header */}
        <div style={{
          padding: '24px 32px 0', borderBottom: '1px solid #e2e8f0', background: '#fff',
        }}>
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 20, paddingBottom: 18 }}>
            <div style={{ position: 'relative', flexShrink: 0 }}>
              <Avatar name={p.name} size={88} style={{ fontSize: 30, boxShadow: '0 4px 14px rgba(15,23,42,0.10)' }} />
              <span style={{
                position: 'absolute', bottom: 4, right: 4,
                width: 16, height: 16, borderRadius: 9999,
                background: status.dot, border: '3px solid #fff',
              }} title={status.label} />
            </div>

            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap' }}>
                <h1 style={{
                  margin: 0, fontSize: 26, fontWeight: 700, letterSpacing: '-0.02em',
                  color: '#0f172a',
                }}>{p.name}</h1>
                <StatusPill status={p.status} />
              </div>
              <div style={{
                display: 'flex', alignItems: 'center', gap: 10, marginTop: 8,
                fontSize: 14, color: '#334155',
              }}>
                <span style={{ fontWeight: 500 }}>{p.position}</span>
                <span style={{ color: '#cbd5e1' }}>·</span>
                <span style={{ color: '#64748b' }}>{p.department}</span>
                <span style={{ color: '#cbd5e1' }}>·</span>
                <span style={{ color: '#64748b' }}>{p.branch}</span>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 12, flexWrap: 'wrap' }}>
                {p.roles.map(r => <RoleTag key={r} role={r} />)}
                <span style={{
                  display: 'inline-flex', alignItems: 'center', gap: 6,
                  padding: '3px 10px', borderRadius: 6,
                  background: '#f1f5f9', color: '#475569',
                  fontSize: 12, fontWeight: 500,
                }}>
                  <Icon.Briefcase size={11} stroke="#475569" />{p.employmentType}
                </span>
                <span style={{
                  display: 'inline-flex', alignItems: 'center', gap: 6,
                  padding: '3px 10px', borderRadius: 6,
                  background: '#f1f5f9', color: '#475569',
                  fontSize: 12, fontWeight: 500,
                }}>
                  <Icon.Calendar size={11} stroke="#475569" />в организации с {p.joined}
                </span>
              </div>
            </div>

            <div style={{ display: 'flex', gap: 8 }}>
              <Button variant="secondary" size="sm">
                <Icon.MessageCircle size={14} />Написать
              </Button>
              <Button variant="secondary" size="sm">
                <Icon.Mail size={14} />Email
              </Button>
              <Button size="sm">
                <Icon.Settings size={14} />Редактировать
              </Button>
              <button style={{
                width: 32, height: 32, borderRadius: 8, border: '1px solid #e2e8f0',
                background: '#fff', display: 'flex', alignItems: 'center', justifyContent: 'center',
                cursor: 'pointer', color: '#64748b',
              }}>
                <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                  <circle cx="12" cy="5" r="1.5"/><circle cx="12" cy="12" r="1.5"/><circle cx="12" cy="19" r="1.5"/>
                </svg>
              </button>
            </div>
          </div>

          {/* Tabs */}
          <div style={{ display: 'flex', gap: 2, marginTop: 4 }}>
            {TABS.map(t => {
              const IC = Icon[t.icon];
              const active = tab === t.id;
              return (
                <button
                  key={t.id}
                  onClick={() => setTab(t.id)}
                  style={{
                    display: 'inline-flex', alignItems: 'center', gap: 8,
                    padding: '10px 16px',
                    border: 'none', background: 'transparent',
                    fontSize: 13.5, fontWeight: active ? 600 : 500,
                    fontFamily: 'inherit', cursor: 'pointer',
                    color: active ? '#4f46e5' : '#64748b',
                    borderBottom: `2px solid ${active ? '#4f46e5' : 'transparent'}`,
                    marginBottom: -1,
                  }}
                  onMouseEnter={e => { if (!active) e.currentTarget.style.color = '#0f172a'; }}
                  onMouseLeave={e => { if (!active) e.currentTarget.style.color = '#64748b'; }}
                >
                  <IC size={14} />{t.label}
                  {t.count != null && (
                    <span style={{
                      display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                      minWidth: 18, height: 18, padding: '0 6px', borderRadius: 9999,
                      background: active ? 'rgba(79,70,229,0.10)' : '#f1f5f9',
                      color: active ? '#4338ca' : '#64748b',
                      fontSize: 11, fontWeight: 600, fontVariantNumeric: 'tabular-nums',
                    }}>{t.count}</span>
                  )}
                </button>
              );
            })}
          </div>
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 40px' }}>
          <div style={{ maxWidth: 1280, margin: '0 auto' }}>
            {tab === 'overview'   && <OverviewTab p={p} />}
            {tab === 'groups'     && <GroupsTab p={p} />}
            {tab === 'schedule'   && <ScheduleTab p={p} />}
            {tab === 'documents'  && <DocumentsTab p={p} />}
            {tab === 'history'    && <HistoryTab p={p} />}
          </div>
        </div>
      </div>
    </div>
  );
}

// ── Overview tab ─────────────────────────────────────────────────────
function OverviewTab({ p }) {
  return (
    <div style={{
      display: 'grid',
      gridTemplateColumns: 'minmax(0,340px) minmax(0,1fr)',
      gap: 20, alignItems: 'flex-start',
    }}>
      {/* Left column */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16, position: 'sticky', top: 0 }}>
        <SectionCard title="Сведения о работе">
          <div style={{ display: 'flex', flexDirection: 'column', gap: 0 }}>
            <InfoRow label="Должность">{p.position}</InfoRow>
            <InfoRow label="Кафедра">{p.department}</InfoRow>
            <InfoRow label="Корпус">{p.branch}</InfoRow>
            <InfoRow label="Тип занятости">
              <span>{p.employmentType}</span>
              <span style={{ color: '#94a3b8' }}> · ставка {p.rate.toFixed(2).replace('.', ',')}</span>
            </InfoRow>
            <InfoRow label="Договор">
              {p.contract}
              <div style={{ fontSize: 11.5, color: '#94a3b8', marginTop: 2,
                fontFamily: 'var(--edv-font-mono)' }}>№ {p.contractNumber}</div>
            </InfoRow>
            <InfoRow label="Принят(а)">{p.joined}
              <span style={{ color: '#94a3b8' }}> · {Math.floor(p.joinedDays/365)} г. {Math.floor((p.joinedDays%365)/30)} мес.</span>
            </InfoRow>
            <InfoRow label="Таб. номер" mono>{p.staffNumber}</InfoRow>
            <div style={{ padding: '10px 0 0', display: 'flex', alignItems: 'center', gap: 12 }}>
              <div style={{ fontSize: 12.5, color: '#64748b', fontWeight: 500, width: 120, flexShrink: 0 }}>
                Руководитель
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10, flex: 1, minWidth: 0 }}>
                <Avatar name={p.manager.name} size={28} style={{ fontSize: 11 }} />
                <div style={{ minWidth: 0 }}>
                  <div style={{ fontSize: 13, color: '#0f172a', fontWeight: 500,
                    whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    {p.manager.name}
                  </div>
                  <div style={{ fontSize: 11.5, color: '#64748b' }}>{p.manager.position}</div>
                </div>
              </div>
            </div>
          </div>
        </SectionCard>

        <SectionCard title="Рабочие контакты">
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <ContactRow icon="Mail" label="Рабочий email" value={p.workEmail} copyable />
            <ContactRow icon="Phone" label="Рабочий телефон" value={p.workPhone} />
            <ContactRow icon="Building2" label="Кабинет" value={p.cabinet + ' · доб. ' + p.internalCode} />
            <ContactRow icon="Send" label="Telegram" value={p.telegram} />
          </div>
        </SectionCard>

        <SectionCard title="Доступ" padding="14px 22px">
          <div style={{
            display: 'flex', alignItems: 'center', justifyContent: 'space-between',
            padding: '6px 0',
          }}>
            <div>
              <div style={{ fontSize: 12.5, color: '#64748b' }}>Последняя активность</div>
              <div style={{ fontSize: 13.5, color: '#0f172a', fontWeight: 500, marginTop: 2 }}>
                {p.lastActive}
              </div>
            </div>
            <a href="#" style={{
              fontSize: 12.5, color: '#4f46e5', fontWeight: 500,
            }}>Открыть учётную запись →</a>
          </div>
        </SectionCard>
      </div>

      {/* Right column */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        {/* Workload stats strip */}
        <div style={{
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
          display: 'flex', overflow: 'hidden',
        }}>
          <Stat icon="Users" tone="primary"
            value={p.workload.activeGroups + p.workload.curated}
            label="Групп и курсов"
            sub={`${p.workload.activeGroups} преп. + ${p.workload.curated} кур.`} />
          <Stat icon="GraduationCap" tone="teal"
            value={p.workload.studentsTotal}
            label="Студентов"
            sub="по всем группам" />
          <Stat icon="CalendarDays" tone="slate"
            value={p.workload.weeklyHours}
            suffix={`/ ${p.workload.contractHours} ч`}
            label="Часов в неделю"
            sub={`${Math.round(p.workload.weeklyHours / p.workload.contractHours * 100)}% нагрузки`} />
          <Stat icon="CircleCheck" tone="success"
            value={Math.round(p.workload.avgAttendance * 100)}
            suffix="%"
            label="Посещаемость"
            sub="ср. за квартал" />
          <div style={{ flex: 1, minWidth: 0, padding: '14px 16px', display: 'flex', alignItems: 'center', gap: 12 }}>
            <div style={{
              width: 36, height: 36, borderRadius: 10, flexShrink: 0,
              background: 'rgba(245,158,11,0.14)', color: '#92400e',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}><Icon.Sparkles size={17} stroke="#92400e" /></div>
            <div style={{ minWidth: 0, flex: 1 }}>
              <div style={{
                fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em',
                color: '#0f172a', fontVariantNumeric: 'tabular-nums', lineHeight: 1,
              }}>{p.workload.avgGrade.toString().replace('.', ',')}</div>
              <div style={{ fontSize: 12, color: '#64748b', fontWeight: 500, marginTop: 4 }}>
                Средний балл
              </div>
              <div style={{ fontSize: 11, color: '#94a3b8', marginTop: 2 }}>по группам</div>
            </div>
          </div>
        </div>

        {/* Subjects */}
        <SectionCard title="Предметы и направления" padding="14px 22px">
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
            {p.subjects.map(s => (
              <span key={s} style={{
                display: 'inline-flex', alignItems: 'center', gap: 6,
                padding: '6px 12px', borderRadius: 9999,
                background: 'rgba(79,70,229,0.08)', color: '#4338ca',
                fontSize: 12.5, fontWeight: 500,
              }}>
                <Icon.BookOpen size={12} stroke="#4338ca" />{s}
              </span>
            ))}
          </div>
        </SectionCard>

        {/* Groups */}
        <SectionCard
          title="Группы и кураторство"
          action={<a href="#" style={{ fontSize: 12.5, color: '#4f46e5', fontWeight: 500 }}>Все группы →</a>}
          padding="0"
        >
          <div>
            {p.groups.map(g => <GroupRow key={g.id} g={g} />)}
          </div>
        </SectionCard>

        {/* Schedule + Notes */}
        <div style={{ display: 'grid', gridTemplateColumns: '1.4fr 1fr', gap: 16 }}>
          <SectionCard
            title="Ближайшие занятия"
            action={<a href="#" style={{ fontSize: 12.5, color: '#4f46e5', fontWeight: 500 }}>Расписание →</a>}
            padding="0"
          >
            {p.schedule.map((s, i) => <ScheduleRow key={i} item={s} />)}
          </SectionCard>

          <SectionCard
            title="Внутренние заметки"
            action={
              <button style={{
                background: 'transparent', border: 'none', color: '#4f46e5',
                fontSize: 12.5, fontWeight: 500, cursor: 'pointer',
                fontFamily: 'inherit', display: 'inline-flex', alignItems: 'center', gap: 4,
              }}>
                <Icon.Plus size={12} />Добавить
              </button>
            }
          >
            <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
              {p.notes.map((n, i) => <NoteCard key={i} n={n} />)}
              <div style={{
                fontSize: 11, color: '#94a3b8', marginTop: 2, display: 'flex', alignItems: 'center', gap: 6,
              }}>
                <Icon.Shield size={11} stroke="#94a3b8" />
                Видны только администраторам и методистам
              </div>
            </div>
          </SectionCard>
        </div>

        {/* Qualifications + Activity */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
          <SectionCard title="Образование и квалификация">
            <div>
              {p.qualifications.map((q, i) => <QualificationItem key={i} q={q} />)}
            </div>
          </SectionCard>

          <SectionCard title="Активность">
            <div>
              {p.activity.map((a, i) => (
                <ActivityItem key={i} item={a} last={i === p.activity.length - 1} />
              ))}
            </div>
          </SectionCard>
        </div>
      </div>
    </div>
  );
}

// ── Contact row helper ───────────────────────────────────────────────
function ContactRow({ icon, label, value, copyable }) {
  const IC = Icon[icon];
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: '#f1f5f9', color: '#64748b',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}><IC size={14} stroke="#64748b" /></div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 11.5, color: '#94a3b8', fontWeight: 500 }}>{label}</div>
        <div style={{ fontSize: 13, color: '#0f172a', marginTop: 1,
          whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{value}</div>
      </div>
      {copyable && (
        <button style={{
          width: 28, height: 28, borderRadius: 6, border: 'none',
          background: 'transparent', color: '#94a3b8', cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }} title="Скопировать">
          <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <rect x="9" y="9" width="13" height="13" rx="2"/><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"/>
          </svg>
        </button>
      )}
    </div>
  );
}

// ── Other tabs (lighter) ─────────────────────────────────────────────
function GroupsTab({ p }) {
  return (
    <SectionCard title={`Группы и курсы — ${p.groups.length}`} padding="0">
      <div>{p.groups.map(g => <GroupRow key={g.id} g={g} />)}</div>
    </SectionCard>
  );
}
function ScheduleTab({ p }) {
  return (
    <SectionCard title="Ближайшие занятия — 2 недели" padding="0">
      <div>{p.schedule.map((s, i) => <ScheduleRow key={i} item={s} />)}</div>
    </SectionCard>
  );
}
function DocumentsTab({ p }) {
  return (
    <SectionCard title="Документы">
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px,1fr))', gap: 10 }}>
        {p.documents.map((d, i) => <DocumentChip key={i} doc={d} />)}
      </div>
    </SectionCard>
  );
}
function HistoryTab({ p }) {
  return (
    <SectionCard title="История событий">
      <div>{p.activity.map((a, i) => (
        <ActivityItem key={i} item={a} last={i === p.activity.length - 1} />
      ))}</div>
    </SectionCard>
  );
}

window.MemberProfileApp = MemberProfileApp;
