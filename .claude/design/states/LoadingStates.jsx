// Loading states — page-level skeletons, inline, full app boot.

// ─── 1. Full-page skeleton (Students table) ───────────────────────────────
function LoadingStudentsSkeleton() {
  const rows = 7;
  return (
    <AppFrame active="students"
      crumbs={['Школа','Студенты']}
      title="Студенты"
      action={
        <div style={{display:'flex',gap:8}}>
          <Button variant="secondary" disabled><Icon.Upload size={14}/>Импорт</Button>
          <Button variant="primary" disabled><Icon.UserPlus size={14}/>Добавить</Button>
        </div>
      }
    >
      <div style={{display:'flex', flexDirection:'column', gap:16, height:'100%'}}>
        {/* KPI strip skeleton */}
        <div style={{display:'grid', gridTemplateColumns:'repeat(4,1fr)', gap:14}}>
          {[0,1,2,3].map(i=>(
            <Card key={i} style={{padding:20, display:'flex', flexDirection:'column', gap:14}}>
              <div style={{display:'flex', justifyContent:'space-between', alignItems:'center'}}>
                <Shimmer w={90} h={11}/>
                <Shimmer w={32} h={32} r={8}/>
              </div>
              <Shimmer w={120} h={26} r={8}/>
              <Shimmer w={70} h={10}/>
            </Card>
          ))}
        </div>
        {/* Toolbar skeleton */}
        <div style={{display:'flex', justifyContent:'space-between', alignItems:'center'}}>
          <Shimmer w={420} h={36} r={10}/>
          <div style={{display:'flex', gap:8}}>
            <Shimmer w={240} h={36} r={8}/>
            {[0,1,2].map(i=><Shimmer key={i} w={110} h={36} r={8}/>)}
          </div>
        </div>
        {/* Table */}
        <Card style={{flex:1, padding:0, overflow:'hidden'}}>
          <div style={{
            display:'grid', gridTemplateColumns:'24px 2fr 1.2fr 1.5fr 1fr 1fr 32px',
            gap:16, padding:'14px 20px', borderBottom:'1px solid #f1f5f9'
          }}>
            <Shimmer w={16} h={16} r={4}/>
            <Shimmer w={60} h={11}/>
            <Shimmer w={50} h={11}/>
            <Shimmer w={45} h={11}/>
            <Shimmer w={60} h={11}/>
            <Shimmer w={50} h={11}/>
            <span/>
          </div>
          {Array.from({length:rows}).map((_,r)=>(
            <div key={r} style={{
              display:'grid', gridTemplateColumns:'24px 2fr 1.2fr 1.5fr 1fr 1fr 32px',
              gap:16, padding:'16px 20px', borderBottom: r<rows-1 ? '1px solid #f1f5f9' : 0,
              alignItems:'center'
            }}>
              <Shimmer w={16} h={16} r={4}/>
              <div style={{display:'flex', alignItems:'center', gap:12}}>
                <Shimmer w={32} h={32} r={9999}/>
                <div style={{display:'flex', flexDirection:'column', gap:6}}>
                  <Shimmer w={120 + (r*13)%80} h={12}/>
                  <Shimmer w={80 + (r*19)%60} h={10}/>
                </div>
              </div>
              <Shimmer w={92} h={20} r={9999}/>
              <div style={{display:'flex', gap:6}}>
                <Shimmer w={70} h={20} r={6}/>
                {r%2===0 && <Shimmer w={50} h={20} r={6}/>}
              </div>
              <Shimmer w={120} h={6} r={9999}/>
              <Shimmer w={80} h={20} r={9999}/>
              <Shimmer w={20} h={20} r={6}/>
            </div>
          ))}
        </Card>
      </div>
    </AppFrame>
  );
}

// ─── 2. Dashboard widgets skeleton (with refresh indicator on top) ────────
function LoadingDashboardSkeleton() {
  return (
    <AppFrame active="dashboard"
      crumbs={['Обзор','Дашборд']}
      title="Школа «Креатив Плюс»"
      action={
        <div style={{display:'flex',alignItems:'center',gap:12}}>
          <span style={{display:'inline-flex',alignItems:'center',gap:8, fontSize:12.5, color:'#64748b'}}>
            <Spinner size={14} thickness={2}/>Обновляем данные…
          </span>
          <Button variant="secondary" disabled>Апрель 2026 <Icon.ChevronDown size={13}/></Button>
        </div>
      }
    >
      {/* indeterminate progress strip below header */}
      <div style={{margin:'-24px -32px 16px', height:2, background:'#eef2ff', position:'relative', overflow:'hidden'}}>
        <div style={{
          position:'absolute', top:0, bottom:0, width:'30%',
          background:'linear-gradient(90deg, transparent, #4f46e5, transparent)',
          animation:'edv-indeterminate 1.6s ease-in-out infinite',
        }}/>
        <style>{`@keyframes edv-indeterminate { 0%{left:-30%;} 100%{left:100%;} }`}</style>
      </div>

      <div style={{display:'flex', flexDirection:'column', gap:16, height:'100%'}}>
        <div style={{display:'grid', gridTemplateColumns:'repeat(4,1fr)', gap:14}}>
          {[0,1,2,3].map(i=>(
            <Card key={i} style={{padding:20, display:'flex', flexDirection:'column', gap:14}}>
              <div style={{display:'flex', justifyContent:'space-between'}}>
                <Shimmer w={100} h={11}/>
                <Shimmer w={34} h={34} r={9}/>
              </div>
              <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-end'}}>
                <div style={{display:'flex', flexDirection:'column', gap:6}}>
                  <Shimmer w={100} h={26} r={8}/>
                  <Shimmer w={70} h={10}/>
                </div>
                <Shimmer w={70} h={28} r={4}/>
              </div>
            </Card>
          ))}
        </div>

        <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:16, flex:1}}>
          <Card style={{padding:20, display:'flex', flexDirection:'column', gap:14}}>
            <div style={{display:'flex', justifyContent:'space-between'}}>
              <div style={{display:'flex', flexDirection:'column', gap:6}}>
                <Shimmer w={140} h={13}/>
                <Shimmer w={90} h={10}/>
              </div>
              <Shimmer w={150} h={32} r={8}/>
            </div>
            {/* ghost bars */}
            <div style={{flex:1, display:'flex', alignItems:'flex-end', gap:8, padding:'14px 4px 0'}}>
              {Array.from({length:14}).map((_,i)=>(
                <Shimmer key={i} w={`${100/14}%`} h={`${30 + ((i*37)%55)}%`} r={6}/>
              ))}
            </div>
          </Card>
          <Card style={{padding:20, display:'flex', flexDirection:'column', gap:14}}>
            <div style={{display:'flex', justifyContent:'space-between'}}>
              <Shimmer w={120} h={13}/>
              <Shimmer w={50} h={11}/>
            </div>
            {Array.from({length:5}).map((_,i)=>(
              <div key={i} style={{display:'flex', alignItems:'center', gap:10}}>
                <Shimmer w={32} h={32} r={9999}/>
                <div style={{flex:1, display:'flex', flexDirection:'column', gap:6}}>
                  <Shimmer w={`${60 + (i*17)%40}%`} h={11}/>
                  <Shimmer w={`${40 + (i*23)%30}%`} h={9}/>
                </div>
                <Shimmer w={50} h={18} r={9999}/>
              </div>
            ))}
          </Card>
        </div>
      </div>
    </AppFrame>
  );
}

// ─── 3. Initial app boot — centered logo spinner ──────────────────────────
function LoadingAppBoot() {
  return (
    <div style={{
      width:1280, height:800, background:'#f8fafc',
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a',
      display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center', gap:24,
      position:'relative', overflow:'hidden'
    }}>
      {/* subtle aurora */}
      <div style={{position:'absolute', top:'15%', left:'30%', width:380, height:380, borderRadius:'50%', background:'radial-gradient(circle, rgba(79,70,229,0.08), transparent 70%)', filter:'blur(40px)'}}/>
      <div style={{position:'absolute', bottom:'10%', right:'25%', width:320, height:320, borderRadius:'50%', background:'radial-gradient(circle, rgba(139,92,246,0.06), transparent 70%)', filter:'blur(40px)'}}/>

      {/* Logo with orbiting rings */}
      <div style={{position:'relative', width:96, height:96, display:'flex', alignItems:'center', justifyContent:'center'}}>
        <div style={{
          position:'absolute', inset:0, borderRadius:9999,
          border:'2px solid #4f46e5', borderTopColor:'transparent', borderRightColor:'transparent',
          animation:'edv-spin 1.4s linear infinite'
        }}/>
        <div style={{
          position:'absolute', inset:10, borderRadius:9999,
          border:'2px solid #818cf8', borderBottomColor:'transparent', borderLeftColor:'transparent',
          animation:'edv-spin 1.8s linear infinite reverse'
        }}/>
        <div style={{
          width:56, height:56, borderRadius:14, background:'#4f46e5',
          display:'flex', alignItems:'center', justifyContent:'center',
          boxShadow:'0 12px 30px -8px rgba(79,70,229,0.55)'
        }}>
          <Icon.GraduationCap size={28} stroke="#fff"/>
        </div>
      </div>

      <div style={{display:'flex', flexDirection:'column', alignItems:'center', gap:6, position:'relative', zIndex:1}}>
        <div style={{fontSize:22, fontWeight:700, letterSpacing:'-0.02em'}}>
          Edv<span style={{color:'#4f46e5'}}>antix</span>
        </div>
        <div style={{fontSize:13, color:'#64748b'}}>Готовим вашу школу к работе…</div>
      </div>

      {/* slim progress bar */}
      <div style={{width:240, height:4, borderRadius:9999, background:'#e2e8f0', overflow:'hidden', position:'relative'}}>
        <div style={{
          position:'absolute', top:0, bottom:0, width:'40%',
          background:'linear-gradient(90deg, #4f46e5, #818cf8)', borderRadius:9999,
          animation:'edv-bootbar 1.8s ease-in-out infinite'
        }}/>
        <style>{`@keyframes edv-bootbar { 0%{left:-40%;} 100%{left:100%;} }`}</style>
      </div>

      {/* checklist */}
      <div style={{
        display:'flex', flexDirection:'column', gap:10, marginTop:8,
        padding:'14px 18px', borderRadius:12, background:'rgba(255,255,255,0.7)',
        border:'1px solid #e2e8f0', backdropFilter:'blur(8px)', minWidth:280
      }}>
        <BootStep done label="Подключение к серверу"/>
        <BootStep done label="Загрузка профиля школы"/>
        <BootStep active label="Синхронизация студентов"/>
        <BootStep label="Загрузка расписания"/>
      </div>
    </div>
  );
}

function BootStep({label, done, active}) {
  return (
    <div style={{display:'flex', alignItems:'center', gap:10, fontSize:12.5}}>
      {done ? (
        <div style={{width:18, height:18, borderRadius:9999, background:'#10b981', display:'flex', alignItems:'center', justifyContent:'center'}}>
          <Icon.Check size={11} stroke="#fff" sw={3}/>
        </div>
      ) : active ? (
        <Spinner size={16} thickness={2}/>
      ) : (
        <div style={{width:18, height:18, borderRadius:9999, border:'1.5px solid #cbd5e1'}}/>
      )}
      <span style={{color: done?'#94a3b8': active?'#0f172a':'#94a3b8', fontWeight: active?600:500}}>{label}</span>
    </div>
  );
}

// ─── 4. Inline / micro loaders panel ──────────────────────────────────────
function LoadingMicroStates() {
  return (
    <div style={{
      width:920, height:520, background:'#f8fafc', padding:32,
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a',
      display:'grid', gridTemplateColumns:'1fr 1fr', gap:20, alignContent:'start'
    }}>
      {/* Button loading */}
      <Card style={{padding:24, display:'flex', flexDirection:'column', gap:14}}>
        <div style={{fontSize:11, fontWeight:600, letterSpacing:'0.08em', textTransform:'uppercase', color:'#94a3b8'}}>Кнопки</div>
        <div style={{display:'flex', flexDirection:'column', gap:10}}>
          <Button variant="primary" style={{justifyContent:'center', width:'100%'}}>
            <Spinner size={14} color="#fff" thickness={2}/>Сохранение…
          </Button>
          <Button variant="secondary" style={{justifyContent:'center', width:'100%'}}>
            <Spinner size={14} color="#475569" thickness={2}/>Загружаем CSV
          </Button>
          <Button variant="primary" style={{justifyContent:'center', width:'100%', position:'relative', overflow:'hidden'}}>
            <span style={{opacity:0.6}}>Отправка приглашения</span>
            <span style={{
              position:'absolute', left:0, bottom:0, height:3, width:'62%',
              background:'rgba(255,255,255,0.8)', animation:'edv-pulse 1.6s ease-in-out infinite'
            }}/>
          </Button>
        </div>
      </Card>

      {/* Card with overlay loader */}
      <Card style={{padding:0, display:'flex', flexDirection:'column', position:'relative', overflow:'hidden'}}>
        <div style={{padding:'18px 20px 0', display:'flex', alignItems:'center', justifyContent:'space-between'}}>
          <div>
            <div style={{fontSize:14, fontWeight:600}}>Посещаемость</div>
            <div style={{fontSize:11.5, color:'#94a3b8', marginTop:2}}>Обновление…</div>
          </div>
          <Spinner size={16} thickness={2.5}/>
        </div>
        {/* ghost donut */}
        <div style={{display:'flex', alignItems:'center', justifyContent:'center', padding:'12px 0 22px', position:'relative'}}>
          <svg width="140" height="140" viewBox="0 0 36 36" style={{transform:'rotate(-90deg)'}}>
            <circle cx="18" cy="18" r="15" fill="none" stroke="#f1f5f9" strokeWidth="4"/>
            <circle cx="18" cy="18" r="15" fill="none" stroke="#4f46e5" strokeWidth="4"
              strokeDasharray="20 100" strokeLinecap="round"
              style={{animation:'edv-donut 1.8s ease-in-out infinite'}}/>
          </svg>
          <style>{`@keyframes edv-donut { 0%{stroke-dasharray:5 100;} 50%{stroke-dasharray:60 100;} 100%{stroke-dasharray:5 100;} }`}</style>
        </div>
        <div style={{display:'flex', justifyContent:'space-around', padding:'12px 16px 18px', borderTop:'1px solid #f1f5f9'}}>
          {[0,1,2].map(i=>(
            <div key={i} style={{display:'flex', flexDirection:'column', alignItems:'center', gap:6}}>
              <Shimmer w={40} h={18} r={6}/>
              <Shimmer w={50} h={10}/>
            </div>
          ))}
        </div>
      </Card>

      {/* Inline list loading rows */}
      <Card style={{padding:18, gridColumn:'span 2', display:'flex', flexDirection:'column', gap:0}}>
        <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:14, paddingBottom:12, borderBottom:'1px solid #f1f5f9'}}>
          <div style={{fontSize:14, fontWeight:600}}>Последние платежи</div>
          <div style={{display:'inline-flex', alignItems:'center', gap:6, fontSize:11.5, color:'#94a3b8'}}>
            <Spinner size={12} thickness={2}/>Загрузка ещё 25 записей
          </div>
        </div>
        {[0,1,2,3,4].map(i=>(
          <div key={i} style={{
            display:'grid', gridTemplateColumns:'40px 1fr 120px 100px 100px', alignItems:'center', gap:14,
            padding:'12px 4px', borderBottom: i<4?'1px solid #f1f5f9':0
          }}>
            <Shimmer w={32} h={32} r={9999}/>
            <div style={{display:'flex', flexDirection:'column', gap:6}}>
              <Shimmer w={`${50 + (i*17)%40}%`} h={12}/>
              <Shimmer w={`${30 + (i*13)%20}%`} h={10}/>
            </div>
            <Shimmer w={80} h={20} r={9999}/>
            <Shimmer w={70} h={12}/>
            <Shimmer w={60} h={12}/>
          </div>
        ))}
      </Card>
    </div>
  );
}

Object.assign(window, { LoadingStudentsSkeleton, LoadingDashboardSkeleton, LoadingAppBoot, LoadingMicroStates });
