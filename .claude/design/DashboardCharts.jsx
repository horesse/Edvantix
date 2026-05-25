// ─── Tiny SVG sparkline ────────────────────────────────────────────────
function Sparkline({values, color='#4f46e5', width=92, height=28}) {
  const min = Math.min(...values), max = Math.max(...values);
  const range = Math.max(1, max - min);
  const stepX = width / (values.length - 1);
  const pts = values.map((v,i) => [i*stepX, height - ((v-min)/range)*(height-2) - 1]);
  const d = pts.map((p,i)=> (i===0?'M':'L')+p[0].toFixed(1)+','+p[1].toFixed(1)).join(' ');
  const area = d + ` L ${width.toFixed(1)},${height} L 0,${height} Z`;
  const gid = 'spg-' + color.replace('#','');
  return (
    <svg width={width} height={height} viewBox={`0 0 ${width} ${height}`} style={{display:'block'}}>
      <defs>
        <linearGradient id={gid} x1="0" x2="0" y1="0" y2="1">
          <stop offset="0%"   stopColor={color} stopOpacity="0.18"/>
          <stop offset="100%" stopColor={color} stopOpacity="0"/>
        </linearGradient>
      </defs>
      <path d={area} fill={`url(#${gid})`}/>
      <path d={d} fill="none" stroke={color} strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round"/>
      <circle cx={pts[pts.length-1][0]} cy={pts[pts.length-1][1]} r="2.5" fill={color}/>
    </svg>
  );
}

// ─── Revenue area chart ────────────────────────────────────────────────
function RevenueChart() {
  const months = ['Ноя','Дек','Янв','Фев','Мар','Апр','Май*'];
  const data    = [186, 198, 214, 226, 244, 263, 284]; // ₽ тыс.
  const dataLY  = [142, 158, 170, 181, 188, 201, 218];

  // Render geometry
  const W = 820, H = 240, PAD_L = 44, PAD_R = 24, PAD_T = 22, PAD_B = 36;
  const innerW = W - PAD_L - PAD_R, innerH = H - PAD_T - PAD_B;
  const max = 320, min = 100;
  const x = i => PAD_L + (i / (months.length-1)) * innerW;
  const y = v => PAD_T + (1 - (v - min) / (max - min)) * innerH;

  const path = data.map((v,i)=>(i===0?'M':'L')+x(i)+','+y(v)).join(' ');
  const area = path + ` L ${x(data.length-1)},${PAD_T+innerH} L ${PAD_L},${PAD_T+innerH} Z`;
  const pathLY = dataLY.map((v,i)=>(i===0?'M':'L')+x(i)+','+y(v)).join(' ');

  const gridLevels = [100, 160, 220, 280];

  return (
    <svg viewBox={`0 0 ${W} ${H}`} width="100%" preserveAspectRatio="none" style={{display:'block'}}>
      <defs>
        <linearGradient id="rev-area" x1="0" x2="0" y1="0" y2="1">
          <stop offset="0%"   stopColor="#4f46e5" stopOpacity="0.22"/>
          <stop offset="100%" stopColor="#4f46e5" stopOpacity="0"/>
        </linearGradient>
      </defs>
      {/* Grid */}
      {gridLevels.map(v => (
        <g key={v}>
          <line x1={PAD_L} x2={W-PAD_R} y1={y(v)} y2={y(v)} stroke="#f1f5f9" strokeWidth="1"/>
          <text x={PAD_L-10} y={y(v)+4} fontSize="10.5" fontWeight="500" fill="#94a3b8" textAnchor="end" fontFamily="Inter">
            ₽{v}K
          </text>
        </g>
      ))}
      {/* LY ghost line */}
      <path d={pathLY} fill="none" stroke="#cbd5e1" strokeWidth="1.5" strokeDasharray="3 4"/>
      {/* This year */}
      <path d={area} fill="url(#rev-area)"/>
      <path d={path} fill="none" stroke="#4f46e5" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round"/>
      {/* Points */}
      {data.map((v,i)=>(
        <g key={i}>
          <circle cx={x(i)} cy={y(v)} r="3.5" fill="#fff" stroke="#4f46e5" strokeWidth="2"/>
        </g>
      ))}
      {/* April highlight callout */}
      {(() => {
        const i = 5, v = data[i], cx = x(i), cy = y(v);
        return (
          <g>
            <line x1={cx} x2={cx} y1={cy+6} y2={PAD_T+innerH} stroke="#4f46e5" strokeWidth="1" strokeDasharray="2 3" opacity="0.4"/>
            <g transform={`translate(${cx+10}, ${cy-54})`}>
              <rect x="0" y="0" width="118" height="48" rx="10" fill="#0f172a"/>
              <text x="12" y="17" fill="#94a3b8" fontSize="10" fontWeight="500" fontFamily="Inter">Апрель</text>
              <text x="106" y="17" fill="#10b981" fontSize="10.5" fontWeight="600" fontFamily="Inter" textAnchor="end">+8%</text>
              <text x="12" y="36" fill="#fff" fontSize="13" fontWeight="700" fontFamily="Inter">₽263 400</text>
            </g>
          </g>
        );
      })()}
      {/* X labels */}
      {months.map((m,i)=>(
        <text key={m} x={x(i)} y={H-12} fontSize="11" fill="#64748b" textAnchor="middle" fontWeight="500" fontFamily="Inter">
          {m}
        </text>
      ))}
    </svg>
  );
}

// ─── Attendance donut (re-used pattern from kit, expanded) ─────────────
function AttendanceDonut({present=87, late=7, absent=6, size=160, strokeW=18}) {
  const r = (size - strokeW) / 2;
  const C = 2 * Math.PI * r;
  const segs = [
    { v: present, color: '#10b981' },
    { v: late,    color: '#f59e0b' },
    { v: absent,  color: '#f43f5e' },
  ];
  let acc = 0;
  return (
    <div style={{position:'relative', width:size, height:size}}>
      <svg width={size} height={size} viewBox={`0 0 ${size} ${size}`} style={{transform:'rotate(-90deg)'}}>
        <circle cx={size/2} cy={size/2} r={r} fill="none" stroke="#f1f5f9" strokeWidth={strokeW}/>
        {segs.map((s,i) => {
          const len = (s.v/100) * C;
          const el = (
            <circle key={i} cx={size/2} cy={size/2} r={r} fill="none"
              stroke={s.color} strokeWidth={strokeW}
              strokeDasharray={`${len} ${C-len}`}
              strokeDashoffset={-acc} strokeLinecap="butt"/>
          );
          acc += len;
          return el;
        })}
      </svg>
      <div style={{
        position:'absolute', inset:0, display:'flex', flexDirection:'column',
        alignItems:'center', justifyContent:'center'
      }}>
        <span style={{fontSize:28, fontWeight:700, letterSpacing:'-0.02em', fontVariantNumeric:'tabular-nums'}}>
          {present}%
        </span>
        <span style={{fontSize:11.5, color:'#64748b', marginTop:2}}>присутствие</span>
      </div>
    </div>
  );
}

// ─── Progress ring (for goals) ─────────────────────────────────────────
function ProgressRing({value, size=44, color='#4f46e5'}) {
  const r = (size - 6) / 2, C = 2 * Math.PI * r;
  return (
    <svg width={size} height={size} viewBox={`0 0 ${size} ${size}`} style={{transform:'rotate(-90deg)', flexShrink:0}}>
      <circle cx={size/2} cy={size/2} r={r} fill="none" stroke="#f1f5f9" strokeWidth="5"/>
      <circle cx={size/2} cy={size/2} r={r} fill="none" stroke={color} strokeWidth="5"
        strokeLinecap="round"
        strokeDasharray={`${(value/100)*C} ${C}`}/>
    </svg>
  );
}

window.Sparkline = Sparkline;
window.RevenueChart = RevenueChart;
window.AttendanceDonut = AttendanceDonut;
window.ProgressRing = ProgressRing;
