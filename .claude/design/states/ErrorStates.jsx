// Error 500 — full-page, in-layout, and toast variations.

// Decorative number "500" with broken glyph
function Broken500Mark({size=180, color='#4f46e5'}) {
  return (
    <div style={{position:'relative', display:'inline-flex', alignItems:'center', justifyContent:'center', height:size}}>
      <span style={{
        fontSize:size, fontWeight:800, letterSpacing:'-0.06em', lineHeight:1,
        background:`linear-gradient(180deg, ${color}, ${color}88)`,
        WebkitBackgroundClip:'text', WebkitTextFillColor:'transparent',
        fontVariantNumeric:'tabular-nums', userSelect:'none'
      }}>500</span>
      {/* glitch shadow layer */}
      <span style={{
        position:'absolute', inset:0, display:'flex', alignItems:'center', justifyContent:'center',
        fontSize:size, fontWeight:800, letterSpacing:'-0.06em', lineHeight:1,
        color:'rgba(239,68,68,0.18)', mixBlendMode:'multiply', userSelect:'none',
        animation:'edv-glitch 3.5s steps(1) infinite'
      }}>500</span>
    </div>
  );
}

// ─── 1. Full-page error (within app shell) ────────────────────────────────
function ErrorFullPage() {
  return (
    <AppFrame active="dashboard"
      crumbs={['Обзор','Дашборд']}
      title="Школа «Креатив Плюс»"
      action={<Button variant="secondary" disabled>Апрель 2026 <Icon.ChevronDown size={13}/></Button>}
    >
      <Card style={{
        height:'100%', display:'flex', alignItems:'center', justifyContent:'center',
        padding:40, position:'relative', overflow:'hidden'
      }}>
        {/* subtle grid pattern */}
        <div style={{
          position:'absolute', inset:0,
          backgroundImage:'linear-gradient(#f1f5f9 1px, transparent 1px), linear-gradient(90deg, #f1f5f9 1px, transparent 1px)',
          backgroundSize:'48px 48px', opacity:0.7, pointerEvents:'none',
          maskImage:'radial-gradient(circle at center, black 30%, transparent 70%)',
          WebkitMaskImage:'radial-gradient(circle at center, black 30%, transparent 70%)'
        }}/>
        <div style={{
          display:'flex', alignItems:'center', gap:48, position:'relative', zIndex:1,
          maxWidth:780
        }}>
          <Broken500Mark size={180}/>
          <div style={{display:'flex', flexDirection:'column', gap:14, maxWidth:380}}>
            <Badge variant="danger" style={{alignSelf:'flex-start', padding:'4px 12px'}} dot>
              Ошибка сервера
            </Badge>
            <h2 style={{margin:0, fontSize:28, fontWeight:700, letterSpacing:'-0.02em', lineHeight:1.15}}>
              Что-то пошло не&nbsp;так на&nbsp;нашей стороне
            </h2>
            <p style={{margin:0, fontSize:14.5, color:'#64748b', lineHeight:1.6}}>
              Мы уже знаем о&nbsp;проблеме и&nbsp;чиним её. Обычно это занимает несколько минут. Ваши данные в&nbsp;безопасности —&nbsp;ничего не&nbsp;потерялось.
            </p>
            <div style={{display:'flex', gap:8, marginTop:4}}>
              <Button variant="primary"><Icon.ArrowRight size={14}/>Попробовать снова</Button>
              <Button variant="secondary">На главную</Button>
            </div>
            <div style={{
              marginTop:12, padding:'12px 14px', borderRadius:10,
              background:'#f8fafc', border:'1px solid #e2e8f0',
              display:'flex', alignItems:'center', gap:10, fontSize:12.5, color:'#475569'
            }}>
              <Icon.AlertCircle size={14} stroke="#94a3b8"/>
              <span>Код инцидента:</span>
              <code style={{fontFamily:'ui-monospace, monospace', fontSize:12, color:'#0f172a', fontWeight:600}}>
                INC-2026-04-18-7F3A
              </code>
              <span style={{marginLeft:'auto', color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>Скопировать</span>
            </div>
            <div style={{fontSize:12, color:'#94a3b8'}}>
              Если ошибка повторяется — напишите нам в&nbsp;
              <span style={{color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>поддержку</span>
              {' '}или проверьте&nbsp;
              <span style={{color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>статус системы →</span>
            </div>
          </div>
        </div>
      </Card>
    </AppFrame>
  );
}

// ─── 2. Full-screen error (no shell) — for boot-time / fatal ──────────────
function ErrorFullScreen() {
  return (
    <div style={{
      width:1280, height:800, background:'#f8fafc',
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a',
      display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center',
      position:'relative', overflow:'hidden', padding:40
    }}>
      {/* aurora */}
      <div style={{position:'absolute', top:'-10%', left:'-10%', width:520, height:520, borderRadius:'50%', background:'radial-gradient(circle, rgba(239,68,68,0.10), transparent 70%)', filter:'blur(40px)'}}/>
      <div style={{position:'absolute', bottom:'-10%', right:'-10%', width:520, height:520, borderRadius:'50%', background:'radial-gradient(circle, rgba(79,70,229,0.08), transparent 70%)', filter:'blur(40px)'}}/>

      {/* tiny top brand */}
      <div style={{position:'absolute', top:24, left:32, display:'flex', alignItems:'center', gap:10}}>
        <div style={{
          width:28, height:28, borderRadius:7, background:'#4f46e5',
          display:'flex', alignItems:'center', justifyContent:'center'
        }}>
          <Icon.GraduationCap size={15} stroke="#fff"/>
        </div>
        <div style={{fontSize:15, fontWeight:700, letterSpacing:'-0.02em'}}>
          Edv<span style={{color:'#4f46e5'}}>antix</span>
        </div>
      </div>
      {/* status badge top right */}
      <div style={{position:'absolute', top:24, right:32, display:'flex', alignItems:'center', gap:8, fontSize:12.5, color:'#64748b'}}>
        <span style={{display:'inline-flex', alignItems:'center', gap:6}}>
          <span style={{width:8, height:8, borderRadius:9999, background:'#ef4444', animation:'edv-pulse 1.4s ease-in-out infinite'}}/>
          Сервис временно недоступен
        </span>
        <span style={{color:'#cbd5e1'}}>·</span>
        <span style={{color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>status.edvantix.ru →</span>
      </div>

      <div style={{
        position:'relative', zIndex:1,
        display:'flex', flexDirection:'column', alignItems:'center', textAlign:'center', maxWidth:600, gap:20
      }}>
        {/* concentric ripple under broken icon */}
        <div style={{position:'relative', width:120, height:120, display:'flex', alignItems:'center', justifyContent:'center', marginBottom:8}}>
          {[0,1,2].map(i=>(
            <span key={i} style={{
              position:'absolute', inset:0, borderRadius:9999, border:'2px solid #ef4444',
              animation:'edv-ripple 2.4s ease-out infinite', animationDelay:`${i*0.7}s`, opacity:0
            }}/>
          ))}
          <div style={{
            width:88, height:88, borderRadius:22, background:'#fff',
            border:'1px solid #fee2e2', boxShadow:'0 16px 40px -10px rgba(239,68,68,0.25)',
            display:'flex', alignItems:'center', justifyContent:'center', position:'relative', zIndex:1
          }}>
            <Icon.AlertCircle size={42} stroke="#ef4444"/>
          </div>
        </div>

        <div style={{fontSize:11, fontWeight:600, letterSpacing:'0.12em', textTransform:'uppercase', color:'#ef4444'}}>
          Ошибка 500 · Internal Server Error
        </div>

        <h1 style={{margin:0, fontSize:42, fontWeight:800, letterSpacing:'-0.03em', lineHeight:1.05}}>
          Мы&nbsp;уже работаем над&nbsp;этим
        </h1>

        <p style={{margin:0, fontSize:15.5, color:'#475569', lineHeight:1.6, maxWidth:480}}>
          Произошёл сбой на&nbsp;стороне сервера. Команда получила уведомление и&nbsp;разбирается. Обычно мы&nbsp;возвращаемся в&nbsp;строй меньше&nbsp;чем за&nbsp;15&nbsp;минут.
        </p>

        <div style={{display:'flex', gap:10, marginTop:4}}>
          <Button variant="primary" size="lg">
            <Icon.ArrowRight size={15}/>Перезагрузить страницу
          </Button>
          <Button variant="secondary" size="lg">
            <Icon.MessageCircle size={15}/>Написать в поддержку
          </Button>
        </div>

        <div style={{
          display:'flex', alignItems:'center', gap:14, marginTop:14, padding:'10px 16px',
          borderRadius:9999, background:'#fff', border:'1px solid #e2e8f0',
          fontSize:12, color:'#475569'
        }}>
          <span>Код инцидента</span>
          <code style={{fontFamily:'ui-monospace, monospace', fontSize:12.5, color:'#0f172a', fontWeight:600}}>
            INC-2026-04-18-7F3A
          </code>
          <span style={{color:'#cbd5e1'}}>·</span>
          <span>18 апреля, 14:47 MSK</span>
        </div>
      </div>

      {/* bottom helper links */}
      <div style={{
        position:'absolute', bottom:32, left:0, right:0,
        display:'flex', alignItems:'center', justifyContent:'center', gap:24, fontSize:12.5, color:'#64748b'
      }}>
        <a style={{cursor:'pointer'}}>Статус системы</a>
        <span style={{color:'#cbd5e1'}}>·</span>
        <a style={{cursor:'pointer'}}>База знаний</a>
        <span style={{color:'#cbd5e1'}}>·</span>
        <a style={{cursor:'pointer'}}>Telegram-канал</a>
        <span style={{color:'#cbd5e1'}}>·</span>
        <a style={{cursor:'pointer'}}>support@edvantix.ru</a>
      </div>
    </div>
  );
}

// ─── 3. Inline error within a card (recoverable widget error) ─────────────
function ErrorInlineWidget() {
  return (
    <AppFrame active="dashboard"
      crumbs={['Обзор','Дашборд']}
      title="Школа «Креатив Плюс»"
      action={<Button variant="secondary">Апрель 2026 <Icon.ChevronDown size={13}/></Button>}
    >
      {/* Top-banner error */}
      <div style={{
        display:'flex', alignItems:'center', gap:14, padding:'12px 16px',
        background:'#fef2f2', border:'1px solid #fecaca', borderRadius:12, marginBottom:16
      }}>
        <div style={{
          width:32, height:32, borderRadius:9, background:'#fee2e2', color:'#b91c1c',
          display:'flex', alignItems:'center', justifyContent:'center', flexShrink:0
        }}>
          <Icon.AlertCircle size={16}/>
        </div>
        <div style={{flex:1, minWidth:0}}>
          <div style={{fontSize:13.5, fontWeight:600, color:'#7f1d1d'}}>
            Не&nbsp;удалось загрузить часть данных
          </div>
          <div style={{fontSize:12.5, color:'#991b1b', marginTop:2}}>
            Сервис аналитики недоступен. Карточки могут показывать устаревшие значения.
          </div>
        </div>
        <Button variant="secondary" size="sm" style={{borderColor:'#fecaca', color:'#b91c1c'}}>
          <Icon.ArrowRight size={13}/>Повторить
        </Button>
        <button style={{background:'transparent', border:0, padding:6, color:'#991b1b', cursor:'pointer', borderRadius:6}}>
          <Icon.X size={14}/>
        </button>
      </div>

      <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:14, marginBottom:16}}>
        {[
          {l:'Всего студентов', v:'248', s:'+12 за месяц'},
          {l:'Активные группы', v:'18', s:'5 новых'},
          {l:'Выручка апреля', v:'₽284 000', s:'+8% vs март'},
        ].map((c,i)=>(
          <Card key={i} style={{padding:20, display:'flex', flexDirection:'column', gap:10}}>
            <div style={{fontSize:12, color:'#64748b', fontWeight:500}}>{c.l}</div>
            <div style={{fontSize:26, fontWeight:700, letterSpacing:'-0.02em'}}>{c.v}</div>
            <div style={{fontSize:12, color:'#94a3b8'}}>{c.s}</div>
          </Card>
        ))}
        {/* errored KPI */}
        <Card style={{padding:20, display:'flex', flexDirection:'column', gap:10, borderColor:'#fecaca', background:'#fffbfb'}}>
          <div style={{fontSize:12, color:'#94a3b8', fontWeight:500, display:'flex', justifyContent:'space-between'}}>
            Средний рейтинг<Icon.AlertCircle size={13} stroke="#ef4444"/>
          </div>
          <div style={{fontSize:18, fontWeight:600, color:'#94a3b8', letterSpacing:'-0.01em'}}>Нет данных</div>
          <button style={{
            alignSelf:'flex-start', background:'transparent', border:0, color:'#b91c1c',
            fontSize:12, fontWeight:600, cursor:'pointer', padding:0
          }}>Повторить →</button>
        </Card>
      </div>

      <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:16, height:'calc(100% - 220px)'}}>
        {/* Big chart with inline error */}
        <Card style={{padding:24, display:'flex', flexDirection:'column'}}>
          <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', marginBottom:14}}>
            <div>
              <div style={{fontSize:14.5, fontWeight:600}}>Динамика выручки</div>
              <div style={{fontSize:12, color:'#94a3b8', marginTop:2}}>Источник данных недоступен</div>
            </div>
          </div>
          <div style={{flex:1, display:'flex', alignItems:'center', justifyContent:'center'}}>
            <div style={{display:'flex', flexDirection:'column', alignItems:'center', textAlign:'center', gap:12, maxWidth:340}}>
              <div style={{
                width:56, height:56, borderRadius:14, background:'#fee2e2', color:'#b91c1c',
                display:'flex', alignItems:'center', justifyContent:'center', position:'relative'
              }}>
                <Icon.BarChart2 size={24}/>
                <div style={{
                  position:'absolute', right:-4, bottom:-4, width:24, height:24, borderRadius:9999,
                  background:'#ef4444', color:'#fff', border:'2px solid #fff',
                  display:'flex', alignItems:'center', justifyContent:'center'
                }}>
                  <Icon.X size={12} sw={3}/>
                </div>
              </div>
              <div style={{fontSize:15, fontWeight:600}}>Не удалось построить график</div>
              <div style={{fontSize:13, color:'#64748b', lineHeight:1.55}}>
                Сервис аналитики ответил с&nbsp;ошибкой 500. Мы автоматически повторим запрос через&nbsp;<strong>32&nbsp;секунды</strong>.
              </div>
              <div style={{display:'flex', gap:8, marginTop:4}}>
                <Button variant="primary" size="sm"><Icon.ArrowRight size={13}/>Повторить сейчас</Button>
                <Button variant="ghost" size="sm">Скрыть виджет</Button>
              </div>
            </div>
          </div>
        </Card>

        <Card style={{padding:20, display:'flex', flexDirection:'column', gap:12}}>
          <div style={{fontSize:14, fontWeight:600}}>Новые студенты</div>
          {[
            {n:'Алина Соколова', d:'2 дня назад'},
            {n:'Дмитрий Орлов', d:'3 дня назад'},
            {n:'Мария Петрова', d:'5 дней назад'},
          ].map((s,i)=>(
            <div key={i} style={{display:'flex', alignItems:'center', gap:10, padding:'6px 0'}}>
              <Avatar name={s.n} size={32}/>
              <div style={{flex:1, minWidth:0}}>
                <div style={{fontSize:13, fontWeight:600}}>{s.n}</div>
                <div style={{fontSize:11.5, color:'#94a3b8'}}>{s.d}</div>
              </div>
              <Badge variant="primary">Новый</Badge>
            </div>
          ))}
        </Card>
      </div>
    </AppFrame>
  );
}

// ─── 4. Save-action error (toast + form) ─────────────────────────────────
function ErrorToastForm() {
  return (
    <div style={{
      width:920, height:560, background:'#f8fafc', padding:32, position:'relative',
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a', overflow:'hidden'
    }}>
      {/* Faux form */}
      <Card style={{padding:0, height:'100%', overflow:'hidden'}}>
        <div style={{padding:'20px 24px', borderBottom:'1px solid #f1f5f9'}}>
          <h2 style={{margin:0, fontSize:18, fontWeight:700, letterSpacing:'-0.01em'}}>Создать студента</h2>
          <div style={{fontSize:12.5, color:'#94a3b8', marginTop:4}}>Анна Мельникова · черновик · 5 полей</div>
        </div>
        <div style={{padding:24, display:'grid', gridTemplateColumns:'1fr 1fr', gap:18}}>
          <FormField label="Имя" value="Алина"/>
          <FormField label="Фамилия" value="Соколова"/>
          <FormField label="Email" value="alina.s@gmail.com"/>
          <FormField label="Телефон" value="+7 (916) 234-56-78"/>
          <FormField label="Курс" value="Python для детей" select/>
          <FormField label="Группа" value="Группа Б-3" select/>
        </div>
        <div style={{padding:'16px 24px', borderTop:'1px solid #f1f5f9', display:'flex', justifyContent:'space-between', alignItems:'center'}}>
          <div style={{display:'inline-flex', alignItems:'center', gap:8, fontSize:12.5, color:'#b91c1c'}}>
            <Icon.AlertCircle size={14}/>
            Изменения не сохранены
          </div>
          <div style={{display:'flex', gap:8}}>
            <Button variant="ghost">Отмена</Button>
            <Button variant="primary"><Icon.ArrowRight size={13}/>Попробовать снова</Button>
          </div>
        </div>
      </Card>

      {/* Toast */}
      <div style={{
        position:'absolute', right:32, bottom:32, width:380,
        background:'#fff', border:'1px solid #fecaca', borderRadius:14,
        boxShadow:'0 20px 40px -10px rgba(15,23,42,0.18)',
        padding:'14px 16px', display:'flex', gap:12, animation:'edv-float 4s ease-in-out infinite'
      }}>
        <div style={{
          width:36, height:36, borderRadius:10, background:'#fee2e2', color:'#b91c1c',
          display:'flex', alignItems:'center', justifyContent:'center', flexShrink:0
        }}>
          <Icon.AlertCircle size={18}/>
        </div>
        <div style={{flex:1, minWidth:0, display:'flex', flexDirection:'column', gap:6}}>
          <div style={{fontSize:13.5, fontWeight:600, color:'#0f172a'}}>Не удалось сохранить</div>
          <div style={{fontSize:12.5, color:'#64748b', lineHeight:1.5}}>
            Сервер вернул ошибку 500. Мы&nbsp;сохранили черновик локально — нажмите «Попробовать снова».
          </div>
          <div style={{display:'flex', alignItems:'center', gap:14, marginTop:4}}>
            <button style={{background:'transparent', border:0, padding:0, fontSize:12, fontWeight:600, color:'#b91c1c', cursor:'pointer'}}>
              Повторить
            </button>
            <button style={{background:'transparent', border:0, padding:0, fontSize:12, fontWeight:500, color:'#64748b', cursor:'pointer'}}>
              Показать детали
            </button>
            <span style={{marginLeft:'auto', fontSize:11, color:'#cbd5e1', fontFamily:'ui-monospace, monospace'}}>
              7F3A
            </span>
          </div>
        </div>
        <button style={{
          background:'transparent', border:0, padding:4, cursor:'pointer',
          color:'#94a3b8', alignSelf:'flex-start', borderRadius:6
        }}>
          <Icon.X size={14}/>
        </button>
        {/* progress bar (auto-dismiss timer) */}
        <div style={{
          position:'absolute', left:0, right:0, bottom:0, height:3,
          background:'#fecaca', borderRadius:'0 0 14px 14px', overflow:'hidden'
        }}>
          <div style={{
            height:'100%', background:'#ef4444', width:'62%', borderRadius:'0 0 14px 14px'
          }}/>
        </div>
      </div>
    </div>
  );
}

function FormField({label, value, select}) {
  return (
    <div style={{display:'flex', flexDirection:'column', gap:6}}>
      <label style={{fontSize:12.5, fontWeight:600, color:'#475569'}}>{label}</label>
      <div style={{
        height:38, padding:'0 14px', borderRadius:10, border:'1px solid #e2e8f0',
        background:'#fff', display:'flex', alignItems:'center', fontSize:13.5,
        color: value ? '#0f172a' : '#94a3b8', justifyContent: select ? 'space-between' : 'flex-start'
      }}>
        {value}
        {select && <Icon.ChevronDown size={14} stroke="#94a3b8"/>}
      </div>
    </div>
  );
}

Object.assign(window, { ErrorFullPage, ErrorFullScreen, ErrorInlineWidget, ErrorToastForm });
