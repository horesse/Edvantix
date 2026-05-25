// ─── Today's schedule ──────────────────────────────────────────────────
function ScheduleCard() {
  const items = [
    { time:'09:00', end:'10:30', title:'Дизайн 3D — группа А',
      teacher:'Ольга Иванова', room:'Zoom · онлайн', students:12, status:'done' },
    { time:'10:00', end:'11:30', title:'UX/UI основы — поток апр.',
      teacher:'Анна Сергеева', room:'Zoom · онлайн', students:18, status:'live', progress:64 },
    { time:'12:30', end:'14:00', title:'Веб-разработка — основы',
      teacher:'Мария Корецкая', room:'Zoom · онлайн', students:18, status:'upcoming' },
    { time:'14:00', end:'15:30', title:'Иллюстрация для начинающих',
      teacher:'Елена Петрова',  room:'Zoom · онлайн', students:9,  status:'upcoming' },
    { time:'16:30', end:'18:00', title:'Python — продвинутый курс',
      teacher:'Артём Захаров',  room:'Zoom · онлайн', students:15, status:'upcoming' },
  ];

  return (
    <Card style={{padding:0, display:'flex', flexDirection:'column'}}>
      <div style={{
        display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'18px 20px 14px', borderBottom:'1px solid #f1f5f9'
      }}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Расписание сегодня</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>
            5 уроков · 1 идёт сейчас · следующий через 1 ч 30 мин
          </div>
        </div>
        <div style={{display:'flex', alignItems:'center', gap:6}}>
          <button style={iconBtnStyle()}><Icon.ChevronLeft size={14}/></button>
          <button style={{...iconBtnStyle(), padding:'0 12px', width:'auto', fontSize:12.5, fontWeight:600, gap:6, display:'inline-flex', alignItems:'center'}}>
            <Icon.CalendarDays size={13}/>Сегодня
          </button>
          <button style={iconBtnStyle()}><Icon.ChevronRight size={14}/></button>
        </div>
      </div>
      <div style={{padding:'8px 20px 16px'}}>
        {items.map((it, i) => (
          <div key={i} style={{
            display:'grid', gridTemplateColumns:'62px 1fr auto', alignItems:'stretch', gap:14,
            padding:'12px 0', borderTop: i>0 ? '1px solid #f1f5f9' : '0'
          }}>
            <div style={{display:'flex', flexDirection:'column', alignItems:'flex-start'}}>
              <div style={{fontSize:13.5, fontWeight:700, color: it.status==='done' ? '#94a3b8' : '#0f172a', fontVariantNumeric:'tabular-nums'}}>
                {it.time}
              </div>
              <div style={{fontSize:11, color:'#94a3b8', marginTop:1}}>до {it.end}</div>
            </div>
            <div style={{display:'flex', alignItems:'center', gap:12, minWidth:0}}>
              <div style={{
                width:3, height:32, borderRadius:9999,
                background: it.status==='live' ? '#10b981'
                         : it.status==='done' ? '#cbd5e1'
                         : '#c7d2fe'
              }}/>
              <div style={{minWidth:0}}>
                <div style={{fontSize:13.5, fontWeight:600,
                  color: it.status==='done' ? '#64748b' : '#0f172a',
                  textDecoration: it.status==='done' ? 'line-through' : 'none',
                  textDecorationColor:'#cbd5e1'}}>{it.title}</div>
                <div style={{fontSize:12, color:'#64748b', marginTop:2, display:'flex', gap:8, alignItems:'center'}}>
                  <Avatar name={it.teacher} size={18}/>
                  <span>{it.teacher}</span>
                  <span style={{width:3, height:3, borderRadius:9999, background:'#cbd5e1'}}/>
                  <span>{it.students} студентов</span>
                </div>
              </div>
            </div>
            <div style={{display:'flex', alignItems:'center', gap:10}}>
              {it.status==='live' && (
                <div style={{display:'flex', flexDirection:'column', alignItems:'flex-end', gap:6}}>
                  <Badge variant="success" dot>Идёт сейчас</Badge>
                  <div style={{width:90, height:4, background:'#f1f5f9', borderRadius:9999, overflow:'hidden'}}>
                    <div style={{width: it.progress+'%', height:'100%', background:'#10b981'}}/>
                  </div>
                </div>
              )}
              {it.status==='done' && <Badge variant="default" dot>Завершён</Badge>}
              {it.status==='upcoming' && <Badge variant="default">Скоро</Badge>}
            </div>
          </div>
        ))}
      </div>
    </Card>
  );
}

// ─── Attendance breakdown ──────────────────────────────────────────────
function AttendanceCard() {
  const rows = [
    { color:'#10b981', label:'Присутствуют',  count:216, pct:87 },
    { color:'#f59e0b', label:'Опаздывают',    count:17,  pct:7 },
    { color:'#f43f5e', label:'Отсутствуют',   count:15,  pct:6 },
  ];
  return (
    <Card style={{padding:20, display:'flex', flexDirection:'column', gap:18}}>
      <div style={{display:'flex', alignItems:'flex-start', justifyContent:'space-between'}}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Посещаемость</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>За сегодня · 248 студентов</div>
        </div>
        <a style={{fontSize:12, color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>Отчёт →</a>
      </div>
      <div style={{display:'flex', alignItems:'center', justifyContent:'center', paddingTop:4}}>
        <AttendanceDonut present={87} late={7} absent={6} size={172} strokeW={20}/>
      </div>
      <div style={{display:'flex', flexDirection:'column', gap:10}}>
        {rows.map(r => (
          <div key={r.label} style={{display:'flex', alignItems:'center', justifyContent:'space-between'}}>
            <div style={{display:'flex', alignItems:'center', gap:10}}>
              <span style={{width:8, height:8, borderRadius:9999, background:r.color}}/>
              <span style={{fontSize:13, color:'#334155'}}>{r.label}</span>
            </div>
            <div style={{display:'flex', alignItems:'baseline', gap:8}}>
              <span style={{fontSize:13.5, fontWeight:600, fontVariantNumeric:'tabular-nums'}}>{r.count}</span>
              <span style={{fontSize:11.5, color:'#94a3b8', fontVariantNumeric:'tabular-nums'}}>{r.pct}%</span>
            </div>
          </div>
        ))}
      </div>
    </Card>
  );
}

// ─── Top courses ───────────────────────────────────────────────────────
function TopCoursesCard() {
  const courses = [
    { name:'Веб-разработка с нуля',  category:'Программирование', students:48, revenue:'₽82 400', rating:4.9,
      trend:'up',   trendVal:'+12%', iconBg:'#e0eaff', iconColor:'#4338ca', icon:'BookOpen' },
    { name:'UX/UI дизайн в Figma',   category:'Дизайн',           students:36, revenue:'₽64 800', rating:4.8,
      trend:'up',   trendVal:'+8%',  iconBg:'#ede9fe', iconColor:'#7c3aed', icon:'Sparkles' },
    { name:'Python для детей',       category:'Программирование', students:28, revenue:'₽42 000', rating:4.7,
      trend:'up',   trendVal:'+4%',  iconBg:'#d1fae5', iconColor:'#059669', icon:'BookOpen' },
    { name:'Иллюстрация и скетчинг', category:'Дизайн',           students:24, revenue:'₽38 400', rating:4.9,
      trend:'flat', trendVal:'0%',   iconBg:'#fef3c7', iconColor:'#b45309', icon:'Sparkles' },
    { name:'Дизайн 3D-моделей',      category:'Дизайн',           students:22, revenue:'₽35 200', rating:4.6,
      trend:'down', trendVal:'-2%',  iconBg:'#fee2e2', iconColor:'#b91c1c', icon:'Sparkles' },
  ];

  return (
    <Card style={{padding:0, display:'flex', flexDirection:'column'}}>
      <div style={{
        display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'18px 20px 14px', borderBottom:'1px solid #f1f5f9'
      }}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Топ курсов</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>По выручке за апрель</div>
        </div>
        <div style={{display:'flex', alignItems:'center', gap:6}}>
          <button style={{...iconBtnStyle(), padding:'0 12px', width:'auto', fontSize:12.5, fontWeight:600, gap:6, display:'inline-flex', alignItems:'center'}}>
            Выручка <Icon.ChevronDown size={12}/>
          </button>
          <a style={{fontSize:12, color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>Все курсы →</a>
        </div>
      </div>
      <table style={{width:'100%', borderCollapse:'separate', borderSpacing:0}}>
        <thead>
          <tr>
            <th style={thStyle()}>Курс</th>
            <th style={thStyle('center')}>Студентов</th>
            <th style={thStyle('center')}>Рейтинг</th>
            <th style={thStyle('right')}>Выручка</th>
            <th style={thStyle('right')}>Тренд</th>
          </tr>
        </thead>
        <tbody>
          {courses.map((c,i) => {
            const Ic = Icon[c.icon];
            const tcolor = c.trend==='up' ? '#059669' : c.trend==='down' ? '#e11d48' : '#94a3b8';
            const TIc = c.trend==='up' ? Icon.TrendingUp : c.trend==='down' ? Icon.TrendingDown : Icon.ArrowRight;
            return (
              <tr key={c.name}>
                <td style={tdStyle()}>
                  <div style={{display:'flex', alignItems:'center', gap:12}}>
                    <div style={{
                      width:36, height:36, borderRadius:10,
                      background:c.iconBg, color:c.iconColor,
                      display:'flex', alignItems:'center', justifyContent:'center'
                    }}><Ic size={16}/></div>
                    <div>
                      <div style={{fontSize:13.5, fontWeight:600}}>{c.name}</div>
                      <div style={{fontSize:11.5, color:'#94a3b8', marginTop:1}}>{c.category}</div>
                    </div>
                  </div>
                </td>
                <td style={tdStyle('center')}>
                  <div style={{fontSize:13, fontWeight:600, fontVariantNumeric:'tabular-nums'}}>{c.students}</div>
                </td>
                <td style={tdStyle('center')}>
                  <span style={{display:'inline-flex', alignItems:'center', gap:4, fontSize:12.5, fontWeight:600, color:'#334155'}}>
                    <span style={{color:'#f59e0b', fontSize:13}}>★</span>{c.rating.toFixed(1)}
                  </span>
                </td>
                <td style={tdStyle('right')}>
                  <div style={{fontSize:13.5, fontWeight:600, fontVariantNumeric:'tabular-nums'}}>{c.revenue}</div>
                </td>
                <td style={tdStyle('right')}>
                  <span style={{display:'inline-flex', alignItems:'center', gap:3, fontSize:12, fontWeight:600, color:tcolor}}>
                    <TIc size={13}/>{c.trendVal}
                  </span>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </Card>
  );
}

// ─── New students ──────────────────────────────────────────────────────
function NewStudentsCard() {
  const items = [
    { name:'Иван Козлов',     course:'Дизайн 3D-моделей',  time:'15 мин назад', source:'Лендинг' },
    { name:'Кирилл Михайлов', course:'Дизайн 3D-моделей',  time:'2 часа назад', source:'Лендинг' },
    { name:'Мария Корецкая',  course:'Веб-разработка',     time:'вчера',        source:'Реферал' },
    { name:'Елена Петрова',   course:'Иллюстрация',        time:'14 апр.',      source:'Instagram' },
    { name:'Дмитрий Соколов', course:'Python для детей',   time:'13 апр.',      source:'Лендинг' },
  ];
  return (
    <Card style={{padding:0, display:'flex', flexDirection:'column'}}>
      <div style={{
        display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'18px 20px 14px', borderBottom:'1px solid #f1f5f9'
      }}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Новые студенты</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>24 в апреле</div>
        </div>
        <a style={{fontSize:12, color:'#4f46e5', fontWeight:600, cursor:'pointer'}}>Все →</a>
      </div>
      <div style={{padding:'8px 20px 14px'}}>
        {items.map((s,i) => (
          <div key={s.name} style={{
            display:'flex', alignItems:'center', gap:12, padding:'10px 0',
            borderTop: i>0 ? '1px solid #f1f5f9' : '0'
          }}>
            <Avatar name={s.name} size={36}/>
            <div style={{flex:1, minWidth:0}}>
              <div style={{fontSize:13, fontWeight:600}}>{s.name}</div>
              <div style={{fontSize:12, color:'#64748b'}}>{s.course}</div>
            </div>
            <div style={{display:'flex', flexDirection:'column', alignItems:'flex-end', gap:3}}>
              <span style={{fontSize:11, color:'#94a3b8'}}>{s.time}</span>
              <span style={{
                fontSize:10.5, fontWeight:600, padding:'1px 7px', borderRadius:4,
                background:'#f1f5f9', color:'#64748b', letterSpacing:'0.02em'
              }}>{s.source}</span>
            </div>
          </div>
        ))}
      </div>
    </Card>
  );
}

// ─── Activity feed ─────────────────────────────────────────────────────
function ActivityFeedCard() {
  const events = [
    { kind:'enrolled', icon:'UserPlus',     color:'#4f46e5',
      text:<span><b>Иван Козлов</b> записался на курс «Дизайн 3D-моделей»</span>, time:'15 мин назад' },
    { kind:'payment',  icon:'CreditCard',   color:'#059669',
      text:<span><b>Анна Сергеева</b> оплатила курс UX/UI · ₽14 800</span>, time:'42 мин назад' },
    { kind:'lesson',   icon:'CheckCircle2', color:'#10b981',
      text:<span><b>Виктория Лебедева</b> сдала задание «Цветовая палитра»</span>, time:'1 час назад' },
    { kind:'warn',     icon:'AlertCircle',  color:'#b91c1c',
      text:<span>Просрочена оплата у <b>Сергей Новиков</b> · группа Web-fullstack-2</span>, time:'2 часа назад' },
    { kind:'lesson',   icon:'BookOpen',     color:'#7c3aed',
      text:<span><b>Артём Захаров</b> прошёл урок «Циклы в Python»</span>, time:'3 часа назад' },
    { kind:'enrolled', icon:'UserPlus',     color:'#4f46e5',
      text:<span><b>Кирилл Михайлов</b> записался на курс «Дизайн 3D-моделей»</span>, time:'4 часа назад' },
  ];
  return (
    <Card style={{padding:0}}>
      <div style={{
        display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'18px 20px 14px', borderBottom:'1px solid #f1f5f9'
      }}>
        <div>
          <div style={{fontSize:14.5, fontWeight:600}}>Лента активности</div>
          <div style={{fontSize:12.5, color:'#64748b', marginTop:2}}>Реальное время · 142 события за сегодня</div>
        </div>
        <div style={{display:'flex', alignItems:'center', gap:6}}>
          <FilterChip active>Все</FilterChip>
          <FilterChip>Записи</FilterChip>
          <FilterChip>Платежи</FilterChip>
          <FilterChip>Уроки</FilterChip>
        </div>
      </div>
      <div style={{padding:'14px 20px 18px', display:'grid', gridTemplateColumns:'1fr 1fr', gap:'0 32px'}}>
        {events.map((e,i) => {
          const Ic = Icon[e.icon];
          return (
            <div key={i} style={{
              display:'flex', gap:12, alignItems:'flex-start', padding:'10px 0',
              borderBottom: i < events.length-2 ? '1px solid #f1f5f9' : '0'
            }}>
              <div style={{
                width:30, height:30, borderRadius:9999, flexShrink:0,
                background:e.color+'1A', color:e.color, border:'1px solid '+e.color+'33',
                display:'flex', alignItems:'center', justifyContent:'center'
              }}><Ic size={13}/></div>
              <div style={{flex:1, minWidth:0}}>
                <div style={{fontSize:13, color:'#334155', lineHeight:1.4}}>{e.text}</div>
                <div style={{fontSize:11.5, color:'#94a3b8', marginTop:3}}>{e.time}</div>
              </div>
            </div>
          );
        })}
      </div>
    </Card>
  );
}

// ─── helpers ───────────────────────────────────────────────────────────
function FilterChip({children, active}) {
  return (
    <button style={{
      padding:'4px 10px', borderRadius:7, fontSize:12, fontWeight:500,
      border:'1px solid '+(active ? '#c7d2fe' : '#e2e8f0'),
      background: active ? '#eef2ff' : '#fff',
      color: active ? '#4338ca' : '#475569', cursor:'pointer'
    }}>{children}</button>
  );
}
function iconBtnStyle() {
  return {
    width:30, height:30, borderRadius:7, border:'1px solid #e2e8f0', background:'#fff',
    color:'#475569', cursor:'pointer', display:'inline-flex', alignItems:'center', justifyContent:'center'
  };
}
function thStyle(align='left') {
  return {
    textAlign:align, padding:'10px 18px', fontSize:11, fontWeight:600,
    color:'#94a3b8', textTransform:'uppercase', letterSpacing:'0.05em',
    background:'#fafbfc', borderBottom:'1px solid #f1f5f9', whiteSpace:'nowrap'
  };
}
function tdStyle(align='left') {
  return { textAlign:align, padding:'12px 18px', borderBottom:'1px solid #f8fafc', verticalAlign:'middle' };
}

window.ScheduleCard = ScheduleCard;
window.AttendanceCard = AttendanceCard;
window.TopCoursesCard = TopCoursesCard;
window.NewStudentsCard = NewStudentsCard;
window.ActivityFeedCard = ActivityFeedCard;
