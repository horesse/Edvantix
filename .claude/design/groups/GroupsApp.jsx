// Groups page main app — учебные группы организации
const { useState: useStateG, useMemo: useMemoG, useEffect: useEffectG, useRef: useRefG } = React;

function GroupsApp() {
  const [query, setQuery] = useStateG('');
  const [levelFilter, setLevelFilter] = useStateG(new Set());
  const [statusFilter, setStatusFilter] = useStateG(new Set());
  const [formatFilter, setFormatFilter] = useStateG(new Set());
  const [view, setView] = useStateG('table'); // 'table' | 'cards'
  const [sort, setSort] = useStateG({ field: 'name', dir: 'asc' });
  const [selected, setSelected] = useStateG(new Set());

  const filtered = useMemoG(() => {
    const q = query.trim().toLowerCase();
    let list = GROUPS.filter(g => {
      if (q && !g.name.toLowerCase().includes(q)
          && !g.code.toLowerCase().includes(q)
          && !g.teacher.toLowerCase().includes(q)
          && !g.course.toLowerCase().includes(q)) return false;
      if (levelFilter.size && !levelFilter.has(g.level)) return false;
      if (statusFilter.size && !statusFilter.has(g.status)) return false;
      if (formatFilter.size && !formatFilter.has(g.format)) return false;
      return true;
    });
    list = [...list].sort((a, b) => {
      let av, bv;
      if      (sort.field === 'name')     { av = a.name; bv = b.name; }
      else if (sort.field === 'level')    { av = a.level; bv = b.level; }
      else if (sort.field === 'teacher')  { av = a.teacher; bv = b.teacher; }
      else if (sort.field === 'schedule') { av = a.schedule; bv = b.schedule; }
      else if (sort.field === 'students') { av = a.students / a.capacity; bv = b.students / b.capacity;
        return sort.dir === 'asc' ? av - bv : bv - av; }
      else                                { av = a.status; bv = b.status; }
      const c = String(av).localeCompare(String(bv), 'ru');
      return sort.dir === 'asc' ? c : -c;
    });
    return list;
  }, [query, levelFilter, statusFilter, formatFilter, sort]);

  const onSort = (field) => {
    setSort(s => s.field === field
      ? { field, dir: s.dir === 'asc' ? 'desc' : 'asc' }
      : { field, dir: 'asc' });
  };

  const kpi = useMemoG(() => {
    const active = GROUPS.filter(g => g.status === 'Active');
    const recruiting = GROUPS.filter(g => g.status === 'Recruiting');
    const totalSeats = GROUPS.reduce((a, g) => a + g.capacity, 0);
    const filledSeats = GROUPS.reduce((a, g) => a + g.students, 0);
    const totalStudents = active.reduce((a, g) => a + g.students, 0);
    return {
      total: GROUPS.length,
      active: active.length,
      recruiting: recruiting.length,
      students: totalStudents,
      load: Math.round(filledSeats / totalSeats * 100),
    };
  }, []);

  const toggleAll = () => {
    const ids = filtered.map(r => r.id);
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
  const allSelected = filtered.length > 0 && filtered.every(r => selected.has(r.id));
  const someSelected = filtered.some(r => selected.has(r.id));

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="groups" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Школа</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Группы</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '22px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 20,
        }}>
          <div style={{ flex: 1, minWidth: 0 }}>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
              Группы
            </h1>
            <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
              Учебные группы организации «Школа Эврика» — расписание, преподаватели, наполнение
            </div>
          </div>
          <Button variant="secondary"><Icon.FileText size={15} />Экспорт</Button>
          <Button><Icon.Plus size={16} />Создать группу</Button>
        </div>

        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 32px' }}>
          <div style={{ maxWidth: 1240, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 20 }}>

            {/* KPIs */}
            <div style={{
              display: 'grid', gap: 14,
              gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
            }}>
              <KpiBlock label="Всего групп" value={kpi.total}
                icon="Users" tone="slate" delta="за всё время" />
              <KpiBlock label="Идёт обучение" value={kpi.active}
                icon="CircleCheck" tone="success" delta={`${kpi.students} студентов`} />
              <KpiBlock label="Идёт набор" value={kpi.recruiting}
                icon="UserPlus" tone="primary" delta="открыты для записи" />
              <KpiBlock label="Заполненность" value={`${kpi.load}%`}
                icon="BarChart2" tone="warning" delta="по всем группам" />
            </div>

            {/* Toolbar + content card */}
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
                    placeholder="Поиск по названию, коду, преподавателю"
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
                  label="Уровень" icon="GraduationCap"
                  value={levelFilter} onChange={setLevelFilter}
                  options={GROUP_LEVELS.map(l => ({ value: l.value, label: l.label, swatch: LEVEL_TONES[l.tone].fg }))}
                />
                <FilterDropdown
                  label="Статус" icon="Filter"
                  value={statusFilter} onChange={setStatusFilter}
                  options={Object.entries(GROUP_STATUSES).map(([k, v]) => ({
                    value: k, label: v.label, swatch: v.dot,
                  }))}
                />
                <FilterDropdown
                  label="Формат" icon="School"
                  value={formatFilter} onChange={setFormatFilter}
                  options={Object.entries(GROUP_FORMATS).map(([k, v]) => ({
                    value: k, label: v.label, swatch: '#94a3b8',
                  }))}
                />

                {(levelFilter.size || statusFilter.size || formatFilter.size || query) ? (
                  <button
                    onClick={() => { setLevelFilter(new Set()); setStatusFilter(new Set()); setFormatFilter(new Set()); setQuery(''); }}
                    style={{
                      height: 36, padding: '0 12px', borderRadius: 10, border: 'none',
                      background: 'transparent', color: '#64748b', fontSize: 13,
                      fontFamily: 'inherit', cursor: 'pointer',
                    }}
                  >Сбросить</button>
                ) : null}

                <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: 12 }}>
                  <div style={{ fontSize: 12.5, color: '#64748b' }}>
                    Найдено:{' '}
                    <strong style={{ color: '#0f172a', fontWeight: 600, fontVariantNumeric: 'tabular-nums' }}>
                      {filtered.length}
                    </strong>
                    {filtered.length !== GROUPS.length && (
                      <span style={{ color: '#94a3b8' }}> из {GROUPS.length}</span>
                    )}
                  </div>
                  <ViewToggle view={view} onChange={setView} />
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
                  <button style={bulkBtnG}><Icon.UserPlus size={13} />Зачислить студентов</button>
                  <button style={bulkBtnG}><Icon.Calendar size={13} />Изменить расписание</button>
                  <button style={bulkBtnG}><Icon.FileText size={13} />Экспорт списков</button>
                  <button style={{ ...bulkBtnG, color: '#b91c1c' }}><Icon.X size={13} />Архивировать</button>
                  <button
                    onClick={() => setSelected(new Set())}
                    style={{ marginLeft: 'auto', ...bulkBtnG, color: '#64748b' }}
                  >Снять выделение</button>
                </div>
              )}

              {/* Content */}
              {filtered.length === 0 ? (
                <div style={{ padding: '60px 20px' }}>
                  <EmptyStateG onReset={() => { setQuery(''); setLevelFilter(new Set()); setStatusFilter(new Set()); setFormatFilter(new Set()); }} />
                </div>
              ) : view === 'table' ? (
                <GroupsTable
                  rows={filtered} sort={sort} onSort={onSort}
                  selected={selected} toggleAll={toggleAll} toggleOne={toggleOne}
                  allSelected={allSelected} someSelected={someSelected}
                />
              ) : (
                <GroupsCards rows={filtered} />
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

const bulkBtnG = {
  display: 'inline-flex', alignItems: 'center', gap: 6,
  height: 28, padding: '0 10px', borderRadius: 6, border: 'none',
  background: 'transparent', color: '#4338ca', fontSize: 12.5, fontWeight: 500,
  fontFamily: 'inherit', cursor: 'pointer',
};

// ── View toggle ──────────────────────────────────────────────────────
function ViewToggle({ view, onChange }) {
  const items = [
    { id: 'table', icon: 'FileText', label: 'Таблица' },
    { id: 'cards', icon: 'LayoutDashboard', label: 'Карточки' },
  ];
  return (
    <div style={{
      display: 'inline-flex', padding: 2, background: '#f1f5f9',
      borderRadius: 8, gap: 2,
    }}>
      {items.map(it => {
        const Ic = Icon[it.icon];
        const active = view === it.id;
        return (
          <button key={it.id} onClick={() => onChange(it.id)}
            title={it.label}
            style={{
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              width: 32, height: 28, borderRadius: 6, border: 'none',
              background: active ? '#fff' : 'transparent',
              boxShadow: active ? '0 1px 2px rgba(15,23,42,0.08)' : 'none',
              color: active ? '#0f172a' : '#64748b',
              cursor: 'pointer',
            }}>
            <Ic size={14} />
          </button>
        );
      })}
    </div>
  );
}

window.GroupsApp = GroupsApp;
