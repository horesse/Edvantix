// Lesson create — секции формы (без билдера структуры — он отдельным файлом).

// ── Section card — переиспользуем тот же паттерн что в EditApp ───────
function LcSection({ icon, title, subtitle, children, rightSlot, accent }) {
  const IC = Icon[icon];
  const accentBg = accent || 'rgba(79,70,229,0.08)';
  const accentFg = accent ? '#0f172a' : '#4f46e5';
  return (
    <section style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
      overflow: 'hidden',
    }}>
      <header style={{
        padding: '16px 22px', borderBottom: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', gap: 14,
      }}>
        <div style={{
          width: 34, height: 34, borderRadius: 10, flexShrink: 0,
          background: accentBg, color: accentFg,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <IC size={17} stroke={accentFg} />
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <h2 style={{ margin: 0, fontSize: 15, fontWeight: 600,
            color: '#0f172a', letterSpacing: '-0.01em' }}>{title}</h2>
          {subtitle && <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>{subtitle}</div>}
        </div>
        {rightSlot}
      </header>
      <div style={{ padding: '20px 22px' }}>{children}</div>
    </section>
  );
}

// ── 1. Размещение: где в курсе живёт этот урок ───────────────────────
function PlacementSection({ value, onChange }) {
  const opts = MODULES_AS_OPTIONS();
  const current = opts.find(o => o.value === value.moduleId);
  const nextLessonNumber = current
    ? window.MODULES.find(m => m.id === current.value).lessons
        .reduce((max, l) => Math.max(max, l.n), 0) + 1
    : null;

  return (
    <LcSection icon="BookOpen" title="Где разместить" subtitle="Модуль в составе курса · номер урока назначится автоматически">
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 160px', gap: 14, alignItems: 'flex-start' }}>
        <F.Field label="Модуль программы" required>
          <F.Select
            value={value.moduleId}
            onChange={v => onChange({ moduleId: v })}
            options={opts.map(o => ({ value: o.value, label: o.label }))}
            placeholder="Выберите модуль"
          />
          {current && (
            <div style={{ marginTop: 8, fontSize: 12.5, color: '#64748b', display: 'flex', alignItems: 'center', gap: 8 }}>
              <Icon.Info size={13} stroke="#94a3b8" />
              <span>{current.summary} · {current.lessonCount} уроков, {current.weeks} нед.</span>
            </div>
          )}
        </F.Field>

        <F.Field label="Номер урока">
          <div style={{
            height: 42, borderRadius: 12, border: '1px dashed #cbd5e1', background: '#fafbfc',
            display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8,
            color: '#475569',
          }}>
            <span style={{ fontFamily: 'var(--edv-font-mono)', fontSize: 13, color: '#94a3b8' }}>УР</span>
            <span style={{ fontSize: 22, fontWeight: 700, letterSpacing: '-0.01em',
              fontVariantNumeric: 'tabular-nums', color: '#0f172a' }}>
              {nextLessonNumber || '—'}
            </span>
          </div>
        </F.Field>
      </div>
    </LcSection>
  );
}

// ── 2. О занятии: title, type ────────────────────────────────────────
function AboutSection({ value, onChange, errors, onTypeChange }) {
  return (
    <LcSection icon="FileText" title="О занятии" subtitle="Что увидит студент и преподаватель в расписании">
      <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>

        {/* Title — крупный input */}
        <F.Field label="Название урока" required error={errors.title}
          hint="Коротко и по делу: «Past Simple — irregular verbs», «Roleplay: at the airport»">
          <input
            value={value.title}
            onChange={e => onChange({ title: e.target.value })}
            placeholder="Введите название…"
            style={{
              width: '100%', height: 54, borderRadius: 12,
              border: `1px solid ${errors.title ? '#ef4444' : '#e2e8f0'}`,
              background: '#fff', padding: '0 16px',
              fontSize: 22, fontWeight: 600, color: '#0f172a',
              fontFamily: 'inherit', outline: 'none', letterSpacing: '-0.01em',
              transition: '.15s',
            }}
            onFocus={e => { if (!errors.title) { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.2)'; }}}
            onBlur={e => { e.target.style.borderColor = errors.title ? '#ef4444' : '#e2e8f0'; e.target.style.boxShadow = 'none'; }}
          />
        </F.Field>

        {/* Type — card radio */}
        <F.Field label="Тип урока" required hint="Определяет иконку, шаблон структуры и блоки по умолчанию">
          <LessonTypeRadio value={value.type} onChange={onTypeChange} />
        </F.Field>
      </div>
    </LcSection>
  );
}

// Card radio for lesson type — небольшие плитки с иконкой и подписью.
function LessonTypeRadio({ value, onChange }) {
  return (
    <div style={{
      display: 'grid',
      gridTemplateColumns: 'repeat(auto-fill, minmax(140px, 1fr))',
      gap: 10,
    }}>
      {Object.entries(window.LESSON_TYPES).map(([k, t]) => {
        const active = value === k;
        const Ic = Icon[t.icon];
        return (
          <button key={k} type="button" onClick={() => onChange(k)}
            style={{
              display: 'flex', alignItems: 'center', gap: 10,
              padding: '10px 12px', borderRadius: 12,
              border: `1px solid ${active ? '#4f46e5' : '#e2e8f0'}`,
              background: active ? 'rgba(79,70,229,0.04)' : '#fff',
              boxShadow: active ? '0 0 0 3px rgba(79,70,229,0.12)' : 'none',
              cursor: 'pointer', fontFamily: 'inherit', textAlign: 'left',
              transition: '.15s',
            }}
            onMouseEnter={e => { if (!active) { e.currentTarget.style.borderColor = '#c7d6fe'; e.currentTarget.style.background = '#fafbff'; } }}
            onMouseLeave={e => { if (!active) { e.currentTarget.style.borderColor = '#e2e8f0'; e.currentTarget.style.background = '#fff'; } }}>
            <span style={{
              width: 30, height: 30, borderRadius: 8, flexShrink: 0,
              background: t.bg, color: t.fg,
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            }}><Ic size={15} stroke="currentColor" /></span>
            <span style={{ fontSize: 13, fontWeight: active ? 600 : 500, color: '#0f172a' }}>
              {t.label}
            </span>
          </button>
        );
      })}
    </div>
  );
}

// ── 3. Цели урока — bullet list editor ──────────────────────────────
function ObjectivesSection({ value, onChange }) {
  const objectives = value.objectives;
  const update = (i, text) => {
    const next = objectives.slice();
    next[i] = text;
    onChange({ objectives: next });
  };
  const add = () => onChange({ objectives: [...objectives, ''] });
  const remove = (i) => {
    if (objectives.length <= 1) {
      onChange({ objectives: [''] });
    } else {
      onChange({ objectives: objectives.filter((_, j) => j !== i) });
    }
  };
  const examples = [
    'Различать Present Simple и Continuous',
    'Уверенно описывать свою рутину',
    'Понимать 80% реплик в диалоге',
  ];

  return (
    <LcSection icon="CircleCheck" title="Цели урока"
      subtitle="Что должен уметь студент после занятия — 2–5 пунктов"
      rightSlot={<span style={{ fontSize: 12, color: '#94a3b8' }}>{objectives.filter(o => o.trim()).length} / 5</span>}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        {objectives.map((o, i) => (
          <div key={i} style={{
            display: 'grid', gridTemplateColumns: '28px 1fr 32px', gap: 10,
            alignItems: 'center',
          }}>
            <div style={{
              width: 24, height: 24, borderRadius: 9999,
              background: o.trim() ? '#d1fae5' : '#f1f5f9',
              color: o.trim() ? '#047857' : '#94a3b8',
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              flexShrink: 0,
            }}>
              {o.trim() ? <Icon.Check size={13} sw={2.5} /> : (
                <span style={{ fontSize: 11, fontWeight: 600,
                  fontVariantNumeric: 'tabular-nums' }}>{i + 1}</span>
              )}
            </div>
            <input value={o} onChange={e => update(i, e.target.value)}
              placeholder={examples[i % examples.length]}
              style={{
                height: 38, borderRadius: 10, border: '1px solid #e2e8f0',
                background: '#fff', padding: '0 12px', fontSize: 13.5,
                fontFamily: 'inherit', color: '#0f172a', outline: 'none',
                transition: '.15s',
              }}
              onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.15)'; }}
              onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }}
            />
            <button type="button" onClick={() => remove(i)}
              title="Удалить" style={{
                width: 32, height: 32, borderRadius: 8, border: '1px solid transparent',
                background: 'transparent', color: '#94a3b8', cursor: 'pointer',
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              }}
              onMouseEnter={e => { e.currentTarget.style.background = '#fef2f2'; e.currentTarget.style.color = '#b91c1c'; }}
              onMouseLeave={e => { e.currentTarget.style.background = 'transparent'; e.currentTarget.style.color = '#94a3b8'; }}>
              <Icon.X size={14} />
            </button>
          </div>
        ))}
      </div>

      {objectives.length < 5 && (
        <button type="button" onClick={add}
          style={{
            marginTop: 12, display: 'inline-flex', alignItems: 'center', gap: 6,
            padding: '7px 12px', borderRadius: 8, border: '1px dashed #cbd5e1',
            background: '#fafbfc', color: '#475569', fontSize: 13,
            fontFamily: 'inherit', cursor: 'pointer',
          }}>
          <Icon.Plus size={14} />Добавить цель
        </button>
      )}
    </LcSection>
  );
}

// ── 4. Материалы — список приложенных файлов / ссылок ───────────────
function MaterialsSection({ value, onChange }) {
  const remove = (id) => onChange({ materials: value.materials.filter(m => m.id !== id) });
  return (
    <LcSection icon="FileText" title="Материалы урока"
      subtitle="PDF, презентации, аудио, ссылки — будут доступны студенту"
      rightSlot={<span style={{ fontSize: 12, color: '#94a3b8',
        fontVariantNumeric: 'tabular-nums' }}>{value.materials.length} файл.</span>}>

      <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginBottom: 14 }}>
        {value.materials.map(m => <MaterialRow key={m.id} mat={m} onRemove={() => remove(m.id)} />)}
      </div>

      {/* Drop zone */}
      <div style={{
        padding: '18px', borderRadius: 12, border: '1.5px dashed #cbd5e1',
        background: '#fafbfc', display: 'flex', alignItems: 'center',
        justifyContent: 'space-between', gap: 14,
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          <div style={{
            width: 40, height: 40, borderRadius: 10, background: '#fff',
            border: '1px solid #e2e8f0', display: 'inline-flex',
            alignItems: 'center', justifyContent: 'center', color: '#4f46e5',
          }}><Icon.Plus size={18} /></div>
          <div>
            <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>
              Перетащите файлы сюда
            </div>
            <div style={{ fontSize: 12.5, color: '#64748b' }}>
              PDF, DOCX, PPTX, MP3, MP4 · до 50 МБ за файл
            </div>
          </div>
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          <Button variant="secondary" size="sm">
            <Icon.FileText size={14} />Выбрать файл
          </Button>
          <Button variant="secondary" size="sm">
            <Icon.Mail size={14} />Вставить ссылку
          </Button>
        </div>
      </div>
    </LcSection>
  );
}

function MaterialRow({ mat, onRemove }) {
  const isLink = mat.kind === 'link';
  const accent = isLink ? { bg: '#eef2ff', fg: '#4338ca', icon: 'Mail' }
                        : { bg: '#fef3c7', fg: '#92400e', icon: 'FileText' };
  const Ic = Icon[accent.icon];
  return (
    <div style={{
      display: 'grid', gridTemplateColumns: '36px 1fr auto 32px', gap: 12,
      alignItems: 'center', padding: '10px 12px',
      border: '1px solid #e2e8f0', borderRadius: 10, background: '#fff',
    }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, background: accent.bg, color: accent.fg,
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      }}><Ic size={15} /></div>
      <div style={{ minWidth: 0 }}>
        <div style={{ fontSize: 13.5, fontWeight: 500, color: '#0f172a',
          overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
          {mat.name}
        </div>
        <div style={{ fontSize: 11.5, color: '#64748b',
          fontFamily: isLink ? 'var(--edv-font-mono)' : 'inherit' }}>
          {isLink ? mat.url : mat.size}
        </div>
      </div>
      <span style={{
        padding: '2px 8px', borderRadius: 6,
        background: '#f1f5f9', color: '#475569', fontSize: 11,
        fontFamily: 'var(--edv-font-mono)', textTransform: 'uppercase',
      }}>{isLink ? 'link' : mat.kind}</span>
      <button type="button" onClick={onRemove} title="Убрать"
        style={{
          width: 28, height: 28, borderRadius: 8, border: '1px solid transparent',
          background: 'transparent', color: '#94a3b8', cursor: 'pointer',
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}
        onMouseEnter={e => { e.currentTarget.style.background = '#fef2f2'; e.currentTarget.style.color = '#b91c1c'; }}
        onMouseLeave={e => { e.currentTarget.style.background = 'transparent'; e.currentTarget.style.color = '#94a3b8'; }}>
        <Icon.X size={13} />
      </button>
    </div>
  );
}

window.LcSection = LcSection;
window.PlacementSection = PlacementSection;
window.AboutSection = AboutSection;
window.ObjectivesSection = ObjectivesSection;
window.MaterialsSection = MaterialsSection;
