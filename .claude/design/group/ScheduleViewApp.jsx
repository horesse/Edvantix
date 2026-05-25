// Schedule view — Расписание tab on the group dashboard. Browse all lessons.
const { useState: useStateSV, useMemo: useMemoSV } = React;

// ── Helpers ──────────────────────────────────────────────────────────
function svParse(d) { return new Date(d + 'T00:00:00'); }
function svKey(d) {
  const y = d.getFullYear();
  const m = String(d.getMonth()+1).padStart(2,'0');
  const dd = String(d.getDate()).padStart(2,'0');
  return `${y}-${m}-${dd}`;
}
function svDateRu(key) {
  const [y,m,d] = key.split('-');
  return `${d}.${m}.${y}`;
}
function svDayMonth(key) {
  const [y,m,d] = key.split('-');
  return `${parseInt(d,10)} ${SCHEDULE_RU_MONTHS[parseInt(m,10)-1].toLowerCase()}`;
}
function svPlural(n, one, few, many) {
  const m10 = n%10, m100 = n%100;
  if (m10===1 && m100!==11) return one;
  if (m10>=2 && m10<=4 && (m100<10 || m100>=20)) return few;
  return many;
}
function svDaysBetween(aKey, bKey) {
  return Math.round((svParse(bKey) - svParse(aKey)) / 86400000);
}

// ── Status palette ───────────────────────────────────────────────────
const STATUS_TONE = {
  done:      { bg:'rgba(16,185,129,0.12)', fg:'#047857', dot:'#10b981', label:'Проведено',     border:'rgba(16,185,129,0.30)' },
  today:     { bg:'rgba(79,70,229,0.10)',  fg:'#4338ca', dot:'#4f46e5', label:'Сегодня',       border:'rgba(79,70,229,0.35)' },
  upcoming:  { bg:'#fff',                  fg:'#4338ca', dot:'#6366f1', label:'Запланировано', border:'rgba(99,102,241,0.30)' },
  cancelled: { bg:'rgba(245,158,11,0.10)', fg:'#92400e', dot:'#f59e0b', label:'Отменено',      border:'rgba(245,158,11,0.30)' },
};

// ── App root ─────────────────────────────────────────────────────────
function ScheduleViewApp() {
  const g = GRP;
  const lessons = SCHEDULE_LESSONS;
  const stats = SCHEDULE_STATS;

  // Default selection: today's lesson if exists, else next upcoming
  const initialSelectedId = (lessons.find(l => l.status==='today')
    || SCHEDULE_NEXT || lessons[lessons.length-1]).id;

  const [view, setView] = useStateSV('month');   // 'month' | 'list'
  const [monthKey, setMonthKey] = useStateSV(TODAY_GS_VIEW.slice(0,7));
  const [selectedId, setSelectedId] = useStateSV(initialSelectedId);
  const [statusFilter, setStatusFilter] = useStateSV('all');   // 'all'|'done'|'upcoming'|'cancelled'

  const selectedLesson = lessons.find(l => l.id === selectedId);

  const filteredLessons = useMemoSV(() => {
    if (statusFilter === 'all') return lessons;
    if (statusFilter === 'upcoming')
      return lessons.filter(l => l.status==='upcoming' || l.status==='today');
    return lessons.filter(l => l.status === statusFilter);
  }, [statusFilter]);

  return (
    <div style={{ display:'flex', height:'100vh', minHeight:700,
      background:'#f8fafc', overflow:'hidden' }}>
      <Sidebar active="groups" />
      <div style={{ flex:1, display:'flex', flexDirection:'column', minWidth:0 }}>
        <SvBreadcrumb group={g} />
        <SvHeader group={g} />
        <GdTabs active="schedule" />

        <div style={{ flex:1, overflowY:'auto', padding:'20px 32px 40px',
          background:'#f8fafc' }}>
          <div style={{ maxWidth:1320, margin:'0 auto', display:'flex',
            flexDirection:'column', gap:18 }}>

            {/* PATTERN STRIP */}
            <PatternStrip />

            {/* COUNTERS */}
            <CountersRow stats={stats} />

            {/* VIEW TOGGLE + MONTH NAV */}
            <ViewToolbar
              view={view} onView={setView}
              monthKey={monthKey} onMonth={setMonthKey}
              months={stats.months}
              onJumpToday={() => { setMonthKey(TODAY_GS_VIEW.slice(0,7));
                setSelectedId(initialSelectedId); }}
              statusFilter={statusFilter} onStatusFilter={setStatusFilter}
            />

            {/* MAIN + DETAIL */}
            <div style={{ display:'grid', gap:18,
              gridTemplateColumns:'minmax(0, 1fr) 360px', alignItems:'start' }}>
              <div style={{ minWidth:0 }}>
                {view === 'month'
                  ? <MonthGrid monthKey={monthKey} lessons={filteredLessons}
                      selectedId={selectedId} onSelect={setSelectedId} />
                  : <ListView lessons={filteredLessons}
                      selectedId={selectedId} onSelect={setSelectedId} />
                }
              </div>
              <div style={{ position:'sticky', top:0, alignSelf:'start',
                display:'flex', flexDirection:'column', gap:14 }}>
                <LessonDetail lesson={selectedLesson} group={g} />
                <LegendCard />
              </div>
            </div>

            {/* UPCOMING STRIP */}
            <UpcomingStrip lessons={lessons} onSelect={(id) => {
              setSelectedId(id);
              const l = lessons.find(x => x.id===id);
              if (l) setMonthKey(l.date.slice(0,7));
            }} />
          </div>
        </div>
      </div>
    </div>
  );
}

// ── Breadcrumb & Header (mirror Group.html but with Schedule context) ────
function SvBreadcrumb({ group }) {
  return (
    <div style={{
      padding:'13px 32px', borderBottom:'1px solid #e2e8f0',
      background:'#fff', display:'flex', alignItems:'center', gap:8,
      fontSize:13, color:'#64748b',
    }}>
      <span>Школа</span>
      <Icon.ChevronRight size={13} stroke="#cbd5e1" />
      <a href="Groups.html" style={{ color:'#475569' }}>Группы</a>
      <Icon.ChevronRight size={13} stroke="#cbd5e1" />
      <a href="Group.html" style={{ color:'#475569' }}>{group.code}</a>
      <Icon.ChevronRight size={13} stroke="#cbd5e1" />
      <span style={{ color:'#0f172a', fontWeight:500 }}>Расписание</span>
    </div>
  );
}

function SvHeader({ group }) {
  const FmtIc = Icon.School;
  return (
    <div style={{
      padding:'18px 32px 16px', background:'#fff',
      display:'flex', alignItems:'flex-start', gap:24,
    }}>
      <div style={{ flex:1, minWidth:0, display:'flex', flexDirection:'column', gap:8 }}>
        <div style={{ display:'flex', alignItems:'center', gap:8, flexWrap:'wrap' }}>
          <GdStatusPill status={group.status} />
          <GdLevelChip level={group.level} full />
          <span style={{
            display:'inline-flex', alignItems:'center', gap:6,
            padding:'3px 10px', borderRadius:9999, background:'#f1f5f9', color:'#475569',
            fontSize:11.5, fontWeight:500,
          }}>
            <FmtIc size={12} stroke="#64748b"/>{group.formatLabel} · {group.room}
          </span>
          <span style={{ fontSize:12, color:'#94a3b8',
            fontFamily:'var(--edv-font-mono, ui-monospace)', fontWeight:600 }}>
            {group.code}
          </span>
        </div>
        <div>
          <h1 style={{ margin:0, fontSize:22, fontWeight:700,
            letterSpacing:'-0.02em', lineHeight:1.2 }}>{group.name}</h1>
        </div>
      </div>
      <div style={{ display:'flex', alignItems:'center', gap:8, flexShrink:0 }}>
        <a href="Group Schedule Setup.html" style={{ textDecoration:'none' }}>
          <Button variant="secondary" size="md">
            <Icon.Settings size={14}/>Шаблон расписания
          </Button>
        </a>
        <Button variant="secondary" size="md" style={{
          padding:0, width:36, height:36, borderRadius:8 }} title="Экспорт в .ics">
          <Icon.CalendarDays size={15}/>
        </Button>
        <a href="Lesson Create.html" style={{ textDecoration:'none' }}>
          <Button size="md"><Icon.Plus size={15}/>Добавить урок</Button>
        </a>
      </div>
    </div>
  );
}

// ── Pattern strip ────────────────────────────────────────────────────
function PatternStrip() {
  const p = SCHEDULE_PATTERN;
  const startKey = p.starts.replace(/-/g, '.');
  const fmtRu = (k) => {
    const [y,m,d] = k.split('-');
    return `${d}.${m}.${y}`;
  };
  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      padding:'14px 18px',
      display:'flex', alignItems:'center', gap:18, flexWrap:'wrap',
    }}>
      <div style={{ display:'flex', alignItems:'center', gap:12 }}>
        <div style={{
          width:38, height:38, borderRadius:10,
          background:'rgba(79,70,229,0.10)', color:'#4338ca',
          display:'flex', alignItems:'center', justifyContent:'center',
        }}>
          <Icon.CalendarDays size={18} />
        </div>
        <div>
          <div style={{ fontSize:11, fontWeight:600, color:'#94a3b8',
            letterSpacing:'0.08em', textTransform:'uppercase' }}>
            Шаблон расписания
          </div>
          <div style={{ display:'flex', alignItems:'baseline', gap:10, marginTop:3 }}>
            <span style={{ fontSize:15, fontWeight:600, color:'#0f172a' }}>
              Пн / Ср · 18:00 – 19:30
            </span>
            <span style={{ fontSize:12, color:'#64748b' }}>
              · 90 мин · {p.room}
            </span>
          </div>
        </div>
      </div>

      <div style={{ width:1, height:38, background:'#e2e8f0' }}/>

      <div>
        <div style={{ fontSize:11, fontWeight:600, color:'#94a3b8',
          letterSpacing:'0.08em', textTransform:'uppercase' }}>
          Период
        </div>
        <div style={{ display:'flex', alignItems:'center', gap:6, marginTop:3,
          fontSize:13.5, color:'#0f172a', fontVariantNumeric:'tabular-nums' }}>
          <span style={{ fontFamily:'var(--edv-font-mono, ui-monospace)' }}>
            {fmtRu(p.starts)}
          </span>
          <Icon.ArrowRight size={13} stroke="#94a3b8"/>
          <span style={{ fontFamily:'var(--edv-font-mono, ui-monospace)' }}>
            {fmtRu(p.ends)}
          </span>
        </div>
      </div>

      <div style={{ width:1, height:38, background:'#e2e8f0' }}/>

      <div style={{ display:'flex', alignItems:'center', gap:8 }}>
        <div style={{
          width:32, height:32, borderRadius:8,
          background:'rgba(16,185,129,0.12)', color:'#047857',
          display:'flex', alignItems:'center', justifyContent:'center',
        }}>
          <Icon.Bell size={15}/>
        </div>
        <div style={{ fontSize:12.5, color:'#475569' }}>
          Напоминания студентам<br/>
          <span style={{ fontSize:11.5, color:'#94a3b8' }}>
            за 24 ч и за 30 мин до занятия
          </span>
        </div>
      </div>

      <div style={{ flex:1 }}/>

      <a href="Group Schedule Setup.html" style={{
        fontSize:12.5, color:'#4f46e5', fontWeight:500,
        display:'inline-flex', alignItems:'center', gap:5,
      }}>
        Изменить шаблон <Icon.ArrowRight size={13}/>
      </a>
    </div>
  );
}

// ── Counters row ─────────────────────────────────────────────────────
function CountersRow({ stats }) {
  const donePct = Math.round(stats.done / stats.heldTotal * 100);
  return (
    <div style={{ display:'grid', gap:14,
      gridTemplateColumns:'minmax(0, 1fr) 240px 240px 240px' }}>
      {/* Progress card */}
      <div style={{
        background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
        padding:'14px 18px',
      }}>
        <div style={{ display:'flex', alignItems:'baseline',
          justifyContent:'space-between', gap:8 }}>
          <div style={{ display:'flex', alignItems:'baseline', gap:6 }}>
            <span style={{ fontSize:28, fontWeight:700, color:'#0f172a',
              letterSpacing:'-0.02em', fontVariantNumeric:'tabular-nums', lineHeight:1 }}>
              {stats.done}
            </span>
            <span style={{ fontSize:18, color:'#94a3b8', fontWeight:500,
              fontVariantNumeric:'tabular-nums' }}>
              / {stats.heldTotal}
            </span>
            <span style={{ fontSize:13, color:'#64748b', marginLeft:8 }}>
              занятий проведено
            </span>
          </div>
          <span style={{ fontSize:13, fontWeight:600, color:'#047857',
            fontVariantNumeric:'tabular-nums' }}>
            {donePct}%
          </span>
        </div>
        <div style={{ marginTop:12, height:8, borderRadius:9999,
          background:'#f1f5f9', overflow:'hidden', display:'flex' }}>
          <div style={{ width:`${donePct}%`, height:'100%',
            background:'linear-gradient(90deg, #10b981, #14b8a6)' }}/>
        </div>
        <div style={{ display:'flex', justifyContent:'space-between',
          fontSize:11.5, color:'#94a3b8', marginTop:6,
          fontVariantNumeric:'tabular-nums' }}>
          <span>{stats.hoursDone.toFixed(1)} ч учебной нагрузки</span>
          <span>осталось {(stats.hoursTotal - stats.hoursDone).toFixed(1)} ч</span>
        </div>
      </div>

      <CountChip tone="upcoming" icon="Clock" value={stats.upcoming}
        label="Запланировано" sub="включая сегодня" />
      <CountChip tone="cancelled" icon="X" value={stats.cancelled}
        label="Отменено" sub="праздники и каникулы" />
      <CountChip tone="total" icon="CalendarDays" value={stats.total}
        label="Всего занятий" sub="за весь курс" />
    </div>
  );
}

function CountChip({ tone, icon, value, label, sub }) {
  const palettes = {
    upcoming:  { bg:'rgba(79,70,229,0.10)',  fg:'#4338ca' },
    cancelled: { bg:'rgba(245,158,11,0.14)', fg:'#92400e' },
    total:     { bg:'#f1f5f9',               fg:'#475569' },
  };
  const c = palettes[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      padding:'14px 16px', display:'flex', gap:14, alignItems:'center',
    }}>
      <div style={{
        width:42, height:42, borderRadius:11,
        background:c.bg, color:c.fg,
        display:'flex', alignItems:'center', justifyContent:'center',
        flexShrink:0,
      }}>
        <Ic size={19}/>
      </div>
      <div style={{ minWidth:0 }}>
        <div style={{ fontSize:22, fontWeight:700, color:'#0f172a',
          letterSpacing:'-0.02em', lineHeight:1.1,
          fontVariantNumeric:'tabular-nums' }}>
          {value}
        </div>
        <div style={{ fontSize:12.5, color:'#0f172a', fontWeight:500, marginTop:2 }}>
          {label}
        </div>
        <div style={{ fontSize:11, color:'#94a3b8', marginTop:1 }}>{sub}</div>
      </div>
    </div>
  );
}

// ── View toolbar (toggle + month nav + filter) ───────────────────────
function ViewToolbar({ view, onView, monthKey, onMonth, months,
                       onJumpToday, statusFilter, onStatusFilter }) {
  const idx = months.findIndex(m => m.key === monthKey);
  const prev = idx > 0 ? months[idx-1].key : null;
  const next = idx < months.length-1 ? months[idx+1].key : null;
  const cur = months[idx];
  const [y, m] = monthKey.split('-');
  const monthLabel = `${SCHEDULE_RU_MONTHS_FULL[parseInt(m,10)-1]} ${y}`;

  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      padding:'10px 12px',
      display:'flex', alignItems:'center', gap:14, flexWrap:'wrap',
    }}>
      {/* View toggle */}
      <div style={{ display:'inline-flex', padding:3, background:'#f1f5f9',
        borderRadius:10 }}>
        <ToolbarToggle active={view==='month'} onClick={() => onView('month')}
          icon="CalendarDays" label="Календарь" />
        <ToolbarToggle active={view==='list'} onClick={() => onView('list')}
          icon="LayoutDashboard" label="Список" />
      </div>

      {/* Month nav (only when month view) */}
      {view === 'month' && (
        <div style={{ display:'flex', alignItems:'center', gap:4 }}>
          <button onClick={() => prev && onMonth(prev)} disabled={!prev}
            style={navBtnStyle(!prev)}>
            <Icon.ArrowLeft size={14}/>
          </button>
          <div style={{
            padding:'7px 12px', borderRadius:8, background:'#f8fafc',
            border:'1px solid #e2e8f0', minWidth:170, textAlign:'center',
            fontSize:13, fontWeight:600, color:'#0f172a',
          }}>{monthLabel}</div>
          <button onClick={() => next && onMonth(next)} disabled={!next}
            style={navBtnStyle(!next)}>
            <Icon.ArrowRight size={14}/>
          </button>
          {cur && (
            <span style={{ marginLeft:10, fontSize:12, color:'#64748b' }}>
              {cur.count} {svPlural(cur.count,'занятие','занятия','занятий')} в месяце
              {cur.cancelled > 0 && <>, <span style={{ color:'#92400e' }}>
                {cur.cancelled} {svPlural(cur.cancelled,'отменено','отменено','отменено')}
              </span></>}
            </span>
          )}
        </div>
      )}

      <div style={{ flex:1 }}/>

      <button onClick={onJumpToday} style={{
        display:'inline-flex', alignItems:'center', gap:6,
        padding:'7px 12px', borderRadius:8, border:'1px solid #4f46e5',
        background:'rgba(79,70,229,0.05)', color:'#4338ca', cursor:'pointer',
        fontSize:12.5, fontWeight:600, fontFamily:'inherit',
      }}>
        <Icon.Sparkles size={13}/>Сегодня · 14 мая
      </button>

      {/* Filter (only on list view) */}
      {view === 'list' && (
        <div style={{ display:'inline-flex', gap:4 }}>
          {[
            { id:'all',       label:'Все' },
            { id:'upcoming',  label:'Запланированы' },
            { id:'done',      label:'Проведены' },
            { id:'cancelled', label:'Отменены' },
          ].map(f => (
            <button key={f.id} onClick={() => onStatusFilter(f.id)}
              style={{
                padding:'6px 10px', borderRadius:8, border:'1px solid #e2e8f0',
                background: statusFilter===f.id ? 'rgba(79,70,229,0.08)' : '#fff',
                color: statusFilter===f.id ? '#4338ca' : '#475569',
                fontSize:12, fontWeight:500, cursor:'pointer',
                fontFamily:'inherit',
              }}>
              {f.label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

function ToolbarToggle({ active, onClick, icon, label }) {
  const Ic = Icon[icon];
  return (
    <button onClick={onClick} style={{
      display:'inline-flex', alignItems:'center', gap:6,
      padding:'7px 12px', borderRadius:7,
      background: active ? '#fff' : 'transparent',
      color: active ? '#0f172a' : '#64748b',
      border:'none', cursor:'pointer', fontFamily:'inherit',
      fontSize:12.5, fontWeight:600,
      boxShadow: active ? '0 1px 3px rgba(15,23,42,0.08)' : 'none',
    }}>
      <Ic size={13}/>
      {label}
    </button>
  );
}

function navBtnStyle(disabled) {
  return {
    width:32, height:32, borderRadius:8,
    border:'1px solid #e2e8f0', background:'#fff',
    color: disabled ? '#cbd5e1' : '#475569',
    cursor: disabled ? 'not-allowed' : 'pointer',
    display:'inline-flex', alignItems:'center', justifyContent:'center',
    opacity: disabled ? 0.5 : 1,
  };
}

// ── Month grid ───────────────────────────────────────────────────────
function MonthGrid({ monthKey, lessons, selectedId, onSelect }) {
  const [y, m] = monthKey.split('-').map(n => parseInt(n,10));
  const firstOfMonth = new Date(y, m-1, 1);
  // Russian week starts Mon — offset such that Mon=0
  const dowFirst = (firstOfMonth.getDay() + 6) % 7;   // 0=Mon..6=Sun
  const daysInMonth = new Date(y, m, 0).getDate();
  const totalCells = Math.ceil((dowFirst + daysInMonth) / 7) * 7;

  // Build cells: each cell = { key, day, inMonth, isWeekend, lessons:[] }
  const lessonsByDate = {};
  lessons.forEach(l => {
    if (!lessonsByDate[l.date]) lessonsByDate[l.date] = [];
    lessonsByDate[l.date].push(l);
  });
  const cells = [];
  for (let i = 0; i < totalCells; i++) {
    const dayNum = i - dowFirst + 1;
    const inMonth = dayNum >= 1 && dayNum <= daysInMonth;
    let date;
    if (inMonth) date = new Date(y, m-1, dayNum);
    else if (dayNum < 1) date = new Date(y, m-1, dayNum);
    else date = new Date(y, m-1, dayNum);
    const key = svKey(date);
    const dow = (date.getDay() + 6) % 7;
    cells.push({
      key,
      day: date.getDate(),
      inMonth,
      isWeekend: dow === 5 || dow === 6,
      isToday: key === TODAY_GS_VIEW,
      lessons: lessonsByDate[key] || [],
    });
  }

  const weekdayHeaders = ['Пн','Вт','Ср','Чт','Пт','Сб','Вс'];

  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      overflow:'hidden',
    }}>
      {/* Headers */}
      <div style={{
        display:'grid', gridTemplateColumns:'repeat(7, 1fr)',
        borderBottom:'1px solid #e2e8f0', background:'#fafbfc',
      }}>
        {weekdayHeaders.map((d, i) => (
          <div key={d} style={{
            padding:'10px 12px', fontSize:11, fontWeight:600,
            letterSpacing:'0.08em', textTransform:'uppercase',
            color: i >= 5 ? '#b91c1c' : '#94a3b8',
            textAlign:'left',
            borderRight: i < 6 ? '1px solid #f1f5f9' : 'none',
          }}>{d}</div>
        ))}
      </div>
      {/* Grid */}
      <div style={{
        display:'grid', gridTemplateColumns:'repeat(7, 1fr)',
        gridAutoRows:'minmax(112px, auto)',
      }}>
        {cells.map((c, i) => (
          <MonthCell key={c.key + ':' + i} cell={c} cellIndex={i} totalCells={cells.length}
            selectedId={selectedId} onSelect={onSelect} />
        ))}
      </div>
    </div>
  );
}

function MonthCell({ cell, cellIndex, totalCells, selectedId, onSelect }) {
  const col = cellIndex % 7;
  const isLastRow = cellIndex >= totalCells - 7;
  return (
    <div style={{
      position:'relative', padding:8,
      background: !cell.inMonth ? '#fafbfc'
                : cell.isToday ? 'rgba(79,70,229,0.045)'
                : cell.isWeekend ? '#fafbfc'
                : '#fff',
      borderRight: col < 6 ? '1px solid #f1f5f9' : 'none',
      borderBottom: !isLastRow ? '1px solid #f1f5f9' : 'none',
      display:'flex', flexDirection:'column', gap:6,
      minHeight:112,
    }}>
      <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between' }}>
        <span style={{
          display:'inline-flex', alignItems:'center', justifyContent:'center',
          minWidth:22, height:22, borderRadius:9999,
          fontSize:12, fontWeight: cell.isToday ? 700 : 500,
          fontVariantNumeric:'tabular-nums',
          background: cell.isToday ? '#4f46e5' : 'transparent',
          color: cell.isToday ? '#fff'
              : !cell.inMonth ? '#cbd5e1'
              : (col >= 5) ? '#b91c1c'
              : '#475569',
          padding: cell.isToday ? '0 7px' : 0,
        }}>{cell.day}</span>
        {cell.isToday && (
          <span style={{ fontSize:10, fontWeight:700, color:'#4338ca',
            letterSpacing:'0.06em', textTransform:'uppercase' }}>сегодня</span>
        )}
      </div>
      {cell.inMonth && cell.lessons.map(l => (
        <LessonChip key={l.id} lesson={l}
          selected={l.id === selectedId} onClick={() => onSelect(l.id)} />
      ))}
    </div>
  );
}

function LessonChip({ lesson, selected, onClick }) {
  const t = STATUS_TONE[lesson.status];
  const isCancelled = lesson.status === 'cancelled';
  return (
    <button onClick={onClick} style={{
      textAlign:'left', cursor:'pointer', fontFamily:'inherit',
      padding:'6px 8px', borderRadius:8,
      border:`1px solid ${selected ? '#4f46e5' : t.border}`,
      background: selected ? 'rgba(79,70,229,0.08)' : t.bg,
      boxShadow: selected ? '0 0 0 3px rgba(79,70,229,0.18)' : 'none',
      display:'flex', flexDirection:'column', gap:3,
      transition:'all .12s',
    }}>
      <div style={{ display:'flex', alignItems:'center', gap:6 }}>
        <span style={{ width:6, height:6, borderRadius:9999, background:t.dot,
          flexShrink:0 }}/>
        <span style={{
          fontSize:11, fontWeight:700, fontFamily:'var(--edv-font-mono, ui-monospace)',
          fontVariantNumeric:'tabular-nums', color:t.fg, letterSpacing:'-0.01em',
        }}>{lesson.startTime}</span>
        {lesson.unit && (
          <span style={{ fontSize:10, fontWeight:600, color:'#94a3b8',
            marginLeft:'auto' }}>U{lesson.unit}</span>
        )}
        {lesson.isKey && (
          <span title="Ключевое занятие"><Icon.Sparkles size={10} stroke="#92400e"/></span>
        )}
      </div>
      <div style={{
        fontSize:11.5, color: isCancelled ? '#92400e' : '#0f172a',
        lineHeight:1.3, fontWeight:500,
        textDecoration: isCancelled ? 'line-through' : 'none',
        overflow:'hidden', display:'-webkit-box', WebkitBoxOrient:'vertical',
        WebkitLineClamp:2,
      }}>{lesson.topic}</div>
    </button>
  );
}

// ── List view ────────────────────────────────────────────────────────
function ListView({ lessons, selectedId, onSelect }) {
  // Group by month
  const byMonth = {};
  lessons.forEach(l => {
    const k = l.date.slice(0,7);
    if (!byMonth[k]) byMonth[k] = [];
    byMonth[k].push(l);
  });
  const monthKeys = Object.keys(byMonth).sort();

  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      overflow:'hidden',
    }}>
      {monthKeys.map(mk => {
        const [y, m] = mk.split('-');
        const monthName = SCHEDULE_RU_MONTHS_FULL[parseInt(m,10)-1];
        const isCurrentMonth = mk === TODAY_GS_VIEW.slice(0,7);
        const items = byMonth[mk];
        const done = items.filter(l => l.status==='done').length;
        return (
          <div key={mk}>
            <div style={{
              position:'sticky', top:0, zIndex:1,
              padding:'10px 18px', background:'#f8fafc',
              borderBottom:'1px solid #e2e8f0',
              display:'flex', alignItems:'baseline', gap:10,
            }}>
              <span style={{ fontSize:13, fontWeight:700, color:'#0f172a',
                letterSpacing:'-0.01em' }}>
                {monthName} {y}
              </span>
              <span style={{ fontSize:11.5, color:'#64748b',
                fontVariantNumeric:'tabular-nums' }}>
                {items.length} {svPlural(items.length,'занятие','занятия','занятий')}
                {done > 0 && ` · ${done} проведено`}
              </span>
              {isCurrentMonth && (
                <span style={{ marginLeft:'auto', fontSize:10.5, fontWeight:700,
                  padding:'2px 8px', borderRadius:9999, color:'#4338ca',
                  background:'rgba(79,70,229,0.10)', letterSpacing:'0.06em',
                  textTransform:'uppercase' }}>текущий</span>
              )}
            </div>
            {items.map(l => (
              <ListRow key={l.id} lesson={l}
                selected={l.id === selectedId} onClick={() => onSelect(l.id)} />
            ))}
          </div>
        );
      })}
    </div>
  );
}

function ListRow({ lesson, selected, onClick }) {
  const t = STATUS_TONE[lesson.status];
  const date = svParse(lesson.date);
  const dow = SCHEDULE_RU_WEEKDAYS_SHORT[date.getDay()];
  const isCancelled = lesson.status === 'cancelled';
  const att = lesson.attendance;
  const total = GRP.students;
  return (
    <button onClick={onClick} style={{
      width:'100%', textAlign:'left', cursor:'pointer', fontFamily:'inherit',
      display:'grid', gridTemplateColumns:'68px 1fr 110px 140px 90px 24px',
      alignItems:'center', gap:14,
      padding:'12px 18px', border:'none',
      background: selected ? 'rgba(79,70,229,0.04)' : '#fff',
      borderBottom:'1px solid #f1f5f9',
      borderLeft: selected ? '3px solid #4f46e5' : '3px solid transparent',
      paddingLeft: selected ? 15 : 18,
      transition:'background .1s',
    }}
      onMouseEnter={e => { if (!selected) e.currentTarget.style.background = '#fafbfc'; }}
      onMouseLeave={e => { if (!selected) e.currentTarget.style.background = '#fff'; }}>
      {/* Date */}
      <div style={{ display:'flex', flexDirection:'column', alignItems:'center',
        justifyContent:'center', flexShrink:0 }}>
        <span style={{
          fontSize:22, fontWeight:700, color: isCancelled ? '#94a3b8' : '#0f172a',
          fontVariantNumeric:'tabular-nums', lineHeight:1, letterSpacing:'-0.02em',
        }}>{date.getDate()}</span>
        <span style={{ fontSize:10, color:'#94a3b8', marginTop:3,
          textTransform:'uppercase', letterSpacing:'0.08em', fontWeight:600 }}>
          {dow}
        </span>
      </div>
      {/* Topic */}
      <div style={{ minWidth:0 }}>
        <div style={{ display:'flex', alignItems:'center', gap:8 }}>
          <span style={{ width:8, height:8, borderRadius:9999, background:t.dot }}/>
          <span style={{ fontSize:13.5, fontWeight: isCancelled ? 500 : 600,
            color: isCancelled ? '#92400e' : '#0f172a',
            textDecoration: isCancelled ? 'line-through' : 'none',
          }}>{lesson.topic}</span>
          {lesson.isKey && (
            <span style={{
              padding:'1px 7px', borderRadius:9999, fontSize:10, fontWeight:700,
              background:'rgba(245,158,11,0.15)', color:'#92400e',
              letterSpacing:'0.05em', textTransform:'uppercase',
            }}>ключевое</span>
          )}
        </div>
        <div style={{ fontSize:12, color:'#94a3b8', marginTop:3 }}>
          {lesson.unit ? `Unit ${lesson.unit} · ${lesson.unitTitle}` : 'Урок не проведён'}
        </div>
      </div>
      {/* Time */}
      <div style={{
        fontFamily:'var(--edv-font-mono, ui-monospace)',
        fontVariantNumeric:'tabular-nums',
        fontSize:12.5, color: isCancelled ? '#94a3b8' : '#475569',
        textDecoration: isCancelled ? 'line-through' : 'none',
      }}>
        {lesson.startTime}–{lesson.endTime}
      </div>
      {/* Status */}
      <div>
        <span style={{
          display:'inline-flex', alignItems:'center', gap:6,
          padding:'3px 9px', borderRadius:9999,
          background: t.bg, color: t.fg,
          fontSize:11, fontWeight:600,
          border: lesson.status==='upcoming' ? '1px solid rgba(99,102,241,0.30)' : 'none',
        }}>
          <span style={{ width:6, height:6, borderRadius:9999, background:t.dot }}/>
          {t.label}
        </span>
      </div>
      {/* Attendance */}
      <div style={{ fontSize:12, color:'#475569',
        fontVariantNumeric:'tabular-nums', textAlign:'right' }}>
        {att ? (
          <>
            <span style={{ fontWeight:600, color:'#047857' }}>
              {att.present}/{total}
            </span>
            {att.late > 0 && <span style={{ color:'#92400e' }}> · {att.late} опозд.</span>}
          </>
        ) : isCancelled ? <span style={{ color:'#94a3b8' }}>—</span>
          : <span style={{ color:'#cbd5e1' }}>не проведён</span>}
      </div>
      <Icon.ChevronRight size={14} stroke="#cbd5e1"/>
    </button>
  );
}

// ── Lesson detail (right panel) ──────────────────────────────────────
function LessonDetail({ lesson, group }) {
  if (!lesson) return null;
  const t = STATUS_TONE[lesson.status];
  const date = svParse(lesson.date);
  const dowLong = SCHEDULE_RU_WEEKDAYS_LONG[date.getDay()];
  const daysFromToday = svDaysBetween(TODAY_GS_VIEW, lesson.date);
  const isCancelled = lesson.status === 'cancelled';
  const att = lesson.attendance;
  const isToday = lesson.status === 'today';
  const isPast = lesson.status === 'done';

  let when;
  if (daysFromToday === 0) when = 'сегодня';
  else if (daysFromToday === 1) when = 'завтра';
  else if (daysFromToday === -1) when = 'вчера';
  else if (daysFromToday > 0) when = `через ${daysFromToday} ${svPlural(daysFromToday,'день','дня','дней')}`;
  else when = `${Math.abs(daysFromToday)} ${svPlural(Math.abs(daysFromToday),'день','дня','дней')} назад`;

  return (
    <section style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      overflow:'hidden',
    }}>
      {/* Date strip */}
      <div style={{
        padding:'14px 18px', borderBottom:'1px solid #f1f5f9',
        background: isToday ? 'linear-gradient(135deg, #4f46e5, #4338ca)'
                 : 'linear-gradient(180deg, #fafbfc, #fff)',
        color: isToday ? '#fff' : '#0f172a',
        display:'flex', alignItems:'center', gap:14,
      }}>
        <div style={{
          width:54, height:54, borderRadius:12,
          background: isToday ? 'rgba(255,255,255,0.18)' : '#fff',
          border: isToday ? 'none' : '1px solid #e2e8f0',
          display:'flex', flexDirection:'column', alignItems:'center',
          justifyContent:'center', flexShrink:0,
        }}>
          <span style={{
            fontSize:11, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase',
            color: isToday ? 'rgba(255,255,255,0.78)' : '#94a3b8',
          }}>
            {SCHEDULE_RU_MONTHS[date.getMonth()].toLowerCase()}
          </span>
          <span style={{
            fontSize:22, fontWeight:700, lineHeight:1,
            color: isToday ? '#fff' : '#0f172a',
            fontVariantNumeric:'tabular-nums', letterSpacing:'-0.02em', marginTop:2,
          }}>{date.getDate()}</span>
        </div>
        <div style={{ flex:1, minWidth:0 }}>
          <div style={{ fontSize:12.5,
            color: isToday ? 'rgba(255,255,255,0.85)' : '#64748b',
            textTransform:'lowercase',
          }}>{dowLong} · {when}</div>
          <div style={{ display:'flex', alignItems:'center', gap:6, marginTop:4 }}>
            <span style={{
              fontFamily:'var(--edv-font-mono, ui-monospace)',
              fontVariantNumeric:'tabular-nums', fontSize:15, fontWeight:700,
              color: isToday ? '#fff' : '#0f172a',
            }}>{lesson.startTime} – {lesson.endTime}</span>
            <span style={{
              fontSize:11.5, color: isToday ? 'rgba(255,255,255,0.7)' : '#94a3b8',
            }}>· {lesson.duration} мин</span>
          </div>
        </div>
      </div>

      {/* Status pill */}
      <div style={{ padding:'14px 18px 6px',
        display:'flex', alignItems:'center', gap:10, flexWrap:'wrap' }}>
        <span style={{
          display:'inline-flex', alignItems:'center', gap:6,
          padding:'4px 10px', borderRadius:9999,
          background:t.bg, color:t.fg,
          fontSize:11.5, fontWeight:600,
        }}>
          <span style={{ width:6, height:6, borderRadius:9999, background:t.dot }}/>
          {t.label}
        </span>
        {lesson.unit && (
          <span style={{
            fontSize:11.5, fontWeight:600, color:'#475569',
            padding:'4px 10px', borderRadius:9999, background:'#f1f5f9',
          }}>Unit {lesson.unit}</span>
        )}
        {lesson.isKey && (
          <span style={{
            display:'inline-flex', alignItems:'center', gap:5,
            fontSize:11, fontWeight:700, color:'#92400e',
            padding:'4px 10px', borderRadius:9999,
            background:'rgba(245,158,11,0.14)',
            letterSpacing:'0.05em', textTransform:'uppercase',
          }}>
            <Icon.Sparkles size={11} stroke="#92400e"/>Ключевое
          </span>
        )}
      </div>

      {/* Topic */}
      <div style={{ padding:'4px 18px 14px' }}>
        <h3 style={{ margin:0, fontSize:17, fontWeight:700, color:'#0f172a',
          letterSpacing:'-0.01em', lineHeight:1.3,
          textDecoration: isCancelled ? 'line-through' : 'none' }}>
          {lesson.topic}
        </h3>
        {lesson.unitTitle && (
          <div style={{ fontSize:12.5, color:'#64748b', marginTop:4 }}>
            {lesson.unitTitle}
          </div>
        )}
        {lesson.cancelReason && (
          <div style={{
            marginTop:10, padding:'8px 12px', borderRadius:10,
            background:'rgba(245,158,11,0.08)', border:'1px solid rgba(245,158,11,0.25)',
            fontSize:12, color:'#92400e',
            display:'flex', alignItems:'center', gap:8,
          }}>
            <Icon.AlertCircle size={13} stroke="#92400e"/>
            <span>Урок отменён: {lesson.cancelReason}</span>
          </div>
        )}
      </div>

      {/* Meta facts */}
      <div style={{ padding:'0 18px 14px' }}>
        <DetailRow icon="School" label="Кабинет" value={lesson.room}/>
        <DetailRow icon="UserCheck" label="Преподаватель" value={group.teacher.name}/>
        <DetailRow icon="Users" label="Студенты"
          value={`${group.students} в группе`}/>
        {att && (
          <DetailRow icon="ClipboardCheck" label="Посещаемость"
            value={
              <span style={{ display:'inline-flex', alignItems:'baseline', gap:4 }}>
                <span style={{ fontWeight:600, color:'#047857' }}>
                  {att.present}/{group.students}
                </span>
                <span style={{ fontSize:11.5, color:'#94a3b8' }}>
                  · {Math.round(att.present/group.students*100)}%
                </span>
              </span>
            }/>
        )}
      </div>

      {/* Actions */}
      <div style={{
        padding:14, borderTop:'1px solid #f1f5f9', background:'#fafbfc',
        display:'flex', flexDirection:'column', gap:8,
      }}>
        {isPast && (
          <>
            <a href="Attendance.html" style={{ textDecoration:'none' }}>
              <Button size="sm" style={{ width:'100%' }}>
                <Icon.ClipboardCheck size={13}/>Открыть журнал
              </Button>
            </a>
            <div style={{ display:'flex', gap:8 }}>
              <a href="Lesson Create.html" style={{ textDecoration:'none', flex:1 }}>
                <Button variant="secondary" size="sm" style={{ width:'100%' }}>
                  <Icon.FileText size={13}/>Материалы
                </Button>
              </a>
              <Button variant="secondary" size="sm" style={{ flex:1 }}>
                <Icon.MessageCircle size={13}/>Заметка
              </Button>
            </div>
          </>
        )}
        {(isToday || lesson.status === 'upcoming') && !isCancelled && (
          <>
            <a href="Lesson Create.html" style={{ textDecoration:'none' }}>
              <Button size="sm" style={{ width:'100%' }}>
                <Icon.FileText size={13}/>Подготовить материалы
              </Button>
            </a>
            <div style={{ display:'flex', gap:8 }}>
              <Button variant="secondary" size="sm" style={{ flex:1 }}>
                <Icon.Clock size={13}/>Перенести
              </Button>
              <Button variant="secondary" size="sm" style={{ flex:1, color:'#b91c1c',
                borderColor:'rgba(239,68,68,0.25)' }}>
                <Icon.X size={13}/>Отменить
              </Button>
            </div>
          </>
        )}
        {isCancelled && (
          <Button variant="secondary" size="sm" style={{ width:'100%' }}>
            <Icon.ArrowUp size={13} style={{ transform:'rotate(180deg)' }}/>Восстановить занятие
          </Button>
        )}
      </div>
    </section>
  );
}

function DetailRow({ icon, label, value }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      display:'grid', gridTemplateColumns:'18px 100px 1fr', alignItems:'center',
      gap:10, padding:'7px 0',
      borderTop:'1px solid #f1f5f9',
    }}>
      <Ic size={14} stroke="#94a3b8"/>
      <span style={{ fontSize:12, color:'#64748b' }}>{label}</span>
      <span style={{ fontSize:13, color:'#0f172a', fontWeight:500, textAlign:'right' }}>
        {value}
      </span>
    </div>
  );
}

// ── Legend card ──────────────────────────────────────────────────────
function LegendCard() {
  const items = [
    { ...STATUS_TONE.done,      hint:'журнал заполнен' },
    { ...STATUS_TONE.today,     hint:'занятие сегодня' },
    { ...STATUS_TONE.upcoming,  hint:'по плану' },
    { ...STATUS_TONE.cancelled, hint:'перенос / праздник' },
  ];
  return (
    <section style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      padding:'14px 16px',
    }}>
      <div style={{ fontSize:11, fontWeight:600, color:'#94a3b8',
        letterSpacing:'0.08em', textTransform:'uppercase', marginBottom:10 }}>
        Условные обозначения
      </div>
      <div style={{ display:'flex', flexDirection:'column', gap:8 }}>
        {items.map(it => (
          <div key={it.label} style={{ display:'flex', alignItems:'center', gap:10 }}>
            <span style={{ width:10, height:10, borderRadius:9999, background:it.dot,
              flexShrink:0 }}/>
            <span style={{ fontSize:12.5, fontWeight:500, color:'#0f172a' }}>
              {it.label}
            </span>
            <span style={{ fontSize:11.5, color:'#94a3b8', marginLeft:'auto' }}>
              {it.hint}
            </span>
          </div>
        ))}
        <div style={{ display:'flex', alignItems:'center', gap:10, marginTop:4,
          paddingTop:8, borderTop:'1px solid #f1f5f9' }}>
          <Icon.Sparkles size={12} stroke="#92400e"/>
          <span style={{ fontSize:12, color:'#0f172a' }}>Ключевое занятие</span>
          <span style={{ fontSize:11.5, color:'#94a3b8', marginLeft:'auto' }}>
            экзамен / контрольная
          </span>
        </div>
      </div>
    </section>
  );
}

// ── Upcoming strip ───────────────────────────────────────────────────
function UpcomingStrip({ lessons, onSelect }) {
  const upcoming = lessons.filter(l =>
    l.status === 'today' || l.status === 'upcoming'
  ).slice(0, 4);
  if (upcoming.length === 0) return null;

  return (
    <section style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      padding:'16px 18px 18px',
    }}>
      <div style={{ display:'flex', alignItems:'baseline',
        justifyContent:'space-between', marginBottom:12 }}>
        <div>
          <h2 style={{ margin:0, fontSize:14.5, fontWeight:600,
            letterSpacing:'-0.005em', color:'#0f172a' }}>Ближайшие занятия</h2>
          <p style={{ margin:'2px 0 0', fontSize:12, color:'#64748b' }}>
            Следующие 4 урока — нажмите, чтобы открыть детали
          </p>
        </div>
        <a href="Lesson Create.html" style={{
          fontSize:12.5, color:'#4f46e5', fontWeight:500,
          display:'inline-flex', alignItems:'center', gap:4,
        }}>
          Создать дополнительный урок <Icon.Plus size={13}/>
        </a>
      </div>
      <div style={{ display:'grid', gap:12,
        gridTemplateColumns:'repeat(4, minmax(0, 1fr))' }}>
        {upcoming.map(l => <UpcomingCard key={l.id} lesson={l}
          onClick={() => onSelect(l.id)} />)}
      </div>
    </section>
  );
}

function UpcomingCard({ lesson, onClick }) {
  const date = svParse(lesson.date);
  const dow = SCHEDULE_RU_WEEKDAYS_LONG[date.getDay()];
  const dom = `${date.getDate()} ${SCHEDULE_RU_MONTHS[date.getMonth()].toLowerCase()}`;
  const daysFromToday = svDaysBetween(TODAY_GS_VIEW, lesson.date);
  const isToday = lesson.status === 'today';
  let pill;
  if (daysFromToday === 0) pill = 'Сегодня';
  else if (daysFromToday === 1) pill = 'Завтра';
  else pill = `Через ${daysFromToday} ${svPlural(daysFromToday,'день','дня','дней')}`;
  return (
    <button onClick={onClick} style={{
      textAlign:'left', cursor:'pointer', fontFamily:'inherit',
      padding:'14px 14px 14px',
      background: isToday ? 'linear-gradient(135deg, #4f46e5, #4338ca)' : '#fff',
      color: isToday ? '#fff' : '#0f172a',
      border:`1px solid ${isToday ? '#4338ca' : '#e2e8f0'}`,
      borderRadius:12,
      display:'flex', flexDirection:'column', gap:8,
      transition:'all .12s',
      boxShadow: isToday ? '0 8px 24px -10px rgba(79,70,229,0.5)' : '0 1px 2px rgba(15,23,42,0.04)',
    }}
      onMouseEnter={e => { if (!isToday) {
        e.currentTarget.style.borderColor='#4f46e5';
        e.currentTarget.style.transform='translateY(-1px)';
        e.currentTarget.style.boxShadow='0 4px 12px rgba(15,23,42,0.06)';
      }}}
      onMouseLeave={e => { if (!isToday) {
        e.currentTarget.style.borderColor='#e2e8f0';
        e.currentTarget.style.transform='translateY(0)';
        e.currentTarget.style.boxShadow='0 1px 2px rgba(15,23,42,0.04)';
      }}}>
      <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between' }}>
        <span style={{
          padding:'3px 9px', borderRadius:9999, fontSize:10.5, fontWeight:700,
          letterSpacing:'0.04em', textTransform:'uppercase',
          background: isToday ? 'rgba(255,255,255,0.20)' : 'rgba(79,70,229,0.10)',
          color: isToday ? '#fff' : '#4338ca',
        }}>{pill}</span>
        {lesson.unit && (
          <span style={{ fontSize:11, color: isToday ? 'rgba(255,255,255,0.75)' : '#94a3b8',
            fontWeight:600 }}>U{lesson.unit}</span>
        )}
      </div>
      <div>
        <div style={{ fontSize:12.5, color: isToday ? 'rgba(255,255,255,0.78)' : '#64748b' }}>
          {dow}, {dom}
        </div>
        <div style={{
          fontFamily:'var(--edv-font-mono, ui-monospace)',
          fontVariantNumeric:'tabular-nums',
          fontSize:18, fontWeight:700, letterSpacing:'-0.02em', marginTop:2,
          color: isToday ? '#fff' : '#0f172a',
        }}>{lesson.startTime} – {lesson.endTime}</div>
      </div>
      <div style={{
        fontSize:12.5, lineHeight:1.4, fontWeight:500,
        color: isToday ? 'rgba(255,255,255,0.92)' : '#0f172a',
        overflow:'hidden', display:'-webkit-box', WebkitBoxOrient:'vertical',
        WebkitLineClamp:2, minHeight:34,
      }}>{lesson.topic}</div>
    </button>
  );
}

window.ScheduleViewApp = ScheduleViewApp;
