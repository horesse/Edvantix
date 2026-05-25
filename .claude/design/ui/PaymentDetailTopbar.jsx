// Topbar for the Payment Detail page.
function PaymentDetailTopbar({payment}) {
  return (
    <div style={{
      display:'flex',alignItems:'center',justifyContent:'space-between',
      padding:'18px 28px',gap:20,borderBottom:'1px solid #e2e8f0',
      background:'#fff',
    }}>
      <div style={{flex:1,minWidth:0,display:'flex',alignItems:'center',gap:14}}>
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
            <span style={{color:'#475569',fontWeight:500,fontVariantNumeric:'tabular-nums'}}>{payment.id}</span>
          </div>
          <div style={{display:'flex',alignItems:'center',gap:12}}>
            <h1 style={{margin:0,fontSize:22,fontWeight:700,letterSpacing:'-0.02em',fontVariantNumeric:'tabular-nums'}}>
              Платёж {payment.id}
            </h1>
            <Badge variant="success" dot>Оплачен</Badge>
          </div>
          <div style={{fontSize:13,color:'#64748b',marginTop:2}}>
            Поступил 21 мая 2026 в 11:42 · {payment.student.name} · {payment.course.name}
          </div>
        </div>
      </div>
      <div style={{display:'flex',alignItems:'center',gap:8,flexShrink:0}}>
        <Button variant="secondary" size="md" style={{width:36,padding:0,height:36}} title="Печать">
          <Icon.Printer size={15}/>
        </Button>
        <Button variant="secondary"><Icon.Send size={14}/>Отправить копию</Button>
        <Button variant="secondary"><Icon.Download size={14}/>Счёт PDF</Button>
        <Button variant="secondary" style={{color:'#b91c1c',borderColor:'#fecaca'}}>
          <Icon.RotateCcw size={14}/>Вернуть платёж
        </Button>
        <Button variant="secondary" size="md" style={{width:36,padding:0,height:36}} title="Ещё">
          <Icon.MoreHorizontal size={16}/>
        </Button>
      </div>
    </div>
  );
}

window.PaymentDetailTopbar = PaymentDetailTopbar;
