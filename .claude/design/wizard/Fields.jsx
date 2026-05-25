// Form field primitives
const F = {};

// ── Field wrapper with label, hint, error ─────────────────────────────
F.Field = function Field({ label, required, hint, error, children, optional }) {
  return (
    <label style={{ display: 'block' }}>
      <div style={{ display: 'flex', alignItems: 'baseline', gap: 8, marginBottom: 6 }}>
        <span style={{ fontSize: 13, fontWeight: 500, color: '#0f172a' }}>
          {label}
          {required && <span style={{ color: '#ef4444', marginLeft: 2 }}>*</span>}
        </span>
        {optional && <span style={{ fontSize: 11, color: '#94a3b8' }}>необязательно</span>}
      </div>
      {children}
      {(error || hint) && (
        <div style={{
          marginTop: 6, fontSize: 12,
          color: error ? '#b91c1c' : '#64748b',
          display: 'flex', alignItems: 'flex-start', gap: 6, lineHeight: 1.4,
        }}>
          {error && <Icon.AlertCircle size={13} stroke="#b91c1c" style={{ flexShrink: 0, marginTop: 1 }} />}
          <span>{error || hint}</span>
        </div>
      )}
    </label>
  );
};

// ── Text input ────────────────────────────────────────────────────────
F.Text = function Text({ error, icon, ...p }) {
  const [focused, setFocused] = React.useState(false);
  const borderColor = error ? '#ef4444' : focused ? '#6366f1' : '#e2e8f0';
  const ring = focused && !error
    ? '0 0 0 3px rgba(99,102,241,0.25)'
    : error
      ? '0 0 0 3px rgba(239,68,68,0.15)'
      : 'none';
  return (
    <div style={{ position: 'relative' }}>
      {icon && (
        <div style={{
          position: 'absolute', left: 12, top: '50%', transform: 'translateY(-50%)',
          color: '#94a3b8', pointerEvents: 'none',
        }}>
          {icon}
        </div>
      )}
      <input
        {...p}
        onFocus={e => { setFocused(true); p.onFocus?.(e); }}
        onBlur={e => { setFocused(false); p.onBlur?.(e); }}
        style={{
          width: '100%', height: 42, borderRadius: 12,
          border: `1px solid ${borderColor}`, background: '#fff',
          padding: icon ? '0 14px 0 38px' : '0 14px',
          fontSize: 14, fontFamily: 'inherit', color: '#0f172a',
          outline: 'none', boxShadow: ring,
          transition: 'border-color .15s, box-shadow .15s',
        }}
      />
    </div>
  );
};

// ── Textarea ──────────────────────────────────────────────────────────
F.Textarea = function Textarea({ error, ...p }) {
  const [focused, setFocused] = React.useState(false);
  const borderColor = error ? '#ef4444' : focused ? '#6366f1' : '#e2e8f0';
  const ring = focused && !error
    ? '0 0 0 3px rgba(99,102,241,0.25)'
    : error
      ? '0 0 0 3px rgba(239,68,68,0.15)'
      : 'none';
  return (
    <textarea
      {...p}
      onFocus={e => { setFocused(true); p.onFocus?.(e); }}
      onBlur={e => { setFocused(false); p.onBlur?.(e); }}
      style={{
        width: '100%', minHeight: 84, borderRadius: 12,
        border: `1px solid ${borderColor}`, background: '#fff',
        padding: '10px 14px', fontSize: 14, fontFamily: 'inherit',
        color: '#0f172a', outline: 'none', boxShadow: ring, resize: 'vertical',
        lineHeight: 1.5, transition: 'border-color .15s, box-shadow .15s',
      }}
    />
  );
};

// ── Select (native styled) ────────────────────────────────────────────
F.Select = function Select({ value, onChange, options, placeholder, error }) {
  const [focused, setFocused] = React.useState(false);
  const borderColor = error ? '#ef4444' : focused ? '#6366f1' : '#e2e8f0';
  const ring = focused && !error
    ? '0 0 0 3px rgba(99,102,241,0.25)'
    : error
      ? '0 0 0 3px rgba(239,68,68,0.15)'
      : 'none';
  return (
    <div style={{ position: 'relative' }}>
      <select
        value={value || ''}
        onChange={e => onChange(e.target.value)}
        onFocus={() => setFocused(true)}
        onBlur={() => setFocused(false)}
        style={{
          width: '100%', height: 42, borderRadius: 12,
          border: `1px solid ${borderColor}`, background: '#fff',
          padding: '0 40px 0 14px', fontSize: 14, fontFamily: 'inherit',
          color: value ? '#0f172a' : '#94a3b8',
          outline: 'none', boxShadow: ring, appearance: 'none',
          cursor: 'pointer',
          transition: 'border-color .15s, box-shadow .15s',
        }}
      >
        {placeholder && <option value="" disabled>{placeholder}</option>}
        {options.map(o => (
          <option key={o.value} value={o.value} style={{ color: '#0f172a' }}>{o.label}</option>
        ))}
      </select>
      <Icon.ChevronDown size={16} stroke="#94a3b8"
        style={{ position: 'absolute', right: 14, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }} />
    </div>
  );
};

// ── Segmented control ─────────────────────────────────────────────────
F.Segmented = function Segmented({ value, onChange, options }) {
  return (
    <div style={{
      display: 'inline-flex', background: '#f1f5f9', borderRadius: 10, padding: 3,
      gap: 2, border: '1px solid #e2e8f0',
    }}>
      {options.map(o => {
        const active = value === o.value;
        return (
          <button
            key={o.value}
            type="button"
            onClick={() => onChange(o.value)}
            style={{
              display: 'inline-flex', alignItems: 'center', gap: 6,
              padding: '7px 14px', borderRadius: 8, border: 'none',
              background: active ? '#fff' : 'transparent',
              color: active ? '#0f172a' : '#64748b',
              fontSize: 13, fontWeight: active ? 600 : 500,
              cursor: 'pointer', fontFamily: 'inherit',
              boxShadow: active ? '0 1px 2px rgba(0,0,0,0.08)' : 'none',
              transition: 'all .15s',
            }}
          >
            {o.icon}{o.label}
          </button>
        );
      })}
    </div>
  );
};

// ── Card-style radio grid (for LegalForm / OrganizationType) ────────
F.CardRadio = function CardRadio({ value, onChange, options, columns = 3 }) {
  return (
    <div style={{
      display: 'grid',
      gridTemplateColumns: `repeat(${columns}, minmax(0, 1fr))`,
      gap: 10,
    }}>
      {options.map(o => {
        const active = value === o.value;
        return (
          <button
            key={o.value}
            type="button"
            onClick={() => onChange(o.value)}
            style={{
              textAlign: 'left', cursor: 'pointer', fontFamily: 'inherit',
              padding: '14px 14px', borderRadius: 12,
              border: `1px solid ${active ? '#4f46e5' : '#e2e8f0'}`,
              background: active ? 'rgba(79,70,229,0.04)' : '#fff',
              boxShadow: active ? '0 0 0 3px rgba(79,70,229,0.12)' : 'none',
              display: 'flex', flexDirection: 'column', gap: 4,
              transition: 'all .15s',
              position: 'relative',
            }}
            onMouseEnter={e => { if (!active) { e.currentTarget.style.borderColor = '#c7d6fe'; e.currentTarget.style.background = '#fafbff'; } }}
            onMouseLeave={e => { if (!active) { e.currentTarget.style.borderColor = '#e2e8f0'; e.currentTarget.style.background = '#fff'; } }}
          >
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 8 }}>
              <div style={{
                display: 'inline-flex', alignItems: 'center',
                padding: '2px 8px', borderRadius: 6,
                background: active ? '#4f46e5' : '#f1f5f9',
                color: active ? '#fff' : '#475569',
                fontSize: 12, fontWeight: 700, letterSpacing: '0.01em',
                fontVariantNumeric: 'tabular-nums',
              }}>{o.tag}</div>
              {active && (
                <div style={{
                  width: 18, height: 18, borderRadius: 9999, background: '#4f46e5',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  <Icon.Check size={12} stroke="#fff" sw={3} />
                </div>
              )}
            </div>
            <div style={{ fontSize: 13, color: '#334155', lineHeight: 1.35, marginTop: 2 }}>
              {o.label}
            </div>
          </button>
        );
      })}
    </div>
  );
};

window.F = F;
