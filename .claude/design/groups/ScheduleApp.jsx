// Group Schedule — Step 2 of group setup. Configure recurrence, time slots, exceptions.
const { useState: useStateGS, useMemo: useMemoGS } = React;

// The group we're configuring — taken from window.GROUPS in real flow
const CONTEXT_GROUP = {
  code: 'EN-B1-12',
  name: 'English Intermediate · вечерняя',
  level: 'B1',
  course: 'General English',
  teacher: 'Петров А. Н.',
  format: 'offline',
  room: 'Каб. 204',
  capacity: 12,
};

const WEEKDAYS = [
  { id: 1, short: 'Пн', long: 'Понедельник' },
  { id: 2, short: 'Вт', long: 'Вторник'    },
  { id: 3, short: 'Ср', long: 'Среда'      },
  { id: 4, short: 'Чт', long: 'Четверг'    },
  { id: 5, short: 'Пт', long: 'Пятница'    },
  { id: 6, short: 'Сб', long: 'Суббота'    },
  { id: 0, short: 'Вс', long: 'Воскресенье'},
];

const DURATION_PRESETS = [
  { value: 45, label: '45 мин', sub: 'академ.' },
  { value: 60, label: '60 мин', sub: '1 час' },
  { value: 90, label: '90 мин', sub: '2 пары' },
  { value: 120,label: '120 мин',sub: '4 пары' },
];

const RECURRENCE = [
  { value: 'weekly',   label: 'Еженедельно',     hint: 'Одни и те же дни каждую неделю' },
  { value: 'biweekly', label: 'Через неделю',    hint: 'Чередование чётной / нечётной недели' },
  { value: 'custom',   label: 'Произвольно',     hint: 'Назначу даты вручную' },
];

const HOLIDAYS_2026 = [
  { date: '2026-05-01', name: 'Праздник Весны и Труда' },
  { date: '2026-05-09', name: 'День Победы' },
  { date: '2026-11-04', name: 'День народного единства' },
  { date: '2026-01-01', name: 'Новогодние каникулы' },
  { date: '2026-01-07', name: 'Рождество Христово' },
];

// ── Helpers ──────────────────────────────────────────────────────────
function fmtTime(mins) {
  const h = Math.floor(mins / 60), m = mins % 60;
  return `${String(h).padStart(2,'0')}:${String(m).padStart(2,'0')}`;
}
function parseTime(s) {
  if (!s) return 0;
  const [h, m] = s.split(':').map(n => parseInt(n, 10) || 0);
  return h * 60 + m;
}
function fmtDateRu(d) {
  if (!d) return '—';
  const [y, m, day] = d.split('-');
  return `${day}.${m}.${y}`;
}
function fmtDateShort(date) {
  const d = String(date.getDate()).padStart(2,'0');
  const m = String(date.getMonth()+1).padStart(2,'0');
  return `${d}.${m}`;
}
function dateKey(date) {
  const y = date.getFullYear();
  const m = String(date.getMonth()+1).padStart(2,'0');
  const d = String(date.getDate()).padStart(2,'0');
  return `${y}-${m}-${d}`;
}
function declensionGS(n, forms) {
  const abs = Math.abs(n);
  const m10 = abs % 10, m100 = abs % 100;
  if (m10 === 1 && m100 !== 11) return forms[0];
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return forms[1];
  return forms[2];
}

// Generate lesson dates given config
function generateLessons(cfg) {
  const out = [];
  if (!cfg.starts || !cfg.ends || cfg.slots.length === 0) return out;
  const start = new Date(cfg.starts + 'T00:00:00');
  const end = new Date(cfg.ends + 'T00:00:00');
  if (end < start) return out;

  const slotsByDay = {};
  cfg.slots.forEach(s => {
    if (!slotsByDay[s.weekday]) slotsByDay[s.weekday] = [];
    slotsByDay[s.weekday].push(s);
  });

  const skipSet = new Set(cfg.skipHolidays ? HOLIDAYS_2026.map(h => h.date) : []);
  cfg.exceptions.forEach(d => skipSet.add(d));

  // Iterate week by week
  const cursor = new Date(start);
  let weekIndex = 0;
  let safety = 0;
  while (cursor <= end && safety < 400) {
    safety++;
    // Sunday=0 ... Saturday=6 (we use 1..6,0 — match WEEKDAYS)
    const weekday = cursor.getDay();
    const slots = slotsByDay[weekday];

    if (slots) {
      const include = cfg.recurrence === 'biweekly'
        ? (Math.floor(weekIndex / 1) % 2 === (cfg.biweeklyParity || 0))
        : true;
      if (include) {
        slots.forEach(s => {
          const k = dateKey(cursor);
          if (skipSet.has(k)) {
            out.push({ date: k, slot: s, skipped: true,
              reason: HOLIDAYS_2026.find(h => h.date === k)?.name || 'Исключение' });
          } else {
            out.push({ date: k, slot: s, skipped: false });
          }
        });
      }
    }
    cursor.setDate(cursor.getDate() + 1);
    if (cursor.getDay() === 1) weekIndex++;
  }
  return out;
}

// ── Initial state ─────────────────────────────────────────────────────
const INITIAL_CFG = {
  recurrence: 'weekly',
  biweeklyParity: 0,
  duration: 90,
  slots: [
    { id: 1, weekday: 1, start: '18:00' }, // Пн 18:00
    { id: 2, weekday: 3, start: '18:00' }, // Ср 18:00
  ],
  starts: '2026-05-04',
  ends:   '2026-12-21',
  endMode: 'date',     // 'date' | 'count'
  lessonCount: 36,
  skipHolidays: true,
  exceptions: [],
  notifyStudents: true,
};

// ─────────────────────────────────────────────────────────────────────
function GroupScheduleApp() {
  const [cfg, setCfg] = useStateGS(INITIAL_CFG);
  const update = (patch) => setCfg(c => ({ ...c, ...patch }));

  const lessons = useMemoGS(() => generateLessons(cfg), [cfg]);
  const heldCount = lessons.filter(l => !l.skipped).length;
  const skippedCount = lessons.filter(l => l.skipped).length;
  const totalHours = (heldCount * cfg.duration / 60).toFixed(1);

  // When endMode = 'count', adjust ends date to match
  // (kept simple — just informational)

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
          <a href="Group Create.html" style={{ color: '#64748b' }}>{CONTEXT_GROUP.name}</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Расписание</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '20px 32px 18px', borderBottom: '1px solid #e2e8f0',
          background: '#fff',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 20, marginBottom: 14 }}>
            <a href="Group Create.html" style={{
              width: 36, height: 36, borderRadius: 10, border: '1px solid #e2e8f0',
              background: '#fff', display: 'inline-flex', alignItems: 'center',
              justifyContent: 'center', color: '#64748b', flexShrink: 0,
            }}><Icon.ArrowLeft size={16} /></a>
            <div style={{ flex: 1, minWidth: 0 }}>
              <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
                Расписание группы
              </h1>
              <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
                Шаг 2 из 3 — задайте дни, время и период проведения занятий
              </div>
            </div>
            <ProgressIndicatorGS current={2} steps={[
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
            }}>{CONTEXT_GROUP.level}</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <span style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>
                  {CONTEXT_GROUP.name}
                </span>
                <span style={{
                  fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b',
                  padding: '2px 7px', borderRadius: 6, background: '#fff', border: '1px solid #e2e8f0',
                }}>{CONTEXT_GROUP.code}</span>
              </div>
              <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2,
                display: 'flex', alignItems: 'center', gap: 12 }}>
                <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5 }}>
                  <Icon.School size={12} stroke="#94a3b8" />{CONTEXT_GROUP.room}
                </span>
                <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                <span>{CONTEXT_GROUP.teacher}</span>
                <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                <span>До {CONTEXT_GROUP.capacity} студентов</span>
              </div>
            </div>
            <a href="Group Create.html" style={{
              fontSize: 12.5, color: '#4338ca', fontWeight: 500,
              padding: '6px 10px', borderRadius: 8,
              border: '1px solid rgba(79,70,229,0.2)',
              background: 'rgba(79,70,229,0.04)',
            }}>Изменить параметры</a>
          </div>
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 120px' }}>
          <div style={{ maxWidth: 1180, margin: '0 auto', display: 'grid',
            gridTemplateColumns: 'minmax(0, 1fr) 360px', gap: 24, alignItems: 'start' }}>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 20, minWidth: 0 }}>

              {/* 2.1 Recurrence */}
              <GSSection icon="Calendar" title="Периодичность" subtitle="Как часто повторяются занятия"
                step="2.1">
                <F.CardRadio
                  value={cfg.recurrence}
                  onChange={v => update({ recurrence: v })}
                  options={RECURRENCE.map(r => ({ value: r.value, tag: shortRecurrence(r.value), label: r.hint }))}
                  columns={3}
                />
                {cfg.recurrence === 'biweekly' && (
                  <div style={{ marginTop: 14, padding: 12, borderRadius: 10,
                    background: '#fafbff', border: '1px solid #e0eaff' }}>
                    <div style={{ fontSize: 12.5, color: '#475569', marginBottom: 8 }}>
                      Какая неделя считается «первой» — занятия пройдут только в эти недели:
                    </div>
                    <F.Segmented
                      value={String(cfg.biweeklyParity)}
                      onChange={v => update({ biweeklyParity: parseInt(v, 10) })}
                      options={[
                        { value: '0', label: 'Нечётная (1, 3, 5…)' },
                        { value: '1', label: 'Чётная (2, 4, 6…)' },
                      ]}
                    />
                  </div>
                )}
              </GSSection>

              {/* 2.2 Day & time picker */}
              <GSSection icon="CalendarDays" title="Дни и время" subtitle="Выберите дни недели и время начала каждого занятия"
                step="2.2">
                <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
                  <DayTimeGrid
                    slots={cfg.slots}
                    duration={cfg.duration}
                    onSlotsChange={slots => update({ slots })}
                  />

                  <div>
                    <div style={{ fontSize: 13, fontWeight: 500, color: '#0f172a', marginBottom: 8 }}>
                      Длительность занятия
                    </div>
                    <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
                      {DURATION_PRESETS.map(p => {
                        const active = cfg.duration === p.value;
                        return (
                          <button key={p.value} type="button"
                            onClick={() => update({ duration: p.value })}
                            style={{
                              padding: '10px 16px', borderRadius: 12, cursor: 'pointer',
                              border: `1px solid ${active ? '#4f46e5' : '#e2e8f0'}`,
                              background: active ? 'rgba(79,70,229,0.04)' : '#fff',
                              boxShadow: active ? '0 0 0 3px rgba(79,70,229,0.12)' : 'none',
                              fontFamily: 'inherit', textAlign: 'left',
                              display: 'flex', flexDirection: 'column', gap: 2, minWidth: 100,
                            }}>
                            <span style={{ fontSize: 14, fontWeight: 600,
                              color: active ? '#4338ca' : '#0f172a' }}>{p.label}</span>
                            <span style={{ fontSize: 11.5, color: '#64748b' }}>{p.sub}</span>
                          </button>
                        );
                      })}
                      <CustomDurationInput value={cfg.duration} onChange={v => update({ duration: v })} />
                    </div>
                  </div>

                  {cfg.slots.length > 0 && (
                    <SlotsList slots={cfg.slots} duration={cfg.duration}
                      onSlotChange={(id, patch) => update({
                        slots: cfg.slots.map(s => s.id === id ? { ...s, ...patch } : s),
                      })}
                      onSlotRemove={(id) => update({ slots: cfg.slots.filter(s => s.id !== id) })}
                    />
                  )}
                </div>
              </GSSection>

              {/* 2.3 Period */}
              <GSSection icon="CalendarDays" title="Период" subtitle="С какого числа и до какого момента идут занятия"
                step="2.3">
                <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
                    <F.Field label="Дата начала" required>
                      <F.Text type="date" value={cfg.starts}
                        onChange={e => update({ starts: e.target.value })}
                        icon={<Icon.Calendar size={16} />}
                      />
                    </F.Field>
                    <F.Field label="Завершение курса" required>
                      <F.Segmented
                        value={cfg.endMode}
                        onChange={v => update({ endMode: v })}
                        options={[
                          { value: 'date',  label: 'По дате' },
                          { value: 'count', label: 'По кол-ву занятий' },
                        ]}
                      />
                    </F.Field>
                  </div>
                  {cfg.endMode === 'date' ? (
                    <F.Field label="Дата окончания" required>
                      <F.Text type="date" value={cfg.ends} min={cfg.starts}
                        onChange={e => update({ ends: e.target.value })}
                        icon={<Icon.Calendar size={16} />}
                      />
                    </F.Field>
                  ) : (
                    <F.Field label="Количество занятий" required
                      hint="Дата окончания будет рассчитана автоматически">
                      <CountStepper value={cfg.lessonCount} onChange={v => update({ lessonCount: v })} />
                    </F.Field>
                  )}
                </div>
              </GSSection>

              {/* 2.4 Holidays / exceptions */}
              <GSSection icon="Shield" title="Исключения" subtitle="Дни, когда занятия не проводятся"
                step="2.4">
                <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                  <ToggleRow
                    icon="Sparkles"
                    title="Пропускать государственные праздники"
                    desc="Согласно производственному календарю РФ — занятия в эти дни не назначаются"
                    checked={cfg.skipHolidays}
                    onChange={v => update({ skipHolidays: v })}
                  />
                  {cfg.skipHolidays && (
                    <HolidayList lessons={lessons} />
                  )}
                  <ExceptionPicker
                    exceptions={cfg.exceptions}
                    onAdd={(d) => update({ exceptions: [...cfg.exceptions, d].sort() })}
                    onRemove={(d) => update({ exceptions: cfg.exceptions.filter(x => x !== d) })}
                  />
                </div>
              </GSSection>

              {/* 2.5 Notifications */}
              <GSSection icon="Bell" title="Оповещения" subtitle="Что произойдёт после сохранения"
                step="2.5">
                <ToggleRow
                  icon="Mail"
                  title="Известить студентов"
                  desc="После создания расписание будет автоматически отправлено зачисленным студентам и добавлено в их календарь"
                  checked={cfg.notifyStudents}
                  onChange={v => update({ notifyStudents: v })}
                />
              </GSSection>
            </div>

            {/* Sticky preview */}
            <div style={{ position: 'sticky', top: 0, alignSelf: 'start',
              display: 'flex', flexDirection: 'column', gap: 14 }}>
              <SummaryCard
                heldCount={heldCount}
                skippedCount={skippedCount}
                totalHours={totalHours}
                cfg={cfg}
              />
              <LessonsPreview lessons={lessons} duration={cfg.duration} />
            </div>
          </div>
        </div>

        {/* Sticky save bar */}
        <SaveBarGS
          heldCount={heldCount}
          totalHours={totalHours}
        />
      </div>
    </div>
  );
}

function shortRecurrence(v) {
  if (v === 'weekly')   return 'Нед.';
  if (v === 'biweekly') return '1/2';
  if (v === 'custom')   return 'Свой';
  return v;
}

// ── Section card ─────────────────────────────────────────────────────
function GSSection({ icon, title, subtitle, children, step }) {
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

// ── Day-time grid (heatmap-ish: rows = days, cols = hours) ───────────
function DayTimeGrid({ slots, duration, onSlotsChange }) {
  // Hours range — 8:00 to 22:00, 30-min cells
  const startHour = 8, endHour = 22;
  const cells = [];
  for (let h = startHour; h < endHour; h++) {
    cells.push(h * 60);
    cells.push(h * 60 + 30);
  }

  const slotByDayStart = {};
  slots.forEach(s => {
    const k = `${s.weekday}-${parseTime(s.start)}`;
    slotByDayStart[k] = s;
  });

  const toggleCell = (weekday, mins) => {
    const k = `${weekday}-${mins}`;
    if (slotByDayStart[k]) {
      onSlotsChange(slots.filter(s => s.id !== slotByDayStart[k].id));
    } else {
      const newId = (slots.reduce((m, s) => Math.max(m, s.id), 0) || 0) + 1;
      onSlotsChange([...slots, { id: newId, weekday, start: fmtTime(mins) }]);
    }
  };

  // For shading the full duration of an active slot
  const getCellState = (weekday, mins) => {
    // Is this cell the start?
    if (slotByDayStart[`${weekday}-${mins}`]) return 'start';
    // Is this cell within an active slot's duration?
    for (const s of slots) {
      if (s.weekday !== weekday) continue;
      const sm = parseTime(s.start);
      if (mins > sm && mins < sm + duration) return 'within';
    }
    return 'none';
  };

  // Hour labels (every hour)
  return (
    <div>
      <div style={{
        display: 'flex', alignItems: 'center', gap: 10, marginBottom: 8,
        fontSize: 11.5, color: '#64748b',
      }}>
        <Icon.Info size={13} stroke="#94a3b8" />
        <span>Кликните по ячейке, чтобы добавить занятие. Подсвеченная область — длительность урока.</span>
      </div>
      <div style={{
        border: '1px solid #e2e8f0', borderRadius: 12, overflow: 'hidden',
        background: '#fff',
      }}>
        {/* Hour ruler */}
        <div style={{
          display: 'grid', gridTemplateColumns: `52px repeat(${cells.length}, 1fr)`,
          background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
        }}>
          <div style={{ height: 28 }} />
          {cells.map((m, i) => (
            <div key={m} style={{
              height: 28, display: 'flex', alignItems: 'center', justifyContent: 'flex-start',
              fontSize: 10.5, color: '#94a3b8', fontVariantNumeric: 'tabular-nums',
              borderLeft: i % 2 === 0 ? '1px solid #e2e8f0' : '1px solid transparent',
              paddingLeft: i % 2 === 0 ? 4 : 0,
              fontFamily: 'var(--edv-font-mono)',
            }}>
              {i % 2 === 0 ? fmtTime(m) : ''}
            </div>
          ))}
        </div>
        {/* Day rows */}
        {WEEKDAYS.map((d, di) => (
          <div key={d.id} style={{
            display: 'grid', gridTemplateColumns: `52px repeat(${cells.length}, 1fr)`,
            borderBottom: di < WEEKDAYS.length - 1 ? '1px solid #f1f5f9' : 'none',
          }}>
            <div style={{
              height: 36, display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontSize: 12, fontWeight: 600,
              color: (d.id === 0 || d.id === 6) ? '#b91c1c' : '#475569',
              background: '#fafbfc', borderRight: '1px solid #e2e8f0',
            }}>{d.short}</div>
            {cells.map((m, ci) => {
              const state = getCellState(d.id, m);
              const onHour = ci % 2 === 0;
              let bg = 'transparent';
              let cursor = 'pointer';
              if (state === 'start') bg = '#4f46e5';
              else if (state === 'within') bg = 'rgba(79,70,229,0.15)';
              return (
                <button key={m} type="button"
                  onClick={() => toggleCell(d.id, m)}
                  title={`${d.long} · ${fmtTime(m)}`}
                  style={{
                    height: 36, border: 'none',
                    borderLeft: onHour ? '1px solid #f1f5f9' : '1px solid transparent',
                    background: bg, cursor, padding: 0,
                    transition: 'background .1s',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                  }}
                  onMouseEnter={e => {
                    if (state === 'none') e.currentTarget.style.background = 'rgba(79,70,229,0.06)';
                  }}
                  onMouseLeave={e => {
                    if (state === 'none') e.currentTarget.style.background = 'transparent';
                  }}>
                  {state === 'start' && (
                    <span style={{
                      fontSize: 9.5, color: '#fff', fontFamily: 'var(--edv-font-mono)',
                      fontWeight: 600, letterSpacing: '-0.02em',
                    }}>{fmtTime(m)}</span>
                  )}
                </button>
              );
            })}
          </div>
        ))}
      </div>
    </div>
  );
}

// ── Custom duration ──────────────────────────────────────────────────
function CustomDurationInput({ value, onChange }) {
  const isPreset = DURATION_PRESETS.some(p => p.value === value);
  const [focused, setFocused] = React.useState(false);
  return (
    <div style={{
      display: 'inline-flex', alignItems: 'center', gap: 8,
      padding: '0 14px', height: 50, minWidth: 130,
      borderRadius: 12,
      border: `1px solid ${!isPreset ? '#4f46e5' : focused ? '#6366f1' : '#e2e8f0'}`,
      boxShadow: !isPreset ? '0 0 0 3px rgba(79,70,229,0.12)' : 'none',
      background: !isPreset ? 'rgba(79,70,229,0.04)' : '#fff',
    }}>
      <input
        type="number" min={15} max={300} step={5}
        value={value}
        onChange={e => onChange(parseInt(e.target.value, 10) || 0)}
        onFocus={() => setFocused(true)} onBlur={() => setFocused(false)}
        style={{
          width: 50, height: 30, border: 'none', outline: 'none',
          fontSize: 14, fontWeight: 600, color: '#0f172a',
          fontFamily: 'inherit', fontVariantNumeric: 'tabular-nums', background: 'transparent',
        }}
      />
      <span style={{ fontSize: 11.5, color: '#64748b' }}>мин<br/>свой</span>
    </div>
  );
}

// ── Slots list (editable) ────────────────────────────────────────────
function SlotsList({ slots, duration, onSlotChange, onSlotRemove }) {
  const sorted = [...slots].sort((a,b) => {
    const wa = a.weekday === 0 ? 7 : a.weekday;
    const wb = b.weekday === 0 ? 7 : b.weekday;
    if (wa !== wb) return wa - wb;
    return parseTime(a.start) - parseTime(b.start);
  });
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
      <div style={{ fontSize: 11.5, fontWeight: 600, color: '#64748b',
        letterSpacing: '0.05em', textTransform: 'uppercase' }}>
        Назначенные слоты <span style={{ color: '#94a3b8' }}>({slots.length})</span>
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
        {sorted.map(s => {
          const day = WEEKDAYS.find(d => d.id === s.weekday);
          const startMins = parseTime(s.start);
          const endMins = startMins + duration;
          return (
            <div key={s.id} style={{
              display: 'flex', alignItems: 'center', gap: 12,
              padding: '8px 10px 8px 14px', borderRadius: 10,
              background: '#f8fafc', border: '1px solid #e2e8f0',
            }}>
              <div style={{
                width: 28, height: 28, borderRadius: 8, flexShrink: 0,
                background: '#fff', border: '1px solid #e2e8f0',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                fontSize: 11.5, fontWeight: 700, color: '#475569',
              }}>{day.short}</div>
              <div style={{ flex: 1, minWidth: 0, fontSize: 13, color: '#0f172a' }}>
                <span style={{ fontWeight: 500 }}>{day.long}</span>
                <span style={{ color: '#94a3b8', margin: '0 8px' }}>·</span>
                <span style={{ fontFamily: 'var(--edv-font-mono)', fontVariantNumeric: 'tabular-nums' }}>
                  {fmtTime(startMins)}–{fmtTime(endMins)}
                </span>
                <span style={{ color: '#94a3b8', marginLeft: 8, fontSize: 12 }}>
                  ({duration} мин)
                </span>
              </div>
              <input
                type="time" value={s.start}
                onChange={e => onSlotChange(s.id, { start: e.target.value })}
                style={{
                  height: 30, padding: '0 8px', borderRadius: 8,
                  border: '1px solid #e2e8f0', background: '#fff',
                  fontSize: 12.5, fontFamily: 'var(--edv-font-mono)',
                  fontVariantNumeric: 'tabular-nums', color: '#0f172a', outline: 'none',
                }}
              />
              <button type="button" onClick={() => onSlotRemove(s.id)}
                style={{
                  width: 28, height: 28, borderRadius: 8, border: 'none',
                  background: 'transparent', color: '#94a3b8', cursor: 'pointer',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                }}
                onMouseEnter={e => { e.currentTarget.style.background = '#fee2e2';
                  e.currentTarget.style.color = '#b91c1c'; }}
                onMouseLeave={e => { e.currentTarget.style.background = 'transparent';
                  e.currentTarget.style.color = '#94a3b8'; }}
                title="Удалить слот">
                <Icon.X size={14} />
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
}

// ── Count stepper ────────────────────────────────────────────────────
function CountStepper({ value, onChange }) {
  return (
    <div style={{
      display: 'flex', alignItems: 'center', maxWidth: 200,
      border: '1px solid #e2e8f0', borderRadius: 12,
      background: '#fff', overflow: 'hidden', height: 42,
    }}>
      <button type="button" onClick={() => onChange(Math.max(1, value - 1))}
        style={{ width: 42, height: 42, border: 'none', background: 'transparent',
          color: '#64748b', cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
          strokeWidth="2.5" strokeLinecap="round"><path d="M5 12h14"/></svg>
      </button>
      <input type="number" min={1} max={200} value={value}
        onChange={e => onChange(parseInt(e.target.value, 10) || 1)}
        style={{ flex: 1, height: 42, border: 'none', outline: 'none', textAlign: 'center',
          fontSize: 15, fontWeight: 600, color: '#0f172a', fontFamily: 'inherit',
          fontVariantNumeric: 'tabular-nums', background: 'transparent' }} />
      <button type="button" onClick={() => onChange(Math.min(200, value + 1))}
        style={{ width: 42, height: 42, border: 'none', background: 'transparent',
          color: '#64748b', cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <Icon.Plus size={14} sw={2.5} />
      </button>
    </div>
  );
}

// ── Toggle row ───────────────────────────────────────────────────────
function ToggleRow({ icon, title, desc, checked, onChange }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      display: 'flex', alignItems: 'flex-start', gap: 14,
      padding: '14px 16px', borderRadius: 12,
      border: `1px solid ${checked ? 'rgba(79,70,229,0.2)' : '#e2e8f0'}`,
      background: checked ? 'rgba(79,70,229,0.03)' : '#fff',
      transition: 'all .15s',
    }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: checked ? 'rgba(79,70,229,0.10)' : '#f1f5f9',
        color: checked ? '#4338ca' : '#64748b',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Ic size={16} />
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>{title}</div>
        <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2, lineHeight: 1.45 }}>{desc}</div>
      </div>
      <button type="button" role="switch" aria-checked={checked}
        onClick={() => onChange(!checked)}
        style={{
          width: 38, height: 22, borderRadius: 9999, border: 'none',
          background: checked ? '#4f46e5' : '#cbd5e1', position: 'relative',
          cursor: 'pointer', flexShrink: 0, padding: 0, transition: 'background .15s',
        }}>
        <span style={{
          position: 'absolute', top: 2, left: checked ? 18 : 2,
          width: 18, height: 18, borderRadius: 9999, background: '#fff',
          boxShadow: '0 1px 3px rgba(0,0,0,0.15)', transition: 'left .15s',
        }} />
      </button>
    </div>
  );
}

// ── Holiday list (auto-skipped) ──────────────────────────────────────
function HolidayList({ lessons }) {
  const skipped = lessons.filter(l => l.skipped);
  if (skipped.length === 0) {
    return (
      <div style={{
        padding: '10px 14px', borderRadius: 10, background: '#f8fafc',
        border: '1px dashed #e2e8f0', fontSize: 12.5, color: '#64748b',
      }}>
        В выбранный период государственных праздников нет
      </div>
    );
  }
  return (
    <div style={{ borderRadius: 10, background: '#fafbfc', border: '1px solid #e2e8f0',
      overflow: 'hidden' }}>
      <div style={{
        padding: '8px 12px', borderBottom: '1px solid #e2e8f0', background: '#f8fafc',
        fontSize: 11.5, fontWeight: 600, color: '#64748b',
        letterSpacing: '0.05em', textTransform: 'uppercase',
      }}>
        Будут пропущены ({skipped.length})
      </div>
      <div style={{ display: 'flex', flexDirection: 'column' }}>
        {skipped.slice(0, 4).map((l, i) => {
          const day = WEEKDAYS.find(d => d.id === new Date(l.date+'T00:00:00').getDay());
          return (
            <div key={i} style={{
              display: 'flex', alignItems: 'center', gap: 10,
              padding: '8px 12px',
              borderTop: i > 0 ? '1px solid #f1f5f9' : 'none',
            }}>
              <div style={{
                fontFamily: 'var(--edv-font-mono)', fontSize: 12, color: '#475569',
                fontVariantNumeric: 'tabular-nums', minWidth: 84,
              }}>{fmtDateRu(l.date)}</div>
              <div style={{ fontSize: 11.5, color: '#94a3b8', minWidth: 28 }}>{day?.short}</div>
              <div style={{ flex: 1, fontSize: 12.5, color: '#0f172a' }}>{l.reason}</div>
            </div>
          );
        })}
        {skipped.length > 4 && (
          <div style={{ padding: '6px 12px', fontSize: 11.5, color: '#94a3b8',
            borderTop: '1px solid #f1f5f9' }}>
            …и ещё {skipped.length - 4}
          </div>
        )}
      </div>
    </div>
  );
}

// ── Manual exception picker ──────────────────────────────────────────
function ExceptionPicker({ exceptions, onAdd, onRemove }) {
  const [date, setDate] = React.useState('');
  const submit = () => {
    if (!date || exceptions.includes(date)) return;
    onAdd(date);
    setDate('');
  };
  return (
    <div style={{
      padding: '14px 16px', borderRadius: 12,
      border: '1px solid #e2e8f0', background: '#fff',
    }}>
      <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a', marginBottom: 4 }}>
        Дополнительные исключения
      </div>
      <div style={{ fontSize: 12.5, color: '#64748b', marginBottom: 12 }}>
        Каникулы, выезды, особые даты — добавьте вручную
      </div>
      <div style={{ display: 'flex', gap: 8, alignItems: 'flex-end' }}>
        <div style={{ flex: 1 }}>
          <F.Text type="date" value={date}
            onChange={e => setDate(e.target.value)}
            icon={<Icon.Calendar size={16} />} />
        </div>
        <Button variant="secondary" size="md" onClick={submit} disabled={!date}>
          <Icon.Plus size={14} />Добавить
        </Button>
      </div>
      {exceptions.length > 0 && (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6, marginTop: 12 }}>
          {exceptions.map(d => (
            <span key={d} style={{
              display: 'inline-flex', alignItems: 'center', gap: 6,
              padding: '4px 6px 4px 10px', borderRadius: 9999,
              background: '#fef3c7', color: '#92400e',
              fontSize: 12, fontFamily: 'var(--edv-font-mono)',
              fontVariantNumeric: 'tabular-nums',
            }}>
              {fmtDateRu(d)}
              <button type="button" onClick={() => onRemove(d)} style={{
                width: 16, height: 16, borderRadius: 9999, border: 'none',
                background: 'rgba(146,64,14,0.15)', color: '#92400e',
                cursor: 'pointer', display: 'inline-flex',
                alignItems: 'center', justifyContent: 'center',
              }}>
                <Icon.X size={9} sw={3} />
              </button>
            </span>
          ))}
        </div>
      )}
    </div>
  );
}

// ── Summary card ─────────────────────────────────────────────────────
function SummaryCard({ heldCount, skippedCount, totalHours, cfg }) {
  const slotsByDay = {};
  cfg.slots.forEach(s => {
    if (!slotsByDay[s.weekday]) slotsByDay[s.weekday] = [];
    slotsByDay[s.weekday].push(s);
  });
  const summaryParts = WEEKDAYS.filter(d => slotsByDay[d.id])
    .map(d => {
      const ts = slotsByDay[d.id].map(s => s.start).join(', ');
      return `${d.short} · ${ts}`;
    });

  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      overflow: 'hidden',
    }}>
      <div style={{
        padding: '12px 16px', borderBottom: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', gap: 8,
        fontSize: 11.5, fontWeight: 600, color: '#64748b',
        letterSpacing: '0.05em', textTransform: 'uppercase',
      }}>
        <Icon.Sparkles size={13} stroke="#4f46e5" />
        <span>Итоги расписания</span>
      </div>
      <div style={{ padding: 16, display: 'flex', flexDirection: 'column', gap: 14 }}>
        {/* Stats */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8 }}>
          <StatBox value={heldCount} unit={declensionGS(heldCount, ['занятие','занятия','занятий'])}
            label="Состоится" tone="primary" />
          <StatBox value={totalHours} unit="часов"
            label="Учебная нагрузка" tone="success" />
        </div>

        {/* Pattern */}
        <div>
          <div style={{ fontSize: 11.5, fontWeight: 600, color: '#64748b',
            letterSpacing: '0.05em', textTransform: 'uppercase', marginBottom: 6 }}>
            Шаблон
          </div>
          {summaryParts.length === 0 ? (
            <div style={{ fontSize: 13, color: '#94a3b8', fontStyle: 'italic' }}>
              Слоты не выбраны
            </div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
              {summaryParts.map((p, i) => (
                <div key={i} style={{
                  fontSize: 13, color: '#0f172a',
                  fontFamily: 'var(--edv-font-mono)',
                  fontVariantNumeric: 'tabular-nums',
                }}>{p}</div>
              ))}
              <div style={{ fontSize: 12, color: '#64748b', marginTop: 2 }}>
                {cfg.duration} мин · {cfg.recurrence === 'biweekly' ? 'через неделю' : 'каждую неделю'}
              </div>
            </div>
          )}
        </div>

        {/* Period */}
        <div style={{ paddingTop: 12, borderTop: '1px solid #f1f5f9' }}>
          <div style={{ fontSize: 11.5, fontWeight: 600, color: '#64748b',
            letterSpacing: '0.05em', textTransform: 'uppercase', marginBottom: 6 }}>
            Период
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 13,
            fontVariantNumeric: 'tabular-nums', color: '#0f172a' }}>
            <span style={{ fontFamily: 'var(--edv-font-mono)' }}>{fmtDateRu(cfg.starts)}</span>
            <Icon.ArrowRight size={14} stroke="#94a3b8" />
            <span style={{ fontFamily: 'var(--edv-font-mono)' }}>{fmtDateRu(cfg.ends)}</span>
          </div>
          {skippedCount > 0 && (
            <div style={{ marginTop: 6, fontSize: 12, color: '#92400e',
              display: 'inline-flex', alignItems: 'center', gap: 5 }}>
              <Icon.AlertCircle size={12} stroke="#92400e" />
              Будут пропущены {skippedCount} {declensionGS(skippedCount, ['день','дня','дней'])}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

function StatBox({ value, unit, label, tone }) {
  const tones = {
    primary: { bg: 'rgba(79,70,229,0.08)', fg: '#4338ca' },
    success: { bg: 'rgba(16,185,129,0.10)', fg: '#047857' },
  };
  const t = tones[tone];
  return (
    <div style={{
      padding: '10px 12px', borderRadius: 10, background: t.bg,
      display: 'flex', flexDirection: 'column', gap: 4,
    }}>
      <div style={{ display: 'baseline', display: 'flex', alignItems: 'baseline', gap: 4 }}>
        <span style={{
          fontSize: 22, fontWeight: 700, color: t.fg, lineHeight: 1,
          fontVariantNumeric: 'tabular-nums', letterSpacing: '-0.02em',
        }}>{value}</span>
        <span style={{ fontSize: 11.5, color: t.fg, opacity: 0.75 }}>{unit}</span>
      </div>
      <div style={{ fontSize: 11.5, color: '#64748b' }}>{label}</div>
    </div>
  );
}

// ── Lessons preview ──────────────────────────────────────────────────
function LessonsPreview({ lessons, duration }) {
  // Group by month for better scanability
  const byMonth = {};
  lessons.forEach(l => {
    const [y, m] = l.date.split('-');
    const k = `${y}-${m}`;
    if (!byMonth[k]) byMonth[k] = [];
    byMonth[k].push(l);
  });
  const monthKeys = Object.keys(byMonth).sort();
  const RU_MONTHS = ['Январь','Февраль','Март','Апрель','Май','Июнь',
    'Июль','Август','Сентябрь','Октябрь','Ноябрь','Декабрь'];

  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      display: 'flex', flexDirection: 'column', maxHeight: 540, overflow: 'hidden',
    }}>
      <div style={{
        padding: '12px 16px', borderBottom: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', gap: 8,
      }}>
        <Icon.CalendarDays size={14} stroke="#4f46e5" />
        <span style={{ flex: 1, fontSize: 11.5, fontWeight: 600, color: '#64748b',
          letterSpacing: '0.05em', textTransform: 'uppercase' }}>Все занятия</span>
        <span style={{
          fontSize: 11.5, fontWeight: 600, color: '#4338ca',
          padding: '2px 8px', borderRadius: 9999,
          background: 'rgba(79,70,229,0.08)', fontVariantNumeric: 'tabular-nums',
        }}>{lessons.length}</span>
      </div>
      <div style={{ flex: 1, overflowY: 'auto' }}>
        {lessons.length === 0 ? (
          <div style={{
            padding: '32px 16px', textAlign: 'center', fontSize: 13, color: '#94a3b8',
          }}>
            <Icon.Calendar size={28} stroke="#cbd5e1" style={{ marginBottom: 8 }} />
            <div>Выберите дни и время — здесь появится список занятий</div>
          </div>
        ) : monthKeys.map(mk => {
          const [y, m] = mk.split('-');
          const monthName = RU_MONTHS[parseInt(m, 10) - 1];
          const monthLessons = byMonth[mk];
          const held = monthLessons.filter(l => !l.skipped).length;
          return (
            <div key={mk}>
              <div style={{
                position: 'sticky', top: 0, zIndex: 1,
                padding: '8px 16px', background: '#f8fafc',
                borderBottom: '1px solid #f1f5f9', borderTop: '1px solid #f1f5f9',
                display: 'flex', alignItems: 'center', gap: 8,
              }}>
                <span style={{ fontSize: 12, fontWeight: 600, color: '#0f172a' }}>
                  {monthName} {y}
                </span>
                <span style={{ fontSize: 11.5, color: '#64748b' }}>
                  {held} {declensionGS(held, ['занятие','занятия','занятий'])}
                </span>
              </div>
              {monthLessons.map((l, i) => <LessonRow key={i} lesson={l} duration={duration} />)}
            </div>
          );
        })}
      </div>
    </div>
  );
}

function LessonRow({ lesson, duration }) {
  const date = new Date(lesson.date + 'T00:00:00');
  const day = WEEKDAYS.find(d => d.id === date.getDay());
  const startMins = parseTime(lesson.slot.start);
  const endMins = startMins + duration;
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 10,
      padding: '8px 16px',
      borderBottom: '1px solid #f8fafc',
      opacity: lesson.skipped ? 0.55 : 1,
    }}>
      <div style={{
        width: 36, textAlign: 'center', flexShrink: 0,
      }}>
        <div style={{
          fontSize: 14, fontWeight: 700, color: lesson.skipped ? '#94a3b8' : '#0f172a',
          lineHeight: 1, fontVariantNumeric: 'tabular-nums',
        }}>{date.getDate()}</div>
        <div style={{
          fontSize: 10, color: '#94a3b8', marginTop: 2, textTransform: 'uppercase',
          letterSpacing: '0.05em', fontWeight: 600,
        }}>{day?.short}</div>
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{
          fontSize: 12.5, color: lesson.skipped ? '#94a3b8' : '#0f172a',
          fontFamily: 'var(--edv-font-mono)', fontVariantNumeric: 'tabular-nums',
          textDecoration: lesson.skipped ? 'line-through' : 'none',
        }}>
          {fmtTime(startMins)}–{fmtTime(endMins)}
        </div>
        {lesson.skipped && (
          <div style={{ fontSize: 11, color: '#92400e', marginTop: 2 }}>
            <Icon.Info size={10} stroke="#92400e" /> {lesson.reason}
          </div>
        )}
      </div>
      {lesson.skipped ? (
        <span style={{
          fontSize: 10.5, fontWeight: 600, color: '#92400e',
          padding: '2px 7px', borderRadius: 9999, background: '#fef3c7',
          letterSpacing: '0.03em', textTransform: 'uppercase',
        }}>пропуск</span>
      ) : (
        <span style={{
          width: 6, height: 6, borderRadius: 9999, background: '#10b981',
        }} />
      )}
    </div>
  );
}

// ── Progress indicator ───────────────────────────────────────────────
function ProgressIndicatorGS({ current, steps }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
      {steps.map((s, i) => {
        const done = current > s.id;
        const active = current === s.id;
        return (
          <React.Fragment key={s.id}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <div style={{
                width: 24, height: 24, borderRadius: 9999, flexShrink: 0,
                background: done ? '#4f46e5' : active ? 'rgba(79,70,229,0.12)' : '#f1f5f9',
                color: done ? '#fff' : active ? '#4338ca' : '#94a3b8',
                border: active ? '1.5px solid #4f46e5' : 'none',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                fontSize: 11.5, fontWeight: 700, fontVariantNumeric: 'tabular-nums',
              }}>
                {done ? <Icon.Check size={12} stroke="#fff" sw={3} /> : s.id}
              </div>
              <span style={{
                fontSize: 12.5, fontWeight: active ? 600 : 500,
                color: active ? '#0f172a' : done ? '#475569' : '#94a3b8',
              }}>{s.label}</span>
            </div>
            {i < steps.length - 1 && (
              <div style={{ width: 18, height: 1, background: '#e2e8f0' }} />
            )}
          </React.Fragment>
        );
      })}
    </div>
  );
}

// ── Sticky save bar ──────────────────────────────────────────────────
function SaveBarGS({ heldCount, totalHours }) {
  return (
    <div style={{
      position: 'absolute', left: 0, right: 0, bottom: 0,
      background: '#fff', borderTop: '1px solid #e2e8f0',
      boxShadow: '0 -4px 12px rgba(15,23,42,0.06)',
      padding: '14px 32px',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 20,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, fontSize: 13 }}>
        <div style={{
          width: 32, height: 32, borderRadius: 9999, flexShrink: 0,
          background: 'rgba(16,185,129,0.12)', color: '#047857',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <Icon.CircleCheck size={16} stroke="#047857" />
        </div>
        <div>
          <div style={{ fontWeight: 600, color: '#0f172a' }}>
            Готово к сохранению — {heldCount} {declensionGS(heldCount, ['занятие','занятия','занятий'])}, {totalHours} ч.
          </div>
          <div style={{ fontSize: 12, color: '#64748b' }}>
            После сохранения откроется зачисление студентов
          </div>
        </div>
      </div>
      <div style={{ display: 'flex', gap: 10 }}>
        <a href="Group Create.html"><Button variant="ghost">Назад</Button></a>
        <Button variant="secondary">Сохранить как черновик</Button>
        <a href="Group Students.html">
          <Button>
            Сохранить и продолжить<Icon.ArrowRight size={15} sw={2.5} />
          </Button>
        </a>
      </div>
    </div>
  );
}

window.GroupScheduleApp = GroupScheduleApp;
