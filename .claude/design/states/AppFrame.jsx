// AppFrame — sidebar + topbar + content. Reused across every state artboard.
// Width: 1280 (sidebar 240 + content 1040). Height: variable, container scrolls.

function CrumbBar({crumbs=[], title, action}) {
  return (
    <div style={{padding:'18px 32px 14px', borderBottom:'1px solid #e2e8f0', background:'#fff'}}>
      <div style={{display:'flex', alignItems:'center', gap:8, marginBottom:6, fontSize:12.5, color:'#94a3b8'}}>
        {crumbs.map((c,i)=>(
          <React.Fragment key={i}>
            {i>0 && <Icon.ChevronRight size={12} stroke="#cbd5e1"/>}
            <span style={i===crumbs.length-1 ? {color:'#0f172a', fontWeight:600} : {}}>{c}</span>
          </React.Fragment>
        ))}
      </div>
      <div style={{display:'flex', alignItems:'center', justifyContent:'space-between', gap:16}}>
        <h1 style={{margin:0, fontSize:22, fontWeight:700, letterSpacing:'-0.02em'}}>{title}</h1>
        {action}
      </div>
    </div>
  );
}

function AppFrame({active='students', crumbs, title, action, children, width=1280, height=800}) {
  return (
    <div style={{
      width, height, background:'#f8fafc', display:'flex',
      fontFamily:'Inter, system-ui, sans-serif', color:'#0f172a',
      overflow:'hidden', borderRadius:0,
    }}>
      <Sidebar active={active}/>
      <main style={{flex:1, display:'flex', flexDirection:'column', minWidth:0}}>
        <CrumbBar crumbs={crumbs} title={title} action={action}/>
        <div style={{flex:1, overflow:'hidden', padding:'24px 32px'}}>
          {children}
        </div>
      </main>
    </div>
  );
}

// Spinner — used in loading + button loading
function Spinner({size=20, color='#4f46e5', thickness=2.5}) {
  return (
    <span style={{
      display:'inline-block', width:size, height:size,
      border:`${thickness}px solid ${color}25`,
      borderTopColor: color, borderRadius:'50%',
      animation:'edv-spin .9s linear infinite',
    }}/>
  );
}

// Shimmer skeleton — pulsing
function Shimmer({w='100%', h=14, r=6, style}) {
  return (
    <div style={{
      width:w, height:h, borderRadius:r,
      background:'linear-gradient(90deg, #eef2f7 0%, #f8fafc 50%, #eef2f7 100%)',
      backgroundSize:'200% 100%',
      animation:'edv-shimmer 1.6s ease-in-out infinite',
      ...style
    }}/>
  );
}

// Inject animation keyframes once
if (!document.getElementById('edv-anim-styles')) {
  const s = document.createElement('style');
  s.id = 'edv-anim-styles';
  s.textContent = `
    @keyframes edv-spin { to { transform: rotate(360deg); } }
    @keyframes edv-shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
    @keyframes edv-pulse { 0%,100% { opacity: 1; } 50% { opacity: 0.55; } }
    @keyframes edv-float { 0%,100% { transform: translateY(0); } 50% { transform: translateY(-6px); } }
    @keyframes edv-orbit { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
    @keyframes edv-ripple { 0% { transform: scale(0.8); opacity: .8; } 100% { transform: scale(2.4); opacity: 0; } }
    @keyframes edv-glitch { 0%, 100% { transform: translate(0); } 20% { transform: translate(-1px, 1px); } 40% { transform: translate(1px, -1px); } 60% { transform: translate(-1px, 0); } 80% { transform: translate(1px, 1px); } }
  `;
  document.head.appendChild(s);
}

Object.assign(window, { AppFrame, CrumbBar, Spinner, Shimmer });
