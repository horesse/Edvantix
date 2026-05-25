// Member profile — small components

// ── InfoRow: label · value ───────────────────────────────────────────
function InfoRow({ label, children, mono }) {
  return (
    <div style={{
      display: 'grid', gridTemplateColumns: '120px 1fr', gap: 14,
      padding: '10px 0', borderBottom: '1px solid #f1f5f9', alignItems: 'baseline',
    }}>
      <div style={{ fontSize: 12.5, color: '#64748b', fontWeight: 500 }}>{label}</div>
      <div style={{
        fontSize: 13.5, color: '#0f172a',
        fontFamily: mono ? 'var(--edv-font-mono)' : 'inherit',
        fontVariantNumeric: mono ? 'tabular-nums' : 'normal',
      }}>{children}</div>
    </div>
  );
}

// ── SectionCard with header/title ────────────────────────────────────
function SectionCard({ title, action, children, padding = '20px 22px' }) {
  return (
    <section style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
      overflow: 'hidden',
    }}>
      {(title || action) && (
        <header style={{
          padding: '14px 22px', borderBottom: '1px solid #f1f5f9',
          display: 'flex', alignItems: 'center', gap: 10,
        }}>
          <h2 style={{ margin: 0, flex: 1, fontSize: 14, fontWeight: 600, color: '#0f172a' }}>
            {title}
          </h2>
          {action}
        </header>
      )}
      <div style={{ padding }}>{children}</div>
    </section>
  );
}

// ── Stat tile (compact) ──────────────────────────────────────────────
function Stat({ label, value, suffix, sub, tone = 'slate', icon }) {
  const tones = {
    slate:   { bg: '#f1f5f9',                fg: '#475569' },
    success: { bg: 'rgba(16,185,129,0.12)', fg: '#047857' },
    primary: { bg: 'rgba(79,70,229,0.10)',  fg: '#4338ca' },
    amber:   { bg: 'rgba(245,158,11,0.14)', fg: '#92400e' },
    teal:    { bg: 'rgba(20,184,166,0.12)', fg: '#0f766e' },
  }[tone];
  const IC = icon ? Icon[icon] : null;
  return (
    <div style={{
      flex: 1, minWidth: 0,
      padding: '14px 16px',
      borderRight: '1px solid #f1f5f9',
      display: 'flex', alignItems: 'center', gap: 12,
    }}>
      {IC && (
        <div style={{
          width: 36, height: 36, borderRadius: 10, flexShrink: 0,
          background: tones.bg, color: tones.fg,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}><IC size={17} stroke={tones.fg} /></div>
      )}
      <div style={{ minWidth: 0, flex: 1 }}>
        <div style={{ display: 'flex', alignItems: 'baseline', gap: 4 }}>
          <div style={{
            fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em',
            color: '#0f172a', fontVariantNumeric: 'tabular-nums', lineHeight: 1,
          }}>{value}</div>
          {suffix && (
            <div style={{ fontSize: 12, color: '#64748b', fontWeight: 500 }}>{suffix}</div>
          )}
        </div>
        <div style={{
          fontSize: 12, color: '#64748b', fontWeight: 500, marginTop: 4,
          whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
        }}>{label}</div>
        {sub && (
          <div style={{ fontSize: 11, color: '#94a3b8', marginTop: 2 }}>{sub}</div>
        )}
      </div>
    </div>
  );
}

// ── Group card row ───────────────────────────────────────────────────
function GroupRow({ g }) {
  const isCurator = g.role === 'curator';
  return (
    <div style={{
      display: 'grid',
      gridTemplateColumns: '54px 1fr 96px 130px 24px',
      gap: 14, alignItems: 'center',
      padding: '12px 16px',
      borderBottom: '1px solid #f1f5f9',
      transition: 'background .1s',
    }}
      onMouseEnter={e => e.currentTarget.style.background = '#fafbfc'}
      onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
    >
      <div style={{
        fontFamily: 'var(--edv-font-mono)', fontSize: 11,
        color: '#4338ca', background: 'rgba(79,70,229,0.08)',
        padding: '4px 8px', borderRadius: 6, textAlign: 'center',
        fontWeight: 600, letterSpacing: '0.02em',
      }}>{g.code}</div>
      <div style={{ minWidth: 0 }}>
        <div style={{ fontSize: 13.5, fontWeight: 500, color: '#0f172a',
          whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
          {g.name}
        </div>
        <div style={{ fontSize: 12, color: '#64748b', marginTop: 2,
          display: 'flex', alignItems: 'center', gap: 8 }}>
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
            <Icon.Users size={11} stroke="#94a3b8" />{g.students}
          </span>
          <span style={{ color: '#cbd5e1' }}>·</span>
          <span>{g.schedule}</span>
        </div>
      </div>
      <div>
        {isCurator ? (
          <span style={{
            display: 'inline-flex', alignItems: 'center',
            padding: '3px 10px', borderRadius: 6,
            background: 'rgba(245,158,11,0.14)', color: '#92400e',
            fontSize: 11.5, fontWeight: 500,
          }}>Куратор</span>
        ) : (
          <span style={{
            display: 'inline-flex', alignItems: 'center',
            padding: '3px 10px', borderRadius: 6,
            background: 'rgba(14,165,233,0.12)', color: '#0369a1',
            fontSize: 11.5, fontWeight: 500,
          }}>Преподаватель</span>
        )}
      </div>
      <div>
        {g.progress != null ? (
          <div>
            <div style={{
              height: 6, borderRadius: 9999, background: '#f1f5f9', overflow: 'hidden',
            }}>
              <div style={{
                height: '100%', width: `${g.progress * 100}%`,
                background: 'linear-gradient(90deg,#6366f1,#4f46e5)', borderRadius: 9999,
              }} />
            </div>
            <div style={{
              fontSize: 11, color: '#64748b', marginTop: 4,
              fontVariantNumeric: 'tabular-nums',
            }}>Курс пройден на {Math.round(g.progress * 100)}%</div>
          </div>
        ) : (
          <span style={{ fontSize: 11.5, color: '#94a3b8' }}>—</span>
        )}
      </div>
      <button style={{
        background: 'transparent', border: 'none', cursor: 'pointer',
        color: '#94a3b8', display: 'flex', alignItems: 'center',
      }}>
        <Icon.ChevronRight size={16} />
      </button>
    </div>
  );
}

// ── Schedule row ─────────────────────────────────────────────────────
function ScheduleRow({ item }) {
  const kindStyles = {
    lesson:  { label: 'Урок',         bg: 'rgba(79,70,229,0.10)',  fg: '#4338ca' },
    consult: { label: 'Консультация', bg: 'rgba(20,184,166,0.12)', fg: '#0f766e' },
    meeting: { label: 'Встреча',      bg: 'rgba(245,158,11,0.14)', fg: '#92400e' },
  }[item.kind];

  return (
    <div style={{
      display: 'grid',
      gridTemplateColumns: '74px 110px 1fr 72px',
      gap: 14, alignItems: 'center',
      padding: '12px 16px',
      borderBottom: '1px solid #f1f5f9',
    }}>
      <div>
        <div style={{ fontSize: 12.5, fontWeight: 600, color: '#0f172a' }}>{item.day}</div>
        <div style={{ fontSize: 11, color: '#94a3b8', marginTop: 1 }}>{item.date}</div>
      </div>
      <div style={{
        fontSize: 13, color: '#0f172a', fontVariantNumeric: 'tabular-nums',
        fontFamily: 'var(--edv-font-mono)',
      }}>{item.time}</div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, minWidth: 0 }}>
        <span style={{
          padding: '2px 8px', borderRadius: 4, fontSize: 11, fontWeight: 500,
          background: kindStyles.bg, color: kindStyles.fg, flexShrink: 0,
        }}>{kindStyles.label}</span>
        <span style={{ fontSize: 13, color: '#0f172a', fontWeight: 500 }}>
          {item.subject}
        </span>
        <span style={{ fontSize: 12, color: '#64748b' }}>· {item.group}</span>
      </div>
      <div style={{
        fontSize: 12, color: '#475569', textAlign: 'right',
        fontVariantNumeric: 'tabular-nums',
      }}>каб. {item.room}</div>
    </div>
  );
}

// ── Document chip ────────────────────────────────────────────────────
function DocumentChip({ doc }) {
  const ext = doc.kind === 'img' ? 'JPG' : 'PDF';
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 12,
      padding: '12px 14px', borderRadius: 10,
      border: '1px solid #e2e8f0', background: '#fff',
      cursor: 'pointer', transition: 'all .12s',
    }}
      onMouseEnter={e => { e.currentTarget.style.borderColor = '#c7d6fe'; e.currentTarget.style.background = '#fafbff'; }}
      onMouseLeave={e => { e.currentTarget.style.borderColor = '#e2e8f0'; e.currentTarget.style.background = '#fff'; }}
    >
      <div style={{
        width: 36, height: 44, borderRadius: 6, flexShrink: 0,
        background: doc.kind === 'img' ? 'linear-gradient(160deg,#fef3c7,#fde68a)' : 'linear-gradient(160deg,#fee2e2,#fecaca)',
        color: doc.kind === 'img' ? '#92400e' : '#b91c1c',
        fontSize: 9, fontWeight: 700, letterSpacing: '0.05em',
        display: 'flex', alignItems: 'flex-end', justifyContent: 'center',
        padding: '0 0 4px',
      }}>{ext}</div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{
          fontSize: 13, fontWeight: 500, color: '#0f172a',
          whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
        }}>{doc.name}</div>
        <div style={{ fontSize: 11.5, color: '#94a3b8', marginTop: 2 }}>{doc.meta}</div>
      </div>
      <Icon.ArrowRight size={14} stroke="#94a3b8" />
    </div>
  );
}

// ── Qualification chip ───────────────────────────────────────────────
function QualificationItem({ q }) {
  const iconMap = {
    edu:      { icon: 'GraduationCap', tone: { bg: 'rgba(79,70,229,0.10)', fg: '#4338ca' } },
    category: { icon: 'Shield',         tone: { bg: 'rgba(16,185,129,0.12)', fg: '#047857' } },
    cert:     { icon: 'FileText',       tone: { bg: 'rgba(14,165,233,0.12)', fg: '#0369a1' } },
    award:    { icon: 'Sparkles',       tone: { bg: 'rgba(245,158,11,0.14)', fg: '#92400e' } },
  }[q.kind];
  const IC = Icon[iconMap.icon];
  return (
    <div style={{
      display: 'flex', alignItems: 'flex-start', gap: 12,
      padding: '12px 0', borderBottom: '1px solid #f1f5f9',
    }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: iconMap.tone.bg, color: iconMap.tone.fg,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}><IC size={15} stroke={iconMap.tone.fg} /></div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 13.5, fontWeight: 500, color: '#0f172a' }}>{q.title}</div>
        <div style={{ fontSize: 12, color: '#64748b', marginTop: 2 }}>{q.meta}</div>
      </div>
    </div>
  );
}

// ── Activity item ────────────────────────────────────────────────────
function ActivityItem({ item, last }) {
  const IC = Icon[item.icon];
  return (
    <div style={{
      display: 'flex', alignItems: 'flex-start', gap: 12,
      paddingBottom: last ? 0 : 16, position: 'relative',
    }}>
      {!last && (
        <div style={{
          position: 'absolute', left: 13, top: 28, bottom: 0, width: 1,
          background: '#e2e8f0',
        }} />
      )}
      <div style={{
        width: 28, height: 28, borderRadius: 9999, flexShrink: 0,
        background: '#f8fafc', border: '1px solid #e2e8f0',
        color: '#64748b',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        position: 'relative', zIndex: 1,
      }}><IC size={13} stroke="#64748b" /></div>
      <div style={{ flex: 1, minWidth: 0, paddingTop: 4 }}>
        <div style={{ fontSize: 13, color: '#0f172a' }}>{item.text}</div>
        <div style={{ fontSize: 11.5, color: '#94a3b8', marginTop: 2 }}>{item.when}</div>
      </div>
    </div>
  );
}

// ── Internal note ────────────────────────────────────────────────────
function NoteCard({ n }) {
  return (
    <div style={{
      padding: '14px 16px',
      background: 'rgba(245,158,11,0.06)',
      border: '1px solid rgba(245,158,11,0.20)',
      borderRadius: 12,
    }}>
      <div style={{
        fontSize: 13, color: '#0f172a', lineHeight: 1.5,
      }}>{n.text}</div>
      <div style={{
        marginTop: 8, fontSize: 11.5, color: '#92400e', fontWeight: 500,
        display: 'flex', alignItems: 'center', gap: 8,
      }}>
        <Avatar name={n.author} size={18} />
        <span>{n.author}</span>
        <span style={{ color: '#cbd5e1' }}>·</span>
        <span style={{ color: '#94a3b8', fontWeight: 400 }}>{n.when}</span>
      </div>
    </div>
  );
}

Object.assign(window, {
  InfoRow, SectionCard, Stat, GroupRow, ScheduleRow,
  DocumentChip, QualificationItem, ActivityItem, NoteCard,
});
