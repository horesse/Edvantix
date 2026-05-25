// Course detail — компоненты страницы курса.

// ── Hero ─────────────────────────────────────────────────────────────
function CourseHero({ c, progress }) {
  const subj = COURSE_SUBJECTS[c.subject];
  const tone = SUBJECT_TONES[subj.tone];
  const status = COURSE_STATUSES[c.status];
  return (
    <div style={{
      padding: '24px 32px 22px', background: '#fff',
      borderBottom: '1px solid #e2e8f0',
    }}>
      <div style={{ maxWidth: 1240, margin: '0 auto', display: 'flex', alignItems: 'flex-start', gap: 22 }}>
        {/* Cover */}
        <div style={{
          width: 96, height: 96, borderRadius: 16, flexShrink: 0,
          background: tone.cover, color: '#fff',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 38, fontWeight: 700, fontFamily: 'var(--edv-font-mono)',
          letterSpacing: '-0.04em', boxShadow: '0 8px 24px -8px rgba(99,102,241,0.45)',
        }}>{c.cover}</div>

        <div style={{ flex: 1, minWidth: 0 }}>
          {/* Top row: chips */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 8, flexWrap: 'wrap' }}>
            <SubjectChip subject={c.subject} />
            <CourseLevelChip level={c.level} />
            <span style={{
              display: 'inline-flex', alignItems: 'center', gap: 6,
              padding: '3px 10px', borderRadius: 9999, background: status.bg, color: status.fg,
              fontSize: 12, fontWeight: 500,
            }}>
              <span style={{ width: 6, height: 6, borderRadius: 9999, background: status.dot }} />
              {status.label}
            </span>
            <span style={{
              fontFamily: 'var(--edv-font-mono)', fontSize: 12, color: '#64748b',
              padding: '3px 8px', background: '#f1f5f9', borderRadius: 6,
            }}>{c.code}</span>
          </div>

          {/* Title */}
          <h1 style={{
            margin: 0, fontSize: 28, fontWeight: 700, letterSpacing: '-0.02em',
            color: '#0f172a', lineHeight: 1.2,
          }}>{c.name}</h1>

          {/* Description */}
          <p style={{
            margin: '8px 0 0', fontSize: 14, lineHeight: 1.55, color: '#475569',
            maxWidth: 720, textWrap: 'pretty',
          }}>{c.description}</p>

          {/* Owner row */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 14, marginTop: 14, fontSize: 12.5, color: '#64748b' }}>
            <span style={{ display: 'inline-flex', alignItems: 'center', gap: 7 }}>
              <Avatar name={c.owner} size={20} />
              <span>Автор · <strong style={{ color: '#0f172a', fontWeight: 600 }}>{c.owner}</strong></span>
            </span>
            <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
            <span>Создан {c.created}</span>
            <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
            <span>Обновлён {c.updated}</span>
          </div>
        </div>

        {/* Actions */}
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, flexShrink: 0 }}>
          <Button variant="secondary"><Icon.FileText size={15} />Редактировать</Button>
          <Button><Icon.UserPlus size={16} />Создать группу</Button>
          <button style={ghostIconBtn}>
            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
              <circle cx="5" cy="12" r="2"/><circle cx="12" cy="12" r="2"/><circle cx="19" cy="12" r="2"/>
            </svg>
          </button>
        </div>
      </div>

      {/* KPI strip */}
      <div style={{
        maxWidth: 1240, margin: '20px auto 0',
        display: 'grid', gap: 12,
        gridTemplateColumns: 'repeat(5, 1fr)',
      }}>
        <CourseStat icon="FileText"      label="Уроков"     value={c.lessons}        sub={`${progress.published} готовы`} />
        <CourseStat icon="CalendarDays"  label="Длительность" value={`${c.durationWeeks} нед.`} sub="2 урока в неделю" />
        <CourseStat icon="Users"         label="Групп"      value={c.groups}         sub={`${c.students} студентов`} />
        <CourseStat icon="BarChart2"     label="Готовность" value={`${Math.round(progress.ratio*100)}%`}
          progress={progress.ratio} sub={`${progress.published}/${c.lessons} уроков`} />
        <CourseStat icon="GraduationCap" label="Уровень"    value={c.level}          sub="CEFR" />
      </div>
    </div>
  );
}

const ghostIconBtn = {
  width: 36, height: 36, borderRadius: 8, border: '1px solid #e2e8f0',
  background: '#fff', color: '#64748b', cursor: 'pointer',
  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
};

function CourseStat({ icon, label, value, sub, progress }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      padding: '12px 14px', background: '#fafbfc',
      border: '1px solid #e2e8f0', borderRadius: 12,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 8 }}>
        <Ic size={14} stroke="#94a3b8" />
        <span style={{ fontSize: 11.5, color: '#64748b', fontWeight: 500,
          textTransform: 'uppercase', letterSpacing: '0.04em' }}>{label}</span>
      </div>
      <div style={{ fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em',
        color: '#0f172a', fontVariantNumeric: 'tabular-nums', lineHeight: 1 }}>{value}</div>
      {progress !== undefined && (
        <div style={{
          height: 4, marginTop: 8, background: '#e2e8f0',
          borderRadius: 9999, overflow: 'hidden',
        }}>
          <div style={{
            width: `${progress*100}%`, height: '100%',
            background: '#4f46e5', borderRadius: 9999,
          }} />
        </div>
      )}
      {sub && <div style={{ marginTop: progress !== undefined ? 6 : 6, fontSize: 12, color: '#64748b' }}>{sub}</div>}
    </div>
  );
}

// ── Tabs ─────────────────────────────────────────────────────────────
function CourseTabs({ tab, setTab, tabs }) {
  return (
    <div style={{
      background: '#fff', borderBottom: '1px solid #e2e8f0',
      padding: '0 32px', position: 'sticky', top: 0, zIndex: 5,
    }}>
      <div style={{
        maxWidth: 1240, margin: '0 auto',
        display: 'flex', alignItems: 'center', gap: 4,
      }}>
        {tabs.map(t => {
          const active = tab === t.id;
          const Ic = Icon[t.icon];
          return (
            <button key={t.id} onClick={() => setTab(t.id)}
              style={{
                position: 'relative', display: 'inline-flex', alignItems: 'center', gap: 8,
                padding: '14px 14px', border: 'none', background: 'transparent',
                fontFamily: 'inherit', fontSize: 13.5,
                fontWeight: active ? 600 : 500,
                color: active ? '#0f172a' : '#64748b',
                cursor: 'pointer',
              }}>
              <Ic size={15} stroke={active ? '#4f46e5' : 'currentColor'} />
              {t.label}
              {t.count !== undefined && (
                <span style={{
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                  minWidth: 20, height: 20, padding: '0 6px', borderRadius: 9999,
                  background: active ? '#e0eaff' : '#f1f5f9',
                  color: active ? '#4338ca' : '#64748b',
                  fontSize: 11, fontWeight: 600, fontVariantNumeric: 'tabular-nums',
                }}>{t.count}</span>
              )}
              {active && (
                <span style={{
                  position: 'absolute', left: 8, right: 8, bottom: -1, height: 2,
                  background: '#4f46e5', borderRadius: 2,
                }} />
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
}

// ── Lesson type chip ─────────────────────────────────────────────────
function LessonTypeChip({ type }) {
  const t = LESSON_TYPES[type];
  const Ic = Icon[t.icon];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 5,
      padding: '3px 8px', borderRadius: 6, background: t.bg, color: t.fg,
      fontSize: 11.5, fontWeight: 500, lineHeight: 1.4, flexShrink: 0,
    }}>
      <Ic size={11} stroke="currentColor" />{t.label}
    </span>
  );
}

// ── Lesson status dot ────────────────────────────────────────────────
function LessonStatusBadge({ status }) {
  const s = LESSON_STATUSES[status];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 5,
      fontSize: 11.5, color: s.fg, fontWeight: 500,
    }}>
      <span style={{ width: 6, height: 6, borderRadius: 9999, background: s.dot }} />
      {s.label}
    </span>
  );
}

// ── Lesson row ───────────────────────────────────────────────────────
function LessonRow({ lesson, isOpen, onClick }) {
  const [hover, setHover] = React.useState(false);
  const t = LESSON_TYPES[lesson.type];
  const isPublished = lesson.status === 'published';
  return (
    <div onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      onClick={onClick}
      style={{
        display: 'grid',
        gridTemplateColumns: '40px 1fr auto auto auto 32px',
        alignItems: 'center', gap: 14,
        padding: '12px 18px',
        background: isOpen ? '#f0f4ff' : hover ? '#fafbfc' : 'transparent',
        borderLeft: `3px solid ${isOpen ? '#4f46e5' : 'transparent'}`,
        cursor: 'pointer', transition: 'background .12s',
        borderBottom: '1px solid #f1f5f9',
      }}>
      {/* Number */}
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: isPublished ? t.bg : '#f8fafc',
        color: isPublished ? t.fg : '#94a3b8',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        fontFamily: 'var(--edv-font-mono)', fontSize: 13, fontWeight: 600,
        border: isPublished ? 'none' : '1px dashed #cbd5e1',
      }}>{lesson.n}</div>

      {/* Title + meta */}
      <div style={{ minWidth: 0 }}>
        <div style={{
          fontSize: 14, fontWeight: 500, color: '#0f172a',
          lineHeight: 1.35, marginBottom: 4,
          opacity: isPublished ? 1 : 0.7,
        }}>{lesson.title}</div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, flexWrap: 'wrap' }}>
          <LessonTypeChip type={lesson.type} />
          <span style={{ fontSize: 11.5, color: '#94a3b8' }}>·</span>
          <span style={{ fontSize: 11.5, color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>
            {lesson.blocks} блоков
          </span>
          <span style={{ fontSize: 11.5, color: '#94a3b8' }}>·</span>
          <span style={{ fontSize: 11.5, color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>
            {lesson.materials} материалов
          </span>
        </div>
      </div>

      {/* Date */}
      <div style={{
        fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#94a3b8',
        whiteSpace: 'nowrap',
      }}>{lesson.date}</div>

      {/* Duration */}
      <div style={{
        display: 'inline-flex', alignItems: 'center', gap: 5,
        fontSize: 12.5, color: '#475569', fontVariantNumeric: 'tabular-nums',
        whiteSpace: 'nowrap',
      }}>
        <Icon.CalendarDays size={12} stroke="#94a3b8" />{lesson.minutes} мин
      </div>

      {/* Status */}
      <div style={{ minWidth: 100 }}>
        <LessonStatusBadge status={lesson.status} />
      </div>

      {/* Chevron */}
      <Icon.ChevronRight size={16} stroke={hover || isOpen ? '#4f46e5' : '#cbd5e1'} />
    </div>
  );
}

// ── Module ───────────────────────────────────────────────────────────
function ModuleAccordion({ module, openLessonId, onLessonClick, expanded, onToggle }) {
  const totalMin = module.lessons.reduce((a, l) => a + l.minutes, 0);
  const published = module.lessons.filter(l => l.status === 'published').length;
  const ratio = published / module.lessons.length;
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      overflow: 'hidden',
    }}>
      <button onClick={onToggle} style={{
        display: 'grid', gridTemplateColumns: 'auto 1fr auto', alignItems: 'center', gap: 16,
        width: '100%', padding: '14px 18px',
        background: expanded ? '#fafbfc' : '#fff', border: 'none',
        borderBottom: expanded ? '1px solid #e2e8f0' : 'none',
        cursor: 'pointer', textAlign: 'left', fontFamily: 'inherit',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
          <Icon.ChevronDown size={16} stroke="#64748b"
            style={{ transform: expanded ? 'none' : 'rotate(-90deg)', transition: 'transform .15s' }} />
          <div style={{
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            minWidth: 32, height: 32, padding: '0 10px', borderRadius: 8,
            background: '#eef2ff', color: '#4338ca',
            fontSize: 12, fontWeight: 600,
            fontFamily: 'var(--edv-font-mono)', letterSpacing: '0.05em',
          }}>МОД {module.n}</div>
        </div>
        <div style={{ minWidth: 0 }}>
          <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a', lineHeight: 1.3 }}>
            {module.name}
          </div>
          <div style={{ marginTop: 3, fontSize: 12.5, color: '#64748b' }}>
            {module.summary}
          </div>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 18, flexShrink: 0 }}>
          <div style={{ textAlign: 'right' }}>
            <div style={{ fontSize: 12.5, color: '#0f172a', fontVariantNumeric: 'tabular-nums', fontWeight: 500 }}>
              {module.lessons.length} уроков
            </div>
            <div style={{ fontSize: 11.5, color: '#94a3b8', fontVariantNumeric: 'tabular-nums' }}>
              {Math.round(totalMin/60)} ч · {module.weeks} нед.
            </div>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 4, minWidth: 96 }}>
            <div style={{ fontSize: 11.5, color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>
              {published}/{module.lessons.length} готово
            </div>
            <div style={{ width: 96, height: 4, background: '#f1f5f9', borderRadius: 9999, overflow: 'hidden' }}>
              <div style={{
                width: `${ratio*100}%`, height: '100%',
                background: ratio === 1 ? '#10b981' : '#4f46e5',
                borderRadius: 9999, transition: 'width .25s',
              }} />
            </div>
          </div>
        </div>
      </button>
      {expanded && (
        <div>
          {module.lessons.map(l => (
            <LessonRow key={l.id} lesson={l}
              isOpen={openLessonId === l.id}
              onClick={() => onLessonClick(l)} />
          ))}
        </div>
      )}
    </div>
  );
}

// ── Lesson drawer (right slide-over) ─────────────────────────────────
function LessonDrawer({ lesson, onClose }) {
  if (!lesson) return null;
  const t = LESSON_TYPES[lesson.type];
  const Ic = Icon[t.icon];
  const status = LESSON_STATUSES[lesson.status];

  // Mock blocks — структура урока: каждый блок = атомарный кусок (общая инфа + что-то ещё)
  const blocks = mockBlocksForLesson(lesson);

  return (
    <React.Fragment>
      <div onClick={onClose} style={{
        position: 'fixed', inset: 0, background: 'rgba(15,23,42,0.32)',
        zIndex: 40, animation: 'fadeIn .15s',
      }} />
      <div style={{
        position: 'fixed', top: 0, right: 0, bottom: 0, width: 'min(560px, 96vw)',
        background: '#fff', zIndex: 41, boxShadow: '-12px 0 48px -12px rgba(15,23,42,0.25)',
        display: 'flex', flexDirection: 'column',
        animation: 'slideIn .2s ease-out',
      }}>
        {/* Header */}
        <div style={{
          padding: '18px 22px 14px', borderBottom: '1px solid #e2e8f0',
          display: 'flex', alignItems: 'flex-start', gap: 14,
        }}>
          <div style={{
            width: 40, height: 40, borderRadius: 10, flexShrink: 0,
            background: t.bg, color: t.fg,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
          }}><Ic size={18} stroke="currentColor" /></div>
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 4 }}>
              <span style={{
                fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b',
              }}>УРОК {lesson.n}</span>
              <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
              <LessonStatusBadge status={lesson.status} />
            </div>
            <h2 style={{
              margin: 0, fontSize: 18, fontWeight: 600, color: '#0f172a',
              letterSpacing: '-0.01em', lineHeight: 1.3,
            }}>{lesson.title}</h2>
          </div>
          <button onClick={onClose} style={{
            width: 32, height: 32, borderRadius: 8, border: 'none', background: '#f1f5f9',
            cursor: 'pointer', display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            color: '#64748b', flexShrink: 0,
          }}><Icon.X size={15} /></button>
        </div>

        {/* Quick meta row */}
        <div style={{
          padding: '12px 22px', display: 'flex', gap: 18, flexWrap: 'wrap',
          background: '#fafbfc', borderBottom: '1px solid #e2e8f0',
          fontSize: 12.5, color: '#475569',
        }}>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <LessonTypeChip type={lesson.type} />
          </span>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <Icon.CalendarDays size={13} stroke="#94a3b8" />{lesson.minutes} минут
          </span>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <Icon.FileText size={13} stroke="#94a3b8" />{lesson.materials} материалов
          </span>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <Icon.Sparkles size={13} stroke="#94a3b8" />{lesson.blocks} блоков
          </span>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6,
            fontFamily: 'var(--edv-font-mono)', color: '#94a3b8' }}>
            <Icon.Calendar size={13} stroke="#94a3b8" />{lesson.date}
          </span>
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '20px 22px' }}>
          {/* Objectives */}
          {lesson.objectives && (
            <div style={{ marginBottom: 22 }}>
              <SectionLabel style={{ padding: 0, marginBottom: 10 }}>Цели урока</SectionLabel>
              <ul style={{ margin: 0, padding: 0, listStyle: 'none',
                display: 'flex', flexDirection: 'column', gap: 8 }}>
                {lesson.objectives.map((o, i) => (
                  <li key={i} style={{ display: 'flex', alignItems: 'flex-start', gap: 8,
                    fontSize: 13.5, color: '#334155', lineHeight: 1.45 }}>
                    <Icon.Check size={14} stroke="#10b981" sw={2.5} style={{ marginTop: 3, flexShrink: 0 }} />
                    {o}
                  </li>
                ))}
              </ul>
            </div>
          )}

          {/* Structure */}
          <div style={{ marginBottom: 22 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 10 }}>
              <SectionLabel style={{ padding: 0 }}>Структура урока</SectionLabel>
              <span style={{ fontSize: 11.5, color: '#94a3b8' }}>Что увидит студент в плеере</span>
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              {blocks.map((b, i) => <LessonBlockRow key={i} block={b} index={i+1} />)}
            </div>
          </div>

          {/* Future placeholder — что ещё может быть в уроке */}
          <div style={{
            padding: '14px 16px',
            background: 'repeating-linear-gradient(135deg, #f8fafc, #f8fafc 8px, #f1f5f9 8px, #f1f5f9 12px)',
            border: '1px dashed #cbd5e1', borderRadius: 12,
            display: 'flex', alignItems: 'flex-start', gap: 12,
          }}>
            <div style={{
              width: 32, height: 32, borderRadius: 8, background: '#fff',
              border: '1px solid #e2e8f0', display: 'flex', alignItems: 'center', justifyContent: 'center',
              color: '#4f46e5', flexShrink: 0,
            }}><Icon.Sparkles size={15} /></div>
            <div style={{ flex: 1 }}>
              <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a', marginBottom: 3 }}>
                Интерактивная составляющая урока
              </div>
              <div style={{ fontSize: 12.5, color: '#64748b', lineHeight: 1.5,
                fontFamily: 'var(--edv-font-mono)' }}>
                {/* Placeholder — здесь будет редактор интерактивных блоков */}
                {'/* блоки превратятся в плеер: theory · exercise · quiz · speaking */'}
              </div>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div style={{
          padding: '12px 22px', borderTop: '1px solid #e2e8f0', background: '#fff',
          display: 'flex', alignItems: 'center', gap: 8,
        }}>
          <Button variant="secondary" size="sm">
            <Icon.FileText size={14} />Материалы
          </Button>
          <Button variant="secondary" size="sm">
            <Icon.Sparkles size={14} />Дублировать
          </Button>
          <div style={{ marginLeft: 'auto', display: 'flex', gap: 8 }}>
            <Button variant="secondary" size="sm" onClick={onClose}>Закрыть</Button>
            <Button size="sm"><Icon.ArrowRight size={14} />Открыть редактор</Button>
          </div>
        </div>
      </div>
    </React.Fragment>
  );
}

function LessonBlockRow({ block, index }) {
  const def = BLOCK_TYPES[block.type];
  const Ic = Icon[def.icon];
  return (
    <div style={{
      display: 'grid', gridTemplateColumns: '24px 28px 1fr auto', gap: 12,
      alignItems: 'center', padding: '10px 12px',
      border: '1px solid #e2e8f0', borderRadius: 10, background: '#fff',
    }}>
      <span style={{ fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#94a3b8',
        textAlign: 'right' }}>{String(index).padStart(2, '0')}</span>
      <div style={{
        width: 28, height: 28, borderRadius: 7, background: '#f1f5f9', color: '#475569',
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      }}><Ic size={14} stroke="currentColor" /></div>
      <div style={{ minWidth: 0 }}>
        <div style={{ fontSize: 13, fontWeight: 500, color: '#0f172a' }}>{block.title}</div>
        <div style={{ fontSize: 11.5, color: '#64748b' }}>{def.label} · {block.duration} мин</div>
      </div>
      <Icon.ChevronRight size={14} stroke="#cbd5e1" />
    </div>
  );
}

// Mock generator — каждый урок получает структуру из 5–8 блоков, тип зависит от типа урока
function mockBlocksForLesson(lesson) {
  const recipes = {
    lecture:   ['intro','theory','video','exercise','quiz','homework'],
    practice:  ['intro','theory','exercise','exercise','quiz','homework'],
    speaking:  ['intro','theory','speaking','speaking','quiz'],
    listening: ['intro','listening','listening','exercise','quiz','homework'],
    writing:   ['intro','theory','writing','exercise','homework'],
    test:      ['intro','quiz','quiz','exercise'],
    review:    ['intro','theory','exercise','exercise','speaking','quiz','homework'],
  };
  const titles = {
    intro:     'Цели и план занятия',
    theory:    'Грамматический разбор',
    video:     'Видео-объяснение преподавателя',
    exercise:  'Тренировка: подставь правильную форму',
    speaking:  'Pair-work · обсуждение в парах',
    listening: 'Аудио и вопросы на понимание',
    writing:   'Письменное задание',
    quiz:      'Мини-квиз — 8 вопросов',
    homework:  'Домашняя работа',
  };
  const durations = { intro: 5, theory: 18, video: 8, exercise: 12, speaking: 15, listening: 14, writing: 16, quiz: 7, homework: 5 };
  const types = (recipes[lesson.type] || recipes.lecture).slice(0, lesson.blocks);
  return types.map(t => ({ type: t, title: titles[t], duration: durations[t] }));
}

// ── Toolbar over module list ─────────────────────────────────────────
function LessonsToolbar({ query, setQuery, typeFilter, setTypeFilter, expandAll, collapseAll }) {
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap',
      padding: '4px 0',
    }}>
      <div style={{ position: 'relative', flex: '1 1 280px', maxWidth: 360, height: 36 }}>
        <Icon.Search size={15} stroke="#94a3b8"
          style={{ position: 'absolute', left: 12, top: 11, pointerEvents: 'none' }} />
        <input value={query} onChange={e => setQuery(e.target.value)}
          placeholder="Поиск по названию урока"
          style={{
            width: '100%', height: 36, paddingLeft: 34, paddingRight: 12,
            borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
            fontSize: 13, fontFamily: 'inherit', color: '#0f172a', outline: 'none',
          }}
          onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.2)'; }}
          onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }} />
      </div>

      <CourseFilterDropdown
        label="Тип" icon="Filter"
        value={typeFilter} onChange={setTypeFilter}
        options={Object.entries(LESSON_TYPES).map(([k, v]) => ({
          value: k, label: v.label, swatch: v.fg,
        }))}
      />

      <button onClick={expandAll} style={ghostBtn}>
        <Icon.ChevronDown size={13} />Развернуть все
      </button>
      <button onClick={collapseAll} style={ghostBtn}>
        <Icon.ChevronRight size={13} />Свернуть все
      </button>

      <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: 8 }}>
        <Button variant="secondary" size="sm">
          <Icon.FileText size={14} />Экспорт PDF
        </Button>
        <Button size="sm"><Icon.Plus size={14} />Добавить урок</Button>
      </div>
    </div>
  );
}

const ghostBtn = {
  display: 'inline-flex', alignItems: 'center', gap: 6,
  height: 36, padding: '0 12px', borderRadius: 10,
  border: '1px solid #e2e8f0', background: '#fff', color: '#475569',
  fontSize: 13, fontFamily: 'inherit', cursor: 'pointer',
};

// ── Sidebar widgets (right rail on Уроки tab) ───────────────────────
function CourseGroupsRail({ groups }) {
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '14px 16px',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12 }}>
        <Icon.Users size={15} stroke="#475569" />
        <strong style={{ fontSize: 13.5, color: '#0f172a' }}>Группы на курсе</strong>
        <span style={{
          marginLeft: 'auto', fontSize: 11.5, fontWeight: 600,
          padding: '2px 7px', borderRadius: 9999, background: '#eef2ff', color: '#4338ca',
        }}>{groups.length}</span>
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {groups.map(g => (
          <div key={g.id} style={{
            padding: '10px 12px', borderRadius: 10, background: '#fafbfc',
            border: '1px solid #f1f5f9',
          }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 6 }}>
              <div style={{ fontSize: 13, fontWeight: 500, color: '#0f172a',
                overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{g.name}</div>
              <span style={{ fontSize: 11.5, color: '#64748b', fontVariantNumeric: 'tabular-nums', flexShrink: 0, marginLeft: 8 }}>
                {g.students} студ.
              </span>
            </div>
            <div style={{ height: 4, background: '#e2e8f0', borderRadius: 9999, overflow: 'hidden', marginBottom: 6 }}>
              <div style={{
                width: `${g.progress*100}%`, height: '100%', background: '#4f46e5',
              }} />
            </div>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between',
              fontSize: 11.5, color: '#64748b' }}>
              <span style={{ fontVariantNumeric: 'tabular-nums' }}>{Math.round(g.progress*100)}% программы</span>
              <span style={{ fontFamily: 'var(--edv-font-mono)' }}>{g.next}</span>
            </div>
          </div>
        ))}
      </div>
      <button style={{
        marginTop: 12, width: '100%', height: 32, borderRadius: 8,
        border: '1px solid #e2e8f0', background: '#fff', color: '#475569',
        fontSize: 12.5, fontFamily: 'inherit', fontWeight: 500, cursor: 'pointer',
      }}>Все группы курса</button>
    </div>
  );
}

function CourseGoalsRail({ goals }) {
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '14px 16px',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 10 }}>
        <Icon.Sparkles size={15} stroke="#475569" />
        <strong style={{ fontSize: 13.5, color: '#0f172a' }}>Чему научится студент</strong>
      </div>
      <ul style={{ margin: 0, padding: 0, listStyle: 'none',
        display: 'flex', flexDirection: 'column', gap: 8 }}>
        {goals.map((g, i) => (
          <li key={i} style={{ display: 'flex', alignItems: 'flex-start', gap: 8,
            fontSize: 13, color: '#334155', lineHeight: 1.45 }}>
            <Icon.Check size={13} stroke="#10b981" sw={2.5} style={{ marginTop: 3, flexShrink: 0 }} />
            {g}
          </li>
        ))}
      </ul>
    </div>
  );
}

function CourseLessonTypesRail({ modules }) {
  const counts = {};
  modules.forEach(m => m.lessons.forEach(l => { counts[l.type] = (counts[l.type] || 0) + 1; }));
  const total = Object.values(counts).reduce((a, b) => a + b, 0);
  const order = Object.keys(LESSON_TYPES).filter(k => counts[k]);
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '14px 16px',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12 }}>
        <Icon.BarChart2 size={15} stroke="#475569" />
        <strong style={{ fontSize: 13.5, color: '#0f172a' }}>Типы уроков</strong>
      </div>
      {/* Stacked bar */}
      <div style={{ display: 'flex', height: 8, borderRadius: 9999, overflow: 'hidden',
        background: '#f1f5f9', marginBottom: 12 }}>
        {order.map(k => (
          <div key={k} title={`${LESSON_TYPES[k].label}: ${counts[k]}`}
            style={{ width: `${(counts[k]/total)*100}%`, background: LESSON_TYPES[k].fg }} />
        ))}
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
        {order.map(k => {
          const t = LESSON_TYPES[k];
          return (
            <div key={k} style={{ display: 'grid', gridTemplateColumns: '10px 1fr auto', gap: 10,
              alignItems: 'center' }}>
              <span style={{ width: 8, height: 8, borderRadius: 9999, background: t.fg }} />
              <span style={{ fontSize: 12.5, color: '#334155' }}>{t.label}</span>
              <span style={{ fontSize: 12, color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>{counts[k]}</span>
            </div>
          );
        })}
      </div>
    </div>
  );
}

window.CourseHero = CourseHero;
window.CourseTabs = CourseTabs;
window.ModuleAccordion = ModuleAccordion;
window.LessonDrawer = LessonDrawer;
window.LessonsToolbar = LessonsToolbar;
window.CourseGroupsRail = CourseGroupsRail;
window.CourseGoalsRail = CourseGoalsRail;
window.CourseLessonTypesRail = CourseLessonTypesRail;
