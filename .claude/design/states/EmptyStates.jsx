// Empty states — different contexts, different remedies.

// ─── Illustration: floating cards composition (for big first-run empty) ───
function EmptyIllustrationStudents() {
  return (
    <svg width="220" height="160" viewBox="0 0 220 160" fill="none">
      <defs>
        <linearGradient id="es-g1" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0" stopColor="#eef2ff"/>
          <stop offset="1" stopColor="#fff"/>
        </linearGradient>
      </defs>
      {/* soft background blob */}
      <ellipse cx="110" cy="130" rx="90" ry="10" fill="#eef2ff" opacity="0.6"/>
      {/* back card */}
      <g transform="translate(35 38) rotate(-8 55 30)">
        <rect width="110" height="60" rx="10" fill="url(#es-g1)" stroke="#e0e7ff" strokeWidth="1"/>
        <circle cx="20" cy="22" r="9" fill="#c7d2fe"/>
        <rect x="36" y="16" width="56" height="6" rx="3" fill="#c7d2fe"/>
        <rect x="36" y="28" width="40" height="4" rx="2" fill="#e0e7ff"/>
        <rect x="14" y="44" width="80" height="4" rx="2" fill="#eef2ff"/>
      </g>
      {/* front card */}
      <g transform="translate(60 56) rotate(4 55 30)">
        <rect width="110" height="60" rx="10" fill="#fff" stroke="#c7d2fe" strokeWidth="1.2"/>
        <circle cx="20" cy="22" r="9" fill="#4f46e5"/>
        <rect x="36" y="16" width="56" height="6" rx="3" fill="#cbd5e1"/>
        <rect x="36" y="28" width="40" height="4" rx="2" fill="#e2e8f0"/>
        <rect x="14" y="44" width="80" height="4" rx="2" fill="#f1f5f9"/>
      </g>
      {/* plus badge */}
      <g transform="translate(150 40)" style={{animation:'edv-float 4s ease-in-out infinite'}}>
        <circle cx="14" cy="14" r="14" fill="#4f46e5"/>
        <path d="M14 8v12M8 14h12" stroke="#fff" strokeWidth="2.5" strokeLinecap="round"/>
      </g>
    </svg>
  );
}

// ─── 1. First-time empty — no students at all ─────────────────────────────
function EmptyFirstRun() {
  return (
    <AppFrame active="students"
      crumbs={['Школа','Студенты']}
      title="Студенты"
      action={
        <div style={{display:'flex',gap:8}}>
          <Button variant="secondary"><Icon.Upload size={14}/>Импорт</Button>
          <Button variant="primary"><Icon.UserPlus size={14}/>Добавить студента</Button>
        </div>
      }
    >
      <Card style={{height:'100%', display:'flex', alignItems:'center', justifyContent:'center', padding:40}}>
        <div style={{display:'flex', flexDirection:'column', alignItems:'center', maxWidth:480, textAlign:'center', gap:20}}>
          <EmptyIllustrationStudents/>
          <div style={{display:'flex', flexDirection:'column', gap:8}}>
            <h2 style={{margin:0, fontSize:24, fontWeight:700, letterSpacing:'-0.02em'}}>
              Здесь будут ваши студенты
            </h2>
            <p style={{margin:0, fontSize:14.5, color:'#64748b', lineHeight:1.55}}>
              Добавьте первого студента вручную или импортируйте список из&nbsp;CSV/Excel —&nbsp;всё займёт меньше пяти минут.
            </p>
          </div>
          <div style={{display:'flex', gap:10}}>
            <Button variant="primary"><Icon.UserPlus size={14}/>Добавить студента</Button>
            <Button variant="secondary"><Icon.Download size={14}/>Скачать шаблон CSV</Button>
          </div>
          <div style={{
            display:'flex', alignItems:'center', gap:8, marginTop:8,
            padding:'10px 14px', borderRadius:10, background:'#f8fafc', border:'1px solid #e2e8f0'
          }}>
            <Icon.Sparkles size={14} stroke="#4f46e5"/>
            <span style={{fontSize:12.5, color:'#475569'}}>
              Подсказка: подключите интеграцию с GetCourse, чтобы переносить студентов автоматически.
            </span>
            <span style={{fontSize:12.5, color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>Настроить →</span>
          </div>
        </div>
      </Card>
    </AppFrame>
  );
}

// ─── 2. Filtered list: no results within data ─────────────────────────────
function EmptyFiltered() {
  // Show toolbar context (KPI strip + filters) but content area says "no matches".
  return (
    <AppFrame active="students"
      crumbs={['Школа','Студенты']}
      title="Студенты"
      action={<Button variant="primary"><Icon.UserPlus size={14}/>Добавить студента</Button>}
    >
      <div style={{display:'flex', flexDirection:'column', gap:16, height:'100%'}}>
        {/* compressed filter bar */}
        <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', gap:12, flexWrap:'wrap'}}>
          <div style={{display:'inline-flex', background:'#f1f5f9', padding:4, borderRadius:10, border:'1px solid #e2e8f0', gap:2}}>
            {['Все · 248','Активные · 231','Приостановлены · 10','С долгом · 7'].map((l,i)=>(
              <span key={i} style={{
                padding:'6px 12px', fontSize:13, fontWeight: i===3 ? 600 : 500,
                background: i===3 ? '#fff' : 'transparent',
                color: i===3 ? '#0f172a' : '#475569',
                borderRadius:7, boxShadow: i===3 ? '0 1px 2px rgba(0,0,0,0.06)' : 'none'
              }}>{l}</span>
            ))}
          </div>
          <div style={{display:'flex', gap:8, alignItems:'center'}}>
            <div style={{position:'relative'}}>
              <Icon.Search size={14} stroke="#94a3b8" style={{position:'absolute', left:11, top:11, pointerEvents:'none'}}/>
              <Input value="Иванов" readOnly style={{width:240, paddingLeft:32, height:36, fontSize:13, borderRadius:8}}/>
            </div>
            <FilterChip>Курс: Python для детей</FilterChip>
            <FilterChip>Статус: С долгом</FilterChip>
            <FilterChip>Группа: Группа Б-3</FilterChip>
          </div>
        </div>

        {/* empty table card */}
        <Card style={{flex:1, padding:0, display:'flex', flexDirection:'column'}}>
          {/* faux table header */}
          <div style={{
            display:'grid', gridTemplateColumns:'2fr 1.2fr 1.5fr 1fr 1fr 0.6fr',
            padding:'12px 20px', borderBottom:'1px solid #f1f5f9',
            fontSize:11.5, fontWeight:600, color:'#94a3b8', textTransform:'uppercase', letterSpacing:'0.05em'
          }}>
            <span>Студент</span><span>Статус</span><span>Курс</span><span>Прогресс</span><span>Оплата</span><span/>
          </div>

          <div style={{flex:1, display:'flex', alignItems:'center', justifyContent:'center', padding:'32px 20px'}}>
            <div style={{display:'flex', flexDirection:'column', alignItems:'center', textAlign:'center', maxWidth:380, gap:14}}>
              <div style={{
                width:56, height:56, borderRadius:16, background:'#f1f5f9',
                display:'flex', alignItems:'center', justifyContent:'center'
              }}>
                <Icon.Search size={24} stroke="#94a3b8"/>
              </div>
              <div style={{display:'flex', flexDirection:'column', gap:6}}>
                <h3 style={{margin:0, fontSize:17, fontWeight:600}}>Ничего не нашлось</h3>
                <p style={{margin:0, fontSize:13.5, color:'#64748b', lineHeight:1.55}}>
                  По запросу <strong style={{color:'#0f172a'}}>«Иванов»</strong> с применёнными фильтрами нет студентов. Попробуйте изменить условия поиска.
                </p>
              </div>
              <div style={{display:'flex', gap:8, marginTop:4}}>
                <Button variant="secondary" size="sm"><Icon.X size={13}/>Сбросить фильтры</Button>
                <Button variant="ghost" size="sm">Очистить поиск</Button>
              </div>
            </div>
          </div>
        </Card>
      </div>
    </AppFrame>
  );
}

function FilterChip({children}) {
  return (
    <span style={{
      display:'inline-flex', alignItems:'center', gap:6, height:32, padding:'0 10px 0 12px',
      borderRadius:8, border:'1px solid #c7d2fe', background:'#eef2ff', color:'#4338ca',
      fontSize:12.5, fontWeight:500
    }}>
      {children}
      <Icon.X size={12}/>
    </span>
  );
}

// ─── 3. Empty payments — finance/transactional flavour ────────────────────
function EmptyPayments() {
  return (
    <AppFrame active="settings"
      crumbs={['Школа','Финансы','Платежи']}
      title="Платежи"
      action={
        <div style={{display:'flex',gap:8}}>
          <Button variant="secondary"><Icon.Download size={14}/>Экспорт</Button>
          <Button variant="primary"><Icon.Plus size={14}/>Создать счёт</Button>
        </div>
      }
    >
      <div style={{display:'flex', flexDirection:'column', gap:16, height:'100%'}}>
        {/* stat row showing zeros */}
        <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:14}}>
          {[
            {l:'Поступления за месяц', v:'₽0', s:'нет операций', i:'TrendingUp', ig:'#eef2ff', ic:'#4f46e5'},
            {l:'Ожидает оплаты', v:'0', s:'счетов выставлено', i:'Clock', ig:'#fef3c7', ic:'#b45309'},
            {l:'Просрочено', v:'0', s:'всё в порядке', i:'CheckCircle2', ig:'#d1fae5', ic:'#059669'},
            {l:'Возвраты', v:'₽0', s:'нет операций', i:'CreditCard', ig:'#f1f5f9', ic:'#64748b'},
          ].map((c,i)=>{
            const Ic = Icon[c.i];
            return (
              <Card key={i} style={{padding:20, display:'flex', flexDirection:'column', gap:14, opacity:0.7}}>
                <div style={{display:'flex', alignItems:'center', justifyContent:'space-between'}}>
                  <span style={{fontSize:12, color:'#64748b', fontWeight:500}}>{c.l}</span>
                  <div style={{width:32, height:32, borderRadius:8, background:c.ig, color:c.ic, display:'flex', alignItems:'center', justifyContent:'center'}}>
                    <Ic size={16}/>
                  </div>
                </div>
                <div>
                  <div style={{fontSize:26, fontWeight:700, color:'#cbd5e1', letterSpacing:'-0.02em', fontVariantNumeric:'tabular-nums'}}>{c.v}</div>
                  <div style={{fontSize:12, color:'#94a3b8', marginTop:4}}>{c.s}</div>
                </div>
              </Card>
            );
          })}
        </div>

        <Card style={{flex:1, padding:0, display:'flex'}}>
          {/* split layout: left guidance, right preview */}
          <div style={{flex:1, padding:'36px 40px', display:'flex', flexDirection:'column', justifyContent:'center', gap:20, borderRight:'1px solid #f1f5f9'}}>
            <Badge variant="primary" style={{alignSelf:'flex-start', padding:'4px 12px'}}>
              <Icon.CreditCard size={12}/>Платежи
            </Badge>
            <div style={{display:'flex', flexDirection:'column', gap:8}}>
              <h2 style={{margin:0, fontSize:22, fontWeight:700, letterSpacing:'-0.02em'}}>
                Принимайте оплату от&nbsp;студентов
              </h2>
              <p style={{margin:0, fontSize:14, color:'#64748b', lineHeight:1.6}}>
                Выставляйте счета, принимайте платежи онлайн и&nbsp;следите за&nbsp;долгами. Подключите эквайринг за&nbsp;5&nbsp;минут.
              </p>
            </div>
            {/* checklist of next steps */}
            <div style={{display:'flex', flexDirection:'column', gap:10}}>
              <StepRow done label="Создать аккаунт в Edvantix"/>
              <StepRow done label="Добавить студентов"/>
              <StepRow active label="Подключить эквайринг (ЮKassa / Тинькофф)"/>
              <StepRow label="Выставить первый счёт"/>
            </div>
            <div style={{display:'flex', gap:10, marginTop:6}}>
              <Button variant="primary"><Icon.Plus size={14}/>Подключить эквайринг</Button>
              <Button variant="ghost">Создать счёт вручную</Button>
            </div>
          </div>
          {/* faux invoice preview */}
          <div style={{width:340, padding:32, background:'#f8fafc', display:'flex', alignItems:'center', justifyContent:'center'}}>
            <FauxInvoicePreview/>
          </div>
        </Card>
      </div>
    </AppFrame>
  );
}

function StepRow({label, done, active}) {
  return (
    <div style={{display:'flex', alignItems:'center', gap:12}}>
      <div style={{
        width:22, height:22, borderRadius:9999, flexShrink:0,
        display:'flex', alignItems:'center', justifyContent:'center',
        background: done ? '#4f46e5' : active ? '#fff' : '#f1f5f9',
        border: active ? '2px solid #4f46e5' : '1px solid '+(done?'#4f46e5':'#e2e8f0'),
        color: done ? '#fff' : active ? '#4f46e5' : '#94a3b8'
      }}>
        {done ? <Icon.Check size={12} sw={3}/> : <span style={{width:6, height:6, borderRadius:9999, background: active?'#4f46e5':'#cbd5e1'}}/>}
      </div>
      <span style={{
        fontSize:13.5, fontWeight: active ? 600 : 500,
        color: done ? '#94a3b8' : active ? '#0f172a' : '#475569',
        textDecoration: done ? 'line-through' : 'none'
      }}>{label}</span>
    </div>
  );
}

function FauxInvoicePreview() {
  return (
    <div style={{width:'100%', transform:'rotate(-2deg)', animation:'edv-float 5s ease-in-out infinite'}}>
      <Card style={{padding:20, boxShadow:'0 20px 50px -10px rgba(15,23,42,0.18)', border:'1px solid #e2e8f0'}}>
        <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', marginBottom:14}}>
          <div style={{fontSize:11, fontWeight:600, letterSpacing:'0.1em', color:'#94a3b8', textTransform:'uppercase'}}>Счёт №000142</div>
          <Badge variant="warning" dot>Ожидает</Badge>
        </div>
        <div style={{fontSize:11, color:'#94a3b8', marginBottom:4}}>Получатель</div>
        <div style={{fontSize:13.5, fontWeight:600, marginBottom:14}}>Школа «Креатив Плюс»</div>
        <div style={{height:1, background:'#f1f5f9', margin:'10px 0'}}/>
        <div style={{display:'flex', justifyContent:'space-between', fontSize:12.5, color:'#475569', marginBottom:6}}>
          <span>Python для детей · 8 занятий</span><span>₽12 800</span>
        </div>
        <div style={{display:'flex', justifyContent:'space-between', fontSize:12.5, color:'#475569', marginBottom:14}}>
          <span>Скидка раннего платежа</span><span>−₽1 280</span>
        </div>
        <div style={{height:1, background:'#f1f5f9', margin:'10px 0'}}/>
        <div style={{display:'flex', justifyContent:'space-between', alignItems:'baseline', marginTop:8}}>
          <span style={{fontSize:13, fontWeight:500, color:'#0f172a'}}>К оплате</span>
          <span style={{fontSize:22, fontWeight:700, letterSpacing:'-0.02em', color:'#4f46e5', fontVariantNumeric:'tabular-nums'}}>₽11 520</span>
        </div>
      </Card>
    </div>
  );
}

// ─── 4. Inline empty widgets — small card empty (KPI / list) ──────────────
function EmptyWidgetGrid() {
  return (
    <AppFrame active="dashboard"
      crumbs={['Обзор','Дашборд']}
      title="Школа «Креатив Плюс»"
      action={<Button variant="secondary">Апрель 2026 <Icon.ChevronDown size={13}/></Button>}
    >
      <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:16, height:'100%'}}>
        {/* Big chart empty */}
        <Card style={{padding:24, display:'flex', flexDirection:'column', gap:18}}>
          <div style={{display:'flex', alignItems:'center', justifyContent:'space-between'}}>
            <div>
              <div style={{fontSize:14.5, fontWeight:600}}>Динамика выручки</div>
              <div style={{fontSize:12, color:'#94a3b8', marginTop:2}}>Апрель 2026 · по дням</div>
            </div>
            <div style={{display:'inline-flex', background:'#f8fafc', padding:3, borderRadius:8, border:'1px solid #e2e8f0', gap:2}}>
              {['День','Неделя','Месяц'].map((l,i)=>(
                <span key={i} style={{
                  padding:'5px 10px', fontSize:12, fontWeight: i===2?600:500,
                  background: i===2?'#fff':'transparent', borderRadius:6,
                  color: i===2?'#0f172a':'#64748b',
                  boxShadow: i===2?'0 1px 2px rgba(0,0,0,0.05)':'none'
                }}>{l}</span>
              ))}
            </div>
          </div>
          <div style={{flex:1, position:'relative', minHeight:240}}>
            {/* ghost chart axes */}
            <svg width="100%" height="100%" viewBox="0 0 600 240" preserveAspectRatio="none">
              {[0,1,2,3,4].map(i=>(
                <line key={i} x1="40" y1={20+i*50} x2="590" y2={20+i*50} stroke="#f1f5f9" strokeDasharray="3 4"/>
              ))}
              {[0,1,2,3,4].map(i=>(
                <text key={i} x="32" y={24+i*50} textAnchor="end" fontSize="10" fill="#cbd5e1">{['₽400К','₽300К','₽200К','₽100К','₽0'][i]}</text>
              ))}
            </svg>
            <div style={{
              position:'absolute', inset:0, display:'flex', alignItems:'center', justifyContent:'center'
            }}>
              <div style={{display:'flex', flexDirection:'column', alignItems:'center', textAlign:'center', gap:10, maxWidth:280}}>
                <div style={{width:44, height:44, borderRadius:12, background:'#eef2ff', display:'flex', alignItems:'center', justifyContent:'center'}}>
                  <Icon.BarChart2 size={20} stroke="#4f46e5"/>
                </div>
                <div style={{fontSize:14.5, fontWeight:600}}>Нет данных за период</div>
                <div style={{fontSize:12.5, color:'#64748b', lineHeight:1.5}}>
                  В апреле пока не было платежей. Данные появятся, как только пройдёт первая оплата.
                </div>
              </div>
            </div>
          </div>
        </Card>

        <div style={{display:'flex', flexDirection:'column', gap:16}}>
          {/* List empty */}
          <Card style={{padding:20, flex:1, display:'flex', flexDirection:'column'}}>
            <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', marginBottom:12}}>
              <div style={{fontSize:14, fontWeight:600}}>Новые студенты</div>
              <span style={{fontSize:12, color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>Все →</span>
            </div>
            <div style={{flex:1, display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center', gap:10, textAlign:'center', padding:'14px 8px'}}>
              <div style={{
                width:38, height:38, borderRadius:10, background:'#f1f5f9',
                display:'flex', alignItems:'center', justifyContent:'center', position:'relative'
              }}>
                <Icon.UserPlus size={17} stroke="#94a3b8"/>
              </div>
              <div style={{fontSize:13, fontWeight:600}}>Пока никого</div>
              <div style={{fontSize:11.5, color:'#94a3b8', lineHeight:1.5, maxWidth:200}}>
                Здесь появятся студенты, зарегистрированные за&nbsp;последние 7&nbsp;дней.
              </div>
              <Button variant="ghost" size="sm" style={{marginTop:4, color:'#4f46e5'}}>
                <Icon.UserPlus size={12}/>Пригласить
              </Button>
            </div>
          </Card>

          {/* Notifications empty */}
          <Card style={{padding:20, flex:1, display:'flex', flexDirection:'column'}}>
            <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', marginBottom:12}}>
              <div style={{fontSize:14, fontWeight:600}}>Уведомления</div>
              <Badge variant="default">0</Badge>
            </div>
            <div style={{flex:1, display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center', gap:10, textAlign:'center', padding:'14px 8px'}}>
              <div style={{
                width:42, height:42, borderRadius:9999, background:'#d1fae5', color:'#059669',
                display:'flex', alignItems:'center', justifyContent:'center'
              }}>
                <Icon.Check size={20} sw={3}/>
              </div>
              <div style={{fontSize:13, fontWeight:600}}>Всё прочитано</div>
              <div style={{fontSize:11.5, color:'#94a3b8', lineHeight:1.5, maxWidth:200}}>
                Новые уведомления появятся здесь.
              </div>
            </div>
          </Card>
        </div>
      </div>
    </AppFrame>
  );
}

// ─── 5. No-results compact (small, dialog/drawer style) ──────────────────
function EmptyCompactSearch() {
  return (
    <div style={{
      width:520, height:340, background:'#fff', borderRadius:16, border:'1px solid #e2e8f0',
      boxShadow:'0 24px 60px -20px rgba(15,23,42,0.25)',
      display:'flex', flexDirection:'column', overflow:'hidden',
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a'
    }}>
      <div style={{padding:'14px 18px', borderBottom:'1px solid #f1f5f9', display:'flex', alignItems:'center', gap:10}}>
        <Icon.Search size={16} stroke="#94a3b8"/>
        <input placeholder="Поиск по платформе…" value="курс химии для взрослых" readOnly style={{
          flex:1, border:0, outline:0, fontSize:14, fontFamily:'inherit', color:'#0f172a', background:'transparent'
        }}/>
        <kbd style={{
          padding:'2px 6px', fontSize:11, fontFamily:'inherit', fontWeight:600,
          color:'#64748b', background:'#f1f5f9', borderRadius:4, border:'1px solid #e2e8f0'
        }}>ESC</kbd>
      </div>
      <div style={{flex:1, display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center', padding:24, gap:12, textAlign:'center'}}>
        <div style={{
          width:54, height:54, borderRadius:14, background:'#f8fafc', border:'1px solid #e2e8f0',
          display:'flex', alignItems:'center', justifyContent:'center', position:'relative'
        }}>
          <Icon.Search size={22} stroke="#94a3b8"/>
          <div style={{
            position:'absolute', right:-6, bottom:-6, width:22, height:22, borderRadius:9999,
            background:'#fff', border:'1px solid #e2e8f0',
            display:'flex', alignItems:'center', justifyContent:'center'
          }}>
            <Icon.X size={11} stroke="#94a3b8"/>
          </div>
        </div>
        <div style={{fontSize:15, fontWeight:600}}>Ничего не найдено</div>
        <div style={{fontSize:13, color:'#64748b', lineHeight:1.55, maxWidth:340}}>
          В курсах, группах, студентах и&nbsp;документации нет совпадений с&nbsp;<strong style={{color:'#0f172a'}}>«курс химии для взрослых»</strong>.
        </div>
        <div style={{display:'flex', gap:6, marginTop:6}}>
          {['Создать курс','Пригласить студента','Открыть документацию'].map((s,i)=>(
            <span key={i} style={{
              fontSize:12, padding:'5px 10px', borderRadius:9999,
              background:'#f1f5f9', color:'#475569', fontWeight:500, cursor:'pointer'
            }}>{s}</span>
          ))}
        </div>
      </div>
      <div style={{padding:'10px 18px', borderTop:'1px solid #f1f5f9', display:'flex', alignItems:'center', justifyContent:'space-between', fontSize:11, color:'#94a3b8'}}>
        <span>Подсказка: используйте ⌘K для быстрого поиска</span>
        <span>0 результатов</span>
      </div>
    </div>
  );
}

// ─── 6. Sub-content empty inside a detail view (Group with no students) ───
function EmptyDetailTab() {
  return (
    <div style={{
      width:920, height:560, background:'#f8fafc', borderRadius:0,
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a',
      display:'flex', flexDirection:'column', overflow:'hidden'
    }}>
      {/* condensed header */}
      <div style={{padding:'18px 28px 14px', borderBottom:'1px solid #e2e8f0', background:'#fff'}}>
        <div style={{display:'flex', alignItems:'center', gap:8, fontSize:12.5, color:'#94a3b8', marginBottom:8}}>
          <span>Школа</span><Icon.ChevronRight size={12} stroke="#cbd5e1"/>
          <span>Группы</span><Icon.ChevronRight size={12} stroke="#cbd5e1"/>
          <span style={{color:'#0f172a', fontWeight:600}}>Группа Б-3 · Python для детей</span>
        </div>
        <div style={{display:'flex', alignItems:'center', gap:14}}>
          <div style={{
            width:44, height:44, borderRadius:12, background:'linear-gradient(135deg,#4f46e5,#8b5cf6)',
            color:'#fff', display:'flex', alignItems:'center', justifyContent:'center', fontWeight:700
          }}>Б3</div>
          <div>
            <h1 style={{margin:0, fontSize:20, fontWeight:700, letterSpacing:'-0.02em'}}>Группа Б-3</h1>
            <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>
              Python для детей · Среда, Пятница · 18:00 · Преподаватель: М. Карелин
            </div>
          </div>
          <div style={{marginLeft:'auto', display:'flex', gap:8}}>
            <Button variant="secondary" size="sm">Расписание</Button>
            <Button variant="primary" size="sm"><Icon.UserPlus size={13}/>Добавить</Button>
          </div>
        </div>
        {/* tabs */}
        <div style={{display:'flex', gap:24, marginTop:14}}>
          {['Обзор','Студенты','Уроки','Посещаемость','Файлы'].map((t,i)=>(
            <span key={i} style={{
              fontSize:13.5, fontWeight: i===1?600:500, paddingBottom:10,
              color: i===1?'#0f172a':'#64748b',
              borderBottom: i===1?'2px solid #4f46e5':'2px solid transparent',
              marginBottom:-1, cursor:'pointer'
            }}>{t}{i===1 && ' · 0'}</span>
          ))}
        </div>
      </div>
      <div style={{flex:1, padding:24}}>
        <Card style={{height:'100%', padding:0, display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center', gap:14, position:'relative', overflow:'hidden'}}>
          {/* subtle dotted bg */}
          <div style={{
            position:'absolute', inset:0,
            backgroundImage:'radial-gradient(circle, #e2e8f0 1px, transparent 1px)',
            backgroundSize:'18px 18px', opacity:0.5, pointerEvents:'none'
          }}/>
          <div style={{position:'relative', zIndex:1, display:'flex', flexDirection:'column', alignItems:'center', gap:14, maxWidth:380, textAlign:'center'}}>
            <div style={{display:'flex', gap:-12, marginBottom:4}}>
              {/* stack of avatar placeholders */}
              {[0,1,2].map(i=>(
                <div key={i} style={{
                  width:44, height:44, borderRadius:9999,
                  background:'#fff', border:'2px dashed #cbd5e1',
                  marginLeft: i>0 ? -12 : 0, display:'flex', alignItems:'center', justifyContent:'center',
                  color:'#cbd5e1', fontSize:18
                }}>+</div>
              ))}
            </div>
            <h3 style={{margin:0, fontSize:17, fontWeight:600}}>В группе пока нет студентов</h3>
            <p style={{margin:0, fontSize:13, color:'#64748b', lineHeight:1.6}}>
              Добавьте студентов вручную из&nbsp;общего списка или пригласите по&nbsp;ссылке —&nbsp;они&nbsp;присоединятся сами.
            </p>
            <div style={{display:'flex', gap:8, marginTop:6}}>
              <Button variant="primary" size="sm"><Icon.UserPlus size={13}/>Добавить студентов</Button>
              <Button variant="secondary" size="sm"><Icon.Send size={13}/>Скопировать ссылку</Button>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}

Object.assign(window, { EmptyFirstRun, EmptyFiltered, EmptyPayments, EmptyWidgetGrid, EmptyCompactSearch, EmptyDetailTab });
