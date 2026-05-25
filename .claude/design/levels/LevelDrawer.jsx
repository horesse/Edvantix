// Slide-over drawer for create / edit of a directory entry.
// Designed so it can be reused for any reference: name + code + color + description + status.
const { useState: useStateD, useEffect: useEffectD } = React;

function LevelDrawer({ open, mode, initial, onClose, onSave, onDelete }) {
  const empty = { name: '', code: '', color: 'indigo', description: '', status: 'active' };
  const [form, setForm] = useStateD(empty);
  const [mounted, setMounted] = useStateD(false);

  useEffectD(() => {
    if (open) {
      setForm(initial ? { ...empty, ...initial } : empty);
      setMounted(true);
    }
  }, [open, initial]);

  if (!open) return null;

  const set = (k, v) => setForm(f => ({ ...f, [k]: v }));
  const title = mode === 'edit' ? 'Изменить уровень' : 'Новый уровень';
  const canSave = form.name.trim().length > 0;

  return (
    <div style={{ position: 'fixed', inset: 0, zIndex: 80, display: 'flex' }}>
      <div onClick={onClose} style={{
        position: 'absolute', inset: 0, background: 'rgba(15,23,42,0.35)',
        animation: 'fadeIn .15s ease',
      }}/>
      <aside style={{
        marginLeft: 'auto', width: 460, background: '#fff', height: '100%',
        boxShadow: '-12px 0 32px -8px rgba(15,23,42,0.18)',
        display: 'flex', flexDirection: 'column', position: 'relative',
        animation: 'slideInRight .18s ease',
      }}>
        <div style={{
          padding: '18px 24px', borderBottom: '1px solid #e2e8f0',
          display: 'flex', alignItems: 'center', gap: 12,
        }}>
          <div style={{
            width: 36, height: 36, borderRadius: 10, flexShrink: 0,
            background: `${COLOR_DOTS[form.color]}1f`,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
          }}>
            <span style={{
              width: 12, height: 12, borderRadius: 999, background: COLOR_DOTS[form.color],
            }}/>
          </div>
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ fontSize: 16, fontWeight: 600 }}>{title}</div>
            <div style={{ fontSize: 12, color: '#94a3b8', marginTop: 2 }}>
              Справочник «Уровень»
            </div>
          </div>
          <button onClick={onClose} aria-label="Закрыть" style={{
            width: 32, height: 32, borderRadius: 8, border: '0', background: 'transparent',
            display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#64748b',
          }}
            onMouseEnter={e => e.currentTarget.style.background = '#f1f5f9'}
            onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
            <Icon.X size={18}/>
          </button>
        </div>

        <div style={{ flex: 1, overflowY: 'auto', padding: 24, display: 'flex', flexDirection: 'column', gap: 18 }}>
          <Field label="Название" required>
            <input
              autoFocus value={form.name} onChange={e => set('name', e.target.value)}
              placeholder="Например, B1 — Средний"
              style={inputStyle}
              onFocus={focusInput} onBlur={blurInput}
            />
          </Field>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
            <Field label="Код" hint="до 8 символов">
              <input
                value={form.code} onChange={e => set('code', e.target.value.slice(0,8))}
                placeholder="B1"
                style={{ ...inputStyle, fontFamily: 'var(--edv-font-mono)', textTransform: 'uppercase', letterSpacing: '0.04em' }}
                onFocus={focusInput} onBlur={blurInput}
              />
            </Field>
            <Field label="Статус">
              <div style={{
                display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 4,
                background: '#f1f5f9', borderRadius: 10, padding: 4, height: 38,
              }}>
                {[['active', 'Активный'], ['archived', 'Архив']].map(([v, l]) => {
                  const on = form.status === v;
                  return (
                    <button key={v} onClick={() => set('status', v)} style={{
                      borderRadius: 8, border: 0, fontSize: 13, fontWeight: 500,
                      background: on ? '#fff' : 'transparent',
                      color: on ? '#0f172a' : '#64748b',
                      boxShadow: on ? '0 1px 2px rgba(15,23,42,0.08)' : 'none',
                    }}>{l}</button>
                  );
                })}
              </div>
            </Field>
          </div>

          <Field label="Цвет метки">
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
              {Object.entries(COLOR_DOTS).map(([k, v]) => {
                const on = form.color === k;
                return (
                  <button key={k} onClick={() => set('color', k)} aria-label={k} style={{
                    width: 30, height: 30, borderRadius: 10, border: on ? `2px solid ${v}` : '1px solid #e2e8f0',
                    background: '#fff', padding: 0, display: 'flex', alignItems: 'center', justifyContent: 'center',
                  }}>
                    <span style={{ width: 14, height: 14, borderRadius: 999, background: v }}/>
                  </button>
                );
              })}
            </div>
          </Field>

          <Field label="Описание" hint="видно только в справочнике">
            <textarea
              value={form.description} onChange={e => set('description', e.target.value)}
              rows={4}
              placeholder="Кратко: для кого, что входит, как используется в курсах"
              style={{ ...inputStyle, resize: 'vertical', minHeight: 88, lineHeight: 1.5 }}
              onFocus={focusInput} onBlur={blurInput}
            />
          </Field>

          {mode === 'edit' && initial?.usage && (
            <div style={{
              marginTop: 4, padding: 14, background: '#f8fafc', borderRadius: 12,
              border: '1px solid #e2e8f0',
            }}>
              <div style={{ fontSize: 12, fontWeight: 600, color: '#475569', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: 8 }}>
                Где используется
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 8 }}>
                {[
                  ['Группы', initial.usage.groups],
                  ['Курсы', initial.usage.courses],
                  ['Студенты', initial.usage.students],
                ].map(([l, n]) => (
                  <div key={l}>
                    <div style={{ fontSize: 19, fontWeight: 600, color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>{n}</div>
                    <div style={{ fontSize: 12, color: '#64748b' }}>{l}</div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        <div style={{
          padding: '14px 24px', borderTop: '1px solid #e2e8f0',
          display: 'flex', alignItems: 'center', gap: 10,
        }}>
          {mode === 'edit' && (
            <button onClick={() => onDelete?.(initial)} style={{
              ...btnGhost, color: '#b91c1c',
            }}
              onMouseEnter={e => e.currentTarget.style.background = '#fef2f2'}
              onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
              <Icon.Trash size={15}/> Удалить
            </button>
          )}
          <div style={{ marginLeft: 'auto', display: 'flex', gap: 8 }}>
            <Button variant="secondary" onClick={onClose}>Отмена</Button>
            <Button onClick={() => canSave && onSave(form)} disabled={!canSave}>
              {mode === 'edit' ? 'Сохранить' : 'Создать'}
            </Button>
          </div>
        </div>
      </aside>
    </div>
  );
}

function Field({ label, hint, required, children }) {
  return (
    <label style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
      <div style={{ display: 'flex', alignItems: 'baseline', gap: 6 }}>
        <span style={{ fontSize: 13, fontWeight: 500, color: '#334155' }}>{label}</span>
        {required && <span style={{ color: '#ef4444', fontSize: 13 }}>*</span>}
        {hint && <span style={{ fontSize: 12, color: '#94a3b8', marginLeft: 'auto' }}>{hint}</span>}
      </div>
      {children}
    </label>
  );
}

const inputStyle = {
  border: '1px solid #e2e8f0', background: '#fff', borderRadius: 10,
  padding: '10px 12px', fontSize: 14, fontFamily: 'inherit', outline: 'none',
  width: '100%', transition: '.15s', color: '#0f172a',
};
function focusInput(e) { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.18)'; }
function blurInput(e)  { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }

const btnGhost = {
  display: 'inline-flex', alignItems: 'center', gap: 8,
  padding: '8px 12px', fontSize: 13.5, fontWeight: 500,
  borderRadius: 8, border: '0', background: 'transparent', cursor: 'pointer',
  transition: '.12s',
};

window.LevelDrawer = LevelDrawer;
