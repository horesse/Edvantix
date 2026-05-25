// ─── Slide-over student detail drawer ──────────────────────────────────
function StudentDrawer({student, onClose}) {
  // Animate in
  const [shown, setShown] = React.useState(false);
  React.useEffect(() => {
    if (student) {
      requestAnimationFrame(() => setShown(true));
    } else {
      setShown(false);
    }
  }, [student]);

  if (!student) return null;

  const s = student;

  return (
    <div style={{position:'fixed', inset:0, zIndex:60, pointerEvents:'auto'}}>
      {/* Backdrop */}
      <div onClick={onClose} style={{
        position:'absolute', inset:0, background:'rgba(15,23,42,0.35)',
        opacity: shown ? 1 : 0, transition:'opacity .25s ease'
      }}/>
      {/* Panel */}
      <div style={{
        position:'absolute', top:0, right:0, height:'100%', width:520,
        background:'#fff', boxShadow:'-12px 0 32px rgba(15,23,42,0.12)',
        display:'flex', flexDirection:'column',
        transform: shown ? 'translateX(0)' : 'translateX(100%)',
        transition:'transform .3s cubic-bezier(.4,0,.2,1)',
        overflowY:'auto'
      }}>
        {/* Sticky head */}
        <div style={{
          position:'sticky', top:0, background:'#fff', zIndex:2,
          padding:'18px 24px 14px', borderBottom:'1px solid #e2e8f0',
          display:'flex', alignItems:'center', justifyContent:'space-between'
        }}>
          <div style={{display:'flex', alignItems:'center', gap:8, color:'#64748b', fontSize:12.5}}>
            <Icon.Users size={13}/>
            <span>Студенты</span>
            <Icon.ChevronRight size={12}/>
            <span style={{color:'#0f172a', fontWeight:600}}>{s.name}</span>
          </div>
          <div style={{display:'flex', alignItems:'center', gap:6}}>
            <IconBtn icon="Edit" title="Редактировать"/>
            <IconBtn icon="MoreHorizontal" title="Ещё"/>
            <IconBtn icon="X" title="Закрыть" onClick={onClose}/>
          </div>
        </div>

        {/* Hero */}
        <div style={{padding:'24px 24px 20px', display:'flex', flexDirection:'column', gap:16, borderBottom:'1px solid #e2e8f0'}}>
          <div style={{display:'flex', alignItems:'flex-start', gap:16}}>
            <Avatar name={s.name} size={64} style={{boxShadow:'0 6px 16px rgba(15,23,42,0.12)'}}/>
            <div style={{flex:1, minWidth:0}}>
              <div style={{display:'flex', alignItems:'center', gap:8}}>
                <h2 style={{margin:0, fontSize:20, fontWeight:700, letterSpacing:'-0.01em'}}>{s.name}</h2>
                <StatusPill status={s.status}/>
              </div>
              <div style={{marginTop:4, display:'flex', alignItems:'center', gap:12, fontSize:13, color:'#64748b'}}>
                <span style={{display:'inline-flex', alignItems:'center', gap:5}}>
                  <Icon.GraduationCap size={13}/>{s.group}
                </span>
                <span style={{width:3, height:3, borderRadius:9999, background:'#cbd5e1'}}/>
                <span>с {s.enrolled}</span>
              </div>
            </div>
          </div>
          {/* Quick action bar */}
          <div style={{display:'grid', gridTemplateColumns:'1fr 1fr 1fr 1fr', gap:8}}>
            <QuickBtn icon="Send"          label="Сообщение" primary/>
            <QuickBtn icon="Phone"         label="Звонок"/>
            <QuickBtn icon="CalendarDays"  label="Записать"/>
            <QuickBtn icon="CreditCard"    label="Счёт"/>
          </div>
        </div>

        {/* Contact + meta */}
        <SectionBlock title="Контакты">
          <ContactRow icon="Mail"   label="Email"    value={s.email}/>
          <ContactRow icon="Phone"  label="Телефон"  value={s.phone}/>
          <ContactRow icon="MapPin" label="Город"    value="Москва"/>
        </SectionBlock>

        {/* Progress / courses */}
        <SectionBlock title="Курсы и прогресс">
          <div style={{display:'flex', flexDirection:'column', gap:12}}>
            {s.courses.map((c, i) => {
              const prog = [s.progress, Math.max(0, s.progress-18), Math.min(100, s.progress+11)][i] ?? s.progress;
              const lessons = [12, 8, 16][i] ?? 10;
              const done = Math.round(prog/100*lessons);
              return (
                <div key={c} style={{
                  border:'1px solid #e2e8f0', borderRadius:12, padding:14,
                  display:'flex', flexDirection:'column', gap:10
                }}>
                  <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', gap:8}}>
                    <div style={{display:'flex', alignItems:'center', gap:10, minWidth:0}}>
                      <div style={{
                        width:32, height:32, borderRadius:8,
                        background:'#eef2ff', color:'#4338ca',
                        display:'flex', alignItems:'center', justifyContent:'center'
                      }}><Icon.BookOpen size={15}/></div>
                      <div style={{minWidth:0}}>
                        <div style={{fontSize:13.5, fontWeight:600, color:'#0f172a'}}>{c}</div>
                        <div style={{fontSize:12, color:'#64748b'}}>{done} из {lessons} уроков</div>
                      </div>
                    </div>
                    <span style={{fontSize:12, fontWeight:600, color:'#334155', fontVariantNumeric:'tabular-nums'}}>{prog}%</span>
                  </div>
                  <ProgressBar value={prog}/>
                </div>
              );
            })}
            <button style={{
              padding:'10px 12px', borderRadius:10, border:'1px dashed #cbd5e1',
              background:'transparent', color:'#475569', fontSize:13, fontWeight:600,
              display:'inline-flex', alignItems:'center', justifyContent:'center', gap:6, cursor:'pointer'
            }}>
              <Icon.Plus size={14}/>Назначить курс
            </button>
          </div>
        </SectionBlock>

        {/* Payments */}
        <SectionBlock title="Платежи">
          <div style={{
            background:'#fafbfc', border:'1px solid #e2e8f0', borderRadius:12,
            padding:'14px 16px', display:'flex', alignItems:'center', justifyContent:'space-between', gap:12
          }}>
            <div>
              <div style={{fontSize:12, color:'#64748b'}}>Текущий статус</div>
              <div style={{marginTop:4}}>
                <Badge variant={PAYMENT_META[s.payment].variant}>{PAYMENT_META[s.payment].label}</Badge>
              </div>
            </div>
            <div style={{textAlign:'right'}}>
              <div style={{fontSize:12, color:'#64748b'}}>Действует до</div>
              <div style={{fontSize:14, fontWeight:600, color:'#0f172a', marginTop:2}}>{s.paidUntil}</div>
            </div>
          </div>
          <div style={{display:'flex', flexDirection:'column'}}>
            {[
              { date:'12 апр. 2026', amount:'₽14 800', label:'Курс UX/UI дизайн', status:'ok' },
              { date:'12 мар. 2026', amount:'₽14 800', label:'Курс UX/UI дизайн', status:'ok' },
              { date:'12 фев. 2026', amount:'₽4 200',  label:'Доп. урок · Figma', status:'ok' },
            ].map((p,i) => (
              <div key={i} style={{
                display:'flex', alignItems:'center', justifyContent:'space-between',
                padding:'10px 0', borderTop: i>0 ? '1px solid #f1f5f9' : '0'
              }}>
                <div style={{display:'flex', alignItems:'center', gap:10}}>
                  <div style={{
                    width:28, height:28, borderRadius:8, background:'#d1fae5', color:'#047857',
                    display:'flex', alignItems:'center', justifyContent:'center'
                  }}><Icon.Check size={13}/></div>
                  <div>
                    <div style={{fontSize:13, fontWeight:500, color:'#0f172a'}}>{p.label}</div>
                    <div style={{fontSize:11.5, color:'#94a3b8'}}>{p.date}</div>
                  </div>
                </div>
                <span style={{fontSize:13.5, fontWeight:600, color:'#0f172a', fontVariantNumeric:'tabular-nums'}}>{p.amount}</span>
              </div>
            ))}
          </div>
        </SectionBlock>

        {/* Activity timeline */}
        <SectionBlock title="Активность" last>
          <div style={{display:'flex', flexDirection:'column'}}>
            {[
              { icon:'CheckCircle2', color:'#10b981', text:'Сдал(а) задание «Onboarding-флоу»', time:'5 мин назад'},
              { icon:'CalendarDays', color:'#4f46e5', text:'Был(а) на занятии «Атомарный дизайн»', time:'вчера, 12:00'},
              { icon:'BookOpen',     color:'#7c3aed', text:'Открыл(а) урок «Auto-layout в Figma»',   time:'14 апр., 18:42'},
              { icon:'CreditCard',   color:'#059669', text:'Оплатил(а) курс на месяц',               time:'12 апр., 09:15'},
            ].map((e,i,arr) => {
              const Ic = Icon[e.icon];
              return (
                <div key={i} style={{display:'flex', gap:12, position:'relative'}}>
                  <div style={{display:'flex', flexDirection:'column', alignItems:'center'}}>
                    <div style={{
                      width:28, height:28, borderRadius:9999, background: e.color+'1A', color:e.color,
                      display:'flex', alignItems:'center', justifyContent:'center', flexShrink:0,
                      border:'1px solid '+e.color+'33'
                    }}><Ic size={13}/></div>
                    {i < arr.length-1 && <div style={{width:1, flex:1, background:'#e2e8f0', minHeight:14, marginTop:2}}/>}
                  </div>
                  <div style={{flex:1, paddingBottom: i<arr.length-1 ? 14 : 0}}>
                    <div style={{fontSize:13, color:'#0f172a'}}>{e.text}</div>
                    <div style={{fontSize:11.5, color:'#94a3b8', marginTop:2}}>{e.time}</div>
                  </div>
                </div>
              );
            })}
          </div>
        </SectionBlock>
      </div>
    </div>
  );
}

function IconBtn({icon, onClick, title}) {
  const [h, sh] = React.useState(false);
  const Ic = Icon[icon];
  return (
    <button onClick={onClick} title={title}
      onMouseEnter={()=>sh(true)} onMouseLeave={()=>sh(false)}
      style={{
        width:32, height:32, borderRadius:8, border:'1px solid '+(h?'#e2e8f0':'transparent'),
        background: h ? '#f8fafc' : 'transparent', color:'#475569', cursor:'pointer',
        display:'inline-flex', alignItems:'center', justifyContent:'center'
      }}>
      <Ic size={15}/>
    </button>
  );
}

function QuickBtn({icon, label, primary}) {
  const [h, sh] = React.useState(false);
  const Ic = Icon[icon];
  return (
    <button onMouseEnter={()=>sh(true)} onMouseLeave={()=>sh(false)} style={{
      display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center',
      gap:5, padding:'10px 8px', borderRadius:10,
      border:'1px solid '+(primary ? '#c7d2fe' : '#e2e8f0'),
      background: primary
        ? (h?'#e0e7ff':'#eef2ff')
        : (h?'#f8fafc':'#fff'),
      color: primary ? '#4338ca' : '#334155',
      fontSize:12, fontWeight:600, cursor:'pointer', transition:'.1s'
    }}>
      <Ic size={15}/>{label}
    </button>
  );
}

function SectionBlock({title, children, last}) {
  return (
    <div style={{padding:'20px 24px', borderBottom: last ? 'none' : '1px solid #e2e8f0', display:'flex', flexDirection:'column', gap:12}}>
      <div style={{
        fontSize:10.5, fontWeight:700, letterSpacing:'0.1em', textTransform:'uppercase', color:'#94a3b8'
      }}>{title}</div>
      <div>{children}</div>
    </div>
  );
}

function ContactRow({icon, label, value}) {
  const Ic = Icon[icon];
  return (
    <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', padding:'8px 0', borderTop:'1px solid #f1f5f9'}}>
      <div style={{display:'flex', alignItems:'center', gap:10, color:'#64748b'}}>
        <Ic size={14}/>
        <span style={{fontSize:13}}>{label}</span>
      </div>
      <span style={{fontSize:13, fontWeight:500, color:'#0f172a'}}>{value}</span>
    </div>
  );
}

window.StudentDrawer = StudentDrawer;
