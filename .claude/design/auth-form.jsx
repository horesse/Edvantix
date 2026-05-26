// AuthForm — left side of the split-screen. Handles 4 views: login / register / forgot / otp.
const { useState, useRef, useEffect } = React;

// ── Field primitive ──────────────────────────────────────────────────
function Field({ label, hint, error, children }) {
  return (
    <label style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
      <span style={{ fontSize: 13, fontWeight: 600, color: 'var(--af-label)' }}>{label}</span>
      {children}
      {error && <span style={{ fontSize: 12, color: '#ef4444' }}>{error}</span>}
      {hint && !error && <span style={{ fontSize: 12, color: 'var(--af-muted)' }}>{hint}</span>}
    </label>
  );
}

function TextInput({ icon: Icon, type='text', rightSlot, ...p }) {
  const [f, setF] = useState(false);
  return (
    <div style={{
      display:'flex', alignItems:'center', gap: 10,
      background: 'var(--af-input-bg)',
      border: `1px solid ${f ? 'var(--af-ring)' : 'var(--af-input-border)'}`,
      boxShadow: f ? '0 0 0 3px var(--af-ring-glow)' : 'none',
      borderRadius: 12, padding: '11px 14px',
      transition: 'border-color .15s, box-shadow .15s',
    }}>
      {Icon && <Icon size={18} stroke={1.75} style={{ color: 'var(--af-muted)', flexShrink:0 }}/>}
      <input
        type={type}
        onFocus={()=>setF(true)} onBlur={()=>setF(false)}
        style={{
          flex:1, border:0, outline:'none', background:'transparent',
          fontFamily:'inherit', fontSize:14, fontWeight:500,
          color:'var(--af-fg)',
          minWidth:0,
        }}
        {...p}
      />
      {rightSlot}
    </div>
  );
}

function PasswordInput(p) {
  const [show, setShow] = useState(false);
  return (
    <TextInput
      icon={IconLock}
      type={show ? 'text' : 'password'}
      rightSlot={
        <button type="button" onClick={()=>setShow(s=>!s)}
          style={{
            border:0, background:'transparent', cursor:'pointer',
            color:'var(--af-muted)', display:'flex', padding:0,
          }}
          aria-label={show?'Скрыть пароль':'Показать пароль'}
        >
          {show ? <IconEyeOff size={18} stroke={1.75}/> : <IconEye size={18} stroke={1.75}/>}
        </button>
      }
      {...p}
    />
  );
}

function Checkbox({ checked, onChange, children }) {
  return (
    <label style={{ display:'inline-flex', alignItems:'flex-start', gap: 10, cursor:'pointer', userSelect:'none' }}>
      <span
        onClick={()=>onChange(!checked)}
        style={{
          width: 18, height: 18, borderRadius: 5, flexShrink: 0,
          background: checked ? '#4f46e5' : 'var(--af-input-bg)',
          border: `1.5px solid ${checked ? '#4f46e5' : 'var(--af-input-border)'}`,
          display:'inline-flex', alignItems:'center', justifyContent:'center',
          color:'#fff', transition:'all .15s', marginTop: 1,
        }}
      >
        {checked && <IconCheck size={12} stroke={3}/>}
      </span>
      <span style={{ fontSize: 13, color: 'var(--af-secondary)', lineHeight: 1.5 }}>{children}</span>
    </label>
  );
}

function PrimaryButton({ children, loading, ...p }) {
  return (
    <button
      type="submit"
      disabled={loading}
      style={{
        width:'100%',
        display:'inline-flex', alignItems:'center', justifyContent:'center', gap: 10,
        background:'#4f46e5', color:'#fff',
        border:0, borderRadius: 12,
        padding: '14px 20px',
        fontFamily:'inherit', fontSize: 15, fontWeight: 600, lineHeight: 1,
        cursor: loading?'wait':'pointer',
        boxShadow: '0 4px 12px -2px rgba(79,70,229,0.35), 0 2px 4px rgba(79,70,229,0.2)',
        transition: 'all .2s ease',
        opacity: loading ? 0.85 : 1,
      }}
      onMouseEnter={e=>{ if(!loading) e.currentTarget.style.background='#4338ca' }}
      onMouseLeave={e=>{ e.currentTarget.style.background='#4f46e5' }}
      {...p}
    >
      {loading ? <Spinner/> : children}
    </button>
  );
}

function Spinner() {
  return (
    <span style={{
      width:16, height:16, border:'2px solid rgba(255,255,255,0.4)',
      borderTopColor:'#fff', borderRadius:'50%', display:'inline-block',
      animation: 'af-spin 0.7s linear infinite',
    }}/>
  );
}

// ── Strength meter ───────────────────────────────────────────────────
function PasswordStrength({ value }) {
  const score = (() => {
    let s = 0;
    if (!value) return 0;
    if (value.length >= 8) s++;
    if (/[A-ZА-Я]/.test(value)) s++;
    if (/[0-9]/.test(value)) s++;
    if (/[^A-Za-zА-Яа-я0-9]/.test(value)) s++;
    return s;
  })();
  const labels = ['Слабый', 'Средний', 'Хороший', 'Надёжный'];
  const colors = ['#ef4444', '#f59e0b', '#6366f1', '#10b981'];
  const idx = Math.max(0, score - 1);
  return (
    <div style={{ display:'flex', alignItems:'center', gap: 10, marginTop: 2 }}>
      <div style={{ display:'flex', gap: 4, flex: 1 }}>
        {[0,1,2,3].map(i => (
          <div key={i} style={{
            flex:1, height: 4, borderRadius: 2,
            background: score > i ? colors[idx] : 'var(--af-input-border)',
            transition: 'background .2s',
          }}/>
        ))}
      </div>
      <span style={{ fontSize: 11, fontWeight: 600, color: score ? colors[idx] : 'var(--af-muted)', minWidth: 56, textAlign: 'right' }}>
        {score ? labels[idx] : ''}
      </span>
    </div>
  );
}

// ── OTP input ────────────────────────────────────────────────────────
function OtpInput({ value, onChange, length=6 }) {
  const refs = useRef([]);
  const cells = Array.from({length}, (_,i) => value[i] || '');
  const setCell = (i, v) => {
    const next = cells.slice();
    next[i] = v.slice(-1);
    onChange(next.join(''));
    if (v && i < length-1) refs.current[i+1]?.focus();
  };
  const onKey = (i, e) => {
    if (e.key === 'Backspace' && !cells[i] && i > 0) refs.current[i-1]?.focus();
  };
  const onPaste = (e) => {
    const t = e.clipboardData.getData('text').replace(/\D/g,'').slice(0,length);
    if (t) { e.preventDefault(); onChange(t); refs.current[Math.min(t.length, length-1)]?.focus(); }
  };
  return (
    <div style={{ display:'flex', gap: 10, justifyContent:'space-between' }} onPaste={onPaste}>
      {cells.map((c, i) => (
        <input
          key={i}
          ref={el => refs.current[i] = el}
          value={c}
          onChange={e => setCell(i, e.target.value.replace(/\D/g,''))}
          onKeyDown={e => onKey(i, e)}
          inputMode="numeric"
          maxLength={1}
          style={{
            width: 56, height: 64,
            background: 'var(--af-input-bg)',
            border: `1.5px solid ${c ? '#4f46e5' : 'var(--af-input-border)'}`,
            borderRadius: 12, textAlign: 'center',
            fontFamily: 'var(--edv-font-mono)', fontSize: 24, fontWeight: 600,
            color: 'var(--af-fg)', outline: 'none',
            transition: 'border-color .15s, box-shadow .15s',
          }}
          onFocus={e => { e.target.style.borderColor='#4f46e5'; e.target.style.boxShadow='0 0 0 3px var(--af-ring-glow)'; }}
          onBlur={e => { e.target.style.borderColor = c ? '#4f46e5' : 'var(--af-input-border)'; e.target.style.boxShadow='none'; }}
        />
      ))}
    </div>
  );
}

// ── Header eyebrow + title block ─────────────────────────────────────
function FormHeader({ eyebrow, title, subtitle, backTo }) {
  return (
    <div style={{ display:'flex', flexDirection:'column', gap: 10, marginBottom: 4 }}>
      {backTo && (
        <button type="button" onClick={backTo.onClick}
          style={{
            alignSelf:'flex-start', display:'inline-flex', alignItems:'center', gap: 6,
            background:'transparent', border:0, cursor:'pointer', padding: 0,
            color: 'var(--af-muted)', fontSize: 13, fontWeight: 500, marginBottom: 4,
          }}>
          <IconArrowLeft size={14}/> {backTo.label}
        </button>
      )}
      <span style={{
        alignSelf:'flex-start',
        display:'inline-flex', alignItems:'center', gap: 6,
        fontSize: 11, fontWeight: 600, letterSpacing: '0.08em', textTransform:'uppercase',
        color: '#4f46e5', background: 'rgba(79,70,229,0.08)',
        padding: '4px 10px', borderRadius: 999,
      }}>
        <span style={{ width: 4, height: 4, borderRadius: 99, background: 'currentColor' }}/>
        {eyebrow}
      </span>
      <h1 style={{
        margin: 0, fontSize: 32, fontWeight: 700, letterSpacing: '-0.02em',
        lineHeight: 1.15, color: 'var(--af-fg)',
      }}>{title}</h1>
      {subtitle && (
        <p style={{ margin: 0, fontSize: 15, lineHeight: 1.55, color: 'var(--af-secondary)' }}>
          {subtitle}
        </p>
      )}
    </div>
  );
}

// ── Switch link footer ───────────────────────────────────────────────
function FormSwitch({ children }) {
  return (
    <p style={{
      textAlign:'center', fontSize: 14, color: 'var(--af-secondary)',
      margin: 0, lineHeight: 1.5,
    }}>{children}</p>
  );
}

function SwitchLink({ onClick, children }) {
  return (
    <button type="button" onClick={onClick}
      style={{
        background:'transparent', border:0, padding:0, cursor:'pointer',
        color:'#4f46e5', fontWeight: 600, fontFamily:'inherit', fontSize: 'inherit',
      }}
      onMouseEnter={e=>e.currentTarget.style.textDecoration='underline'}
      onMouseLeave={e=>e.currentTarget.style.textDecoration='none'}
    >{children}</button>
  );
}

// ═══════════════════════════════════════════════════════════════════
// VIEWS
// ═══════════════════════════════════════════════════════════════════

function LoginView({ setView }) {
  const [email, setEmail] = useState('');
  const [pw, setPw] = useState('');
  const [remember, setRemember] = useState(true);
  return (
    <form onSubmit={e=>e.preventDefault()} style={{ display:'flex', flexDirection:'column', gap: 22 }}>
      <FormHeader
        eyebrow="С возвращением"
        title="Войти в Edvantix"
        subtitle="Управляйте школой, расписанием и платежами из одной панели."
      />
      <div style={{ display:'flex', flexDirection:'column', gap: 14 }}>
        <Field label="Email">
          <TextInput icon={IconMail} type="email" placeholder="anna@example.ru" value={email} onChange={e=>setEmail(e.target.value)}/>
        </Field>
        <Field label="Пароль">
          <PasswordInput placeholder="Минимум 8 символов" value={pw} onChange={e=>setPw(e.target.value)}/>
        </Field>
        <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', marginTop: 2 }}>
          <Checkbox checked={remember} onChange={setRemember}>Запомнить меня</Checkbox>
          <SwitchLink onClick={()=>setView('forgot')}>Забыли пароль?</SwitchLink>
        </div>
      </div>
      <PrimaryButton>Войти <IconArrowRight size={18}/></PrimaryButton>
      <FormSwitch>
        Ещё нет аккаунта?{' '}
        <SwitchLink onClick={()=>setView('register')}>Создать аккаунт</SwitchLink>
      </FormSwitch>
    </form>
  );
}

function RegisterView({ setView }) {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [pw, setPw] = useState('');
  const [agree, setAgree] = useState(false);
  return (
    <form onSubmit={e=>e.preventDefault()} style={{ display:'flex', flexDirection:'column', gap: 22 }}>
      <FormHeader
        eyebrow="14 дней бесплатно"
        title="Создать аккаунт"
        subtitle="Без кредитной карты. Отмена в любой момент."
      />
      <div style={{ display:'flex', flexDirection:'column', gap: 14 }}>
        <Field label="Ваше имя">
          <TextInput icon={IconUser} placeholder="Анна Соколова" value={name} onChange={e=>setName(e.target.value)}/>
        </Field>
        <Field label="Email">
          <TextInput icon={IconMail} type="email" placeholder="anna@example.ru" value={email} onChange={e=>setEmail(e.target.value)}/>
        </Field>
        <Field label="Пароль">
          <PasswordInput placeholder="Минимум 8 символов" value={pw} onChange={e=>setPw(e.target.value)}/>
          <PasswordStrength value={pw}/>
        </Field>
        <div style={{ marginTop: 2 }}>
          <Checkbox checked={agree} onChange={setAgree}>
            Я согласен с <SwitchLink onClick={()=>{}}>условиями использования</SwitchLink> и{' '}
            <SwitchLink onClick={()=>{}}>политикой обработки данных</SwitchLink>.
          </Checkbox>
        </div>
      </div>
      <PrimaryButton>Создать аккаунт <IconArrowRight size={18}/></PrimaryButton>
      <FormSwitch>
        Уже есть аккаунт?{' '}
        <SwitchLink onClick={()=>setView('login')}>Войти</SwitchLink>
      </FormSwitch>
    </form>
  );
}

function ForgotView({ setView }) {
  const [email, setEmail] = useState('');
  return (
    <form onSubmit={e=>e.preventDefault()} style={{ display:'flex', flexDirection:'column', gap: 22 }}>
      <FormHeader
        eyebrow="Восстановление"
        title="Сбросить пароль"
        subtitle="Введите email — пришлём ссылку для восстановления. Срок действия 30 минут."
        backTo={{ label: 'Назад ко входу', onClick: () => setView('login') }}
      />
      <Field label="Email">
        <TextInput icon={IconMail} type="email" placeholder="anna@example.ru" value={email} onChange={e=>setEmail(e.target.value)} autoFocus/>
      </Field>
      <PrimaryButton>Отправить ссылку <IconArrowRight size={18}/></PrimaryButton>
      <div style={{
        display:'flex', gap: 12, padding: '14px 16px', borderRadius: 12,
        background: 'var(--af-tip-bg)', border: '1px solid var(--af-tip-border)',
      }}>
        <IconShield size={18} stroke={1.75} style={{ color:'#10b981', flexShrink:0, marginTop:1 }}/>
        <p style={{ margin: 0, fontSize: 13, lineHeight: 1.55, color: 'var(--af-secondary)' }}>
          Если письмо не приходит в течение 5&nbsp;минут — проверьте папку «Спам» или напишите в&nbsp;
          <SwitchLink onClick={()=>{}}>поддержку</SwitchLink>.
        </p>
      </div>
    </form>
  );
}

function OtpView({ setView }) {
  const [code, setCode] = useState('');
  const [seconds, setSeconds] = useState(47);
  useEffect(() => {
    if (seconds <= 0) return;
    const t = setTimeout(() => setSeconds(s => s - 1), 1000);
    return () => clearTimeout(t);
  }, [seconds]);
  const mm = String(Math.floor(seconds/60)).padStart(1,'0');
  const ss = String(seconds%60).padStart(2,'0');
  return (
    <form onSubmit={e=>e.preventDefault()} style={{ display:'flex', flexDirection:'column', gap: 22 }}>
      <FormHeader
        eyebrow="Проверка"
        title="Подтвердите email"
        subtitle={<>Мы отправили 6-значный код на <b style={{ color:'var(--af-fg)' }}>anna@example.ru</b>. Введите его, чтобы продолжить.</>}
        backTo={{ label: 'Изменить email', onClick: () => setView('register') }}
      />
      <div style={{ display:'flex', flexDirection:'column', gap: 10 }}>
        <OtpInput value={code} onChange={setCode}/>
        <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', fontSize: 13 }}>
          <span style={{ color:'var(--af-muted)' }}>
            {seconds > 0
              ? <>Отправить ещё раз через <span style={{ fontFamily:'var(--edv-font-mono)', color:'var(--af-fg)' }}>{mm}:{ss}</span></>
              : 'Не получили код?'}
          </span>
          <SwitchLink onClick={()=> seconds<=0 && setSeconds(47)}>
            <span style={{ opacity: seconds>0 ? 0.5 : 1 }}>Отправить ещё раз</span>
          </SwitchLink>
        </div>
      </div>
      <PrimaryButton disabled={code.length < 6}>Подтвердить <IconArrowRight size={18}/></PrimaryButton>
    </form>
  );
}

// ═══════════════════════════════════════════════════════════════════
function AuthForm({ view, setView }) {
  const Comp = { login: LoginView, register: RegisterView, forgot: ForgotView, otp: OtpView }[view] || LoginView;
  return (
    <div key={view} className="af-fade">
      <Comp setView={setView}/>
    </div>
  );
}

Object.assign(window, { AuthForm });
