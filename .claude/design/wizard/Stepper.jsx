// Vertical stepper for onboarding sidebar
function Stepper({ steps, current, completed, onJump }) {
  return (
    <ol style={{ listStyle: 'none', margin: 0, padding: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
      {steps.map((s, i) => {
        const isDone = completed.has(i);
        const isActive = i === current;
        const isFuture = !isDone && !isActive;
        const canJump = isDone || isActive;

        const circleBg = isActive ? '#4f46e5' : isDone ? '#e0eaff' : '#fff';
        const circleFg = isActive ? '#fff' : isDone ? '#4338ca' : '#94a3b8';
        const circleBd = isActive ? '#4f46e5' : isDone ? '#c7d6fe' : '#e2e8f0';

        return (
          <li key={s.id}>
            <button
              disabled={!canJump}
              onClick={() => canJump && onJump?.(i)}
              style={{
                display: 'flex', alignItems: 'flex-start', gap: 12, width: '100%',
                padding: '10px 12px', borderRadius: 10, border: 'none',
                background: isActive ? 'rgba(79,70,229,0.06)' : 'transparent',
                cursor: canJump ? 'pointer' : 'default',
                textAlign: 'left', fontFamily: 'inherit',
                transition: 'background .15s',
              }}
              onMouseEnter={e => { if (canJump && !isActive) e.currentTarget.style.background = '#f1f5f9'; }}
              onMouseLeave={e => { if (!isActive) e.currentTarget.style.background = 'transparent'; }}
            >
              <div style={{ position: 'relative', flexShrink: 0 }}>
                <div style={{
                  width: 28, height: 28, borderRadius: 9999,
                  background: circleBg, color: circleFg,
                  border: `1px solid ${circleBd}`,
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  fontSize: 13, fontWeight: 600,
                  boxShadow: isActive ? '0 0 0 4px rgba(79,70,229,0.12)' : 'none',
                  transition: 'all .2s',
                }}>
                  {isDone ? <Icon.Check size={14} stroke="#4338ca" sw={2.5} /> : i + 1}
                </div>
                {i < steps.length - 1 && (
                  <div style={{
                    position: 'absolute', left: 13, top: 30, bottom: -14, width: 2,
                    background: isDone ? '#c7d6fe' : '#e2e8f0',
                  }} />
                )}
              </div>
              <div style={{ flex: 1, minWidth: 0, paddingTop: 3 }}>
                <div style={{
                  fontSize: 13.5, fontWeight: isActive ? 600 : 500,
                  color: isActive ? '#0f172a' : isFuture ? '#94a3b8' : '#334155',
                  lineHeight: 1.3,
                }}>{s.title}</div>
                <div style={{
                  fontSize: 12, color: isFuture ? '#cbd5e1' : '#64748b',
                  marginTop: 2, lineHeight: 1.4,
                }}>{s.hint}</div>
              </div>
            </button>
          </li>
        );
      })}
    </ol>
  );
}

window.Stepper = Stepper;
