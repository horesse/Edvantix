// Group dashboard — visual building blocks
const { useState: useStateGD, useMemo: useMemoGD, useEffect: useEffectGD } = React;

// ── KPI card ─────────────────────────────────────────────────────────
function GdKpi({ label, value, sub, icon, tone='primary', delta }) {
  const palettes = {
    primary: { bg:'rgba(79,70,229,0.10)',  fg:'#4338ca' },
    success: { bg:'rgba(16,185,129,0.12)', fg:'#047857' },
    warning: { bg:'rgba(245,158,11,0.16)', fg:'#92400e' },
    violet:  { bg:'rgba(139,92,246,0.12)', fg:'#6d28d9' },
    slate:   { bg:'#f1f5f9',               fg:'#475569' },
  };
  const c = palettes[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:14,
      padding:18, display:'flex', flexDirection:'column', gap:12,
      boxShadow:'0 1px 2px rgba(15,23,42,0.04)',
    }}>
      <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between' }}>
        <div style={{
          width:36, height:36, borderRadius:10,
          background:c.bg, color:c.fg,
          display:'flex', alignItems:'center', justifyContent:'center',
        }}><Ic size={18} /></div>
        {delta != null && (
          <div style={{
            display:'inline-flex', alignItems:'center', gap:4,
            padding:'3px 8px', borderRadius:9999, fontSize:11.5, fontWeight:600,
            background: delta.tone==='up' ? 'rgba(16,185,129,0.10)'
                      : delta.tone==='down' ? 'rgba(239,68,68,0.10)'
                      : '#f1f5f9',
            color: delta.tone==='up' ? '#047857'
                 : delta.tone==='down' ? '#b91c1c'
                 : '#64748b',
          }}>
            {delta.tone==='up' && <Icon.TrendingUp size={11} />}
            {delta.tone==='down' && <Icon.TrendingDown size={11} />}
            {delta.text}
          </div>
        )}
      </div>
      <div style={{ display:'flex', flexDirection:'column', gap:2 }}>
        <div style={{ fontSize:12.5, color:'#64748b', fontWeight:500 }}>{label}</div>
        <div style={{
          fontSize:30, fontWeight:700, letterSpacing:'-0.02em', lineHeight:1.1,
          fontVariantNumeric:'tabular-nums', color:'#0f172a',
        }}>{value}</div>
        {sub && <div style={{ fontSize:12, color:'#94a3b8', marginTop:2 }}>{sub}</div>}
      </div>
    </div>
  );
}

// ── Section card wrapper ─────────────────────────────────────────────
function GdSection({ title, subtitle, right, children, padding=20, style }) {
  return (
    <section style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:16,
      boxShadow:'0 1px 2px rgba(15,23,42,0.04)', display:'flex', flexDirection:'column',
      ...style,
    }}>
      <header style={{
        padding:`${padding-4}px ${padding}px ${padding-8}px`,
        display:'flex', alignItems:'flex-start', justifyContent:'space-between', gap:16,
      }}>
        <div style={{ display:'flex', flexDirection:'column', gap:2, minWidth:0 }}>
          <div style={{ fontSize:14.5, fontWeight:600, letterSpacing:'-0.005em' }}>{title}</div>
          {subtitle && <div style={{ fontSize:12.5, color:'#64748b' }}>{subtitle}</div>}
        </div>
        {right && <div style={{ flexShrink:0 }}>{right}</div>}
      </header>
      <div style={{ padding:`0 ${padding}px ${padding}px` }}>{children}</div>
    </section>
  );
}

// ── Level chip ───────────────────────────────────────────────────────
function GdLevelChip({ level, full=false, size='md' }) {
  const tone = LEVEL_TONES?.indigo || { bg:'rgba(79,70,229,0.12)', fg:'#4338ca' };
  // Map level to its tone via GROUP_LEVELS if available
  const meta = (typeof GROUP_LEVELS !== 'undefined') && GROUP_LEVELS.find(l => l.value === level);
  const t = meta ? LEVEL_TONES[meta.tone] : tone;
  const padding = size === 'lg' ? '6px 12px' : '3px 10px';
  const fs = size === 'lg' ? 13 : 11.5;
  return (
    <span style={{
      display:'inline-flex', alignItems:'center', gap:6,
      padding, borderRadius:9999,
      background:t.bg, color:t.fg,
      fontSize:fs, fontWeight:600, lineHeight:1.2,
      whiteSpace:'nowrap',
    }}>
      {full && meta ? meta.label : level}
    </span>
  );
}

// ── Status pill (Active/Recruiting/Paused/Finished) ──────────────────
function GdStatusPill({ status, size='md' }) {
  const s = (typeof GROUP_STATUSES !== 'undefined' && GROUP_STATUSES[status]) || GROUP_STATUSES.Active;
  const padding = size === 'lg' ? '5px 12px' : '3px 10px';
  const fs = size === 'lg' ? 12.5 : 11.5;
  return (
    <span style={{
      display:'inline-flex', alignItems:'center', gap:6,
      padding, borderRadius:9999, background:s.bg, color:s.fg,
      fontSize:fs, fontWeight:600, lineHeight:1.2,
    }}>
      <span style={{ width:6, height:6, borderRadius:9999, background:s.dot }}/>
      {s.label}
    </span>
  );
}

// ── Tabs under header ────────────────────────────────────────────────
function GdTabs({ active='overview', onChange }) {
  const tabs = [
    { id:'overview',   label:'Обзор',        icon:'LayoutDashboard' },
    { id:'students',   label:'Студенты',     icon:'Users',         href:'Group Students.html' },
    { id:'schedule',   label:'Расписание',   icon:'CalendarDays',  href:'Group Schedule.html' },
    { id:'attendance', label:'Журнал',       icon:'BarChart2',     href:'Attendance.html', badge:'13/14' },
    { id:'program',    label:'Программа',    icon:'BookOpen' },
    { id:'finance',    label:'Финансы',      icon:'Briefcase' },
  ];
  return (
    <div style={{
      display:'flex', alignItems:'center', gap:2,
      borderBottom:'1px solid #e2e8f0', background:'#fff',
      padding:'0 32px',
    }}>
      {tabs.map(t => {
        const Ic = Icon[t.icon];
        const isActive = t.id === active;
        const content = (
          <span style={{
            display:'inline-flex', alignItems:'center', gap:8,
            padding:'12px 14px', borderBottom: isActive ? '2px solid #4f46e5' : '2px solid transparent',
            color: isActive ? '#0f172a' : '#64748b',
            fontSize:13, fontWeight: isActive ? 600 : 500,
            marginBottom:-1, cursor: t.href ? 'pointer' : 'default',
          }}>
            <Ic size={14} />
            {t.label}
            {t.badge && (
              <span style={{
                fontSize:10.5, fontWeight:600, padding:'1px 6px',
                borderRadius:9999, background:'#f1f5f9', color:'#475569',
                fontVariantNumeric:'tabular-nums',
              }}>{t.badge}</span>
            )}
          </span>
        );
        return t.href
          ? <a key={t.id} href={t.href}>{content}</a>
          : <button key={t.id} onClick={() => onChange?.(t.id)}
              style={{ background:'none', border:'none', padding:0, fontFamily:'inherit' }}>
              {content}
            </button>;
      })}
    </div>
  );
}

// ── Weekly attendance bar chart ──────────────────────────────────────
function GdAttendanceChart({ weeks }) {
  const max = 100;
  const W_BAR = 28;
  return (
    <div style={{ display:'flex', flexDirection:'column', gap:16 }}>
      <div style={{ display:'flex', alignItems:'flex-end', gap:14, height:160,
        padding:'0 4px', borderBottom:'1px solid #f1f5f9' }}>
        {weeks.map(w => {
          const p = w.present, l = w.late, a = w.absent;
          const ph = Math.round(p / max * 140);
          const lh = Math.round(l / max * 140);
          const ah = Math.round(a / max * 140);
          return (
            <div key={w.id} style={{
              flex:'1 1 0', minWidth:0, display:'flex', flexDirection:'column',
              alignItems:'center', gap:6, position:'relative',
            }}>
              <div style={{
                fontSize:11.5, fontWeight:600, color:'#0f172a',
                fontVariantNumeric:'tabular-nums', opacity: w.isCurrent ? 0.55 : 1,
              }}>{p}%</div>
              <div title={`${w.label}: ${p}% присутствие, ${l}% опоздания, ${a}% пропуски`}
                style={{
                  width:W_BAR, display:'flex', flexDirection:'column-reverse',
                  borderRadius:6, overflow:'hidden',
                  outline: w.isCurrent ? '2px dashed rgba(148,163,184,0.6)' : 'none',
                  outlineOffset: 2,
                }}>
                {ph > 0 && <div style={{ height:ph, background:'#10b981' }}/>}
                {lh > 0 && <div style={{ height:lh, background:'#f59e0b' }}/>}
                {ah > 0 && <div style={{ height:ah, background:'#ef4444' }}/>}
              </div>
            </div>
          );
        })}
      </div>
      <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'0 4px', gap:12 }}>
        <div style={{ display:'flex', gap:10, flex:1 }}>
          {weeks.map(w => (
            <div key={w.id} style={{
              flex:'1 1 0', textAlign:'center',
              fontSize:11, color: w.isCurrent ? '#4f46e5' : '#94a3b8',
              fontWeight: w.isCurrent ? 600 : 400,
            }}>{w.label}{w.isCurrent ? ' · сейчас' : ''}</div>
          ))}
        </div>
      </div>
      <div style={{ display:'flex', gap:18, fontSize:12, color:'#475569' }}>
        <LegendDot color="#10b981" label="Присутствие" />
        <LegendDot color="#f59e0b" label="Опоздания" />
        <LegendDot color="#ef4444" label="Пропуски" />
      </div>
    </div>
  );
}

function LegendDot({ color, label }) {
  return (
    <span style={{ display:'inline-flex', alignItems:'center', gap:6 }}>
      <span style={{ width:8, height:8, borderRadius:9999, background:color }}/>
      <span>{label}</span>
    </span>
  );
}

// ── Program track ────────────────────────────────────────────────────
function GdProgramTrack({ units }) {
  return (
    <div style={{ display:'flex', flexDirection:'column', gap:10 }}>
      {units.map(u => {
        const pct = Math.round(u.done / u.lessons * 100);
        const state =
          u.status === 'done'    ? { bg:'#10b981', track:'#d1fae5', fg:'#047857',  label:'Завершён',   chip:'success' } :
          u.status === 'current' ? { bg:'#4f46e5', track:'#e0eaff', fg:'#4338ca',  label:'Сейчас',     chip:'primary' } :
          u.status === 'next'    ? { bg:'#94a3b8', track:'#f1f5f9', fg:'#475569',  label:'Следующий',  chip:'default' } :
                                   { bg:'#cbd5e1', track:'#f8fafc', fg:'#94a3b8',  label:'Запланирован', chip:'default' };
        return (
          <div key={u.id} style={{
            display:'grid',
            gridTemplateColumns:'72px 1fr auto',
            alignItems:'center', gap:14,
            padding:'10px 12px', borderRadius:10,
            background: u.status === 'current' ? 'rgba(79,70,229,0.04)' : 'transparent',
            border: u.status === 'current' ? '1px solid rgba(79,70,229,0.18)' : '1px solid transparent',
          }}>
            <div style={{ fontSize:12, fontWeight:600, color:'#475569',
              fontVariantNumeric:'tabular-nums' }}>{u.code}</div>
            <div style={{ display:'flex', flexDirection:'column', gap:6, minWidth:0 }}>
              <div style={{ display:'flex', alignItems:'center', gap:10 }}>
                <div style={{ fontSize:13.5, fontWeight: u.status==='current' ? 600 : 500,
                  color:'#0f172a', whiteSpace:'nowrap', overflow:'hidden', textOverflow:'ellipsis' }}>
                  {u.title}
                </div>
                {u.status === 'current' && (
                  <Badge variant="primary" dot>Текущий юнит</Badge>
                )}
                {u.status === 'done' && (
                  <Icon.CircleCheck size={14} stroke="#10b981" />
                )}
              </div>
              <div style={{ height:6, borderRadius:9999, background:state.track, overflow:'hidden' }}>
                <div style={{ width:`${pct}%`, height:'100%', background:state.bg, borderRadius:9999 }}/>
              </div>
            </div>
            <div style={{
              fontSize:12, color:'#64748b', fontVariantNumeric:'tabular-nums',
              minWidth:46, textAlign:'right',
            }}>{u.done}/{u.lessons}</div>
          </div>
        );
      })}
    </div>
  );
}

// ── Sparkline (per student) ──────────────────────────────────────────
function GdStudentSpark({ data }) {
  const colors = { 1:'#10b981', 2:'#f59e0b', 0:'#ef4444', 3:'#6366f1', 9:'#f1f5f9' };
  return (
    <div style={{ display:'flex', gap:2, alignItems:'center' }}>
      {data.map((v, i) => (
        <span key={i} style={{
          width:6, height: v===9 ? 4 : 14, borderRadius:2,
          background: colors[v], opacity: v===9 ? 0.6 : 1,
        }}/>
      ))}
    </div>
  );
}

// ── Mini info row ────────────────────────────────────────────────────
function GdInfoRow({ icon, label, value, valueRight }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      display:'grid', gridTemplateColumns:'24px 1fr auto', gap:10,
      alignItems:'center', padding:'8px 0',
    }}>
      <div style={{ color:'#94a3b8' }}><Ic size={15} /></div>
      <div style={{ fontSize:12.5, color:'#64748b' }}>{label}</div>
      <div style={{ fontSize:13, fontWeight:500, color:'#0f172a' }}>
        {valueRight || value}
      </div>
    </div>
  );
}

Object.assign(window, {
  GdKpi, GdSection, GdLevelChip, GdStatusPill, GdTabs,
  GdAttendanceChart, GdProgramTrack, GdStudentSpark, GdInfoRow,
});
