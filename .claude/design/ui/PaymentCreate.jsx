// ── "Принять платёж" — create / accept payment form ────────────────────
// Light theme · indigo brand · sectioned form with live receipt sidebar.

// — Demo data —
const RECENT_STUDENTS = [
  { id:'STU-1782', name:'Иван Козлов',    email:'i.kozlov@mail.ru',     phone:'+7 (916) 234-15-08', group:'Дизайн-3 (D3-2026)',     lastAmt: 18900, lastWhen:'вчера' },
  { id:'STU-1654', name:'Мария Соколова', email:'m.sokolova@yandex.ru', phone:'+7 (903) 871-09-42', group:'Frontend-Pro (FP-26)',   lastAmt: 24500, lastWhen:'3 дня назад' },
  { id:'STU-1701', name:'Артём Васильев', email:'artem.v@gmail.com',    phone:'+7 (985) 432-12-87', group:'Python-Junior (PJ-26)',  lastAmt: 12000, lastWhen:'неделю назад' },
  { id:'STU-1820', name:'Елена Жукова',   email:'e.zhukova@mail.ru',    phone:'+7 (926) 119-44-21', group:'UX-Research (UX-26)',    lastAmt: 32000, lastWhen:'2 недели назад' },
];

const COURSE_OPTIONS = [
  { id:'c1', name:'Дизайн 3D-моделей',         plan:'Стандарт',  period:'Июнь — Август 2026',     lessons:24, pricePerLesson:875, group:'Дизайн-3 (D3-2026)' },
  { id:'c2', name:'Дизайн 3D-моделей',         plan:'Премиум',   period:'Июнь — Август 2026',     lessons:24, pricePerLesson:1250, group:'Дизайн-3 (D3-2026)' },
  { id:'c3', name:'Индивидуальный пакет — 4 занятия', plan:'Разовая',   period:'до 30 июня 2026',  lessons:4,  pricePerLesson:1800, group:'1-на-1' },
];

const DISCOUNT_PRESETS = [
  { id:'none', label:'Без скидки',          pct:0 },
  { id:'loyal',label:'Постоянный студент',  pct:10 },
  { id:'sib',  label:'Семейная (брат/сестра)', pct:15 },
  { id:'prom', label:'Промокод SUMMER26',   pct:20 },
];

// — formatters —
const fmtRub = (n) => '₽\u00A0' + new Intl.NumberFormat('ru-RU').format(n);

// ── Section wrapper with numbered chip ────────────────────────────────
function Section({n, title, hint, action, children, complete}) {
  return (
    <div style={{display:'flex',gap:18}}>
      <div style={{
        flexShrink:0,width:32,height:32,marginTop:2,borderRadius:9999,
        display:'inline-flex',alignItems:'center',justifyContent:'center',
        background: complete ? '#d1fae5' : '#eef2ff',
        color: complete ? '#047857' : '#4f46e5',
        fontSize:13,fontWeight:700,
        border: complete ? '1px solid #a7f3d0' : '1px solid #e0e7ff',
      }}>
        {complete ? <Icon.Check size={14}/> : n}
      </div>
      <div style={{flex:1,minWidth:0}}>
        <div style={{
          display:'flex',alignItems:'flex-end',justifyContent:'space-between',gap:12,
          paddingBottom:10,
        }}>
          <div>
            <h2 style={{margin:0,fontSize:15,fontWeight:700,color:'#0f172a',letterSpacing:'-0.01em'}}>{title}</h2>
            {hint && <div style={{fontSize:12.5,color:'#94a3b8',marginTop:3}}>{hint}</div>}
          </div>
          {action}
        </div>
        {children}
      </div>
    </div>
  );
}

// ── Step 1. Student ────────────────────────────────────────────────────
function StudentPicker({student, onPick, onClear}) {
  if (student) {
    return (
      <Card style={{padding:'14px 18px',display:'flex',alignItems:'center',gap:14}}>
        <Avatar name={student.name} size={44}/>
        <div style={{flex:1,minWidth:0}}>
          <div style={{display:'flex',alignItems:'center',gap:8,flexWrap:'wrap'}}>
            <span style={{fontSize:15,fontWeight:600,color:'#0f172a'}}>{student.name}</span>
            <Badge variant="default" style={{fontVariantNumeric:'tabular-nums'}}>{student.id}</Badge>
            <Badge variant="primary">{student.group}</Badge>
          </div>
          <div style={{
            display:'flex',gap:14,fontSize:12.5,color:'#64748b',marginTop:4,flexWrap:'wrap',
          }}>
            <span style={{display:'inline-flex',alignItems:'center',gap:4}}><Icon.Mail size={12} stroke="#94a3b8"/>{student.email}</span>
            <span style={{display:'inline-flex',alignItems:'center',gap:4}}><Icon.Phone size={12} stroke="#94a3b8"/>{student.phone}</span>
          </div>
        </div>
        <button onClick={onClear} title="Сменить студента" style={{
          width:32,height:32,borderRadius:8,border:'1px solid #e2e8f0',background:'#fff',color:'#64748b',
          display:'inline-flex',alignItems:'center',justifyContent:'center',cursor:'pointer',
        }}
        onMouseEnter={e=>e.currentTarget.style.background='#f8fafc'}
        onMouseLeave={e=>e.currentTarget.style.background='#fff'}>
          <Icon.RefreshCw size={14}/>
        </button>
      </Card>
    );
  }
  return (
    <Card style={{padding:16}}>
      <div style={{position:'relative',marginBottom:14}}>
        <Icon.Search size={15} stroke="#94a3b8" style={{position:'absolute',left:14,top:'50%',transform:'translateY(-50%)'}}/>
        <Input placeholder="Найти студента — имя, email, телефон, ID…" style={{paddingLeft:38, paddingRight:14, paddingTop:11, paddingBottom:11}}/>
      </div>
      <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,letterSpacing:'0.08em',textTransform:'uppercase',marginBottom:8}}>
        Недавние · 4
      </div>
      <div style={{display:'flex',flexDirection:'column',gap:4}}>
        {RECENT_STUDENTS.map(s=>(
          <button key={s.id} onClick={()=>onPick(s)} style={{
            display:'flex',alignItems:'center',gap:12,padding:'10px 12px',borderRadius:10,
            border:'1px solid transparent',background:'transparent',
            textAlign:'left',cursor:'pointer',fontFamily:'inherit',transition:'.1s',
          }}
          onMouseEnter={e=>{e.currentTarget.style.background='#f8fafc';e.currentTarget.style.borderColor='#eef2f7'}}
          onMouseLeave={e=>{e.currentTarget.style.background='transparent';e.currentTarget.style.borderColor='transparent'}}>
            <Avatar name={s.name} size={36}/>
            <div style={{flex:1,minWidth:0}}>
              <div style={{fontSize:13.5,fontWeight:600,color:'#0f172a'}}>{s.name}</div>
              <div style={{fontSize:12,color:'#94a3b8'}}>{s.group} · {s.email}</div>
            </div>
            <div style={{textAlign:'right',flexShrink:0}}>
              <div style={{fontSize:12.5,color:'#475569',fontWeight:500,fontVariantNumeric:'tabular-nums'}}>{fmtRub(s.lastAmt)}</div>
              <div style={{fontSize:11,color:'#cbd5e1'}}>{s.lastWhen}</div>
            </div>
          </button>
        ))}
      </div>
      <button style={{
        marginTop:10,width:'100%',padding:'10px 12px',borderRadius:10,
        border:'1px dashed #cbd5e1',background:'#fff',color:'#4f46e5',
        fontSize:13,fontWeight:600,fontFamily:'inherit',cursor:'pointer',
        display:'inline-flex',alignItems:'center',justifyContent:'center',gap:6,
      }}>
        <Icon.UserPlus size={14}/>Создать нового студента
      </button>
    </Card>
  );
}

// ── Step 2. What for — course / plan ──────────────────────────────────
function CoursePicker({course, onPick}) {
  return (
    <Card style={{padding:8}}>
      {COURSE_OPTIONS.map((c, i)=>{
        const on = course?.id === c.id;
        const total = c.lessons * c.pricePerLesson;
        return (
          <button key={c.id} onClick={()=>onPick(c)} style={{
            display:'flex',alignItems:'center',gap:14,padding:'14px 14px',borderRadius:12,
            border:'1px solid '+(on?'#4f46e5':'transparent'),
            background: on ? '#f5f7ff' : 'transparent',
            width:'100%',textAlign:'left',cursor:'pointer',fontFamily:'inherit',transition:'.12s',
            marginBottom: i<COURSE_OPTIONS.length-1?4:0,
          }}
          onMouseEnter={e=>{ if(!on){e.currentTarget.style.background='#f8fafc'} }}
          onMouseLeave={e=>{ if(!on){e.currentTarget.style.background='transparent'} }}>
            <div style={{
              width:20,height:20,borderRadius:9999,flexShrink:0,
              border:'2px solid '+(on?'#4f46e5':'#cbd5e1'),
              display:'inline-flex',alignItems:'center',justifyContent:'center',
              background: on?'#4f46e5':'#fff',
            }}>
              {on && <span style={{width:8,height:8,borderRadius:9999,background:'#fff'}}/>}
            </div>
            <div style={{
              width:38,height:38,borderRadius:8,flexShrink:0,
              background: c.plan==='Премиум' ? 'linear-gradient(135deg,#f59e0b,#ef4444)' : c.plan==='Разовая' ? 'linear-gradient(135deg,#10b981,#06b6d4)' : 'linear-gradient(135deg,#818cf8,#6366f1)',
              color:'#fff',display:'inline-flex',alignItems:'center',justifyContent:'center',
            }}><Icon.BookOpen size={17}/></div>
            <div style={{flex:1,minWidth:0}}>
              <div style={{display:'flex',alignItems:'center',gap:8,flexWrap:'wrap'}}>
                <span style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>{c.name}</span>
                <Badge variant={c.plan==='Премиум'?'warning':c.plan==='Разовая'?'success':'primary'} style={{fontSize:11}}>{c.plan}</Badge>
              </div>
              <div style={{fontSize:12.5,color:'#64748b',marginTop:3}}>
                {c.period} · {c.lessons} уроков · {fmtRub(c.pricePerLesson)}/урок
              </div>
            </div>
            <div style={{
              fontSize:15,fontWeight:700,color: on?'#4f46e5':'#0f172a',
              fontVariantNumeric:'tabular-nums',letterSpacing:'-0.01em',flexShrink:0,
            }}>{fmtRub(total)}</div>
          </button>
        );
      })}
    </Card>
  );
}

// ── Step 3. Amount & discount ─────────────────────────────────────────
function AmountBlock({subtotal, discountId, onDiscount, custom, onCustom, useCustom, setUseCustom}) {
  const preset = DISCOUNT_PRESETS.find(d=>d.id===discountId) || DISCOUNT_PRESETS[0];
  const discount = Math.round(subtotal * preset.pct / 100);
  const total = useCustom ? custom : (subtotal - discount);
  return (
    <Card style={{padding:'18px 20px'}}>
      <div style={{fontSize:12,color:'#64748b',fontWeight:600,marginBottom:10}}>Скидка</div>
      <div style={{display:'flex',flexWrap:'wrap',gap:6,marginBottom:18}}>
        {DISCOUNT_PRESETS.map(d=>{
          const on = discountId===d.id;
          return (
            <button key={d.id} onClick={()=>onDiscount(d.id)} style={{
              padding:'7px 12px',borderRadius:9999,border:'1px solid '+(on?'#4f46e5':'#e2e8f0'),
              background: on ? '#4f46e5':'#fff', color: on ? '#fff':'#475569',
              fontSize:12.5,fontWeight:600,cursor:'pointer',fontFamily:'inherit',
              display:'inline-flex',alignItems:'center',gap:6,transition:'.12s',
            }}>
              {d.label}
              {d.pct>0 && <span style={{
                fontSize:11,padding:'1px 6px',borderRadius:9999,fontVariantNumeric:'tabular-nums',
                background: on ? 'rgba(255,255,255,0.22)' : '#f1f5f9',
                color: on ? '#fff' : '#64748b',
              }}>−{d.pct}%</span>}
            </button>
          );
        })}
      </div>

      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr 1fr',gap:0,borderTop:'1px solid #eef2f7',paddingTop:14}}>
        <div>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,textTransform:'uppercase',letterSpacing:'0.06em'}}>Подытог</div>
          <div style={{fontSize:18,fontWeight:600,color:'#0f172a',marginTop:4,fontVariantNumeric:'tabular-nums'}}>{fmtRub(subtotal)}</div>
        </div>
        <div>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,textTransform:'uppercase',letterSpacing:'0.06em'}}>Скидка</div>
          <div style={{fontSize:18,fontWeight:600,color: discount>0 ? '#059669' : '#cbd5e1',marginTop:4,fontVariantNumeric:'tabular-nums'}}>
            {discount>0 ? '−'+fmtRub(discount) : '—'}
          </div>
        </div>
        <div>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,textTransform:'uppercase',letterSpacing:'0.06em'}}>К оплате</div>
          <div style={{fontSize:22,fontWeight:800,color:'#0f172a',marginTop:2,fontVariantNumeric:'tabular-nums',letterSpacing:'-0.02em'}}>{fmtRub(total)}</div>
        </div>
      </div>

      <div style={{
        marginTop:16,paddingTop:14,borderTop:'1px dashed #eef2f7',
        display:'flex',alignItems:'center',gap:10,
      }}>
        <label style={{display:'inline-flex',alignItems:'center',gap:8,fontSize:13,color:'#475569',cursor:'pointer'}}>
          <input type="checkbox" checked={useCustom} onChange={e=>setUseCustom(e.target.checked)}
            style={{width:16,height:16,accentColor:'#4f46e5',cursor:'pointer'}}/>
          Произвольная сумма
        </label>
        {useCustom && (
          <div style={{flex:1,maxWidth:200,position:'relative'}}>
            <span style={{position:'absolute',left:12,top:'50%',transform:'translateY(-50%)',color:'#94a3b8',fontSize:14}}>₽</span>
            <Input value={custom} onChange={e=>onCustom(+e.target.value.replace(/\D/g,'')||0)}
              style={{paddingLeft:26,fontVariantNumeric:'tabular-nums',fontWeight:600}}/>
          </div>
        )}
        <span style={{marginLeft:'auto',fontSize:11.5,color:'#94a3b8'}}>Без НДС · УСН</span>
      </div>
    </Card>
  );
}

// ── Step 4. Payment method ────────────────────────────────────────────
const METHODS = [
  { id:'terminal', label:'Карта (терминал)', sub:'POS-терминал', icon:'CreditCard' },
  { id:'link',     label:'Ссылка',           sub:'Email · SMS · Telegram', icon:'Link2' },
  { id:'sbp',      label:'СБП',              sub:'QR-код · перевод', icon:'Smartphone' },
  { id:'cash',     label:'Наличные',         sub:'Без терминала', icon:'Banknote' },
  { id:'transfer', label:'Перевод',          sub:'Расчётный счёт', icon:'Building2' },
];

function MethodTabs({method, onMethod}) {
  return (
    <div style={{
      display:'grid',gridTemplateColumns:'repeat(auto-fit,minmax(140px,1fr))',gap:10,marginBottom:14,
    }}>
      {METHODS.map(m=>{
        const Ic = Icon[m.icon];
        const on = method===m.id;
        return (
          <button key={m.id} onClick={()=>onMethod(m.id)} style={{
            display:'flex',flexDirection:'column',alignItems:'flex-start',gap:6,
            padding:'14px 14px',borderRadius:12,
            border:'1px solid '+(on?'#4f46e5':'#e2e8f0'),
            background: on?'#f5f7ff':'#fff',
            cursor:'pointer',fontFamily:'inherit',textAlign:'left',transition:'.12s',
            position:'relative',
          }}
          onMouseEnter={e=>{ if(!on){e.currentTarget.style.background='#f8fafc'} }}
          onMouseLeave={e=>{ if(!on){e.currentTarget.style.background='#fff'} }}>
            <div style={{
              width:32,height:32,borderRadius:8,
              background: on?'#4f46e5':'#f1f5f9',color: on?'#fff':'#475569',
              display:'inline-flex',alignItems:'center',justifyContent:'center',
            }}><Ic size={16}/></div>
            <div style={{fontSize:13,fontWeight:600,color:'#0f172a'}}>{m.label}</div>
            <div style={{fontSize:11.5,color:'#94a3b8'}}>{m.sub}</div>
            {on && (
              <div style={{
                position:'absolute',top:10,right:10,width:18,height:18,borderRadius:9999,
                background:'#4f46e5',color:'#fff',
                display:'inline-flex',alignItems:'center',justifyContent:'center',
              }}><Icon.Check size={11}/></div>
            )}
          </button>
        );
      })}
    </div>
  );
}

// — Terminal capture UI —
function TerminalPanel({amount}) {
  return (
    <Card style={{padding:0,overflow:'hidden'}}>
      <div style={{
        display:'grid',gridTemplateColumns:'1.1fr 1fr',
      }}>
        <div style={{padding:'22px 24px',borderRight:'1px solid #eef2f7'}}>
          <Badge variant="default" style={{marginBottom:12}}>Сбербанк-Эквайринг · POS-1209</Badge>
          <div style={{fontSize:18,fontWeight:700,color:'#0f172a',marginBottom:6}}>Поднесите карту к&nbsp;терминалу</div>
          <div style={{fontSize:13,color:'#64748b',marginBottom:16,lineHeight:1.5}}>
            Сумма уже&nbsp;отправлена на&nbsp;терминал. Студент может оплатить картой, Apple/Google Pay или Mir Pay.
          </div>
          <div style={{display:'flex',flexDirection:'column',gap:8}}>
            <TerminalStep n={1} active title="Сумма передана" sub="₽\u00A018\u00A0900 на POS-1209"/>
            <TerminalStep n={2} active title="Ожидание карты" sub="14 сек…" loading/>
            <TerminalStep n={3} title="Подтверждение банка"/>
            <TerminalStep n={4} title="Печать чека (54-ФЗ)"/>
          </div>
        </div>
        {/* — terminal mock — */}
        <div style={{
          padding:'24px',display:'flex',alignItems:'center',justifyContent:'center',
          background:'radial-gradient(circle at 50% 30%, #eef2ff 0%, #f8fafc 70%)',
        }}>
          <div style={{
            width:200,padding:'18px 16px 22px',borderRadius:18,
            background:'linear-gradient(180deg,#1e293b,#0f172a)',
            boxShadow:'0 24px 60px -16px rgba(15,23,42,0.4), inset 0 1px 0 rgba(255,255,255,0.08)',
            color:'#fff',position:'relative',
          }}>
            <div style={{
              display:'flex',justifyContent:'space-between',alignItems:'center',
              fontSize:10,color:'#94a3b8',marginBottom:14,fontWeight:600,letterSpacing:'0.05em',
            }}>
              <span>SBER · POS</span>
              <span style={{display:'inline-flex',alignItems:'center',gap:4,color:'#10b981'}}>
                <span style={{width:6,height:6,borderRadius:9999,background:'#10b981',boxShadow:'0 0 8px #10b981'}}/>
                ONLINE
              </span>
            </div>
            <div style={{
              background:'#0b1220',border:'1px solid rgba(255,255,255,0.06)',
              borderRadius:10,padding:'18px 14px',textAlign:'center',
            }}>
              <div style={{fontSize:10,color:'#64748b',fontWeight:600,letterSpacing:'0.08em',marginBottom:4}}>К ОПЛАТЕ</div>
              <div style={{fontSize:24,fontWeight:800,color:'#fff',fontVariantNumeric:'tabular-nums',letterSpacing:'-0.02em'}}>
                {fmtRub(amount)}
              </div>
              <div style={{
                marginTop:14,padding:'10px 8px',borderRadius:8,
                background:'rgba(99,102,241,0.14)',color:'#a5b4fc',
                fontSize:11,fontWeight:600,letterSpacing:'0.05em',
              }}>
                ПОДНЕСИТЕ КАРТУ
              </div>
              <div style={{display:'flex',justifyContent:'center',gap:14,marginTop:14,color:'#475569'}}>
                <Icon.CreditCard size={20}/>
                <Icon.Smartphone size={20}/>
                <span style={{fontSize:11,fontWeight:700,letterSpacing:'0.08em',color:'#94a3b8'}}>NFC</span>
              </div>
            </div>
            {/* keys */}
            <div style={{display:'grid',gridTemplateColumns:'1fr 1fr 1fr',gap:6,marginTop:14}}>
              {['1','2','3','4','5','6','7','8','9','','0',''].map((k,i)=>(
                <div key={i} style={{
                  height:18,borderRadius:4,background: k?'#1e293b':'transparent',
                  display:'inline-flex',alignItems:'center',justifyContent:'center',
                  fontSize:10,color:'#64748b',fontWeight:600,
                }}>{k}</div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
}

function TerminalStep({n, title, sub, active, loading}) {
  return (
    <div style={{display:'flex',alignItems:'center',gap:10}}>
      <div style={{
        width:22,height:22,borderRadius:9999,flexShrink:0,
        background: active?'#4f46e5':'#f1f5f9',
        color: active?'#fff':'#94a3b8',
        display:'inline-flex',alignItems:'center',justifyContent:'center',
        fontSize:11,fontWeight:700,
        position:'relative',
      }}>
        {n}
        {loading && (
          <span style={{
            position:'absolute',inset:-3,borderRadius:9999,
            border:'2px solid #4f46e5',borderRightColor:'transparent',
            animation:'ev-spin 0.9s linear infinite',
          }}/>
        )}
      </div>
      <div style={{minWidth:0,flex:1}}>
        <div style={{fontSize:13,fontWeight:active?600:500,color:active?'#0f172a':'#94a3b8'}}>{title}</div>
        {sub && <div style={{fontSize:11.5,color:'#94a3b8'}}>{sub}</div>}
      </div>
    </div>
  );
}

// — Link panel —
function LinkPanel({amount, student}) {
  const [channels, setChannels] = React.useState({email:true, sms:false, telegram:true});
  const [expiry, setExpiry] = React.useState('24h');
  const link = 'pay.edvantix.ru/i/INV-2026-0248-9k3f';
  const [copied, setCopied] = React.useState(false);
  return (
    <Card style={{padding:'20px 22px'}}>
      <div style={{display:'flex',alignItems:'center',gap:8,marginBottom:12}}>
        <span style={{fontSize:13,fontWeight:600,color:'#0f172a'}}>Куда отправить ссылку</span>
        <span style={{fontSize:11.5,color:'#94a3b8'}}>· можно несколько каналов</span>
      </div>
      <div style={{display:'flex',gap:8,marginBottom:18,flexWrap:'wrap'}}>
        {[
          {id:'email',label:'Email', sub: student?.email || '—', icon:'Mail'},
          {id:'sms',  label:'SMS',   sub: student?.phone || '—', icon:'Smartphone'},
          {id:'telegram',label:'Telegram', sub:'@i_kozlov',     icon:'Send'},
        ].map(c=>{
          const on = channels[c.id];
          const Ic = Icon[c.icon];
          return (
            <button key={c.id} onClick={()=>setChannels(s=>({...s,[c.id]:!s[c.id]}))} style={{
              display:'inline-flex',alignItems:'center',gap:10,padding:'10px 14px',borderRadius:12,
              border:'1px solid '+(on?'#4f46e5':'#e2e8f0'),
              background: on?'#f5f7ff':'#fff',
              cursor:'pointer',fontFamily:'inherit',transition:'.12s',minWidth:180,
            }}>
              <div style={{
                width:28,height:28,borderRadius:8,
                background: on?'#4f46e5':'#f1f5f9',color: on?'#fff':'#64748b',
                display:'inline-flex',alignItems:'center',justifyContent:'center',
              }}><Ic size={14}/></div>
              <div style={{textAlign:'left',minWidth:0}}>
                <div style={{fontSize:12.5,fontWeight:600,color:'#0f172a'}}>{c.label}</div>
                <div style={{fontSize:11,color:'#94a3b8',overflow:'hidden',textOverflow:'ellipsis',maxWidth:120,whiteSpace:'nowrap'}}>{c.sub}</div>
              </div>
              {on ? <Icon.CheckCircle2 size={16} stroke="#4f46e5"/> : <span style={{width:16}}/>}
            </button>
          );
        })}
      </div>

      <div style={{
        padding:'14px 16px',background:'#f8fafc',borderRadius:12,border:'1px solid #eef2f7',
        display:'flex',alignItems:'center',gap:12,
      }}>
        <div style={{
          width:36,height:36,borderRadius:9,background:'#fff',border:'1px solid #e2e8f0',
          display:'inline-flex',alignItems:'center',justifyContent:'center',color:'#4f46e5',
        }}><Icon.Link2 size={16}/></div>
        <div style={{flex:1,minWidth:0}}>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,letterSpacing:'0.06em',textTransform:'uppercase',marginBottom:2}}>
            Платёжная ссылка
          </div>
          <div style={{fontSize:13,color:'#0f172a',fontFamily:'ui-monospace, "SF Mono", Menlo, monospace',fontWeight:500}}>
            {link}
          </div>
        </div>
        <button onClick={()=>{navigator.clipboard?.writeText(link);setCopied(true);setTimeout(()=>setCopied(false),1200)}} style={{
          padding:'7px 12px',borderRadius:8,border:'1px solid #e2e8f0',background:'#fff',
          color: copied?'#047857':'#475569',fontSize:12.5,fontWeight:600,
          cursor:'pointer',fontFamily:'inherit',display:'inline-flex',alignItems:'center',gap:6,
        }}>
          {copied ? <><Icon.Check size={13}/>Скопировано</> : <><Icon.Copy size={13}/>Копировать</>}
        </button>
      </div>

      <div style={{
        marginTop:14,display:'flex',alignItems:'center',gap:14,flexWrap:'wrap',
      }}>
        <div style={{display:'flex',alignItems:'center',gap:8}}>
          <Icon.Clock size={14} stroke="#94a3b8"/>
          <span style={{fontSize:12.5,color:'#64748b'}}>Действительна</span>
          <select value={expiry} onChange={e=>setExpiry(e.target.value)} style={{
            border:'1px solid #e2e8f0',borderRadius:8,padding:'5px 26px 5px 10px',
            fontSize:12.5,fontWeight:500,color:'#0f172a',background:'#fff',fontFamily:'inherit',cursor:'pointer',
            appearance:'none',
            backgroundImage:'url(\'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="%2364748b" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="m6 9 6 6 6-6"/></svg>\')',
            backgroundRepeat:'no-repeat',backgroundPosition:'right 8px center',
          }}>
            <option value="1h">1 час</option>
            <option value="24h">24 часа</option>
            <option value="72h">3 дня</option>
            <option value="168h">7 дней</option>
          </select>
        </div>
        <label style={{display:'inline-flex',alignItems:'center',gap:8,fontSize:12.5,color:'#475569',cursor:'pointer'}}>
          <input type="checkbox" defaultChecked style={{width:14,height:14,accentColor:'#4f46e5'}}/>
          Напомнить за 1 час до истечения
        </label>
      </div>
    </Card>
  );
}

// — SBP QR panel —
function SbpPanel({amount}) {
  return (
    <Card style={{padding:0,overflow:'hidden'}}>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr'}}>
        <div style={{padding:'22px 24px',borderRight:'1px solid #eef2f7'}}>
          <div style={{display:'inline-flex',alignItems:'center',gap:8,marginBottom:12}}>
            <SbpLogo/>
            <span style={{fontSize:11,color:'#64748b',fontWeight:600,letterSpacing:'0.05em'}}>СИСТЕМА БЫСТРЫХ ПЛАТЕЖЕЙ</span>
          </div>
          <div style={{fontSize:18,fontWeight:700,color:'#0f172a',marginBottom:6}}>Покажите QR-код студенту</div>
          <div style={{fontSize:13,color:'#64748b',lineHeight:1.55,marginBottom:16}}>
            Студент сканирует код в&nbsp;любом банковском приложении. Комиссия — 0,4% (вместо 1,5–2% по&nbsp;карте).
          </div>
          <div style={{display:'flex',flexDirection:'column',gap:6,fontSize:13}}>
            <Row k="Получатель" v="ООО «Школа Edvantix»"/>
            <Row k="Банк" v="Тинькофф Бизнес"/>
            <Row k="Сумма" v={fmtRub(amount)} mono/>
            <Row k="Назначение" v="Оплата курса · INV-2026-0248" muted/>
          </div>
          <div style={{
            marginTop:16,padding:'10px 12px',background:'#fef3c7',borderRadius:10,
            color:'#92400e',fontSize:12,display:'flex',gap:8,
          }}>
            <Icon.Clock size={14}/>
            <span>QR-код действителен 15&nbsp;минут. Платёж зафиксируется автоматически.</span>
          </div>
        </div>
        <div style={{
          padding:'22px',display:'flex',alignItems:'center',justifyContent:'center',
          background:'radial-gradient(circle at 50% 30%, #fff7ed 0%, #fff 70%)',
        }}>
          <div style={{
            padding:18,borderRadius:18,background:'#fff',
            boxShadow:'0 20px 50px -20px rgba(0,0,0,0.18), 0 0 0 1px #f1f5f9',
            textAlign:'center',
          }}>
            <QrCode/>
            <div style={{marginTop:12,fontSize:11,color:'#94a3b8',fontWeight:600,letterSpacing:'0.06em'}}>
              ID С1B19248C7F40824
            </div>
            <div style={{marginTop:10,display:'flex',gap:6,justifyContent:'center'}}>
              <Button variant="secondary" size="sm" style={{fontSize:11.5}}><Icon.Download size={12}/>PNG</Button>
              <Button variant="secondary" size="sm" style={{fontSize:11.5}}><Icon.Printer size={12}/>Печать</Button>
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
}

function Row({k, v, mono, muted}) {
  return (
    <div style={{display:'flex',justifyContent:'space-between',gap:10}}>
      <span style={{color:'#94a3b8'}}>{k}</span>
      <span style={{
        color: muted?'#94a3b8':'#0f172a',
        fontWeight:500,textAlign:'right',
        fontVariantNumeric: mono?'tabular-nums':'normal',
      }}>{v}</span>
    </div>
  );
}

function SbpLogo() {
  // Stylised SBP-like glyph
  return (
    <svg width="28" height="28" viewBox="0 0 32 32">
      <defs>
        <linearGradient id="sbp" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor="#5b30b8"/>
          <stop offset="50%" stopColor="#ee2a7b"/>
          <stop offset="100%" stopColor="#f59e0b"/>
        </linearGradient>
      </defs>
      <path d="M16 3 L27 9.5 V22.5 L16 29 L5 22.5 V9.5 Z" fill="none" stroke="url(#sbp)" strokeWidth="2.2"/>
      <path d="M11 11 L16 16 L11 21 M21 11 L16 16 L21 21" fill="none" stroke="url(#sbp)" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round"/>
    </svg>
  );
}

function QrCode() {
  // Static placeholder QR (deterministic random-looking grid)
  const size = 25; const cells = [];
  const pattern = [];
  // simple LCG so it looks like real QR not random per render
  let s = 7;
  for (let y=0;y<size;y++) {
    pattern[y]=[];
    for (let x=0;x<size;x++) {
      s = (s*9301 + 49297) % 233280;
      pattern[y][x] = (s/233280) > 0.52 ? 1 : 0;
    }
  }
  // finder squares
  const finder = (cx,cy) => {
    for (let y=0;y<7;y++) for (let x=0;x<7;x++) {
      const v = (x===0||y===0||x===6||y===6) ? 1 : (x>=2&&x<=4&&y>=2&&y<=4 ? 1 : 0);
      pattern[cy+y][cx+x] = v;
    }
  };
  finder(0,0); finder(size-7,0); finder(0,size-7);
  for (let y=0;y<size;y++) for (let x=0;x<size;x++) {
    if (pattern[y][x]) cells.push(<rect key={`${x}-${y}`} x={x*7} y={y*7} width={7} height={7} fill="#0f172a"/>);
  }
  return (
    <div style={{position:'relative',display:'inline-block'}}>
      <svg width="175" height="175" viewBox={`0 0 ${size*7} ${size*7}`} style={{display:'block'}}>
        <rect width={size*7} height={size*7} fill="#fff"/>
        {cells}
      </svg>
      <div style={{
        position:'absolute',top:'50%',left:'50%',transform:'translate(-50%,-50%)',
        width:36,height:36,borderRadius:8,background:'#fff',padding:5,
        boxShadow:'0 2px 6px rgba(0,0,0,0.12)',
        display:'inline-flex',alignItems:'center',justifyContent:'center',
      }}>
        <SbpLogo/>
      </div>
    </div>
  );
}

// — Cash panel —
function CashPanel({amount}) {
  const [given, setGiven] = React.useState(20000);
  const change = Math.max(0, given - amount);
  return (
    <Card style={{padding:'20px 22px'}}>
      <div style={{display:'flex',alignItems:'center',gap:12,marginBottom:14}}>
        <div style={{
          width:40,height:40,borderRadius:10,background:'#d1fae5',color:'#047857',
          display:'inline-flex',alignItems:'center',justifyContent:'center',
        }}><Icon.Banknote size={20}/></div>
        <div>
          <div style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>Внесение наличных</div>
          <div style={{fontSize:12.5,color:'#64748b'}}>Чек 54-ФЗ будет сформирован автоматически</div>
        </div>
      </div>

      <div style={{
        display:'grid',gridTemplateColumns:'1fr 1fr 1fr',gap:14,
        padding:'16px 18px',background:'#f8fafc',borderRadius:12,border:'1px solid #eef2f7',
      }}>
        <div>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,letterSpacing:'0.06em',textTransform:'uppercase',marginBottom:6}}>К оплате</div>
          <div style={{fontSize:20,fontWeight:700,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>{fmtRub(amount)}</div>
        </div>
        <div>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,letterSpacing:'0.06em',textTransform:'uppercase',marginBottom:6}}>Внесено</div>
          <div style={{position:'relative'}}>
            <span style={{position:'absolute',left:10,top:'50%',transform:'translateY(-50%)',color:'#94a3b8'}}>₽</span>
            <Input value={new Intl.NumberFormat('ru-RU').format(given)}
              onChange={e=>setGiven(+e.target.value.replace(/\D/g,'')||0)}
              style={{paddingLeft:24,fontVariantNumeric:'tabular-nums',fontWeight:700,fontSize:18,padding:'6px 10px 6px 24px'}}/>
          </div>
        </div>
        <div>
          <div style={{fontSize:11,color:'#94a3b8',fontWeight:600,letterSpacing:'0.06em',textTransform:'uppercase',marginBottom:6}}>Сдача</div>
          <div style={{fontSize:20,fontWeight:700,color: change>0?'#4f46e5':'#cbd5e1',fontVariantNumeric:'tabular-nums'}}>{fmtRub(change)}</div>
        </div>
      </div>

      <div style={{display:'flex',gap:6,marginTop:12,flexWrap:'wrap'}}>
        {[amount, 20000, 25000, 30000, 50000].map((v,i)=>(
          <button key={i} onClick={()=>setGiven(v)} style={{
            padding:'6px 12px',borderRadius:8,border:'1px solid #e2e8f0',background:'#fff',
            fontSize:12,color:'#475569',fontFamily:'inherit',cursor:'pointer',fontVariantNumeric:'tabular-nums',
            fontWeight:500,
          }}>{i===0?'Без сдачи':fmtRub(v)}</button>
        ))}
      </div>

      <div style={{
        marginTop:14,padding:'10px 12px',borderRadius:10,
        background:'#fef3c7',color:'#92400e',fontSize:12,display:'flex',gap:8,
      }}>
        <Icon.AlertCircle size={14}/>
        <span>Внесение наличных требует подтверждения старшего менеджера, если сумма&nbsp;&gt;&nbsp;₽&nbsp;100&nbsp;000.</span>
      </div>
    </Card>
  );
}

// — Bank transfer panel —
function TransferPanel({amount, student}) {
  const fields = [
    {k:'Получатель', v:'ООО «Школа Edvantix»'},
    {k:'ИНН',        v:'7708123456',         mono:true, copy:true},
    {k:'КПП',        v:'770801001',          mono:true, copy:true},
    {k:'Расчётный счёт', v:'40702 810 1 38000 123456', mono:true, copy:true},
    {k:'Банк',       v:'ПАО «Сбербанк», г.&nbsp;Москва'},
    {k:'БИК',        v:'044525225', mono:true, copy:true},
    {k:'Корр. счёт', v:'30101 810 4 00000 000225', mono:true, copy:true},
  ];
  return (
    <Card style={{padding:'18px 22px'}}>
      <div style={{display:'flex',alignItems:'center',justifyContent:'space-between',gap:10,marginBottom:14}}>
        <div>
          <div style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>Реквизиты для перевода</div>
          <div style={{fontSize:12,color:'#94a3b8',marginTop:2}}>Будут отправлены студенту вместе со счётом</div>
        </div>
        <Button variant="secondary" size="sm"><Icon.Download size={13}/>Счёт PDF</Button>
      </div>
      <div style={{
        display:'grid',gridTemplateColumns:'1fr 1fr',gap:'0 28px',
        padding:'4px 0',
      }}>
        {fields.map((f,i)=>(
          <div key={i} style={{
            display:'flex',justifyContent:'space-between',alignItems:'baseline',gap:10,
            padding:'10px 0',borderBottom:'1px dashed #eef2f7',
          }}>
            <span style={{fontSize:12.5,color:'#64748b'}}>{f.k}</span>
            <span style={{
              fontSize:13,fontWeight:500,color:'#0f172a',textAlign:'right',
              fontFamily: f.mono?'ui-monospace, "SF Mono", Menlo, monospace':'inherit',
              fontVariantNumeric: f.mono?'tabular-nums':'normal',
            }} dangerouslySetInnerHTML={{__html:f.v}}/>
          </div>
        ))}
      </div>
      <div style={{
        marginTop:14,padding:'12px 14px',borderRadius:10,
        background:'#eff6ff',border:'1px solid #dbeafe',
        display:'flex',gap:10,
      }}>
        <Icon.Info size={16} stroke="#1d4ed8" style={{flexShrink:0,marginTop:1}}/>
        <div style={{fontSize:12.5,color:'#1e3a8a',lineHeight:1.5}}>
          <strong>Назначение платежа:</strong> «Оплата по&nbsp;счёту № INV-2026-0248 от&nbsp;{new Date().toLocaleDateString('ru-RU')} за&nbsp;курс “Дизайн 3D-моделей”. Без&nbsp;НДС.»
        </div>
      </div>
    </Card>
  );
}

// ── Step 5. Documents ─────────────────────────────────────────────────
function DocumentsBlock({docs, setDocs, mode, student}) {
  const items = [
    {id:'receipt',  label:'Кассовый чек (54-ФЗ)', sub:'Отправим на email и в ОФД', icon:'Receipt', required:true},
    {id:'invoice',  label:'Счёт на оплату',        sub:'PDF со штампом и подписью', icon:'FileText'},
    {id:'contract', label:'Договор-оферта',         sub:'Если ещё не подписан',     icon:'FileText'},
    {id:'notify',   label:'Уведомить студента',     sub: mode==='request' ? 'Сразу после отправки' : 'После зачисления платежа', icon:'Mail'},
  ];
  return (
    <Card style={{padding:6}}>
      {items.map((it,i)=>{
        const on = docs[it.id];
        const Ic = Icon[it.icon];
        const disabled = it.required;
        return (
          <label key={it.id} style={{
            display:'flex',alignItems:'center',gap:12,padding:'12px 14px',borderRadius:10,
            cursor: disabled?'default':'pointer',
            background: i%2===0?'transparent':'#fafbfc',
          }}>
            <div style={{
              width:36,height:36,borderRadius:8,
              background: on?'#eef2ff':'#f1f5f9',
              color: on?'#4f46e5':'#94a3b8',
              display:'inline-flex',alignItems:'center',justifyContent:'center',flexShrink:0,
            }}><Ic size={16}/></div>
            <div style={{flex:1,minWidth:0}}>
              <div style={{display:'flex',alignItems:'center',gap:8}}>
                <span style={{fontSize:13.5,fontWeight:600,color:'#0f172a'}}>{it.label}</span>
                {it.required && <Badge variant="default" style={{fontSize:10}}>Обязательно</Badge>}
              </div>
              <div style={{fontSize:12,color:'#94a3b8',marginTop:2}}>{it.sub}</div>
            </div>
            <Switch on={on} disabled={disabled} onChange={()=>setDocs(d=>({...d,[it.id]:!d[it.id]}))}/>
          </label>
        );
      })}
    </Card>
  );
}

function Switch({on, onChange, disabled}) {
  return (
    <button onClick={disabled?undefined:onChange} disabled={disabled} style={{
      width:38,height:22,borderRadius:9999,border:0,padding:2,
      background: on ? '#4f46e5' : '#cbd5e1',
      cursor: disabled?'not-allowed':'pointer',position:'relative',transition:'.15s',
      opacity: disabled?0.7:1,flexShrink:0,
    }}>
      <span style={{
        position:'absolute',top:2,left: on?18:2,width:18,height:18,borderRadius:9999,
        background:'#fff',boxShadow:'0 1px 3px rgba(0,0,0,0.2)',transition:'.15s',
      }}/>
    </button>
  );
}

// ── Right column: live receipt + CTA ──────────────────────────────────
function ReceiptSidebar({student, course, subtotal, discount, total, method, mode, useCustom, custom}) {
  const finalTotal = useCustom ? custom : total;
  const methodLabel = METHODS.find(m=>m.id===method)?.label || '—';
  const cta = mode==='request'
    ? { label:'Отправить ссылку на оплату', icon:'Send' }
    : method==='terminal' ? { label:'Отправить на терминал', icon:'ArrowRight' }
    : method==='cash'     ? { label:'Подтвердить приём наличных', icon:'Check' }
    : method==='sbp'      ? { label:'Показать QR студенту', icon:'Smartphone' }
    : method==='transfer' ? { label:'Сформировать счёт', icon:'FileText' }
    : { label:'Принять платёж', icon:'Check' };
  const CtaIc = Icon[cta.icon];

  return (
    <div style={{
      position:'sticky',top:18,display:'flex',flexDirection:'column',gap:14,
    }}>
      <Card style={{padding:0,overflow:'hidden'}}>
        {/* Receipt header */}
        <div style={{
          padding:'18px 22px 14px',
          background:'linear-gradient(135deg,#ffffff 0%, #f5f3ff 70%, #eef2ff 100%)',
          borderBottom:'1px dashed #e2e8f0',
        }}>
          <div style={{
            display:'flex',alignItems:'center',justifyContent:'space-between',gap:8,marginBottom:10,
          }}>
            <div style={{fontSize:11,color:'#94a3b8',fontWeight:700,letterSpacing:'0.08em',textTransform:'uppercase'}}>
              {mode==='request' ? 'Счёт студенту' : 'Квитанция'}
            </div>
            <span style={{fontSize:11.5,color:'#94a3b8',fontVariantNumeric:'tabular-nums'}}>INV-2026-0248</span>
          </div>
          <div style={{
            fontSize:36,fontWeight:800,letterSpacing:'-0.03em',color:'#0f172a',
            fontVariantNumeric:'tabular-nums',lineHeight:1,
          }}>{fmtRub(finalTotal)}</div>
          <div style={{fontSize:12.5,color:'#64748b',marginTop:8}}>
            {mode==='request' ? 'Будет отправлено сейчас' : 'Будет зачислено сейчас'} · {new Date().toLocaleDateString('ru-RU',{day:'numeric',month:'long',year:'numeric'})}
          </div>
        </div>

        {/* Receipt body */}
        <div style={{padding:'14px 22px 4px',display:'flex',flexDirection:'column',gap:10,fontSize:13}}>
          <ReceiptLine k="Студент" v={student?.name || <span style={{color:'#cbd5e1'}}>не выбран</span>}/>
          {student && <ReceiptLine k="ID / группа" v={`${student.id} · ${student.group}`} small/>}
          <ReceiptLine k="Курс"    v={course?.name || <span style={{color:'#cbd5e1'}}>не выбран</span>}/>
          {course && <ReceiptLine k="Период · уроки" v={`${course.period} · ${course.lessons} ур.`} small/>}
          <ReceiptLine k="Метод"   v={methodLabel}/>
        </div>

        <div style={{padding:'10px 22px 12px',borderTop:'1px dashed #eef2f7',marginTop:8}}>
          <SumRow k="Подытог" v={fmtRub(subtotal)}/>
          {discount>0 && <SumRow k="Скидка" v={`−${fmtRub(discount)}`} valueColor="#059669"/>}
          <SumRow k="НДС" v="—" muted/>
        </div>

        <div style={{
          padding:'14px 22px 18px',background:'#f8fafc',borderTop:'1px solid #eef2f7',
          display:'flex',alignItems:'baseline',justifyContent:'space-between',
        }}>
          <span style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>Итого к&nbsp;оплате</span>
          <span style={{
            fontSize:22,fontWeight:800,color:'#0f172a',letterSpacing:'-0.02em',
            fontVariantNumeric:'tabular-nums',
          }}>{fmtRub(finalTotal)}</span>
        </div>
      </Card>

      {/* What happens next */}
      <Card style={{padding:'14px 18px'}}>
        <div style={{fontSize:11,color:'#94a3b8',fontWeight:700,letterSpacing:'0.08em',textTransform:'uppercase',marginBottom:10}}>
          После подтверждения
        </div>
        <div style={{display:'flex',flexDirection:'column',gap:9,fontSize:12.5,color:'#475569'}}>
          {(mode==='request' ? [
            'Студент получит ссылку через выбранные каналы',
            'Платёж появится в реестре со&nbsp;статусом «Ожидает оплаты»',
            'После оплаты — авто-чек в&nbsp;ОФД и&nbsp;уведомление менеджера',
          ] : [
            'Платёж зачислится в&nbsp;реестр и&nbsp;на&nbsp;баланс студента',
            'Кассовый чек уйдёт в&nbsp;ОФД и&nbsp;на&nbsp;email студенту',
            'Студенту откроется доступ к&nbsp;материалам курса',
          ]).map((t,i)=>(
            <div key={i} style={{display:'flex',gap:8}}>
              <Icon.Check size={14} stroke="#10b981" style={{flexShrink:0,marginTop:1}}/>
              <span dangerouslySetInnerHTML={{__html:t}}/>
            </div>
          ))}
        </div>
      </Card>

      {/* CTA */}
      <Button variant="primary" size="lg" style={{
        width:'100%',padding:'14px 18px',fontSize:15,borderRadius:12,
        boxShadow:'0 8px 24px -8px rgba(79,70,229,0.45), 0 0 0 1px rgba(79,70,229,0.04)',
      }}>
        <CtaIc size={16}/>{cta.label} · {fmtRub(finalTotal)}
      </Button>
      <button style={{
        padding:'10px 14px',borderRadius:10,border:'1px solid #e2e8f0',background:'#fff',
        color:'#64748b',fontSize:13,fontWeight:500,fontFamily:'inherit',cursor:'pointer',
      }}>Сохранить как черновик</button>

      <div style={{
        display:'flex',alignItems:'center',gap:8,fontSize:11.5,color:'#94a3b8',padding:'4px 6px',
      }}>
        <Icon.ShieldCheck size={13} stroke="#10b981"/>
        152-ФЗ · 54-ФЗ · PCI&nbsp;DSS L1
      </div>
    </div>
  );
}

function ReceiptLine({k, v, small}) {
  return (
    <div style={{display:'flex',justifyContent:'space-between',gap:12,fontSize: small?12:13}}>
      <span style={{color: small?'#cbd5e1':'#94a3b8'}}>{k}</span>
      <span style={{
        color: small?'#94a3b8':'#0f172a',fontWeight: small?500:600,
        textAlign:'right',maxWidth:'62%',
      }}>{v}</span>
    </div>
  );
}
function SumRow({k, v, valueColor, muted}) {
  return (
    <div style={{
      display:'flex',justifyContent:'space-between',padding:'4px 0',
      fontSize:13,color: muted?'#94a3b8':'#475569',
    }}>
      <span>{k}</span>
      <span style={{
        fontWeight:600,color: valueColor || (muted?'#94a3b8':'#0f172a'),
        fontVariantNumeric:'tabular-nums',
      }}>{v}</span>
    </div>
  );
}

// ── Main page ──────────────────────────────────────────────────────────
function PaymentCreate({mode}) {
  const [student, setStudent] = React.useState(RECENT_STUDENTS[0]);
  const [course, setCourse]   = React.useState(COURSE_OPTIONS[0]);
  const [discountId, setDiscountId] = React.useState('loyal');
  const [method, setMethod]   = React.useState(mode==='request' ? 'link' : 'terminal');
  const [useCustom, setUseCustom] = React.useState(false);
  const [custom, setCustom]   = React.useState(15000);
  const [docs, setDocs] = React.useState({receipt:true, invoice:true, contract:false, notify:true});

  // when switching mode, default the method appropriately
  React.useEffect(()=>{
    if (mode==='request' && !['link','sbp','transfer'].includes(method)) setMethod('link');
    if (mode==='accept'  && !['terminal','cash','sbp'].includes(method)) setMethod('terminal');
  }, [mode]);

  const subtotal = course ? course.lessons * course.pricePerLesson : 0;
  const preset = DISCOUNT_PRESETS.find(d=>d.id===discountId);
  const discount = Math.round(subtotal * preset.pct / 100);
  const total = subtotal - discount;

  return (
    <div data-screen-label="Принять платёж" className="pc-grid" style={{
      padding:'24px 28px 64px',display:'grid',gap:24,
      gridTemplateColumns:'minmax(0,1fr) 340px',
      background:'#f8fafc',minHeight:'100%',
    }}>
      <div style={{display:'flex',flexDirection:'column',gap:26,minWidth:0}}>
        <Section n={1} title="Студент" hint="От кого принимаем оплату" complete={!!student}>
          <StudentPicker student={student} onPick={setStudent} onClear={()=>setStudent(null)}/>
        </Section>

        <Section n={2} title="За что оплата" hint="Курс, пакет занятий или индивидуальный тариф" complete={!!course}
          action={<button style={{fontSize:12.5,color:'#4f46e5',fontWeight:600,background:'none',border:0,cursor:'pointer',fontFamily:'inherit'}}>+ Произвольная услуга</button>}>
          <CoursePicker course={course} onPick={setCourse}/>
        </Section>

        <Section n={3} title="Сумма и скидка" complete={total>0}>
          <AmountBlock
            subtotal={subtotal}
            discountId={discountId} onDiscount={setDiscountId}
            custom={custom} onCustom={setCustom}
            useCustom={useCustom} setUseCustom={setUseCustom}
          />
        </Section>

        <Section n={4} title="Способ оплаты"
          hint={mode==='request' ? 'Студент оплатит через ссылку' : 'Где и как студент платит сейчас'}
          complete={!!method}>
          <MethodTabs method={method} onMethod={setMethod}/>
          {method==='terminal' && <TerminalPanel amount={useCustom?custom:total}/>}
          {method==='link'     && <LinkPanel amount={useCustom?custom:total} student={student}/>}
          {method==='sbp'      && <SbpPanel amount={useCustom?custom:total}/>}
          {method==='cash'     && <CashPanel amount={useCustom?custom:total}/>}
          {method==='transfer' && <TransferPanel amount={useCustom?custom:total} student={student}/>}
        </Section>

        <Section n={5} title="Документы и уведомления" hint="Что отправить и сформировать">
          <DocumentsBlock docs={docs} setDocs={setDocs} mode={mode} student={student}/>
        </Section>
      </div>

      <ReceiptSidebar
        student={student} course={course}
        subtotal={subtotal} discount={discount} total={total}
        method={method} mode={mode}
        useCustom={useCustom} custom={custom}
      />

      <style>{`
        @keyframes ev-spin { to { transform: rotate(360deg) } }
        @media (max-width: 1100px) {
          .pc-grid { grid-template-columns: minmax(0,1fr) !important; }
          .pc-grid > div:last-child { position: static !important; }
        }
      `}</style>
    </div>
  );
}

window.PaymentCreate = PaymentCreate;
