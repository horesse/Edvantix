// Reusable components for Groups page

// ── KPI block ────────────────────────────────────────────────────────
const kpiTones = {
  slate:   { bg: '#f1f5f9', fg: '#475569' },
  primary: { bg: '#e0eaff', fg: '#4338ca' },
  success: { bg: '#d1fae5', fg: '#047857' },
  warning: { bg: '#fef3c7', fg: '#92400e' },
};
function KpiBlock({ label, value, icon, tone = 'slate', delta }) {
  const t = kpiTones[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '16px 18px', display: 'flex', flexDirection: 'column', gap: 12,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
        <div style={{
          width: 32, height: 32, borderRadius: 8, background: t.bg, color: t.fg,
          display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0,
        }}><Ic size={16} /></div>
        <div style={{ fontSize: 12.5, color: '#64748b', fontWeight: 500 }}>{label}</div>
      </div>
      <div>
        <div style={{ fontSize: 26, fontWeight: 700, lineHeight: 1, letterSpacing: '-0.02em',
          color: '#0f172a', fontVariantNumeric: 'tabular-nums',
        }}>{value}</div>
        {delta && (
          <div style={{ marginTop: 6, fontSize: 12, color: '#64748b' }}>{delta}</div>
        )}
      </div>
    </div>
  );
}

// ── Filter dropdown ──────────────────────────────────────────────────
function FilterDropdown({ label, icon, value, onChange, options }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);

  const Ic = Icon[icon];
  const count = value.size;
  const toggle = (v) => {
    const n = new Set(value);
    n.has(v) ? n.delete(v) : n.add(v);
    onChange(n);
  };

  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button
        onClick={() => setOpen(o => !o)}
        style={{
          display: 'inline-flex', alignItems: 'center', gap: 6,
          height: 36, padding: '0 12px', borderRadius: 10,
          border: `1px solid ${count ? '#c7d6fe' : '#e2e8f0'}`,
          background: count ? '#f0f4ff' : '#fff',
          color: count ? '#4338ca' : '#334155',
          fontSize: 13, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
        }}>
        <Ic size={14} />{label}
        {count > 0 && (
          <span style={{
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            minWidth: 18, height: 18, padding: '0 5px', borderRadius: 9999,
            background: '#4f46e5', color: '#fff', fontSize: 11, fontWeight: 600,
          }}>{count}</span>
        )}
        <Icon.ChevronDown size={14} stroke={count ? '#4338ca' : '#94a3b8'} />
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 4px)', left: 0, zIndex: 30,
          minWidth: 220, background: '#fff', border: '1px solid #e2e8f0',
          borderRadius: 12, boxShadow: '0 10px 30px rgba(15,23,42,0.10)',
          padding: 6, maxHeight: 320, overflowY: 'auto',
        }}>
          {options.map(o => {
            const checked = value.has(o.value);
            return (
              <button key={o.value} onClick={() => toggle(o.value)}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: checked ? '#f0f4ff' : 'transparent', cursor: 'pointer',
                  textAlign: 'left', fontFamily: 'inherit',
                }}
                onMouseEnter={e => { if (!checked) e.currentTarget.style.background = '#f8fafc'; }}
                onMouseLeave={e => { if (!checked) e.currentTarget.style.background = 'transparent'; }}>
                <span style={{
                  width: 16, height: 16, borderRadius: 4, flexShrink: 0,
                  border: `1.5px solid ${checked ? '#4f46e5' : '#cbd5e1'}`,
                  background: checked ? '#4f46e5' : '#fff',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                }}>{checked && <Icon.Check size={11} stroke="#fff" sw={3} />}</span>
                <span style={{
                  width: 8, height: 8, borderRadius: 9999, background: o.swatch, flexShrink: 0,
                }}/>
                <span style={{ fontSize: 13, color: '#0f172a', flex: 1 }}>{o.label}</span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

// ── Checkbox ─────────────────────────────────────────────────────────
function CheckboxG({ checked, indeterminate, onChange }) {
  return (
    <span
      onClick={onChange}
      style={{
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        width: 16, height: 16, borderRadius: 4,
        border: `1.5px solid ${checked || indeterminate ? '#4f46e5' : '#cbd5e1'}`,
        background: checked || indeterminate ? '#4f46e5' : '#fff',
        cursor: 'pointer', flexShrink: 0,
      }}
    >
      {checked && <Icon.Check size={11} stroke="#fff" sw={3} />}
      {indeterminate && !checked && <span style={{ width: 8, height: 2, background: '#fff', borderRadius: 1 }} />}
    </span>
  );
}

// ── Sort header ──────────────────────────────────────────────────────
function SortHeader({ field, sort, onSort, width, align = 'left', children }) {
  const active = sort.field === field;
  return (
    <th style={{
      padding: '12px 16px', width, textAlign: align,
      background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
      fontSize: 12, fontWeight: 600, color: '#64748b', letterSpacing: 0.2,
      textTransform: 'uppercase', cursor: 'pointer', userSelect: 'none',
    }} onClick={() => onSort(field)}>
      <span style={{
        display: 'inline-flex', alignItems: 'center', gap: 4,
        color: active ? '#0f172a' : '#64748b',
      }}>
        {children}
        <span style={{
          display: 'inline-flex', flexDirection: 'column', lineHeight: 0.6,
          color: active ? '#4f46e5' : '#cbd5e1',
        }}>
          <span style={{ fontSize: 8, opacity: active && sort.dir === 'asc' ? 1 : 0.5 }}>▲</span>
          <span style={{ fontSize: 8, opacity: active && sort.dir === 'desc' ? 1 : 0.5 }}>▼</span>
        </span>
      </span>
    </th>
  );
}

// ── Level pill ───────────────────────────────────────────────────────
function LevelPill({ level }) {
  const def = GROUP_LEVELS.find(l => l.value === level);
  if (!def) return null;
  const t = LEVEL_TONES[def.tone];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 9px', borderRadius: 9999, background: t.bg, color: t.fg,
      fontSize: 12, fontWeight: 600, lineHeight: 1.4,
    }}>{def.label}</span>
  );
}

// ── Status pill ──────────────────────────────────────────────────────
function StatusPillG({ status }) {
  const s = GROUP_STATUSES[status];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 10px', borderRadius: 9999, background: s.bg, color: s.fg,
      fontSize: 12, fontWeight: 500, lineHeight: 1.4,
    }}>
      <span style={{ width: 6, height: 6, borderRadius: 9999, background: s.dot }} />
      {s.label}
    </span>
  );
}

// ── Format chip ──────────────────────────────────────────────────────
function FormatChip({ format }) {
  const f = GROUP_FORMATS[format];
  const Ic = Icon[f.icon];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      fontSize: 12.5, color: '#475569',
    }}>
      <Ic size={13} stroke="#94a3b8" />
      {f.label}
    </span>
  );
}

// ── Capacity bar ─────────────────────────────────────────────────────
function CapacityBar({ students, capacity }) {
  const pct = Math.min(100, Math.round(students / capacity * 100));
  const full = students >= capacity;
  const empty = students === 0;
  const color = full ? '#10b981' : empty ? '#cbd5e1' : pct >= 80 ? '#f59e0b' : '#6366f1';
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 4, minWidth: 100 }}>
      <div style={{
        fontSize: 13, fontWeight: 500, color: '#0f172a', fontVariantNumeric: 'tabular-nums',
      }}>
        {students}<span style={{ color: '#94a3b8', fontWeight: 400 }}> / {capacity}</span>
      </div>
      <div style={{
        width: '100%', height: 5, background: '#f1f5f9', borderRadius: 9999, overflow: 'hidden',
      }}>
        <div style={{
          width: `${pct}%`, height: '100%', background: color, borderRadius: 9999,
          transition: 'width .3s',
        }} />
      </div>
    </div>
  );
}

// ── Row menu ─────────────────────────────────────────────────────────
function RowMenuG() {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);
  return (
    <div ref={ref} style={{ position: 'relative', display: 'inline-block' }}>
      <button onClick={(e) => { e.preventDefault(); setOpen(o => !o); }}
        style={{
          width: 28, height: 28, borderRadius: 6, border: 'none',
          background: open ? '#f1f5f9' : 'transparent', color: '#64748b',
          cursor: 'pointer', display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}
        onMouseEnter={e => { if (!open) e.currentTarget.style.background = '#f1f5f9'; }}
        onMouseLeave={e => { if (!open) e.currentTarget.style.background = 'transparent'; }}>
        <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
          <circle cx="5" cy="12" r="2"/><circle cx="12" cy="12" r="2"/><circle cx="19" cy="12" r="2"/>
        </svg>
      </button>
      {open && (
        <div style={{
          position: 'absolute', right: 0, top: 'calc(100% + 4px)', zIndex: 20,
          minWidth: 200, background: '#fff', border: '1px solid #e2e8f0',
          borderRadius: 10, boxShadow: '0 10px 30px rgba(15,23,42,0.12)',
          padding: 4,
        }}>
          <MenuItem icon="Users" label="Открыть группу" />
          <MenuItem icon="UserPlus" label="Зачислить студентов" />
          <MenuItem icon="Calendar" label="Расписание занятий" />
          <MenuItem icon="FileText" label="Журнал" />
          <div style={{ height: 1, background: '#f1f5f9', margin: '4px 0' }} />
          <MenuItem icon="X" label="Архивировать" danger />
        </div>
      )}
    </div>
  );
}
function MenuItem({ icon, label, danger }) {
  const Ic = Icon[icon];
  return (
    <button style={{
      display: 'flex', alignItems: 'center', gap: 10, width: '100%',
      padding: '8px 10px', borderRadius: 6, border: 'none', background: 'transparent',
      fontSize: 13, color: danger ? '#b91c1c' : '#0f172a', cursor: 'pointer',
      fontFamily: 'inherit', textAlign: 'left',
    }}
      onMouseEnter={e => e.currentTarget.style.background = danger ? '#fef2f2' : '#f8fafc'}
      onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
      <Ic size={14} stroke={danger ? '#b91c1c' : '#64748b'} />{label}
    </button>
  );
}

// ── Empty state ──────────────────────────────────────────────────────
function EmptyStateG({ onReset }) {
  return (
    <div style={{
      display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 10,
      color: '#64748b', textAlign: 'center',
    }}>
      <div style={{
        width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Search size={24} stroke="#94a3b8" />
      </div>
      <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>Группы не найдены</div>
      <div style={{ fontSize: 13, color: '#64748b', maxWidth: 360 }}>
        Попробуйте изменить запрос или сбросить фильтры.
      </div>
      <button
        onClick={onReset}
        style={{
          marginTop: 4, height: 32, padding: '0 14px', borderRadius: 8,
          border: '1px solid #e2e8f0', background: '#fff', color: '#334155',
          fontSize: 13, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
        }}
      >Сбросить фильтры</button>
    </div>
  );
}

// ── Table view ───────────────────────────────────────────────────────
function GroupsTable({ rows, sort, onSort, selected, toggleAll, toggleOne, allSelected, someSelected }) {
  return (
    <div style={{ overflowX: 'auto' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13.5 }}>
        <thead>
          <tr>
            <th style={{
              padding: '12px 12px 12px 20px', width: 40,
              background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
            }}>
              <CheckboxG
                checked={allSelected}
                indeterminate={!allSelected && someSelected}
                onChange={toggleAll}
              />
            </th>
            <SortHeader field="name"     sort={sort} onSort={onSort}>Группа</SortHeader>
            <SortHeader field="level"    sort={sort} onSort={onSort} width={170}>Уровень</SortHeader>
            <SortHeader field="teacher"  sort={sort} onSort={onSort} width={180}>Преподаватель</SortHeader>
            <SortHeader field="schedule" sort={sort} onSort={onSort} width={210}>Расписание</SortHeader>
            <SortHeader field="students" sort={sort} onSort={onSort} width={140}>Состав</SortHeader>
            <SortHeader field="status"   sort={sort} onSort={onSort} width={150}>Статус</SortHeader>
            <th style={{ width: 56, background: '#f8fafc', borderBottom: '1px solid #e2e8f0' }} />
          </tr>
        </thead>
        <tbody>
          {rows.map(g => (
            <tr key={g.id} style={{
              borderBottom: '1px solid #f1f5f9',
              background: selected.has(g.id) ? 'rgba(79,70,229,0.03)' : 'transparent',
              transition: 'background .1s',
            }}
              onMouseEnter={e => { if (!selected.has(g.id)) e.currentTarget.style.background = '#fafbfc'; }}
              onMouseLeave={e => { if (!selected.has(g.id)) e.currentTarget.style.background = 'transparent'; }}>
              <td style={{ padding: '14px 12px 14px 20px' }}>
                <CheckboxG checked={selected.has(g.id)} onChange={() => toggleOne(g.id)} />
              </td>
              <td style={{ padding: '14px 16px' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <GroupBadge code={g.code} level={g.level} />
                  <div style={{ minWidth: 0 }}>
                    <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>
                      {g.name}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 3 }}>
                      <span style={{
                        fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b',
                      }}>{g.code}</span>
                      <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                      <span style={{ fontSize: 12, color: '#64748b' }}>{g.course}</span>
                    </div>
                  </div>
                </div>
              </td>
              <td style={{ padding: '14px 16px' }}><LevelPill level={g.level} /></td>
              <td style={{ padding: '14px 16px' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <Avatar name={g.teacher} size={24} />
                  <span style={{ fontSize: 13, color: '#0f172a' }}>{g.teacher}</span>
                </div>
              </td>
              <td style={{ padding: '14px 16px' }}>
                <div style={{ fontSize: 13, color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>
                  {g.schedule}
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 3 }}>
                  <FormatChip format={g.format} />
                  <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
                  <span style={{ fontSize: 12, color: '#64748b' }}>{g.room}</span>
                </div>
              </td>
              <td style={{ padding: '14px 16px' }}>
                <CapacityBar students={g.students} capacity={g.capacity} />
              </td>
              <td style={{ padding: '14px 16px' }}><StatusPillG status={g.status} /></td>
              <td style={{ padding: '14px 12px 14px 8px', textAlign: 'right' }}>
                <RowMenuG />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

// ── Group badge (square initial) ─────────────────────────────────────
function GroupBadge({ code, level }) {
  const def = GROUP_LEVELS.find(l => l.value === level);
  const t = def ? LEVEL_TONES[def.tone] : LEVEL_TONES.slate;
  return (
    <div style={{
      width: 38, height: 38, borderRadius: 10, flexShrink: 0,
      background: t.bg, color: t.fg,
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontSize: 12, fontWeight: 700, fontFamily: 'var(--edv-font-mono)',
      letterSpacing: '-0.01em',
    }}>{level}</div>
  );
}

// ── Cards view ───────────────────────────────────────────────────────
function GroupsCards({ rows }) {
  return (
    <div style={{
      padding: 16, display: 'grid', gap: 14,
      gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))',
    }}>
      {rows.map(g => <GroupCard key={g.id} group={g} />)}
    </div>
  );
}

function GroupCard({ group: g }) {
  const [hover, setHover] = React.useState(false);
  return (
    <div onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      style={{
        background: '#fff', border: `1px solid ${hover ? '#c7d6fe' : '#e2e8f0'}`,
        borderRadius: 14, padding: 16, transition: 'all .15s',
        boxShadow: hover ? '0 6px 16px rgba(15,23,42,0.06)' : 'none',
        display: 'flex', flexDirection: 'column', gap: 12, cursor: 'pointer',
      }}>
      <div style={{ display: 'flex', alignItems: 'flex-start', gap: 12 }}>
        <GroupBadge code={g.code} level={g.level} />
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a', lineHeight: 1.3 }}>
            {g.name}
          </div>
          <div style={{
            fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b', marginTop: 3,
          }}>{g.code} · {g.course}</div>
        </div>
        <StatusPillG status={g.status} />
      </div>

      <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <Avatar name={g.teacher} size={24} />
        <span style={{ fontSize: 12.5, color: '#475569' }}>{g.teacher}</span>
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: 6, fontSize: 12.5, color: '#475569' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <Icon.Calendar size={13} stroke="#94a3b8" />
          <span>{g.schedule}</span>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <FormatChip format={g.format} />
          <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
          <span style={{ color: '#64748b' }}>{g.room}</span>
        </div>
      </div>

      <div style={{
        marginTop: 'auto', paddingTop: 10, borderTop: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 10,
      }}>
        <CapacityBar students={g.students} capacity={g.capacity} />
        <span style={{ fontSize: 11.5, color: '#94a3b8' }}>
          до {g.ends.split('.').slice(1).join('.')}
        </span>
      </div>
    </div>
  );
}

window.KpiBlock = KpiBlock;
window.FilterDropdown = FilterDropdown;
window.GroupsTable = GroupsTable;
window.GroupsCards = GroupsCards;
