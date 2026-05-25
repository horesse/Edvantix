// Main wizard app
const { useState, useMemo } = React;

const STEPS = [
  { id: 'legal', title: 'Форма собственности', hint: 'Правовой статус школы' },
  { id: 'about', title: 'Об организации', hint: 'Название, дата, тип' },
  { id: 'contact', title: 'Основной контакт', hint: 'Канал связи с нами' },
  { id: 'review', title: 'Проверка', hint: 'Подтверждение данных' },
];

const INITIAL = {
  legalForm: '',
  isLegalEntity: true,
  fullLegalName: '',
  shortName: '',
  registrationDate: '',
  organizationType: '',
  primaryContactType: 'Email',
  primaryContactValue: '',
  primaryContactDescription: '',
};

function validateStep(step, data) {
  const e = {};
  if (step === 0) {
    if (!data.legalForm) e.legalForm = 'Выберите форму собственности';
  }
  if (step === 1) {
    if (!data.fullLegalName.trim()) e.fullLegalName = 'Укажите полное наименование организации';
    else if (data.fullLegalName.trim().length < 3) e.fullLegalName = 'Минимум 3 символа';
    if (!data.registrationDate) e.registrationDate = 'Укажите дату регистрации';
    else if (new Date(data.registrationDate) > new Date()) e.registrationDate = 'Дата не может быть в будущем';
    if (!data.organizationType) e.organizationType = 'Выберите тип организации';
  }
  if (step === 2) {
    if (!data.primaryContactType) e.primaryContactType = 'Выберите канал';
    if (!data.primaryContactValue.trim()) e.primaryContactValue = 'Укажите контакт';
    else {
      if (data.primaryContactType === 'Email' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.primaryContactValue))
        e.primaryContactValue = 'Введите корректный email';
      if (['MobilePhone', 'WhatsApp', 'Viber'].includes(data.primaryContactType)
        && !/^[+\d\s()\-]{10,}$/.test(data.primaryContactValue))
        e.primaryContactValue = 'Введите номер в международном формате';
    }
  }
  return e;
}

function WizardApp() {
  const [current, setCurrent] = useState(0);
  const [data, setData] = useState(INITIAL);
  const [touched, setTouched] = useState(new Set()); // steps that have been attempted
  const [done, setDone] = useState(false);

  const completed = useMemo(() => {
    const s = new Set();
    for (let i = 0; i < current; i++) {
      if (Object.keys(validateStep(i, data)).length === 0) s.add(i);
    }
    return s;
  }, [current, data]);

  const liveErrors = touched.has(current) ? validateStep(current, data) : {};

  const update = (patch) => setData(d => ({ ...d, ...patch }));

  const next = () => {
    const errs = validateStep(current, data);
    setTouched(t => new Set(t).add(current));
    if (Object.keys(errs).length === 0) {
      if (current < STEPS.length - 1) setCurrent(current + 1);
      else setDone(true);
    }
  };
  const back = () => current > 0 && setCurrent(current - 1);
  const jumpTo = (i) => { if (i <= current || completed.has(i)) setCurrent(i); };

  if (done) {
    return (
      <DashboardFrame current={STEPS.length} completed={new Set([0, 1, 2, 3])}>
        <StepDone data={data} />
      </DashboardFrame>
    );
  }

  let content;
  if (current === 0) content = <StepLegalForm data={data} errors={liveErrors} update={update} />;
  else if (current === 1) content = <StepAbout data={data} errors={liveErrors} update={update} />;
  else if (current === 2) content = <StepContact data={data} errors={liveErrors} update={update} />;
  else content = <StepReview data={data} goTo={setCurrent} />;

  return (
    <DashboardFrame current={current} completed={completed} onJump={jumpTo}>
      <div style={{ padding: '40px 48px 24px', flex: 1, overflowY: 'auto' }}>
        <div style={{ maxWidth: 720, margin: '0 auto' }}>{content}</div>
      </div>
      <WizardFooter
        current={current}
        total={STEPS.length}
        onBack={back}
        onNext={next}
        isLast={current === STEPS.length - 1}
      />
    </DashboardFrame>
  );
}

// ── Frame: sidebar (nav) + onboarding stepper column + content ──────
function DashboardFrame({ current, completed, onJump, children }) {
  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="org" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
        {/* Topbar / breadcrumb */}
        <div style={{
          padding: '18px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Организация</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Регистрация</span>
        </div>

        <div style={{ flex: 1, display: 'flex', minHeight: 0 }}>
          {/* Stepper column */}
          <aside style={{
            width: 280, flexShrink: 0, padding: '32px 20px',
            borderRight: '1px solid #e2e8f0', background: '#fff',
            display: 'flex', flexDirection: 'column', gap: 18, overflowY: 'auto',
          }}>
            <div>
              <div style={{
                display: 'inline-flex', alignItems: 'center', gap: 6,
                padding: '4px 10px', borderRadius: 9999,
                background: 'rgba(79,70,229,0.08)', color: '#4338ca',
                fontSize: 11, fontWeight: 600, letterSpacing: '0.03em',
              }}>
                <Icon.Sparkles size={12} stroke="#4338ca" />
                НАСТРОЙКА ШКОЛЫ
              </div>
              <h1 style={{
                margin: '12px 0 4px', fontSize: 20, fontWeight: 700,
                letterSpacing: '-0.02em', color: '#0f172a',
              }}>
                Регистрация организации
              </h1>
              <p style={{
                margin: 0, fontSize: 13, color: '#64748b', lineHeight: 1.5,
              }}>
                Осталось {Math.max(0, STEPS.length - current)} {declension(STEPS.length - current, ['шаг', 'шага', 'шагов'])} до запуска школы.
              </p>
            </div>

            <Stepper steps={STEPS} current={current} completed={completed} onJump={onJump} />

            <div style={{ marginTop: 'auto', paddingTop: 16, borderTop: '1px solid #f1f5f9' }}>
              <div style={{
                display: 'flex', gap: 10, padding: 12, borderRadius: 10,
                background: '#f8fafc', border: '1px solid #e2e8f0',
              }}>
                <div style={{
                  width: 32, height: 32, borderRadius: 8, flexShrink: 0,
                  background: '#fff', border: '1px solid #e2e8f0',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  color: '#4f46e5',
                }}>
                  <Icon.Info size={16} />
                </div>
                <div style={{ flex: 1, minWidth: 0, fontSize: 12, color: '#475569', lineHeight: 1.45 }}>
                  Нужна помощь? Напишите нам — ответим в течение 15 минут.
                  <a href="#" style={{ color: '#4f46e5', fontWeight: 500, display: 'inline-block', marginTop: 4 }}>
                    support@edvantix.ru →
                  </a>
                </div>
              </div>
            </div>
          </aside>

          <main style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
            {children}
          </main>
        </div>
      </div>
    </div>
  );
}

function WizardFooter({ current, total, onBack, onNext, isLast }) {
  return (
    <div style={{
      padding: '16px 48px', borderTop: '1px solid #e2e8f0', background: '#fff',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between',
    }}>
      <div style={{ fontSize: 13, color: '#64748b' }}>
        Шаг <strong style={{ color: '#0f172a', fontWeight: 600 }}>{current + 1}</strong> из {total}
      </div>
      <div style={{ display: 'flex', gap: 10 }}>
        <Button variant="ghost" onClick={() => window.history.back()}>Отмена</Button>
        {current > 0 && (
          <Button variant="secondary" onClick={onBack}>
            <Icon.ArrowLeft size={16} />Назад
          </Button>
        )}
        <Button onClick={onNext}>
          {isLast ? 'Зарегистрировать' : 'Далее'}
          {!isLast && <Icon.ArrowRight size={16} />}
          {isLast && <Icon.Check size={16} sw={2.5} />}
        </Button>
      </div>
    </div>
  );
}

function declension(n, forms) {
  const abs = Math.abs(n);
  const mod10 = abs % 10, mod100 = abs % 100;
  if (mod10 === 1 && mod100 !== 11) return forms[0];
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return forms[1];
  return forms[2];
}

window.WizardApp = WizardApp;
