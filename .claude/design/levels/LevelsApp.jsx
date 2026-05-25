// Уровень — справочник организации (шаблон).
// Шаблон состоит из: breadcrumb -> page header -> toolbar -> table -> drawer.
// Все остальные справочники (предметы, типы занятий, статусы и т.д.) реализуются по этой же схеме —
// меняется только конфиг колонок, набор полей в drawer и текст плейсхолдера.

const { useState, useMemo, useEffect } = React;

// Локальные иконки, которые нужны странице, но отсутствуют в общем kit/Icons.jsx
const LevelIcons = {
  GripVertical: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" {...p}>
    <circle cx="9" cy="6" r="1"/><circle cx="9" cy="12" r="1"/><circle cx="9" cy="18" r="1"/>
    <circle cx="15" cy="6" r="1"/><circle cx="15" cy="12" r="1"/><circle cx="15" cy="18" r="1"/>
  </svg>,
  Pencil: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5z"/>
  </svg>,
  Trash: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M3 6h18M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/>
  </svg>,
  Archive: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <rect x="2" y="3" width="20" height="5" rx="1"/><path d="M4 8v11a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8M10 12h4"/>
  </svg>,
  MoreHorizontal: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="12" cy="12" r="1"/><circle cx="19" cy="12" r="1"/><circle cx="5" cy="12" r="1"/>
  </svg>,
  Download: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M7 10l5 5 5-5M12 15V3"/>
  </svg>,
  Upload: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M17 8l-5-5-5 5M12 3v12"/>
  </svg>,
  Inbox: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M22 12h-6l-2 3h-4l-2-3H2"/><path d="M5.45 5.11 2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z"/>
  </svg>,
  Layers: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="m12 2 10 6-10 6L2 8z"/><path d="m2 17 10 6 10-6"/><path d="m2 12 10 6 10-6"/>
  </svg>,
};
Object.assign(Icon, LevelIcons);

// ──────────────────────────────────────────────────────────────────────────
// Конфиг шаблона — для нового справочника меняется только этот объект.
const DIRECTORY = {
  slug: 'level',
  singular: 'уровень',
  plural: 'Уровни',
  description: 'Уровни обучения, по которым формируются группы и подбираются программы. Используется в курсах, расписании и зачислении.',
  icon: 'Layers',
};

// Шаблон по умолчанию для tweaks
const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "density": "comfortable",
  "showColorDot": true,
  "showCode": true,
  "showDescription": true,
  "showOrder": true,
  "groupArchived": "tab"
}/*EDITMODE-END*/;

function LevelsApp() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);

  const [items, setItems] = useState(LEVELS);
  const [query, setQuery] = useState('');
  const [tab, setTab] = useState('active'); // active | archived
  const [drawer, setDrawer] = useState({ open: false, mode: 'create', initial: null });
  const [menuFor, setMenuFor] = useState(null);

  // Close any open row menu on outside click.
  useEffect(() => {
    const onDoc = () => setMenuFor(null);
    document.addEventListener('click', onDoc);
    return () => document.removeEventListener('click', onDoc);
  }, []);

  const counts = useMemo(() => ({
    active: items.filter(i => i.status === 'active').length,
    archived: items.filter(i => i.status === 'archived').length,
  }), [items]);

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    return items
      .filter(i => t.groupArchived === 'mixed' ? true : i.status === tab)
      .filter(i => !q || i.name.toLowerCase().includes(q) || (i.code||'').toLowerCase().includes(q) || (i.description||'').toLowerCase().includes(q))
      .sort((a, b) => a.order - b.order);
  }, [items, query, tab, t.groupArchived]);

  const openCreate = () => setDrawer({ open: true, mode: 'create', initial: null });
  const openEdit = (item) => setDrawer({ open: true, mode: 'edit', initial: item });
  const closeDrawer = () => setDrawer(d => ({ ...d, open: false }));

  const handleSave = (form) => {
    setItems(prev => {
      if (drawer.mode === 'create') {
        const maxOrder = prev.reduce((m, i) => Math.max(m, i.order), 0);
        return [...prev, {
          id: `l-${Date.now()}`, ...form, order: maxOrder + 1,
          usage: { groups: 0, courses: 0, students: 0 },
        }];
      }
      return prev.map(i => i.id === drawer.initial.id ? { ...i, ...form } : i);
    });
    closeDrawer();
  };
  const handleArchive = (item) => {
    setItems(prev => prev.map(i => i.id === item.id ? { ...i, status: i.status === 'active' ? 'archived' : 'active' } : i));
    setMenuFor(null);
  };
  const handleDelete = (item) => {
    setItems(prev => prev.filter(i => i.id !== item.id));
    closeDrawer();
  };
  const reorder = (fromId, toId) => {
    setItems(prev => {
      const list = [...prev].sort((a, b) => a.order - b.order);
      const fromIdx = list.findIndex(i => i.id === fromId);
      const toIdx   = list.findIndex(i => i.id === toId);
      if (fromIdx < 0 || toIdx < 0) return prev;
      const [moved] = list.splice(fromIdx, 1);
      list.splice(toIdx, 0, moved);
      return list.map((i, idx) => ({ ...i, order: idx + 1 }));
    });
  };

  return (
    <div style={{ display: 'flex', height: '100vh', minHeight: 700, background: '#f8fafc', overflow: 'hidden' }}>
      <Sidebar active="settings" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Настройки</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span>Справочники организации</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>{DIRECTORY.plural}</span>
        </div>

        {/* Page header */}
        <div style={{
          padding: '24px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', flexDirection: 'column', gap: 12,
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
            <div style={{
              width: 44, height: 44, borderRadius: 12, flexShrink: 0,
              background: 'rgba(79,70,229,0.10)', color: '#4338ca',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}>
              <Icon.Layers size={22}/>
            </div>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em', flex: 1, minWidth: 0 }}>
              {DIRECTORY.plural}
            </h1>
            <div style={{ display: 'flex', gap: 8, flexShrink: 0 }}>
              <Button variant="secondary" size="md">
                <Icon.Upload size={15}/> Импорт
              </Button>
              <Button variant="secondary" size="md">
                <Icon.Download size={15}/> Экспорт
              </Button>
              <Button onClick={openCreate}>
                <Icon.Plus size={16}/> Добавить {DIRECTORY.singular}
              </Button>
            </div>
          </div>
          <div style={{
            fontSize: 13.5, color: '#64748b', maxWidth: 820, lineHeight: 1.5,
            paddingLeft: 60, /* align under title (44 icon + 16 gap) */
          }}>
            {DIRECTORY.description}
          </div>
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '20px 32px 48px' }}>
          <div style={{ maxWidth: 1180, margin: '0 auto' }}>

            {/* Tabs + search row */}
            <div style={{
              display: 'flex', alignItems: 'center', gap: 16,
              marginBottom: 16,
            }}>
              {t.groupArchived === 'tab' && (
                <div style={{ display: 'flex', gap: 4, background: '#fff', border: '1px solid #e2e8f0', borderRadius: 10, padding: 4 }}>
                  {[
                    ['active', 'Активные', counts.active],
                    ['archived', 'Архив', counts.archived],
                  ].map(([v, l, n]) => {
                    const on = tab === v;
                    return (
                      <button key={v} onClick={() => setTab(v)} style={{
                        display: 'inline-flex', alignItems: 'center', gap: 8,
                        padding: '6px 12px', borderRadius: 7, border: 0, fontSize: 13, fontWeight: 500,
                        background: on ? '#4f46e5' : 'transparent',
                        color: on ? '#fff' : '#475569',
                      }}>
                        {l}
                        <span style={{
                          background: on ? 'rgba(255,255,255,0.22)' : '#f1f5f9',
                          color: on ? '#fff' : '#64748b',
                          padding: '1px 7px', borderRadius: 9999, fontSize: 11.5, fontWeight: 600,
                          fontVariantNumeric: 'tabular-nums',
                        }}>{n}</span>
                      </button>
                    );
                  })}
                </div>
              )}

              <div style={{ position: 'relative', flex: 1, maxWidth: 360 }}>
                <Icon.Search size={15} stroke="#94a3b8"
                  style={{ position: 'absolute', left: 12, top: 11, pointerEvents: 'none' }}/>
                <input value={query} onChange={e => setQuery(e.target.value)}
                  placeholder={`Поиск по ${DIRECTORY.plural.toLowerCase()}`}
                  style={{
                    width: '100%', height: 38, paddingLeft: 34, paddingRight: 12,
                    borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
                    fontSize: 13, fontFamily: 'inherit', outline: 'none',
                  }}
                  onFocus={e => { e.target.style.borderColor='#6366f1'; e.target.style.boxShadow='0 0 0 3px rgba(99,102,241,0.18)'; }}
                  onBlur={e => { e.target.style.borderColor='#e2e8f0'; e.target.style.boxShadow='none'; }}/>
              </div>

              <div style={{ marginLeft: 'auto', fontSize: 13, color: '#64748b' }}>
                Показано: <strong style={{ color: '#0f172a' }}>{filtered.length}</strong>
                <span style={{ color: '#cbd5e1', margin: '0 8px' }}>·</span>
                Всего: <strong style={{ color: '#0f172a' }}>{items.length}</strong>
              </div>
            </div>

            {/* Table */}
            {filtered.length === 0 ? (
              <EmptyState query={query} onCreate={openCreate} singular={DIRECTORY.singular}/>
            ) : (
              <Table
                items={filtered} t={t}
                onEdit={openEdit}
                onArchive={handleArchive}
                onDelete={(item) => setItems(prev => prev.filter(i => i.id !== item.id))}
                onReorder={reorder}
                menuFor={menuFor} setMenuFor={setMenuFor}
              />
            )}

            {/* Footer help */}
            <div style={{
              marginTop: 20, fontSize: 12, color: '#94a3b8',
              display: 'flex', alignItems: 'center', gap: 8,
            }}>
              <Icon.Info size={13}/>
              Порядок записей определяет, как уровни будут отображаться в выпадающих списках и фильтрах.
              Удалить можно только записи, которые ни в чём не используются — иначе переведите в «Архив».
            </div>
          </div>
        </div>
      </div>

      <LevelDrawer
        open={drawer.open} mode={drawer.mode} initial={drawer.initial}
        onClose={closeDrawer} onSave={handleSave} onDelete={handleDelete}
      />

      <TweaksPanel title="Tweaks">
        <TweakSection label="Плотность">
          <TweakRadio label="Строки" value={t.density}
            onChange={v => setTweak('density', v)}
            options={[
              { value: 'compact',     label: 'Компактно' },
              { value: 'comfortable', label: 'Стандарт' },
            ]}/>
        </TweakSection>
        <TweakSection label="Колонки">
          <TweakToggle label="Цветовая метка"   value={t.showColorDot}    onChange={v => setTweak('showColorDot', v)}/>
          <TweakToggle label="Код"               value={t.showCode}        onChange={v => setTweak('showCode', v)}/>
          <TweakToggle label="Порядок (drag)"    value={t.showOrder}       onChange={v => setTweak('showOrder', v)}/>
          <TweakToggle label="Описание"          value={t.showDescription} onChange={v => setTweak('showDescription', v)}/>
        </TweakSection>
        <TweakSection label="Архив">
          <TweakRadio label="Где" value={t.groupArchived}
            onChange={v => setTweak('groupArchived', v)}
            options={[
              { value: 'tab',   label: 'Вкладка' },
              { value: 'mixed', label: 'В списке' },
            ]}/>
        </TweakSection>
      </TweaksPanel>
    </div>
  );
}

// ──────────────────────────────────────────────────────────────────────────
function Table({ items, t, onEdit, onArchive, onDelete, onReorder, menuFor, setMenuFor }) {
  const [dragId, setDragId] = useState(null);
  const [overId, setOverId] = useState(null);

  const rowPadY = t.density === 'compact' ? 10 : 14;
  const rowGap  = t.density === 'compact' ? 2 : 4;

  // Build columns dynamically based on tweaks
  const cols = [];
  if (t.showOrder)    cols.push({ w: 36 });               // drag
  if (t.showColorDot) cols.push({ w: 28 });               // color
  cols.push({ w: 'minmax(260px, 1fr)' });                  // name
  if (t.showCode)     cols.push({ w: 92 });                // code
  cols.push({ w: 200 });                                   // usage
  cols.push({ w: 110 });                                   // status
  cols.push({ w: 56 });                                    // actions
  const gridCols = cols.map(c => typeof c.w === 'number' ? `${c.w}px` : c.w).join(' ');

  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14, overflow: 'hidden',
      boxShadow: '0 1px 2px rgba(15,23,42,0.04)',
    }}>
      {/* Head */}
      <div style={{
        display: 'grid', gridTemplateColumns: gridCols, gap: 12, alignItems: 'center',
        padding: `12px 18px`, borderBottom: '1px solid #e2e8f0', background: '#f8fafc',
        fontSize: 11, fontWeight: 600, color: '#64748b',
        textTransform: 'uppercase', letterSpacing: '0.06em',
      }}>
        {t.showOrder && <span/>}
        {t.showColorDot && <span/>}
        <span>Название</span>
        {t.showCode && <span>Код</span>}
        <span>Использование</span>
        <span>Статус</span>
        <span/>
      </div>

      {/* Rows */}
      {items.map((it, idx) => {
        const isLast = idx === items.length - 1;
        const isDragOver = overId === it.id && dragId !== it.id;
        return (
          <div key={it.id}
            draggable={t.showOrder && it.status === 'active'}
            onDragStart={() => setDragId(it.id)}
            onDragOver={(e) => { e.preventDefault(); if (dragId) setOverId(it.id); }}
            onDragLeave={() => setOverId(null)}
            onDrop={() => { if (dragId && dragId !== it.id) onReorder(dragId, it.id); setDragId(null); setOverId(null); }}
            onDragEnd={() => { setDragId(null); setOverId(null); }}
            style={{
              display: 'grid', gridTemplateColumns: gridCols, gap: 12, alignItems: 'center',
              padding: `${rowPadY}px 18px`,
              borderBottom: isLast ? '0' : '1px solid #f1f5f9',
              background: isDragOver ? 'rgba(99,102,241,0.06)' : dragId === it.id ? 'rgba(99,102,241,0.04)' : '#fff',
              opacity: dragId === it.id ? 0.5 : 1,
              transition: 'background .12s',
              position: 'relative',
              cursor: 'default',
            }}
            onMouseEnter={e => { if (dragId !== it.id) e.currentTarget.style.background = '#fafbfc'; }}
            onMouseLeave={e => { if (dragId !== it.id && !isDragOver) e.currentTarget.style.background = '#fff'; }}
          >
            {t.showOrder && (
              <span style={{
                color: '#cbd5e1', display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                cursor: it.status === 'active' ? 'grab' : 'not-allowed', userSelect: 'none',
              }} title={it.status === 'active' ? 'Перетащите для изменения порядка' : 'Архивные записи нельзя сортировать'}>
                <Icon.GripVertical size={16}/>
              </span>
            )}

            {t.showColorDot && (
              <span style={{
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              }}>
                <span style={{
                  width: 14, height: 14, borderRadius: 999,
                  background: COLOR_DOTS[it.color] || '#94a3b8',
                  boxShadow: `0 0 0 3px ${COLOR_DOTS[it.color] || '#94a3b8'}1a`,
                }}/>
              </span>
            )}

            <div style={{ minWidth: 0, display: 'flex', flexDirection: 'column', gap: rowGap }}>
              <button onClick={() => onEdit(it)} style={{
                background: 'transparent', border: 0, padding: 0, textAlign: 'left',
                fontSize: 14, fontWeight: 600, color: '#0f172a',
                whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
                cursor: 'pointer',
              }}
                onMouseEnter={e => e.currentTarget.style.color = '#4f46e5'}
                onMouseLeave={e => e.currentTarget.style.color = '#0f172a'}>
                {it.name}
              </button>
              {t.showDescription && it.description && (
                <div style={{
                  fontSize: 12.5, color: '#64748b', lineHeight: 1.45,
                  overflow: 'hidden', textOverflow: 'ellipsis',
                  display: '-webkit-box', WebkitLineClamp: t.density === 'compact' ? 1 : 2, WebkitBoxOrient: 'vertical',
                }}>{it.description}</div>
              )}
            </div>

            {t.showCode && (
              <span style={{
                fontFamily: 'var(--edv-font-mono)', fontSize: 12, color: '#475569',
                background: '#f1f5f9', padding: '3px 8px', borderRadius: 6,
                justifySelf: 'start', letterSpacing: '0.04em',
              }}>{it.code || '—'}</span>
            )}

            <UsageCell usage={it.usage} dim={it.status === 'archived'}/>

            <StatusBadge status={it.status}/>

            <div style={{ position: 'relative', justifySelf: 'end' }}>
              <button
                onClick={(e) => { e.stopPropagation(); setMenuFor(menuFor === it.id ? null : it.id); }}
                aria-label="Действия" style={{
                  width: 30, height: 30, borderRadius: 8, border: '0', background: 'transparent',
                  display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#64748b',
                }}
                onMouseEnter={e => e.currentTarget.style.background = '#f1f5f9'}
                onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
                <Icon.MoreHorizontal size={18}/>
              </button>

              {menuFor === it.id && (
                <RowMenu
                  item={it}
                  onEdit={() => { onEdit(it); setMenuFor(null); }}
                  onArchive={() => onArchive(it)}
                  onDelete={() => { if (canDelete(it)) onDelete(it); setMenuFor(null); }}
                />
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
}

function canDelete(it) {
  return !it.usage || (it.usage.groups + it.usage.courses + it.usage.students) === 0;
}

function UsageCell({ usage, dim }) {
  if (!usage) return <span style={{ color: '#94a3b8' }}>—</span>;
  const total = usage.groups + usage.courses + usage.students;
  if (total === 0) return <span style={{ fontSize: 13, color: '#94a3b8' }}>не используется</span>;
  const fg = dim ? '#94a3b8' : '#475569';
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 14, fontSize: 13, color: fg, fontVariantNumeric: 'tabular-nums' }}>
      <UsageNum n={usage.groups} l="групп" dim={dim}/>
      <UsageNum n={usage.courses} l="курсов" dim={dim}/>
      <UsageNum n={usage.students} l="студ." dim={dim}/>
    </div>
  );
}
function UsageNum({ n, l, dim }) {
  return (
    <span style={{ display: 'inline-flex', alignItems: 'baseline', gap: 4 }}>
      <strong style={{ color: dim ? '#94a3b8' : '#0f172a', fontWeight: 600 }}>{n}</strong>
      <span style={{ color: '#94a3b8', fontSize: 12 }}>{l}</span>
    </span>
  );
}

function StatusBadge({ status }) {
  if (status === 'active') {
    return (
      <span style={{
        display: 'inline-flex', alignItems: 'center', gap: 6,
        padding: '3px 9px', borderRadius: 9999, fontSize: 12, fontWeight: 500,
        background: '#d1fae5', color: '#047857', justifySelf: 'start',
      }}>
        <span style={{ width: 6, height: 6, borderRadius: 999, background: '#10b981' }}/>
        Активный
      </span>
    );
  }
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 9px', borderRadius: 9999, fontSize: 12, fontWeight: 500,
      background: '#f1f5f9', color: '#64748b', justifySelf: 'start',
    }}>
      <span style={{ width: 6, height: 6, borderRadius: 999, background: '#94a3b8' }}/>
      В архиве
    </span>
  );
}

function RowMenu({ item, onEdit, onArchive, onDelete }) {
  const archived = item.status === 'archived';
  const deletable = canDelete(item);
  return (
    <div onClick={e => e.stopPropagation()} style={{
      position: 'absolute', right: 0, top: 36, zIndex: 20, minWidth: 200,
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 10,
      boxShadow: '0 8px 24px -4px rgba(15,23,42,0.18), 0 2px 6px rgba(15,23,42,0.06)',
      padding: 4, animation: 'fadeIn .1s ease',
    }}>
      <MenuItem icon="Pencil" onClick={onEdit}>Редактировать</MenuItem>
      <MenuItem icon="Archive" onClick={onArchive}>
        {archived ? 'Восстановить' : 'В архив'}
      </MenuItem>
      <div style={{ height: 1, background: '#f1f5f9', margin: '4px 0' }}/>
      <MenuItem icon="Trash" tone="danger" disabled={!deletable}
        onClick={deletable ? onDelete : undefined}
        hint={!deletable ? 'Используется в группах/курсах' : null}>
        Удалить
      </MenuItem>
    </div>
  );
}
function MenuItem({ icon, children, onClick, tone, disabled, hint }) {
  const Ic = Icon[icon];
  const fg = disabled ? '#cbd5e1' : tone === 'danger' ? '#b91c1c' : '#334155';
  return (
    <button onClick={disabled ? undefined : onClick}
      style={{
        display: 'flex', alignItems: 'center', gap: 10, width: '100%',
        padding: '8px 10px', borderRadius: 7, border: 0, background: 'transparent',
        color: fg, fontSize: 13, fontWeight: 500, textAlign: 'left',
        cursor: disabled ? 'not-allowed' : 'pointer',
      }}
      onMouseEnter={e => { if (!disabled) e.currentTarget.style.background = tone === 'danger' ? '#fef2f2' : '#f1f5f9'; }}
      onMouseLeave={e => { if (!disabled) e.currentTarget.style.background = 'transparent'; }}
      title={hint || ''}>
      <Ic size={15}/>
      <span style={{ flex: 1 }}>{children}</span>
    </button>
  );
}

function EmptyState({ query, onCreate, singular }) {
  return (
    <div style={{
      background: '#fff', border: '1px dashed #cbd5e1', borderRadius: 14,
      padding: '56px 24px', textAlign: 'center',
    }}>
      <div style={{
        width: 56, height: 56, borderRadius: 14, margin: '0 auto 16px',
        background: 'rgba(99,102,241,0.08)', color: '#4f46e5',
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Inbox size={28}/>
      </div>
      <div style={{ fontSize: 16, fontWeight: 600, color: '#0f172a', marginBottom: 6 }}>
        {query ? 'Ничего не найдено' : 'Список пуст'}
      </div>
      <div style={{ fontSize: 13.5, color: '#64748b', marginBottom: 18 }}>
        {query
          ? `По запросу «${query}» нет совпадений. Попробуйте изменить запрос.`
          : `Здесь будут отображаться все ${singular === 'уровень' ? 'уровни' : 'записи'}. Добавьте первую запись, чтобы начать.`}
      </div>
      {!query && (
        <Button onClick={onCreate}><Icon.Plus size={16}/> Добавить {singular}</Button>
      )}
    </div>
  );
}

window.LevelsApp = LevelsApp;
