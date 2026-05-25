// Custom checkbox
function Checkbox({checked, indeterminate, onChange}) {
  const ref = React.useRef(null);
  React.useEffect(()=>{ if(ref.current) ref.current.indeterminate = !!indeterminate; }, [indeterminate]);
  const on = checked || indeterminate;
  return (
    <label style={{display:'inline-flex', alignItems:'center', justifyContent:'center', cursor:'pointer', padding:2}}
      onClick={e=>e.stopPropagation()}>
      <input ref={ref} type="checkbox" checked={!!checked} onChange={onChange}
        style={{position:'absolute', opacity:0, pointerEvents:'none', width:0, height:0}}/>
      <span style={{
        width:16, height:16, borderRadius:4,
        border:'1.5px solid '+(on ? '#4f46e5' : '#cbd5e1'),
        background: on ? '#4f46e5' : '#fff',
        display:'inline-flex', alignItems:'center', justifyContent:'center',
        color:'#fff', transition:'.1s'
      }}>
        {checked && <Icon.Check size={11} sw={3}/>}
        {indeterminate && !checked && <span style={{width:8, height:2, background:'#fff', borderRadius:1}}/>}
      </span>
    </label>
  );
}

// Progress bar
function ProgressBar({value}) {
  const color = value >= 80 ? '#10b981' : value >= 40 ? '#4f46e5' : '#f59e0b';
  return (
    <div style={{display:'flex', alignItems:'center', gap:10, minWidth:140}}>
      <div style={{flex:1, height:6, background:'#f1f5f9', borderRadius:9999, overflow:'hidden'}}>
        <div style={{
          width: value + '%', height:'100%', background: color, borderRadius:9999,
          transition:'.3s'
        }}/>
      </div>
      <span style={{fontSize:12, fontWeight:600, color:'#334155', fontVariantNumeric:'tabular-nums', width:30, textAlign:'right'}}>
        {value}%
      </span>
    </div>
  );
}

// Status pill (with dot)
function StatusPill({status}) {
  const m = STATUS_META[status];
  return (
    <span style={{
      display:'inline-flex', alignItems:'center', gap:6,
      padding:'3px 10px', borderRadius:9999,
      background: m.bg, color: m.fg,
      fontSize:12, fontWeight:500, lineHeight:1.4
    }}>
      <span style={{width:6, height:6, borderRadius:9999, background:m.dotColor}}/>
      {m.label}
    </span>
  );
}

// Course chips (compact, max 2 visible + overflow)
function CourseChips({courses}) {
  const visible = courses.slice(0, 2);
  const rest = courses.length - visible.length;
  return (
    <div style={{display:'flex', alignItems:'center', gap:4, flexWrap:'wrap'}}>
      {visible.map(c => (
        <span key={c} style={{
          display:'inline-flex', alignItems:'center', gap:5,
          padding:'2px 8px', borderRadius:6,
          background:'#f1f5f9', color:'#334155',
          fontSize:11.5, fontWeight:500, whiteSpace:'nowrap', maxWidth:140,
          overflow:'hidden', textOverflow:'ellipsis'
        }}>{c}</span>
      ))}
      {rest > 0 && (
        <span style={{
          padding:'2px 7px', borderRadius:6, background:'#eef2ff', color:'#4338ca',
          fontSize:11.5, fontWeight:600
        }}>+{rest}</span>
      )}
    </div>
  );
}

function SortableTh({children, sortKey, sort, setSort, align='left', width}) {
  const active = sort.key === sortKey;
  const dir = active ? sort.dir : null;
  return (
    <th style={{
      textAlign:align, padding:'10px 14px', fontWeight:600, fontSize:11.5, color:'#64748b',
      textTransform:'uppercase', letterSpacing:'0.05em',
      borderBottom:'1px solid #e2e8f0', background:'#fafbfc',
      whiteSpace:'nowrap', width
    }}>
      <button onClick={()=>setSort({
        key: sortKey,
        dir: active && sort.dir==='asc' ? 'desc' : 'asc'
      })}
      style={{
        display:'inline-flex', alignItems:'center', gap:6, background:'transparent',
        border:0, padding:0, color: active ? '#0f172a' : '#64748b', cursor:'pointer',
        fontWeight: active ? 700 : 600, fontSize:11.5, letterSpacing:'0.05em', textTransform:'uppercase',
        fontFamily:'inherit'
      }}>
        {children}
        {active
          ? <Icon.ChevronUp size={12} style={{transform: dir==='desc'?'rotate(180deg)':'none', transition:'.1s'}}/>
          : <Icon.ChevronsUpDown size={12} stroke="#cbd5e1"/>}
      </button>
    </th>
  );
}

// ─── Main table ─────────────────────────────────────────────────────────
function StudentsTable({rows, selected, setSelected, sort, setSort, onOpenRow}) {
  const allChecked = rows.length>0 && rows.every(r=>selected[r.id]);
  const someChecked = rows.some(r=>selected[r.id]) && !allChecked;
  const toggleAll = () => {
    if (allChecked) setSelected({});
    else {
      const next = {};
      rows.forEach(r => next[r.id] = true);
      setSelected(next);
    }
  };

  return (
    <Card style={{padding:0, overflow:'hidden'}}>
      <div style={{overflowX:'auto'}}>
        <table style={{width:'100%', borderCollapse:'separate', borderSpacing:0, minWidth:1080}}>
          <thead>
            <tr>
              <th style={{padding:'10px 0 10px 18px', width:36, borderBottom:'1px solid #e2e8f0', background:'#fafbfc'}}>
                <Checkbox checked={allChecked} indeterminate={someChecked} onChange={toggleAll}/>
              </th>
              <SortableTh sortKey="name"     sort={sort} setSort={setSort}>Студент</SortableTh>
              <SortableTh sortKey="group"    sort={sort} setSort={setSort}>Группа</SortableTh>
              <SortableTh sortKey="courses"  sort={sort} setSort={setSort}>Курсы</SortableTh>
              <SortableTh sortKey="progress" sort={sort} setSort={setSort} width={180}>Прогресс</SortableTh>
              <SortableTh sortKey="status"   sort={sort} setSort={setSort}>Статус</SortableTh>
              <SortableTh sortKey="payment"  sort={sort} setSort={setSort}>Оплата</SortableTh>
              <SortableTh sortKey="lastActive" sort={sort} setSort={setSort}>Активность</SortableTh>
              <th style={{padding:'10px 18px 10px 14px', width:48, borderBottom:'1px solid #e2e8f0', background:'#fafbfc'}}></th>
            </tr>
          </thead>
          <tbody>
            {rows.map((s, idx) => (
              <Row key={s.id} s={s} selected={!!selected[s.id]}
                onToggle={() => setSelected({...selected, [s.id]: !selected[s.id]})}
                onOpen={() => onOpenRow(s)}/>
            ))}
          </tbody>
        </table>
      </div>
      {/* Pagination */}
      <div style={{
        display:'flex', alignItems:'center', justifyContent:'space-between',
        padding:'12px 20px', borderTop:'1px solid #e2e8f0', background:'#fff'
      }}>
        <div style={{fontSize:13, color:'#64748b'}}>
          Показано <span style={{color:'#0f172a', fontWeight:600}}>1–{rows.length}</span> из{' '}
          <span style={{color:'#0f172a', fontWeight:600}}>248</span> студентов
        </div>
        <div style={{display:'flex', alignItems:'center', gap:8}}>
          <span style={{fontSize:12.5, color:'#64748b'}}>На странице</span>
          <button style={{
            display:'inline-flex', alignItems:'center', gap:4, height:30, padding:'0 10px',
            borderRadius:7, border:'1px solid #e2e8f0', background:'#fff',
            fontSize:12.5, fontWeight:500, color:'#0f172a', cursor:'pointer'
          }}>
            14 <Icon.ChevronDown size={12}/>
          </button>
          <div style={{width:1, height:20, background:'#e2e8f0', margin:'0 6px'}}/>
          <PagBtn icon="ChevronLeft" disabled/>
          <PagBtn label="1" active/>
          <PagBtn label="2"/>
          <PagBtn label="3"/>
          <span style={{padding:'0 4px', color:'#94a3b8'}}>…</span>
          <PagBtn label="18"/>
          <PagBtn icon="ChevronRight"/>
        </div>
      </div>
    </Card>
  );
}

function PagBtn({label, icon, active, disabled}) {
  const Ic = icon ? Icon[icon] : null;
  return (
    <button disabled={disabled} style={{
      minWidth:30, height:30, padding: icon ? 0 : '0 9px', borderRadius:7,
      border:'1px solid '+(active ? '#4f46e5' : '#e2e8f0'),
      background: active ? '#4f46e5' : '#fff',
      color: active ? '#fff' : disabled ? '#cbd5e1' : '#334155',
      fontSize:12.5, fontWeight:600, cursor: disabled ? 'default' : 'pointer',
      display:'inline-flex', alignItems:'center', justifyContent:'center'
    }}>
      {Ic ? <Ic size={13}/> : label}
    </button>
  );
}

// Single row
function Row({s, selected, onToggle, onOpen}) {
  const [hover, setHover] = React.useState(false);
  const m = PAYMENT_META[s.payment];
  return (
    <tr
      onMouseEnter={()=>setHover(true)}
      onMouseLeave={()=>setHover(false)}
      onClick={onOpen}
      style={{
        cursor:'pointer',
        background: selected ? '#f5f7ff' : hover ? '#fafbfc' : '#fff',
        transition:'background .1s'
      }}>
      <td style={{padding:'14px 0 14px 18px', borderBottom:'1px solid #f1f5f9'}}>
        <Checkbox checked={selected} onChange={onToggle}/>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <div style={{display:'flex', alignItems:'center', gap:12}}>
          <Avatar name={s.name} size={38}/>
          <div style={{minWidth:0}}>
            <div style={{display:'flex', alignItems:'center', gap:6}}>
              <span style={{fontSize:13.5, fontWeight:600, color:'#0f172a'}}>{s.name}</span>
              {s.status === 'new' && (
                <span style={{
                  padding:'1px 6px', borderRadius:4, background:'#eef2ff', color:'#4338ca',
                  fontSize:10, fontWeight:700, letterSpacing:'0.04em'
                }}>NEW</span>
              )}
            </div>
            <div style={{fontSize:12, color:'#64748b', display:'flex', alignItems:'center', gap:5}}>
              <Icon.AtSign size={11} stroke="#94a3b8"/>{s.email}
            </div>
          </div>
        </div>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <span style={{fontSize:13, color:'#334155', whiteSpace:'nowrap'}}>{s.group}</span>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <CourseChips courses={s.courses}/>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <ProgressBar value={s.progress}/>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <StatusPill status={s.status}/>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <div style={{display:'flex', flexDirection:'column', gap:2}}>
          <Badge variant={m.variant}>{m.label}</Badge>
          <span style={{fontSize:11, color:'#94a3b8', paddingLeft:2}}>до {s.paidUntil}</span>
        </div>
      </td>
      <td style={{padding:'14px', borderBottom:'1px solid #f1f5f9'}}>
        <span style={{fontSize:12.5, color:'#64748b', whiteSpace:'nowrap'}}>{s.lastActive}</span>
      </td>
      <td style={{padding:'14px 18px 14px 14px', borderBottom:'1px solid #f1f5f9'}}>
        <button onClick={e=>e.stopPropagation()} style={{
          width:30, height:30, borderRadius:7, border:'1px solid transparent',
          background: hover ? '#f1f5f9' : 'transparent', color:'#64748b',
          cursor:'pointer', display:'inline-flex', alignItems:'center', justifyContent:'center'
        }}>
          <Icon.MoreHorizontal size={16}/>
        </button>
      </td>
    </tr>
  );
}

window.StudentsTable = StudentsTable;
