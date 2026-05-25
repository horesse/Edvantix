// Topbar for the "Принять платёж" page.
function PaymentCreateTopbar({mode, onMode}) {
  return (
    <div style={{
      display:'flex',alignItems:'center',justifyContent:'space-between',
      padding:'14px 24px',gap:16,borderBottom:'1px solid #e2e8f0',
      background:'#fff',flexWrap:'wrap',
    }}>
      <div style={{flex:'1 1 320px',minWidth:0,display:'flex',alignItems:'center',gap:14}}>
        <a href="Payments.html" title="Назад к платежам"
          style={{
            width:36,height:36,borderRadius:8,border:'1px solid #e2e8f0',
            background:'#fff',color:'#475569',cursor:'pointer',
            display:'inline-flex',alignItems:'center',justifyContent:'center',
            flexShrink:0,transition:'.1s',
          }}
          onMouseEnter={e=>{e.currentTarget.style.background='#f8fafc'}}
          onMouseLeave={e=>{e.currentTarget.style.background='#fff'}}>
          <Icon.ArrowLeft size={16}/>
        </a>
        <div style={{minWidth:0}}>
          <div style={{display:'flex',alignItems:'center',gap:8,fontSize:12,color:'#94a3b8',marginBottom:4}}>
            <span>Финансы</span>
            <Icon.ChevronRight size={12} stroke="#cbd5e1"/>
            <a href="Payments.html" style={{color:'#64748b'}}>Платежи</a>
            <Icon.ChevronRight size={12} stroke="#cbd5e1"/>
            <span style={{color:'#475569',fontWeight:500}}>Новый платёж</span>
          </div>
          <div style={{display:'flex',alignItems:'center',gap:12,flexWrap:'wrap'}}>
            <h1 style={{margin:0,fontSize:22,fontWeight:700,letterSpacing:'-0.02em',whiteSpace:'nowrap'}}>
              Принять платёж
            </h1>
            <span style={{
              display:'inline-flex',alignItems:'center',gap:6,
              background:'#fef3c7',color:'#92400e',padding:'3px 10px',borderRadius:9999,
              fontSize:12,fontWeight:500,
            }}>
              <span style={{width:6,height:6,borderRadius:9999,background:'currentColor'}}/>
              Черновик
            </span>
          </div>
          <div style={{fontSize:13,color:'#64748b',marginTop:2}}>
            Зарегистрируйте оплату от&nbsp;студента или&nbsp;отправьте ссылку для&nbsp;оплаты
          </div>
        </div>
      </div>

      {/* Segmented: режим — принять сразу vs запросить ссылку */}
      <div style={{
        display:'inline-flex',padding:3,background:'#f1f5f9',borderRadius:10,
        border:'1px solid #e2e8f0',flexShrink:0,
      }}>
        {[
          {id:'accept',  label:'Принять сейчас', icon:'CreditCard'},
          {id:'request', label:'Запросить оплату', icon:'Send'},
        ].map(m=>{
          const Ic = Icon[m.icon];
          const on = mode===m.id;
          return (
            <button key={m.id} onClick={()=>onMode(m.id)} style={{
              display:'inline-flex',alignItems:'center',gap:7,
              padding:'7px 14px',borderRadius:8,border:'1px solid transparent',
              background: on ? '#fff' : 'transparent',
              boxShadow: on ? '0 1px 2px rgba(15,23,42,0.06), 0 0 0 1px rgba(15,23,42,0.04)' : 'none',
              color: on ? '#0f172a' : '#64748b',
              fontSize:13,fontWeight:600,fontFamily:'inherit',cursor:'pointer',transition:'.12s',
            }}>
              <Ic size={14} stroke={on?'#4f46e5':'#94a3b8'}/>
              {m.label}
            </button>
          );
        })}
      </div>

      <div style={{display:'flex',alignItems:'center',gap:8,flexShrink:0,flexWrap:'wrap'}}>
        <Button variant="secondary"><Icon.FileText size={14}/>Из шаблона</Button>
        <Button variant="ghost" style={{color:'#64748b'}}>Отменить</Button>
      </div>
    </div>
  );
}

window.PaymentCreateTopbar = PaymentCreateTopbar;
