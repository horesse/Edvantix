// Lesson create — билдер структуры урока.
// Урок = упорядоченный список блоков. Каждый блок: тип, заголовок, длительность,
// заметки, ссылки и привязка (учебник/упражнения).
// Поддерживаем: добавление через палитру, удаление, дублирование, drag-reorder,
// инлайновое редактирование, expand для подробностей.

const COLOR_BY_BLOCK = {
  intro:     '#94a3b8',
  theory:    '#4f46e5',
  video:     '#6366f1',
  exercise:  '#0ea5e9',
  speaking:  '#0d9488',
  listening: '#a855f7',
  writing:   '#d97706',
  quiz:      '#ef4444',
  homework:  '#475569',
};

function StructureSection({ value, onChange, totalMin }) {
  const blocks = value.blocks;
  const [paletteOpen, setPaletteOpen] = React.useState(false);
  const [insertAfter, setInsertAfter] = React.useState(null);
  const [dragId, setDragId] = React.useState(null);
  const [dragOverId, setDragOverId] = React.useState(null);
  const [expandedId, setExpandedId] = React.useState(null);

  const updateBlock = (id, patch) => {
    onChange({
      blocks: blocks.map(b => b.id === id ? { ...b, ...patch } : b),
    });
  };
  const removeBlock = (id) =>
    onChange({ blocks: blocks.filter(b => b.id !== id) });
  const duplicateBlock = (id) => {
    const idx = blocks.findIndex(b => b.id === id);
    const b = blocks[idx];
    const copy = { ...b, id: `b${Date.now()}-${Math.random().toString(36).slice(2,5)}` };
    const next = blocks.slice();
    next.splice(idx + 1, 0, copy);
    onChange({ blocks: next });
  };
  const insertBlock = (type, afterId) => {
    const newBlock = makeNewBlock(type);
    if (!afterId) {
      onChange({ blocks: [...blocks, newBlock] });
    } else {
      const idx = blocks.findIndex(b => b.id === afterId);
      const next = blocks.slice();
      next.splice(idx + 1, 0, newBlock);
      onChange({ blocks: next });
    }
    setPaletteOpen(false);
    setInsertAfter(null);
    setExpandedId(newBlock.id);
  };

  // Drag & drop reorder
  const onDragStart = (e, id) => {
    setDragId(id);
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/plain', id);
  };
  const onDragOver = (e, id) => {
    e.preventDefault();
    if (id !== dragOverId) setDragOverId(id);
  };
  const onDrop = (e, targetId) => {
    e.preventDefault();
    if (!dragId || dragId === targetId) { setDragId(null); setDragOverId(null); return; }
    const from = blocks.findIndex(b => b.id === dragId);
    const to = blocks.findIndex(b => b.id === targetId);
    if (from < 0 || to < 0) return;
    const next = blocks.slice();
    const [moved] = next.splice(from, 1);
    next.splice(to, 0, moved);
    onChange({ blocks: next });
    setDragId(null);
    setDragOverId(null);
  };
  const onDragEnd = () => { setDragId(null); setDragOverId(null); };

  return (
    <LcSection icon="Sparkles" title="Структура урока"
      subtitle="Из чего собирается занятие — последовательность блоков для плеера"
      rightSlot={
        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <span style={{
            display: 'inline-flex', alignItems: 'center', gap: 6,
            padding: '5px 12px', borderRadius: 9999,
            background: '#f1f5f9', color: '#0f172a',
            fontSize: 12, fontWeight: 600,
          }}>
            <Icon.Clock size={12} stroke="#64748b" />
            <span style={{ fontVariantNumeric: 'tabular-nums' }}>~{totalMin} мин</span>
          </span>
          <span style={{ fontSize: 11.5, color: '#94a3b8' }}>
            рассчитано из блоков
          </span>
        </div>
      }>

      {/* Meta bar */}
      <StructureMetaBar blocks={blocks} totalMin={totalMin} />

      {/* Empty state */}
      {blocks.length === 0 && (
        <div style={{
          marginTop: 12, padding: '32px 20px', borderRadius: 12,
          border: '1.5px dashed #cbd5e1', background: '#fafbfc',
          textAlign: 'center',
        }}>
          <div style={{
            width: 44, height: 44, borderRadius: 12, background: '#fff',
            border: '1px solid #e2e8f0', display: 'inline-flex',
            alignItems: 'center', justifyContent: 'center', color: '#4f46e5',
            marginBottom: 10,
          }}><Icon.Sparkles size={20} /></div>
          <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>
            Пока ни одного блока
          </div>
          <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 4, maxWidth: 320, margin: '4px auto 0' }}>
            Добавьте первый блок или загрузите шаблон под выбранный тип урока
          </div>
          <div style={{ marginTop: 14, display: 'inline-flex', gap: 10 }}>
            <Button size="sm" onClick={() => { setInsertAfter(null); setPaletteOpen(true); }}>
              <Icon.Plus size={14} />Добавить блок
            </Button>
            <Button variant="secondary" size="sm"
              onClick={() => onChange({
                blocks: LESSON_TEMPLATES[value.type].map((t, i) => ({
                  id: `b${Date.now()}-${i}`,
                  type: t,
                  title: BLOCK_DEFAULT_TITLES[t],
                  durationMin: BLOCK_LIBRARY.find(b => b.type === t).defaultMin,
                  notes: '', links: [], reference: '',
                })),
              })}>
              <Icon.Sparkles size={14} />Загрузить шаблон
            </Button>
          </div>
        </div>
      )}

      {/* Block list */}
      {blocks.length > 0 && (
        <div style={{ marginTop: 14, display: 'flex', flexDirection: 'column', gap: 8 }}>
          {blocks.map((b, i) => (
            <BlockCard
              key={b.id}
              block={b}
              index={i + 1}
              dragging={dragId === b.id}
              dragOver={dragOverId === b.id && dragId !== b.id}
              expanded={expandedId === b.id}
              onDragStart={e => onDragStart(e, b.id)}
              onDragOver={e => onDragOver(e, b.id)}
              onDrop={e => onDrop(e, b.id)}
              onDragEnd={onDragEnd}
              onToggleExpand={() => setExpandedId(expandedId === b.id ? null : b.id)}
              onPatch={(patch) => updateBlock(b.id, patch)}
              onRemove={() => removeBlock(b.id)}
              onDuplicate={() => duplicateBlock(b.id)}
              onInsertAfter={() => { setInsertAfter(b.id); setPaletteOpen(true); }}
            />
          ))}

          {/* Final add */}
          <button type="button"
            onClick={() => { setInsertAfter(null); setPaletteOpen(true); }}
            style={{
              marginTop: 4, display: 'flex', alignItems: 'center', gap: 8,
              width: '100%', padding: '12px 14px',
              border: '1.5px dashed #cbd5e1', background: '#fafbfc',
              borderRadius: 10, color: '#4f46e5', fontFamily: 'inherit',
              fontSize: 13, fontWeight: 600, cursor: 'pointer',
              transition: '.15s',
            }}
            onMouseEnter={e => { e.currentTarget.style.borderColor = '#a5b4fc'; e.currentTarget.style.background = '#f5f7ff'; }}
            onMouseLeave={e => { e.currentTarget.style.borderColor = '#cbd5e1'; e.currentTarget.style.background = '#fafbfc'; }}>
            <Icon.Plus size={14} />Добавить блок в конец урока
          </button>
        </div>
      )}

      {paletteOpen && (
        <BlockPalette
          onClose={() => { setPaletteOpen(false); setInsertAfter(null); }}
          onPick={(type) => insertBlock(type, insertAfter)}
        />
      )}
    </LcSection>
  );
}

// ── Meta bar with stacked time chart + tally legend ─────────────────
function StructureMetaBar({ blocks, totalMin }) {
  const tally = {};
  blocks.forEach(b => { tally[b.type] = (tally[b.type] || 0) + 1; });
  const order = Object.keys(window.BLOCK_TYPES).filter(t => tally[t]);

  if (blocks.length === 0) return null;

  return (
    <div style={{
      padding: '12px 14px', background: '#fafbfc', border: '1px solid #f1f5f9',
      borderRadius: 12, display: 'flex', alignItems: 'center', gap: 16, flexWrap: 'wrap',
    }}>
      <div style={{ flex: 1, minWidth: 220 }}>
        <div style={{
          height: 10, borderRadius: 9999, overflow: 'hidden', background: '#e2e8f0',
          display: 'flex',
        }}>
          {blocks.map(b => (
            <div key={b.id}
              title={`${window.BLOCK_TYPES[b.type].label} · ${b.durationMin} мин`}
              style={{
                width: `${(b.durationMin / Math.max(1, totalMin)) * 100}%`,
                background: COLOR_BY_BLOCK[b.type] || '#94a3b8',
                borderRight: '1px solid rgba(255,255,255,0.4)',
              }} />
          ))}
        </div>
        <div style={{
          marginTop: 6, display: 'flex', justifyContent: 'space-between',
          fontSize: 11, color: '#94a3b8', fontVariantNumeric: 'tabular-nums',
        }}>
          <span>{blocks.length} блок.</span>
          <span>≈ {Math.round(totalMin / 60 * 10) / 10} ч</span>
        </div>
      </div>

      <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
        {order.map(t => (
          <span key={t} style={{
            display: 'inline-flex', alignItems: 'center', gap: 6,
            padding: '3px 9px', borderRadius: 9999,
            background: '#fff', border: '1px solid #e2e8f0',
            fontSize: 11.5, color: '#475569',
          }}>
            <span style={{ width: 6, height: 6, borderRadius: 9999,
              background: COLOR_BY_BLOCK[t] }} />
            {window.BLOCK_TYPES[t].label}
            <span style={{ fontVariantNumeric: 'tabular-nums', color: '#94a3b8' }}>×{tally[t]}</span>
          </span>
        ))}
      </div>
    </div>
  );
}

// ── Block card (collapsed + expanded details) ───────────────────────
function BlockCard({
  block, index, dragging, dragOver, expanded,
  onDragStart, onDragOver, onDrop, onDragEnd,
  onToggleExpand, onPatch, onRemove, onDuplicate, onInsertAfter,
}) {
  const t = window.BLOCK_TYPES[block.type];
  const Ic = Icon[t.icon];
  const accent = COLOR_BY_BLOCK[block.type] || '#475569';
  const description = window.BLOCK_LIBRARY.find(b => b.type === block.type)?.description || '';

  // Indicators for which details exist (notes / links / reference)
  const detailFlags = {
    notes: !!(block.notes || '').trim(),
    links: (block.links || []).length > 0,
    reference: !!(block.reference || '').trim(),
  };
  const detailCount = Object.values(detailFlags).filter(Boolean).length;

  return (
    <div
      onDragOver={onDragOver}
      onDrop={onDrop}
      style={{
        border: `1px solid ${dragOver ? '#4f46e5' : '#e2e8f0'}`,
        borderRadius: 12, background: '#fff',
        boxShadow: dragOver ? '0 0 0 3px rgba(79,70,229,0.12)' : 'none',
        opacity: dragging ? 0.4 : 1,
        transition: 'box-shadow .15s, border-color .15s',
        overflow: 'hidden',
      }}>

      {/* Header row */}
      <div
        draggable
        onDragStart={onDragStart}
        onDragEnd={onDragEnd}
        style={{
          display: 'grid',
          gridTemplateColumns: '20px 36px 36px 1fr auto auto auto',
          gap: 12, alignItems: 'center',
          padding: '10px 12px 10px 8px',
        }}>

        {/* Drag handle */}
        <span style={{
          width: 20, color: '#cbd5e1', cursor: 'grab',
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }} title="Перетащите, чтобы изменить порядок">
          <svg width="10" height="14" viewBox="0 0 10 14" fill="currentColor">
            <circle cx="2" cy="2" r="1.2"/><circle cx="8" cy="2" r="1.2"/>
            <circle cx="2" cy="7" r="1.2"/><circle cx="8" cy="7" r="1.2"/>
            <circle cx="2" cy="12" r="1.2"/><circle cx="8" cy="12" r="1.2"/>
          </svg>
        </span>

        {/* Index */}
        <span style={{
          width: 36, fontFamily: 'var(--edv-font-mono)', fontSize: 12,
          color: '#94a3b8', textAlign: 'right',
        }}>{String(index).padStart(2, '0')}</span>

        {/* Type icon */}
        <span style={{
          width: 36, height: 36, borderRadius: 10, flexShrink: 0,
          background: `${accent}1a`, color: accent,
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}><Ic size={16} stroke="currentColor" /></span>

        {/* Title + type label */}
        <div style={{ minWidth: 0 }}>
          <InlineTitleInput value={block.title}
            onChange={v => onPatch({ title: v })}
            placeholder={BLOCK_DEFAULT_TITLES[block.type]} />
          <div style={{ fontSize: 11.5, color: '#64748b', marginTop: 2,
            display: 'flex', alignItems: 'center', gap: 6,
            overflow: 'hidden', whiteSpace: 'nowrap', textOverflow: 'ellipsis' }}>
            <span style={{ color: accent, fontWeight: 600 }}>{t.label}</span>
            <span style={{ color: '#cbd5e1' }}>·</span>
            <span style={{ fontFamily: 'var(--edv-font-mono)' }}>{description}</span>
          </div>
        </div>

        {/* Duration */}
        <div style={{
          display: 'inline-flex', alignItems: 'center', gap: 4,
          padding: '5px 10px', borderRadius: 8, background: '#fafbfc',
          border: '1px solid #e2e8f0',
        }}>
          <input type="number" min={1} max={120} value={block.durationMin}
            onChange={e => onPatch({ durationMin: parseInt(e.target.value || 0, 10) })}
            style={{
              width: 36, border: 'none', background: 'transparent',
              textAlign: 'right', fontSize: 13, fontWeight: 600, color: '#0f172a',
              fontVariantNumeric: 'tabular-nums', outline: 'none', fontFamily: 'inherit',
            }} />
          <span style={{ fontSize: 11.5, color: '#64748b' }}>мин</span>
        </div>

        {/* Detail indicators (only when collapsed) */}
        {!expanded && detailCount > 0 && (
          <div style={{ display: 'inline-flex', gap: 5, paddingLeft: 4 }}>
            {detailFlags.notes &&
              <DetailDot title="Есть заметка" icon="FileText" color="#475569" />}
            {detailFlags.links &&
              <DetailDot title={`Ссылок: ${block.links.length}`} icon="Mail" color="#4338ca" />}
            {detailFlags.reference &&
              <DetailDot title="Привязка к учебнику" icon="BookOpen" color="#0369a1" />}
          </div>
        )}
        {(!expanded && detailCount === 0) && <span />}

        {/* Actions */}
        <div style={{ display: 'inline-flex', gap: 2 }}>
          <RowIconBtn title={expanded ? 'Свернуть' : 'Подробнее'} onClick={onToggleExpand}
            highlight={expanded}>
            <Icon.ChevronDown size={14}
              style={{ transform: expanded ? 'none' : 'rotate(-90deg)', transition: 'transform .15s' }} />
          </RowIconBtn>
          <RowIconBtn title="Дублировать" onClick={onDuplicate}>
            <Icon.FileText size={14} />
          </RowIconBtn>
          <RowIconBtn title="Вставить блок ниже" onClick={onInsertAfter}>
            <Icon.Plus size={14} />
          </RowIconBtn>
          <RowIconBtn title="Удалить" onClick={onRemove} danger>
            <Icon.X size={14} />
          </RowIconBtn>
        </div>
      </div>

      {/* Expanded details */}
      {expanded && (
        <BlockDetails block={block} accent={accent} onPatch={onPatch} />
      )}
    </div>
  );
}

function DetailDot({ title, icon, color }) {
  const Ic = Icon[icon];
  return (
    <span title={title} style={{
      width: 22, height: 22, borderRadius: 6, background: `${color}14`, color,
      display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
    }}><Ic size={11} /></span>
  );
}

// ── Inline title input — выглядит как текст, но всегда редактируем ───
function InlineTitleInput({ value, onChange, placeholder }) {
  const [focused, setFocused] = React.useState(false);
  return (
    <input value={value} placeholder={placeholder}
      onChange={e => onChange(e.target.value)}
      onFocus={() => setFocused(true)}
      onBlur={() => setFocused(false)}
      style={{
        width: '100%', height: 28, padding: '0 8px', marginLeft: -8,
        borderRadius: 6, border: `1px solid ${focused ? '#6366f1' : 'transparent'}`,
        background: focused ? '#fff' : 'transparent',
        boxShadow: focused ? '0 0 0 3px rgba(99,102,241,0.15)' : 'none',
        fontSize: 13.5, fontWeight: 500, color: '#0f172a',
        fontFamily: 'inherit', outline: 'none',
        transition: '.1s',
      }}
      onMouseEnter={e => { if (!focused) e.currentTarget.style.background = '#f8fafc'; }}
      onMouseLeave={e => { if (!focused) e.currentTarget.style.background = 'transparent'; }} />
  );
}

function RowIconBtn({ children, onClick, title, danger, highlight }) {
  return (
    <button type="button" onClick={onClick} title={title}
      style={{
        width: 30, height: 30, borderRadius: 8, border: '1px solid transparent',
        background: highlight ? '#eef2ff' : 'transparent',
        color: highlight ? '#4f46e5' : '#94a3b8',
        cursor: 'pointer',
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        transition: '.1s',
      }}
      onMouseEnter={e => {
        e.currentTarget.style.background = danger ? '#fef2f2' : highlight ? '#e0eaff' : '#f1f5f9';
        e.currentTarget.style.color = danger ? '#b91c1c' : highlight ? '#4338ca' : '#475569';
      }}
      onMouseLeave={e => {
        e.currentTarget.style.background = highlight ? '#eef2ff' : 'transparent';
        e.currentTarget.style.color = highlight ? '#4f46e5' : '#94a3b8';
      }}>
      {children}
    </button>
  );
}

// ── Expanded block details: notes, links, reference ─────────────────
function BlockDetails({ block, accent, onPatch }) {
  const links = block.links || [];
  const addLink = () => onPatch({ links: [...links, { url: '', label: '' }] });
  const updateLink = (i, patch) => {
    const next = links.slice();
    next[i] = { ...next[i], ...patch };
    onPatch({ links: next });
  };
  const removeLink = (i) => onPatch({ links: links.filter((_, j) => j !== i) });

  return (
    <div style={{
      padding: '14px 16px 16px 60px',
      background: '#fafbfc', borderTop: '1px solid #f1f5f9',
      display: 'flex', flexDirection: 'column', gap: 14,
    }}>
      {/* Notes */}
      <DetailField icon="FileText" label="Заметки для преподавателя"
        hint="Подсказки по подаче, где сделать паузу, на что обратить внимание">
        <textarea
          value={block.notes || ''}
          onChange={e => onPatch({ notes: e.target.value })}
          placeholder="Что важно сказать или показать в этом блоке…"
          rows={2}
          style={{
            width: '100%', borderRadius: 10, border: '1px solid #e2e8f0',
            background: '#fff', padding: '8px 12px',
            fontSize: 13, fontFamily: 'inherit', color: '#0f172a',
            outline: 'none', resize: 'vertical', minHeight: 60, lineHeight: 1.5,
            transition: '.15s',
          }}
          onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.15)'; }}
          onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }} />
      </DetailField>

      {/* Reference (учебник / страницы / упражнения) */}
      <DetailField icon="BookOpen" label="Привязка к материалам"
        hint="Учебник, страницы, номера упражнений или ссылка на конкретный материал">
        <input
          value={block.reference || ''}
          onChange={e => onPatch({ reference: e.target.value })}
          placeholder="Например: Murphy, Unit 4, упр. 4.2–4.5"
          style={{
            width: '100%', height: 36, borderRadius: 10,
            border: '1px solid #e2e8f0', background: '#fff', padding: '0 12px',
            fontSize: 13, fontFamily: 'inherit', color: '#0f172a', outline: 'none',
            transition: '.15s',
          }}
          onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.15)'; }}
          onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }} />
      </DetailField>

      {/* Links */}
      <DetailField icon="Mail" label="Ссылки"
        hint="Видео, статьи, аудио, тесты — будут открываться в плеере блока"
        action={
          <button type="button" onClick={addLink}
            style={{
              display: 'inline-flex', alignItems: 'center', gap: 4,
              padding: '4px 8px', borderRadius: 6, border: '1px solid #e2e8f0',
              background: '#fff', color: accent, fontSize: 11.5, fontWeight: 600,
              fontFamily: 'inherit', cursor: 'pointer',
            }}>
            <Icon.Plus size={11} sw={2.5} />Добавить
          </button>
        }>
        {links.length === 0 ? (
          <div style={{
            padding: '10px 12px', borderRadius: 8, border: '1px dashed #cbd5e1',
            fontSize: 12, color: '#94a3b8', textAlign: 'center',
            fontFamily: 'var(--edv-font-mono)', background: '#fff',
          }}>
            {'// пока ссылок нет'}
          </div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
            {links.map((l, i) => (
              <div key={i} style={{
                display: 'grid', gridTemplateColumns: '160px 1fr 28px', gap: 6,
                alignItems: 'center',
              }}>
                <input
                  value={l.label}
                  onChange={e => updateLink(i, { label: e.target.value })}
                  placeholder="Подпись"
                  style={{
                    height: 32, borderRadius: 8, border: '1px solid #e2e8f0',
                    background: '#fff', padding: '0 10px',
                    fontSize: 12.5, fontFamily: 'inherit', color: '#0f172a',
                    outline: 'none',
                  }} />
                <input
                  value={l.url}
                  onChange={e => updateLink(i, { url: e.target.value })}
                  placeholder="https://…"
                  style={{
                    height: 32, borderRadius: 8, border: '1px solid #e2e8f0',
                    background: '#fff', padding: '0 10px',
                    fontSize: 12.5, fontFamily: 'var(--edv-font-mono)',
                    color: '#0f172a', outline: 'none',
                  }} />
                <button type="button" onClick={() => removeLink(i)}
                  style={{
                    width: 28, height: 28, borderRadius: 8, border: 'none',
                    background: 'transparent', color: '#94a3b8', cursor: 'pointer',
                    display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                  }}
                  onMouseEnter={e => { e.currentTarget.style.background = '#fef2f2'; e.currentTarget.style.color = '#b91c1c'; }}
                  onMouseLeave={e => { e.currentTarget.style.background = 'transparent'; e.currentTarget.style.color = '#94a3b8'; }}>
                  <Icon.X size={13} />
                </button>
              </div>
            ))}
          </div>
        )}
      </DetailField>
    </div>
  );
}

function DetailField({ icon, label, hint, action, children }) {
  const Ic = Icon[icon];
  return (
    <div>
      <div style={{
        display: 'flex', alignItems: 'center', gap: 8, marginBottom: 6,
      }}>
        <Ic size={13} stroke="#64748b" />
        <span style={{ fontSize: 11.5, fontWeight: 600, color: '#334155' }}>
          {label}
        </span>
        {hint && <span style={{ fontSize: 11, color: '#94a3b8' }}>· {hint}</span>}
        <span style={{ marginLeft: 'auto' }}>{action}</span>
      </div>
      {children}
    </div>
  );
}

// ── Block palette modal ─────────────────────────────────────────────
function BlockPalette({ onClose, onPick }) {
  const categories = Object.entries(window.BLOCK_CATEGORIES);
  return (
    <div onClick={onClose} style={{
      position: 'fixed', inset: 0, zIndex: 60, padding: 20,
      background: 'rgba(15,23,42,0.42)', backdropFilter: 'blur(3px)',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      animation: 'fadeIn .15s ease-out',
    }}>
      <div onClick={e => e.stopPropagation()} style={{
        background: '#fff', borderRadius: 16, maxWidth: 640, width: '100%',
        boxShadow: '0 25px 50px -12px rgba(0,0,0,0.25)',
        overflow: 'hidden',
      }}>
        <div style={{
          padding: '18px 22px 16px', borderBottom: '1px solid #f1f5f9',
          display: 'flex', alignItems: 'center', gap: 12,
        }}>
          <div style={{
            width: 36, height: 36, borderRadius: 10,
            background: 'rgba(79,70,229,0.08)', color: '#4f46e5',
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
          }}><Icon.Plus size={18} /></div>
          <div style={{ flex: 1 }}>
            <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>
              Добавить блок
            </div>
            <div style={{ fontSize: 12.5, color: '#64748b' }}>
              Выберите тип контента, который добавится в урок
            </div>
          </div>
          <button onClick={onClose} style={{
            width: 30, height: 30, borderRadius: 8, border: 'none',
            background: '#f1f5f9', color: '#64748b', cursor: 'pointer',
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
          }}><Icon.X size={15} /></button>
        </div>

        <div style={{ padding: '14px 22px 20px', maxHeight: '60vh', overflowY: 'auto' }}>
          {categories.map(([catKey, cat]) => {
            const items = window.BLOCK_LIBRARY.filter(b => b.category === catKey);
            return (
              <div key={catKey} style={{ marginTop: 14 }}>
                <div style={{
                  fontSize: 10, fontWeight: 600, letterSpacing: '0.1em',
                  textTransform: 'uppercase', color: cat.fg, marginBottom: 8,
                }}>{cat.label}</div>
                <div style={{
                  display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 8,
                }}>
                  {items.map(b => {
                    const def = window.BLOCK_TYPES[b.type];
                    const Ic = Icon[def.icon];
                    const accent = COLOR_BY_BLOCK[b.type];
                    return (
                      <button key={b.type} onClick={() => onPick(b.type)}
                        style={{
                          textAlign: 'left', padding: '12px 12px', borderRadius: 12,
                          border: '1px solid #e2e8f0', background: '#fff',
                          cursor: 'pointer', fontFamily: 'inherit',
                          display: 'flex', flexDirection: 'column', gap: 8,
                          transition: '.15s',
                        }}
                        onMouseEnter={e => { e.currentTarget.style.borderColor = accent; e.currentTarget.style.background = `${accent}08`; }}
                        onMouseLeave={e => { e.currentTarget.style.borderColor = '#e2e8f0'; e.currentTarget.style.background = '#fff'; }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                          <span style={{
                            width: 30, height: 30, borderRadius: 8,
                            background: `${accent}1a`, color: accent,
                            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                          }}><Ic size={15} /></span>
                          <span style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>
                            {def.label}
                          </span>
                          <span style={{
                            marginLeft: 'auto', fontSize: 11,
                            fontVariantNumeric: 'tabular-nums', color: '#94a3b8',
                            fontFamily: 'var(--edv-font-mono)',
                          }}>{b.defaultMin}м</span>
                        </div>
                        <div style={{ fontSize: 12, color: '#64748b', lineHeight: 1.45 }}>
                          {b.description}
                        </div>
                      </button>
                    );
                  })}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

window.StructureSection = StructureSection;
