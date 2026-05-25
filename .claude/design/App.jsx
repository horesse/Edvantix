function StudentsPage() {
  const [tab, setTab]           = React.useState('all');
  const [query, setQuery]       = React.useState('');
  const [selected, setSelected] = React.useState({});
  const [sort, setSort]         = React.useState({key:'lastActive', dir:'desc'});
  const [openStudent, setOpen]  = React.useState(null);

  const selectedCount = Object.values(selected).filter(Boolean).length;

  const rows = React.useMemo(() => {
    let r = STUDENTS.slice();
    if (tab === 'active')  r = r.filter(s => s.status === 'active');
    if (tab === 'paused')  r = r.filter(s => s.status === 'paused');
    if (tab === 'new')     r = r.filter(s => s.status === 'new');
    if (tab === 'overdue') r = r.filter(s => s.payment === 'overdue');
    if (query) {
      const q = query.toLowerCase();
      r = r.filter(s =>
        s.name.toLowerCase().includes(q) ||
        s.email.toLowerCase().includes(q) ||
        s.phone.toLowerCase().includes(q) ||
        s.courses.some(c => c.toLowerCase().includes(q)) ||
        s.group.toLowerCase().includes(q)
      );
    }
    // simplistic sort
    const k = sort.key, dir = sort.dir === 'asc' ? 1 : -1;
    r.sort((a,b) => {
      let va = a[k], vb = b[k];
      if (k === 'courses') { va = a.courses.length; vb = b.courses.length; }
      if (typeof va === 'string') return va.localeCompare(vb, 'ru') * dir;
      return (va - vb) * dir;
    });
    return r;
  }, [tab, query, sort]);

  return (
    <div style={{display:'flex', flexDirection:'column', minHeight:'100%'}}>
      {/* Custom topbar for Students */}
      <div style={{
        background:'#fff', borderBottom:'1px solid #e2e8f0',
        padding:'20px 28px', display:'flex', alignItems:'center', justifyContent:'space-between', gap:20
      }}>
        <div>
          <div style={{display:'flex', alignItems:'center', gap:8, fontSize:12.5, color:'#64748b', marginBottom:4}}>
            <span>Школа</span>
            <Icon.ChevronRight size={12}/>
            <span style={{color:'#0f172a', fontWeight:600}}>Студенты</span>
          </div>
          <h1 style={{margin:0, fontSize:24, fontWeight:700, letterSpacing:'-0.02em'}}>
            Студенты <span style={{color:'#94a3b8', fontWeight:500}}>248</span>
          </h1>
          <div style={{fontSize:13, color:'#64748b', marginTop:4}}>
            Школа «Креатив Плюс» · 18 групп · последняя синхронизация — 2 мин назад
          </div>
        </div>
        <div style={{display:'flex', alignItems:'center', gap:8}}>
          <Button variant="ghost" size="md" style={{color:'#475569', borderColor:'transparent'}}>
            <Icon.Download size={15}/>Экспорт
          </Button>
          <Button variant="secondary" size="md">
            <Icon.Upload size={15}/>Импорт CSV
          </Button>
          <Button variant="secondary" size="md">
            <Icon.Mail size={15}/>Пригласить
          </Button>
          <Button variant="primary" size="md">
            <Icon.Plus size={16}/>Добавить студента
          </Button>
        </div>
      </div>

      {/* Body */}
      <div style={{padding:28, display:'flex', flexDirection:'column', gap:18, background:'#f8fafc', flex:1, minHeight:0}}>
        <StudentsKpiStrip/>
        <StudentsToolbar tab={tab} setTab={setTab} query={query} setQuery={setQuery}/>
        {selectedCount > 0 && <BulkBar count={selectedCount} onClear={()=>setSelected({})}/>}
        <StudentsTable
          rows={rows}
          selected={selected}
          setSelected={setSelected}
          sort={sort}
          setSort={setSort}
          onOpenRow={setOpen}
        />
      </div>

      <StudentDrawer student={openStudent} onClose={()=>setOpen(null)}/>
    </div>
  );
}

function App() {
  return (
    <div style={{display:'flex', height:'100vh', overflow:'hidden'}}>
      <Sidebar active="students"/>
      <div style={{flex:1, display:'flex', flexDirection:'column', minWidth:0, overflow:'hidden'}}>
        <div style={{flex:1, overflowY:'auto'}}>
          <StudentsPage/>
        </div>
      </div>
    </div>
  );
}

window.App = App;
