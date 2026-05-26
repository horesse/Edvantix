// Topbar for the Subscription / Billing page.
function SubscriptionTopbar() {
  return (
    <div style={{
      display:'flex',alignItems:'center',justifyContent:'space-between',
      padding:'20px 28px',gap:20,borderBottom:'1px solid #e2e8f0',
      background:'#fff',
    }}>
      <div style={{flex:1,minWidth:0}}>
        <div style={{display:'flex',alignItems:'center',gap:8,fontSize:12,color:'#94a3b8',marginBottom:4}}>
          <span>Система</span>
          <Icon.ChevronRight size={12} stroke="#cbd5e1"/>
          <span style={{color:'#475569',fontWeight:500}}>Подписка</span>
        </div>
        <div style={{display:'flex',alignItems:'center',gap:12}}>
          <h1 style={{margin:0,fontSize:24,fontWeight:700,letterSpacing:'-0.02em'}}>Подписка</h1>
          <Badge variant="primary" dot>План&nbsp;Pro</Badge>
        </div>
        <div style={{fontSize:13,color:'#64748b',marginTop:2}}>
          Школа «Креатив Плюс» · биллинг и тариф Edvantix
        </div>
      </div>
      <div style={{display:'flex',alignItems:'center',gap:10}}>
        <Button variant="secondary" size="md" style={{width:36,padding:0,height:36,position:'relative'}}>
          <Icon.Bell size={16}/>
          <span style={{position:'absolute',top:6,right:7,width:8,height:8,background:'#ef4444',borderRadius:9999,border:'2px solid #fff'}}/>
        </Button>
        <Button variant="secondary"><Icon.FileText size={15}/>Все счета</Button>
        <Button><Icon.ArrowUpRight size={16}/>Сменить план</Button>
      </div>
    </div>
  );
}

window.SubscriptionTopbar = SubscriptionTopbar;
