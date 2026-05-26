// AuthApp — split-screen composition + Tweaks wiring.
const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "view": "login",
  "theme": "light"
}/*EDITMODE-END*/;

function FormPanelHeader({ theme, setTheme }) {
  return (
    <div style={{
      display:'flex', justifyContent:'space-between', alignItems:'center',
      padding: '24px 48px', gap: 16,
    }}>
      {/* Mobile-only brand — hidden on desktop where right side has it */}
      <a href="#" className="af-mobile-brand" style={{
        display:'none', alignItems:'center', gap: 10,
      }}>
        <div style={{
          width: 32, height: 32, borderRadius: 8, background:'#4f46e5',
          display:'inline-flex', alignItems:'center', justifyContent:'center',
        }}>
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M22 10v6M2 10l10-5 10 5-10 5z"/><path d="M6 12v5c3 1.5 9 1.5 12 0v-5"/>
          </svg>
        </div>
        <span style={{ fontWeight: 700, fontSize: 17, color: 'var(--af-fg)', letterSpacing:'-0.01em' }}>
          Edv<span style={{ color: '#4f46e5' }}>antix</span>
        </span>
      </a>

      <a href="#" style={{
        display:'inline-flex', alignItems:'center', gap: 6,
        fontSize: 13, fontWeight: 500, color: 'var(--af-muted)',
        textDecoration:'none',
      }}
      onMouseEnter={e=>e.currentTarget.style.color='var(--af-fg)'}
      onMouseLeave={e=>e.currentTarget.style.color='var(--af-muted)'}
      >
        <IconArrowLeft size={14}/> На главную
      </a>

      <div style={{ display:'flex', alignItems:'center', gap: 14 }}>
        <button onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
          aria-label="Переключить тему"
          style={{
            width: 36, height: 36, borderRadius: 10,
            background:'var(--af-input-bg)',
            border:'1px solid var(--af-input-border)',
            color:'var(--af-muted)',
            cursor:'pointer', display:'inline-flex', alignItems:'center', justifyContent:'center',
            transition:'all .15s',
          }}
          onMouseEnter={e=>e.currentTarget.style.background='var(--af-hover-bg)'}
          onMouseLeave={e=>e.currentTarget.style.background='var(--af-input-bg)'}
        >
          {theme === 'dark' ? (
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41"/>
            </svg>
          ) : (
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
            </svg>
          )}
        </button>
      </div>
    </div>
  );
}

function FormPanelFooter() {
  return (
    <div style={{
      padding: '20px 48px 28px',
      display:'flex', justifyContent:'space-between', alignItems:'center',
      gap: 16, flexWrap:'wrap',
      fontSize: 12, color:'var(--af-muted)',
    }}>
      <div style={{ display:'flex', gap: 18, flexWrap:'wrap' }}>
        <span style={{ display:'inline-flex', alignItems:'center', gap: 6 }}>
          <IconShield size={12} stroke={2}/> Защищённое соединение
        </span>
        <span>Без кредитной карты</span>
        <span>Отмена в любой момент</span>
      </div>
      <span>© 2026 Edvantix</span>
    </div>
  );
}

function AuthApp() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);
  const view = t.view;
  const theme = t.theme;

  React.useEffect(() => {
    document.documentElement.dataset.afTheme = theme;
  }, [theme]);

  const setView = (v) => setTweak('view', v);
  const setTheme = (v) => setTweak('theme', v);

  return (
    <div className="af-shell" data-af-theme={theme}>
      {/* LEFT — Form panel */}
      <section className="af-left" data-screen-label="Auth — Form">
        <FormPanelHeader theme={theme} setTheme={setTheme}/>
        <div className="af-form-wrap">
          <div style={{ width:'100%', maxWidth: 460 }}>
            <AuthForm view={view} setView={setView}/>
          </div>
        </div>
        <FormPanelFooter/>
      </section>

      {/* RIGHT — Marketing panel */}
      <aside className="af-right" data-screen-label="Auth — Marketing">
        <AuthMarketing/>
      </aside>

      {/* Tweaks */}
      <TweaksPanel title="Tweaks">
        <TweakSection label="Экран">
          <TweakRadio
            label="Состояние формы"
            value={view}
            onChange={(v) => setTweak('view', v)}
            options={[
              { value: 'login', label: 'Вход' },
              { value: 'register', label: 'Регистрация' },
              { value: 'forgot', label: 'Восстановление' },
              { value: 'otp', label: 'OTP-код' },
            ]}
          />
        </TweakSection>
        <TweakSection label="Тема формы">
          <TweakRadio
            label="Светлая / тёмная"
            value={theme}
            onChange={(v) => setTweak('theme', v)}
            options={[
              { value: 'light', label: 'Светлая' },
              { value: 'dark', label: 'Тёмная' },
            ]}
          />
        </TweakSection>
      </TweaksPanel>
    </div>
  );
}

Object.assign(window, { AuthApp });
