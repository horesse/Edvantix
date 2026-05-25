// ── Onboarding wizard — main shell + state ─────────────────────────────────
const { useState, useMemo, useEffect } = React;

const STEPS = [
  { id: 'school',       title: 'О школе',          subtitle: 'Основная информация',  icon: 'Building2',  required: true,  est: '1 мин' },
  { id: 'branding',     title: 'Брендинг',         subtitle: 'Логотип и цвета',      icon: 'Palette',    required: false, est: '1 мин' },
  { id: 'team',         title: 'Команда',          subtitle: 'Приглашение коллег',   icon: 'Users',      required: false, est: '1 мин' },
  { id: 'students',     title: 'Студенты',         subtitle: 'Импорт базы',           icon: 'UserPlus',   required: false, est: '1 мин' },
  { id: 'integrations', title: 'Интеграции',       subtitle: 'Платежи и каналы',     icon: 'Plug',       required: false, est: '1 мин' },
];

const PALETTE = [
  { id: 'indigo',  label: 'Индиго',   color: '#4f46e5' },
  { id: 'emerald', label: 'Изумруд',  color: '#10b981' },
  { id: 'amber',   label: 'Янтарь',   color: '#f59e0b' },
  { id: 'rose',    label: 'Роза',     color: '#f43f5e' },
  { id: 'violet',  label: 'Фиолет',   color: '#8b5cf6' },
  { id: 'cyan',    label: 'Циан',     color: '#06b6d4' },
];

const INITIAL = {
  school: {
    name: '',
    subdomain: '',
    field: 'languages',
    timezone: 'Europe/Moscow',
    size: 'small',
  },
  branding: {
    logoLetter: '',
    color: '#4f46e5',
    accent: 'gradient',
  },
  team: {
    invites: [],
    draftEmail: '',
    draftRole: 'teacher',
  },
  students: {
    method: null,           // 'csv' | 'manual' | null (skipped)
    csvName: '',
    csvCount: 0,
    manualCount: 0,
  },
  integrations: {
    enabled: { telegram: false, email: true, payments: false, calendar: false, crm: false },
  },
};

// ───────────────────────────────────────────────────────────────────────────
function Onboarding() {
  // -1 = welcome screen, 0..4 = steps, 5 = success
  const [idx, setIdx] = useState(-1);
  const [data, setData] = useState(INITIAL);
  const [completed, setCompleted] = useState({}); // {0:true, 1:true, ...}

  const update = (key, patch) => setData(d => ({ ...d, [key]: { ...d[key], ...patch } }));

  // Welcome / Success screens
  if (idx === -1) {
    return <WelcomeScreen onStart={() => setIdx(0)} onSkip={() => setIdx(5)} />;
  }
  if (idx === 5) {
    return <SuccessScreen data={data} completed={completed} />;
  }

  const current = STEPS[idx];
  const isValid = idx === 0 ? (data.school.name.trim().length > 1 && data.school.subdomain.trim().length > 1) : true;

  const onNext = () => {
    setCompleted(c => ({ ...c, [idx]: true }));
    setIdx(i => i + 1);
  };
  const onSkip = () => setIdx(i => i + 1);
  const onBack = () => setIdx(i => Math.max(-1, i - 1));

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', background: '#fff' }}>
      <Header idx={idx} completed={completed} onJump={(i) => i <= idx && setIdx(i)} onExit={() => alert('Сохранено как черновик. Вернёмся к этому позже.')} />

      {/* Body — split: form / preview */}
      <div style={{ flex: 1, display: 'grid', gridTemplateColumns: 'minmax(0, 1fr) minmax(0, 0.82fr)', minHeight: 0 }}>
        {/* Left: form */}
        <div className="ob-scroll" style={{ overflowY: 'auto', borderRight: '1px solid #e2e8f0' }}>
          <div style={{ maxWidth: 540, margin: '0 auto', padding: '56px 48px 48px' }}>
            <div key={idx} className="ob-fade">
              <StepHeader step={current} idx={idx} />
              {idx === 0 && <StepSchool data={data.school} update={(p) => update('school', p)} />}
              {idx === 1 && <StepBranding data={data.branding} schoolName={data.school.name} update={(p) => update('branding', p)} />}
              {idx === 2 && <StepTeam data={data.team} update={(p) => update('team', p)} />}
              {idx === 3 && <StepStudents data={data.students} update={(p) => update('students', p)} />}
              {idx === 4 && <StepIntegrations data={data.integrations} update={(p) => update('integrations', p)} />}
            </div>
          </div>
        </div>

        {/* Right: live preview */}
        <div style={{ background: '#f8fafc', position: 'relative', overflow: 'hidden' }}>
          <div className="ob-scroll" style={{ position: 'absolute', inset: 0, overflowY: 'auto', padding: '56px 48px 48px' }}>
            <div key={idx} className="ob-fade" style={{ maxWidth: 560, margin: '0 auto' }}>
              <PreviewLabel idx={idx} />
              {idx === 0 && <PreviewSchool data={data} />}
              {idx === 1 && <PreviewBranding data={data} />}
              {idx === 2 && <PreviewTeam data={data} />}
              {idx === 3 && <PreviewStudents data={data} />}
              {idx === 4 && <PreviewIntegrations data={data} />}
            </div>
          </div>
        </div>
      </div>

      <Footer
        idx={idx}
        canContinue={isValid}
        skippable={!current.required}
        onBack={onBack}
        onSkip={onSkip}
        onNext={onNext}
        isLast={idx === STEPS.length - 1}
      />
    </div>
  );
}

// ── Header (logo + stepper + exit) ─────────────────────────────────────────
function Header({ idx, completed, onJump, onExit }) {
  return (
    <header style={{
      height: 72, display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      padding: '0 32px', borderBottom: '1px solid #e2e8f0', background: '#fff', flexShrink: 0,
    }}>
      <Logo />
      <Stepper idx={idx} completed={completed} onJump={onJump} />
      <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <span style={{ fontSize: 13, color: '#64748b' }}>Сохранено автоматически</span>
        <span style={{ width: 6, height: 6, borderRadius: 999, background: '#10b981', display: 'inline-block' }} />
        <button onClick={onExit} style={{
          background: 'transparent', border: '1px solid #e2e8f0', color: '#475569',
          padding: '7px 14px', borderRadius: 8, fontSize: 13, fontWeight: 500, marginLeft: 12,
        }}>Выйти</button>
      </div>
    </header>
  );
}

function Logo() {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, background: '#4f46e5',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        boxShadow: '0 2px 6px rgba(79,70,229,.35)',
      }}>
        <Icon.GraduationCap size={18} stroke="#fff" sw={2.25} />
      </div>
      <div style={{ fontWeight: 700, fontSize: 17, letterSpacing: '-0.01em' }}>
        Edv<span style={{ color: '#4f46e5' }}>antix</span>
      </div>
    </div>
  );
}

function Stepper({ idx, completed, onJump }) {
  return (
    <ol style={{ display: 'flex', alignItems: 'center', gap: 0, padding: 0, margin: 0, listStyle: 'none' }}>
      {STEPS.map((s, i) => {
        const done = !!completed[i];
        const active = i === idx;
        const reached = i <= idx;
        const ahead = i > idx;
        const Glyph = Icon[s.icon];
        return (
          <li key={s.id} style={{ display: 'flex', alignItems: 'center' }}>
            <button
              onClick={() => onJump(i)}
              disabled={ahead}
              style={{
                display: 'flex', alignItems: 'center', gap: 10,
                background: 'transparent', border: 0, padding: '6px 10px', borderRadius: 8,
                cursor: ahead ? 'default' : 'pointer',
                opacity: ahead ? 0.5 : 1,
              }}
              title={s.title}
            >
              <span style={{
                width: 28, height: 28, borderRadius: 999,
                display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                background: done ? '#4f46e5' : (active ? '#4f46e5' : '#f1f5f9'),
                color: done || active ? '#fff' : '#94a3b8',
                border: active && !done ? '2px solid #4f46e5' : '0',
                boxShadow: active ? '0 0 0 4px rgba(79,70,229,0.12)' : 'none',
                fontSize: 12, fontWeight: 700,
                transition: 'all .2s ease',
              }}>
                {done ? <Icon.Check size={14} sw={3} /> : <Glyph size={14} sw={2.25} />}
              </span>
              <span style={{
                fontSize: 13, fontWeight: active ? 600 : 500,
                color: active ? '#0f172a' : (done ? '#475569' : '#94a3b8'),
                whiteSpace: 'nowrap',
              }}>{s.title}</span>
            </button>
            {i < STEPS.length - 1 && (
              <span style={{
                width: 28, height: 2, background: i < idx ? '#4f46e5' : '#e2e8f0',
                margin: '0 4px', borderRadius: 2, transition: 'background .25s',
              }}/>
            )}
          </li>
        );
      })}
    </ol>
  );
}

// ── Step Header inside form ────────────────────────────────────────────────
function StepHeader({ step, idx }) {
  const Glyph = Icon[step.icon];
  return (
    <div style={{ marginBottom: 32 }}>
      <div style={{
        display: 'inline-flex', alignItems: 'center', gap: 8,
        background: '#eef2ff', border: '1px solid #e0e7ff', color: '#4338ca',
        padding: '4px 12px', borderRadius: 999, fontSize: 12, fontWeight: 600,
        marginBottom: 16,
      }}>
        <Glyph size={13} sw={2.25} />
        <span>Шаг {idx + 1} из {STEPS.length} · {step.title}</span>
        {!step.required && (
          <span style={{ color: '#94a3b8', fontWeight: 500, marginLeft: 2 }}>· необязательно</span>
        )}
      </div>
      <h1 style={{
        margin: 0, fontSize: 32, lineHeight: 1.15, fontWeight: 700, letterSpacing: '-0.02em',
      }}>{STEP_HEADINGS[idx].title}</h1>
      <p style={{ margin: '12px 0 0', color: '#475569', fontSize: 16, lineHeight: 1.55 }}>
        {STEP_HEADINGS[idx].subtitle}
      </p>
    </div>
  );
}

const STEP_HEADINGS = [
  { title: 'Расскажите о вашей школе',
    subtitle: 'Это базовая информация — её увидят студенты при первом входе. Можно поменять в любой момент.' },
  { title: 'Сделайте платформу своей',
    subtitle: 'Логотип и цвет применятся к личному кабинету, письмам и сертификатам.' },
  { title: 'Соберите команду',
    subtitle: 'Пригласите преподавателей, методистов и кураторов. Они получат письмо со ссылкой на вход.' },
  { title: 'Добавьте первых студентов',
    subtitle: 'Импортируйте список из таблицы или добавьте вручную. Студенты получат приглашение по email.' },
  { title: 'Подключите сервисы',
    subtitle: 'Платежи, уведомления и CRM — настройте только то, что нужно сейчас.' },
];

// ── Footer (Back / Skip / Next) ────────────────────────────────────────────
function Footer({ idx, canContinue, skippable, onBack, onSkip, onNext, isLast }) {
  const progress = ((idx + 1) / STEPS.length) * 100;
  return (
    <footer style={{
      borderTop: '1px solid #e2e8f0', background: '#fff', flexShrink: 0, position: 'relative',
    }}>
      {/* progress bar */}
      <div style={{ height: 3, background: '#f1f5f9' }}>
        <div style={{
          height: '100%', width: `${progress}%`, background: 'linear-gradient(90deg, #6366f1, #4f46e5)',
          transition: 'width .35s ease-out',
        }}/>
      </div>
      <div style={{
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        padding: '18px 32px',
      }}>
        <button onClick={onBack} style={{
          display: 'inline-flex', alignItems: 'center', gap: 8,
          background: 'transparent', border: 0, color: '#475569', padding: '10px 12px',
          borderRadius: 8, fontSize: 14, fontWeight: 500,
        }}>
          <Icon.ArrowLeft size={16}/> Назад
        </button>
        <div style={{ fontSize: 13, color: '#94a3b8' }}>
          Шаг <span style={{ color: '#0f172a', fontWeight: 600 }}>{idx + 1}</span> из {STEPS.length} · осталось {STEPS.length - idx - 1} {plural(STEPS.length - idx - 1, ['шаг','шага','шагов'])}
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          {skippable && (
            <button onClick={onSkip} style={{
              background: 'transparent', border: 0, color: '#64748b',
              padding: '10px 16px', borderRadius: 8, fontSize: 14, fontWeight: 500,
            }}>Пропустить</button>
          )}
          <button
            onClick={onNext}
            disabled={!canContinue}
            style={{
              display: 'inline-flex', alignItems: 'center', gap: 8,
              background: canContinue ? '#4f46e5' : '#c7d6fe',
              color: '#fff', border: 0, padding: '12px 22px', borderRadius: 10,
              fontSize: 14, fontWeight: 600, cursor: canContinue ? 'pointer' : 'not-allowed',
              boxShadow: canContinue ? '0 4px 12px rgba(79,70,229,.32)' : 'none',
              transition: 'all .15s',
            }}
          >
            {isLast ? 'Завершить настройку' : 'Продолжить'}
            <Icon.ArrowRight size={16} sw={2.25}/>
          </button>
        </div>
      </div>
    </footer>
  );
}

function plural(n, [one, few, many]) {
  const mod10 = n % 10, mod100 = n % 100;
  if (mod10 === 1 && mod100 !== 11) return one;
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return few;
  return many;
}

window.Onboarding = Onboarding;
window.STEPS = STEPS;
window.PALETTE = PALETTE;
window.plural = plural;
