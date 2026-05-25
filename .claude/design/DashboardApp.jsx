// ─── KPI card with sparkline ────────────────────────────────────────────
function KpiCard({label, value, sub, trend, trendVal, sparkColor, spark, icon, iconBg, iconColor}) {
  const Ic = Icon[icon];
  const tcolor = trend==='up' ? '#059669' : trend==='down' ? '#e11d48' : '#94a3b8';
  const TIc = trend==='up' ? Icon.TrendingUp : trend==='down' ? Icon.TrendingDown : Icon.ArrowRight;
  return (
    <Card style={{padding:20, display:'flex', flexDirection:'column', gap:14}}>
      <div style={{display:'flex', alignItems:'center', justifyContent:'space-between'}}>
        <span style={{fontSize:12.5, color:'#64748b', fontWeight:500}}>{label}</span>
        <div style={{
          width:34, height:34, borderRadius:9, background:iconBg, color:iconColor,
          display:'flex', alignItems:'center', justifyContent:'center'
        }}><Ic size={16}/></div>
      </div>
      <div style={{display:'flex', alignItems:'flex-end', justifyContent:'space-between', gap:12}}>
        <div style={{display:'flex', flexDirection:'column', gap:4, minWidth:0}}>
          <span style={{fontSize:28, fontWeight:700, fontVariantNumeric:'tabular-nums', letterSpacing:'-0.02em', lineHeight:1}}>
            {value}
          </span>
          <span style={{display:'inline-flex', alignItems:'center', gap:6, fontSize:12, color:'#94a3b8'}}>
            <span style={{
              display:'inline-flex', alignItems:'center', gap:2, fontWeight:600, color:tcolor
            }}>
              <TIc size={12}/>{trendVal}
            </span>
            {sub}
          </span>
        </div>
        <Sparkline values={spark} color={sparkColor}/>
      </div>
    </Card>
  );
}

// ─── Monthly goals card ────────────────────────────────────────────────
function MonthlyGoalsCard() {
  const goals = [
    { label:'Выручка',           current:'₽284K', target:'₽360K', pct:79, color:'#4f46e5' },
    { label:'Новые студенты',    current:'24',    target:'40',    pct:60, color:'#059669' },
    { label:'Запуск курсов',     current:'3',     target:'5',     pct:60, color:'#7c3aed' },
    { label:'Средний рейтинг',   current:'4.8',   target:'4.9',   pct:90, color:'#f59e0b' },
  ];
  return (
    <Card style={{padding:0, display:'flex', flexDirection:'column', height:'100%'}}>
      <div style={{
        display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'18px 20px 14px', borderBottom:'1px solid #f1f5f9'
      }}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Цели апреля</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>Осталось 9 дней до конца месяца</div>
        </div>
        <button style={{
          width:30, height:30, borderRadius:7, border:'1px solid #e2e8f0',
          background:'#fff', color:'#475569', cursor:'pointer',
          display:'inline-flex', alignItems:'center', justifyContent:'center'
        }}><Icon.Settings size={14}/></button>
      </div>
      <div style={{padding:'8px 20px 18px', display:'flex', flexDirection:'column', gap:4, flex:1}}>
        {goals.map((g,i) => (
          <div key={g.label} style={{
            display:'flex', alignItems:'center', gap:14, padding:'14px 0',
            borderTop: i>0 ? '1px solid #f1f5f9' : '0'
          }}>
            <ProgressRing value={g.pct} color={g.color}/>
            <div style={{flex:1, minWidth:0}}>
              <div style={{display:'flex', justifyContent:'space-between', alignItems:'baseline', gap:8}}>
                <span style={{fontSize:13.5, fontWeight:600}}>{g.label}</span>
                <span style={{fontSize:12, color:'#94a3b8', fontVariantNumeric:'tabular-nums'}}>
                  <span style={{color:'#0f172a', fontWeight:600}}>{g.current}</span>
                  {' '}/ {g.target}
                </span>
              </div>
              <div style={{display:'flex', alignItems:'center', gap:8, marginTop:6}}>
                <div style={{flex:1, height:5, background:'#f1f5f9', borderRadius:9999, overflow:'hidden'}}>
                  <div style={{width: g.pct+'%', height:'100%', background:g.color, borderRadius:9999}}/>
                </div>
                <span style={{fontSize:11.5, fontWeight:600, color:g.color, fontVariantNumeric:'tabular-nums', width:32, textAlign:'right'}}>
                  {g.pct}%
                </span>
              </div>
            </div>
          </div>
        ))}
      </div>
    </Card>
  );
}

// ─── Revenue card (wraps the chart) ────────────────────────────────────
function RevenueCard() {
  return (
    <Card style={{padding:0, display:'flex', flexDirection:'column', height:'100%'}}>
      <div style={{
        display:'flex', alignItems:'flex-start', justifyContent:'space-between',
        padding:'18px 20px 14px', borderBottom:'1px solid #f1f5f9', gap:16
      }}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Выручка</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>Последние 6 месяцев · в тыс. ₽</div>
          <div style={{display:'flex', alignItems:'baseline', gap:10, marginTop:12}}>
            <span style={{fontSize:30, fontWeight:700, letterSpacing:'-0.02em', fontVariantNumeric:'tabular-nums'}}>
              ₽1.43M
            </span>
            <span style={{display:'inline-flex', alignItems:'center', gap:3, padding:'2px 8px', borderRadius:9999, background:'#d1fae5', color:'#047857', fontSize:11.5, fontWeight:600}}>
              <Icon.TrendingUp size={11}/>+24% к прошл. полугодию
            </span>
          </div>
        </div>
        <div style={{display:'flex', flexDirection:'column', alignItems:'flex-end', gap:8}}>
          <div style={{
            display:'inline-flex', background:'#f1f5f9', padding:3, borderRadius:8,
            border:'1px solid #e2e8f0', gap:1
          }}>
            {['Неделя','Месяц','Год'].map((t,i) => (
              <button key={t} style={{
                padding:'4px 10px', borderRadius:6, border:0,
                background: i===1 ? '#fff' : 'transparent',
                color: i===1 ? '#0f172a' : '#64748b',
                fontSize:12, fontWeight: i===1 ? 600 : 500, cursor:'pointer',
                boxShadow: i===1 ? '0 1px 2px rgba(0,0,0,0.06)' : 'none'
              }}>{t}</button>
            ))}
          </div>
          <div style={{display:'flex', alignItems:'center', gap:14, fontSize:11.5}}>
            <span style={{display:'inline-flex', alignItems:'center', gap:5, color:'#334155'}}>
              <span style={{width:10, height:2.5, borderRadius:9999, background:'#4f46e5'}}/>2026
            </span>
            <span style={{display:'inline-flex', alignItems:'center', gap:5, color:'#64748b'}}>
              <span style={{width:10, height:0, borderTop:'2px dashed #cbd5e1'}}/>2025
            </span>
          </div>
        </div>
      </div>
      <div style={{padding:'10px 12px 4px', flex:1, display:'flex', alignItems:'stretch'}}>
        <RevenueChart/>
      </div>
    </Card>
  );
}

// ─── Greeting bar ──────────────────────────────────────────────────────
function GreetingBar() {
  const today = new Date('2026-04-21').toLocaleDateString('ru-RU', {weekday:'long', day:'numeric', month:'long'});
  return (
    <div style={{
      display:'flex', alignItems:'center', justifyContent:'space-between', gap:20,
      padding:'4px 0 0'
    }}>
      <div style={{display:'flex', alignItems:'center', gap:16}}>
        <Avatar name="Анна Мельникова" size={48}/>
        <div>
          <div style={{fontSize:13, color:'#64748b', textTransform:'capitalize'}}>{today}</div>
          <div style={{fontSize:22, fontWeight:700, letterSpacing:'-0.02em', marginTop:2}}>
            Доброе утро, Анна
          </div>
          <div style={{fontSize:13.5, color:'#475569', marginTop:2}}>
            Сегодня у школы 5 уроков и 12 запланированных платежей.
            <a style={{color:'#4f46e5', fontWeight:600, marginLeft:6, cursor:'pointer'}}>Посмотреть всё →</a>
          </div>
        </div>
      </div>
      <div style={{display:'flex', alignItems:'center', gap:8}}>
        <div style={{
          display:'inline-flex', background:'#fff', padding:3, borderRadius:10,
          border:'1px solid #e2e8f0', gap:1
        }}>
          {['Сегодня','Неделя','Месяц','Квартал'].map((t,i) => (
            <button key={t} style={{
              padding:'6px 12px', borderRadius:7, border:0,
              background: i===2 ? '#0f172a' : 'transparent',
              color: i===2 ? '#fff' : '#64748b',
              fontSize:12.5, fontWeight: i===2 ? 600 : 500, cursor:'pointer'
            }}>{t}</button>
          ))}
        </div>
        <Button variant="secondary" size="md">
          <Icon.Download size={15}/>Отчёт
        </Button>
        <Button variant="primary" size="md">
          <Icon.Plus size={16}/>Создать
        </Button>
      </div>
    </div>
  );
}

// ─── Page ──────────────────────────────────────────────────────────────
function DashboardPage() {
  const kpis = [
    { label:'Всего студентов', value:'248',     sub:'к месяцу',  trend:'up', trendVal:'+12',
      spark:[212,218,222,226,232,238,242,248], sparkColor:'#4f46e5',
      icon:'GraduationCap', iconBg:'#e0eaff', iconColor:'#4f46e5' },
    { label:'Активные курсы',  value:'18',      sub:'к месяцу',  trend:'up', trendVal:'+2',
      spark:[14,14,15,15,16,17,18,18],          sparkColor:'#059669',
      icon:'BookOpen',      iconBg:'#d1fae5', iconColor:'#059669' },
    { label:'Доход за месяц',  value:'₽284K',   sub:'к марту',   trend:'up', trendVal:'+8%',
      spark:[198,214,226,244,260,250,272,284],  sparkColor:'#7c3aed',
      icon:'TrendingUp',    iconBg:'#ede9fe', iconColor:'#7c3aed' },
    { label:'Посещаемость',    value:'87%',     sub:'к неделе',  trend:'up', trendVal:'+2pp',
      spark:[81,83,82,85,84,86,86,87],          sparkColor:'#f59e0b',
      icon:'BarChart2',     iconBg:'#fef3c7', iconColor:'#b45309' },
  ];

  return (
    <div style={{display:'flex', flexDirection:'column', minHeight:'100%'}}>
      {/* Topbar */}
      <div style={{
        background:'#fff', borderBottom:'1px solid #e2e8f0',
        padding:'16px 28px', display:'flex', alignItems:'center', justifyContent:'space-between', gap:20
      }}>
        <div>
          <div style={{display:'flex', alignItems:'center', gap:8, fontSize:12.5, color:'#64748b', marginBottom:4}}>
            <span>Обзор</span>
            <Icon.ChevronRight size={12}/>
            <span style={{color:'#0f172a', fontWeight:600}}>Дашборд</span>
          </div>
          <h1 style={{margin:0, fontSize:22, fontWeight:700, letterSpacing:'-0.02em'}}>
            Школа «Креатив Плюс»
          </h1>
        </div>
        <div style={{display:'flex', alignItems:'center', gap:10}}>
          <div style={{position:'relative'}}>
            <Icon.Search size={14} stroke="#94a3b8" style={{position:'absolute', left:11, top:11, pointerEvents:'none'}}/>
            <Input placeholder="Поиск студентов, курсов, групп…" style={{width:280, paddingLeft:32, height:36, fontSize:13, borderRadius:8}}/>
          </div>
          <button style={{
            width:36, height:36, borderRadius:8, border:'1px solid #e2e8f0', background:'#fff',
            color:'#475569', cursor:'pointer', position:'relative',
            display:'inline-flex', alignItems:'center', justifyContent:'center'
          }}>
            <Icon.Bell size={16}/>
            <span style={{position:'absolute', top:7, right:8, width:8, height:8, background:'#ef4444', borderRadius:9999, border:'2px solid #fff'}}/>
          </button>
        </div>
      </div>

      {/* Body */}
      <div style={{padding:28, display:'flex', flexDirection:'column', gap:18, background:'#f8fafc', flex:1, minHeight:0}}>
        <GreetingBar/>

        {/* KPI strip */}
        <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:14}}>
          {kpis.map(k => <KpiCard key={k.label} {...k}/>)}
        </div>

        {/* Revenue + goals */}
        <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:14}}>
          <RevenueCard/>
          <MonthlyGoalsCard/>
        </div>

        {/* Schedule + attendance */}
        <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:14}}>
          <ScheduleCard/>
          <AttendanceCard/>
        </div>

        {/* Top courses + new students */}
        <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:14}}>
          <TopCoursesCard/>
          <NewStudentsCard/>
        </div>

        {/* Activity feed */}
        <ActivityFeedCard/>
      </div>
    </div>
  );
}

function App() {
  return (
    <div style={{display:'flex', height:'100vh', overflow:'hidden'}}>
      <Sidebar active="dashboard"/>
      <div style={{flex:1, display:'flex', flexDirection:'column', minWidth:0, overflow:'hidden'}}>
        <div style={{flex:1, overflowY:'auto'}}>
          <DashboardPage/>
        </div>
      </div>
    </div>
  );
}

window.App = App;
