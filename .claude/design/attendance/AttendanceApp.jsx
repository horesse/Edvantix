// Attendance App — журнал посещаемости группы EN-B1-12
const { useState: useStateA, useMemo: useMemoA, useEffect: useEffectA, useRef: useRefA } = React;

const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "density": "regular",
  "showPercents": true,
  "colorMode": "soft",
  "period": "all"
}/*EDITMODE-END*/;

const STATUS_ICON = {
  present: (sz) => <Icon.Check size={sz} sw={3} />,
  absent:  (sz) => <Icon.X size={sz} sw={3} />,
  late:    (sz) => <Icon.Clock size={sz} sw={2.2} />,
  excused: (sz) => <Icon.Shield size={sz} sw={2.2} />,
};

// Mini-icons local to this file
const PrinterIcon = ({size=14, stroke='currentColor'}) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={stroke}
    strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <polyline points="6 9 6 2 18 2 18 9"/><path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"/>
    <rect x="6" y="14" width="12" height="8"/>
  </svg>
);
const DownloadIcon = ({size=14, stroke='currentColor'}) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={stroke}
    strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
    <polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/>
  </svg>
);
const HeartIcon = ({size=12, stroke='currentColor'}) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={stroke}
    strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
  </svg>
);

// ── Helpers ─────────────────────────────────────────────────────────────
function pct(n, d) {
  if (!d) return 0;
  return Math.round((n / d) * 100);
}

function studentSummary(studentId, log) {
  const rec = log[studentId] || {};
  let present = 0, absent = 0, late = 0, excused = 0, none = 0, cancelled = 0;
  Object.values(rec).forEach(s => {
    if (s === 'present')   present++;
    else if (s === 'absent') absent++;
    else if (s === 'late') late++;
    else if (s === 'excused') excused++;
    else if (s === 'cancelled') cancelled++;
    else none++;
  });
  const effective = present + absent + late + excused; // занятия, в которых студент мог быть
  const attended = present + late + excused;            // зачитываем поздних и больных
  return { present, absent, late, excused, none, cancelled,
    effective, attended, rate: pct(attended, effective) };
}

function lessonSummary(lessonId, log) {
  let present = 0, absent = 0, late = 0, excused = 0, none = 0;
  ATT_STUDENTS.forEach(s => {
    const st = log[s.id]?.[lessonId] || 'none';
    if (st === 'present')   present++;
    else if (st === 'absent') absent++;
    else if (st === 'late') late++;
    else if (st === 'excused') excused++;
    else none++;
  });
  const total = ATT_STUDENTS.length;
  return { present, absent, late, excused, none, total,
    rate: pct(present + late + excused, total) };
}

function declensionA(n, f) {
  const a = Math.abs(n), m10 = a % 10, m100 = a % 100;
  if (m10 === 1 && m100 !== 11) return f[0];
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return f[1];
  return f[2];
}

// ── Root app ────────────────────────────────────────────────────────────
function AttendanceApp() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);
  const [log, setLog] = useStateA(() => JSON.parse(JSON.stringify(ATT_LOG)));
  const [period, setPeriod] = useStateA('all'); // 'all' | 'apr' | 'may' | 'future'
  const [query, setQuery] = useStateA('');
  const [openStudent, setOpenStudent] = useStateA(null);
  const [cellNote, setCellNote] = useStateA(null); // {studentId, lessonId, x, y}
  const [toast, setToast] = useStateA(null);

  // Sync period to/from tweak
  useEffectA(() => {
    if (t.period && t.period !== period) setPeriod(t.period);
  }, [t.period]);

  const visibleLessons = useMemoA(() => {
    if (period === 'all') return ATT_LESSONS;
    if (period === 'apr')   return ATT_LESSONS.filter(l => l.date.endsWith('.04'));
    if (period === 'may')   return ATT_LESSONS.filter(l => l.date.endsWith('.05') && !l.isFuture);
    if (period === 'future')return ATT_LESSONS.filter(l => l.isFuture);
    return ATT_LESSONS;
  }, [period]);

  const filteredStudents = useMemoA(() => {
    const q = query.trim().toLowerCase();
    if (!q) return ATT_STUDENTS;
    return ATT_STUDENTS.filter(s => s.name.toLowerCase().includes(q));
  }, [query]);

  // Group-level KPIs
  const kpi = useMemoA(() => {
    const heldLessons = ATT_LESSONS.filter(l => !l.isFuture && !l.isCancelled);
    let totalPresent = 0, totalEffective = 0, totalAbsent = 0;
    let atRisk = 0;
    ATT_STUDENTS.forEach(s => {
      const sm = studentSummary(s.id, log);
      totalPresent += sm.attended;
      totalEffective += sm.effective;
      totalAbsent += sm.absent;
      if (sm.effective > 0 && sm.rate < 75) atRisk++;
    });
    const next = ATT_LESSONS.find(l => l.isFuture);
    return {
      avg: pct(totalPresent, totalEffective),
      held: heldLessons.length,
      total: ATT_LESSONS.length,
      atRisk,
      absences: totalAbsent,
      next: next ? next.full : '—',
    };
  }, [log]);

  const setStatus = (studentId, lessonId, status) => {
    setLog(prev => ({
      ...prev,
      [studentId]: { ...(prev[studentId] || {}), [lessonId]: status },
    }));
  };

  const cycleCell = (studentId, lessonId) => {
    const lesson = ATT_LESSONS.find(l => l.id === lessonId);
    if (lesson?.isCancelled || lesson?.isFuture) return;
    const cur = log[studentId]?.[lessonId] || 'none';
    const idx = ATT_CYCLE.indexOf(cur);
    const next = ATT_CYCLE[(idx + 1) % ATT_CYCLE.length];
    setStatus(studentId, lessonId, next);
  };

  const markAllPresent = (lessonId) => {
    setLog(prev => {
      const next = { ...prev };
      ATT_STUDENTS.forEach(s => {
        next[s.id] = { ...(next[s.id] || {}), [lessonId]: 'present' };
      });
      return next;
    });
    const l = ATT_LESSONS.find(x => x.id === lessonId);
    flash(`Отмечено: «все присутствуют» на ${l?.date || ''}`);
  };

  const flash = (msg) => {
    setToast(msg);
    setTimeout(() => setToast(null), 2400);
  };

  // ── render ────────────────────────────────────────────────────────────
  const density = t.density || 'regular';
  const colorMode = t.colorMode || 'soft';
  const showPct = t.showPercents !== false;

  return (
    <div style={{ display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden' }}>
      <Sidebar active="attendance" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <a href="Groups.html" style={{ color: '#64748b' }}>Школа</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span>Посещаемость</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>EN-B1-12 · English Intermediate</span>
        </div>

        {/* Page header */}
        <div style={{
          padding: '20px 32px 14px', borderBottom: '1px solid #e2e8f0',
          background: '#fff',
        }}>
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 20 }}>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 4 }}>
                <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
                  Посещаемость
                </h1>
                <span style={{
                  fontFamily: 'var(--edv-font-mono)', fontSize: 12, color: '#64748b',
                  padding: '3px 8px', borderRadius: 8, background: '#f1f5f9',
                }}>{ATT_GROUP.code}</span>
              </div>
              <div style={{ fontSize: 13, color: '#64748b' }}>
                Журнал группы «{ATT_GROUP.name}»
                <span style={{ margin: '0 8px', color: '#cbd5e1' }}>·</span>
                {ATT_GROUP.teacher}
                <span style={{ margin: '0 8px', color: '#cbd5e1' }}>·</span>
                {ATT_GROUP.schedule}
                <span style={{ margin: '0 8px', color: '#cbd5e1' }}>·</span>
                {ATT_GROUP.room}
              </div>
            </div>
            <div style={{ display: 'flex', gap: 8 }}>
              <Button variant="secondary" size="md"><DownloadIcon size={14} stroke="#475569" />Экспорт</Button>
              <Button variant="secondary" size="md"><PrinterIcon size={14} stroke="#475569" />Печать журнала</Button>
              <Button size="md"><Icon.Megaphone size={15} sw={2.2} />Уведомить отстающих</Button>
            </div>
          </div>
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '20px 32px 32px' }}>
          <div style={{ maxWidth: 1480, margin: '0 auto', display: 'flex',
            flexDirection: 'column', gap: 18 }}>

            <KpiStrip kpi={kpi} />

            {/* The matrix card */}
            <section style={{
              background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
              overflow: 'hidden',
            }}>
              <MatrixToolbar
                query={query} onQuery={setQuery}
                period={period} onPeriod={(p) => { setPeriod(p); setTweak('period', p); }}
                visibleLessons={visibleLessons}
                onMarkAllToday={() => {
                  const today = ATT_LESSONS.find(l => l.isJustHappened);
                  if (today) markAllPresent(today.id);
                }}
              />
              <Matrix
                students={filteredStudents}
                lessons={visibleLessons}
                log={log}
                density={density}
                colorMode={colorMode}
                showPct={showPct}
                onCellClick={cycleCell}
                onCellRightClick={(sid, lid, e) => {
                  e.preventDefault();
                  setCellNote({ studentId: sid, lessonId: lid });
                }}
                onMarkAllPresent={markAllPresent}
                onOpenStudent={setOpenStudent}
              />
              <Legend />
            </section>
          </div>
        </div>
      </div>

      {/* Drawer */}
      {openStudent && (
        <StudentDrawer
          student={openStudent}
          log={log}
          onClose={() => setOpenStudent(null)}
          onNotify={(s) => { flash(`Уведомление родителям ${s.name} отправлено`); setOpenStudent(null); }}
          onSetStatus={(lid, st) => setStatus(openStudent.id, lid, st)}
        />
      )}

      {/* Note dialog */}
      {cellNote && (
        <NoteDialog
          ctx={cellNote}
          log={log}
          onSave={(text) => {
            ATT_NOTES[`${cellNote.studentId}-${cellNote.lessonId}`] = text;
            setCellNote(null);
            flash('Комментарий сохранён');
          }}
          onClose={() => setCellNote(null)}
        />
      )}

      {/* Toast */}
      {toast && (
        <div style={{
          position: 'fixed', bottom: 24, left: '50%', transform: 'translateX(-50%)',
          background: '#0f172a', color: '#fff', padding: '10px 18px',
          borderRadius: 12, fontSize: 13, fontWeight: 500,
          boxShadow: '0 12px 32px rgba(15,23,42,0.30)', zIndex: 50,
          display: 'flex', alignItems: 'center', gap: 10,
          animation: 'fadeIn 0.2s ease',
        }}>
          <Icon.CircleCheck size={16} stroke="#10b981" sw={2.5} />
          {toast}
        </div>
      )}

      <AttTweaks t={t} setTweak={setTweak} />
    </div>
  );
}

// ── KPI strip ──────────────────────────────────────────────────────────
function KpiStrip({ kpi }) {
  return (
    <div style={{ display: 'grid', gap: 14,
      gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))' }}>
      <KpiTile label="Средняя посещаемость"
        value={`${kpi.avg}%`}
        sub={<>за {kpi.held} {declensionA(kpi.held, ['занятие','занятия','занятий'])} периода</>}
        tone="success" icon="CircleCheck" />
      <KpiTile label="Студенты в зоне риска"
        value={kpi.atRisk}
        sub={<>посещаемость &lt; 75%</>}
        tone="danger" icon="AlertCircle" />
      <KpiTile label="Пропусков всего"
        value={kpi.absences}
        sub="без уваж. причины"
        tone="warning" icon="X" />
      <KpiTile label="Следующее занятие"
        value="18.05"
        sub={kpi.next} tone="primary" icon="Calendar" />
    </div>
  );
}
function KpiTile({ label, value, sub, tone, icon }) {
  const tones = {
    success: { bg: 'rgba(16,185,129,0.10)', fg: '#047857', accent: '#10b981' },
    danger:  { bg: 'rgba(239,68,68,0.10)',  fg: '#b91c1c', accent: '#ef4444' },
    warning: { bg: 'rgba(245,158,11,0.12)', fg: '#92400e', accent: '#f59e0b' },
    primary: { bg: 'rgba(79,70,229,0.10)',  fg: '#4338ca', accent: '#6366f1' },
  };
  const T = tones[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '14px 16px', display: 'flex', alignItems: 'center', gap: 14,
    }}>
      <div style={{
        width: 42, height: 42, borderRadius: 12, flexShrink: 0,
        background: T.bg, color: T.fg,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}><Ic size={20} sw={2.2} /></div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 11, fontWeight: 600, color: '#64748b',
          letterSpacing: '0.04em', textTransform: 'uppercase' }}>{label}</div>
        <div style={{ display: 'flex', alignItems: 'baseline', gap: 6, marginTop: 2 }}>
          <span style={{ fontSize: 26, fontWeight: 700, color: '#0f172a',
            letterSpacing: '-0.02em', fontVariantNumeric: 'tabular-nums' }}>{value}</span>
        </div>
        <div style={{ fontSize: 12, color: '#64748b', marginTop: 2 }}>{sub}</div>
      </div>
    </div>
  );
}

// ── Matrix toolbar ─────────────────────────────────────────────────────
function MatrixToolbar({ query, onQuery, period, onPeriod, visibleLessons, onMarkAllToday }) {
  const PERIODS = [
    { value: 'all',    label: 'Весь период' },
    { value: 'apr',    label: 'Апрель' },
    { value: 'may',    label: 'Май' },
    { value: 'future', label: 'Будущие' },
  ];
  return (
    <div style={{
      padding: '14px 16px', borderBottom: '1px solid #f1f5f9',
      display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap',
    }}>
      <div style={{ position: 'relative', width: 280, height: 36 }}>
        <Icon.Search size={14} stroke="#94a3b8"
          style={{ position: 'absolute', left: 12, top: 11 }} />
        <input value={query} onChange={e => onQuery(e.target.value)}
          placeholder="Поиск студента"
          style={{
            width: '100%', height: 36, paddingLeft: 34, paddingRight: 12,
            borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
            fontSize: 13, fontFamily: 'inherit', color: '#0f172a', outline: 'none',
          }}
          onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.2)'; }}
          onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }} />
      </div>

      <div style={{
        display: 'inline-flex', alignItems: 'center', height: 36, padding: 3,
        borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
      }}>
        {PERIODS.map(p => {
          const active = period === p.value;
          return (
            <button key={p.value} type="button" onClick={() => onPeriod(p.value)}
              style={{
                height: 28, padding: '0 12px', border: 'none', cursor: 'pointer',
                borderRadius: 8,
                background: active ? '#0f172a' : 'transparent',
                color: active ? '#fff' : '#475569',
                fontSize: 12.5, fontWeight: active ? 600 : 500,
                fontFamily: 'inherit',
              }}>{p.label}</button>
          );
        })}
      </div>

      <span style={{ fontSize: 12.5, color: '#64748b' }}>
        Видимо <strong style={{ color: '#0f172a' }}>{visibleLessons.length}</strong> {declensionA(visibleLessons.length, ['занятие','занятия','занятий'])}
      </span>

      <div style={{ flex: 1 }} />

      <button type="button" onClick={onMarkAllToday}
        style={{
          height: 36, padding: '0 14px', borderRadius: 10,
          border: '1px solid rgba(16,185,129,0.3)',
          background: 'rgba(16,185,129,0.08)', color: '#047857',
          display: 'inline-flex', alignItems: 'center', gap: 8,
          fontSize: 13, fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer',
        }}
        onMouseEnter={e => { e.currentTarget.style.background = 'rgba(16,185,129,0.16)'; }}
        onMouseLeave={e => { e.currentTarget.style.background = 'rgba(16,185,129,0.08)'; }}>
        <Icon.UserCheck size={15} stroke="#047857" sw={2.2} />
        Все присутствуют — 13.05
      </button>
    </div>
  );
}

// ── The matrix ─────────────────────────────────────────────────────────
function Matrix({ students, lessons, log, density, colorMode, showPct,
                  onCellClick, onCellRightClick, onMarkAllPresent, onOpenStudent }) {
  const colW    = density === 'compact' ? 44 : 56;
  const rowH    = density === 'compact' ? 36 : 44;
  const nameW   = 280;
  const sumW    = 132;

  // Lesson summary row at the bottom
  const summaries = lessons.map(l => ({ id: l.id, sum: lessonSummary(l.id, log) }));

  // Group lessons by month for the month-strip
  const monthGroups = useMemoA(() => {
    const groups = [];
    let cur = null;
    lessons.forEach(l => {
      const month = l.date.split('.')[1];
      if (!cur || cur.month !== month) {
        cur = { month, lessons: [l] };
        groups.push(cur);
      } else {
        cur.lessons.push(l);
      }
    });
    return groups;
  }, [lessons]);

  const RU_MONTH = { '04': 'Апрель', '05': 'Май', '06': 'Июнь' };

  return (
    <div style={{ overflowX: 'auto', position: 'relative' }}>
      <div style={{
        minWidth: nameW + sumW + colW * lessons.length,
        display: 'grid',
        gridTemplateColumns: `${nameW}px repeat(${lessons.length}, ${colW}px) ${sumW}px`,
      }}>
        {/* ── Header row 1 — month groups ── */}
        <div style={hCellL(nameW, '#fafbfc', '#e2e8f0')} />
        {monthGroups.map((g, gi) => (
          <div key={gi} style={{
            gridColumn: `span ${g.lessons.length}`,
            height: 32, background: '#fafbfc',
            borderBottom: '1px solid #f1f5f9',
            borderRight: '1px solid #f1f5f9',
            display: 'flex', alignItems: 'center', paddingLeft: 12,
            fontSize: 11.5, fontWeight: 600, color: '#475569',
            letterSpacing: '0.03em',
          }}>
            {RU_MONTH[g.month] || g.month} 2026
          </div>
        ))}
        <div style={{
          height: 32, background: '#fafbfc',
          borderBottom: '1px solid #f1f5f9',
          display: 'flex', alignItems: 'center', justifyContent: 'flex-end',
          paddingRight: 14, fontSize: 11, fontWeight: 600, color: '#94a3b8',
          letterSpacing: '0.05em', textTransform: 'uppercase',
        }}>Итого</div>

        {/* ── Header row 2 — dates ── */}
        <div style={{
          ...hCellL(nameW, '#fff', '#e2e8f0'),
          fontSize: 11, fontWeight: 600, color: '#94a3b8',
          letterSpacing: '0.05em', textTransform: 'uppercase',
          paddingLeft: 16, display: 'flex', alignItems: 'center',
          borderBottom: '1px solid #e2e8f0',
        }}>
          Студенты <span style={{ marginLeft: 6, color: '#cbd5e1' }}>·</span>
          <span style={{ marginLeft: 6, color: '#475569' }}>{students.length}</span>
        </div>
        {lessons.map(l => (
          <LessonHeader key={l.id} lesson={l} colW={colW}
            onMarkAll={() => onMarkAllPresent(l.id)} />
        ))}
        <div style={{
          background: '#fff', borderBottom: '1px solid #e2e8f0',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 10.5, color: '#94a3b8', letterSpacing: '0.05em',
          textTransform: 'uppercase', fontWeight: 600,
        }}>посещ.</div>

        {/* ── Student rows ── */}
        {students.map((s, ri) => {
          const sum = studentSummary(s.id, log);
          const lastBorder = ri === students.length - 1 ? 'none' : '1px solid #f1f5f9';
          return (
            <React.Fragment key={s.id}>
              <StudentNameCell student={s} sum={sum} height={rowH}
                showPct={showPct} onOpen={() => onOpenStudent(s)} borderBottom={lastBorder} />
              {lessons.map(l => {
                const status = log[s.id]?.[l.id] || (l.isCancelled ? 'cancelled' : 'none');
                const note = ATT_NOTES[`${s.id}-${l.id}`];
                return (
                  <Cell key={l.id} status={status} lesson={l}
                    height={rowH} width={colW} colorMode={colorMode}
                    note={note}
                    onClick={() => onCellClick(s.id, l.id)}
                    onContextMenu={(e) => onCellRightClick(s.id, l.id, e)}
                    borderBottom={lastBorder} />
                );
              })}
              <StudentRateCell sum={sum} height={rowH} showPct={showPct} borderBottom={lastBorder} />
            </React.Fragment>
          );
        })}

        {/* ── Footer summary row ── */}
        <div style={{
          ...hCellL(nameW, '#fafbfc', '#e2e8f0'),
          fontSize: 11, fontWeight: 600, color: '#475569',
          paddingLeft: 16, display: 'flex', alignItems: 'center',
          letterSpacing: '0.03em', textTransform: 'uppercase',
          borderTop: '1px solid #e2e8f0',
        }}>посещ. занятия</div>
        {summaries.map(({ id, sum }) => {
          const l = lessons.find(x => x.id === id);
          if (l.isCancelled || l.isFuture) {
            return (
              <div key={id} style={{
                height: 40, borderRight: '1px solid #f1f5f9',
                borderTop: '1px solid #e2e8f0', background: '#fafbfc',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                fontSize: 12, color: '#94a3b8',
              }}>—</div>
            );
          }
          const r = sum.rate;
          const tone = r >= 90 ? { fg: '#047857', bg: 'rgba(16,185,129,0.12)' }
                    : r >= 70 ? { fg: '#92400e', bg: 'rgba(245,158,11,0.14)' }
                              : { fg: '#b91c1c', bg: 'rgba(239,68,68,0.10)' };
          return (
            <div key={id} title={`Присутствуют ${sum.present + sum.late + sum.excused} из ${sum.total}`} style={{
              height: 40, borderRight: '1px solid #f1f5f9',
              borderTop: '1px solid #e2e8f0', background: '#fafbfc',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              padding: '0 4px',
            }}>
              <span style={{
                fontSize: 11.5, fontWeight: 700, color: tone.fg,
                padding: '3px 7px', borderRadius: 6, background: tone.bg,
                fontVariantNumeric: 'tabular-nums',
              }}>{r}%</span>
            </div>
          );
        })}
        <div style={{
          height: 40, background: '#fafbfc', borderTop: '1px solid #e2e8f0',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 11.5, color: '#94a3b8',
        }}>—</div>
      </div>
    </div>
  );
}
function hCellL(width, bg, borderColor) {
  return {
    width, background: bg,
    borderRight: `1px solid ${borderColor}`,
    position: 'sticky', left: 0, zIndex: 2,
  };
}

// ── Lesson column header ───────────────────────────────────────────────
function LessonHeader({ lesson, colW, onMarkAll }) {
  const [hover, setHover] = useStateA(false);
  const isToday = lesson.isJustHappened;
  const isCancelled = lesson.isCancelled;
  const isFuture = lesson.isFuture;
  const day = lesson.date.split('.')[0];
  const wd = lesson.full.split(',')[0];

  let bg = '#fff';
  let topAccent = null;
  if (isToday)     { bg = 'rgba(79,70,229,0.06)'; topAccent = '#4f46e5'; }
  if (isCancelled) bg = 'repeating-linear-gradient(45deg,#f8fafc,#f8fafc 4px,#f1f5f9 4px,#f1f5f9 8px)';
  if (isFuture)    bg = '#fafbfc';

  return (
    <div style={{
      position: 'relative', background: bg,
      borderBottom: '1px solid #e2e8f0',
      borderRight: '1px solid #f1f5f9',
      display: 'flex', flexDirection: 'column', alignItems: 'center',
      justifyContent: 'center', gap: 1,
      padding: '8px 4px', cursor: hover && !isCancelled && !isFuture ? 'pointer' : 'default',
    }}
      onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      title={lesson.full + (lesson.topic ? ` — ${lesson.topic}` : '')}>
      {topAccent && (
        <div style={{
          position: 'absolute', top: 0, left: 0, right: 0, height: 2,
          background: topAccent,
        }} />
      )}
      <div style={{
        fontSize: 16, fontWeight: 700, color: isFuture ? '#cbd5e1' : '#0f172a',
        lineHeight: 1, fontVariantNumeric: 'tabular-nums',
        letterSpacing: '-0.02em',
      }}>{day}</div>
      <div style={{
        fontSize: 10, color: isFuture ? '#cbd5e1' : '#94a3b8', fontWeight: 600,
        letterSpacing: '0.05em', textTransform: 'uppercase',
      }}>{wd.slice(0, 2)}</div>
      {isToday && (
        <div style={{
          fontSize: 9, fontWeight: 700, color: '#4338ca', marginTop: 1,
          letterSpacing: '0.05em', textTransform: 'uppercase',
        }}>сегодня</div>
      )}
      {isCancelled && (
        <div style={{
          fontSize: 9, fontWeight: 700, color: '#64748b', marginTop: 1,
          letterSpacing: '0.05em', textTransform: 'uppercase',
        }}>отмена</div>
      )}
      {hover && !isCancelled && !isFuture && (
        <button type="button" onClick={(e) => { e.stopPropagation(); onMarkAll(); }}
          style={{
            position: 'absolute', bottom: 4, left: '50%', transform: 'translateX(-50%)',
            width: colW - 16, height: 18, fontSize: 9, fontWeight: 600,
            border: 'none', borderRadius: 6, cursor: 'pointer',
            background: '#10b981', color: '#fff', fontFamily: 'inherit',
            display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 3,
            boxShadow: '0 2px 6px rgba(16,185,129,0.35)',
          }}
          title="Отметить всех присутствующими">
          <Icon.Check size={9} sw={3} /> Все
        </button>
      )}
    </div>
  );
}

// ── Student name cell (frozen left) ────────────────────────────────────
function StudentNameCell({ student, sum, height, showPct, onOpen, borderBottom }) {
  const [hover, setHover] = useStateA(false);
  const rate = sum.rate;
  const trouble = sum.effective > 0 && rate < 75;
  return (
    <div style={{
      ...hCellL(280, hover ? '#f8fafc' : '#fff', '#e2e8f0'),
      height, display: 'flex', alignItems: 'center', gap: 10,
      padding: '0 14px', cursor: 'pointer', borderBottom,
      transition: 'background .12s',
    }}
      onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      onClick={onOpen}>
      <Avatar name={student.name} size={28} />
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 13, fontWeight: 500, color: '#0f172a',
          overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
          {student.name}
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 1 }}>
          {student.parent ? (
            <span style={{ fontSize: 10.5, color: '#94a3b8' }}>{student.parent}</span>
          ) : (
            <span style={{ fontSize: 10.5, color: '#94a3b8' }}>{student.age} лет</span>
          )}
        </div>
      </div>
      {showPct && sum.effective > 0 && (
        <span style={{
          fontSize: 11.5, fontWeight: 700,
          padding: '3px 7px', borderRadius: 6,
          background: trouble ? 'rgba(239,68,68,0.10)' : '#f1f5f9',
          color: trouble ? '#b91c1c' : '#475569',
          fontVariantNumeric: 'tabular-nums',
        }}>{rate}%</span>
      )}
    </div>
  );
}

// ── Student summary cell (rightmost) ────────────────────────────────────
function StudentRateCell({ sum, height, showPct, borderBottom }) {
  if (sum.effective === 0) {
    return (
      <div style={{
        height, borderBottom, background: '#fafbfc',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        fontSize: 12, color: '#94a3b8',
      }}>—</div>
    );
  }
  const rate = sum.rate;
  const tone = rate >= 90 ? { fg: '#047857', bar: '#10b981', bg: 'rgba(16,185,129,0.08)' }
            : rate >= 75 ? { fg: '#0f172a', bar: '#6366f1', bg: '#fafbfc' }
            : rate >= 60 ? { fg: '#92400e', bar: '#f59e0b', bg: 'rgba(245,158,11,0.06)' }
                         : { fg: '#b91c1c', bar: '#ef4444', bg: 'rgba(239,68,68,0.06)' };
  return (
    <div style={{
      height, background: tone.bg, borderBottom,
      display: 'flex', flexDirection: 'column', justifyContent: 'center',
      padding: '0 14px', gap: 4,
    }}>
      <div style={{ display: 'flex', alignItems: 'baseline', gap: 6 }}>
        <span style={{
          fontSize: 14, fontWeight: 700, color: tone.fg,
          fontVariantNumeric: 'tabular-nums', letterSpacing: '-0.01em',
        }}>{rate}%</span>
        <span style={{ fontSize: 10.5, color: '#94a3b8' }}>
          {sum.attended}/{sum.effective}
        </span>
      </div>
      <div style={{ height: 4, borderRadius: 9999, background: '#e2e8f0',
        position: 'relative', overflow: 'hidden' }}>
        <div style={{
          position: 'absolute', left: 0, top: 0, bottom: 0, width: `${rate}%`,
          background: tone.bar, borderRadius: 9999,
          transition: 'width .25s ease',
        }} />
      </div>
    </div>
  );
}

// ── A single attendance cell ───────────────────────────────────────────
function Cell({ status, lesson, height, width, colorMode, note,
                onClick, onContextMenu, borderBottom }) {
  const [hover, setHover] = useStateA(false);
  const S = ATT_STATUSES[status] || ATT_STATUSES.none;
  const isCancelled = lesson.isCancelled;
  const isFuture = lesson.isFuture;
  const disabled = isCancelled || isFuture;

  let bg = S.bg;
  let fg = S.fg;
  let iconColor = S.fg;
  if (colorMode === 'vivid' && status !== 'none' && status !== 'cancelled') {
    bg = S.bgStrong;
    fg = '#fff';
    iconColor = '#fff';
  }
  if (isCancelled) {
    bg = ATT_STATUSES.cancelled.bg;
  }
  if (isFuture) {
    bg = '#fafbfc';
  }

  const renderIcon = () => {
    if (disabled) return null;
    if (status === 'none') return (
      <span style={{ fontSize: 14, color: '#cbd5e1', fontWeight: 500 }}>·</span>
    );
    const f = STATUS_ICON[status];
    return f ? React.cloneElement(f(14), { stroke: iconColor }) : null;
  };

  return (
    <div
      onClick={disabled ? undefined : onClick}
      onContextMenu={disabled ? undefined : onContextMenu}
      onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      title={disabled
        ? (isCancelled ? `Отменено: ${lesson.reason || ''}` : 'Будущее занятие')
        : `${S.label} — ${lesson.full}${note ? '\n📝 ' + note : ''}${hover ? '\n\nКлик — следующий статус\nПрав. клик — комментарий' : ''}`}
      style={{
        height, width: '100%',
        background: bg,
        borderRight: '1px solid #f1f5f9',
        borderBottom,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        cursor: disabled ? 'not-allowed' : 'pointer',
        position: 'relative',
        boxShadow: hover && !disabled
          ? `inset 0 0 0 2px ${status === 'none' ? '#cbd5e1' : S.bgStrong}` : 'none',
        transition: 'box-shadow .1s',
      }}>
      {renderIcon()}
      {note && !disabled && (
        <span style={{
          position: 'absolute', top: 3, right: 3,
          width: 5, height: 5, borderRadius: 9999, background: '#4f46e5',
        }} />
      )}
    </div>
  );
}

// ── Legend ─────────────────────────────────────────────────────────────
function Legend() {
  return (
    <div style={{
      padding: '12px 16px', borderTop: '1px solid #f1f5f9',
      display: 'flex', alignItems: 'center', gap: 18, flexWrap: 'wrap',
      fontSize: 11.5, color: '#64748b',
    }}>
      <span style={{ fontWeight: 600, letterSpacing: '0.04em',
        textTransform: 'uppercase', color: '#94a3b8' }}>Легенда</span>
      <LegendItem status="present" />
      <LegendItem status="absent" />
      <LegendItem status="late" />
      <LegendItem status="excused" />
      <LegendItem status="none" />
      <LegendItem status="cancelled" />
      <div style={{ flex: 1 }} />
      <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
        <Icon.Info size={12} stroke="#94a3b8" />
        Клик — следующий статус. Правый клик — комментарий.
      </span>
    </div>
  );
}
function LegendItem({ status }) {
  const S = ATT_STATUSES[status];
  return (
    <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
      <span style={{
        width: 20, height: 20, borderRadius: 6, background: S.bg,
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        border: status === 'none' ? '1px dashed #cbd5e1' : '0',
        color: S.fg,
      }}>{STATUS_ICON[status] ? React.cloneElement(STATUS_ICON[status](11), { stroke: S.fg })
          : <span style={{ fontSize: 11 }}>{S.short}</span>}</span>
      <span>{S.label}</span>
    </span>
  );
}

// ── Student drawer ─────────────────────────────────────────────────────
function StudentDrawer({ student, log, onClose, onNotify, onSetStatus }) {
  const sum = studentSummary(student.id, log);
  // Detect missed streak
  const streakAbsences = useMemoA(() => {
    const past = ATT_LESSONS.filter(l => !l.isFuture && !l.isCancelled).slice().reverse();
    let count = 0;
    for (const l of past) {
      const s = log[student.id]?.[l.id];
      if (s === 'absent') count++;
      else break;
    }
    return count;
  }, [log, student.id]);

  return (
    <>
      <div onClick={onClose} style={{
        position: 'fixed', inset: 0, background: 'rgba(15,23,42,0.32)',
        zIndex: 40, animation: 'fadeIn .15s ease',
      }} />
      <aside style={{
        position: 'fixed', top: 0, right: 0, bottom: 0, width: 460,
        background: '#fff', boxShadow: '-12px 0 32px rgba(15,23,42,0.18)',
        zIndex: 41, display: 'flex', flexDirection: 'column',
        animation: 'slideInRight .2s ease',
      }}>
        {/* Header */}
        <div style={{
          padding: '20px 24px 16px', borderBottom: '1px solid #f1f5f9',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 14 }}>
            <button onClick={onClose} style={{
              width: 32, height: 32, borderRadius: 8, border: '1px solid #e2e8f0',
              background: '#fff', color: '#475569', cursor: 'pointer',
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              marginLeft: 'auto',
            }}><Icon.X size={14} /></button>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
            <Avatar name={student.name} size={56} />
            <div style={{ flex: 1, minWidth: 0 }}>
              <h2 style={{ margin: 0, fontSize: 20, fontWeight: 700, letterSpacing: '-0.01em' }}>
                {student.name}
              </h2>
              <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 4,
                display: 'flex', alignItems: 'center', gap: 8 }}>
                <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
                  <Icon.Phone size={11} stroke="#94a3b8" />{student.phone}
                </span>
                {student.parent && (
                  <>
                    <span style={{ color: '#cbd5e1' }}>·</span>
                    <span>{student.parent}</span>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>

        {streakAbsences >= 2 && (
          <div style={{
            margin: '14px 24px 0', padding: '12px 14px', borderRadius: 12,
            background: 'rgba(239,68,68,0.08)', border: '1px solid rgba(239,68,68,0.2)',
            display: 'flex', alignItems: 'flex-start', gap: 10,
          }}>
            <div style={{
              width: 28, height: 28, borderRadius: 8, flexShrink: 0,
              background: 'rgba(239,68,68,0.15)', color: '#b91c1c',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}><Icon.AlertCircle size={14} sw={2.4} /></div>
            <div style={{ flex: 1, fontSize: 12.5 }}>
              <div style={{ fontWeight: 600, color: '#0f172a' }}>
                Пропустил {streakAbsences} {declensionA(streakAbsences, ['занятие','занятия','занятий'])} подряд
              </div>
              <div style={{ color: '#475569', marginTop: 2, lineHeight: 1.45 }}>
                Стоит связаться с {student.parent ? 'родителями' : 'студентом'} — выяснить причину.
              </div>
            </div>
          </div>
        )}

        {/* Stats */}
        <div style={{ padding: '16px 24px 12px',
          display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 8 }}>
          <DrawerStat value={sum.present} label="Был" tone="success" />
          <DrawerStat value={sum.absent}  label="Пропуски" tone="danger" />
          <DrawerStat value={sum.late}    label="Опозд." tone="warning" />
          <DrawerStat value={sum.excused} label="Уваж." tone="primary" />
        </div>

        {/* Big rate */}
        <div style={{ padding: '0 24px 16px' }}>
          <div style={{
            padding: 16, borderRadius: 14,
            background: 'linear-gradient(135deg, #f8fafc, #fff)',
            border: '1px solid #e2e8f0',
          }}>
            <div style={{ display: 'flex', alignItems: 'baseline', gap: 10 }}>
              <span style={{
                fontSize: 36, fontWeight: 800, letterSpacing: '-0.03em',
                color: sum.rate >= 75 ? '#047857' : '#b91c1c',
                fontVariantNumeric: 'tabular-nums',
              }}>{sum.rate}%</span>
              <span style={{ fontSize: 12.5, color: '#64748b' }}>посещаемость</span>
            </div>
            <div style={{ marginTop: 10, height: 8, borderRadius: 9999,
              background: '#e2e8f0', overflow: 'hidden' }}>
              <div style={{
                height: '100%', width: `${sum.rate}%`,
                background: sum.rate >= 75
                  ? 'linear-gradient(90deg,#10b981,#059669)'
                  : 'linear-gradient(90deg,#f59e0b,#ef4444)',
                transition: 'width .3s',
              }} />
            </div>
            <div style={{ fontSize: 11.5, color: '#94a3b8', marginTop: 8 }}>
              Зачитываем как «был на занятии»: присутствие, опоздание и пропуск по уваж. причине.
            </div>
          </div>
        </div>

        {/* Lessons timeline */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '0 24px 12px' }}>
          <div style={{
            fontSize: 11, fontWeight: 600, color: '#94a3b8',
            letterSpacing: '0.05em', textTransform: 'uppercase',
            margin: '4px 0 8px',
          }}>Динамика по занятиям</div>
          <Sparkline log={log} student={student} />
          <div style={{ display: 'flex', flexDirection: 'column', gap: 4, marginTop: 12 }}>
            {ATT_LESSONS.filter(l => !l.isFuture).slice().reverse().map(l => {
              const st = log[student.id]?.[l.id] || (l.isCancelled ? 'cancelled' : 'none');
              const S = ATT_STATUSES[st];
              const note = ATT_NOTES[`${student.id}-${l.id}`];
              return (
                <div key={l.id} style={{
                  display: 'flex', alignItems: 'center', gap: 10,
                  padding: '8px 10px', borderRadius: 10,
                  background: l.isJustHappened ? 'rgba(79,70,229,0.04)' : 'transparent',
                  border: l.isJustHappened ? '1px solid rgba(79,70,229,0.15)' : '1px solid transparent',
                }}>
                  <span style={{
                    width: 22, height: 22, borderRadius: 6, background: S.bg,
                    color: S.fg, fontSize: 10, fontWeight: 700,
                    display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                    flexShrink: 0,
                  }}>{STATUS_ICON[st] ? React.cloneElement(STATUS_ICON[st](12), { stroke: S.fg })
                       : <span>{S.short}</span>}</span>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ fontSize: 12.5, color: '#0f172a' }}>
                      <span style={{ fontFamily: 'var(--edv-font-mono)', fontWeight: 600 }}>
                        {l.date}
                      </span>
                      <span style={{ color: '#94a3b8', margin: '0 6px' }}>·</span>
                      <span style={{ color: '#475569' }}>{l.topic}</span>
                    </div>
                    {note && (
                      <div style={{ fontSize: 11.5, color: '#64748b', marginTop: 2,
                        display: 'inline-flex', alignItems: 'center', gap: 4 }}>
                        <Icon.MessageCircle size={10} stroke="#94a3b8" />{note}
                      </div>
                    )}
                  </div>
                  <span style={{ fontSize: 10.5, color: S.fg, fontWeight: 600,
                    letterSpacing: '0.03em' }}>{S.label}</span>
                </div>
              );
            })}
          </div>
        </div>

        <div style={{ padding: '14px 24px', borderTop: '1px solid #f1f5f9',
          display: 'flex', gap: 8 }}>
          <Button variant="secondary" size="md" style={{ flex: 1 }}>
            <Icon.MessageCircle size={14} sw={2} />Открыть карточку
          </Button>
          <Button size="md" style={{ flex: 1 }} onClick={() => onNotify(student)}>
            <Icon.Megaphone size={14} sw={2.2} />Уведомить
          </Button>
        </div>
      </aside>
    </>
  );
}
function DrawerStat({ value, label, tone }) {
  const tones = {
    success: { fg: '#047857', bg: 'rgba(16,185,129,0.10)' },
    danger:  { fg: '#b91c1c', bg: 'rgba(239,68,68,0.08)' },
    warning: { fg: '#92400e', bg: 'rgba(245,158,11,0.12)' },
    primary: { fg: '#4338ca', bg: 'rgba(99,102,241,0.10)' },
  };
  const t = tones[tone];
  return (
    <div style={{
      background: t.bg, borderRadius: 10, padding: '8px 10px',
      display: 'flex', flexDirection: 'column', gap: 1,
    }}>
      <span style={{ fontSize: 20, fontWeight: 700, color: t.fg,
        fontVariantNumeric: 'tabular-nums', letterSpacing: '-0.02em' }}>{value}</span>
      <span style={{ fontSize: 10.5, color: '#64748b', letterSpacing: '0.02em' }}>{label}</span>
    </div>
  );
}

// ── Mini sparkline of student's attendance ─────────────────────────────
function Sparkline({ log, student }) {
  const past = ATT_LESSONS.filter(l => !l.isFuture);
  const W = 412, H = 56, pad = 4;
  const stepX = (W - pad * 2) / Math.max(1, past.length - 1);
  return (
    <svg width="100%" height={H} viewBox={`0 0 ${W} ${H}`} style={{ display: 'block' }}>
      {past.map((l, i) => {
        const st = log[student.id]?.[l.id] || (l.isCancelled ? 'cancelled' : 'none');
        const S = ATT_STATUSES[st];
        const x = pad + i * stepX;
        const cy = H / 2;
        let dotY = cy;
        if (st === 'present')  dotY = 12;
        if (st === 'late')     dotY = 22;
        if (st === 'excused')  dotY = 30;
        if (st === 'absent')   dotY = H - 12;
        return (
          <React.Fragment key={l.id}>
            <line x1={x} y1={12} x2={x} y2={H-12} stroke="#f1f5f9" strokeWidth="1" />
            {st !== 'cancelled' && st !== 'none' && (
              <circle cx={x} cy={dotY} r="4" fill={S.bgStrong} />
            )}
            {st === 'cancelled' && (
              <text x={x} y={H/2+3} fontSize="9" textAnchor="middle" fill="#cbd5e1">×</text>
            )}
          </React.Fragment>
        );
      })}
      <line x1={pad} y1={H/2} x2={W-pad} y2={H/2} stroke="#e2e8f0" strokeDasharray="2 3" strokeWidth="1" />
    </svg>
  );
}

// ── Note dialog ────────────────────────────────────────────────────────
function NoteDialog({ ctx, log, onSave, onClose }) {
  const student = ATT_STUDENTS.find(s => s.id === ctx.studentId);
  const lesson = ATT_LESSONS.find(l => l.id === ctx.lessonId);
  const status = log[ctx.studentId]?.[ctx.lessonId] || 'none';
  const S = ATT_STATUSES[status];
  const [text, setText] = useStateA(ATT_NOTES[`${ctx.studentId}-${ctx.lessonId}`] || '');
  return (
    <>
      <div onClick={onClose} style={{
        position: 'fixed', inset: 0, background: 'rgba(15,23,42,0.32)',
        zIndex: 50, animation: 'fadeIn .15s ease',
      }} />
      <div style={{
        position: 'fixed', top: '50%', left: '50%', transform: 'translate(-50%, -50%)',
        width: 440, background: '#fff', borderRadius: 16, zIndex: 51,
        boxShadow: '0 24px 56px rgba(15,23,42,0.25)', overflow: 'hidden',
      }}>
        <div style={{ padding: '18px 20px 12px', borderBottom: '1px solid #f1f5f9' }}>
          <div style={{ fontSize: 11, fontWeight: 600, color: '#94a3b8',
            letterSpacing: '0.05em', textTransform: 'uppercase', marginBottom: 6 }}>
            Комментарий к отметке
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
            <span style={{ width: 28, height: 28, borderRadius: 8, background: S.bg,
              color: S.fg, fontSize: 12, fontWeight: 700,
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center' }}>
              {STATUS_ICON[status] ? React.cloneElement(STATUS_ICON[status](14), { stroke: S.fg })
                : <span>{S.short}</span>}
            </span>
            <div>
              <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{student.name}</div>
              <div style={{ fontSize: 12, color: '#64748b' }}>
                {lesson.full} · {S.label}
              </div>
            </div>
          </div>
        </div>
        <div style={{ padding: 20 }}>
          <textarea value={text} onChange={e => setText(e.target.value)}
            autoFocus rows={4} placeholder="Например: справка от врача, болел весь день…"
            style={{
              width: '100%', padding: '10px 12px', borderRadius: 10,
              border: '1px solid #e2e8f0', fontSize: 13.5, fontFamily: 'inherit',
              outline: 'none', resize: 'vertical', color: '#0f172a',
            }}
            onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.2)'; }}
            onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }} />
        </div>
        <div style={{ padding: '12px 20px', borderTop: '1px solid #f1f5f9',
          display: 'flex', justifyContent: 'flex-end', gap: 8, background: '#fafbfc' }}>
          <Button variant="ghost" onClick={onClose}>Отмена</Button>
          <Button onClick={() => onSave(text.trim())}>Сохранить</Button>
        </div>
      </div>
    </>
  );
}

// ── Tweaks ─────────────────────────────────────────────────────────────
function AttTweaks({ t, setTweak }) {
  return (
    <TweaksPanel title="Tweaks · Посещаемость">
      <TweakSection label="Вид">
        <TweakRadio label="Плотность" value={t.density}
          options={[
            { value: 'compact', label: 'Компакт' },
            { value: 'regular', label: 'Обычно' },
          ]}
          onChange={v => setTweak('density', v)} />
        <TweakRadio label="Цвет статусов" value={t.colorMode}
          options={[
            { value: 'soft',  label: 'Мягкий' },
            { value: 'vivid', label: 'Контраст' },
          ]}
          onChange={v => setTweak('colorMode', v)} />
        <TweakToggle label="Показывать %"
          value={!!t.showPercents}
          onChange={v => setTweak('showPercents', v)} />
      </TweakSection>
      <TweakSection label="Период">
        <TweakSelect label="Колонки" value={t.period}
          options={[
            { value: 'all',    label: 'Весь период' },
            { value: 'apr',    label: 'Только апрель' },
            { value: 'may',    label: 'Только май' },
            { value: 'future', label: 'Только будущие' },
          ]}
          onChange={v => setTweak('period', v)} />
      </TweakSection>
    </TweaksPanel>
  );
}

window.AttendanceApp = AttendanceApp;
