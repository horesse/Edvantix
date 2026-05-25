// Lesson create — правая колонка (status, видимость, preview, AI placeholder).

// ── Status panel — Draft / Planned / Published ──────────────────────
function StatusRail({ value, onChange }) {
  const options = [
    { value: 'draft',     icon: 'FileText',    label: 'Черновик',
      desc: 'Виден только вам и соавторам' },
    { value: 'planned',   icon: 'Clock',       label: 'Запланирован',
      desc: 'Появится в расписании, но скрыт до даты' },
    { value: 'published', icon: 'CircleCheck', label: 'Опубликован',
      desc: 'Доступен студентам всех привязанных групп' },
  ];
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '14px 16px',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12 }}>
        <Icon.Shield size={15} stroke="#475569" />
        <strong style={{ fontSize: 13.5, color: '#0f172a' }}>Статус публикации</strong>
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
        {options.map(o => {
          const active = value.status === o.value;
          const Ic = Icon[o.icon];
          const dotColor = {
            draft: '#f59e0b', planned: '#94a3b8', published: '#10b981',
          }[o.value];
          return (
            <button key={o.value} type="button"
              onClick={() => onChange({ status: o.value })}
              style={{
                display: 'grid', gridTemplateColumns: '24px 1fr 16px', gap: 10,
                alignItems: 'center', padding: '10px 12px', borderRadius: 10,
                border: `1px solid ${active ? '#4f46e5' : '#e2e8f0'}`,
                background: active ? 'rgba(79,70,229,0.04)' : '#fff',
                cursor: 'pointer', fontFamily: 'inherit', textAlign: 'left',
                transition: '.15s',
              }}>
              <span style={{ width: 20, height: 20, borderRadius: 9999,
                background: `${dotColor}22`, color: dotColor,
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              }}><Ic size={11} /></span>
              <div>
                <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{o.label}</div>
                <div style={{ fontSize: 11.5, color: '#64748b', lineHeight: 1.4, marginTop: 2 }}>
                  {o.desc}
                </div>
              </div>
              <span style={{
                width: 14, height: 14, borderRadius: 9999,
                border: `2px solid ${active ? '#4f46e5' : '#cbd5e1'}`,
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              }}>
                {active && <span style={{ width: 6, height: 6, borderRadius: 9999, background: '#4f46e5' }} />}
              </span>
            </button>
          );
        })}
      </div>
    </div>
  );
}

// ── Live preview — как урок будет выглядеть в списке курса ──────────
function LessonPreviewRail({ value, totalMin, moduleN }) {
  const t = window.LESSON_TYPES[value.type];
  const Ic = Icon[t.icon];
  const status = window.LESSON_STATUSES[value.status];
  const displayTitle = value.title.trim() || 'Без названия';
  const blocks = value.blocks.length;
  const materials = value.materials.length;

  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '14px 16px',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12 }}>
        <Icon.Eye size={15} stroke="#475569" />
        <strong style={{ fontSize: 13.5, color: '#0f172a' }}>Как это увидит студент</strong>
      </div>

      {/* Pseudo lesson row (как в module accordion) */}
      <div style={{
        padding: '12px 14px', borderRadius: 10, background: '#fafbfc',
        border: '1px solid #f1f5f9',
        display: 'grid', gridTemplateColumns: '32px 1fr', gap: 12, alignItems: 'center',
      }}>
        <div style={{
          width: 32, height: 32, borderRadius: 8,
          background: t.bg, color: t.fg,
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
          fontFamily: 'var(--edv-font-mono)', fontSize: 13, fontWeight: 600,
        }}><Ic size={14} /></div>
        <div style={{ minWidth: 0 }}>
          <div style={{
            fontSize: 13, fontWeight: 500, color: value.title.trim() ? '#0f172a' : '#94a3b8',
            lineHeight: 1.35, marginBottom: 4,
            overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
          }}>{displayTitle}</div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6, flexWrap: 'wrap',
            fontSize: 11, color: '#64748b' }}>
            <span style={{
              padding: '2px 6px', borderRadius: 5, background: t.bg, color: t.fg,
              fontWeight: 500,
            }}>{t.label}</span>
            <span style={{ color: '#cbd5e1' }}>·</span>
            <span style={{ fontVariantNumeric: 'tabular-nums' }}>{blocks} блоков</span>
            <span style={{ color: '#cbd5e1' }}>·</span>
            <span style={{ fontVariantNumeric: 'tabular-nums' }}>{totalMin} мин</span>
          </div>
        </div>
      </div>

      {/* Meta */}
      <div style={{
        marginTop: 10, display: 'flex', flexDirection: 'column', gap: 6,
        fontSize: 12, color: '#64748b',
      }}>
        <PreviewMetaRow label="Модуль"
          value={<span><span style={{
            fontFamily: 'var(--edv-font-mono)', color: '#4338ca', fontSize: 11,
            background: '#eef2ff', padding: '1px 6px', borderRadius: 5,
          }}>МОД {moduleN}</span></span>} />
        <PreviewMetaRow label="Статус"
          value={<span style={{ display: 'inline-flex', alignItems: 'center', gap: 5,
            color: status.fg, fontWeight: 500 }}>
            <span style={{ width: 6, height: 6, borderRadius: 9999, background: status.dot }} />
            {status.label}
          </span>} />
        <PreviewMetaRow label="Материалы"
          value={<span style={{ fontVariantNumeric: 'tabular-nums', color: '#0f172a' }}>{materials} файл.</span>} />
        <PreviewMetaRow label="Целей"
          value={<span style={{ fontVariantNumeric: 'tabular-nums', color: '#0f172a' }}>
            {value.objectives.filter(o => o.trim()).length}
          </span>} />
      </div>
    </div>
  );
}

function PreviewMetaRow({ label, value }) {
  return (
    <div style={{
      display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      padding: '4px 0',
    }}>
      <span style={{ fontSize: 11.5, color: '#94a3b8' }}>{label}</span>
      <span style={{ fontSize: 12 }}>{value}</span>
    </div>
  );
}

// ── AI assist teaser ────────────────────────────────────────────────
function AiAssistRail({ onClick }) {
  return (
    <div style={{
      borderRadius: 14,
      border: '1px solid #c7d6fe',
      background: 'linear-gradient(135deg, #f0f4ff 0%, #faf5ff 100%)',
      padding: '14px 16px', position: 'relative', overflow: 'hidden',
    }}>
      {/* sparkle dots */}
      <span style={{ position: 'absolute', top: 12, right: 14,
        width: 8, height: 8, borderRadius: 9999, background: '#a5b4fc' }} />
      <span style={{ position: 'absolute', top: 24, right: 28,
        width: 4, height: 4, borderRadius: 9999, background: '#c7d6fe' }} />

      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 8 }}>
        <span style={{
          width: 28, height: 28, borderRadius: 8,
          background: '#fff', border: '1px solid #c7d6fe', color: '#4f46e5',
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}><Icon.Sparkles size={14} /></span>
        <strong style={{ fontSize: 13.5, color: '#0f172a' }}>Помощник AI</strong>
        <span style={{
          marginLeft: 'auto', fontSize: 9, fontWeight: 700, letterSpacing: '0.08em',
          padding: '2px 6px', borderRadius: 4, background: '#4f46e5', color: '#fff',
        }}>BETA</span>
      </div>
      <div style={{ fontSize: 12.5, color: '#475569', lineHeight: 1.5 }}>
        Сгенерировать структуру урока, цели и упражнения по теме «{'<заголовок>'}»
      </div>
      <button onClick={onClick} style={{
        marginTop: 12, width: '100%', height: 34, borderRadius: 8,
        border: 'none', background: '#4f46e5', color: '#fff',
        fontSize: 12.5, fontWeight: 600, fontFamily: 'inherit',
        cursor: 'pointer',
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center', gap: 6,
      }}>
        <Icon.Sparkles size={13} />Заполнить с AI
      </button>
    </div>
  );
}

// ── Save bar (sticky bottom) — близок к EditApp SaveBar ─────────────
function LcSaveBar({ canPublish, savingState, errorCount, validationItems, onSaveDraft, onPublish, onCancel }) {
  const hasErrors = errorCount > 0;
  return (
    <div style={{
      position: 'absolute', left: 240, right: 0, bottom: 0,
      background: '#fff', borderTop: '1px solid #e2e8f0',
      boxShadow: '0 -4px 12px rgba(15,23,42,0.06)',
      padding: '12px 32px',
      display: 'flex', alignItems: 'center', gap: 16,
    }}>
      {/* Validation status */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, flex: 1, minWidth: 0 }}>
        <div style={{
          width: 32, height: 32, borderRadius: 9999, flexShrink: 0,
          background: hasErrors ? 'rgba(239,68,68,0.10)' : 'rgba(16,185,129,0.10)',
          color: hasErrors ? '#b91c1c' : '#047857',
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}>
          {hasErrors
            ? <Icon.AlertCircle size={16} stroke="currentColor" />
            : <Icon.Check size={15} stroke="currentColor" sw={2.5} />}
        </div>
        <div style={{ minWidth: 0 }}>
          <div style={{ fontSize: 13, fontWeight: 600,
            color: hasErrors ? '#991b1b' : '#0f172a' }}>
            {hasErrors
              ? `Нужно ${errorCount} ${declensionLC(errorCount, ['исправление', 'исправления', 'исправлений'])} перед публикацией`
              : 'Урок готов к публикации'}
          </div>
          <div style={{ fontSize: 11.5, color: '#64748b',
            display: 'flex', gap: 10, flexWrap: 'wrap', marginTop: 2 }}>
            {validationItems.map((v, i) => (
              <span key={i} style={{ display: 'inline-flex', alignItems: 'center', gap: 4,
                color: v.ok ? '#047857' : '#94a3b8' }}>
                {v.ok ? <Icon.Check size={11} stroke="currentColor" sw={3} />
                      : <span style={{ width: 4, height: 4, borderRadius: 9999, background: '#cbd5e1' }} />}
                {v.label}
              </span>
            ))}
          </div>
        </div>
      </div>

      {/* Actions */}
      <div style={{ display: 'flex', gap: 10, flexShrink: 0 }}>
        <Button variant="ghost" onClick={onCancel}>Отмена</Button>
        <Button variant="secondary" onClick={onSaveDraft}
          disabled={savingState === 'saving'}>
          <Icon.FileText size={14} />Сохранить черновик
        </Button>
        <Button onClick={onPublish}
          disabled={!canPublish || savingState === 'saving'}>
          {savingState === 'saving'
            ? <><LcSpinner />Публикуем…</>
            : <><Icon.Check size={15} sw={2.5} />Опубликовать урок</>}
        </Button>
      </div>
    </div>
  );
}

function LcSpinner() {
  return (
    <span style={{
      display: 'inline-block', width: 13, height: 13,
      border: '2px solid rgba(255,255,255,0.35)', borderTopColor: '#fff',
      borderRadius: 9999, animation: 'spin 0.7s linear infinite',
    }} />
  );
}

function declensionLC(n, forms) {
  const abs = Math.abs(n);
  const mod10 = abs % 10, mod100 = abs % 100;
  if (mod10 === 1 && mod100 !== 11) return forms[0];
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return forms[1];
  return forms[2];
}

window.StatusRail = StatusRail;
window.LessonPreviewRail = LessonPreviewRail;
window.AiAssistRail = AiAssistRail;
window.LcSaveBar = LcSaveBar;
