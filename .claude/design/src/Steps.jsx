// ── Step content components ────────────────────────────────────────────────

// ── Reusable form primitives ───────────────────────────────────────────────
function Field({ label, hint, optional, children, icon }) {
  const Glyph = icon ? Icon[icon] : null;
  return (
    <label style={{ display: 'block', marginBottom: 20 }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 8 }}>
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 13, fontWeight: 600, color: '#0f172a' }}>
          {Glyph && <Glyph size={14} stroke="#64748b" sw={2}/>}
          {label}
          {optional && <span style={{ color: '#94a3b8', fontWeight: 500 }}>· необязательно</span>}
        </span>
      </div>
      {children}
      {hint && (
        <div style={{ marginTop: 6, fontSize: 12.5, color: '#64748b', lineHeight: 1.45, display: 'flex', gap: 6 }}>
          <Icon.Info size={13} stroke="#94a3b8" sw={2} style={{ flexShrink: 0, marginTop: 1 }}/>
          <span>{hint}</span>
        </div>
      )}
    </label>
  );
}

function TextInput({ value, onChange, placeholder, prefix, suffix, autoFocus }) {
  const [focused, setFocused] = React.useState(false);
  return (
    <div style={{
      display: 'flex', alignItems: 'stretch',
      border: focused ? '1px solid #6366f1' : '1px solid #e2e8f0',
      background: '#fff', borderRadius: 12, overflow: 'hidden',
      boxShadow: focused ? '0 0 0 3px rgba(99,102,241,0.18)' : 'none',
      transition: 'all .15s',
    }}>
      {prefix && (
        <span style={{
          display: 'inline-flex', alignItems: 'center', padding: '0 12px',
          background: '#f8fafc', borderRight: '1px solid #e2e8f0',
          color: '#64748b', fontSize: 14, fontWeight: 500,
        }}>{prefix}</span>
      )}
      <input
        autoFocus={autoFocus}
        value={value} onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        onFocus={() => setFocused(true)} onBlur={() => setFocused(false)}
        style={{
          flex: 1, minWidth: 0, border: 0, outline: 'none',
          padding: '11px 14px', fontSize: 14, fontFamily: 'inherit',
          background: 'transparent', color: '#0f172a',
        }}
      />
      {suffix && (
        <span style={{
          display: 'inline-flex', alignItems: 'center', padding: '0 12px',
          background: '#f8fafc', borderLeft: '1px solid #e2e8f0',
          color: '#64748b', fontSize: 13.5, fontWeight: 500,
        }}>{suffix}</span>
      )}
    </div>
  );
}

function SegmentedSelect({ value, onChange, options }) {
  return (
    <div style={{
      display: 'grid', gridTemplateColumns: `repeat(${options.length}, 1fr)`,
      background: '#f1f5f9', padding: 4, borderRadius: 10, gap: 4,
    }}>
      {options.map(o => {
        const active = o.value === value;
        return (
          <button key={o.value} type="button" onClick={() => onChange(o.value)}
            style={{
              padding: '8px 6px', borderRadius: 7, border: 0,
              background: active ? '#fff' : 'transparent',
              color: active ? '#0f172a' : '#64748b',
              fontWeight: active ? 600 : 500, fontSize: 13,
              boxShadow: active ? '0 1px 2px rgba(0,0,0,0.06)' : 'none',
              transition: 'all .15s', cursor: 'pointer',
            }}>{o.label}</button>
        );
      })}
    </div>
  );
}

function CardChoice({ icon, title, desc, active, onClick, badge }) {
  const Glyph = icon ? Icon[icon] : null;
  return (
    <button type="button" onClick={onClick} style={{
      display: 'flex', alignItems: 'flex-start', gap: 14,
      padding: 16, borderRadius: 12, textAlign: 'left',
      border: active ? '1.5px solid #4f46e5' : '1px solid #e2e8f0',
      background: active ? '#f5f7ff' : '#fff',
      boxShadow: active ? '0 0 0 3px rgba(79,70,229,0.12)' : 'none',
      cursor: 'pointer', transition: 'all .15s',
      width: '100%',
    }}>
      {Glyph && (
        <span style={{
          flexShrink: 0,
          width: 36, height: 36, borderRadius: 10,
          background: active ? '#4f46e5' : '#f1f5f9',
          color: active ? '#fff' : '#475569',
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <Glyph size={18} sw={2}/>
        </span>
      )}
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 2 }}>
          <span style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{title}</span>
          {badge && (
            <span style={{
              fontSize: 11, fontWeight: 600, color: '#4338ca',
              background: '#e0eaff', padding: '2px 7px', borderRadius: 999,
            }}>{badge}</span>
          )}
        </div>
        <p style={{ margin: 0, fontSize: 13, color: '#64748b', lineHeight: 1.45 }}>{desc}</p>
      </div>
      <span style={{
        flexShrink: 0,
        width: 18, height: 18, borderRadius: 999,
        border: active ? '5px solid #4f46e5' : '1.5px solid #cbd5e1',
        background: '#fff', marginTop: 2,
      }}/>
    </button>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// WELCOME
// ═══════════════════════════════════════════════════════════════════════════
function WelcomeScreen({ onStart, onSkip }) {
  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', background: '#fff' }}>
      <header style={{
        height: 72, display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        padding: '0 32px', borderBottom: '1px solid #e2e8f0',
      }}>
        <Logo />
        <button onClick={onSkip} style={{
          background: 'transparent', border: 0, color: '#64748b',
          padding: '8px 14px', borderRadius: 8, fontSize: 13.5, fontWeight: 500,
        }}>Пропустить настройку →</button>
      </header>
      <main style={{
        flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center',
        padding: '48px 32px',
        backgroundImage: 'radial-gradient(circle at 20% 0%, rgba(99,102,241,0.07), transparent 50%), radial-gradient(circle at 80% 100%, rgba(244,63,94,0.05), transparent 50%)',
      }}>
        <div className="ob-scale" style={{ maxWidth: 880, width: '100%' }}>
          <div style={{
            display: 'inline-flex', alignItems: 'center', gap: 8,
            background: '#eef2ff', border: '1px solid #e0e7ff', color: '#4338ca',
            padding: '5px 14px', borderRadius: 999, fontSize: 13, fontWeight: 600, marginBottom: 24,
          }}>
            <Icon.Sparkles size={14} sw={2}/> Добро пожаловать, Алина
          </div>
          <h1 style={{
            margin: 0, fontSize: 56, lineHeight: 1.05, fontWeight: 800, letterSpacing: '-0.03em',
            maxWidth: 760,
          }}>
            Запустим вашу школу<br/>
            <span style={{ color: '#4f46e5' }}>за 5 минут</span>
          </h1>
          <p style={{
            margin: '20px 0 36px', fontSize: 18, color: '#475569', lineHeight: 1.55, maxWidth: 600,
          }}>
            Пройдём по пяти коротким шагам — настроим бренд, добавим команду и студентов, подключим платежи. Можно пропустить любой шаг и вернуться позже.
          </p>

          {/* Steps preview row */}
          <ol style={{
            display: 'grid', gridTemplateColumns: 'repeat(5, 1fr)', gap: 12,
            padding: 0, margin: '0 0 40px', listStyle: 'none',
          }}>
            {STEPS.map((s, i) => {
              const Glyph = Icon[s.icon];
              return (
                <li key={s.id} style={{
                  background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
                  padding: 18,
                  boxShadow: '0 1px 3px rgba(0,0,0,0.04)',
                  animation: `obFadeIn .4s ease-out ${0.05 * i}s both`,
                }}>
                  <div style={{
                    display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 14,
                  }}>
                    <span style={{
                      width: 40, height: 40, borderRadius: 10,
                      background: i === 0 ? '#4f46e5' : '#eef2ff',
                      color: i === 0 ? '#fff' : '#4f46e5',
                      display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                    }}>
                      <Glyph size={20} sw={2}/>
                    </span>
                    <span style={{
                      fontSize: 11, fontWeight: 700, color: '#94a3b8',
                      fontVariantNumeric: 'tabular-nums',
                    }}>0{i+1}</span>
                  </div>
                  <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a', marginBottom: 2 }}>{s.title}</div>
                  <div style={{ fontSize: 12.5, color: '#64748b', lineHeight: 1.4 }}>{s.subtitle}</div>
                  <div style={{
                    marginTop: 12, display: 'flex', alignItems: 'center', gap: 6,
                    fontSize: 11, color: '#94a3b8', fontWeight: 500,
                  }}>
                    <Icon.Clock size={12} sw={2}/> {s.est}
                    {s.required ? (
                      <span style={{ marginLeft: 'auto', color: '#4338ca', background: '#e0eaff', padding: '2px 7px', borderRadius: 999, fontSize: 10, fontWeight: 700 }}>обязательно</span>
                    ) : (
                      <span style={{ marginLeft: 'auto', color: '#64748b', background: '#f1f5f9', padding: '2px 7px', borderRadius: 999, fontSize: 10, fontWeight: 600 }}>опционально</span>
                    )}
                  </div>
                </li>
              );
            })}
          </ol>

          <div style={{ display: 'flex', alignItems: 'center', gap: 16, flexWrap: 'wrap' }}>
            <button onClick={onStart} style={{
              display: 'inline-flex', alignItems: 'center', gap: 10,
              background: '#4f46e5', color: '#fff', border: 0,
              padding: '14px 26px', borderRadius: 12, fontSize: 15, fontWeight: 600,
              boxShadow: '0 8px 24px rgba(79,70,229,.35)',
            }}>
              Начать настройку
              <Icon.ArrowRight size={16} sw={2.25}/>
            </button>
            <button onClick={onSkip} style={{
              background: 'transparent', border: '1px solid #e2e8f0', color: '#0f172a',
              padding: '14px 22px', borderRadius: 12, fontSize: 14.5, fontWeight: 500,
            }}>
              Перейти в дашборд
            </button>
            <div style={{ display: 'flex', alignItems: 'center', gap: 18, marginLeft: 'auto', fontSize: 13, color: '#64748b' }}>
              <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
                <Icon.Check size={14} stroke="#10b981" sw={2.5}/> Без кредитной карты
              </span>
              <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
                <Icon.Check size={14} stroke="#10b981" sw={2.5}/> 14 дней бесплатно
              </span>
              <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
                <Icon.Lock size={14} stroke="#10b981" sw={2.5}/> 152-ФЗ
              </span>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// STEP 1 — О школе
// ═══════════════════════════════════════════════════════════════════════════
const FIELDS = [
  { id: 'languages', label: 'Иностранные языки' },
  { id: 'school',    label: 'Подготовка к экзаменам' },
  { id: 'it',        label: 'IT и программирование' },
  { id: 'design',    label: 'Дизайн и творчество' },
  { id: 'business',  label: 'Бизнес и маркетинг' },
  { id: 'soft',      label: 'Soft skills' },
  { id: 'kids',      label: 'Дети и подростки' },
  { id: 'other',     label: 'Другое' },
];

const TIMEZONES = [
  { id: 'Europe/Kaliningrad', label: 'Калининград',  off: 'UTC+2' },
  { id: 'Europe/Moscow',      label: 'Москва',       off: 'UTC+3' },
  { id: 'Europe/Samara',      label: 'Самара',       off: 'UTC+4' },
  { id: 'Asia/Yekaterinburg', label: 'Екатеринбург', off: 'UTC+5' },
  { id: 'Asia/Novosibirsk',   label: 'Новосибирск',  off: 'UTC+7' },
  { id: 'Asia/Vladivostok',   label: 'Владивосток',  off: 'UTC+10' },
];

function StepSchool({ data, update }) {
  return (
    <div>
      <Field label="Название школы" icon="Building2"
        hint="Так школа будет называться в кабинете студентов, письмах и сертификатах.">
        <TextInput value={data.name} onChange={(v) => update({ name: v, subdomain: data.subdomain || slugify(v) })}
          placeholder="Например, «Школа Лингва»" autoFocus />
      </Field>

      <Field label="Адрес школы" icon="Globe"
        hint="Только латиница, цифры и дефис. Поменять адрес можно один раз в течение 14 дней.">
        <TextInput value={data.subdomain}
          onChange={(v) => update({ subdomain: v.toLowerCase().replace(/[^a-z0-9-]/g, '') })}
          placeholder="lingva-school"
          prefix="https://" suffix=".edvantix.ru" />
      </Field>

      <Field label="Направление обучения" icon="BookOpen"
        hint="Поможем подобрать шаблоны курсов и сертификатов. Можно выбрать другое позже.">
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 8 }}>
          {FIELDS.map(f => {
            const active = f.id === data.field;
            return (
              <button key={f.id} type="button" onClick={() => update({ field: f.id })}
                style={{
                  textAlign: 'left', padding: '11px 14px', borderRadius: 10,
                  border: active ? '1.5px solid #4f46e5' : '1px solid #e2e8f0',
                  background: active ? '#f5f7ff' : '#fff',
                  fontSize: 13.5, fontWeight: active ? 600 : 500,
                  color: active ? '#1e293b' : '#475569',
                  cursor: 'pointer', transition: 'all .15s',
                }}>{f.label}</button>
            );
          })}
        </div>
      </Field>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
        <Field label="Часовой пояс" icon="Clock" hint="По нему рассчитывается расписание.">
          <TimezoneSelect value={data.timezone} onChange={(v) => update({ timezone: v })}/>
        </Field>
        <Field label="Размер школы" icon="Users" hint="Подскажем тариф.">
          <SegmentedSelect value={data.size} onChange={(v) => update({ size: v })}
            options={[
              { value: 'small', label: '< 50' },
              { value: 'medium', label: '50–500' },
              { value: 'large', label: '500+' },
            ]} />
        </Field>
      </div>
    </div>
  );
}

function TimezoneSelect({ value, onChange }) {
  const [open, setOpen] = React.useState(false);
  const cur = TIMEZONES.find(t => t.id === value) || TIMEZONES[1];
  return (
    <div style={{ position: 'relative' }}>
      <button type="button" onClick={() => setOpen(o => !o)} style={{
        width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        border: '1px solid #e2e8f0', background: '#fff', borderRadius: 12,
        padding: '11px 14px', fontSize: 14, color: '#0f172a', fontFamily: 'inherit',
      }}>
        <span><strong style={{ fontWeight: 600 }}>{cur.label}</strong> <span style={{ color: '#94a3b8', marginLeft: 6 }}>{cur.off}</span></span>
        <Icon.ChevronDown size={16} stroke="#94a3b8"/>
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 6px)', left: 0, right: 0,
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 12,
          boxShadow: '0 8px 24px rgba(0,0,0,0.10)', zIndex: 10, padding: 4,
        }}>
          {TIMEZONES.map(t => {
            const active = t.id === value;
            return (
              <button key={t.id} type="button" onClick={() => { onChange(t.id); setOpen(false); }} style={{
                width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                background: active ? '#f5f7ff' : 'transparent', border: 0,
                padding: '9px 12px', borderRadius: 8, fontSize: 13.5,
                color: active ? '#4338ca' : '#0f172a', fontWeight: active ? 600 : 500,
                textAlign: 'left', cursor: 'pointer',
              }}>
                <span>{t.label}</span><span style={{ color: '#94a3b8', fontSize: 12.5 }}>{t.off}</span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

function slugify(s) {
  const map = {а:'a',б:'b',в:'v',г:'g',д:'d',е:'e',ё:'e',ж:'zh',з:'z',и:'i',й:'i',к:'k',л:'l',м:'m',н:'n',о:'o',п:'p',р:'r',с:'s',т:'t',у:'u',ф:'f',х:'h',ц:'c',ч:'ch',ш:'sh',щ:'sch',ъ:'',ы:'y',ь:'',э:'e',ю:'yu',я:'ya'};
  return s.toLowerCase().split('').map(c => map[c] !== undefined ? map[c] : c).join('')
    .replace(/[^a-z0-9-]+/g,'-').replace(/-+/g,'-').replace(/^-|-$/g,'').slice(0, 40);
}

// ═══════════════════════════════════════════════════════════════════════════
// STEP 2 — Брендинг
// ═══════════════════════════════════════════════════════════════════════════
function StepBranding({ data, schoolName, update }) {
  const fileRef = React.useRef();
  return (
    <div>
      <Field label="Логотип школы" icon="Image" optional
        hint="PNG или SVG, квадратный, минимум 256×256. Используется на сайте, в письмах и сертификатах.">
        <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
          <div style={{
            width: 84, height: 84, borderRadius: 16,
            background: `linear-gradient(135deg, ${data.color}, ${shade(data.color, -20)})`,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            color: '#fff', fontSize: 32, fontWeight: 700, letterSpacing: '-0.02em',
            boxShadow: `0 8px 24px ${data.color}45`,
          }}>
            {data.logoLetter || (schoolName ? schoolName.trim()[0]?.toUpperCase() : 'Л')}
          </div>
          <div style={{ flex: 1 }}>
            <button type="button" onClick={() => fileRef.current?.click()} style={{
              display: 'inline-flex', alignItems: 'center', gap: 8,
              background: '#fff', border: '1px dashed #cbd5e1', color: '#0f172a',
              padding: '10px 16px', borderRadius: 10, fontSize: 14, fontWeight: 500,
            }}>
              <Icon.Upload size={15} sw={2}/> Загрузить файл
            </button>
            <input ref={fileRef} type="file" accept="image/*" style={{ display: 'none' }}/>
            <div style={{ marginTop: 8, fontSize: 12.5, color: '#94a3b8' }}>
              Или используйте инициал: <input value={data.logoLetter}
                onChange={(e) => update({ logoLetter: e.target.value.slice(0,1).toUpperCase() })}
                placeholder="Л"
                style={{
                  width: 36, marginLeft: 6, border: '1px solid #e2e8f0', borderRadius: 6,
                  padding: '3px 6px', fontFamily: 'inherit', fontSize: 13, textAlign: 'center',
                }}/>
            </div>
          </div>
        </div>
      </Field>

      <Field label="Основной цвет" icon="Palette" optional
        hint="Цвет кнопок, ссылок и акцентов в кабинете. Можно подобрать под ваш бренд.">
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 10 }}>
          {PALETTE.map(p => {
            const active = p.color === data.color;
            return (
              <button key={p.id} type="button" onClick={() => update({ color: p.color })}
                title={p.label}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10,
                  padding: '8px 14px 8px 8px', borderRadius: 999,
                  border: active ? `1.5px solid ${p.color}` : '1px solid #e2e8f0',
                  background: active ? `${p.color}10` : '#fff',
                  cursor: 'pointer', transition: 'all .15s',
                  fontSize: 13, fontWeight: active ? 600 : 500,
                  color: active ? p.color : '#475569',
                }}>
                <span style={{
                  width: 22, height: 22, borderRadius: 999,
                  background: p.color,
                  boxShadow: active ? `0 0 0 3px ${p.color}33` : 'none',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                  color: '#fff',
                }}>{active && <Icon.Check size={12} sw={3}/>}</span>
                {p.label}
              </button>
            );
          })}
          {/* custom color */}
          <label style={{
            display: 'inline-flex', alignItems: 'center', gap: 8,
            padding: '8px 14px 8px 8px', borderRadius: 999,
            border: '1px dashed #cbd5e1', cursor: 'pointer',
            fontSize: 13, fontWeight: 500, color: '#475569',
          }}>
            <span style={{
              width: 22, height: 22, borderRadius: 999,
              background: 'conic-gradient(from 0deg, #ef4444, #f59e0b, #10b981, #06b6d4, #6366f1, #d946ef, #ef4444)',
            }}/>
            Свой цвет
            <input type="color" value={data.color}
              onChange={(e) => update({ color: e.target.value })}
              style={{ width: 0, height: 0, opacity: 0, position: 'absolute', pointerEvents: 'none' }}/>
          </label>
        </div>
      </Field>

      <Field label="Где будет применён бренд" icon="Sparkles"
        hint="Все эти места обновятся автоматически.">
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 8 }}>
          {[
            { icon: 'Globe', label: 'Кабинет студента' },
            { icon: 'Award', label: 'Сертификаты' },
            { icon: 'Mail',  label: 'Email‑рассылки' },
            { icon: 'CreditCard', label: 'Страница оплаты' },
          ].map((it, i) => {
            const G = Icon[it.icon];
            return (
              <div key={i} style={{
                display: 'flex', alignItems: 'center', gap: 10,
                padding: '11px 14px', borderRadius: 10,
                background: '#f8fafc', border: '1px solid #f1f5f9',
              }}>
                <G size={16} stroke={data.color} sw={2}/>
                <span style={{ fontSize: 13.5, fontWeight: 500, color: '#475569' }}>{it.label}</span>
                <Icon.Check size={14} stroke="#10b981" sw={2.5} style={{ marginLeft: 'auto' }}/>
              </div>
            );
          })}
        </div>
      </Field>
    </div>
  );
}

function shade(hex, percent) {
  // simple shade — negative darkens
  const n = parseInt(hex.slice(1), 16);
  let r = (n >> 16) & 0xff, g = (n >> 8) & 0xff, b = n & 0xff;
  const f = (percent / 100);
  r = Math.max(0, Math.min(255, Math.round(r + (f > 0 ? (255 - r) * f : r * f))));
  g = Math.max(0, Math.min(255, Math.round(g + (f > 0 ? (255 - g) * f : g * f))));
  b = Math.max(0, Math.min(255, Math.round(b + (f > 0 ? (255 - b) * f : b * f))));
  return '#' + [r,g,b].map(x => x.toString(16).padStart(2,'0')).join('');
}

// ═══════════════════════════════════════════════════════════════════════════
// STEP 3 — Команда
// ═══════════════════════════════════════════════════════════════════════════
const ROLES = [
  { id: 'teacher',    label: 'Преподаватель', desc: 'Ведёт уроки, проверяет работы' },
  { id: 'methodist',  label: 'Методист',      desc: 'Разрабатывает программы' },
  { id: 'manager',    label: 'Менеджер',      desc: 'Работает со студентами и продажами' },
  { id: 'curator',    label: 'Куратор',       desc: 'Сопровождает группы' },
];

function StepTeam({ data, update }) {
  const addInvite = () => {
    const email = data.draftEmail.trim();
    if (!email || !email.includes('@')) return;
    update({
      invites: [...data.invites, { email, role: data.draftRole, name: nameFromEmail(email) }],
      draftEmail: '',
    });
  };
  return (
    <div>
      <Field label="Кого пригласить" icon="UserPlus" optional
        hint="Введите email и нажмите Enter. Можно пригласить нескольких — каждому отправим письмо со ссылкой на вход.">
        <div style={{ display: 'flex', gap: 8 }}>
          <div style={{ flex: 1 }}>
            <TextInput value={data.draftEmail}
              onChange={(v) => update({ draftEmail: v })}
              placeholder="ivan@school.ru"
              autoFocus
              />
          </div>
          <RoleDropdown value={data.draftRole} onChange={(v) => update({ draftRole: v })}/>
          <button type="button" onClick={addInvite} style={{
            background: '#4f46e5', color: '#fff', border: 0, padding: '0 18px',
            borderRadius: 10, fontSize: 14, fontWeight: 600,
            display: 'inline-flex', alignItems: 'center', gap: 6,
          }}>
            <Icon.Plus size={15} sw={2.5}/> Добавить
          </button>
        </div>
      </Field>

      {data.invites.length > 0 && (
        <div style={{ marginBottom: 24 }}>
          <div style={{ fontSize: 12, color: '#94a3b8', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em', marginBottom: 10 }}>
            Будет приглашено · {data.invites.length}
          </div>
          <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: 6 }}>
            {data.invites.map((inv, i) => (
              <li key={i} style={{
                display: 'flex', alignItems: 'center', gap: 12,
                padding: '8px 12px 8px 8px', background: '#fff',
                border: '1px solid #e2e8f0', borderRadius: 10,
              }}>
                <Avatar name={inv.name} size={32}/>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div style={{ fontSize: 14, fontWeight: 500, color: '#0f172a' }}>{inv.email}</div>
                  <div style={{ fontSize: 12, color: '#64748b' }}>{ROLES.find(r => r.id === inv.role)?.label}</div>
                </div>
                <button type="button" onClick={() => update({ invites: data.invites.filter((_, j) => j !== i) })}
                  style={{ background: 'transparent', border: 0, color: '#94a3b8', padding: 6, borderRadius: 6 }}>
                  <Icon.Trash size={14}/>
                </button>
              </li>
            ))}
          </ul>
        </div>
      )}

      <Field label="Шаблон письма приглашения" icon="Mail">
        <div style={{
          background: '#f8fafc', border: '1px solid #f1f5f9', borderRadius: 12,
          padding: 14, fontSize: 13, color: '#475569', lineHeight: 1.6,
        }}>
          «Здравствуйте! Вас приглашают присоединиться к команде Edvantix в качестве преподавателя. Перейдите по ссылке, чтобы создать пароль и начать работу.»
          <button style={{
            background: 'transparent', border: 0, color: '#4f46e5', padding: '6px 0 0', fontSize: 13, fontWeight: 500,
          }}>Редактировать текст →</button>
        </div>
      </Field>
    </div>
  );
}

function nameFromEmail(e) {
  const local = e.split('@')[0].replace(/[._-]+/g, ' ');
  return local.split(' ').map(w => w[0]?.toUpperCase() + w.slice(1)).join(' ');
}

function RoleDropdown({ value, onChange }) {
  const [open, setOpen] = React.useState(false);
  const cur = ROLES.find(r => r.id === value);
  return (
    <div style={{ position: 'relative' }}>
      <button type="button" onClick={() => setOpen(o => !o)} style={{
        height: '100%', display: 'inline-flex', alignItems: 'center', gap: 8,
        border: '1px solid #e2e8f0', background: '#fff', borderRadius: 10,
        padding: '0 12px', fontSize: 13.5, color: '#0f172a',
      }}>
        {cur.label}<Icon.ChevronDown size={14} stroke="#94a3b8"/>
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 6px)', right: 0, minWidth: 240,
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 12,
          boxShadow: '0 8px 24px rgba(0,0,0,0.10)', zIndex: 20, padding: 4,
        }}>
          {ROLES.map(r => (
            <button key={r.id} type="button" onClick={() => { onChange(r.id); setOpen(false); }} style={{
              width: '100%', textAlign: 'left', background: r.id === value ? '#f5f7ff' : 'transparent',
              border: 0, padding: '8px 12px', borderRadius: 8, cursor: 'pointer',
            }}>
              <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>{r.label}</div>
              <div style={{ fontSize: 12, color: '#64748b' }}>{r.desc}</div>
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// STEP 4 — Студенты
// ═══════════════════════════════════════════════════════════════════════════
function StepStudents({ data, update }) {
  return (
    <div>
      <Field label="Как добавить студентов" icon="UserPlus">
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          <CardChoice
            icon="Upload"
            title="Импортировать из таблицы"
            desc="CSV или Excel, до 10 000 строк. Подскажем, как сопоставить колонки."
            badge="быстро"
            active={data.method === 'csv'}
            onClick={() => update({ method: 'csv', csvName: 'students-2026-q1.csv', csvCount: 127 })}
          />
          <CardChoice
            icon="Plus"
            title="Добавить вручную"
            desc="Подойдёт, если у вас несколько студентов и нет таблицы."
            active={data.method === 'manual'}
            onClick={() => update({ method: 'manual', manualCount: 3 })}
          />
          <CardChoice
            icon="Clock"
            title="Сделаю позже"
            desc="Пропустить этот шаг. База студентов всегда доступна в разделе «Студенты»."
            active={data.method === null}
            onClick={() => update({ method: null, csvName: '', csvCount: 0, manualCount: 0 })}
          />
        </div>
      </Field>

      {data.method === 'csv' && data.csvName && (
        <div className="ob-fade" style={{
          padding: 16, borderRadius: 12,
          background: '#f0fdf4', border: '1px solid #bbf7d0',
          display: 'flex', alignItems: 'center', gap: 12,
          marginBottom: 20,
        }}>
          <div style={{
            width: 36, height: 36, borderRadius: 10, background: '#10b981',
            display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff',
          }}>
            <Icon.FileText size={18} sw={2}/>
          </div>
          <div style={{ flex: 1 }}>
            <div style={{ fontSize: 14, fontWeight: 600, color: '#065f46' }}>{data.csvName}</div>
            <div style={{ fontSize: 12.5, color: '#047857' }}>Распознано {data.csvCount} студентов · 5 колонок сопоставлены</div>
          </div>
          <button style={{
            background: '#fff', border: '1px solid #bbf7d0', color: '#047857',
            padding: '7px 12px', borderRadius: 8, fontSize: 13, fontWeight: 500,
          }}>Проверить</button>
        </div>
      )}

      {data.method === 'manual' && (
        <div className="ob-fade" style={{ marginBottom: 20 }}>
          <Field label="Первые студенты" icon="Users"
            hint="Введите email — мы автоматически создадим профили и отправим приглашения.">
            <textarea
              defaultValue={"anna.ivanova@email.ru\npetr.smirnov@email.ru\nelena.k@gmail.com"}
              onChange={(e) => update({ manualCount: e.target.value.split(/\n/).filter(l => l.includes('@')).length })}
              style={{
                width: '100%', minHeight: 110, border: '1px solid #e2e8f0', borderRadius: 12,
                padding: '12px 14px', fontFamily: 'var(--edv-font-mono)', fontSize: 13.5,
                resize: 'vertical', outline: 'none', color: '#0f172a',
              }}/>
          </Field>
        </div>
      )}

      <div style={{
        padding: 14, background: '#eff6ff', border: '1px solid #dbeafe', borderRadius: 12,
        display: 'flex', alignItems: 'flex-start', gap: 10, fontSize: 13, color: '#1e40af', lineHeight: 1.5,
      }}>
        <Icon.Info size={16} stroke="#3b82f6" sw={2}/>
        <span>Студенты получат письмо с приглашением только после того, как вы завершите настройку.</span>
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// STEP 5 — Интеграции
// ═══════════════════════════════════════════════════════════════════════════
const INTEGRATIONS = [
  { id: 'payments', label: 'Приём платежей', icon: 'CreditCard',
    desc: 'ЮKassa, CloudPayments или Тинькофф. Без комиссии Edvantix.',
    detail: 'ЮKassa, CloudPayments, Тинькофф',
    color: '#10b981', recommended: true,
  },
  { id: 'telegram', label: 'Telegram‑бот', icon: 'Send',
    desc: 'Уведомления студентам, домашние задания и расписание в чате.',
    detail: '@your_school_bot',
    color: '#06b6d4', recommended: true,
  },
  { id: 'email', label: 'Email‑рассылки', icon: 'Mail',
    desc: 'Триггерные письма и рассылки. По умолчанию — наш SMTP.',
    detail: 'Включено по умолчанию',
    color: '#4f46e5',
  },
  { id: 'calendar', label: 'Google Calendar', icon: 'CalendarDays',
    desc: 'Уроки автоматически добавляются в календари преподавателей.',
    detail: 'OAuth подключение',
    color: '#f59e0b',
  },
  { id: 'crm', label: 'AmoCRM', icon: 'Sliders',
    desc: 'Синхронизация лидов и сделок с вашей CRM.',
    detail: 'Двусторонняя синхронизация',
    color: '#8b5cf6',
  },
];

function StepIntegrations({ data, update }) {
  const toggle = (id) => update({ enabled: { ...data.enabled, [id]: !data.enabled[id] } });
  return (
    <div>
      <Field label="Выберите, что подключить" icon="Plug" optional
        hint="Включите тумблер — мы сразу покажем как настроить. Любую интеграцию можно подключить позже.">
        <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: 10 }}>
          {INTEGRATIONS.map(it => {
            const G = Icon[it.icon];
            const on = data.enabled[it.id];
            return (
              <li key={it.id}>
                <div onClick={() => toggle(it.id)} style={{
                  display: 'flex', alignItems: 'center', gap: 14,
                  padding: 14, borderRadius: 12,
                  border: on ? `1.5px solid ${it.color}` : '1px solid #e2e8f0',
                  background: on ? `${it.color}08` : '#fff',
                  boxShadow: on ? `0 0 0 3px ${it.color}1f` : 'none',
                  cursor: 'pointer', transition: 'all .15s',
                }}>
                  <div style={{
                    flexShrink: 0,
                    width: 40, height: 40, borderRadius: 10,
                    background: on ? it.color : '#f1f5f9',
                    color: on ? '#fff' : '#64748b',
                    display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                  }}>
                    <G size={20} sw={2}/>
                  </div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                      <span style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{it.label}</span>
                      {it.recommended && (
                        <span style={{
                          fontSize: 10.5, fontWeight: 700, color: '#4338ca',
                          background: '#e0eaff', padding: '2px 7px', borderRadius: 999,
                          textTransform: 'uppercase', letterSpacing: '0.04em',
                        }}>рекомендуем</span>
                      )}
                    </div>
                    <p style={{ margin: '2px 0 0', fontSize: 12.8, color: '#64748b', lineHeight: 1.45 }}>{it.desc}</p>
                  </div>
                  <Toggle on={on} color={it.color}/>
                </div>
              </li>
            );
          })}
        </ul>
      </Field>
    </div>
  );
}

function Toggle({ on, color = '#4f46e5' }) {
  return (
    <span style={{
      flexShrink: 0,
      width: 38, height: 22, borderRadius: 999,
      background: on ? color : '#cbd5e1',
      position: 'relative', transition: 'background .2s',
    }}>
      <span style={{
        position: 'absolute', top: 2, left: on ? 18 : 2,
        width: 18, height: 18, borderRadius: 999, background: '#fff',
        transition: 'left .2s ease',
        boxShadow: '0 1px 3px rgba(0,0,0,.15)',
      }}/>
    </span>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// SUCCESS
// ═══════════════════════════════════════════════════════════════════════════
function SuccessScreen({ data, completed }) {
  const items = [
    { done: !!data.school.name, label: 'Школа создана', value: data.school.name || '—', icon: 'Building2' },
    { done: completed[1], label: 'Бренд настроен', value: data.branding.color, icon: 'Palette', swatch: data.branding.color },
    { done: data.team.invites.length > 0, label: 'Команда приглашена', value: `${data.team.invites.length} ${plural(data.team.invites.length, ['человек','человека','человек'])}`, icon: 'Users' },
    { done: !!data.students.method, label: 'Студенты добавлены', value: data.students.method === 'csv' ? `${data.students.csvCount} студентов` : data.students.method === 'manual' ? `${data.students.manualCount} студентов` : '—', icon: 'UserPlus' },
    { done: Object.values(data.integrations.enabled).filter(Boolean).length > 0, label: 'Интеграции', value: `${Object.values(data.integrations.enabled).filter(Boolean).length} подключено`, icon: 'Plug' },
  ];
  return (
    <div style={{
      minHeight: '100vh', display: 'flex', flexDirection: 'column',
      background: 'radial-gradient(circle at 50% 0%, rgba(99,102,241,0.10), transparent 50%), #fff',
    }}>
      <header style={{
        height: 72, display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        padding: '0 32px', borderBottom: '1px solid #e2e8f0',
      }}>
        <Logo />
      </header>
      <main style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '48px 32px' }}>
        <div className="ob-scale" style={{ maxWidth: 720, width: '100%', textAlign: 'center' }}>
          {/* Confetti-ish dots */}
          <div style={{ position: 'relative', height: 0 }}>
            {[
              { l: '12%', t: -10, c: '#4f46e5', s: 8 },
              { l: '20%', t: 28,  c: '#10b981', s: 6 },
              { l: '78%', t: 18,  c: '#f59e0b', s: 7 },
              { l: '88%', t: -2,  c: '#f43f5e', s: 6 },
              { l: '35%', t: -22, c: '#06b6d4', s: 5 },
              { l: '64%', t: -18, c: '#8b5cf6', s: 6 },
            ].map((d, i) => (
              <span key={i} style={{
                position: 'absolute', left: d.l, top: d.t,
                width: d.s, height: d.s, borderRadius: 999, background: d.c,
                animation: `obFloat ${3 + i * 0.3}s ease-in-out infinite ${i * 0.15}s`,
              }}/>
            ))}
          </div>
          <div style={{
            width: 80, height: 80, borderRadius: 999, margin: '0 auto 28px',
            background: 'linear-gradient(135deg, #10b981, #059669)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            boxShadow: '0 16px 40px rgba(16,185,129,.4)',
          }}>
            <Icon.Check size={42} stroke="#fff" sw={3}/>
          </div>
          <h1 style={{ margin: 0, fontSize: 44, fontWeight: 800, letterSpacing: '-0.025em', lineHeight: 1.1 }}>
            Школа <span style={{ color: '#4f46e5' }}>{data.school.name || 'готова'}</span> запущена
          </h1>
          <p style={{ margin: '16px 0 36px', fontSize: 17, color: '#475569', lineHeight: 1.55 }}>
            Можно приступать к работе. Все настройки доступны в разделе «Настройки» — изменить что‑то можно в любой момент.
          </p>

          <ul style={{
            listStyle: 'none', padding: 0, margin: '0 auto 40px', maxWidth: 560,
            display: 'flex', flexDirection: 'column', gap: 8, textAlign: 'left',
          }}>
            {items.map((it, i) => {
              const G = Icon[it.icon];
              return (
                <li key={i} style={{
                  display: 'flex', alignItems: 'center', gap: 12,
                  padding: '12px 16px', borderRadius: 12,
                  background: it.done ? '#fff' : '#f8fafc',
                  border: '1px solid #e2e8f0',
                }}>
                  <span style={{
                    width: 28, height: 28, borderRadius: 8,
                    background: it.done ? '#10b981' : '#f1f5f9',
                    color: it.done ? '#fff' : '#94a3b8',
                    display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                  }}>
                    {it.done ? <Icon.Check size={16} sw={3}/> : <G size={14}/>}
                  </span>
                  <span style={{ fontSize: 14, fontWeight: 500, color: '#0f172a' }}>{it.label}</span>
                  <span style={{ marginLeft: 'auto', display: 'inline-flex', alignItems: 'center', gap: 8, fontSize: 13, color: '#64748b' }}>
                    {it.swatch && <span style={{ width: 14, height: 14, borderRadius: 999, background: it.swatch }}/>}
                    {it.value}
                  </span>
                </li>
              );
            })}
          </ul>

          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 12, flexWrap: 'wrap' }}>
            <button style={{
              display: 'inline-flex', alignItems: 'center', gap: 10,
              background: '#4f46e5', color: '#fff', border: 0,
              padding: '14px 28px', borderRadius: 12, fontSize: 15, fontWeight: 600,
              boxShadow: '0 8px 24px rgba(79,70,229,.35)',
            }}>
              Перейти в дашборд
              <Icon.ArrowRight size={16} sw={2.25}/>
            </button>
            <button style={{
              background: '#fff', border: '1px solid #e2e8f0', color: '#0f172a',
              padding: '14px 22px', borderRadius: 12, fontSize: 14.5, fontWeight: 500,
              display: 'inline-flex', alignItems: 'center', gap: 8,
            }}>
              <Icon.PlayCircle size={16} sw={2}/> Смотреть тур (2 мин)
            </button>
          </div>
        </div>
      </main>
    </div>
  );
}

Object.assign(window, {
  WelcomeScreen, SuccessScreen,
  StepSchool, StepBranding, StepTeam, StepStudents, StepIntegrations,
  Field, TextInput, SegmentedSelect, CardChoice, Toggle, shade, slugify,
});
