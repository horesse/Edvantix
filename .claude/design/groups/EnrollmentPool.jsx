// Step 3 — Enrollment supporting components.

// ═══════════════════════════════════════════════════════════════════
// Capacity overview — big strip above the pool selector
// ═══════════════════════════════════════════════════════════════════
function CapacityOverview({ capacity, enrolled, waitlist, invites, openRecruitment, autoPick }) {
  const pct = Math.min(100, Math.round(enrolled / capacity * 100));
  const seatsLeft = Math.max(0, capacity - enrolled);
  const full = enrolled >= capacity;
  const fillColor = full ? '#10b981' : pct >= 80 ? '#f59e0b' : '#6366f1';

  return (
    <section style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
      padding: '20px 24px', display: 'flex', flexDirection: 'column', gap: 16,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
        {/* Big seats count */}
        <div style={{ display: 'flex', alignItems: 'baseline', gap: 6, flexShrink: 0 }}>
          <span style={{
            fontSize: 38, fontWeight: 700, color: '#0f172a',
            fontVariantNumeric: 'tabular-nums', letterSpacing: '-0.03em', lineHeight: 1,
          }}>{enrolled}</span>
          <span style={{ fontSize: 22, color: '#94a3b8', fontWeight: 500,
            fontVariantNumeric: 'tabular-nums' }}>/ {capacity}</span>
          <span style={{ fontSize: 13, color: '#64748b', marginLeft: 4 }}>
            {declensionEN(enrolled, ['студент','студента','студентов'])}
          </span>
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ display: 'flex', gap: 8, alignItems: 'center', marginBottom: 6 }}>
            {full ? (
              <Badge variant="success" dot>Группа укомплектована</Badge>
            ) : (
              <Badge variant="primary" dot>
                Свободно {seatsLeft} {declensionEN(seatsLeft, ['место','места','мест'])}
              </Badge>
            )}
            {waitlist > 0 && (
              <Badge variant="warning">
                В листе ожидания · {waitlist}
              </Badge>
            )}
            {invites > 0 && (
              <Badge variant="primary">
                Приглашений · {invites}
              </Badge>
            )}
            {openRecruitment && (
              <Badge variant="outline">
                <Icon.Megaphone size={11} stroke="currentColor" />
                Открытый набор
              </Badge>
            )}
          </div>
          <div style={{
            position: 'relative', height: 8, background: '#f1f5f9',
            borderRadius: 9999, overflow: 'hidden',
          }}>
            <div style={{
              width: `${pct}%`, height: '100%', background: fillColor,
              borderRadius: 9999, transition: 'width .3s',
            }} />
            {/* segment ticks */}
            <div style={{
              position: 'absolute', inset: 0, display: 'flex', pointerEvents: 'none',
            }}>
              {Array.from({ length: capacity - 1 }).map((_, i) => (
                <div key={i} style={{
                  flex: 1, borderRight: '1px solid #fff',
                }} />
              ))}
              <div style={{ flex: 1 }} />
            </div>
          </div>
        </div>
        <Button variant="secondary" size="md" onClick={autoPick}
          disabled={seatsLeft === 0}>
          <Icon.Sparkles size={14} />Подобрать автоматически
        </Button>
      </div>

      <div style={{
        display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 10,
        paddingTop: 14, borderTop: '1px solid #f1f5f9',
      }}>
        <SmallStat icon="Users" label="Зачислено" value={enrolled}
          unit={declensionEN(enrolled, ['студент','студента','студентов'])} tone="primary" />
        <SmallStat icon="Clock" label="Лист ожидания" value={waitlist}
          unit={declensionEN(waitlist, ['студент','студента','студентов'])} tone="warning" />
        <SmallStat icon="Mail" label="Приглашений отправим" value={invites}
          unit={declensionEN(invites, ['письмо','письма','писем'])} tone="slate" />
      </div>
    </section>
  );
}
function SmallStat({ icon, label, value, unit, tone }) {
  const tones = {
    primary: { bg: 'rgba(79,70,229,0.08)',  fg: '#4338ca' },
    warning: { bg: 'rgba(245,158,11,0.10)', fg: '#92400e' },
    slate:   { bg: '#f1f5f9',                fg: '#475569' },
  };
  const t = tones[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 10,
      padding: '10px 12px', borderRadius: 10, background: '#fafbfc',
      border: '1px solid #f1f5f9',
    }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: t.bg, color: t.fg,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Ic size={16} />
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'baseline', gap: 5 }}>
          <span style={{ fontSize: 18, fontWeight: 700, color: '#0f172a',
            fontVariantNumeric: 'tabular-nums', letterSpacing: '-0.02em', lineHeight: 1 }}>
            {value}
          </span>
          <span style={{ fontSize: 11.5, color: '#64748b' }}>{unit}</span>
        </div>
        <div style={{ fontSize: 11.5, color: '#64748b', marginTop: 2 }}>{label}</div>
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Student pool — search + filter tabs + list
// ═══════════════════════════════════════════════════════════════════
function StudentPool({ groupLevel, isInRoster, inEnrolled, inWaitlist, onToggle, seatsLeft }) {
  const [query, setQuery]   = React.useState('');
  const [tabId, setTabId]   = React.useState('match');
  const [levelFilter, setLevelFilter] = React.useState(new Set());

  const counts = React.useMemo(() => ({
    match:    STUDENT_POOL.filter(s => s.level === groupLevel).length,
    waitlist: STUDENT_POOL.filter(s => s.status === 'waitlist').length,
    free:     STUDENT_POOL.filter(s => s.status === 'free').length,
    new:      STUDENT_POOL.filter(s => s.status === 'new' || s.status === 'invited').length,
    all:      STUDENT_POOL.length,
  }), [groupLevel]);

  const tabs = [
    { id: 'match',    label: `Подходят (${groupLevel})`, count: counts.match,    icon: 'Sparkles' },
    { id: 'waitlist', label: 'Лист ожидания',             count: counts.waitlist, icon: 'Clock' },
    { id: 'free',     label: 'Без группы',                count: counts.free,     icon: 'UserCheck' },
    { id: 'new',      label: 'Новые заявки',              count: counts.new,      icon: 'UserPlus' },
    { id: 'all',      label: 'Вся база',                  count: counts.all,      icon: 'Users' },
  ];

  const filtered = React.useMemo(() => {
    let list = [...STUDENT_POOL];
    if (tabId === 'match')    list = list.filter(s => s.level === groupLevel);
    if (tabId === 'waitlist') list = list.filter(s => s.status === 'waitlist');
    if (tabId === 'free')     list = list.filter(s => s.status === 'free');
    if (tabId === 'new')      list = list.filter(s => s.status === 'new' || s.status === 'invited');

    if (levelFilter.size > 0) {
      list = list.filter(s => levelFilter.has(s.level));
    }
    if (query.trim()) {
      const q = query.trim().toLowerCase();
      list = list.filter(s =>
        s.name.toLowerCase().includes(q) ||
        s.email.toLowerCase().includes(q) ||
        s.phone.includes(q)
      );
    }
    // Sort: not-in-roster first, then waitlist, then by suitability
    list.sort((a, b) => {
      const ra = isInRoster(a.id) ? 1 : 0;
      const rb = isInRoster(b.id) ? 1 : 0;
      if (ra !== rb) return ra - rb;
      const score = (s) => (
        (s.level === groupLevel ? 0 : 1) +
        (s.status === 'waitlist' ? -0.5 : 0) +
        (s.tags.includes('paid')   ? -0.2 : 0) +
        (s.tags.includes('tested') ? -0.1 : 0)
      );
      return score(a) - score(b);
    });
    return list;
  }, [tabId, query, levelFilter, groupLevel, isInRoster]);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      {/* Tabs */}
      <div style={{
        display: 'flex', gap: 4, borderBottom: '1px solid #e2e8f0',
        margin: '-6px -4px 0', padding: '0 4px', overflowX: 'auto',
      }}>
        {tabs.map(t => {
          const active = tabId === t.id;
          const Ic = Icon[t.icon];
          return (
            <button key={t.id} onClick={() => setTabId(t.id)}
              style={{
                display: 'inline-flex', alignItems: 'center', gap: 6,
                padding: '10px 12px', border: 'none', background: 'transparent',
                fontFamily: 'inherit', fontSize: 13,
                color: active ? '#4338ca' : '#64748b',
                fontWeight: active ? 600 : 500,
                borderBottom: active ? '2px solid #4f46e5' : '2px solid transparent',
                marginBottom: -1, cursor: 'pointer', whiteSpace: 'nowrap',
                transition: '.15s',
              }}>
              <Ic size={13} stroke={active ? '#4f46e5' : '#94a3b8'} />
              {t.label}
              <span style={{
                fontSize: 11, padding: '1px 7px', borderRadius: 9999,
                background: active ? '#f0f4ff' : '#f1f5f9',
                color: active ? '#4338ca' : '#94a3b8', fontWeight: 600,
                fontVariantNumeric: 'tabular-nums',
              }}>{t.count}</span>
            </button>
          );
        })}
      </div>

      {/* Search + level filter */}
      <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
        <div style={{ position: 'relative', flex: 1 }}>
          <Icon.Search size={14} stroke="#94a3b8"
            style={{ position: 'absolute', left: 12, top: 13 }} />
          <input
            value={query}
            onChange={e => setQuery(e.target.value)}
            placeholder="Поиск по имени, email или телефону…"
            style={{
              width: '100%', height: 40, paddingLeft: 36, paddingRight: 12,
              borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
              fontSize: 13.5, fontFamily: 'inherit', outline: 'none',
            }}
            onFocus={e => { e.target.style.borderColor = '#6366f1';
              e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.20)'; }}
            onBlur={e => { e.target.style.borderColor = '#e2e8f0';
              e.target.style.boxShadow = 'none'; }}
          />
        </div>
        <LevelFilter value={levelFilter} onChange={setLevelFilter} />
      </div>

      {/* List */}
      <div style={{
        border: '1px solid #e2e8f0', borderRadius: 12,
        background: '#fff', overflow: 'hidden',
        maxHeight: 460, display: 'flex', flexDirection: 'column',
      }}>
        <div style={{
          padding: '8px 14px', background: '#f8fafc', borderBottom: '1px solid #e2e8f0',
          display: 'flex', alignItems: 'center', gap: 8,
          fontSize: 11.5, fontWeight: 600, color: '#64748b',
          letterSpacing: '0.04em', textTransform: 'uppercase',
        }}>
          <span>Найдено · {filtered.length}</span>
          <span style={{ flex: 1 }} />
          {seatsLeft > 0 ? (
            <span style={{ color: '#4338ca', textTransform: 'none', letterSpacing: 0 }}>
              Можно зачислить ещё {seatsLeft}
            </span>
          ) : (
            <span style={{ color: '#92400e', textTransform: 'none', letterSpacing: 0 }}>
              Мест нет — новые идут в лист ожидания
            </span>
          )}
        </div>
        <div style={{ flex: 1, overflowY: 'auto' }}>
          {filtered.length === 0 ? (
            <EmptyPool query={query} />
          ) : filtered.map(s => (
            <StudentRow key={s.id} student={s}
              inRoster={isInRoster(s.id)}
              isEnrolled={inEnrolled(s.id)}
              isWaitlist={inWaitlist(s.id)}
              onToggle={() => onToggle(s.id)}
              fitsLevel={s.level === groupLevel}
              seatsLeft={seatsLeft}
            />
          ))}
        </div>
      </div>
    </div>
  );
}

function LevelFilter({ value, onChange }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);

  const toggle = (lvl) => {
    const next = new Set(value);
    next.has(lvl) ? next.delete(lvl) : next.add(lvl);
    onChange(next);
  };
  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button onClick={() => setOpen(o => !o)}
        style={{
          display: 'inline-flex', alignItems: 'center', gap: 6,
          height: 40, padding: '0 12px', borderRadius: 10,
          border: `1px solid ${value.size ? '#c7d6fe' : '#e2e8f0'}`,
          background: value.size ? '#f0f4ff' : '#fff',
          color: value.size ? '#4338ca' : '#334155',
          fontSize: 13, fontWeight: 500, fontFamily: 'inherit', cursor: 'pointer',
        }}>
        <Icon.GraduationCap size={14} />Уровень
        {value.size > 0 && (
          <span style={{
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            minWidth: 18, height: 18, padding: '0 5px', borderRadius: 9999,
            background: '#4f46e5', color: '#fff', fontSize: 11, fontWeight: 600,
          }}>{value.size}</span>
        )}
        <Icon.ChevronDown size={13} stroke={value.size ? '#4338ca' : '#94a3b8'} />
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 4px)', right: 0, zIndex: 30,
          minWidth: 180, background: '#fff', border: '1px solid #e2e8f0',
          borderRadius: 12, boxShadow: '0 10px 30px rgba(15,23,42,0.10)',
          padding: 6,
        }}>
          {STUDENT_LEVELS.map(lvl => {
            const checked = value.has(lvl);
            return (
              <button key={lvl} onClick={() => toggle(lvl)}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '7px 10px', borderRadius: 8, border: 'none',
                  background: checked ? '#f0f4ff' : 'transparent', cursor: 'pointer',
                  textAlign: 'left', fontFamily: 'inherit',
                }}>
                <span style={{
                  width: 16, height: 16, borderRadius: 4, flexShrink: 0,
                  border: `1.5px solid ${checked ? '#4f46e5' : '#cbd5e1'}`,
                  background: checked ? '#4f46e5' : '#fff',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  {checked && <Icon.Check size={11} stroke="#fff" sw={3} />}
                </span>
                <span style={{
                  fontFamily: 'var(--edv-font-mono)', fontSize: 12, fontWeight: 600,
                  color: '#475569', minWidth: 22,
                }}>{lvl}</span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

function StudentRow({ student: s, inRoster, isEnrolled, isWaitlist, onToggle, fitsLevel, seatsLeft }) {
  const tone = LEVEL_TONES[GROUP_LEVELS.find(l => l.value === s.level)?.tone || 'slate'];
  const statusDef = STUDENT_STATUS_LABELS[s.status];
  const statusTone = LEVEL_TONES[statusDef?.tone || 'slate'];
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 12,
      padding: '12px 14px', borderBottom: '1px solid #f1f5f9',
      background: inRoster ? 'rgba(79,70,229,0.03)' : 'transparent',
      transition: 'background .1s',
    }}
      onMouseEnter={e => { if (!inRoster) e.currentTarget.style.background = '#fafbfc'; }}
      onMouseLeave={e => { if (!inRoster) e.currentTarget.style.background = 'transparent'; }}>
      <Avatar name={s.name} size={36} />
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <span style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>{s.name}</span>
          <span style={{
            padding: '2px 7px', borderRadius: 6, fontSize: 11, fontWeight: 700,
            fontFamily: 'var(--edv-font-mono)', background: tone.bg, color: tone.fg,
          }}>{s.level}</span>
          {fitsLevel && !inRoster && (
            <span style={{
              display: 'inline-flex', alignItems: 'center', gap: 4,
              fontSize: 11, color: '#047857', fontWeight: 600,
            }}>
              <Icon.Check size={10} sw={3} stroke="#047857" />совпадает
            </span>
          )}
        </div>
        <div style={{ fontSize: 11.5, color: '#64748b', marginTop: 3,
          display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap' }}>
          <span>{s.age} лет</span>
          <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
          <span style={{ fontFamily: 'var(--edv-font-mono)' }}>{s.email}</span>
          <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
          <span style={{
            padding: '1px 7px', borderRadius: 9999,
            background: statusTone.bg, color: statusTone.fg, fontWeight: 600,
          }}>{statusDef?.label}</span>
        </div>
        {(s.tags.length > 0 || s.note) && (
          <div style={{ marginTop: 6, display: 'flex', alignItems: 'center', gap: 6, flexWrap: 'wrap' }}>
            {s.tags.map(tg => {
              const tdef = STUDENT_TAGS[tg];
              return (
                <span key={tg} style={{
                  display: 'inline-flex', alignItems: 'center', gap: 4,
                  fontSize: 10.5, padding: '2px 6px', borderRadius: 6,
                  background: tdef.bg, color: tdef.fg, fontWeight: 600,
                }}>{tdef.label}</span>
              );
            })}
            {s.note && (
              <span style={{ fontSize: 11, color: '#94a3b8' }}>· {s.note}</span>
            )}
          </div>
        )}
      </div>
      <AddRemoveButton
        inEnrolled={isEnrolled}
        inWaitlist={isWaitlist}
        wouldGoToWaitlist={!inRoster && seatsLeft === 0}
        onClick={onToggle}
      />
    </div>
  );
}

function AddRemoveButton({ inEnrolled, inWaitlist, wouldGoToWaitlist, onClick }) {
  if (inEnrolled) {
    return (
      <button onClick={onClick} style={{
        display: 'inline-flex', alignItems: 'center', gap: 6, padding: '0 12px',
        height: 32, borderRadius: 8, border: '1px solid #c7d6fe',
        background: '#f0f4ff', color: '#4338ca',
        fontSize: 12.5, fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer',
        flexShrink: 0,
      }}>
        <Icon.Check size={13} sw={2.5} />Зачислен
      </button>
    );
  }
  if (inWaitlist) {
    return (
      <button onClick={onClick} style={{
        display: 'inline-flex', alignItems: 'center', gap: 6, padding: '0 12px',
        height: 32, borderRadius: 8, border: '1px solid rgba(245,158,11,0.4)',
        background: 'rgba(245,158,11,0.10)', color: '#92400e',
        fontSize: 12.5, fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer',
        flexShrink: 0,
      }}>
        <Icon.Clock size={13} />В ожидании
      </button>
    );
  }
  return (
    <button onClick={onClick} style={{
      display: 'inline-flex', alignItems: 'center', gap: 6, padding: '0 14px',
      height: 32, borderRadius: 8, border: '1px solid transparent',
      background: wouldGoToWaitlist ? '#fff' : '#4f46e5',
      color: wouldGoToWaitlist ? '#92400e' : '#fff',
      borderColor: wouldGoToWaitlist ? 'rgba(245,158,11,0.4)' : 'transparent',
      fontSize: 12.5, fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer',
      flexShrink: 0, transition: 'background .15s',
    }}
      onMouseEnter={e => {
        if (!wouldGoToWaitlist) e.currentTarget.style.background = '#4338ca';
      }}
      onMouseLeave={e => {
        if (!wouldGoToWaitlist) e.currentTarget.style.background = '#4f46e5';
      }}>
      {wouldGoToWaitlist ? (
        <><Icon.Clock size={13} />В ожидание</>
      ) : (
        <><Icon.Plus size={13} sw={2.5} />Зачислить</>
      )}
    </button>
  );
}

function EmptyPool({ query }) {
  return (
    <div style={{
      display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 8,
      padding: '40px 16px', color: '#64748b', textAlign: 'center',
    }}>
      <div style={{
        width: 48, height: 48, borderRadius: 12, background: '#f8fafc',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Search size={22} stroke="#cbd5e1" />
      </div>
      <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>
        Никого не нашли
      </div>
      <div style={{ fontSize: 12.5, color: '#64748b', maxWidth: 320 }}>
        {query
          ? <>По запросу <strong>«{query}»</strong> ничего не подходит. Попробуйте другой фильтр или пригласите по email ниже.</>
          : <>Здесь пусто. Снимите фильтры или пригласите новых студентов по email ниже.</>
        }
      </div>
    </div>
  );
}

window.CapacityOverview = CapacityOverview;
window.StudentPool = StudentPool;
