// Course detail — главное приложение страницы.
const { useState: useStateD, useMemo: useMemoD } = React;

function CourseApp() {
  const [tab, setTab] = useStateD('lessons');
  const [expanded, setExpanded] = useStateD(new Set(['m1', 'm2']));
  const [openLesson, setOpenLesson] = useStateD(null);
  const [query, setQuery] = useStateD('');
  const [typeFilter, setTypeFilter] = useStateD(new Set());

  // Filtered modules: keep modules with at least one matching lesson;
  // отфильтровать сами уроки внутри модуля.
  const filteredModules = useMemoD(() => {
    const q = query.trim().toLowerCase();
    return MODULES.map(m => ({
      ...m,
      lessons: m.lessons.filter(l => {
        if (q && !l.title.toLowerCase().includes(q)) return false;
        if (typeFilter.size && !typeFilter.has(l.type)) return false;
        return true;
      }),
    })).filter(m => m.lessons.length > 0);
  }, [query, typeFilter]);

  const allModuleIds = useMemoD(() => MODULES.map(m => m.id), []);
  const expandAll = () => setExpanded(new Set(allModuleIds));
  const collapseAll = () => setExpanded(new Set());
  const toggleModule = (id) => {
    const n = new Set(expanded);
    n.has(id) ? n.delete(id) : n.add(id);
    setExpanded(n);
  };

  // Auto-open module when search matches it
  React.useEffect(() => {
    if (query.trim()) {
      setExpanded(new Set(filteredModules.map(m => m.id)));
    }
  }, [query]); // eslint-disable-line

  const progress = useMemoD(() => {
    let total = 0, published = 0;
    MODULES.forEach(m => m.lessons.forEach(l => {
      total++;
      if (l.status === 'published') published++;
    }));
    return { total, published, ratio: published / total };
  }, []);

  const totalLessons = useMemoD(
    () => MODULES.reduce((a, m) => a + m.lessons.length, 0), []);

  const tabs = [
    { id: 'lessons',   icon: 'BookOpen',     label: 'Уроки',     count: totalLessons },
    { id: 'about',     icon: 'Info',         label: 'О курсе' },
    { id: 'groups',    icon: 'Users',        label: 'Группы',    count: COURSE_GROUPS.length },
    { id: 'materials', icon: 'FileText',     label: 'Материалы', count: 28 },
    { id: 'settings',  icon: 'Settings',     label: 'Настройки' },
  ];

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
          <a href="Courses.html" style={{ color: '#64748b' }}
            onMouseEnter={e => e.currentTarget.style.color = '#0f172a'}
            onMouseLeave={e => e.currentTarget.style.color = '#64748b'}>Курсы</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500,
            overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{COURSE.name}</span>
        </div>

        <div style={{ flex: 1, overflowY: 'auto' }}>
          {/* Hero */}
          <CourseHero c={COURSE} progress={progress} />

          {/* Tabs */}
          <CourseTabs tab={tab} setTab={setTab} tabs={tabs} />

          {/* Content */}
          {tab === 'lessons' && (
            <div style={{ padding: '24px 32px 40px' }}>
              <div style={{ maxWidth: 1240, margin: '0 auto',
                display: 'grid', gridTemplateColumns: 'minmax(0, 1fr) 320px', gap: 24 }}>

                {/* Main column */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: 16, minWidth: 0 }}>
                  <LessonsToolbar
                    query={query} setQuery={setQuery}
                    typeFilter={typeFilter} setTypeFilter={setTypeFilter}
                    expandAll={expandAll} collapseAll={collapseAll}
                  />

                  {filteredModules.length === 0 ? (
                    <div style={{
                      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
                      padding: '60px 20px', textAlign: 'center',
                    }}>
                      <div style={{
                        width: 48, height: 48, borderRadius: 12, background: '#f1f5f9',
                        display: 'inline-flex', alignItems: 'center', justifyContent: 'center', marginBottom: 12,
                      }}><Icon.Search size={20} stroke="#94a3b8" /></div>
                      <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>Уроки не найдены</div>
                      <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
                        Попробуйте изменить запрос или фильтр.
                      </div>
                    </div>
                  ) : filteredModules.map(m => (
                    <ModuleAccordion key={m.id} module={m}
                      openLessonId={openLesson?.id}
                      onLessonClick={setOpenLesson}
                      expanded={expanded.has(m.id)}
                      onToggle={() => toggleModule(m.id)} />
                  ))}
                </div>

                {/* Right rail */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: 16,
                  position: 'sticky', top: 64, alignSelf: 'flex-start' }}>
                  <CourseGroupsRail groups={COURSE_GROUPS} />
                  <CourseLessonTypesRail modules={MODULES} />
                  <CourseGoalsRail goals={COURSE.goals} />
                </div>
              </div>
            </div>
          )}

          {tab === 'about' && (
            <PaddedTab>
              <div style={{ display: 'grid', gridTemplateColumns: 'minmax(0, 1fr) 320px', gap: 24 }}>
                <div style={{
                  background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
                  padding: '22px 24px',
                }}>
                  <SectionLabel style={{ padding: 0, marginBottom: 8 }}>Описание курса</SectionLabel>
                  <p style={{ margin: 0, fontSize: 15, lineHeight: 1.6, color: '#334155',
                    textWrap: 'pretty' }}>{COURSE.description}</p>

                  <div style={{ height: 24 }} />
                  <SectionLabel style={{ padding: 0, marginBottom: 8 }}>Программа · кратко</SectionLabel>
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
                    {MODULES.map(m => (
                      <div key={m.id} style={{
                        display: 'grid', gridTemplateColumns: '60px 1fr auto', gap: 14,
                        padding: '12px 14px', border: '1px solid #f1f5f9',
                        borderRadius: 10,
                      }}>
                        <span style={{
                          fontFamily: 'var(--edv-font-mono)', fontSize: 11, color: '#4338ca',
                          background: '#eef2ff', padding: '4px 8px', borderRadius: 6,
                          fontWeight: 600, alignSelf: 'flex-start',
                        }}>МОД {m.n}</span>
                        <div>
                          <div style={{ fontSize: 14, fontWeight: 500, color: '#0f172a' }}>{m.name}</div>
                          <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>{m.summary}</div>
                        </div>
                        <div style={{ fontSize: 12, color: '#94a3b8',
                          fontVariantNumeric: 'tabular-nums', textAlign: 'right' }}>
                          {m.lessons.length} ур.<br/>{m.weeks} нед.
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
                  <CourseGoalsRail goals={COURSE.goals} />
                  <CourseLessonTypesRail modules={MODULES} />
                </div>
              </div>
            </PaddedTab>
          )}

          {tab === 'groups' && (
            <PaddedTab>
              <div style={{
                background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
                padding: '20px 22px',
              }}>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16 }}>
                  <div>
                    <h2 style={{ margin: 0, fontSize: 18, fontWeight: 600, color: '#0f172a' }}>
                      Группы на этом курсе
                    </h2>
                    <div style={{ fontSize: 13, color: '#64748b', marginTop: 3 }}>
                      Активные группы, занимающиеся по программе курса
                    </div>
                  </div>
                  <Button size="sm"><Icon.UserPlus size={14} />Создать группу</Button>
                </div>
                <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13.5 }}>
                  <thead>
                    <tr>
                      {['Группа', 'Студентов', 'Прогресс', 'Следующий урок'].map(h => (
                        <th key={h} style={{
                          padding: '10px 14px', textAlign: 'left', background: '#f8fafc',
                          borderBottom: '1px solid #e2e8f0',
                          fontSize: 12, fontWeight: 600, color: '#64748b',
                          textTransform: 'uppercase', letterSpacing: 0.2,
                        }}>{h}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {COURSE_GROUPS.map(g => (
                      <tr key={g.id} style={{ borderBottom: '1px solid #f1f5f9' }}>
                        <td style={{ padding: '14px' }}>
                          <div style={{ fontSize: 14, fontWeight: 500, color: '#0f172a' }}>{g.name}</div>
                        </td>
                        <td style={{ padding: '14px', fontVariantNumeric: 'tabular-nums', color: '#334155' }}>
                          {g.students}
                        </td>
                        <td style={{ padding: '14px' }}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                            <div style={{ flex: 1, maxWidth: 180, height: 5,
                              background: '#f1f5f9', borderRadius: 9999, overflow: 'hidden' }}>
                              <div style={{ width: `${g.progress*100}%`, height: '100%', background: '#4f46e5' }} />
                            </div>
                            <span style={{ fontSize: 12.5, color: '#475569',
                              fontVariantNumeric: 'tabular-nums', minWidth: 40 }}>
                              {Math.round(g.progress*100)}%
                            </span>
                          </div>
                        </td>
                        <td style={{ padding: '14px',
                          fontFamily: 'var(--edv-font-mono)', fontSize: 12, color: '#64748b' }}>
                          {g.next}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </PaddedTab>
          )}

          {tab === 'materials' && (
            <PaddedTab>
              <div style={{
                background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
                padding: '36px 24px', textAlign: 'center', color: '#64748b',
              }}>
                <div style={{
                  width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center', marginBottom: 12,
                }}><Icon.FileText size={22} stroke="#94a3b8" /></div>
                <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>Материалы курса</div>
                <div style={{ fontSize: 13, color: '#64748b', maxWidth: 380, margin: '6px auto 0' }}>
                  Сводный список материалов из всех уроков: учебники, презентации, аудио, ссылки.
                  В черновике — будет следующим.
                </div>
              </div>
            </PaddedTab>
          )}

          {tab === 'settings' && (
            <PaddedTab>
              <div style={{
                background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
                padding: '36px 24px', textAlign: 'center', color: '#64748b',
              }}>
                <div style={{
                  width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center', marginBottom: 12,
                }}><Icon.Settings size={22} stroke="#94a3b8" /></div>
                <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>Настройки курса</div>
                <div style={{ fontSize: 13, maxWidth: 380, margin: '6px auto 0' }}>
                  Видимость, доступы, статус, архивирование, шаблон расписания.
                </div>
              </div>
            </PaddedTab>
          )}
        </div>
      </div>

      {/* Lesson drawer */}
      <LessonDrawer lesson={openLesson} onClose={() => setOpenLesson(null)} />
    </div>
  );
}

function PaddedTab({ children }) {
  return (
    <div style={{ padding: '24px 32px 40px' }}>
      <div style={{ maxWidth: 1240, margin: '0 auto' }}>{children}</div>
    </div>
  );
}

window.CourseApp = CourseApp;
