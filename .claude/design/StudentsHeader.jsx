// === Page-level Tweaks ===

const STATUS_META = {
  active:  { label: 'Активный',          dotColor: '#10b981', bg: '#ecfdf5', fg: '#047857' },
  new:     { label: 'Новый',             dotColor: '#6366f1', bg: '#eef2ff', fg: '#4338ca' },
  paused:  { label: 'Приостановлен',     dotColor: '#94a3b8', bg: '#f1f5f9', fg: '#475569' },
};
const PAYMENT_META = {
  paid:    { label: 'Оплачено',          variant: 'success' },
  pending: { label: 'Ожидает оплаты',    variant: 'warning' },
  overdue: { label: 'Просрочено',        variant: 'danger'  },
};

// ─── KPI strip ──────────────────────────────────────────────────────────
function StudentsKpiStrip() {
  const cards = [
    { label: 'Всего студентов',  value: '248',  sub: '+12 за месяц',     trend: 'up',
      icon: 'GraduationCap', iconBg: '#e0eaff', iconColor: '#4f46e5' },
    { label: 'Активные',         value: '231',  sub: '93% от общего',    trend: 'flat',
      icon: 'CheckCircle2',  iconBg: '#d1fae5', iconColor: '#059669' },
    { label: 'Новые в апреле',   value: '24',   sub: '+18% vs март',     trend: 'up',
      icon: 'UserPlus',      iconBg: '#fef3c7', iconColor: '#b45309' },
    { label: 'Задолженность',    value: '₽47 200', sub: '7 студентов',   trend: 'warn',
      icon: 'AlertCircle',   iconBg: '#fee2e2', iconColor: '#b91c1c' },
  ];
  return (
    <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:14}}>
      {cards.map(c => {
        const Ic = Icon[c.icon];
        return (
          <Card key={c.label} style={{padding:20, display:'flex', flexDirection:'column', gap:14}}>
            <div style={{display:'flex', alignItems:'center', justifyContent:'space-between'}}>
              <span style={{fontSize:12, color:'#64748b', fontWeight:500}}>{c.label}</span>
              <div style={{
                width:32, height:32, borderRadius:8, background:c.iconBg, color:c.iconColor,
                display:'flex', alignItems:'center', justifyContent:'center'
              }}><Ic size={16}/></div>
            </div>
            <div style={{display:'flex', alignItems:'flex-end', justifyContent:'space-between', gap:8}}>
              <span style={{fontSize:26, fontWeight:700, fontVariantNumeric:'tabular-nums', letterSpacing:'-0.02em'}}>
                {c.value}
              </span>
              <span style={{
                display:'inline-flex', alignItems:'center', gap:3, fontSize:12, fontWeight:500,
                color: c.trend==='up' ? '#059669' : c.trend==='warn' ? '#b91c1c' : '#94a3b8'
              }}>
                {c.trend === 'up'   && <Icon.TrendingUp size={14}/>}
                {c.trend === 'warn' && <Icon.AlertCircle size={13}/>}
                {c.sub}
              </span>
            </div>
          </Card>
        );
      })}
    </div>
  );
}

// ─── Tabs ───────────────────────────────────────────────────────────────
function SegmentedTabs({tabs, value, onChange}) {
  return (
    <div style={{
      display:'inline-flex', background:'#f1f5f9', padding:4, borderRadius:10,
      border:'1px solid #e2e8f0', gap:2
    }}>
      {tabs.map(t => {
        const active = value === t.id;
        return (
          <button key={t.id} onClick={()=>onChange(t.id)}
            style={{
              display:'inline-flex', alignItems:'center', gap:8,
              padding:'6px 12px', borderRadius:7, border:'1px solid transparent',
              background: active ? '#fff' : 'transparent',
              color: active ? '#0f172a' : '#475569',
              fontSize:13, fontWeight: active ? 600 : 500, cursor:'pointer',
              boxShadow: active ? '0 1px 2px rgba(0,0,0,0.06)' : 'none',
              transition:'.1s',
            }}>
            {t.label}
            <span style={{
              fontSize:11, fontWeight:600, fontVariantNumeric:'tabular-nums',
              padding:'1px 7px', borderRadius:9999,
              background: active ? (t.tone==='danger'?'#fee2e2':'#e0eaff')
                                 : '#e2e8f0',
              color: active ? (t.tone==='danger'?'#b91c1c':'#4338ca') : '#64748b',
            }}>{t.count}</span>
          </button>
        );
      })}
    </div>
  );
}

// ─── Toolbar (filters) ──────────────────────────────────────────────────
function FilterDropdown({label, value, icon}) {
  const [hover, setHover] = React.useState(false);
  const Ic = icon ? Icon[icon] : null;
  return (
    <button
      onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        display:'inline-flex', alignItems:'center', gap:8, height:36,
        padding:'0 12px', borderRadius:8, border:'1px solid #e2e8f0',
        background: hover ? '#f8fafc' : '#fff', color:'#0f172a',
        fontSize:13, fontWeight:500, cursor:'pointer', transition:'.1s'
      }}>
      {Ic && <Ic size={14} stroke="#64748b"/>}
      <span style={{color:'#64748b'}}>{label}:</span>
      <span>{value}</span>
      <Icon.ChevronDown size={14} stroke="#94a3b8"/>
    </button>
  );
}

function StudentsToolbar({tab, setTab, query, setQuery}) {
  const tabs = [
    { id:'all',       label:'Все',              count: 248 },
    { id:'active',    label:'Активные',         count: 231 },
    { id:'paused',    label:'Приостановлены',   count: 10 },
    { id:'overdue',   label:'С долгом',         count: 7, tone:'danger' },
    { id:'new',       label:'Новые',            count: 24 },
  ];
  return (
    <div style={{display:'flex', flexDirection:'column', gap:12}}>
      <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', gap:16, flexWrap:'wrap'}}>
        <SegmentedTabs tabs={tabs} value={tab} onChange={setTab}/>
        <div style={{display:'flex', alignItems:'center', gap:8}}>
          <div style={{position:'relative'}}>
            <Icon.Search size={14} stroke="#94a3b8"
              style={{position:'absolute', left:11, top:11, pointerEvents:'none'}}/>
            <Input
              placeholder="Имя, email, телефон…"
              value={query} onChange={e=>setQuery(e.target.value)}
              style={{width:260, paddingLeft:32, height:36, fontSize:13, borderRadius:8}}/>
          </div>
          <FilterDropdown label="Курс"        value="Все"        icon="BookOpen"/>
          <FilterDropdown label="Группа"      value="Любая"      icon="Users"/>
          <FilterDropdown label="Оплата"      value="Все"        icon="CreditCard"/>
          <button style={{
            display:'inline-flex', alignItems:'center', gap:6, height:36, padding:'0 12px',
            borderRadius:8, border:'1px solid #e2e8f0', background:'#fff',
            color:'#475569', fontSize:13, fontWeight:500, cursor:'pointer'
          }}>
            <Icon.SlidersHorizontal size={14} stroke="#64748b"/>
            Ещё фильтры
          </button>
        </div>
      </div>
    </div>
  );
}

// ─── Bulk action bar ────────────────────────────────────────────────────
function BulkBar({count, onClear}) {
  return (
    <div style={{
      display:'flex', alignItems:'center', justifyContent:'space-between',
      background:'#eef2ff', border:'1px solid #c7d2fe', borderRadius:12,
      padding:'10px 16px', gap:16
    }}>
      <div style={{display:'flex', alignItems:'center', gap:10}}>
        <div style={{
          width:28, height:28, borderRadius:8, background:'#4f46e5', color:'#fff',
          display:'flex', alignItems:'center', justifyContent:'center', fontSize:12, fontWeight:700
        }}>{count}</div>
        <span style={{fontSize:13.5, fontWeight:600, color:'#1e1b4b'}}>
          выбрано{' '}
          <span style={{color:'#4338ca', fontWeight:500}}>
            · действие будет применено ко всем
          </span>
        </span>
      </div>
      <div style={{display:'flex', alignItems:'center', gap:6}}>
        <BulkBtn icon="Send" label="Сообщение"/>
        <BulkBtn icon="BookOpen" label="Назначить курс"/>
        <BulkBtn icon="Tag" label="Тег"/>
        <BulkBtn icon="Download" label="Экспорт"/>
        <BulkBtn icon="Trash" label="Удалить" danger/>
        <button onClick={onClear} style={{
          marginLeft:6, width:28, height:28, borderRadius:6, border:'1px solid #c7d2fe',
          background:'transparent', display:'inline-flex', alignItems:'center', justifyContent:'center',
          color:'#4338ca', cursor:'pointer'
        }}><Icon.X size={14}/></button>
      </div>
    </div>
  );
}
function BulkBtn({icon, label, danger}) {
  const [h, sh] = React.useState(false);
  const Ic = Icon[icon];
  return (
    <button onMouseEnter={()=>sh(true)} onMouseLeave={()=>sh(false)} style={{
      display:'inline-flex', alignItems:'center', gap:6, height:30, padding:'0 10px',
      borderRadius:7, border:'1px solid '+(h ? (danger?'#fecaca':'#c7d2fe') : 'transparent'),
      background: h ? (danger?'#fee2e2':'#e0e7ff') : 'transparent',
      color: danger ? '#b91c1c' : '#3730a3', fontSize:12.5, fontWeight:600,
      cursor:'pointer', transition:'.1s'
    }}>
      <Ic size={13}/>{label}
    </button>
  );
}

window.StudentsKpiStrip = StudentsKpiStrip;
window.StudentsToolbar = StudentsToolbar;
window.BulkBar = BulkBar;
window.STATUS_META = STATUS_META;
window.PAYMENT_META = PAYMENT_META;
