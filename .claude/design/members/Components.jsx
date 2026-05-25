// Members page — table with search, filters, KPIs, pagination
const { useState: useStateM, useMemo: useMemoM } = React;

// ── Status pill ──────────────────────────────────────────────────────
function StatusPill({ status }) {
  const s = MEMBER_STATUSES[status];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 10px', borderRadius: 9999,
      background: s.bg, color: s.fg,
      fontSize: 12, fontWeight: 500, lineHeight: 1.4,
    }}>
      <span style={{ width: 6, height: 6, borderRadius: 9999, background: s.dot }} />
      {s.label}
    </span>
  );
}

// ── Role tag ─────────────────────────────────────────────────────────
function RoleTag({ role }) {
  const r = MEMBER_ROLES.find(x => x.value === role);
  if (!r) return <span style={{ color: '#94a3b8', fontSize: 13 }}>{role}</span>;
  const t = ROLE_TONES[r.tone];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center',
      padding: '3px 10px', borderRadius: 6,
      background: t.bg, color: t.fg,
      fontSize: 12, fontWeight: 500, lineHeight: 1.4,
    }}>{r.label}</span>
  );
}

// ── KPI card ─────────────────────────────────────────────────────────
function KpiBlock({ label, value, delta, icon, tone = 'slate' }) {
  const IC = Icon[icon];
  const tones = {
    slate:  { bg: '#f1f5f9', fg: '#475569' },
    success:{ bg: 'rgba(16,185,129,0.12)', fg: '#047857' },
    primary:{ bg: 'rgba(79,70,229,0.12)', fg: '#4338ca' },
    warning:{ bg: 'rgba(245,158,11,0.14)', fg: '#92400e' },
  }[tone];
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '16px 18px', display: 'flex', alignItems: 'center', gap: 14,
      minWidth: 0,
    }}>
      <div style={{
        width: 40, height: 40, borderRadius: 10, flexShrink: 0,
        background: tones.bg, color: tones.fg,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}><IC size={19} stroke={tones.fg} /></div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{
          fontSize: 12, color: '#64748b', fontWeight: 500,
          whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
        }}>{label}</div>
        <div style={{ display: 'flex', alignItems: 'baseline', gap: 8, marginTop: 2, minWidth: 0 }}>
          <div style={{
            fontSize: 26, fontWeight: 700, letterSpacing: '-0.02em',
            fontVariantNumeric: 'tabular-nums', color: '#0f172a', flexShrink: 0,
          }}>{value}</div>
          {delta && (
            <div style={{
              fontSize: 11.5, fontWeight: 500, color: '#64748b',
              whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis', minWidth: 0,
            }}>{delta}</div>
          )}
        </div>
      </div>
    </div>
  );
}

// ── Filter dropdown ──────────────────────────────────────────────────
function FilterDropdown({ label, options, value, onChange, icon }) {
  const [open, setOpen] = useStateM(false);
  const IC = icon ? Icon[icon] : null;
  const selected = value.size;
  const ref = React.useRef(null);

  React.useEffect(() => {
    const h = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', h);
    return () => document.removeEventListener('mousedown', h);
  }, []);

  const toggle = (v) => {
    const n = new Set(value);
    if (n.has(v)) n.delete(v); else n.add(v);
    onChange(n);
  };

  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button
        onClick={() => setOpen(!open)}
        style={{
          display: 'inline-flex', alignItems: 'center', gap: 8,
          height: 36, padding: '0 14px', borderRadius: 10,
          border: `1px solid ${selected > 0 ? '#c7d6fe' : '#e2e8f0'}`,
          background: selected > 0 ? 'rgba(79,70,229,0.05)' : '#fff',
          color: selected > 0 ? '#4338ca' : '#334155',
          fontSize: 13, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
          transition: 'all .15s',
        }}
      >
        {IC && <IC size={14} />}
        <span>{label}</span>
        {selected > 0 && (
          <span style={{
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            minWidth: 18, height: 18, padding: '0 6px', borderRadius: 9999,
            background: '#4f46e5', color: '#fff',
            fontSize: 11, fontWeight: 600, fontVariantNumeric: 'tabular-nums',
          }}>{selected}</span>
        )}
        <Icon.ChevronDown size={14} stroke={selected > 0 ? '#4338ca' : '#94a3b8'} />
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 42, left: 0, zIndex: 20,
          minWidth: 220, background: '#fff', borderRadius: 12,
          border: '1px solid #e2e8f0', boxShadow: '0 10px 30px rgba(15,23,42,0.12)',
          padding: 6, animation: 'fadeIn .12s ease-out',
        }}>
          {options.map(o => {
            const checked = value.has(o.value);
            return (
              <button
                key={o.value}
                onClick={() => toggle(o.value)}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: 'transparent', fontSize: 13, color: '#0f172a',
                  fontFamily: 'inherit', textAlign: 'left', cursor: 'pointer',
                }}
                onMouseEnter={e => e.currentTarget.style.background = '#f8fafc'}
                onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
              >
                <span style={{
                  width: 16, height: 16, borderRadius: 4, flexShrink: 0,
                  border: `1.5px solid ${checked ? '#4f46e5' : '#cbd5e1'}`,
                  background: checked ? '#4f46e5' : '#fff',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  {checked && <Icon.Check size={11} stroke="#fff" sw={3} />}
                </span>
                {o.swatch && (
                  <span style={{
                    width: 6, height: 6, borderRadius: 9999, background: o.swatch,
                    flexShrink: 0,
                  }} />
                )}
                <span style={{ flex: 1 }}>{o.label}</span>
              </button>
            );
          })}
          {value.size > 0 && (
            <>
              <div style={{ height: 1, background: '#f1f5f9', margin: '4px 0' }} />
              <button
                onClick={() => onChange(new Set())}
                style={{
                  width: '100%', padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: 'transparent', color: '#64748b', fontSize: 12,
                  fontFamily: 'inherit', cursor: 'pointer', textAlign: 'left',
                }}
                onMouseEnter={e => e.currentTarget.style.background = '#f8fafc'}
                onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
              >Сбросить</button>
            </>
          )}
        </div>
      )}
    </div>
  );
}

// ── Row action menu ──────────────────────────────────────────────────
function RowMenu({ status }) {
  const [open, setOpen] = useStateM(false);
  const ref = React.useRef(null);

  React.useEffect(() => {
    const h = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', h);
    return () => document.removeEventListener('mousedown', h);
  }, []);

  const items = [
    { label: 'Открыть профиль', icon: 'ArrowRight' },
    { label: 'Изменить роль', icon: 'Settings' },
    ...(status === 'Invited' ? [{ label: 'Повторить приглашение', icon: 'Send' }] : []),
    ...(status === 'Active' ? [{ label: 'Заблокировать', icon: 'Shield', danger: true }] : []),
    ...(status === 'Blocked' ? [{ label: 'Разблокировать', icon: 'CircleCheck' }] : []),
    { label: 'Удалить', icon: 'X', danger: true },
  ];

  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button
        onClick={() => setOpen(!open)}
        style={{
          width: 30, height: 30, borderRadius: 8, border: 'none',
          background: open ? '#f1f5f9' : 'transparent', color: '#64748b',
          display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer',
        }}
        onMouseEnter={e => e.currentTarget.style.background = '#f1f5f9'}
        onMouseLeave={e => { if (!open) e.currentTarget.style.background = 'transparent'; }}
        aria-label="Действия"
      >
        <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
          <circle cx="12" cy="5" r="1.5"/><circle cx="12" cy="12" r="1.5"/><circle cx="12" cy="19" r="1.5"/>
        </svg>
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 34, right: 0, zIndex: 30,
          minWidth: 220, background: '#fff', borderRadius: 12,
          border: '1px solid #e2e8f0', boxShadow: '0 10px 30px rgba(15,23,42,0.12)',
          padding: 6, animation: 'fadeIn .12s ease-out',
        }}>
          {items.map((it, i) => {
            const IC = Icon[it.icon];
            return (
              <button
                key={i}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: 'transparent',
                  color: it.danger ? '#b91c1c' : '#0f172a',
                  fontSize: 13, fontFamily: 'inherit', cursor: 'pointer', textAlign: 'left',
                }}
                onMouseEnter={e => e.currentTarget.style.background = it.danger ? '#fef2f2' : '#f8fafc'}
                onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
                onClick={() => setOpen(false)}
              >
                <IC size={14} />{it.label}
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

// ── Sortable column header ───────────────────────────────────────────
function SortHeader({ children, field, sort, onSort, align = 'left', width }) {
  const active = sort.field === field;
  const dir = active ? sort.dir : null;
  return (
    <th style={{
      padding: '12px 16px', textAlign: align, fontSize: 11,
      fontWeight: 600, letterSpacing: '0.05em', textTransform: 'uppercase',
      color: '#64748b', background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
      width, position: 'sticky', top: 0, zIndex: 1, userSelect: 'none',
    }}>
      <button
        onClick={() => onSort(field)}
        style={{
          display: 'inline-flex', alignItems: 'center', gap: 4,
          background: 'transparent', border: 'none',
          color: active ? '#0f172a' : '#64748b',
          font: 'inherit', fontWeight: 600, cursor: 'pointer',
          padding: 0, textTransform: 'inherit', letterSpacing: 'inherit',
        }}
      >
        {children}
        <svg width="12" height="12" viewBox="0 0 12 12" style={{ opacity: active ? 1 : 0.35 }}>
          <path d="M6 2l3 3H3z" fill={dir === 'desc' ? '#cbd5e1' : 'currentColor'} />
          <path d="M6 10l-3-3h6z" fill={dir === 'asc' ? '#cbd5e1' : 'currentColor'} />
        </svg>
      </button>
    </th>
  );
}

window.StatusPill = StatusPill;
window.RoleTag = RoleTag;
window.KpiBlock = KpiBlock;
window.FilterDropdown = FilterDropdown;
window.RowMenu = RowMenu;
window.SortHeader = SortHeader;
