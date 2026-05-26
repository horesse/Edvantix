// Sidebar — matches app-sidebar.tsx structure
const sidebarSections = [
  { id: 'overview', label: 'Обзор', items: [
    { id: 'dashboard', title: 'Дашборд', icon: 'LayoutDashboard' },
  ]},
  { id: 'school', label: 'Школа', items: [
    { id: 'students', title: 'Студенты', icon: 'GraduationCap' },
    { id: 'courses', title: 'Курсы', icon: 'BookOpen' },
    { id: 'schedule', title: 'Расписание', icon: 'CalendarDays' },
    { id: 'attendance', title: 'Посещаемость', icon: 'BarChart2' },
  ]},
  { id: 'finance', label: 'Финансы', items: [
    { id: 'payments', title: 'Платежи', icon: 'CreditCard' },
    { id: 'invoices', title: 'Счета', icon: 'Receipt' },
  ]},
  { id: 'users', label: 'Пользователи', items: [
    { id: 'profiles', title: 'Профили', icon: 'Users' },
    { id: 'org', title: 'Организация', icon: 'Building2' },
  ]},
  { id: 'system', label: 'Система', items: [
    { id: 'subscription', title: 'Подписка', icon: 'CircleDollarSign' },
    { id: 'settings', title: 'Настройки', icon: 'Settings' },
  ]},
];

function SidebarLogo() {
  return (
    <div style={{display:'flex',alignItems:'center',gap:10}}>
      <div style={{
        width:32,height:32,borderRadius:8,background:'#4f46e5',
        display:'flex',alignItems:'center',justifyContent:'center',
        boxShadow:'0 2px 8px rgba(79,70,229,0.25)',
      }}>
        <Icon.GraduationCap size={18} stroke="#fff"/>
      </div>
      <div style={{fontSize:17,fontWeight:700,letterSpacing:'-0.02em'}}>
        Edv<span style={{color:'#4f46e5'}}>antix</span>
      </div>
    </div>
  );
}

function NavItem({icon, label, active, onClick}) {
  const [hover, setHover] = React.useState(false);
  const Ic = Icon[icon];
  const bg = active ? 'rgba(79,70,229,0.10)' : hover ? '#f1f5f9' : 'transparent';
  const fg = active ? '#4f46e5' : '#334155';
  const bd = active ? '1px solid rgba(79,70,229,0.15)' : '1px solid transparent';
  return (
    <button onClick={onClick} onMouseEnter={()=>setHover(true)} onMouseLeave={()=>setHover(false)}
      style={{
        display:'flex',alignItems:'center',gap:10,width:'100%',
        padding:'8px 12px',borderRadius:8,border:bd,background:bg,color:fg,
        fontSize:13.5,fontWeight: active?600:500,textAlign:'left',cursor:'pointer',
        transition:'.1s',
      }}>
      <Ic size={16}/>{label}
    </button>
  );
}

function SidebarUser() {
  return (
    <div style={{display:'flex',alignItems:'center',gap:10,padding:'4px 8px'}}>
      <Avatar name="Анна Мельникова" size={36}/>
      <div style={{flex:1,minWidth:0}}>
        <div style={{fontSize:13,fontWeight:600}}>Анна Мельникова</div>
        <div style={{fontSize:11,color:'#94a3b8',overflow:'hidden',textOverflow:'ellipsis',whiteSpace:'nowrap'}}>a.melnikova@school.ru</div>
      </div>
      <Icon.ChevronRight size={14} stroke="#94a3b8"/>
    </div>
  );
}

function Sidebar({active='dashboard', onNavigate}) {
  return (
    <aside style={{
      width:240,flexShrink:0,display:'flex',flexDirection:'column',
      background:'#fff',borderRight:'1px solid #e2e8f0',height:'100%',
    }}>
      <div style={{padding:'16px 20px',borderBottom:'1px solid #e2e8f0'}}>
        <SidebarLogo/>
      </div>
      <nav style={{flex:1,overflowY:'auto',padding:'12px 12px'}}>
        {sidebarSections.map((s,i)=>(
          <div key={s.id} style={{marginTop: i>0?12:0, paddingTop: i>0?12:0, borderTop: i>0?'1px solid #f1f5f9':'0'}}>
            <SectionLabel>{s.label}</SectionLabel>
            <div style={{display:'flex',flexDirection:'column',gap:2}}>
              {s.items.map(it=>(
                <NavItem key={it.id} icon={it.icon} label={it.title} active={active===it.id}
                  onClick={()=>onNavigate?.(it.id)}/>
              ))}
            </div>
          </div>
        ))}
      </nav>
      <div style={{borderTop:'1px solid #e2e8f0',padding:12}}>
        <SidebarUser/>
      </div>
    </aside>
  );
}

window.Sidebar = Sidebar;
