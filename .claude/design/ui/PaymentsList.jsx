// ── Payments list page ─────────────────────────────────────────────
// Light theme, indigo brand. Reuses Card/Button/Input/Badge/Avatar primitives.

const PAYMENTS = [
  { id:'INV-2026-0247', student:'Иван Козлов',       email:'i.kozlov@mail.ru',      course:'Дизайн 3D-моделей',  amount:18900, method:'card',   methodHint:'Visa •• 4421', date:'2026-05-21T11:42', status:'paid' },
  { id:'INV-2026-0246', student:'Мария Корецкая',    email:'maria.k@gmail.com',     course:'Веб-разработка',     amount:24500, method:'sbp',    methodHint:'СБП · Сбер',    date:'2026-05-21T10:18', status:'paid' },
  { id:'INV-2026-0245', student:'Елена Петрова',     email:'elena.p@yandex.ru',     course:'Иллюстрация',        amount:12400, method:'card',   methodHint:'MIR •• 8807',   date:'2026-05-20T19:05', status:'pending' },
  { id:'INV-2026-0244', student:'Дмитрий Соколов',   email:'sokolov.d@mail.ru',     course:'Python для детей',   amount:9900,  method:'card',   methodHint:'Visa •• 1209', date:'2026-05-20T16:30', status:'paid' },
  { id:'INV-2026-0243', student:'Анна Лебедева',     email:'anna.lebedeva@mail.ru', course:'UX/UI дизайн',       amount:32000, method:'wire',   methodHint:'Тинькофф',     date:'2026-05-20T14:12', status:'overdue' },
  { id:'INV-2026-0242', student:'Сергей Волков',     email:'s.volkov@gmail.com',    course:'Анимация в Blender', amount:21000, method:'sbp',    methodHint:'СБП · Альфа',  date:'2026-05-20T12:48', status:'paid' },
  { id:'INV-2026-0241', student:'Ольга Морозова',    email:'o.morozova@yandex.ru',  course:'Photoshop с нуля',   amount:14500, method:'card',   methodHint:'MC •• 3340',   date:'2026-05-19T20:22', status:'pending' },
  { id:'INV-2026-0240', student:'Александр Новиков', email:'a.novikov@mail.ru',     course:'Frontend на React',  amount:28900, method:'card',   methodHint:'Visa •• 0921', date:'2026-05-19T18:01', status:'paid' },
  { id:'INV-2026-0239', student:'Татьяна Смирнова',  email:'t.smirnova@mail.ru',    course:'Английский (B1)',    amount:8400,  method:'sbp',    methodHint:'СБП · Райф',   date:'2026-05-19T15:55', status:'cancelled' },
  { id:'INV-2026-0238', student:'Никита Орлов',      email:'n.orlov@gmail.com',     course:'Дизайн 3D-моделей',  amount:18900, method:'card',   methodHint:'Visa •• 7712', date:'2026-05-19T11:09', status:'paid' },
  { id:'INV-2026-0237', student:'Юлия Зайцева',      email:'yulia.z@yandex.ru',     course:'Веб-разработка',     amount:24500, method:'wire',   methodHint:'ИП · Зайцева', date:'2026-05-18T17:33', status:'overdue' },
  { id:'INV-2026-0236', student:'Михаил Васильев',   email:'m.vasiliev@mail.ru',    course:'Python для детей',   amount:9900,  method:'card',   methodHint:'MIR •• 4488',  date:'2026-05-18T13:24', status:'paid' },
  { id:'INV-2026-0235', student:'Кристина Павлова',  email:'k.pavlova@gmail.com',   course:'UX/UI дизайн',       amount:32000, method:'card',   methodHint:'Visa •• 6651', date:'2026-05-18T10:48', status:'paid' },
  { id:'INV-2026-0234', student:'Артём Громов',      email:'gromov.a@mail.ru',      course:'Иллюстрация',        amount:12400, method:'sbp',    methodHint:'СБП · ВТБ',    date:'2026-05-17T19:12', status:'pending' },
];

const STATUS_DEF = {
  paid:      { label:'Оплачен',         variant:'success', icon:'CheckCircle2', dotColor:'#10b981' },
  pending:   { label:'Ожидает оплаты',  variant:'warning', icon:'Clock',        dotColor:'#f59e0b' },
  overdue:   { label:'Просрочен',       variant:'danger',  icon:'AlertCircle',  dotColor:'#ef4444' },
  cancelled: { label:'Отменён',         variant:'default', icon:'XCircle',      dotColor:'#94a3b8' },
};

const METHOD_DEF = {
  card: { label:'Карта',   icon:'CreditCard' },
  sbp:  { label:'СБП',     icon:'Wallet' },
  wire: { label:'Перевод', icon:'Banknote' },
};

// — formatters —
const fmtRub = (n) => '₽\u00A0' + new Intl.NumberFormat('ru-RU').format(n);
const fmtRubK = (n) => {
  if (n >= 1_000_000) return '₽\u00A0' + (n/1_000_000).toFixed(1).replace('.0','') + 'М';
  if (n >= 1000) return '₽\u00A0' + Math.round(n/1000) + 'К';
  return '₽\u00A0' + n;
};
const fmtDate = (iso) => {
  const d = new Date(iso);
  const months = ['янв','фев','мар','апр','мая','июня','июля','авг','сен','окт','ноя','дек'];
  return `${d.getDate()} ${months[d.getMonth()]}`;
};
const fmtTime = (iso) => {
  const d = new Date(iso);
  return `${String(d.getHours()).padStart(2,'0')}:${String(d.getMinutes()).padStart(2,'0')}`;
};

// — small parts —
function StatusPill({status}) {
  const s = STATUS_DEF[status];
  return (
    <Badge variant={s.variant} dot>{s.label}</Badge>
  );
}

function MethodCell({method, hint}) {
  const m = METHOD_DEF[method];
  const Ic = Icon[m.icon];
  return (
    <div style={{display:'flex',alignItems:'center',gap:8}}>
      <div style={{
        width:28,height:28,borderRadius:6,background:'#f1f5f9',color:'#475569',
        display:'inline-flex',alignItems:'center',justifyContent:'center'
      }}><Ic size={14}/></div>
      <div style={{lineHeight:1.25,minWidth:0}}>
        <div style={{fontSize:13,fontWeight:500,color:'#0f172a'}}>{m.label}</div>
        <div style={{fontSize:11,color:'#94a3b8',whiteSpace:'nowrap',overflow:'hidden',textOverflow:'ellipsis',maxWidth:140}}>{hint}</div>
      </div>
    </div>
  );
}

function PaymentKpi({label, value, hint, hintColor='#64748b', iconName, iconBg, iconColor}) {
  const Ic = Icon[iconName];
  return (
    <Card style={{padding:20,display:'flex',flexDirection:'column',gap:12}}>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'flex-start'}}>
        <div style={{fontSize:12.5,fontWeight:500,color:'#64748b'}}>{label}</div>
        <div style={{
          width:32,height:32,borderRadius:8,background:iconBg,color:iconColor,
          display:'inline-flex',alignItems:'center',justifyContent:'center'
        }}><Ic size={16}/></div>
      </div>
      <div>
        <div style={{fontSize:26,fontWeight:700,letterSpacing:'-0.02em',fontVariantNumeric:'tabular-nums'}}>{value}</div>
        <div style={{fontSize:12,color:hintColor,marginTop:4,display:'flex',alignItems:'center',gap:4}}>
          {hint}
        </div>
      </div>
    </Card>
  );
}

function StatusTab({active, label, count, onClick, dotColor}) {
  const [hover, setHover] = React.useState(false);
  return (
    <button onClick={onClick}
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        display:'inline-flex',alignItems:'center',gap:8,
        padding:'7px 12px',borderRadius:8,border:'1px solid transparent',
        background: active ? 'rgba(79,70,229,0.10)' : hover ? '#f1f5f9' : 'transparent',
        borderColor: active ? 'rgba(79,70,229,0.18)' : 'transparent',
        color: active ? '#4338ca' : '#475569',
        fontSize:13, fontWeight: active ? 600 : 500, cursor:'pointer',
        fontFamily:'inherit', transition:'.12s',
      }}>
      {dotColor && <span style={{width:7,height:7,borderRadius:9999,background:dotColor}}/>}
      {label}
      <span style={{
        fontSize:11.5, fontWeight:600,
        background: active ? '#fff' : '#f1f5f9',
        color: active ? '#4338ca' : '#64748b',
        padding:'1px 7px', borderRadius:9999, fontVariantNumeric:'tabular-nums',
      }}>{count}</span>
    </button>
  );
}

function PageButton({children, active, disabled, onClick}) {
  const [hover, setHover] = React.useState(false);
  return (
    <button onClick={onClick} disabled={disabled}
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        minWidth:32,height:32,padding:'0 8px',
        borderRadius:8,border:'1px solid '+(active?'#4f46e5':'#e2e8f0'),
        background: active ? '#4f46e5' : hover && !disabled ? '#f8fafc' : '#fff',
        color: active ? '#fff' : disabled ? '#cbd5e1' : '#334155',
        fontSize:13, fontWeight: active?600:500, fontFamily:'inherit',
        cursor: disabled ? 'not-allowed' : 'pointer', transition:'.12s',
        display:'inline-flex',alignItems:'center',justifyContent:'center',
      }}>{children}</button>
  );
}

function TableHeader({children, sortable, align='left', width}) {
  return (
    <th style={{
      textAlign:align, padding:'12px 16px', fontSize:11.5, fontWeight:600,
      color:'#64748b', textTransform:'uppercase', letterSpacing:'0.06em',
      borderBottom:'1px solid #e2e8f0', background:'#f8fafc',
      width, whiteSpace:'nowrap',
      position:'sticky', top:0, zIndex:1,
    }}>
      <div style={{display:'inline-flex',alignItems:'center',gap:6,cursor:sortable?'pointer':'default'}}>
        {children}
        {sortable && <Icon.ArrowUpDown size={12} stroke="#cbd5e1"/>}
      </div>
    </th>
  );
}

function PaymentRow({row, selected, onSelect}) {
  const [hover, setHover] = React.useState(false);
  return (
    <tr
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        background: hover ? '#f8fafc' : '#fff',
        cursor:'pointer', transition:'background .1s',
      }}
      onClick={()=>{/* navigate to /payments/:id */}}
    >
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9', width:36}}
          onClick={(e)=>e.stopPropagation()}>
        <Checkbox checked={selected} onChange={onSelect}/>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9'}}>
        <div style={{display:'flex',flexDirection:'column'}}>
          <span style={{fontSize:13, fontWeight:600, color:'#0f172a', fontVariantNumeric:'tabular-nums'}}>{row.id}</span>
          <span style={{fontSize:11, color:'#94a3b8'}}>{fmtTime(row.date)}</span>
        </div>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9'}}>
        <div style={{display:'flex',alignItems:'center',gap:10}}>
          <Avatar name={row.student} size={32}/>
          <div style={{lineHeight:1.25,minWidth:0}}>
            <div style={{fontSize:13.5,fontWeight:600,color:'#0f172a'}}>{row.student}</div>
            <div style={{fontSize:11.5,color:'#94a3b8',whiteSpace:'nowrap',overflow:'hidden',textOverflow:'ellipsis',maxWidth:180}}>{row.email}</div>
          </div>
        </div>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9'}}>
        <span style={{fontSize:13, color:'#334155'}}>{row.course}</span>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9', textAlign:'right'}}>
        <span style={{fontSize:14, fontWeight:600, color:'#0f172a', fontVariantNumeric:'tabular-nums'}}>{fmtRub(row.amount)}</span>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9'}}>
        <MethodCell method={row.method} hint={row.methodHint}/>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9'}}>
        <span style={{fontSize:13, color:'#475569', fontVariantNumeric:'tabular-nums', whiteSpace:'nowrap'}}>{fmtDate(row.date)}</span>
      </td>
      <td style={{padding:'14px 16px', borderBottom:'1px solid #f1f5f9'}}>
        <StatusPill status={row.status}/>
      </td>
      <td style={{padding:'14px 12px', borderBottom:'1px solid #f1f5f9', textAlign:'right', width:64}}
          onClick={(e)=>e.stopPropagation()}>
        <div style={{display:'inline-flex',alignItems:'center',gap:2}}>
          <IconButton title="Открыть"><Icon.ArrowUpRight size={15}/></IconButton>
          <IconButton title="Ещё"><Icon.MoreHorizontal size={16}/></IconButton>
        </div>
      </td>
    </tr>
  );
}

function IconButton({children, title, onClick}) {
  const [hover, setHover] = React.useState(false);
  return (
    <button title={title} onClick={onClick}
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        width:28,height:28,borderRadius:6,border:'1px solid transparent',
        background: hover ? '#f1f5f9' : 'transparent', color:'#475569',
        cursor:'pointer', fontFamily:'inherit',
        display:'inline-flex',alignItems:'center',justifyContent:'center',
        transition:'.1s',
      }}>{children}</button>
  );
}

function Checkbox({checked, indeterminate, onChange}) {
  const ref = React.useRef(null);
  React.useEffect(()=>{ if (ref.current) ref.current.indeterminate = !!indeterminate; }, [indeterminate]);
  return (
    <label style={{display:'inline-flex',alignItems:'center',cursor:'pointer',padding:2}}>
      <input ref={ref} type="checkbox" checked={!!checked} onChange={onChange}
        style={{
          width:16, height:16, margin:0, cursor:'pointer',
          accentColor:'#4f46e5',
        }}/>
    </label>
  );
}

function Select({value, onChange, options, leftIcon, style}) {
  const Ic = leftIcon ? Icon[leftIcon] : null;
  return (
    <div style={{position:'relative', display:'inline-block', ...style}}>
      {Ic && <Ic size={14} stroke="#94a3b8" style={{position:'absolute',left:11,top:11,pointerEvents:'none'}}/>}
      <select value={value} onChange={e=>onChange?.(e.target.value)} style={{
        appearance:'none', WebkitAppearance:'none',
        border:'1px solid #e2e8f0', background:'#fff', borderRadius:8,
        padding: leftIcon ? '8px 32px 8px 32px' : '8px 32px 8px 12px',
        fontSize:13, fontFamily:'inherit', color:'#0f172a',
        height:36, cursor:'pointer', outline:'none',
      }}>
        {options.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
      </select>
      <Icon.ChevronDown size={14} stroke="#94a3b8" style={{position:'absolute',right:10,top:11,pointerEvents:'none'}}/>
    </div>
  );
}

// — main page —
function PaymentsList() {
  const [statusFilter, setStatusFilter] = React.useState('all');
  const [period, setPeriod] = React.useState('month');
  const [method, setMethod] = React.useState('all');
  const [query, setQuery] = React.useState('');
  const [selected, setSelected] = React.useState(new Set());
  const [page, setPage] = React.useState(1);
  const perPage = 10;

  const counts = React.useMemo(() => ({
    all: PAYMENTS.length,
    paid: PAYMENTS.filter(p=>p.status==='paid').length,
    pending: PAYMENTS.filter(p=>p.status==='pending').length,
    overdue: PAYMENTS.filter(p=>p.status==='overdue').length,
    cancelled: PAYMENTS.filter(p=>p.status==='cancelled').length,
  }), []);

  const filtered = React.useMemo(() => {
    return PAYMENTS.filter(p => {
      if (statusFilter !== 'all' && p.status !== statusFilter) return false;
      if (method !== 'all' && p.method !== method) return false;
      if (query) {
        const q = query.toLowerCase();
        if (!p.student.toLowerCase().includes(q) &&
            !p.id.toLowerCase().includes(q) &&
            !p.email.toLowerCase().includes(q) &&
            !p.course.toLowerCase().includes(q)) return false;
      }
      return true;
    });
  }, [statusFilter, method, query]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / perPage));
  const pageRows = filtered.slice((page-1)*perPage, page*perPage);
  React.useEffect(()=>{ if (page > totalPages) setPage(1); }, [totalPages, page]);

  const toggleAll = () => {
    if (pageRows.every(r => selected.has(r.id))) {
      const next = new Set(selected);
      pageRows.forEach(r => next.delete(r.id));
      setSelected(next);
    } else {
      const next = new Set(selected);
      pageRows.forEach(r => next.add(r.id));
      setSelected(next);
    }
  };
  const allChecked = pageRows.length>0 && pageRows.every(r => selected.has(r.id));
  const someChecked = pageRows.some(r => selected.has(r.id)) && !allChecked;

  return (
    <div style={{padding:28,display:'flex',flexDirection:'column',gap:18,background:'#f8fafc',minHeight:'100%'}}>
      {/* — KPIs — */}
      <div style={{display:'grid', gridTemplateColumns:'repeat(4,1fr)', gap:14}}>
        <PaymentKpi
          label="Выручка за май"
          value={fmtRub(284900)}
          hint={<><Icon.TrendingUp size={12} stroke="#10b981"/><span style={{color:'#059669',fontWeight:600}}>+8,4%</span> к апрелю</>}
          iconName="CircleDollarSign" iconBg="#ede9fe" iconColor="#7c3aed"/>
        <PaymentKpi
          label="Оплачено"
          value={`${counts.paid}`}
          hint={<>из&nbsp;{counts.all} платежей · {fmtRub(212400)}</>}
          iconName="CheckCircle2" iconBg="#d1fae5" iconColor="#059669"/>
        <PaymentKpi
          label="Ожидает оплаты"
          value={fmtRub(48600)}
          hint={<>{counts.pending}&nbsp;платежей · средний срок 3&nbsp;дня</>}
          iconName="Clock" iconBg="#fef3c7" iconColor="#b45309"/>
        <PaymentKpi
          label="Просрочено"
          value={fmtRub(56000)}
          hintColor="#b91c1c"
          hint={<><Icon.AlertCircle size={12} stroke="#ef4444"/><span style={{fontWeight:600}}>{counts.overdue}&nbsp;счёта</span> · напомнить</>}
          iconName="AlertCircle" iconBg="#fee2e2" iconColor="#dc2626"/>
      </div>

      {/* — Table card — */}
      <Card style={{padding:0, overflow:'hidden'}}>
        {/* status tabs + period select */}
        <div style={{
          display:'flex', alignItems:'center', justifyContent:'space-between', gap:12,
          padding:'14px 16px', borderBottom:'1px solid #e2e8f0',
        }}>
          <div style={{display:'flex',alignItems:'center',gap:4,flexWrap:'wrap'}}>
            <StatusTab label="Все"            count={counts.all}       active={statusFilter==='all'}       onClick={()=>setStatusFilter('all')}/>
            <StatusTab label="Оплачены"       count={counts.paid}      active={statusFilter==='paid'}      onClick={()=>setStatusFilter('paid')}      dotColor="#10b981"/>
            <StatusTab label="Ожидают"        count={counts.pending}   active={statusFilter==='pending'}   onClick={()=>setStatusFilter('pending')}   dotColor="#f59e0b"/>
            <StatusTab label="Просрочены"     count={counts.overdue}   active={statusFilter==='overdue'}   onClick={()=>setStatusFilter('overdue')}   dotColor="#ef4444"/>
            <StatusTab label="Отменены"       count={counts.cancelled} active={statusFilter==='cancelled'} onClick={()=>setStatusFilter('cancelled')} dotColor="#94a3b8"/>
          </div>
          <div style={{display:'flex',alignItems:'center',gap:8}}>
            <Select value={period} onChange={setPeriod} leftIcon="Calendar" options={[
              {value:'week', label:'За неделю'},
              {value:'month', label:'За май 2026'},
              {value:'quarter', label:'За квартал'},
              {value:'year', label:'За год'},
              {value:'all', label:'Всё время'},
            ]}/>
            <Select value={method} onChange={setMethod} leftIcon="Wallet" options={[
              {value:'all', label:'Все методы'},
              {value:'card', label:'Карта'},
              {value:'sbp', label:'СБП'},
              {value:'wire', label:'Банк. перевод'},
            ]}/>
          </div>
        </div>

        {/* search row */}
        <div style={{
          display:'flex', alignItems:'center', gap:10,
          padding:'12px 16px', borderBottom:'1px solid #e2e8f0',
        }}>
          <div style={{position:'relative', flex:1, maxWidth:380}}>
            <Icon.Search size={15} stroke="#94a3b8" style={{position:'absolute',left:12,top:11,pointerEvents:'none'}}/>
            <Input placeholder="Поиск по студенту, номеру счёта, курсу…"
              value={query} onChange={e=>setQuery(e.target.value)}
              style={{paddingLeft:36, height:36, fontSize:13}}/>
          </div>
          {selected.size>0 ? (
            <div style={{display:'flex',alignItems:'center',gap:10,marginLeft:'auto'}}>
              <span style={{fontSize:13,color:'#475569'}}>
                Выбрано <strong>{selected.size}</strong>
              </span>
              <Button variant="secondary" size="sm"><Icon.Send size={14}/>Напомнить</Button>
              <Button variant="secondary" size="sm"><Icon.Download size={14}/>Скачать счета</Button>
              <Button variant="ghost"     size="sm" onClick={()=>setSelected(new Set())}>Снять</Button>
            </div>
          ) : (
            <div style={{display:'flex',alignItems:'center',gap:8,marginLeft:'auto',color:'#64748b',fontSize:13}}>
              <span>Найдено: <strong style={{color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>{filtered.length}</strong></span>
            </div>
          )}
        </div>

        {/* table */}
        <div style={{overflowX:'auto'}}>
          <table style={{width:'100%', borderCollapse:'collapse', minWidth:980}}>
            <thead>
              <tr>
                <th style={{padding:'10px 16px', background:'#f8fafc', borderBottom:'1px solid #e2e8f0', width:36}}>
                  <Checkbox checked={allChecked} indeterminate={someChecked} onChange={toggleAll}/>
                </th>
                <TableHeader sortable>№ счёта</TableHeader>
                <TableHeader sortable>Студент</TableHeader>
                <TableHeader>Курс</TableHeader>
                <TableHeader sortable align="right">Сумма</TableHeader>
                <TableHeader>Метод</TableHeader>
                <TableHeader sortable>Дата</TableHeader>
                <TableHeader>Статус</TableHeader>
                <TableHeader align="right"> </TableHeader>
              </tr>
            </thead>
            <tbody>
              {pageRows.map(r => (
                <PaymentRow key={r.id} row={r}
                  selected={selected.has(r.id)}
                  onSelect={()=>{
                    const next = new Set(selected);
                    next.has(r.id) ? next.delete(r.id) : next.add(r.id);
                    setSelected(next);
                  }}/>
              ))}
              {pageRows.length===0 && (
                <tr><td colSpan={9} style={{padding:'56px 16px',textAlign:'center'}}>
                  <div style={{display:'inline-flex',flexDirection:'column',alignItems:'center',gap:10,color:'#94a3b8'}}>
                    <div style={{width:48,height:48,borderRadius:12,background:'#f1f5f9',display:'flex',alignItems:'center',justifyContent:'center',color:'#cbd5e1'}}>
                      <Icon.Receipt size={22}/>
                    </div>
                    <div style={{fontSize:14,fontWeight:600,color:'#334155'}}>Платежей не найдено</div>
                    <div style={{fontSize:12.5}}>Попробуйте изменить фильтры или очистить поиск</div>
                  </div>
                </td></tr>
              )}
            </tbody>
          </table>
        </div>

        {/* pagination */}
        <div style={{
          display:'flex', alignItems:'center', justifyContent:'space-between',
          padding:'14px 16px', borderTop:'1px solid #e2e8f0', background:'#fff',
        }}>
          <div style={{fontSize:13, color:'#64748b'}}>
            Показано <strong style={{color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>
              {filtered.length===0 ? 0 : (page-1)*perPage + 1}–{Math.min(page*perPage, filtered.length)}
            </strong> из <strong style={{color:'#0f172a',fontVariantNumeric:'tabular-nums'}}>{filtered.length}</strong>
          </div>
          <div style={{display:'flex',alignItems:'center',gap:6}}>
            <PageButton disabled={page===1} onClick={()=>setPage(1)}><Icon.ChevronsLeft size={14}/></PageButton>
            <PageButton disabled={page===1} onClick={()=>setPage(p=>Math.max(1,p-1))}><Icon.ChevronLeft size={14}/></PageButton>
            {Array.from({length: totalPages}).map((_,i)=>(
              <PageButton key={i} active={page===i+1} onClick={()=>setPage(i+1)}>{i+1}</PageButton>
            ))}
            <PageButton disabled={page===totalPages} onClick={()=>setPage(p=>Math.min(totalPages,p+1))}><Icon.ChevronRight size={14}/></PageButton>
            <PageButton disabled={page===totalPages} onClick={()=>setPage(totalPages)}><Icon.ChevronsRight size={14}/></PageButton>
          </div>
        </div>
      </Card>
    </div>
  );
}

window.PaymentsList = PaymentsList;
