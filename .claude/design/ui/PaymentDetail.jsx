// ── Payment Detail page ───────────────────────────────────────────────
// Detail view for a single payment. Light theme, indigo brand.

const PAYMENT_DETAIL = {
  id: 'INV-2026-0247',
  status: 'paid',
  amount: 18900,
  subtotal: 21000,
  discount: 2100,
  discountLabel: 'Скидка постоянного студента (10%)',
  vat: 'Без НДС (УСН)',
  createdAt: '2026-05-21T11:35:14',
  paidAt: '2026-05-21T11:42:08',
  dueAt: '2026-05-25T23:59:00',
  student: {
    name: 'Иван Козлов',
    email: 'i.kozlov@mail.ru',
    phone: '+7 (916) 234-15-08',
    city: 'Москва',
    studentId: 'STU-1782',
    group: 'Дизайн-3 (поток D3-2026)',
    totalPaid: 84700,
    paymentsCount: 5,
    since: 'январь 2026',
  },
  course: {
    name: 'Дизайн 3D-моделей',
    plan: 'Стандарт',
    period: 'Июнь — Август 2026',
    lessons: 24,
    pricePerLesson: 875,
    teacher: 'Михаил Гончаров',
  },
  acquirer: {
    method: 'Visa •• 4421',
    bank: 'Сбербанк-Эквайринг',
    txId: '8347-2026-05-21-001',
    rrn: '234562782342',
    auth: '458921',
    secure3d: true,
    ip: '5.61.236.18',
    device: 'iPhone 14, Safari',
    geo: 'Москва, СНГ',
  },
  manager: {
    name: 'Анна Мельникова',
    role: 'Менеджер по работе со студентами',
    email: 'a.melnikova@school.ru',
  },
  documents: [
    { kind:'invoice',  title:'Счёт № INV-2026-0247',         meta:'PDF · 86 KB',  badge:null },
    { kind:'receipt',  title:'Кассовый чек',                  meta:'ФД 4423 · ФП 2873912348', badge:'ОФД' },
    { kind:'contract', title:'Договор-оферта A-1209/2026',    meta:'PDF · 142 KB', badge:null },
    { kind:'act',      title:'Акт выполненных работ',         meta:'будет сформирован после окончания курса', badge:'ожидается', disabled:true },
  ],
  events: [
    { time:'2026-05-21T11:42:08', kind:'success', title:'Платёж зачислен',         body:'Средства поступили на расчётный счёт школы.' },
    { time:'2026-05-21T11:42:05', kind:'check',   title:'Транзакция подтверждена', body:'3-D Secure пройдено, авторизация банка получена.' },
    { time:'2026-05-21T11:41:30', kind:'card',    title:'Оплата картой Visa •• 4421', body:'Сумма ₽\u00A018\u00A0900. Сбербанк-Эквайринг.' },
    { time:'2026-05-21T11:38:00', kind:'mail',    title:'Счёт отправлен на email',  body:'i.kozlov@mail.ru · ссылка на оплату действительна 24 часа.' },
    { time:'2026-05-21T11:35:14', kind:'doc',     title:'Счёт сформирован',         body:'Менеджер Анна Мельникова. Применена скидка 10%.' },
    { time:'2026-05-21T11:34:00', kind:'user',    title:'Студент выбрал тариф «Стандарт»', body:'24 урока · период Июнь — Август 2026.' },
  ],
};

// — formatters reused —
const _fmtRub = (n) => '₽\u00A0' + new Intl.NumberFormat('ru-RU').format(n);
const _fmtDateLong = (iso) => {
  const d = new Date(iso);
  const months = ['января','февраля','марта','апреля','мая','июня','июля','августа','сентября','октября','ноября','декабря'];
  return `${d.getDate()} ${months[d.getMonth()]} ${d.getFullYear()}`;
};
const _fmtTime = (iso) => {
  const d = new Date(iso);
  return `${String(d.getHours()).padStart(2,'0')}:${String(d.getMinutes()).padStart(2,'0')}:${String(d.getSeconds()).padStart(2,'0')}`;
};

// — small UI parts —
function FieldRow({label, value, mono, copy, hint}) {
  const [copied, setCopied] = React.useState(false);
  const doCopy = () => {
    navigator.clipboard?.writeText(String(value));
    setCopied(true); setTimeout(()=>setCopied(false), 1200);
  };
  return (
    <div style={{
      display:'grid',gridTemplateColumns:'160px 1fr',gap:16,alignItems:'baseline',
      padding:'10px 0',borderBottom:'1px dashed #eef2f7',
    }}>
      <div style={{fontSize:12.5,color:'#64748b'}}>{label}</div>
      <div style={{display:'flex',alignItems:'center',gap:8,flexWrap:'wrap'}}>
        <span style={{
          fontSize:13.5,color:'#0f172a',fontWeight:500,
          fontVariantNumeric: mono?'tabular-nums':'normal',
          fontFamily: mono?'ui-monospace, "JetBrains Mono", "SF Mono", Menlo, monospace':'inherit',
        }}>{value}</span>
        {hint && <span style={{fontSize:11.5,color:'#94a3b8'}}>{hint}</span>}
        {copy && (
          <button onClick={doCopy}
            style={{
              border:'1px solid transparent',background:'transparent',color:'#94a3b8',
              borderRadius:6,padding:'2px 6px',cursor:'pointer',
              display:'inline-flex',alignItems:'center',gap:4,fontSize:11,fontFamily:'inherit',
            }}
            onMouseEnter={e=>{e.currentTarget.style.background='#f1f5f9';e.currentTarget.style.color='#475569'}}
            onMouseLeave={e=>{e.currentTarget.style.background='transparent';e.currentTarget.style.color='#94a3b8'}}>
            {copied ? <><Icon.Check size={11} stroke="#10b981"/><span style={{color:'#10b981'}}>Скопировано</span></> : <><Icon.Copy size={11}/>Копировать</>}
          </button>
        )}
      </div>
    </div>
  );
}

function CardHeader({title, hint, action}) {
  return (
    <div style={{
      display:'flex',alignItems:'center',justifyContent:'space-between',gap:12,
      padding:'16px 22px 14px',borderBottom:'1px solid #eef2f7',
    }}>
      <div>
        <div style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>{title}</div>
        {hint && <div style={{fontSize:12,color:'#94a3b8',marginTop:2}}>{hint}</div>}
      </div>
      {action}
    </div>
  );
}

// — Hero summary card —
function PaymentHero({p}) {
  return (
    <Card style={{padding:0,overflow:'hidden'}}>
      <div style={{
        display:'grid',gridTemplateColumns:'1.4fr 1fr',
        background:'linear-gradient(135deg, #ffffff 0%, #f5f3ff 65%, #eef2ff 100%)',
      }}>
        {/* — left: amount + meta — */}
        <div style={{padding:'28px 28px 24px',borderRight:'1px solid #eef2f7'}}>
          <div style={{display:'flex',alignItems:'center',gap:8,marginBottom:12}}>
            <span style={{
              display:'inline-flex',alignItems:'center',gap:6,
              background:'#d1fae5',color:'#047857',padding:'4px 10px',borderRadius:9999,
              fontSize:12,fontWeight:600,
            }}>
              <Icon.CheckCircle2 size={13}/>Оплачен полностью
            </span>
            <span style={{fontSize:11.5,color:'#94a3b8',fontVariantNumeric:'tabular-nums'}}>
              {p.id}
            </span>
          </div>
          <div style={{
            fontSize:48,fontWeight:800,letterSpacing:'-0.03em',lineHeight:1,
            color:'#0f172a',fontVariantNumeric:'tabular-nums',
          }}>{_fmtRub(p.amount)}</div>
          <div style={{fontSize:13.5,color:'#475569',marginTop:10,maxWidth:420}}>
            Поступил <strong>{_fmtDateLong(p.paidAt)} в {_fmtTime(p.paidAt).slice(0,5)}</strong> через {p.acquirer.bank}.
            Спустя 7&nbsp;минут после выставления счёта.
          </div>
          <div style={{display:'flex',alignItems:'center',gap:18,marginTop:18,paddingTop:16,borderTop:'1px solid #eef2f7'}}>
            <MiniStat label="Подытог"   value={_fmtRub(p.subtotal)}/>
            <MiniDivider/>
            <MiniStat label="Скидка"    value={'−' + _fmtRub(p.discount)} valueColor="#059669"/>
            <MiniDivider/>
            <MiniStat label="НДС"       value="—" hint={p.vat}/>
            <MiniDivider/>
            <MiniStat label="К оплате"  value={_fmtRub(p.amount)} accent/>
          </div>
        </div>
        {/* — right: method tile — */}
        <div style={{padding:'28px 28px 24px',display:'flex',flexDirection:'column',gap:14}}>
          <div style={{fontSize:12,fontWeight:600,color:'#64748b',textTransform:'uppercase',letterSpacing:'0.08em'}}>
            Способ оплаты
          </div>
          <div style={{
            border:'1px solid #e2e8f0',borderRadius:14,padding:'16px 18px',
            background:'#fff',display:'flex',flexDirection:'column',gap:14,
            boxShadow:'0 1px 2px rgba(15,23,42,0.04)',
          }}>
            <div style={{display:'flex',alignItems:'center',gap:12}}>
              <div style={{
                width:48,height:32,borderRadius:6,background:'linear-gradient(135deg,#1a1f71,#2a5298)',
                display:'inline-flex',alignItems:'center',justifyContent:'center',color:'#fff',
                fontWeight:700,fontStyle:'italic',fontSize:13,letterSpacing:'-0.02em',
                boxShadow:'0 2px 8px rgba(26,31,113,0.25)',
              }}>VISA</div>
              <div style={{flex:1,minWidth:0}}>
                <div style={{fontSize:14,fontWeight:600,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>
                  •• •• •• 4421
                </div>
                <div style={{fontSize:11.5,color:'#94a3b8'}}>{p.acquirer.bank}</div>
              </div>
              <div style={{
                display:'inline-flex',alignItems:'center',gap:4,
                fontSize:11,color:'#047857',background:'#d1fae5',padding:'3px 8px',borderRadius:9999,fontWeight:600,
              }}>
                <Icon.Lock size={10}/>3-D Secure
              </div>
            </div>
            <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:12,fontSize:12}}>
              <div>
                <div style={{color:'#94a3b8',marginBottom:2}}>RRN</div>
                <div style={{color:'#0f172a',fontFamily:'ui-monospace, "SF Mono", Menlo, monospace',fontWeight:500}}>{p.acquirer.rrn}</div>
              </div>
              <div>
                <div style={{color:'#94a3b8',marginBottom:2}}>Auth-код</div>
                <div style={{color:'#0f172a',fontFamily:'ui-monospace, "SF Mono", Menlo, monospace',fontWeight:500}}>{p.acquirer.auth}</div>
              </div>
            </div>
          </div>
          <div style={{display:'flex',alignItems:'center',gap:6,fontSize:12,color:'#94a3b8'}}>
            <Icon.ShieldCheck size={13} stroke="#10b981"/>
            Безопасно. PCI&nbsp;DSS Level&nbsp;1.
          </div>
        </div>
      </div>
    </Card>
  );
}

function MiniStat({label, value, hint, accent, valueColor}) {
  return (
    <div>
      <div style={{fontSize:11,color:'#94a3b8',marginBottom:4,textTransform:'uppercase',letterSpacing:'0.06em',fontWeight:600}}>{label}</div>
      <div style={{
        fontSize:16,fontWeight:accent?700:600,
        color: valueColor || (accent?'#4f46e5':'#0f172a'),
        fontVariantNumeric:'tabular-nums',letterSpacing:'-0.01em',
      }}>{value}</div>
      {hint && <div style={{fontSize:10.5,color:'#cbd5e1',marginTop:2}}>{hint}</div>}
    </div>
  );
}
const MiniDivider = () => <div style={{width:1,height:30,background:'#e2e8f0'}}/>;

// — Line items table —
function LineItems({p}) {
  return (
    <Card style={{padding:0,overflow:'hidden'}}>
      <CardHeader title="Состав платежа" hint="2 позиции"
        action={<Button variant="ghost" size="sm"><Icon.FileText size={13}/>Открыть полный счёт</Button>}/>
      <table style={{width:'100%',borderCollapse:'collapse'}}>
        <thead>
          <tr>
            {['Описание','Период','Кол-во','Цена','Сумма'].map((h,i)=>(
              <th key={i} style={{
                textAlign: i>=2 ? 'right' : 'left',
                padding:'10px 22px',fontSize:11.5,fontWeight:600,
                color:'#64748b',textTransform:'uppercase',letterSpacing:'0.06em',
                background:'#f8fafc',borderBottom:'1px solid #e2e8f0',
                whiteSpace:'nowrap',
              }}>{h}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          <tr>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9'}}>
              <div style={{display:'flex',alignItems:'flex-start',gap:12}}>
                <div style={{
                  width:36,height:36,borderRadius:8,flexShrink:0,
                  background:'linear-gradient(135deg,#818cf8,#6366f1)',color:'#fff',
                  display:'inline-flex',alignItems:'center',justifyContent:'center',
                }}><Icon.BookOpen size={16}/></div>
                <div>
                  <div style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>
                    Курс «{p.course.name}»
                  </div>
                  <div style={{fontSize:12,color:'#64748b',marginTop:2}}>
                    Тариф «{p.course.plan}» · Преподаватель: {p.course.teacher}
                  </div>
                </div>
              </div>
            </td>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9'}}>
              <span style={{fontSize:13,color:'#334155'}}>{p.course.period}</span>
            </td>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9',textAlign:'right'}}>
              <span style={{fontSize:13,color:'#334155',fontVariantNumeric:'tabular-nums'}}>{p.course.lessons} ур.</span>
            </td>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9',textAlign:'right'}}>
              <span style={{fontSize:13,color:'#334155',fontVariantNumeric:'tabular-nums'}}>{_fmtRub(p.course.pricePerLesson)}</span>
            </td>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9',textAlign:'right'}}>
              <span style={{fontSize:14,fontWeight:600,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>{_fmtRub(p.subtotal)}</span>
            </td>
          </tr>
          <tr>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9'}}>
              <div style={{display:'flex',alignItems:'flex-start',gap:12}}>
                <div style={{
                  width:36,height:36,borderRadius:8,flexShrink:0,
                  background:'#d1fae5',color:'#047857',
                  display:'inline-flex',alignItems:'center',justifyContent:'center',
                }}><Icon.CircleDollarSign size={16}/></div>
                <div>
                  <div style={{fontSize:14,fontWeight:600,color:'#0f172a'}}>{p.discountLabel}</div>
                  <div style={{fontSize:12,color:'#64748b',marginTop:2}}>Применена автоматически. С января 2026.</div>
                </div>
              </div>
            </td>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9'}}>
              <Badge variant="success">−10%</Badge>
            </td>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9'}}/>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9'}}/>
            <td style={{padding:'18px 22px',borderBottom:'1px solid #f1f5f9',textAlign:'right'}}>
              <span style={{fontSize:14,fontWeight:600,color:'#059669',fontVariantNumeric:'tabular-nums'}}>−{_fmtRub(p.discount)}</span>
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr>
            <td colSpan={4} style={{padding:'12px 22px',textAlign:'right',fontSize:13,color:'#64748b'}}>Подытог</td>
            <td style={{padding:'12px 22px',textAlign:'right',fontSize:13,color:'#334155',fontVariantNumeric:'tabular-nums'}}>{_fmtRub(p.subtotal)}</td>
          </tr>
          <tr>
            <td colSpan={4} style={{padding:'4px 22px 12px',textAlign:'right',fontSize:13,color:'#64748b'}}>Скидка</td>
            <td style={{padding:'4px 22px 12px',textAlign:'right',fontSize:13,color:'#059669',fontVariantNumeric:'tabular-nums'}}>−{_fmtRub(p.discount)}</td>
          </tr>
          <tr>
            <td colSpan={4} style={{padding:'4px 22px 12px',textAlign:'right',fontSize:12.5,color:'#94a3b8'}}>{p.vat}</td>
            <td style={{padding:'4px 22px 12px',textAlign:'right',fontSize:12.5,color:'#94a3b8'}}>—</td>
          </tr>
          <tr style={{background:'#f8fafc',borderTop:'2px solid #e2e8f0'}}>
            <td colSpan={4} style={{padding:'16px 22px',textAlign:'right',fontSize:14,fontWeight:600,color:'#0f172a'}}>
              Итого к&nbsp;оплате
            </td>
            <td style={{padding:'16px 22px',textAlign:'right',fontSize:20,fontWeight:700,color:'#0f172a',fontVariantNumeric:'tabular-nums',letterSpacing:'-0.02em'}}>
              {_fmtRub(p.amount)}
            </td>
          </tr>
        </tfoot>
      </table>
    </Card>
  );
}

// — Acquirer details —
function AcquirerCard({p}) {
  const a = p.acquirer;
  return (
    <Card style={{padding:0}}>
      <CardHeader title="Транзакция эквайринга" hint="Полные технические данные платежа"
        action={<Button variant="ghost" size="sm"><Icon.ExternalLink size={13}/>Открыть в банке</Button>}/>
      <div style={{padding:'8px 22px 18px',display:'grid',gridTemplateColumns:'1fr 1fr',gap:'0 36px'}}>
        <FieldRow label="ID транзакции"    value={a.txId} mono copy/>
        <FieldRow label="Платёжная система" value={a.bank}/>
        <FieldRow label="RRN"               value={a.rrn} mono copy/>
        <FieldRow label="Метод"             value={a.method}/>
        <FieldRow label="Auth-код"          value={a.auth} mono copy/>
        <FieldRow label="3-D Secure"        value="Пройдено" hint="MasterCard SecureCode / Visa Secure"/>
        <FieldRow label="IP-адрес"          value={a.ip} mono/>
        <FieldRow label="Устройство"        value={a.device}/>
        <FieldRow label="Геолокация"        value={a.geo}/>
        <FieldRow label="Валюта"            value="RUB · 643 (ISO 4217)"/>
      </div>
    </Card>
  );
}

// — Timeline / event log —
const EVENT_ICONS = {
  success: { icon:'CheckCircle2', bg:'#d1fae5', fg:'#047857' },
  check:   { icon:'ShieldCheck',  bg:'#dbeafe', fg:'#1d4ed8' },
  card:    { icon:'CreditCard',   bg:'#ede9fe', fg:'#6d28d9' },
  mail:    { icon:'Mail',         bg:'#fef3c7', fg:'#b45309' },
  doc:     { icon:'FileText',     bg:'#f1f5f9', fg:'#475569' },
  user:    { icon:'User',         bg:'#fee2e2', fg:'#b91c1c' },
};

function Timeline({events}) {
  return (
    <Card style={{padding:0}}>
      <CardHeader title="История события" hint={`${events.length} записей · в обратном хронологическом порядке`}
        action={<Button variant="ghost" size="sm"><Icon.Download size={13}/>Скачать журнал</Button>}/>
      <div style={{padding:'8px 22px 22px'}}>
        {events.map((ev,i)=>{
          const t = EVENT_ICONS[ev.kind] || EVENT_ICONS.doc;
          const Ic = Icon[t.icon];
          const last = i === events.length-1;
          return (
            <div key={i} style={{display:'flex',gap:14,position:'relative'}}>
              <div style={{flexShrink:0,display:'flex',flexDirection:'column',alignItems:'center'}}>
                <div style={{
                  width:32,height:32,borderRadius:9999,background:t.bg,color:t.fg,
                  display:'inline-flex',alignItems:'center',justifyContent:'center',
                  border:'2px solid #fff',boxShadow:'0 0 0 1px '+t.bg,marginTop:8,
                }}><Ic size={14}/></div>
                {!last && <div style={{width:2,flex:1,background:'#e2e8f0',marginTop:4,marginBottom:-2}}/>}
              </div>
              <div style={{flex:1,padding:'12px 0 18px',borderBottom: last?'0':'0'}}>
                <div style={{display:'flex',alignItems:'baseline',justifyContent:'space-between',gap:12}}>
                  <div style={{fontSize:13.5,fontWeight:600,color:'#0f172a'}}>{ev.title}</div>
                  <div style={{fontSize:11.5,color:'#94a3b8',fontVariantNumeric:'tabular-nums',whiteSpace:'nowrap'}}>
                    {_fmtTime(ev.time)}
                  </div>
                </div>
                <div style={{fontSize:13,color:'#64748b',marginTop:3,lineHeight:1.5}}>{ev.body}</div>
              </div>
            </div>
          );
        })}
      </div>
    </Card>
  );
}

// — Right column cards —
function StudentCard({s}) {
  return (
    <Card style={{padding:0,overflow:'hidden'}}>
      <CardHeader title="Студент" action={
        <a href="Member Profile.html" style={{
          fontSize:12,color:'#4f46e5',fontWeight:600,display:'inline-flex',alignItems:'center',gap:4,
        }}>Профиль <Icon.ArrowUpRight size={12}/></a>
      }/>
      <div style={{padding:'18px 22px',display:'flex',flexDirection:'column',gap:14}}>
        <div style={{display:'flex',alignItems:'center',gap:12}}>
          <Avatar name={s.name} size={48}/>
          <div style={{minWidth:0}}>
            <div style={{fontSize:15,fontWeight:600,color:'#0f172a'}}>{s.name}</div>
            <div style={{fontSize:12,color:'#94a3b8',fontVariantNumeric:'tabular-nums'}}>{s.studentId} · с&nbsp;{s.since}</div>
          </div>
        </div>
        <div style={{display:'flex',flexDirection:'column',gap:8,fontSize:13}}>
          <ContactRow icon="Mail"  text={s.email}/>
          <ContactRow icon="Phone" text={s.phone}/>
          <ContactRow icon="MapPin" text={s.city}/>
          <ContactRow icon="Users" text={s.group}/>
        </div>
        <div style={{
          display:'grid',gridTemplateColumns:'1fr 1fr',gap:10,
          padding:'12px 14px',background:'#f8fafc',borderRadius:10,border:'1px solid #eef2f7',
        }}>
          <div>
            <div style={{fontSize:11,color:'#94a3b8',marginBottom:2,fontWeight:600,textTransform:'uppercase',letterSpacing:'0.06em'}}>Оплачено всего</div>
            <div style={{fontSize:15,fontWeight:700,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>{_fmtRub(s.totalPaid)}</div>
          </div>
          <div>
            <div style={{fontSize:11,color:'#94a3b8',marginBottom:2,fontWeight:600,textTransform:'uppercase',letterSpacing:'0.06em'}}>Платежей</div>
            <div style={{fontSize:15,fontWeight:700,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>{s.paymentsCount}</div>
          </div>
        </div>
        <div style={{display:'flex',gap:8}}>
          <Button variant="secondary" size="sm" style={{flex:1}}><Icon.MessageSquare size={13}/>Написать</Button>
          <Button variant="secondary" size="sm" style={{flex:1}}><Icon.Phone size={13}/>Позвонить</Button>
        </div>
      </div>
    </Card>
  );
}

function ContactRow({icon, text}) {
  const Ic = Icon[icon];
  return (
    <div style={{display:'flex',alignItems:'center',gap:10,color:'#334155'}}>
      <Ic size={14} stroke="#94a3b8"/>
      <span style={{minWidth:0,overflow:'hidden',textOverflow:'ellipsis',whiteSpace:'nowrap'}}>{text}</span>
    </div>
  );
}

function CourseCardSide({c}) {
  return (
    <Card style={{padding:0,overflow:'hidden'}}>
      <CardHeader title="Курс" action={
        <a href="Course.html" style={{
          fontSize:12,color:'#4f46e5',fontWeight:600,display:'inline-flex',alignItems:'center',gap:4,
        }}>Открыть <Icon.ArrowUpRight size={12}/></a>
      }/>
      <div style={{
        height:96,position:'relative',
        background:'linear-gradient(135deg,#4f46e5 0%, #7c3aed 60%, #a855f7 100%)',
        overflow:'hidden',
      }}>
        <div style={{
          position:'absolute',inset:0,
          background:'radial-gradient(circle at 80% 20%, rgba(255,255,255,0.18), transparent 60%)',
        }}/>
        <div style={{
          position:'absolute',left:20,bottom:14,color:'#fff',
        }}>
          <div style={{fontSize:11,opacity:0.8,textTransform:'uppercase',letterSpacing:'0.08em',fontWeight:600}}>Тариф «{c.plan}»</div>
          <div style={{fontSize:17,fontWeight:700,marginTop:2}}>{c.name}</div>
        </div>
        <Icon.BookOpen size={64} stroke="rgba(255,255,255,0.18)" style={{position:'absolute',right:-8,top:-8,transform:'rotate(-12deg)'}}/>
      </div>
      <div style={{padding:'14px 22px 18px',display:'flex',flexDirection:'column',gap:10,fontSize:13}}>
        <div style={{display:'flex',justifyContent:'space-between',gap:8}}>
          <span style={{color:'#64748b'}}>Период</span>
          <span style={{color:'#0f172a',fontWeight:500}}>{c.period}</span>
        </div>
        <div style={{display:'flex',justifyContent:'space-between',gap:8}}>
          <span style={{color:'#64748b'}}>Уроков</span>
          <span style={{color:'#0f172a',fontWeight:500,fontVariantNumeric:'tabular-nums'}}>{c.lessons}</span>
        </div>
        <div style={{display:'flex',justifyContent:'space-between',gap:8}}>
          <span style={{color:'#64748b'}}>Преподаватель</span>
          <span style={{color:'#0f172a',fontWeight:500}}>{c.teacher}</span>
        </div>
        <div style={{display:'flex',justifyContent:'space-between',gap:8}}>
          <span style={{color:'#64748b'}}>Цена за урок</span>
          <span style={{color:'#0f172a',fontWeight:500,fontVariantNumeric:'tabular-nums'}}>{_fmtRub(c.pricePerLesson)}</span>
        </div>
      </div>
    </Card>
  );
}

function DocumentsCard({docs}) {
  return (
    <Card style={{padding:0}}>
      <CardHeader title="Связанные документы" hint={`${docs.filter(d=>!d.disabled).length} доступно`}/>
      <div style={{padding:10}}>
        {docs.map((d,i)=>(<DocumentRow key={i} doc={d}/>))}
      </div>
    </Card>
  );
}

function DocumentRow({doc}) {
  const [hover, setHover] = React.useState(false);
  const KIND = {
    invoice:  { icon:'FileText',    bg:'#e0eaff', fg:'#4338ca' },
    receipt:  { icon:'Receipt',     bg:'#d1fae5', fg:'#047857' },
    contract: { icon:'FileText',    bg:'#fef3c7', fg:'#b45309' },
    act:      { icon:'FileText',    bg:'#f1f5f9', fg:'#94a3b8' },
  };
  const k = KIND[doc.kind];
  const Ic = Icon[k.icon];
  return (
    <div
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        display:'flex',alignItems:'center',gap:12,padding:'10px 12px',borderRadius:10,
        cursor: doc.disabled?'default':'pointer',
        background: hover && !doc.disabled ? '#f8fafc' : 'transparent',
        opacity: doc.disabled ? 0.6 : 1, transition:'.1s',
      }}>
      <div style={{
        width:36,height:36,borderRadius:8,background:k.bg,color:k.fg,
        display:'inline-flex',alignItems:'center',justifyContent:'center',flexShrink:0,
      }}><Ic size={16}/></div>
      <div style={{flex:1,minWidth:0}}>
        <div style={{display:'flex',alignItems:'center',gap:6}}>
          <span style={{fontSize:13,fontWeight:600,color:'#0f172a',whiteSpace:'nowrap',overflow:'hidden',textOverflow:'ellipsis'}}>{doc.title}</span>
          {doc.badge && <Badge variant={doc.kind==='receipt'?'success':'default'} style={{fontSize:10,padding:'2px 7px'}}>{doc.badge}</Badge>}
        </div>
        <div style={{fontSize:11.5,color:'#94a3b8',marginTop:2,whiteSpace:'nowrap',overflow:'hidden',textOverflow:'ellipsis'}}>{doc.meta}</div>
      </div>
      {!doc.disabled && (
        <div style={{display:'flex',gap:2,flexShrink:0}}>
          <button title="Скачать" style={{
            width:28,height:28,borderRadius:6,border:'1px solid transparent',
            background:'transparent',color:'#64748b',cursor:'pointer',
            display:'inline-flex',alignItems:'center',justifyContent:'center',
          }}
          onMouseEnter={e=>{e.currentTarget.style.background='#f1f5f9'}}
          onMouseLeave={e=>{e.currentTarget.style.background='transparent'}}>
            <Icon.Download size={14}/>
          </button>
          <button title="Открыть" style={{
            width:28,height:28,borderRadius:6,border:'1px solid transparent',
            background:'transparent',color:'#64748b',cursor:'pointer',
            display:'inline-flex',alignItems:'center',justifyContent:'center',
          }}
          onMouseEnter={e=>{e.currentTarget.style.background='#f1f5f9'}}
          onMouseLeave={e=>{e.currentTarget.style.background='transparent'}}>
            <Icon.ExternalLink size={13}/>
          </button>
        </div>
      )}
    </div>
  );
}

function ManagerCard({m}) {
  return (
    <Card style={{padding:'18px 22px',display:'flex',alignItems:'center',gap:12}}>
      <Avatar name={m.name} size={40}/>
      <div style={{flex:1,minWidth:0}}>
        <div style={{fontSize:11,color:'#94a3b8',textTransform:'uppercase',letterSpacing:'0.06em',fontWeight:600,marginBottom:2}}>
          Ответственный менеджер
        </div>
        <div style={{fontSize:13.5,fontWeight:600,color:'#0f172a'}}>{m.name}</div>
        <div style={{fontSize:12,color:'#64748b'}}>{m.role}</div>
      </div>
      <button title="Написать менеджеру" style={{
        width:32,height:32,borderRadius:8,border:'1px solid #e2e8f0',
        background:'#fff',color:'#475569',cursor:'pointer',
        display:'inline-flex',alignItems:'center',justifyContent:'center',
      }}
      onMouseEnter={e=>{e.currentTarget.style.background='#f8fafc'}}
      onMouseLeave={e=>{e.currentTarget.style.background='#fff'}}>
        <Icon.MessageSquare size={15}/>
      </button>
    </Card>
  );
}

function ComplianceCard() {
  return (
    <Card style={{padding:'16px 22px'}}>
      <div style={{display:'flex',alignItems:'flex-start',gap:12}}>
        <div style={{
          width:32,height:32,borderRadius:8,background:'#d1fae5',color:'#047857',
          display:'inline-flex',alignItems:'center',justifyContent:'center',flexShrink:0,
        }}><Icon.ShieldCheck size={16}/></div>
        <div style={{flex:1,fontSize:12.5,color:'#475569',lineHeight:1.55}}>
          <div style={{fontWeight:600,color:'#0f172a',fontSize:13,marginBottom:4}}>Соответствие требованиям</div>
          Платёж зарегистрирован в&nbsp;ОФД. Эквайер сертифицирован <strong>PCI&nbsp;DSS Level&nbsp;1</strong>.
        </div>
      </div>
    </Card>
  );
}

// — Main page —
function PaymentDetail() {
  const p = PAYMENT_DETAIL;
  return (
    <div data-screen-label="Платёж — детализация" style={{
      padding:28,display:'grid',gap:18,
      gridTemplateColumns:'minmax(0,1fr) 340px',
      background:'#f8fafc',minHeight:'100%',
    }}>
      <div style={{display:'flex',flexDirection:'column',gap:16,minWidth:0}}>
        <PaymentHero p={p}/>
        <LineItems p={p}/>
        <AcquirerCard p={p}/>
        <Timeline events={p.events}/>
      </div>
      <div style={{display:'flex',flexDirection:'column',gap:14}}>
        <StudentCard s={p.student}/>
        <CourseCardSide c={p.course}/>
        <DocumentsCard docs={p.documents}/>
        <ManagerCard m={p.manager}/>
        <ComplianceCard/>
      </div>
    </div>
  );
}

window.PaymentDetail = PaymentDetail;
window.PAYMENT_DETAIL = PAYMENT_DETAIL;
