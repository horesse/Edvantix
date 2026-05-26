// ── Subscription / Billing page ─────────────────────────────────
// Edvantix subscription management for the SCHOOL (the customer).
// Not to be confused with student payments — this is the school's
// own SaaS plan, payment method, legal details and invoices from Edvantix.

const fmtRubInt = (n) => '₽\u00A0' + new Intl.NumberFormat('ru-RU').format(n);
const fmtDateLong = (iso) => {
  const d = new Date(iso);
  const months = ['января','февраля','марта','апреля','мая','июня','июля','августа','сентября','октября','ноября','декабря'];
  return `${d.getDate()} ${months[d.getMonth()]} ${d.getFullYear()}`;
};
const fmtDateShort = (iso) => {
  const d = new Date(iso);
  const months = ['янв','фев','мар','апр','мая','июня','июля','авг','сен','окт','ноя','дек'];
  return `${d.getDate()} ${months[d.getMonth()]} ${d.getFullYear()}`;
};

// — current subscription state —
const SUBSCRIPTION = {
  plan: 'Pro',
  status: 'active',           // active | trial | past_due | cancelled
  cycle: 'monthly',           // monthly | yearly
  price: 9900,
  currency: 'RUB',
  startedAt: '2026-05-05',
  renewsAt: '2026-06-05',
  cancelAtPeriodEnd: false,
  autoRenew: true,
};

// — usage —
const USAGE = {
  students: { used: 312, limit: 500, label: 'Студенты' },
  courses:  { used: 24,  limit: null, label: 'Активные курсы' },
  storage:  { used: 18.4, limit: 50,  label: 'Файлы и материалы', unit: 'ГБ' },
  staff:    { used: 7,   limit: 15,  label: 'Преподаватели' },
};

// — payment methods —
const PAYMENT_METHODS = [
  { id:'pm1', brand:'visa', last4:'4421', exp:'09/27', holder:'KREATIV PLUS LLC', primary:true },
  { id:'pm2', brand:'mir',  last4:'8807', exp:'02/28', holder:'KREATIV PLUS LLC', primary:false },
];

// — Edvantix invoices to the school —
const INVOICES = [
  { id:'EDV-2026-00184', period:'Май 2026',     issued:'2026-05-05', amount:9900,  status:'paid',    items:'Подписка Pro · помесячно' },
  { id:'EDV-2026-00161', period:'Апрель 2026',  issued:'2026-04-05', amount:9900,  status:'paid',    items:'Подписка Pro · помесячно' },
  { id:'EDV-2026-00138', period:'Март 2026',    issued:'2026-03-05', amount:9900,  status:'paid',    items:'Подписка Pro · помесячно' },
  { id:'EDV-2026-00112', period:'Февраль 2026', issued:'2026-02-05', amount:9900,  status:'paid',    items:'Подписка Pro · помесячно' },
  { id:'EDV-2026-00088', period:'Январь 2026',  issued:'2026-01-05', amount:9900,  status:'paid',    items:'Подписка Pro · помесячно' },
  { id:'EDV-2025-00601', period:'Декабрь 2025', issued:'2025-12-05', amount:4900,  status:'paid',    items:'Подписка Старт · помесячно' },
  { id:'EDV-2025-00567', period:'Ноябрь 2025',  issued:'2025-11-05', amount:4900,  status:'paid',    items:'Подписка Старт · помесячно' },
];

const PLANS = [
  {
    id:'start', name:'Старт',
    price:{ monthly:4900, yearly:49000 },
    blurb:'Для новых школ до 100 студентов',
    features:[
      'До 100 студентов',
      'До 5 преподавателей',
      '10 ГБ хранилища',
      'Базовый дашборд',
      'Email-поддержка',
    ],
  },
  {
    id:'pro', name:'Pro', current:true, popular:true,
    price:{ monthly:9900, yearly:99000 },
    blurb:'Для растущих школ — оптимальный набор',
    features:[
      'До 500 студентов',
      'До 15 преподавателей',
      '50 ГБ хранилища',
      'Расширенная аналитика',
      'Интеграции и приём оплат',
      'Поддержка в чате 24/7',
    ],
  },
  {
    id:'business', name:'Бизнес',
    price:{ monthly:24900, yearly:249000 },
    blurb:'Для крупных школ и сетей',
    features:[
      'Без ограничений по студентам',
      'Без ограничений по штату',
      '500 ГБ хранилища',
      'SSO и кастомный домен',
      'Персональный менеджер',
      'SLA 99,9% и приоритет',
    ],
  },
];

// ── tiny shared bits ────────────────────────────────────────────
function Tile({children, style}) {
  return (
    <div style={{
      background:'#fff', border:'1px solid #e2e8f0', borderRadius:16,
      boxShadow:'0 1px 3px 0 rgba(0,0,0,0.04), 0 1px 2px -1px rgba(0,0,0,0.04)',
      ...style,
    }}>{children}</div>
  );
}

function SectionHead({eyebrow, title, action}) {
  return (
    <div style={{display:'flex',alignItems:'flex-end',justifyContent:'space-between',gap:16,marginBottom:14}}>
      <div>
        {eyebrow && (
          <div style={{fontSize:11,fontWeight:600,letterSpacing:'0.1em',textTransform:'uppercase',color:'#94a3b8',marginBottom:6}}>
            {eyebrow}
          </div>
        )}
        <h2 style={{margin:0,fontSize:18,fontWeight:700,letterSpacing:'-0.01em',color:'#0f172a'}}>{title}</h2>
      </div>
      {action}
    </div>
  );
}

function Bar({value, max, color='#4f46e5', bg='#f1f5f9'}) {
  const pct = max ? Math.min(100, Math.round(value/max*100)) : 0;
  return (
    <div style={{height:6,background:bg,borderRadius:9999,overflow:'hidden'}}>
      <div style={{width:pct+'%',height:'100%',background:color,borderRadius:9999,transition:'width .25s'}}/>
    </div>
  );
}

// brand glyph for cards — small visual chip
function CardBrand({brand, size=36}) {
  const meta = {
    visa: { bg:'linear-gradient(135deg,#1a1f71,#2a3192)', label:'VISA', color:'#fff' },
    mc:   { bg:'linear-gradient(135deg,#eb001b,#f79e1b)', label:'MC',   color:'#fff' },
    mir:  { bg:'linear-gradient(135deg,#0eb87f,#1a8b50)', label:'МИР',  color:'#fff' },
  }[brand] || { bg:'#f1f5f9', label:'••', color:'#475569' };
  return (
    <div style={{
      width:size*1.5, height:size, borderRadius:8, background: meta.bg, color: meta.color,
      display:'inline-flex',alignItems:'center',justifyContent:'center',
      fontFamily:'inherit', fontWeight:800, fontSize: size===36?13:11, letterSpacing:'0.03em',
      flexShrink:0,
    }}>{meta.label}</div>
  );
}

// ── 1. Current plan hero ────────────────────────────────────────
function PlanHero() {
  const days = Math.max(0, Math.round((new Date(SUBSCRIPTION.renewsAt) - new Date()) / 86400000));
  // assume a 30-day cycle for the progress bar
  const usedDays = 30 - days;
  return (
    <Tile style={{padding:0, overflow:'hidden', position:'relative'}}>
      {/* decorative gradient strip */}
      <div style={{
        position:'absolute',inset:'0 0 auto 0', height:6,
        background:'linear-gradient(90deg,#6366f1,#a855f7,#4f46e5)',
      }}/>
      <div style={{
        display:'grid', gridTemplateColumns:'1.4fr 1fr 1fr auto', gap:24, padding:'24px 28px',
        alignItems:'center',
      }}>
        {/* identity */}
        <div style={{display:'flex',gap:18,alignItems:'flex-start'}}>
          <div style={{
            width:56, height:56, borderRadius:14,
            background:'linear-gradient(135deg,#eef2ff,#e0e7ff)',
            border:'1px solid #c7d2fe',
            display:'flex',alignItems:'center',justifyContent:'center',
            color:'#4f46e5', flexShrink:0,
          }}>
            <Icon.GraduationCap size={26}/>
          </div>
          <div style={{minWidth:0}}>
            <div style={{display:'flex',alignItems:'center',gap:8,marginBottom:4}}>
              <Badge variant="primary" dot>Активна</Badge>
              <span style={{fontSize:11.5,color:'#64748b'}}>Помесячная оплата</span>
            </div>
            <div style={{display:'flex',alignItems:'baseline',gap:10}}>
              <h2 style={{margin:0,fontSize:32,fontWeight:800,letterSpacing:'-0.03em'}}>План Pro</h2>
            </div>
            <div style={{fontSize:13,color:'#64748b',marginTop:2}}>
              {fmtRubInt(SUBSCRIPTION.price)}/мес · без НДС · подписка с {fmtDateShort(SUBSCRIPTION.startedAt)}
            </div>
          </div>
        </div>

        {/* renewal */}
        <div style={{borderLeft:'1px solid #e2e8f0',paddingLeft:24}}>
          <div style={{fontSize:11,fontWeight:600,letterSpacing:'0.1em',textTransform:'uppercase',color:'#94a3b8',marginBottom:8}}>
            Следующее списание
          </div>
          <div style={{fontSize:20,fontWeight:700,letterSpacing:'-0.01em',color:'#0f172a',lineHeight:1.15}}>
            {fmtDateLong(SUBSCRIPTION.renewsAt)}
          </div>
          <div style={{fontSize:13,color:'#64748b',marginTop:4,display:'flex',alignItems:'center',gap:6}}>
            <Icon.CreditCard size={13} stroke="#94a3b8"/>
            Visa •• 4421 · автосписание
          </div>
        </div>

        {/* period progress */}
        <div style={{borderLeft:'1px solid #e2e8f0',paddingLeft:24}}>
          <div style={{display:'flex',alignItems:'center',justifyContent:'space-between',marginBottom:8}}>
            <div style={{fontSize:11,fontWeight:600,letterSpacing:'0.1em',textTransform:'uppercase',color:'#94a3b8'}}>
              До конца периода
            </div>
            <div style={{fontSize:12,color:'#64748b',fontVariantNumeric:'tabular-nums'}}>{usedDays}/30 дн.</div>
          </div>
          <div style={{fontSize:20,fontWeight:700,letterSpacing:'-0.01em',color:'#0f172a',lineHeight:1.15,fontVariantNumeric:'tabular-nums'}}>
            {days} {days===1?'день':days<5?'дня':'дней'}
          </div>
          <div style={{marginTop:10}}>
            <Bar value={usedDays} max={30}/>
          </div>
        </div>

        {/* actions */}
        <div style={{display:'flex',flexDirection:'column',gap:8,alignItems:'stretch',minWidth:160}}>
          <Button size="md"><Icon.ArrowUpRight size={15}/>Перейти на Бизнес</Button>
          <Button variant="secondary" size="md"><Icon.RefreshCw size={14}/>Перейти на годовую</Button>
        </div>
      </div>

      {/* yearly nudge */}
      <div style={{
        background:'#f8fafc', borderTop:'1px solid #e2e8f0',
        padding:'12px 28px', display:'flex', alignItems:'center', gap:12,
        fontSize:13, color:'#334155',
      }}>
        <div style={{
          width:24,height:24,borderRadius:6,background:'#ede9fe',color:'#7c3aed',
          display:'inline-flex',alignItems:'center',justifyContent:'center',flexShrink:0,
        }}>
          <Icon.Info size={13}/>
        </div>
        <span>
          При переходе на годовой план — <strong style={{color:'#0f172a'}}>экономия {fmtRubInt(19800)}</strong> в&nbsp;год
          (₽&nbsp;99 000 вместо ₽&nbsp;118 800).
        </span>
        <a href="#" style={{marginLeft:'auto',color:'#4f46e5',fontWeight:600,fontSize:13,display:'inline-flex',alignItems:'center',gap:4}}>
          Посмотреть выгоду <Icon.ArrowRight size={13}/>
        </a>
      </div>
    </Tile>
  );
}

// ── 2. Usage meters ─────────────────────────────────────────────
function UsageMeter({iconName, iconBg, iconColor, label, used, limit, unit, color}) {
  const Ic = Icon[iconName];
  const pct = limit ? Math.min(100, Math.round(used/limit*100)) : null;
  const warning = pct !== null && pct >= 80;
  return (
    <Tile style={{padding:20,display:'flex',flexDirection:'column',gap:14}}>
      <div style={{display:'flex',alignItems:'center',gap:10}}>
        <div style={{
          width:32,height:32,borderRadius:8,background:iconBg,color:iconColor,
          display:'inline-flex',alignItems:'center',justifyContent:'center',
        }}><Ic size={16}/></div>
        <div style={{fontSize:13,fontWeight:600,color:'#334155',flex:1}}>{label}</div>
        {warning && <Icon.AlertCircle size={14} stroke="#f59e0b"/>}
      </div>
      <div>
        <div style={{display:'flex',alignItems:'baseline',gap:6}}>
          <span style={{fontSize:24,fontWeight:700,letterSpacing:'-0.02em',fontVariantNumeric:'tabular-nums',color:'#0f172a'}}>
            {used}{unit?<span style={{fontSize:14,color:'#94a3b8',marginLeft:2,fontWeight:500}}>&nbsp;{unit}</span>:''}
          </span>
          <span style={{fontSize:13,color:'#94a3b8',fontVariantNumeric:'tabular-nums'}}>
            {limit ? `/ ${limit}${unit?'\u00A0'+unit:''}` : '· без лимита'}
          </span>
        </div>
        {pct !== null && (
          <div style={{marginTop:10}}>
            <Bar value={used} max={limit} color={warning?'#f59e0b':color||'#4f46e5'}/>
            <div style={{fontSize:11.5,color:'#94a3b8',marginTop:6,fontVariantNumeric:'tabular-nums'}}>
              {pct}% использовано
            </div>
          </div>
        )}
      </div>
    </Tile>
  );
}

function UsageGrid() {
  return (
    <div>
      <SectionHead
        eyebrow="Расход"
        title="Использование плана"
        action={<a href="#" style={{fontSize:13,color:'#4f46e5',fontWeight:600,display:'inline-flex',alignItems:'center',gap:4}}>
          Детали потребления <Icon.ArrowRight size={13}/>
        </a>}
      />
      <div style={{display:'grid',gridTemplateColumns:'repeat(4,1fr)',gap:14}}>
        <UsageMeter iconName="Users" iconBg="#e0eaff" iconColor="#4338ca"
          label="Студенты" used={USAGE.students.used} limit={USAGE.students.limit}/>
        <UsageMeter iconName="BookOpen" iconBg="#d1fae5" iconColor="#059669"
          label="Активные курсы" used={USAGE.courses.used} limit={USAGE.courses.limit}/>
        <UsageMeter iconName="GraduationCap" iconBg="#fef3c7" iconColor="#b45309"
          label="Преподаватели" used={USAGE.staff.used} limit={USAGE.staff.limit}/>
        <UsageMeter iconName="FileText" iconBg="#ede9fe" iconColor="#7c3aed"
          label="Файлы и материалы" used={USAGE.storage.used} limit={USAGE.storage.limit} unit="ГБ"/>
      </div>
    </div>
  );
}

// ── 3. Payment method + Billing details ─────────────────────────
function PaymentMethodRow({pm, onSetPrimary, onRemove}) {
  const [hover, setHover] = React.useState(false);
  return (
    <div
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        display:'flex',alignItems:'center',gap:14,padding:'14px 16px',
        border:'1px solid '+(pm.primary?'rgba(79,70,229,0.25)':'#e2e8f0'),
        background:pm.primary?'rgba(79,70,229,0.04)':(hover?'#f8fafc':'#fff'),
        borderRadius:12, transition:'.12s',
      }}>
      <CardBrand brand={pm.brand}/>
      <div style={{flex:1,minWidth:0}}>
        <div style={{display:'flex',alignItems:'center',gap:8}}>
          <span style={{fontSize:14,fontWeight:600,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>
            •••• •••• •••• {pm.last4}
          </span>
          {pm.primary && <Badge variant="primary">Основная</Badge>}
        </div>
        <div style={{fontSize:12,color:'#64748b',marginTop:2}}>
          {pm.holder} · действует до {pm.exp}
        </div>
      </div>
      <div style={{display:'flex',gap:6}}>
        {!pm.primary && (
          <Button variant="ghost" size="sm" onClick={onSetPrimary}>Сделать основной</Button>
        )}
        <button style={{
          width:32,height:32,borderRadius:8,border:'1px solid transparent',
          background: hover ? '#f1f5f9' : 'transparent', color:'#64748b',
          cursor:'pointer', display:'inline-flex',alignItems:'center',justifyContent:'center',
        }}><Icon.MoreHorizontal size={16}/></button>
      </div>
    </div>
  );
}

function PaymentMethodCard() {
  return (
    <Tile style={{padding:24,display:'flex',flexDirection:'column',gap:16}}>
      <div style={{display:'flex',alignItems:'flex-start',justifyContent:'space-between',gap:12}}>
        <div>
          <div style={{display:'flex',alignItems:'center',gap:8,marginBottom:4}}>
            <Icon.CreditCard size={16} stroke="#4f46e5"/>
            <h3 style={{margin:0,fontSize:16,fontWeight:700,letterSpacing:'-0.01em'}}>Способ оплаты</h3>
          </div>
          <div style={{fontSize:13,color:'#64748b'}}>
            Карта, с которой Edvantix списывает плату за подписку
          </div>
        </div>
        <Button variant="secondary" size="sm"><Icon.Plus size={14}/>Добавить</Button>
      </div>

      <div style={{display:'flex',flexDirection:'column',gap:8}}>
        {PAYMENT_METHODS.map(pm => <PaymentMethodRow key={pm.id} pm={pm}/>)}
      </div>

      <div style={{
        display:'flex',alignItems:'center',gap:10,padding:'10px 12px',
        background:'#f1f5f9',borderRadius:10,fontSize:12,color:'#475569',
      }}>
        <Icon.ShieldCheck size={14} stroke="#10b981"/>
        Платежи защищены 3-D Secure. Данные карты хранит банк-эквайер, не Edvantix.
      </div>
    </Tile>
  );
}

function DetailRow({label, value, sub, hint, editable}) {
  return (
    <div style={{
      display:'grid', gridTemplateColumns:'140px 1fr',
      gap:16, padding:'12px 0', borderBottom:'1px solid #f1f5f9',
    }}>
      <div style={{fontSize:12.5,color:'#64748b',paddingTop:2}}>{label}</div>
      <div style={{minWidth:0}}>
        <div style={{fontSize:14,color:'#0f172a',fontWeight:500}}>{value}</div>
        {sub && <div style={{fontSize:12,color:'#94a3b8',marginTop:2}}>{sub}</div>}
        {hint && <div style={{fontSize:11.5,color:'#94a3b8',marginTop:2,display:'flex',alignItems:'center',gap:4}}>
          <Icon.Info size={11} stroke="#cbd5e1"/>{hint}
        </div>}
      </div>
    </div>
  );
}

function BillingDetailsCard() {
  return (
    <Tile style={{padding:24}}>
      <div style={{display:'flex',alignItems:'flex-start',justifyContent:'space-between',gap:12,marginBottom:8}}>
        <div>
          <div style={{display:'flex',alignItems:'center',gap:8,marginBottom:4}}>
            <Icon.Building2 size={16} stroke="#4f46e5"/>
            <h3 style={{margin:0,fontSize:16,fontWeight:700,letterSpacing:'-0.01em'}}>Реквизиты для счетов</h3>
          </div>
          <div style={{fontSize:13,color:'#64748b'}}>
            Используются в счетах и актах от Edvantix
          </div>
        </div>
        <Button variant="secondary" size="sm"><Icon.Settings size={14}/>Редактировать</Button>
      </div>

      <div style={{marginTop:8}}>
        <DetailRow
          label="Юр. лицо"
          value='ООО «Креатив Плюс»'
          sub="Резидент РФ"/>
        <DetailRow label="ИНН" value="7728382911"/>
        <DetailRow label="КПП" value="772801001"/>
        <DetailRow label="ОГРН" value="1207700318422" sub="зарегистрировано 14 марта 2020"/>
        <DetailRow
          label="Юр. адрес"
          value="115093, Москва, ул. Большая Серпуховская, д. 44, оф. 312"/>
        <DetailRow
          label="Email для счетов"
          value="finance@kreativplus.ru"
          hint="Счета и акты отправляются на этот адрес каждый месяц"/>
      </div>
    </Tile>
  );
}

// ── 4. Plans comparison ─────────────────────────────────────────
function PlanCard({plan, cycle}) {
  const [hover, setHover] = React.useState(false);
  const price = plan.price[cycle];
  const monthly = cycle === 'yearly' ? Math.round(price/12) : price;
  const isCurrent = plan.current;

  return (
    <div
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        position:'relative', display:'flex',flexDirection:'column',
        background: isCurrent ? 'linear-gradient(180deg, rgba(79,70,229,0.04), #fff)' : '#fff',
        border:'1px solid '+(isCurrent?'rgba(79,70,229,0.4)':'#e2e8f0'),
        borderRadius:16, padding:24, gap:18,
        boxShadow: isCurrent
          ? '0 10px 32px -16px rgba(79,70,229,0.35), 0 0 0 1px rgba(79,70,229,0.06)'
          : hover ? '0 6px 18px -8px rgba(15,23,42,0.10)' : '0 1px 3px 0 rgba(0,0,0,0.04)',
        transition:'.18s',
      }}>
      {plan.popular && (
        <div style={{
          position:'absolute', top:-10, left:24,
          background:'#4f46e5', color:'#fff',
          padding:'3px 10px', borderRadius:9999, fontSize:11, fontWeight:600,
          letterSpacing:'0.02em', boxShadow:'0 4px 12px rgba(79,70,229,0.35)',
        }}>Популярный</div>
      )}

      <div>
        <div style={{display:'flex',alignItems:'center',gap:8,marginBottom:6}}>
          <h3 style={{margin:0,fontSize:18,fontWeight:700,letterSpacing:'-0.01em'}}>{plan.name}</h3>
          {isCurrent && <Badge variant="primary" dot>Ваш план</Badge>}
        </div>
        <div style={{fontSize:13,color:'#64748b'}}>{plan.blurb}</div>
      </div>

      <div>
        <div style={{display:'flex',alignItems:'baseline',gap:4}}>
          <span style={{fontSize:32,fontWeight:800,letterSpacing:'-0.03em',color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>
            {fmtRubInt(monthly)}
          </span>
          <span style={{fontSize:13,color:'#94a3b8'}}>/ мес</span>
        </div>
        <div style={{fontSize:12,color:'#94a3b8',marginTop:4,fontVariantNumeric:'tabular-nums'}}>
          {cycle==='yearly'
            ? <>оплата {fmtRubInt(price)} в год · экономия 17%</>
            : <>при оплате помесячно · {fmtRubInt(plan.price.yearly)} в год</>}
        </div>
      </div>

      <div style={{display:'flex',flexDirection:'column',gap:9,flex:1}}>
        {plan.features.map(f => (
          <div key={f} style={{display:'flex',alignItems:'flex-start',gap:9,fontSize:13.5,color:'#334155'}}>
            <div style={{
              width:18,height:18,borderRadius:9999,
              background: isCurrent?'#4f46e5':'#e0eaff',
              color: isCurrent?'#fff':'#4338ca',
              display:'inline-flex',alignItems:'center',justifyContent:'center',
              flexShrink:0,marginTop:1,
            }}>
              <Icon.Check size={11}/>
            </div>
            {f}
          </div>
        ))}
      </div>

      {isCurrent ? (
        <Button variant="secondary" size="md" style={{justifyContent:'center'}} disabled>
          Текущий план
        </Button>
      ) : plan.id === 'business' ? (
        <Button variant="secondary" size="md" style={{justifyContent:'center'}}>
          Связаться с продажами
        </Button>
      ) : (
        <Button size="md" style={{justifyContent:'center'}}>
          {plan.id === 'start' ? 'Понизить тариф' : 'Перейти на план'}
        </Button>
      )}
    </div>
  );
}

function PlansSection() {
  const [cycle, setCycle] = React.useState('monthly');
  return (
    <div>
      <SectionHead
        eyebrow="Тарифы"
        title="Сменить или сравнить план"
        action={
          <div style={{
            display:'inline-flex', background:'#f1f5f9', borderRadius:9999, padding:3,
            border:'1px solid #e2e8f0',
          }}>
            {[{v:'monthly',l:'Помесячно'},{v:'yearly',l:'Годовая (−17%)'}].map(o=>(
              <button key={o.v} onClick={()=>setCycle(o.v)} style={{
                padding:'6px 14px', borderRadius:9999, border:'none',
                background: cycle===o.v ? '#fff' : 'transparent',
                color: cycle===o.v ? '#0f172a' : '#64748b',
                boxShadow: cycle===o.v ? '0 1px 2px rgba(15,23,42,0.08)' : 'none',
                fontSize:13, fontWeight:600, fontFamily:'inherit', cursor:'pointer',
                transition:'.12s',
              }}>{o.l}</button>
            ))}
          </div>
        }
      />
      <div style={{display:'grid',gridTemplateColumns:'repeat(3,1fr)',gap:14,paddingTop:8}}>
        {PLANS.map(p => <PlanCard key={p.id} plan={p} cycle={cycle}/>)}
      </div>
    </div>
  );
}

// ── 5. Invoice history (Edvantix → school) ─────────────────────
function InvoiceStatus({status}) {
  const map = {
    paid:    { label:'Оплачен',    variant:'success' },
    pending: { label:'Ожидает',    variant:'warning' },
    failed:  { label:'Ошибка',     variant:'danger'  },
    refund:  { label:'Возврат',    variant:'default' },
  };
  const s = map[status] || map.paid;
  return <Badge variant={s.variant} dot>{s.label}</Badge>;
}

function InvoicesTable() {
  const [hovered, setHovered] = React.useState(null);
  return (
    <div>
      <SectionHead
        eyebrow="История"
        title="Счета от Edvantix"
        action={
          <div style={{display:'flex',gap:8}}>
            <Button variant="ghost" size="sm"><Icon.Download size={14}/>Скачать всё (.zip)</Button>
            <Button variant="secondary" size="sm"><Icon.ExternalLink size={14}/>Открыть портал</Button>
          </div>
        }
      />
      <Tile style={{padding:0,overflow:'hidden'}}>
        <div style={{overflowX:'auto'}}>
          <table style={{width:'100%',borderCollapse:'collapse',minWidth:780}}>
            <thead>
              <tr>
                {['№ счёта','Период','Состав','Сумма','Дата','Статус',''].map((h,i)=>(
                  <th key={i} style={{
                    textAlign: h==='Сумма'?'right':'left',
                    padding:'12px 16px',fontSize:11.5,fontWeight:600,
                    color:'#64748b',textTransform:'uppercase',letterSpacing:'0.06em',
                    borderBottom:'1px solid #e2e8f0',background:'#f8fafc',whiteSpace:'nowrap',
                  }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {INVOICES.map((inv,i)=>(
                <tr key={inv.id}
                  onMouseEnter={()=>setHovered(i)} onMouseLeave={()=>setHovered(null)}
                  style={{background: hovered===i?'#f8fafc':'#fff',transition:'.1s'}}>
                  <td style={{padding:'14px 16px',borderBottom:'1px solid #f1f5f9',fontSize:13,fontWeight:600,color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>
                    {inv.id}
                  </td>
                  <td style={{padding:'14px 16px',borderBottom:'1px solid #f1f5f9',fontSize:13,color:'#334155'}}>
                    {inv.period}
                  </td>
                  <td style={{padding:'14px 16px',borderBottom:'1px solid #f1f5f9',fontSize:13,color:'#475569'}}>
                    {inv.items}
                  </td>
                  <td style={{padding:'14px 16px',borderBottom:'1px solid #f1f5f9',fontSize:14,fontWeight:600,color:'#0f172a',fontVariantNumeric:'tabular-nums',textAlign:'right'}}>
                    {fmtRubInt(inv.amount)}
                  </td>
                  <td style={{padding:'14px 16px',borderBottom:'1px solid #f1f5f9',fontSize:13,color:'#475569',fontVariantNumeric:'tabular-nums',whiteSpace:'nowrap'}}>
                    {fmtDateShort(inv.issued)}
                  </td>
                  <td style={{padding:'14px 16px',borderBottom:'1px solid #f1f5f9'}}>
                    <InvoiceStatus status={inv.status}/>
                  </td>
                  <td style={{padding:'10px 12px',borderBottom:'1px solid #f1f5f9',textAlign:'right',whiteSpace:'nowrap'}}>
                    <div style={{display:'inline-flex',gap:4}}>
                      <button title="Счёт .pdf" style={iconBtnStyle}>
                        <Icon.Download size={14}/>
                      </button>
                      <button title="Акт .pdf" style={iconBtnStyle}>
                        <Icon.FileText size={14}/>
                      </button>
                      <button title="Открыть" style={iconBtnStyle}>
                        <Icon.ArrowUpRight size={14}/>
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div style={{padding:'12px 16px',borderTop:'1px solid #e2e8f0',display:'flex',alignItems:'center',justifyContent:'space-between',fontSize:13,color:'#64748b'}}>
          <span>Показано <strong style={{color:'#0f172a'}}>7</strong> из <strong style={{color:'#0f172a'}}>13</strong> счетов</span>
          <a href="#" style={{color:'#4f46e5',fontWeight:600,display:'inline-flex',alignItems:'center',gap:4}}>
            Показать все <Icon.ArrowRight size={13}/>
          </a>
        </div>
      </Tile>
    </div>
  );
}

const iconBtnStyle = {
  width:30,height:30,borderRadius:6,border:'1px solid transparent',
  background:'transparent',color:'#475569',cursor:'pointer',
  display:'inline-flex',alignItems:'center',justifyContent:'center',
  fontFamily:'inherit',
};

// ── 6. Danger zone — cancel ──────────────────────────────────────
function CancelZone() {
  return (
    <Tile style={{padding:24,borderColor:'#fecaca',background:'#fffafa'}}>
      <div style={{display:'flex',alignItems:'flex-start',gap:16}}>
        <div style={{
          width:40,height:40,borderRadius:10,background:'#fee2e2',color:'#b91c1c',
          display:'flex',alignItems:'center',justifyContent:'center',flexShrink:0,
        }}>
          <Icon.AlertCircle size={20}/>
        </div>
        <div style={{flex:1}}>
          <h3 style={{margin:0,fontSize:15,fontWeight:700,color:'#0f172a'}}>Отмена подписки</h3>
          <div style={{fontSize:13,color:'#64748b',marginTop:4,maxWidth:640,lineHeight:1.5}}>
            Доступ к школе сохранится до конца оплаченного периода —
            до <strong style={{color:'#0f172a'}}>{fmtDateLong(SUBSCRIPTION.renewsAt)}</strong>.
            Данные студентов и курсов сохраняются ещё 60 дней.
          </div>
        </div>
        <div style={{display:'flex',gap:8,alignSelf:'center'}}>
          <Button variant="ghost" size="sm">Связаться с поддержкой</Button>
          <Button variant="secondary" size="sm" style={{color:'#b91c1c',borderColor:'#fecaca'}}>
            Отменить подписку
          </Button>
        </div>
      </div>
    </Tile>
  );
}

// ── Page assembly ────────────────────────────────────────────────
function SubscriptionPage() {
  return (
    <div style={{padding:28,display:'flex',flexDirection:'column',gap:32,background:'#f8fafc',minHeight:'100%'}}>
      <PlanHero/>
      <UsageGrid/>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:18}}>
        <PaymentMethodCard/>
        <BillingDetailsCard/>
      </div>
      <PlansSection/>
      <InvoicesTable/>
      <CancelZone/>
    </div>
  );
}

window.SubscriptionPage = SubscriptionPage;
