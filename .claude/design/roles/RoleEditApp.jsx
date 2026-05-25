// Role edit page
const { useState: useStateE2, useMemo: useMemoE2, useEffect: useEffectE2 } = React;

function RoleEditApp() {
  const roleId = new URLSearchParams(location.search).get('id') || 'admin';
  const original = ROLES.find(r => r.id === roleId) || ROLES[2];

  const [name, setName] = useStateE2(original.name);
  const [desc, setDesc] = useStateE2(original.description);
  const [perms, setPerms] = useStateE2(new Set(original.permissions));
  const [query, setQuery] = useStateE2('');
  const [collapsed, setCollapsed] = useStateE2(new Set());
  const [savingState, setSavingState] = useStateE2('idle');

  const hasChanges =
    name !== original.name ||
    desc !== original.description ||
    perms.size !== original.permissions.size ||
    [...perms].some(p => !original.permissions.has(p));

  useEffectE2(() => {
    const h = (e) => { if (hasChanges) { e.preventDefault(); e.returnValue = ''; } };
    window.addEventListener('beforeunload', h);
    return () => window.removeEventListener('beforeunload', h);
  }, [hasChanges]);

  const tone = TONE_COLORS[original.tone];
  const isSystemOwner = original.system && original.id === 'owner';

  const toggle = (permId) => {
    if (isSystemOwner) return;
    const n = new Set(perms);
    n.has(permId) ? n.delete(permId) : n.add(permId);
    setPerms(n);
  };

  const toggleFeature = (featId) => {
    if (isSystemOwner) return;
    const ids = PERMISSIONS[featId].map(p => p.id);
    const allOn = ids.every(id => perms.has(id));
    const n = new Set(perms);
    ids.forEach(id => allOn ? n.delete(id) : n.add(id));
    setPerms(n);
  };

  const toggleCollapse = (featId) => {
    const n = new Set(collapsed);
    n.has(featId) ? n.delete(featId) : n.add(featId);
    setCollapsed(n);
  };

  const reset = () => {
    setName(original.name);
    setDesc(original.description);
    setPerms(new Set(original.permissions));
  };

  const save = () => {
    setSavingState('saving');
    setTimeout(() => {
      original.name = name;
      original.description = desc;
      original.permissions = new Set(perms);
      setSavingState('saved');
    }, 800);
  };

  const q = query.trim().toLowerCase();
  const matchFeat = (feat) => {
    if (!q) return PERMISSIONS[feat.id];
    return PERMISSIONS[feat.id].filter(p =>
      p.label.toLowerCase().includes(q) || feat.label.toLowerCase().includes(q));
  };

  const totalGranted = perms.size;
  const pct = Math.round(totalGranted / ALL_PERM_IDS.length * 100);

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="settings" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0, position: 'relative' }}>
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Настройки</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Roles.html" style={{ color: '#4f46e5' }}>Роли и права</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>{original.name}</span>
        </div>

        <div style={{
          padding: '22px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 16,
        }}>
          <div style={{
            width: 48, height: 48, borderRadius: 12, flexShrink: 0,
            background: tone.bg, color: tone.fg,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            fontSize: 18, fontWeight: 700,
          }}>{name.charAt(0)}</div>
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <h1 style={{ margin: 0, fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em' }}>
                {name}
              </h1>
              {original.system && (
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
            <div style={{ fontSize: 13, color: '#64748b', marginTop: 2 }}>
              {original.members} участников · {totalGranted} из {ALL_PERM_IDS.length} прав ({pct}%)
            </div>
          </div>
          {!original.system && (
            <Button variant="secondary" style={{ color: '#b91c1c', borderColor: '#fecaca' }}>
              <Icon.X size={15} />Удалить роль
            </Button>
          )}
        </div>

        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 120px' }}>
          <div style={{ maxWidth: 880, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 20 }}>

            {isSystemOwner && (
              <div style={{
                padding: '12px 16px', borderRadius: 12,
                background: 'rgba(245,158,11,0.08)', border: '1px solid rgba(245,158,11,0.25)',
                display: 'flex', gap: 10, alignItems: 'flex-start',
              }}>
                <Icon.Info size={16} stroke="#92400e" style={{ flexShrink: 0, marginTop: 1 }} />
                <div style={{ fontSize: 13, color: '#78350f', lineHeight: 1.5 }}>
                  Роль «Владелец» имеет полный доступ ко всем разделам системы и не может быть изменена.
                </div>
              </div>
            )}

            <section style={{ background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16, padding: '20px 22px' }}>
              <h2 style={{ margin: '0 0 14px', fontSize: 14, fontWeight: 600 }}>Основные сведения</h2>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                <F.Field label="Название роли" required>
                  <F.Text value={name} onChange={e => setName(e.target.value)} disabled={isSystemOwner} />
                </F.Field>
                <F.Field label="Описание" hint="Короткое пояснение — кому назначается эта роль">
                  <F.Textarea value={desc} onChange={e => setDesc(e.target.value)} disabled={isSystemOwner} />
                </F.Field>
              </div>
            </section>

            <section style={{ background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16, overflow: 'hidden' }}>
              <header style={{
                padding: '16px 22px', borderBottom: '1px solid #f1f5f9',
                display: 'flex', alignItems: 'center', gap: 14,
              }}>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <h2 style={{ margin: 0, fontSize: 14, fontWeight: 600 }}>Права доступа</h2>
                  <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>
                    Отметьте, что может делать участник с этой ролью
                  </div>
                </div>
                <div style={{ position: 'relative', width: 260, height: 34 }}>
                  <Icon.Search size={14} stroke="#94a3b8"
                    style={{ position: 'absolute', left: 11, top: 10, pointerEvents: 'none' }} />
                  <input
                    value={query}
                    onChange={e => setQuery(e.target.value)}
                    placeholder="Поиск по правам"
                    style={{
                      width: '100%', height: 34, paddingLeft: 32, paddingRight: 12,
                      borderRadius: 8, border: '1px solid #e2e8f0', background: '#fff',
                      fontSize: 13, fontFamily: 'inherit', outline: 'none',
                    }}
                    onFocus={e => { e.target.style.borderColor='#6366f1'; e.target.style.boxShadow='0 0 0 3px rgba(99,102,241,0.2)'; }}
                    onBlur={e => { e.target.style.borderColor='#e2e8f0'; e.target.style.boxShadow='none'; }}
                  />
                </div>
              </header>

              <div>
                {FEATURES.map(feat => {
                  const visible = matchFeat(feat);
                  if (q && visible.length === 0) return null;
                  const all = PERMISSIONS[feat.id];
                  const granted = all.filter(p => perms.has(p.id)).length;
                  const allOn = granted === all.length;
                  const someOn = granted > 0 && !allOn;
                  const IC = Icon[feat.icon];
                  const isCollapsed = collapsed.has(feat.id);

                  return (
                    <div key={feat.id} style={{ borderTop: '1px solid #f1f5f9' }}>
                      <div style={{
                        padding: '14px 22px',
                        display: 'flex', alignItems: 'center', gap: 14,
                        background: '#fafbfc',
                      }}>
                        <button
                          onClick={() => toggleCollapse(feat.id)}
                          style={{
                            width: 22, height: 22, borderRadius: 6, border: 'none',
                            background: 'transparent', color: '#94a3b8', cursor: 'pointer',
                            display: 'flex', alignItems: 'center', justifyContent: 'center',
                            transform: isCollapsed ? 'rotate(-90deg)' : 'rotate(0)',
                            transition: 'transform .15s',
                          }}
                        ><Icon.ChevronDown size={16} /></button>
                        <div style={{
                          width: 32, height: 32, borderRadius: 8, flexShrink: 0,
                          background: 'rgba(79,70,229,0.08)', color: '#4f46e5',
                          display: 'flex', alignItems: 'center', justifyContent: 'center',
                        }}><IC size={16} stroke="#4f46e5" /></div>
                        <div style={{ flex: 1, minWidth: 0 }}>
                          <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>
                            {feat.label}
                          </div>
                          <div style={{ fontSize: 12, color: '#64748b', marginTop: 1 }}>
                            {feat.desc}
                          </div>
                        </div>
                        <div style={{
                          fontSize: 12, color: '#64748b', fontVariantNumeric: 'tabular-nums', minWidth: 48, textAlign: 'right',
                        }}>{granted} / {all.length}</div>
                        <Toggle
                          checked={allOn}
                          indeterminate={someOn}
                          onChange={() => toggleFeature(feat.id)}
                          disabled={isSystemOwner}
                        />
                      </div>

                      {!isCollapsed && (
                        <div style={{ padding: '4px 22px 14px 70px' }}>
                          {visible.map(p => {
                            const on = perms.has(p.id);
                            return (
                              <label key={p.id} style={{
                                display: 'flex', alignItems: 'center', gap: 12,
                                padding: '8px 10px', borderRadius: 8, cursor: isSystemOwner ? 'not-allowed' : 'pointer',
                                transition: 'background .1s',
                              }}
                                onMouseEnter={e => { if (!isSystemOwner) e.currentTarget.style.background = '#f8fafc'; }}
                                onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
                              >
                                <Checkbox2 checked={on} onChange={() => toggle(p.id)} disabled={isSystemOwner} />
                                <span style={{
                                  fontSize: 13, color: on ? '#0f172a' : '#475569',
                                  fontWeight: on ? 500 : 400,
                                }}>{p.label}</span>
                                <code style={{
                                  marginLeft: 'auto', fontFamily: 'var(--edv-font-mono)',
                                  fontSize: 11, color: '#94a3b8', background: '#f8fafc',
                                  padding: '1px 8px', borderRadius: 4,
                                }}>{p.id}</code>
                              </label>
                            );
                          })}
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            </section>
          </div>
        </div>

        <RoleSaveBar
          hasChanges={hasChanges && !isSystemOwner}
          saving={savingState === 'saving'}
          onSave={save}
          onReset={reset}
        />
      </div>
    </div>
  );
}

function Toggle({ checked, indeterminate, onChange, disabled }) {
  const bg = disabled ? '#e2e8f0' : checked ? '#4f46e5' : indeterminate ? '#818cf8' : '#cbd5e1';
  return (
    <button
      onClick={onChange}
      disabled={disabled}
      style={{
        width: 40, height: 22, borderRadius: 9999, border: 'none',
        background: bg, position: 'relative', cursor: disabled ? 'not-allowed' : 'pointer',
        transition: 'background .15s', flexShrink: 0,
      }}
    >
      <span style={{
        position: 'absolute', top: 2, left: (checked || indeterminate) ? 20 : 2,
        width: 18, height: 18, borderRadius: 9999, background: '#fff',
        boxShadow: '0 1px 3px rgba(0,0,0,0.15)', transition: 'left .15s',
      }} />
      {indeterminate && !checked && (
        <span style={{
          position: 'absolute', top: 9, left: 26, width: 6, height: 2,
          background: '#4338ca', borderRadius: 1,
        }} />
      )}
    </button>
  );
}

function Checkbox2({ checked, onChange, disabled }) {
  return (
    <span
      onClick={disabled ? undefined : onChange}
      style={{
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        width: 18, height: 18, borderRadius: 5,
        border: `1.5px solid ${checked ? '#4f46e5' : '#cbd5e1'}`,
        background: checked ? '#4f46e5' : '#fff',
        cursor: disabled ? 'not-allowed' : 'pointer', flexShrink: 0,
        opacity: disabled ? 0.6 : 1,
      }}
    >{checked && <Icon.Check size={12} stroke="#fff" sw={3} />}</span>
  );
}

function RoleSaveBar({ hasChanges, saving, onSave, onReset }) {
  const visible = hasChanges || saving;
  return (
    <div style={{
      position: 'absolute', left: 0, right: 0, bottom: 0,
      transform: visible ? 'translateY(0)' : 'translateY(100%)',
      transition: 'transform .25s cubic-bezier(.4,0,.2,1)',
      background: '#fff', borderTop: '1px solid #e2e8f0',
      boxShadow: '0 -4px 12px rgba(15,23,42,0.06)',
      padding: '14px 32px',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 20,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, fontSize: 13 }}>
        <span style={{ width: 8, height: 8, borderRadius: 9999, background: '#f59e0b' }} />
        <strong style={{ color: '#0f172a' }}>Несохранённые изменения</strong>
        <span style={{ color: '#64748b' }}>— сохраните, чтобы применить</span>
      </div>
      <div style={{ display: 'flex', gap: 10 }}>
        <Button variant="ghost" onClick={onReset} disabled={saving}>Отменить</Button>
        <Button onClick={onSave} disabled={saving}>
          {saving ? (<><span style={{ display:'inline-block', width:14, height:14, border:'2px solid rgba(255,255,255,0.35)', borderTopColor:'#fff', borderRadius:9999, animation:'spin .7s linear infinite' }}/>Сохранение…</>)
            : (<><Icon.Check size={16} sw={2.5} />Сохранить</>)}
        </Button>
      </div>
    </div>
  );
}

window.RoleEditApp = RoleEditApp;
