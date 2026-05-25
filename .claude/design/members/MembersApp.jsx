// Members page main app
function MembersApp() {
  const [query, setQuery] = useStateM('');
  const [roleFilter, setRoleFilter] = useStateM(new Set());
  const [statusFilter, setStatusFilter] = useStateM(new Set());
  const [sort, setSort] = useStateM({ field: 'name', dir: 'asc' });
  const [selected, setSelected] = useStateM(new Set());
  const [page, setPage] = useStateM(1);
  const pageSize = 10;

  const onSort = (field) => {
    setSort(s => s.field === field
      ? { field, dir: s.dir === 'asc' ? 'desc' : 'asc' }
      : { field, dir: 'asc' });
  };

  const filtered = useMemoM(() => {
    const q = query.trim().toLowerCase();
    let list = MEMBERS.filter(m => {
      if (q && !m.name.toLowerCase().includes(q) && !m.email.toLowerCase().includes(q)) return false;
      if (roleFilter.size && !roleFilter.has(m.role)) return false;
      if (statusFilter.size && !statusFilter.has(m.status)) return false;
      return true;
    });
    list = [...list].sort((a, b) => {
      let av, bv;
      if (sort.field === 'name')      { av = a.name; bv = b.name; }
      else if (sort.field === 'role') { av = a.role; bv = b.role; }
      else if (sort.field === 'status'){ av = a.status; bv = b.status; }
      else                            { av = a.lastActive; bv = b.lastActive; }
      const c = String(av).localeCompare(String(bv), 'ru');
      return sort.dir === 'asc' ? c : -c;
    });
    return list;
  }, [query, roleFilter, statusFilter, sort]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
  const currentPage = Math.min(page, totalPages);
  const pageRows = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  React.useEffect(() => { setPage(1); }, [query, roleFilter, statusFilter]);

  // KPIs (based on full list, not filtered)
  const kpi = useMemoM(() => ({
    total: MEMBERS.length,
    active: MEMBERS.filter(m => m.status === 'Active').length,
    invited: MEMBERS.filter(m => m.status === 'Invited').length,
    blocked: MEMBERS.filter(m => m.status === 'Blocked').length,
  }), []);

  const toggleAll = () => {
    const ids = pageRows.map(r => r.id);
    const allSelected = ids.every(id => selected.has(id));
    const n = new Set(selected);
    ids.forEach(id => allSelected ? n.delete(id) : n.add(id));
    setSelected(n);
  };
  const toggleOne = (id) => {
    const n = new Set(selected);
    n.has(id) ? n.delete(id) : n.add(id);
    setSelected(n);
  };
  const allOnPageSelected = pageRows.length > 0 && pageRows.every(r => selected.has(r.id));
  const someOnPageSelected = pageRows.some(r => selected.has(r.id));

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
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Участники</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '22px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 20,
        }}>
          <div style={{ flex: 1, minWidth: 0 }}>
            <h1 style={{
              margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em',
            }}>Участники</h1>
            <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
              Сотрудники и роли в организации «Школа Эврика»
            </div>
          </div>
          <Button variant="secondary">
            <Icon.FileText size={15} />Экспорт
          </Button>
          <Button>
            <Icon.UserPlus size={16} />Пригласить участника
          </Button>
        </div>

        {/* Scrollable body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 32px' }}>
          <div style={{ maxWidth: 1200, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 20 }}>

            {/* KPIs */}
            <div style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
              gap: 14,
            }}>
              <KpiBlock label="Всего"         value={kpi.total}   icon="Users"       tone="slate"
                delta="+3 за месяц" />
              <KpiBlock label="Активные"      value={kpi.active}  icon="CircleCheck" tone="success"
                delta={`${Math.round(kpi.active / kpi.total * 100)}% от всех`} />
              <KpiBlock label="Приглашения"   value={kpi.invited} icon="Send"        tone="primary"
                delta="ждут ответа" />
              <KpiBlock label="Заблокированы" value={kpi.blocked} icon="Shield"      tone="warning"
                delta="требуют внимания" />
            </div>

            {/* Toolbar + table card */}
            <div style={{
              background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
              overflow: 'hidden',
            }}>
              {/* Toolbar */}
              <div style={{
                padding: '14px 16px', borderBottom: '1px solid #f1f5f9',
                display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap',
              }}>
                <div style={{ position: 'relative', flex: '1 1 280px', maxWidth: 360, height: 36 }}>
                  <Icon.Search size={15} stroke="#94a3b8"
                    style={{ position: 'absolute', left: 12, top: 11, pointerEvents: 'none' }} />
                  <input
                    value={query}
                    onChange={e => setQuery(e.target.value)}
                    placeholder="Поиск по имени или email"
                    style={{
                      width: '100%', height: 36, paddingLeft: 34, paddingRight: query ? 32 : 12,
                      borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
                      fontSize: 13, fontFamily: 'inherit', color: '#0f172a', outline: 'none',
                      transition: 'border-color .15s, box-shadow .15s',
                    }}
                    onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.2)'; }}
                    onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }}
                  />
                  {query && (
                    <button onClick={() => setQuery('')}
                      style={{
                        position: 'absolute', right: 8, top: 8, width: 20, height: 20,
                        borderRadius: 9999, background: '#f1f5f9', border: 'none',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        cursor: 'pointer', color: '#64748b',
                      }}><Icon.X size={12} /></button>
                  )}
                </div>

                <FilterDropdown
                  label="Роль"
                  icon="Briefcase"
                  value={roleFilter}
                  onChange={setRoleFilter}
                  options={MEMBER_ROLES.map(r => ({ value: r.value, label: r.label, swatch: ROLE_TONES[r.tone].fg }))}
                />
                <FilterDropdown
                  label="Статус"
                  icon="Filter"
                  value={statusFilter}
                  onChange={setStatusFilter}
                  options={Object.entries(MEMBER_STATUSES).map(([k, v]) => ({
                    value: k, label: v.label, swatch: v.dot,
                  }))}
                />

                {(roleFilter.size || statusFilter.size || query) ? (
                  <button
                    onClick={() => { setRoleFilter(new Set()); setStatusFilter(new Set()); setQuery(''); }}
                    style={{
                      height: 36, padding: '0 12px', borderRadius: 10, border: 'none',
                      background: 'transparent', color: '#64748b', fontSize: 13,
                      fontFamily: 'inherit', cursor: 'pointer',
                    }}
                  >Сбросить</button>
                ) : null}

                <div style={{ marginLeft: 'auto', fontSize: 12.5, color: '#64748b' }}>
                  Найдено:{' '}
                  <strong style={{ color: '#0f172a', fontWeight: 600, fontVariantNumeric: 'tabular-nums' }}>
                    {filtered.length}
                  </strong>
                  {filtered.length !== MEMBERS.length && (
                    <span style={{ color: '#94a3b8' }}> из {MEMBERS.length}</span>
                  )}
                </div>
              </div>

              {/* Selection bar */}
              {selected.size > 0 && (
                <div style={{
                  padding: '10px 16px', borderBottom: '1px solid #e0eaff',
                  background: 'rgba(79,70,229,0.05)', display: 'flex',
                  alignItems: 'center', gap: 12,
                }}>
                  <span style={{ fontSize: 13, color: '#4338ca', fontWeight: 500 }}>
                    Выбрано: {selected.size}
                  </span>
                  <div style={{ width: 1, height: 16, background: '#c7d6fe' }} />
                  <button style={bulkBtn}><Icon.Send size={13} />Отправить приглашение</button>
                  <button style={bulkBtn}><Icon.Shield size={13} />Заблокировать</button>
                  <button style={{ ...bulkBtn, color: '#b91c1c' }}><Icon.X size={13} />Удалить</button>
                  <button
                    onClick={() => setSelected(new Set())}
                    style={{ marginLeft: 'auto', ...bulkBtn, color: '#64748b' }}
                  >Снять выделение</button>
                </div>
              )}

              {/* Table */}
              <div style={{ overflowX: 'auto' }}>
                <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13.5 }}>
                  <thead>
                    <tr>
                      <th style={{
                        padding: '12px 12px 12px 20px', width: 40,
                        background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
                      }}>
                        <Checkbox
                          checked={allOnPageSelected}
                          indeterminate={!allOnPageSelected && someOnPageSelected}
                          onChange={toggleAll}
                        />
                      </th>
                      <SortHeader field="name"   sort={sort} onSort={onSort}>ФИО</SortHeader>
                      <SortHeader field="role"   sort={sort} onSort={onSort} width={200}>Роль</SortHeader>
                      <SortHeader field="status" sort={sort} onSort={onSort} width={160}>Статус</SortHeader>
                      <SortHeader field="lastActive" sort={sort} onSort={onSort} width={170}>Активность</SortHeader>
                      <th style={{
                        width: 60, background: '#f8fafc',
                        borderBottom: '1px solid #e2e8f0',
                      }} />
                    </tr>
                  </thead>
                  <tbody>
                    {pageRows.length === 0 ? (
                      <tr>
                        <td colSpan={6} style={{ padding: '60px 20px', textAlign: 'center' }}>
                          <EmptyState onReset={() => { setQuery(''); setRoleFilter(new Set()); setStatusFilter(new Set()); }} />
                        </td>
                      </tr>
                    ) : pageRows.map(m => (
                      <tr key={m.id} style={{
                        borderBottom: '1px solid #f1f5f9',
                        background: selected.has(m.id) ? 'rgba(79,70,229,0.03)' : 'transparent',
                        transition: 'background .1s',
                      }}
                        onMouseEnter={e => { if (!selected.has(m.id)) e.currentTarget.style.background = '#fafbfc'; }}
                        onMouseLeave={e => { if (!selected.has(m.id)) e.currentTarget.style.background = 'transparent'; }}
                      >
                        <td style={{ padding: '12px 12px 12px 20px' }}>
                          <Checkbox checked={selected.has(m.id)} onChange={() => toggleOne(m.id)} />
                        </td>
                        <td style={{ padding: '12px 16px' }}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                            <Avatar name={m.name} size={36} />
                            <div style={{ minWidth: 0 }}>
                              <div style={{ fontSize: 13.5, fontWeight: 500, color: '#0f172a' }}>
                                {m.name}
                              </div>
                              <div style={{
                                fontSize: 12, color: '#64748b', marginTop: 1,
                                overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
                              }}>{m.email}</div>
                            </div>
                          </div>
                        </td>
                        <td style={{ padding: '12px 16px' }}><RoleTag role={m.role} /></td>
                        <td style={{ padding: '12px 16px' }}><StatusPill status={m.status} /></td>
                        <td style={{ padding: '12px 16px', color: '#475569', fontSize: 13 }}>
                          {m.lastActive}
                        </td>
                        <td style={{ padding: '12px 12px 12px 8px', textAlign: 'right' }}>
                          <RowMenu status={m.status} />
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Pagination */}
              {filtered.length > 0 && (
                <Pagination
                  total={filtered.length}
                  page={currentPage}
                  totalPages={totalPages}
                  pageSize={pageSize}
                  onPage={setPage}
                />
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

const bulkBtn = {
  display: 'inline-flex', alignItems: 'center', gap: 6,
  height: 28, padding: '0 10px', borderRadius: 6, border: 'none',
  background: 'transparent', color: '#4338ca', fontSize: 12.5, fontWeight: 500,
  fontFamily: 'inherit', cursor: 'pointer',
};

// ── Checkbox ─────────────────────────────────────────────────────────
function Checkbox({ checked, indeterminate, onChange }) {
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

// ── Pagination ───────────────────────────────────────────────────────
function Pagination({ total, page, totalPages, pageSize, onPage }) {
  const from = (page - 1) * pageSize + 1;
  const to = Math.min(page * pageSize, total);

  const pages = [];
  const add = (n) => pages.push(n);
  if (totalPages <= 7) {
    for (let i = 1; i <= totalPages; i++) add(i);
  } else {
    add(1);
    if (page > 3) add('…');
    for (let i = Math.max(2, page - 1); i <= Math.min(totalPages - 1, page + 1); i++) add(i);
    if (page < totalPages - 2) add('…');
    add(totalPages);
  }

  return (
    <div style={{
      padding: '12px 16px', borderTop: '1px solid #f1f5f9',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 16,
      fontSize: 13,
    }}>
      <div style={{ color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>
        Показано <strong style={{ color: '#0f172a' }}>{from}–{to}</strong> из{' '}
        <strong style={{ color: '#0f172a' }}>{total}</strong>
      </div>
      <div style={{ display: 'flex', gap: 4 }}>
        <PageBtn disabled={page === 1} onClick={() => onPage(page - 1)}>
          <Icon.ArrowLeft size={14} />
        </PageBtn>
        {pages.map((p, i) => p === '…' ? (
          <span key={i} style={{ padding: '0 6px', color: '#94a3b8', alignSelf: 'center' }}>…</span>
        ) : (
          <PageBtn key={i} active={p === page} onClick={() => onPage(p)}>{p}</PageBtn>
        ))}
        <PageBtn disabled={page === totalPages} onClick={() => onPage(page + 1)}>
          <Icon.ArrowRight size={14} />
        </PageBtn>
      </div>
    </div>
  );
}

function PageBtn({ active, disabled, onClick, children }) {
  return (
    <button
      disabled={disabled}
      onClick={onClick}
      style={{
        minWidth: 32, height: 32, padding: '0 10px',
        borderRadius: 8, border: `1px solid ${active ? '#4f46e5' : 'transparent'}`,
        background: active ? '#4f46e5' : 'transparent',
        color: active ? '#fff' : disabled ? '#cbd5e1' : '#334155',
        fontSize: 13, fontWeight: active ? 600 : 500, fontFamily: 'inherit',
        cursor: disabled ? 'not-allowed' : 'pointer',
        fontVariantNumeric: 'tabular-nums',
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      }}
      onMouseEnter={e => { if (!active && !disabled) e.currentTarget.style.background = '#f1f5f9'; }}
      onMouseLeave={e => { if (!active) e.currentTarget.style.background = 'transparent'; }}
    >{children}</button>
  );
}

// ── Empty state ──────────────────────────────────────────────────────
function EmptyState({ onReset }) {
  return (
    <div style={{
      display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 10,
      padding: '20px', color: '#64748b',
    }}>
      <div style={{
        width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Search size={24} stroke="#94a3b8" />
      </div>
      <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>
        Никого не нашли
      </div>
      <div style={{ fontSize: 13, color: '#64748b', maxWidth: 340, textAlign: 'center' }}>
        Попробуйте другой запрос или сбросьте фильтры.
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

window.MembersApp = MembersApp;
