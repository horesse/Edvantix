// Primitive UI — mirror of shadcn shapes with Edvantix tokens.
const cx = (...xs) => xs.filter(Boolean).join(' ');

// ── Button ─────────────────────────────────────────────────────────────
const btnStyles = {
  base: {
    display: 'inline-flex', alignItems: 'center', justifyContent: 'center', gap: 8,
    fontWeight: 600, fontSize: 14, lineHeight: 1, borderRadius: 8,
    padding: '10px 16px', border: '1px solid transparent', cursor: 'pointer',
    transition: 'all .15s ease', whiteSpace: 'nowrap',
  },
  primary: { background: '#4f46e5', color: '#fff' },
  secondary: { background: '#fff', color: '#0f172a', borderColor: '#e2e8f0' },
  ghost: { background: 'transparent', color: '#475569' },
  destructive: { background: '#ef4444', color: '#fff' },
  sm: { padding: '7px 12px', fontSize: 13, borderRadius: 6 },
  lg: { padding: '12px 20px', fontSize: 15, borderRadius: 10 },
  icon: { padding: 0, width: 36, height: 36, borderRadius: 8 },
};

function Button({variant='primary', size='md', children, style, ...p}) {
  const s = {...btnStyles.base, ...btnStyles[variant], ...(size!=='md'?btnStyles[size]:{}), ...style};
  return <button style={s} onMouseEnter={e=>{
    if(variant==='primary') e.currentTarget.style.background='#4338ca';
    if(variant==='secondary') e.currentTarget.style.background='#f8fafc';
    if(variant==='ghost') e.currentTarget.style.background='#f1f5f9';
    if(variant==='destructive') e.currentTarget.style.background='#dc2626';
  }} onMouseLeave={e=>{
    e.currentTarget.style.background = btnStyles[variant].background;
  }} {...p}>{children}</button>;
}

// ── Card ──────────────────────────────────────────────────────────────
const Card = ({children, style, ...p}) => (
  <div style={{
    background:'#fff', border:'1px solid #e2e8f0', borderRadius:16,
    boxShadow:'0 1px 3px 0 rgba(0,0,0,0.06), 0 1px 2px -1px rgba(0,0,0,0.06)',
    ...style
  }} {...p}>{children}</div>
);

// ── Input ─────────────────────────────────────────────────────────────
function Input({style, ...p}) {
  return <input style={{
    border:'1px solid #e2e8f0', background:'#fff', borderRadius:12,
    padding:'9px 14px', fontSize:14, fontFamily:'inherit', outline:'none',
    width:'100%', transition:'.15s', ...style
  }} onFocus={e=>{e.target.style.borderColor='#6366f1';e.target.style.boxShadow='0 0 0 3px rgba(99,102,241,0.3)'}}
  onBlur={e=>{e.target.style.borderColor='#e2e8f0';e.target.style.boxShadow='none'}} {...p}/>;
}

// ── Badge ─────────────────────────────────────────────────────────────
const badgeColors = {
  default: {bg:'#f1f5f9', fg:'#475569'},
  primary: {bg:'#e0eaff', fg:'#4338ca'},
  success: {bg:'#d1fae5', fg:'#047857'},
  warning: {bg:'#fef3c7', fg:'#92400e'},
  danger:  {bg:'#fee2e2', fg:'#b91c1c'},
  outline: {bg:'transparent', fg:'#475569', bd:'#e2e8f0'},
};
function Badge({variant='default', children, style, dot}) {
  const c = badgeColors[variant];
  return <span style={{
    display:'inline-flex',alignItems:'center',gap:6,
    background:c.bg,color:c.fg,border:c.bd?`1px solid ${c.bd}`:'0',
    padding:'3px 10px',borderRadius:9999,fontSize:12,fontWeight:500,lineHeight:1.4,
    ...style
  }}>
    {dot && <span style={{width:6,height:6,borderRadius:9999,background:'currentColor'}}/>}
    {children}
  </span>;
}

// ── Avatar ────────────────────────────────────────────────────────────
const avatarGradients = [
  'linear-gradient(135deg, #ec4899, #f43f5e)',
  'linear-gradient(135deg, #3b82f6, #06b6d4)',
  'linear-gradient(135deg, #10b981, #22c55e)',
  'linear-gradient(135deg, #f97316, #f59e0b)',
  'linear-gradient(135deg, #8b5cf6, #a855f7)',
  'linear-gradient(135deg, #14b8a6, #06b6d4)',
  'linear-gradient(135deg, #6366f1, #3b82f6)',
  'linear-gradient(135deg, #f43f5e, #ec4899)',
];
function initials(n) {
  const parts = n.trim().split(/\s+/);
  return (parts[0][0] + (parts[1]?.[0]||'')).toUpperCase();
}
function gradFor(s) {
  let h=0; for (let i=0;i<s.length;i++) h = (h*31 + s.charCodeAt(i))>>>0;
  return avatarGradients[h % avatarGradients.length];
}
function Avatar({name, size=36, style}) {
  const fs = Math.max(10, Math.round(size*0.38));
  return <div style={{
    width:size,height:size,borderRadius:9999,display:'inline-flex',
    alignItems:'center',justifyContent:'center',color:'#fff',fontWeight:600,
    fontSize:fs,background:gradFor(name),flexShrink:0,...style
  }}>{initials(name)}</div>;
}

// ── Skeleton ──────────────────────────────────────────────────────────
const Skeleton = ({w=80, h=14, style}) => (
  <div style={{width:w,height:h,borderRadius:6,background:'#f1f5f9',...style}}/>
);

// ── SectionLabel ──────────────────────────────────────────────────────
const SectionLabel = ({children, style}) => (
  <p style={{
    fontSize:10,fontWeight:600,letterSpacing:'0.1em',textTransform:'uppercase',
    color:'#94a3b8',margin:0,padding:'4px 12px 8px',...style
  }}>{children}</p>
);

Object.assign(window, {cx, Button, Card, Input, Badge, Avatar, Skeleton, SectionLabel});
