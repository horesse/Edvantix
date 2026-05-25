// AuthMarketing — right side of the split-screen. Dark surface with aurora, social proof,
// a floating mini-dashboard preview, and a stats strip.

function AuroraBg() {
  return (
    <div aria-hidden="true" style={{ position:'absolute', inset:0, overflow:'hidden', pointerEvents:'none' }}>
      {/* Dot grid */}
      <div style={{
        position:'absolute', inset:0,
        backgroundImage:'radial-gradient(circle, rgba(255,255,255,0.05) 1px, transparent 1px)',
        backgroundSize:'28px 28px',
        maskImage:'radial-gradient(ellipse 80% 60% at 50% 40%, #000 40%, transparent 100%)',
        WebkitMaskImage:'radial-gradient(ellipse 80% 60% at 50% 40%, #000 40%, transparent 100%)',
      }}/>
      {/* Aurora orbs */}
      <div style={{
        position:'absolute', top:'-15%', right:'-10%', width: 560, height: 560,
        borderRadius:'50%', filter:'blur(90px)', opacity: 0.42,
        background:'radial-gradient(circle, #6366f1 0%, transparent 60%)',
        animation: 'amk-float 12s ease-in-out infinite',
      }}/>
      <div style={{
        position:'absolute', bottom:'-20%', left:'-15%', width: 520, height: 520,
        borderRadius:'50%', filter:'blur(100px)', opacity: 0.32,
        background:'radial-gradient(circle, #8b5cf6 0%, transparent 60%)',
        animation: 'amk-float 14s ease-in-out infinite 2s',
      }}/>
      <div style={{
        position:'absolute', top:'40%', left:'30%', width: 300, height: 300,
        borderRadius:'50%', filter:'blur(80px)', opacity: 0.18,
        background:'radial-gradient(circle, #a5b4fc 0%, transparent 60%)',
      }}/>
    </div>
  );
}

function BrandLockup({ size=44 }) {
  return (
    <div style={{ display:'inline-flex', alignItems:'center', gap: 12 }}>
      <div style={{
        width: size, height: size, borderRadius: 12,
        background:'#4f46e5',
        display:'inline-flex', alignItems:'center', justifyContent:'center',
        boxShadow:'0 8px 24px -6px rgba(79,70,229,0.6), inset 0 1px 0 rgba(255,255,255,0.18)',
      }}>
        {/* Graduation cap */}
        <svg width={size*0.58} height={size*0.58} viewBox="0 0 24 24" fill="none"
             stroke="#fff" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          <path d="M22 10v6M2 10l10-5 10 5-10 5z"/>
          <path d="M6 12v5c3 1.5 9 1.5 12 0v-5"/>
        </svg>
      </div>
      <div style={{ fontSize: 22, fontWeight: 700, letterSpacing:'-0.02em', color:'#fff' }}>
        Edv<span style={{ color:'#818cf8' }}>antix</span>
      </div>
    </div>
  );
}

// Small dashboard preview card — abstract KPI snapshot.
function DashboardPeek() {
  const days = [
    { d: 'Пн', h: 0.55 }, { d: 'Вт', h: 0.72 }, { d: 'Ср', h: 0.48 },
    { d: 'Чт', h: 0.81 }, { d: 'Пт', h: 0.66 }, { d: 'Сб', h: 0.92 }, { d: 'Вс', h: 0.38 },
  ];
  return (
    <div style={{
      position:'relative',
      background: 'rgba(30, 37, 54, 0.7)',
      backdropFilter:'blur(12px)',
      WebkitBackdropFilter:'blur(12px)',
      border: '1px solid rgba(255,255,255,0.08)',
      borderRadius: 18, padding: 22,
      boxShadow: '0 20px 60px -20px rgba(0,0,0,0.6), 0 0 0 1px rgba(99,102,241,0.05)',
      width: '100%',
      maxWidth: 420,
    }}>
      {/* Header */}
      <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between', marginBottom: 18 }}>
        <div>
          <div style={{ fontSize: 11, fontWeight: 600, letterSpacing:'0.1em', textTransform:'uppercase', color:'#94a3b8' }}>Эта неделя</div>
          <div style={{ fontSize: 22, fontWeight: 700, color:'#fff', letterSpacing:'-0.01em', marginTop: 4, fontVariantNumeric:'tabular-nums' }}>
            ₽284&nbsp;000
          </div>
        </div>
        <span style={{
          display:'inline-flex', alignItems:'center', gap: 4,
          fontSize: 12, fontWeight: 600,
          color:'#10b981', background:'rgba(16,185,129,0.12)',
          padding:'4px 10px', borderRadius: 999,
        }}>
          ↑ +35%
        </span>
      </div>

      {/* Chart */}
      <div style={{ display:'flex', alignItems:'flex-end', gap: 8, height: 88, marginBottom: 14 }}>
        {days.map((d, i) => (
          <div key={i} style={{ flex: 1, display:'flex', flexDirection:'column', alignItems:'center', gap: 6 }}>
            <div style={{
              width: '100%',
              height: `${d.h * 100}%`,
              borderRadius: 4,
              background: i === 5
                ? 'linear-gradient(180deg, #a5b4fc 0%, #4f46e5 100%)'
                : 'linear-gradient(180deg, rgba(99,102,241,0.45) 0%, rgba(99,102,241,0.2) 100%)',
              border: '1px solid rgba(255,255,255,0.05)',
            }}/>
            <span style={{ fontSize: 10, color:'#64748b', fontWeight: 500 }}>{d.d}</span>
          </div>
        ))}
      </div>

      {/* Activity row */}
      <div style={{
        display:'flex', alignItems:'center', gap: 12,
        padding: '12px 14px',
        background: 'rgba(15, 17, 23, 0.55)',
        border: '1px solid rgba(255,255,255,0.06)',
        borderRadius: 12,
      }}>
        <div style={{
          width: 32, height: 32, borderRadius: 999,
          background:'linear-gradient(135deg, #ec4899, #f43f5e)',
          display:'inline-flex', alignItems:'center', justifyContent:'center',
          color:'#fff', fontWeight: 600, fontSize: 12, flexShrink: 0,
        }}>МК</div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontSize: 13, color:'#e2e8f0', fontWeight: 500 }}>Мария Кравцова</div>
          <div style={{ fontSize: 12, color:'#64748b' }}>оплатила «Английский B1» · 5 мин назад</div>
        </div>
        <span style={{ fontSize: 13, fontWeight: 600, color:'#10b981', fontVariantNumeric:'tabular-nums' }}>+₽12&nbsp;000</span>
      </div>
    </div>
  );
}

function AuthMarketing() {
  return (
    <div style={{
      position:'relative',
      height: '100%',
      background: 'linear-gradient(160deg, #0f1117 0%, #161b27 100%)',
      color:'#e2e8f0',
      display:'flex', flexDirection:'column',
      padding: '40px 56px 40px',
      overflow:'hidden',
    }}>
      <AuroraBg/>

      {/* Top: brand */}
      <div style={{ position:'relative', zIndex: 1 }}>
        <BrandLockup/>
      </div>

      {/* Middle: testimonial + product peek */}
      <div style={{
        position:'relative', zIndex: 1,
        flex: 1, display:'flex', flexDirection:'column', justifyContent:'center',
        gap: 36, maxWidth: 540,
      }}>
        {/* Eyebrow badge */}
        <span style={{
          alignSelf:'flex-start',
          display:'inline-flex', alignItems:'center', gap: 8,
          padding: '6px 12px',
          background:'rgba(255,255,255,0.04)',
          border: '1px solid rgba(255,255,255,0.08)',
          borderRadius: 999,
          fontSize: 12, fontWeight: 500, color:'#cbd5e1',
        }}>
          <span style={{
            width: 6, height: 6, borderRadius:'50%',
            background:'#10b981',
            boxShadow:'0 0 0 4px rgba(16,185,129,0.18)',
          }}/>
          Платформа №1 для онлайн-школ в России
        </span>

        {/* Headline */}
        <h2 style={{
          margin: 0,
          fontSize: 'clamp(28px, 3.2vw, 40px)',
          fontWeight: 700, lineHeight: 1.15, letterSpacing:'-0.025em',
          color:'#fff',
        }}>
          Управляйте школой<br/>
          <span style={{
            background:'linear-gradient(90deg, #a5b4fc, #818cf8)',
            WebkitBackgroundClip:'text', WebkitTextFillColor:'transparent', backgroundClip:'text',
          }}>без хаоса.</span>
        </h2>

        {/* Quote */}
        <figure style={{ margin: 0, display:'flex', flexDirection:'column', gap: 18 }}>
          <blockquote style={{
            margin: 0,
            fontSize: 17, lineHeight: 1.6,
            color:'#cbd5e1',
            borderLeft: '2px solid rgba(255,255,255,0.15)',
            paddingLeft: 18,
          }}>
            «За первую неделю мы заменили четыре сервиса. Расписание, оплаты,
            отчёты, чаты — всё в одном месте. Команда выдохнула.»
          </blockquote>
          <figcaption style={{ display:'flex', alignItems:'center', gap: 12 }}>
            <div style={{
              width: 40, height: 40, borderRadius: 999,
              background:'linear-gradient(135deg, #ec4899, #f43f5e)',
              display:'inline-flex', alignItems:'center', justifyContent:'center',
              color:'#fff', fontWeight: 600, fontSize: 14, flexShrink: 0,
              boxShadow:'0 0 0 3px rgba(255,255,255,0.06)',
            }}>АС</div>
            <div style={{ display:'flex', flexDirection:'column' }}>
              <span style={{ color:'#fff', fontWeight: 600, fontSize: 14 }}>Анна Соколова</span>
              <span style={{ color:'#94a3b8', fontSize: 13 }}>основатель «Английский с&nbsp;Анной» · 380 студентов</span>
            </div>
          </figcaption>
        </figure>

        {/* Dashboard peek — floating */}
        <div style={{ animation: 'amk-rise 8s ease-in-out infinite' }}>
          <DashboardPeek/>
        </div>
      </div>

      {/* Bottom: stats strip */}
      <div style={{
        position:'relative', zIndex: 1,
        display:'grid', gridTemplateColumns:'repeat(3, 1fr)',
        borderTop: '1px solid rgba(255,255,255,0.08)',
        paddingTop: 24,
      }}>
        {[
          { v: '500+', l: 'онлайн-школ' },
          { v: '₽1.4 млрд', l: 'обработано за 2025' },
          { v: '4.9★', l: 'средняя оценка клиентов' },
        ].map((s, i) => (
          <div key={i} style={{
            display:'flex', flexDirection:'column', gap: 4,
            paddingLeft: i === 0 ? 0 : 20,
            borderLeft: i === 0 ? 0 : '1px solid rgba(255,255,255,0.06)',
          }}>
            <span style={{
              fontSize: 22, fontWeight: 700, color:'#fff',
              letterSpacing:'-0.01em', fontVariantNumeric:'tabular-nums',
            }}>{s.v}</span>
            <span style={{ fontSize: 12, color:'#94a3b8' }}>{s.l}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

Object.assign(window, { AuthMarketing });
