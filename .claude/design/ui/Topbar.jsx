function Topbar({title='Дашборд', subtitle, action}) {
  return (
    <div style={{
      display:'flex',alignItems:'center',justifyContent:'space-between',
      padding:'20px 28px',gap:20,borderBottom:'1px solid #e2e8f0',
      background:'#fff',
    }}>
      <div style={{flex:1,minWidth:0}}>
        <h1 style={{margin:0,fontSize:24,fontWeight:700,letterSpacing:'-0.02em'}}>{title}</h1>
        {subtitle && <div style={{fontSize:13,color:'#64748b',marginTop:2}}>{subtitle}</div>}
      </div>
      <div style={{display:'flex',alignItems:'center',gap:10}}>
        <div style={{position:'relative'}}>
          <Icon.Search size={15} stroke="#94a3b8" style={{position:'absolute',left:10,top:10,pointerEvents:'none'}}/>
          <Input placeholder="Поиск…" style={{width:220,paddingLeft:32,height:36,fontSize:13}}/>
        </div>
        <Button variant="secondary" size="md" style={{width:36,padding:0,height:36,position:'relative'}}>
          <Icon.Bell size={16}/>
          <span style={{position:'absolute',top:6,right:7,width:8,height:8,background:'#ef4444',borderRadius:9999,border:'2px solid #fff'}}/>
        </Button>
        {action || <Button><Icon.Plus size={16}/>Добавить студента</Button>}
      </div>
    </div>
  );
}

window.Topbar = Topbar;
