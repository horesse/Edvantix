// Courses page main app — каталог курсов преподавателя
const { useState: useStateC, useMemo: useMemoC } = React;

function CoursesApp() {
  const [query, setQuery] = useStateC('');
  const [subjectFilter, setSubjectFilter] = useStateC(new Set());
  const [levelFilter, setLevelFilter] = useStateC(new Set());
  const [statusFilter, setStatusFilter] = useStateC(new Set());
  const [view, setView] = useStateC('table'); // 'table' | 'cards'
  const [grouped, setGrouped] = useStateC(true);
  const [sort, setSort] = useStateC({ field: 'name', dir: 'asc' });
  const [selected, setSelected] = useStateC(new Set());
  const [expanded, setExpanded] = useStateC(new Set(Object.keys(COURSE_SUBJECTS)));

  const filtered = useMemoC(() => {
    const q = query.trim().toLowerCase();
    let list = COURSES.filter(c => {
      if (q && !c.name.toLowerCase().includes(q)
          && !c.code.toLowerCase().includes(q)) return false;
      if (subjectFilter.size && !subjectFilter.has(c.subject)) return false;
      if (levelFilter.size && !levelFilter.has(c.level)) return false;
      if (statusFilter.size && !statusFilter.has(c.status)) return false;
      return true;
    });
    list = [...list].sort((a, b) => {
      let av, bv;
      if      (sort.field === 'name')     { av = a.name; bv = b.name; }
      else if (sort.field === 'level')    { av = a.level; bv = b.level; }
      else if (sort.field === 'duration') { av = a.lessons; bv = b.lessons;
        return sort.dir === 'asc' ? av - bv : bv - av; }
      else if (sort.field === 'groups')   { av = a.groups; bv = b.groups;
        return sort.dir === 'asc' ? av - bv : bv - av; }
      else                                { av = a.status; bv = b.status; }
      const cm = String(av).localeCompare(String(bv), 'ru');
      return sort.dir === 'asc' ? cm : -cm;
    });
    return list;
  }, [query, subjectFilter, levelFilter, statusFilter, sort]);

  const onSort = (field) => {
    setSort(s => s.field === field
      ? { field, dir: s.dir === 'asc' ? 'desc' : 'asc' }
      : { field, dir: 'asc' });
  };

  const kpi = useMemoC(() => {
    const active = COURSES.filter(c => c.status === 'Active').length;
    const drafts = COURSES.filter(c => c.status === 'Draft' || c.status === 'Review').length;
    const groups = COURSES.reduce((a, c) => a + c.groups, 0);
    const students = COURSES.reduce((a, c) => a + c.students, 0);
    return { total: COURSES.length, active, drafts, groups, students };
  }, []);

  const toggleAll = () => {
    const ids = filtered.map(r => r.id);
    const all = ids.every(id => selected.has(id));
    const n = new Set(selected);
    ids.forEach(id => all ? n.delete(id) : n.add(id));
    setSelected(n);
  };
  const toggleOne = (id) => {
    const n = new Set(selected);
    n.has(id) ? n.delete(id) : n.add(id);
    setSelected(n);
  };
  const toggleGroup = (key) => {
    const n = new Set(expanded);
    n.has(key) ? n.delete(key) : n.add(key);
    setExpanded(n);
  };
  const allSelected = filtered.length > 0 && filtered.every(r => selected.has(r.id));
  const someSelected = filtered.some(r => selected.has(r.id));

  // Tweaks
  const [tweaksOpen, setTweaksOpen] = useStateC(false);
  React.useEffect(() => {
    const onMsg = (e) => {
      if (e.data?.type === '__activate_edit_mode')   setTweaksOpen(true);
      if (e.data?.type === '__deactivate_edit_mode') setTweaksOpen(false);
    };
    window.addEventListener('message', onMsg);
    window.parent.postMessage({ type: '__edit_mode_available' }, '*');
    return () => window.removeEventListener('message', onMsg);
  }, []);

  return (
    <div style={{ display: 'flex', height: '100vh', minHeight: 700, background: '#f8fafc', overflow: 'hidden' }}>
      <Sidebar active="courses" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Школа</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Курсы</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '22px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 20,
        }}>
          <div style={{ flex: 1, minWidth: 0 }}>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
              Курсы
            </h1>
            <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
              Учебные программы со списком занятий и материалов — основа для создания групп
            </div>
          </div>
          <Button variant="secondary"><Icon.FileText size={15} />Импорт</Button>
          <Button><Icon.Plus size={16} />Создать курс</Button>
        </div>

        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 32px' }}>
          <div style={{ maxWidth: 1240, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 20 }}>

            {/* KPIs */}
            <div style={{
              display: 'grid', gap: 14,
              gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
            }}>
              <CourseKpi label="Всего курсов" value={kpi.total} icon="BookOpen" tone="slate" delta={`${kpi.active} активных`} />
              <CourseKpi label="В разработке" value={kpi.drafts} icon="FileText" tone="warning" delta="черновики и проверка" />
              <CourseKpi label="Групп на курсах" value={kpi.groups} icon="Users" tone="primary" delta="используют программы" />
              <CourseKpi label="Студентов охвачено" value={kpi.students} icon="GraduationCap" tone="success" delta="по активным курсам" />
            </div>

            {/* Toolbar + content card */}
            <div style={{ background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16, overflow: 'hidden' }}>

              {/* Toolbar */}
              <div style={{
                padding: '14px 16px', borderBottom: '1px solid #f1f5f9',
                display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap',
              }}>
                <div style={{ position: 'relative', flex: '1 1 280px', maxWidth: 360, height: 36 }}>
                  <Icon.Search size={15} stroke="#94a3b8"
                    style={{ position: 'absolute', left: 12, top: 11, pointerEvents: 'none' }} />
                  <input value={query} onChange={e => setQuery(e.target.value)}
                    placeholder="Поиск по названию или коду курса"
                    style={{
                      width: '100%', height: 36, paddingLeft: 34, paddingRight: query ? 32 : 12,
                      borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
                      fontSize: 13, fontFamily: 'inherit', color: '#0f172a', outline: 'none',
                      transition: 'border-color .15s, box-shadow .15s',
                    }}
                    onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.2)'; }}
                    onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }} />
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

                <CourseFilterDropdown
                  label="Предмет" icon="BookOpen"
                  value={subjectFilter} onChange={setSubjectFilter}
                  options={Object.entries(COURSE_SUBJECTS).map(([k, v]) => ({
                    value: k, label: v.label, swatch: SUBJECT_TONES[v.tone].fg,
                  }))}
                />
                <CourseFilterDropdown
                  label="Уровень" icon="GraduationCap"
                  value={levelFilter} onChange={setLevelFilter}
                  options={COURSE_LEVELS.map(l => ({ value: l.value, label: l.label, swatch: '#94a3b8' }))}
                />
                <CourseFilterDropdown
                  label="Статус" icon="Filter"
                  value={statusFilter} onChange={setStatusFilter}
                  options={Object.entries(COURSE_STATUSES).map(([k, v]) => ({
                    value: k, label: v.label, swatch: v.dot,
                  }))}
                />

                {(subjectFilter.size || levelFilter.size || statusFilter.size || query) ? (
                  <button onClick={() => { setSubjectFilter(new Set()); setLevelFilter(new Set()); setStatusFilter(new Set()); setQuery(''); }}
                    style={{
                      height: 36, padding: '0 12px', borderRadius: 10, border: 'none',
                      background: 'transparent', color: '#64748b', fontSize: 13,
                      fontFamily: 'inherit', cursor: 'pointer',
                    }}>Сбросить</button>
                ) : null}

                <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: 12 }}>
                  <div style={{ fontSize: 12.5, color: '#64748b' }}>
                    Найдено:{' '}
                    <strong style={{ color: '#0f172a', fontWeight: 600, fontVariantNumeric: 'tabular-nums' }}>
                      {filtered.length}
                    </strong>
                    {filtered.length !== COURSES.length && (
                      <span style={{ color: '#94a3b8' }}> из {COURSES.length}</span>
                    )}
                  </div>
                  <CourseViewToggle view={view} onChange={setView} />
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
                  <button style={bulkBtnC}><Icon.Sparkles size={13} />Дублировать</button>
                  <button style={bulkBtnC}><Icon.UserPlus size={13} />Создать группу</button>
                  <button style={bulkBtnC}><Icon.FileText size={13} />Экспорт</button>
                  <button style={{ ...bulkBtnC, color: '#b91c1c' }}><Icon.X size={13} />Архивировать</button>
                  <button onClick={() => setSelected(new Set())}
                    style={{ marginLeft: 'auto', ...bulkBtnC, color: '#64748b' }}>Снять выделение</button>
                </div>
              )}

              {/* Content */}
              {filtered.length === 0 ? (
                <CoursesEmpty onReset={() => { setQuery(''); setSubjectFilter(new Set()); setLevelFilter(new Set()); setStatusFilter(new Set()); }} />
              ) : view === 'table' ? (
                <CoursesTable rows={filtered} sort={sort} onSort={onSort}
                  selected={selected} toggleAll={toggleAll} toggleOne={toggleOne}
                  allSelected={allSelected} someSelected={someSelected}
                  grouped={grouped} expanded={expanded} onToggleGroup={toggleGroup} />
              ) : (
                <CoursesCards rows={filtered}
                  grouped={grouped} expanded={expanded} onToggleGroup={toggleGroup} />
              )}

              {/* Pagination footer */}
              {filtered.length > 0 && (
                <div style={{
                  padding: '12px 20px', borderTop: '1px solid #f1f5f9',
                  display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                  fontSize: 12.5, color: '#64748b',
                }}>
                  <div style={{ fontVariantNumeric: 'tabular-nums' }}>
                    Показано <strong style={{ color: '#0f172a', fontWeight: 600 }}>1–{filtered.length}</strong> из {COURSES.length}
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
                    <button style={pgBtn} disabled><Icon.ArrowLeft size={13} /></button>
                    <button style={{ ...pgBtn, background: '#4f46e5', color: '#fff', border: '1px solid #4f46e5' }}>1</button>
                    <button style={pgBtn}>2</button>
                    <button style={pgBtn}>3</button>
                    <span style={{ padding: '0 6px', color: '#94a3b8' }}>…</span>
                    <button style={pgBtn}>8</button>
                    <button style={pgBtn}><Icon.ArrowRight size={13} /></button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Tweaks panel */}
        {tweaksOpen && (
          <div style={{
            position: 'fixed', right: 20, bottom: 20, width: 280, zIndex: 50,
            background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
            boxShadow: '0 10px 40px rgba(15,23,42,0.18)', padding: 14,
            fontFamily: 'var(--edv-font-sans)',
          }}>
            <div style={{ display: 'flex', alignItems: 'center', marginBottom: 10 }}>
              <strong style={{ fontSize: 13, color: '#0f172a' }}>Tweaks</strong>
              <button onClick={() => {
                setTweaksOpen(false);
                window.parent.postMessage({ type: '__edit_mode_dismissed' }, '*');
              }} style={{
                marginLeft: 'auto', width: 24, height: 24, borderRadius: 6,
                border: 'none', background: '#f1f5f9', cursor: 'pointer',
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              }}><Icon.X size={12} stroke="#64748b" /></button>
            </div>

            <div style={{ fontSize: 11, fontWeight: 600, color: '#94a3b8',
              textTransform: 'uppercase', letterSpacing: '0.08em', marginBottom: 6 }}>Отображение</div>
            <div style={{ display: 'flex', gap: 4, padding: 3, background: '#f1f5f9',
              borderRadius: 8, marginBottom: 12 }}>
              {[{id:'table',l:'Таблица'},{id:'cards',l:'Карточки'}].map(o => (
                <button key={o.id} onClick={() => setView(o.id)}
                  style={{
                    flex: 1, height: 28, border: 'none', borderRadius: 6,
                    background: view === o.id ? '#fff' : 'transparent',
                    boxShadow: view === o.id ? '0 1px 2px rgba(15,23,42,0.08)' : 'none',
                    fontSize: 12, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
                    color: view === o.id ? '#0f172a' : '#64748b',
                  }}>{o.l}</button>
              ))}
            </div>

            <label style={{ display: 'flex', alignItems: 'center', gap: 10, cursor: 'pointer' }}>
              <span style={{ fontSize: 13, color: '#0f172a', flex: 1 }}>Группировать по предмету</span>
              <span onClick={() => setGrouped(g => !g)} style={{
                width: 32, height: 18, borderRadius: 9999, position: 'relative',
                background: grouped ? '#4f46e5' : '#cbd5e1', transition: '.15s', flexShrink: 0,
              }}>
                <span style={{
                  position: 'absolute', top: 2, left: grouped ? 16 : 2,
                  width: 14, height: 14, borderRadius: 9999, background: '#fff',
                  transition: 'left .15s',
                }}/>
              </span>
            </label>
          </div>
        )}
      </div>
    </div>
  );
}

const bulkBtnC = {
  display: 'inline-flex', alignItems: 'center', gap: 6,
  height: 28, padding: '0 10px', borderRadius: 6, border: 'none',
  background: 'transparent', color: '#4338ca', fontSize: 12.5, fontWeight: 500,
  fontFamily: 'inherit', cursor: 'pointer',
};

const pgBtn = {
  minWidth: 30, height: 30, padding: '0 8px', borderRadius: 7,
  border: '1px solid #e2e8f0', background: '#fff', color: '#334155',
  fontSize: 12.5, fontFamily: 'inherit', cursor: 'pointer',
  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
};

window.CoursesApp = CoursesApp;
