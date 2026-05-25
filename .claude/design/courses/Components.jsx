// Reusable components for Courses page

// ── KPI block ────────────────────────────────────────────────────────
const courseKpiTones = {
  slate:   { bg: '#f1f5f9', fg: '#475569' },
  primary: { bg: '#e0eaff', fg: '#4338ca' },
  success: { bg: '#d1fae5', fg: '#047857' },
  warning: { bg: '#fef3c7', fg: '#92400e' },
};
function CourseKpi({ label, value, icon, tone = 'slate', delta }) {
  const t = courseKpiTones[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '16px 18px', display: 'flex', flexDirection: 'column', gap: 12,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
        <div style={{
          width: 32, height: 32, borderRadius: 8, background: t.bg, color: t.fg,
          display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0,
        }}><Ic size={16} /></div>
        <div style={{ fontSize: 12.5, color: '#64748b', fontWeight: 500 }}>{label}</div>
      </div>
      <div>
        <div style={{ fontSize: 26, fontWeight: 700, lineHeight: 1, letterSpacing: '-0.02em',
          color: '#0f172a', fontVariantNumeric: 'tabular-nums',
        }}>{value}</div>
        {delta && <div style={{ marginTop: 6, fontSize: 12, color: '#64748b' }}>{delta}</div>}
      </div>
    </div>
  );
}

// ── Filter dropdown (same shape as Groups) ───────────────────────────
function CourseFilterDropdown({ label, icon, value, onChange, options }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);

  const Ic = Icon[icon];
  const count = value.size;
  const toggle = (v) => {
    const n = new Set(value);
    n.has(v) ? n.delete(v) : n.add(v);
    onChange(n);
  };

  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button onClick={() => setOpen(o => !o)}
        style={{
          display: 'inline-flex', alignItems: 'center', gap: 6,
          height: 36, padding: '0 12px', borderRadius: 10,
          border: `1px solid ${count ? '#c7d6fe' : '#e2e8f0'}`,
          background: count ? '#f0f4ff' : '#fff',
          color: count ? '#4338ca' : '#334155',
          fontSize: 13, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
        }}>
        <Ic size={14} />{label}
        {count > 0 && (
          <span style={{
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            minWidth: 18, height: 18, padding: '0 5px', borderRadius: 9999,
            background: '#4f46e5', color: '#fff', fontSize: 11, fontWeight: 600,
          }}>{count}</span>
        )}
        <Icon.ChevronDown size={14} stroke={count ? '#4338ca' : '#94a3b8'} />
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 4px)', left: 0, zIndex: 30,
          minWidth: 220, background: '#fff', border: '1px solid #e2e8f0',
          borderRadius: 12, boxShadow: '0 10px 30px rgba(15,23,42,0.10)',
          padding: 6, maxHeight: 320, overflowY: 'auto',
        }}>
          {options.map(o => {
            const checked = value.has(o.value);
            return (
              <button key={o.value} onClick={() => toggle(o.value)}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: checked ? '#f0f4ff' : 'transparent', cursor: 'pointer',
                  textAlign: 'left', fontFamily: 'inherit',
                }}>
                <span style={{
                  width: 16, height: 16, borderRadius: 4, flexShrink: 0,
                  border: `1.5px solid ${checked ? '#4f46e5' : '#cbd5e1'}`,
                  background: checked ? '#4f46e5' : '#fff',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                }}>{checked && <Icon.Check size={11} stroke="#fff" sw={3} />}</span>
                <span style={{
                  width: 8, height: 8, borderRadius: 9999, background: o.swatch, flexShrink: 0,
                }}/>
                <span style={{ fontSize: 13, color: '#0f172a', flex: 1 }}>{o.label}</span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

// ── View toggle ──────────────────────────────────────────────────────
function CourseViewToggle({ view, onChange }) {
  const items = [
    { id: 'table', icon: 'FileText',        label: 'Таблица' },
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
              color: active ? '#0f172a' : '#64748b', cursor: 'pointer',
            }}>
            <Ic size={14} />
          </button>
        );
      })}
    </div>
  );
}

// ── Status pill ──────────────────────────────────────────────────────
function CourseStatusPill({ status }) {
  const s = COURSE_STATUSES[status];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 10px', borderRadius: 9999, background: s.bg, color: s.fg,
      fontSize: 12, fontWeight: 500, lineHeight: 1.4,
    }}>
      <span style={{ width: 6, height: 6, borderRadius: 9999, background: s.dot }} />
      {s.label}
    </span>
  );
}

// ── Level chip ───────────────────────────────────────────────────────
function CourseLevelChip({ level }) {
  const def = COURSE_LEVELS.find(l => l.value === level);
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      minWidth: 36, padding: '2px 8px', borderRadius: 6,
      background: '#f1f5f9', color: '#334155',
      fontSize: 12, fontWeight: 600, fontFamily: 'var(--edv-font-mono)',
      letterSpacing: '-0.01em',
    }}>{def ? def.label : level}</span>
  );
}

// ── Course cover (gradient + initials) ──────────────────────────────
function CourseCover({ subject, cover, size = 40, radius = 10 }) {
  const subj = COURSE_SUBJECTS[subject];
  const tone = SUBJECT_TONES[subj.tone];
  const fs = Math.round(size * 0.42);
  return (
    <div style={{
      width: size, height: size, borderRadius: radius, flexShrink: 0,
      background: tone.cover, color: '#fff',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontSize: fs, fontWeight: 700, fontFamily: 'var(--edv-font-mono)',
      letterSpacing: '-0.02em',
    }}>{cover}</div>
  );
}

// ── Subject chip ─────────────────────────────────────────────────────
function SubjectChip({ subject }) {
  const s = COURSE_SUBJECTS[subject];
  const t = SUBJECT_TONES[s.tone];
  const Ic = Icon[s.icon];
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 9px', borderRadius: 9999, background: t.bg, color: t.fg,
      fontSize: 12, fontWeight: 500, lineHeight: 1.4,
    }}>
      <Ic size={12} stroke="currentColor" />{s.label}
    </span>
  );
}

// ── Sort header ──────────────────────────────────────────────────────
function CourseSortHeader({ field, sort, onSort, width, align = 'left', children }) {
  const active = sort.field === field;
  return (
    <th style={{
      padding: '10px 16px', width, textAlign: align,
      background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
      fontSize: 12, fontWeight: 600, color: '#64748b', letterSpacing: 0.2,
      textTransform: 'uppercase', cursor: 'pointer', userSelect: 'none',
    }} onClick={() => onSort(field)}>
      <span style={{
        display: 'inline-flex', alignItems: 'center', gap: 4,
        color: active ? '#0f172a' : '#64748b',
      }}>
        {children}
        <span style={{
          display: 'inline-flex', flexDirection: 'column', lineHeight: 0.6,
          color: active ? '#4f46e5' : '#cbd5e1',
        }}>
          <span style={{ fontSize: 8, opacity: active && sort.dir === 'asc' ? 1 : 0.5 }}>▲</span>
          <span style={{ fontSize: 8, opacity: active && sort.dir === 'desc' ? 1 : 0.5 }}>▼</span>
        </span>
      </span>
    </th>
  );
}

// ── Checkbox ─────────────────────────────────────────────────────────
function CourseCheckbox({ checked, indeterminate, onChange }) {
  return (
    <span onClick={onChange} style={{
      display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      width: 16, height: 16, borderRadius: 4,
      border: `1.5px solid ${checked || indeterminate ? '#4f46e5' : '#cbd5e1'}`,
      background: checked || indeterminate ? '#4f46e5' : '#fff',
      cursor: 'pointer', flexShrink: 0,
    }}>
      {checked && <Icon.Check size={11} stroke="#fff" sw={3} />}
      {indeterminate && !checked && <span style={{ width: 8, height: 2, background: '#fff' }} />}
    </span>
  );
}

// ── Row menu ─────────────────────────────────────────────────────────
function CourseRowMenu({ status }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);
  const archived = status === 'Archived';
  return (
    <div ref={ref} style={{ position: 'relative', display: 'inline-block' }}>
      <button onClick={(e) => { e.preventDefault(); setOpen(o => !o); }}
        style={{
          width: 28, height: 28, borderRadius: 6, border: 'none',
          background: open ? '#f1f5f9' : 'transparent', color: '#64748b',
          cursor: 'pointer', display: 'inline-flex',
          alignItems: 'center', justifyContent: 'center',
        }}
        onMouseEnter={e => { if (!open) e.currentTarget.style.background = '#f1f5f9'; }}
        onMouseLeave={e => { if (!open) e.currentTarget.style.background = 'transparent'; }}>
        <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
          <circle cx="5" cy="12" r="2"/><circle cx="12" cy="12" r="2"/><circle cx="19" cy="12" r="2"/>
        </svg>
      </button>
      {open && (
        <div style={{
          position: 'absolute', right: 0, top: 'calc(100% + 4px)', zIndex: 20,
          minWidth: 220, background: '#fff', border: '1px solid #e2e8f0',
          borderRadius: 10, boxShadow: '0 10px 30px rgba(15,23,42,0.12)', padding: 4,
        }}>
          <CourseMenuItem icon="BookOpen"     label="Открыть курс" />
          <CourseMenuItem icon="FileText"     label="Программа занятий" />
          <CourseMenuItem icon="UserPlus"     label="Создать группу" />
          <CourseMenuItem icon="Sparkles"     label="Дублировать" />
          <div style={{ height: 1, background: '#f1f5f9', margin: '4px 0' }} />
          {archived
            ? <CourseMenuItem icon="ArrowLeft" label="Восстановить" />
            : <CourseMenuItem icon="X"         label="Архивировать" danger />}
        </div>
      )}
    </div>
  );
}
function CourseMenuItem({ icon, label, danger }) {
  const Ic = Icon[icon];
  return (
    <button style={{
      display: 'flex', alignItems: 'center', gap: 10, width: '100%',
      padding: '8px 10px', borderRadius: 6, border: 'none', background: 'transparent',
      fontSize: 13, color: danger ? '#b91c1c' : '#0f172a', cursor: 'pointer',
      fontFamily: 'inherit', textAlign: 'left',
    }}
      onMouseEnter={e => e.currentTarget.style.background = danger ? '#fef2f2' : '#f8fafc'}
      onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
      <Ic size={14} stroke={danger ? '#b91c1c' : '#64748b'} />{label}
    </button>
  );
}

// ── Empty state ──────────────────────────────────────────────────────
function CoursesEmpty({ onReset }) {
  return (
    <div style={{
      padding: '60px 20px',
      display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 10,
      color: '#64748b', textAlign: 'center',
    }}>
      <div style={{
        width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Search size={24} stroke="#94a3b8" />
      </div>
      <div style={{ fontSize: 15, fontWeight: 600, color: '#0f172a' }}>Курсы не найдены</div>
      <div style={{ fontSize: 13, color: '#64748b', maxWidth: 360 }}>
        Попробуйте изменить запрос или сбросить фильтры.
      </div>
      <button onClick={onReset} style={{
        marginTop: 4, height: 32, padding: '0 14px', borderRadius: 8,
        border: '1px solid #e2e8f0', background: '#fff', color: '#334155',
        fontSize: 13, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
      }}>Сбросить фильтры</button>
    </div>
  );
}

// ── Course row ───────────────────────────────────────────────────────
function CourseTableRow({ c, selected, onToggle }) {
  const [hover, setHover] = React.useState(false);
  const bg = selected ? 'rgba(79,70,229,0.04)' : hover ? '#fafbfc' : 'transparent';
  const onRowClick = (e) => {
    if (e.target.closest('button,input,a,[data-stop]')) return;
    window.location.href = 'Course.html';
  };
  return (
    <tr style={{ borderBottom: '1px solid #f1f5f9', background: bg, transition: 'background .1s', cursor: 'pointer' }}
      onClick={onRowClick}
      onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}>
      <td style={{ padding: '14px 12px 14px 20px' }}>
        <CourseCheckbox checked={selected} onChange={onToggle} />
      </td>
      <td style={{ padding: '14px 16px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          <CourseCover subject={c.subject} cover={c.cover} />
          <div style={{ minWidth: 0 }}>
            <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>{c.name}</div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 3 }}>
              <span style={{
                fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b',
              }}>{c.code}</span>
              <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
              <span style={{ fontSize: 12, color: '#64748b' }}>обновлён {c.updated}</span>
            </div>
          </div>
        </div>
      </td>
      <td style={{ padding: '14px 16px' }}><CourseLevelChip level={c.level} /></td>
      <td style={{ padding: '14px 16px' }}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <span style={{ fontSize: 13, color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>
            {c.lessons} занятий
          </span>
          <span style={{ fontSize: 12, color: '#64748b' }}>{c.durationWeeks} недель</span>
        </div>
      </td>
      <td style={{ padding: '14px 16px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 6,
          fontSize: 13, color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>
          <Icon.Users size={13} stroke="#94a3b8" />
          <span>{c.groups}</span>
          <span style={{ color: '#94a3b8', fontWeight: 400 }}>·</span>
          <span style={{ color: '#64748b' }}>{c.students} студ.</span>
        </div>
      </td>
      <td style={{ padding: '14px 16px' }}><CourseStatusPill status={c.status} /></td>
      <td style={{ padding: '14px 12px 14px 8px', textAlign: 'right' }}>
        <CourseRowMenu status={c.status} />
      </td>
    </tr>
  );
}

// ── Subject group section header ─────────────────────────────────────
function SubjectGroupHeader({ subject, count, expanded, onToggle }) {
  const s = COURSE_SUBJECTS[subject];
  const t = SUBJECT_TONES[s.tone];
  const Ic = Icon[s.icon];
  return (
    <tr>
      <td colSpan={7} style={{
        padding: 0, background: '#fbfcfd', borderBottom: '1px solid #e2e8f0',
        borderTop: '1px solid #e2e8f0',
      }}>
        <button onClick={onToggle} style={{
          display: 'flex', alignItems: 'center', gap: 10, width: '100%',
          padding: '10px 20px', border: 'none', background: 'transparent',
          fontFamily: 'inherit', cursor: 'pointer', textAlign: 'left',
        }}>
          <Icon.ChevronDown size={14} stroke="#64748b"
            style={{ transform: expanded ? 'none' : 'rotate(-90deg)', transition: 'transform .15s' }} />
          <div style={{
            width: 22, height: 22, borderRadius: 6, background: t.bg, color: t.fg,
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
          }}><Ic size={13} /></div>
          <span style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{s.label}</span>
          <span style={{ fontSize: 12, color: '#64748b', fontWeight: 400 }}>{count}</span>
        </button>
      </td>
    </tr>
  );
}

// ── Table view ───────────────────────────────────────────────────────
function CoursesTable({ rows, sort, onSort, selected, toggleAll, toggleOne,
  allSelected, someSelected, grouped, expanded, onToggleGroup }) {

  const renderHeader = () => (
    <thead>
      <tr>
        <th style={{
          padding: '10px 12px 10px 20px', width: 40,
          background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
        }}>
          <CourseCheckbox checked={allSelected}
            indeterminate={!allSelected && someSelected} onChange={toggleAll} />
        </th>
        <CourseSortHeader field="name"     sort={sort} onSort={onSort}>Курс</CourseSortHeader>
        <CourseSortHeader field="level"    sort={sort} onSort={onSort} width={110}>Уровень</CourseSortHeader>
        <CourseSortHeader field="duration" sort={sort} onSort={onSort} width={140}>Длительность</CourseSortHeader>
        <CourseSortHeader field="groups"   sort={sort} onSort={onSort} width={150}>Использование</CourseSortHeader>
        <CourseSortHeader field="status"   sort={sort} onSort={onSort} width={140}>Статус</CourseSortHeader>
        <th style={{ width: 56, background: '#f8fafc', borderBottom: '1px solid #e2e8f0' }} />
      </tr>
    </thead>
  );

  if (!grouped) {
    return (
      <div style={{ overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13.5 }}>
          {renderHeader()}
          <tbody>
            {rows.map(c => (
              <CourseTableRow key={c.id} c={c}
                selected={selected.has(c.id)} onToggle={() => toggleOne(c.id)} />
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  // Grouped by subject
  const buckets = {};
  rows.forEach(c => { (buckets[c.subject] ||= []).push(c); });
  const order = Object.keys(COURSE_SUBJECTS).filter(k => buckets[k]);

  return (
    <div style={{ overflowX: 'auto' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13.5 }}>
        {renderHeader()}
        <tbody>
          {order.map(key => {
            const list = buckets[key];
            const isOpen = expanded.has(key);
            return (
              <React.Fragment key={key}>
                <SubjectGroupHeader subject={key} count={list.length}
                  expanded={isOpen} onToggle={() => onToggleGroup(key)} />
                {isOpen && list.map(c => (
                  <CourseTableRow key={c.id} c={c}
                    selected={selected.has(c.id)} onToggle={() => toggleOne(c.id)} />
                ))}
              </React.Fragment>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

// ── Card view ────────────────────────────────────────────────────────
function CoursesCards({ rows, grouped, expanded, onToggleGroup }) {
  if (!grouped) {
    return (
      <div style={{
        padding: 16, display: 'grid', gap: 14,
        gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
      }}>
        {rows.map(c => <CourseCard key={c.id} c={c} />)}
      </div>
    );
  }
  const buckets = {};
  rows.forEach(c => { (buckets[c.subject] ||= []).push(c); });
  const order = Object.keys(COURSE_SUBJECTS).filter(k => buckets[k]);
  return (
    <div style={{ padding: '8px 0 16px' }}>
      {order.map(key => {
        const s = COURSE_SUBJECTS[key];
        const t = SUBJECT_TONES[s.tone];
        const Ic = Icon[s.icon];
        const list = buckets[key];
        const isOpen = expanded.has(key);
        return (
          <div key={key} style={{ borderTop: '1px solid #e2e8f0' }}>
            <button onClick={() => onToggleGroup(key)} style={{
              display: 'flex', alignItems: 'center', gap: 10, width: '100%',
              padding: '12px 20px', background: '#fbfcfd', border: 'none',
              fontFamily: 'inherit', cursor: 'pointer', textAlign: 'left',
            }}>
              <Icon.ChevronDown size={14} stroke="#64748b"
                style={{ transform: isOpen ? 'none' : 'rotate(-90deg)', transition: 'transform .15s' }} />
              <div style={{
                width: 22, height: 22, borderRadius: 6, background: t.bg, color: t.fg,
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
              }}><Ic size={13} /></div>
              <span style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{s.label}</span>
              <span style={{ fontSize: 12, color: '#64748b', fontWeight: 400 }}>{list.length}</span>
            </button>
            {isOpen && (
              <div style={{
                padding: 16, display: 'grid', gap: 14,
                gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
              }}>
                {list.map(c => <CourseCard key={c.id} c={c} />)}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}

function CourseCard({ c }) {
  const [hover, setHover] = React.useState(false);
  const subj = COURSE_SUBJECTS[c.subject];
  const tone = SUBJECT_TONES[subj.tone];
  return (
    <div onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      onClick={() => { window.location.href = 'Course.html'; }}
      style={{
        background: '#fff', border: `1px solid ${hover ? '#c7d6fe' : '#e2e8f0'}`,
        borderRadius: 14, overflow: 'hidden', transition: 'all .15s',
        boxShadow: hover ? '0 6px 16px rgba(15,23,42,0.06)' : 'none',
        display: 'flex', flexDirection: 'column', cursor: 'pointer',
      }}>
      {/* Cover */}
      <div style={{
        height: 96, background: tone.cover, position: 'relative',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <span style={{
          fontSize: 36, fontWeight: 700, color: '#fff',
          fontFamily: 'var(--edv-font-mono)', letterSpacing: '-0.04em',
          textShadow: '0 1px 2px rgba(0,0,0,0.10)',
        }}>{c.cover}</span>
        <div style={{ position: 'absolute', top: 10, right: 10 }}>
          <CourseStatusPill status={c.status} />
        </div>
        <div style={{ position: 'absolute', bottom: 10, left: 12 }}>
          <CourseLevelChip level={c.level} />
        </div>
      </div>
      <div style={{ padding: 14, display: 'flex', flexDirection: 'column', gap: 10 }}>
        <div>
          <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a', lineHeight: 1.3 }}>
            {c.name}
          </div>
          <div style={{
            fontFamily: 'var(--edv-font-mono)', fontSize: 11.5, color: '#64748b', marginTop: 4,
          }}>{c.code}</div>
        </div>
        <div style={{
          display: 'flex', alignItems: 'center', gap: 12,
          fontSize: 12.5, color: '#475569',
        }}>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5 }}>
            <Icon.FileText size={13} stroke="#94a3b8" />{c.lessons} занятий
          </span>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5 }}>
            <Icon.CalendarDays size={13} stroke="#94a3b8" />{c.durationWeeks} нед.
          </span>
        </div>
        <div style={{
          marginTop: 2, paddingTop: 10, borderTop: '1px solid #f1f5f9',
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
          fontSize: 12.5, color: '#475569',
        }}>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5 }}>
            <Icon.Users size={13} stroke="#94a3b8" />
            <strong style={{ fontWeight: 600, color: '#0f172a' }}>{c.groups}</strong>&nbsp;групп
          </span>
          <span style={{ color: '#94a3b8', fontSize: 11.5 }}>обновлён {c.updated}</span>
        </div>
      </div>
    </div>
  );
}

window.CourseKpi = CourseKpi;
window.CourseFilterDropdown = CourseFilterDropdown;
window.CourseViewToggle = CourseViewToggle;
window.CoursesTable = CoursesTable;
window.CoursesCards = CoursesCards;
window.CoursesEmpty = CoursesEmpty;
