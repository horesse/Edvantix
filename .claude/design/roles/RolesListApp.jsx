// Roles list page
const { useState: useStateR, useMemo: useMemoR } = React;

function RolesListApp() {
  const [query, setQuery] = useStateR('');

  const filtered = useMemoR(() => {
    const q = query.trim().toLowerCase();
    if (!q) return ROLES;
    return ROLES.filter(r =>
      r.name.toLowerCase().includes(q) || r.description.toLowerCase().includes(q));
  }, [query]);

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="settings" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Настройки</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Роли и права</span>
        </div>

        <div style={{
          padding: '22px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 20,
        }}>
          <div style={{ flex: 1, minWidth: 0 }}>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
              Роли и права
            </h1>
            <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
              Наборы прав для сотрудников — определяют, что участники видят и могут делать
            </div>
          </div>
          <Button><Icon.Plus size={16} />Создать роль</Button>
        </div>

        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 40px' }}>
          <div style={{ maxWidth: 1040, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 16 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <div style={{ position: 'relative', width: 320, height: 36 }}>
                <Icon.Search size={15} stroke="#94a3b8"
                  style={{ position: 'absolute', left: 12, top: 11, pointerEvents: 'none' }} />
                <input
                  value={query}
                  onChange={e => setQuery(e.target.value)}
                  placeholder="Поиск по названию или описанию"
                  style={{
                    width: '100%', height: 36, paddingLeft: 34, paddingRight: 12,
                    borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
                    fontSize: 13, fontFamily: 'inherit', outline: 'none',
                  }}
                  onFocus={e => { e.target.style.borderColor='#6366f1'; e.target.style.boxShadow='0 0 0 3px rgba(99,102,241,0.2)'; }}
                  onBlur={e => { e.target.style.borderColor='#e2e8f0'; e.target.style.boxShadow='none'; }}
                />
              </div>
              <div style={{ fontSize: 13, color: '#64748b', marginLeft: 'auto' }}>
                Всего ролей: <strong style={{ color: '#0f172a' }}>{ROLES.length}</strong>
              </div>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: 12 }}>
              {filtered.map(r => <RoleCard key={r.id} role={r} />)}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function RoleCard({ role }) {
  const t = TONE_COLORS[role.tone];
  const granted = role.permissions.size;
  const pct = Math.round(granted / ALL_PERM_IDS.length * 100);

  return (
    <a href={`Role Edit.html?id=${role.id}`} style={{
      display: 'block', background: '#fff', border: '1px solid #e2e8f0',
      borderRadius: 14, padding: '18px 20px', transition: 'all .15s',
    }}
      onMouseEnter={e => { e.currentTarget.style.borderColor='#c7d6fe'; e.currentTarget.style.boxShadow='0 4px 12px rgba(15,23,42,0.06)'; }}
      onMouseLeave={e => { e.currentTarget.style.borderColor='#e2e8f0'; e.currentTarget.style.boxShadow='none'; }}
    >
      <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
        <div style={{
          width: 44, height: 44, borderRadius: 12, flexShrink: 0,
          background: t.bg, color: t.fg,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 17, fontWeight: 700,
        }}>{role.name.charAt(0)}</div>

        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 2 }}>
            <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>{role.name}</div>
            {role.system && (
              <span style={{
                display: 'inline-flex', alignItems: 'center', gap: 4,
                padding: '2px 8px', borderRadius: 9999, fontSize: 11, fontWeight: 500,
                background: '#f1f5f9', color: '#475569',
              }}>
                <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>
                </svg>
                системная
              </span>
            )}
          </div>
          <div style={{ fontSize: 13, color: '#64748b', lineHeight: 1.5 }}>{role.description}</div>
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 4, flexShrink: 0, minWidth: 140 }}>
          <div style={{ fontSize: 13, color: '#0f172a', fontWeight: 500 }}>
            {granted} <span style={{ color: '#94a3b8', fontWeight: 400 }}>из {ALL_PERM_IDS.length} прав</span>
          </div>
          <div style={{ width: 140, height: 6, background: '#f1f5f9', borderRadius: 9999, overflow: 'hidden' }}>
            <div style={{
              width: `${pct}%`, height: '100%',
              background: 'linear-gradient(90deg, #6366f1, #818cf8)', borderRadius: 9999,
            }} />
          </div>
          <div style={{ fontSize: 11.5, color: '#64748b' }}>
            {role.members} {declRoles(role.members)}
          </div>
        </div>

        <Icon.ChevronRight size={18} stroke="#cbd5e1" style={{ flexShrink: 0 }} />
      </div>
    </a>
  );
}

function declRoles(n) {
  const a = Math.abs(n), m10 = a % 10, m100 = a % 100;
  if (m10 === 1 && m100 !== 11) return 'участник';
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return 'участника';
  return 'участников';
}

window.RolesListApp = RolesListApp;
