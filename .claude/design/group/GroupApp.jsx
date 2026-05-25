// Group dashboard — единый обзор учебной группы
const { useState: useStateGA, useMemo: useMemoGA } = React;

function GroupApp() {
  const g = GRP;
  const studentsById = useMemoGA(() => Object.fromEntries(GRP_STUDENTS.map(s => [s.id, s])), []);

  return (
    <div style={{
      display:'flex', height:'100vh', minHeight:700,
      background:'#f8fafc', overflow:'hidden',
    }}>
      <Sidebar active="groups" />
      <div style={{ flex:1, display:'flex', flexDirection:'column', minWidth:0 }}>
        <Breadcrumb group={g} />
        <GroupHeader group={g} />
        <GdTabs active="overview" />

        <div style={{ flex:1, overflowY:'auto', padding:'24px 32px 40px', background:'#f8fafc' }}>
          <div style={{ maxWidth:1280, margin:'0 auto', display:'flex', flexDirection:'column', gap:20 }}>

            {/* KPIs */}
            <div style={{ display:'grid', gap:14,
              gridTemplateColumns:'repeat(auto-fit, minmax(220px, 1fr))' }}>
              <GdKpi label="Студенты"     value={`${g.students}/${g.capacity}`} sub={`${g.freeSeats} свободное место`}
                     icon="Users" tone="primary"
                     delta={{ tone:'up', text:'+1 за неделю' }} />
              <GdKpi label="Посещаемость" value={`${Math.round(g.attendanceRate*100)}%`}
                     sub="за последние 4 недели" icon="CircleCheck" tone="success"
                     delta={{ tone:'up', text:`+${Math.round(g.attendanceDelta*100)} п.п.` }} />
              <GdKpi label="Прогресс курса" value={`${g.completedLessons}/${g.totalLessons}`}
                     sub={`${Math.round(g.completedLessons/g.totalLessons*100)}% уроков пройдено`}
                     icon="BookOpen" tone="violet" />
              <GdKpi label="Средний балл" value={g.avgGrade.toFixed(1)}
                     sub="по контрольным" icon="Sparkles" tone="warning"
                     delta={{ tone:'up', text:`+${g.avgGradeDelta.toFixed(1)}` }} />
            </div>

            {/* Two columns */}
            <div style={{ display:'grid', gap:20,
              gridTemplateColumns:'minmax(0, 1fr) 360px' }}>

              {/* LEFT */}
              <div style={{ display:'flex', flexDirection:'column', gap:20, minWidth:0 }}>
                <NextLessonCard lesson={GRP_NEXT_LESSON} group={g} />

                <GdSection
                  title="Прогресс по программе"
                  subtitle={`«${g.course}» · ${g.completedLessons} из ${g.totalLessons} уроков`}
                  right={<Button variant="secondary" size="sm">
                    <Icon.BookOpen size={14}/>Открыть программу</Button>}
                >
                  <GdProgramTrack units={GRP_PROGRAM} />
                </GdSection>

                <GdSection
                  title="Посещаемость по неделям"
                  subtitle="6 завершённых недель + текущая (неполная)"
                  right={
                    <a href="Attendance.html" style={{
                      fontSize:12.5, color:'#4f46e5', fontWeight:500,
                      display:'inline-flex', alignItems:'center', gap:4,
                    }}>Открыть журнал <Icon.ArrowRight size={13}/></a>
                  }
                >
                  <GdAttendanceChart weeks={GRP_WEEKS} />
                </GdSection>

                <GdSection
                  title="Последние занятия"
                  subtitle="Завершённые и отменённые уроки"
                  right={<a href="Attendance.html" style={{
                    fontSize:12.5, color:'#4f46e5', fontWeight:500 }}>Все уроки →</a>}
                  padding={20}
                >
                  <RecentLessonsList rows={GRP_RECENT_LESSONS} total={g.students} />
                </GdSection>
              </div>

              {/* RIGHT */}
              <div style={{ display:'flex', flexDirection:'column', gap:20, minWidth:0 }}>
                <TeacherCard teacher={g.teacher} />
                <GroupInfoCard group={g} />
                <AtRiskCard at={GRP_AT_RISK} students={studentsById} />
                <FinanceCard fin={g.finance} fee={g.monthlyFee} />
              </div>
            </div>

            {/* Students preview — full width */}
            <GdSection
              title="Студенты группы"
              subtitle={`${g.students} активных · ${g.freeSeats > 0 ? `${g.freeSeats} свободное место` : 'мест нет'}`}
              right={
                <div style={{ display:'flex', gap:8 }}>
                  <Button variant="secondary" size="sm">
                    <Icon.FileText size={14}/>Экспорт</Button>
                  <a href="Group Students.html"
                    style={{ textDecoration:'none' }}>
                    <Button size="sm"><Icon.UserPlus size={14}/>Зачислить</Button>
                  </a>
                </div>
              }
              padding={20}
            >
              <StudentsPreview students={GRP_STUDENTS} />
            </GdSection>

            {/* Activity */}
            <GdSection
              title="Активность группы"
              subtitle="События, оплаты, материалы — последние 7 дней"
              padding={20}
            >
              <ActivityFeed items={GRP_ACTIVITY} />
            </GdSection>
          </div>
        </div>
      </div>
    </div>
  );
}

// ── Breadcrumb ───────────────────────────────────────────────────────
function Breadcrumb({ group }) {
  return (
    <div style={{
      padding:'13px 32px', borderBottom:'1px solid #e2e8f0',
      background:'#fff', display:'flex', alignItems:'center', gap:8,
      fontSize:13, color:'#64748b',
    }}>
      <span>Школа</span>
      <Icon.ChevronRight size={13} stroke="#cbd5e1" />
      <a href="Groups.html" style={{ color:'#475569' }}>Группы</a>
      <Icon.ChevronRight size={13} stroke="#cbd5e1" />
      <span style={{ color:'#0f172a', fontWeight:500 }}>{group.code}</span>
    </div>
  );
}

// ── Group header ─────────────────────────────────────────────────────
function GroupHeader({ group }) {
  const fmtIcon = group.format === 'online' ? 'MessageCircle'
                : group.format === 'mixed' ? 'Users'
                : 'School';
  const FmtIc = Icon[fmtIcon];
  return (
    <div style={{
      padding:'22px 32px 22px', background:'#fff',
      display:'flex', alignItems:'flex-start', gap:24,
    }}>
      <div style={{ flex:1, minWidth:0, display:'flex', flexDirection:'column', gap:12 }}>
        {/* chips row */}
        <div style={{ display:'flex', alignItems:'center', gap:8, flexWrap:'wrap' }}>
          <GdStatusPill status={group.status} size="lg" />
          <GdLevelChip level={group.level} full size="lg" />
          <span style={{
            display:'inline-flex', alignItems:'center', gap:6,
            padding:'5px 10px', borderRadius:9999, background:'#f1f5f9', color:'#475569',
            fontSize:12, fontWeight:500,
          }}>
            <FmtIc size={13} stroke="#64748b"/>
            {group.formatLabel} · {group.room}
          </span>
          <span style={{ fontSize:12.5, color:'#94a3b8',
            fontFamily:'var(--edv-font-mono, ui-monospace)', fontWeight:600 }}>
            {group.code}
          </span>
        </div>

        {/* title + subtitle */}
        <div>
          <h1 style={{
            margin:0, fontSize:28, fontWeight:700, letterSpacing:'-0.025em',
            lineHeight:1.15,
          }}>{group.name}</h1>
          <p style={{
            margin:'6px 0 0', fontSize:13.5, color:'#64748b', maxWidth:720,
            lineHeight:1.5,
          }}>{group.description}</p>
        </div>

        {/* meta row */}
        <div style={{
          display:'flex', alignItems:'center', gap:18, flexWrap:'wrap',
          fontSize:12.5, color:'#475569',
        }}>
          <HeadMeta icon="CalendarDays" label={group.schedule}/>
          <HeadMeta icon="BookOpen"     label={group.course}/>
          <HeadMeta icon="UserCheck"    label={group.teacher.name}/>
          <HeadMeta icon="Clock"        label={`${group.starts} – ${group.ends}`}/>
        </div>
      </div>

      {/* Actions */}
      <div style={{ display:'flex', alignItems:'center', gap:8, flexShrink:0 }}>
        <Button variant="secondary" size="md" style={{
          padding:0, width:36, height:36, borderRadius:8 }}>
          <Icon.Bell size={15}/>
        </Button>
        <Button variant="secondary" size="md" style={{
          padding:0, width:36, height:36, borderRadius:8 }}>
          <Icon.Settings size={15}/>
        </Button>
        <Button variant="secondary"><Icon.FileText size={15}/>Редактировать</Button>
        <a href="Lesson Create.html" style={{ textDecoration:'none' }}>
          <Button><Icon.Plus size={16}/>Создать урок</Button>
        </a>
      </div>
    </div>
  );
}
function HeadMeta({ icon, label }) {
  const Ic = Icon[icon];
  return (
    <span style={{ display:'inline-flex', alignItems:'center', gap:6 }}>
      <Ic size={13} stroke="#94a3b8"/>{label}
    </span>
  );
}

// ── Next lesson hero ─────────────────────────────────────────────────
function NextLessonCard({ lesson, group }) {
  const days = lesson.daysAway;
  const dayLabel = days === 0 ? 'сегодня'
                 : days === 1 ? 'завтра'
                 : `через ${days} ${plural(days,'день','дня','дней')}`;
  return (
    <div style={{
      position:'relative', overflow:'hidden',
      background:'linear-gradient(135deg, #4f46e5 0%, #4338ca 60%, #312e81 100%)',
      color:'#fff', borderRadius:18,
      padding:'24px 26px', display:'grid',
      gridTemplateColumns:'1fr auto', gap:24, alignItems:'center',
      boxShadow:'0 14px 40px -16px rgba(79,70,229,0.55)',
    }}>
      {/* aurora glow */}
      <div style={{
        position:'absolute', inset:0, pointerEvents:'none',
        background:'radial-gradient(circle at 88% -10%, rgba(244,114,182,0.32), transparent 55%), radial-gradient(circle at -10% 110%, rgba(56,189,248,0.28), transparent 50%)',
      }}/>
      <div style={{ position:'relative', display:'flex', flexDirection:'column', gap:14, minWidth:0 }}>
        <div style={{ display:'flex', alignItems:'center', gap:10 }}>
          <span style={{
            display:'inline-flex', alignItems:'center', gap:6,
            padding:'4px 10px', borderRadius:9999,
            background:'rgba(255,255,255,0.16)', color:'#fff',
            fontSize:11.5, fontWeight:600, letterSpacing:'0.04em',
            textTransform:'uppercase',
          }}>
            <Icon.Sparkles size={12} stroke="#fff"/>Следующее занятие
          </span>
          <span style={{ fontSize:12, color:'rgba(255,255,255,0.75)' }}>
            {dayLabel} · {lesson.weekday}, {lesson.date}
          </span>
        </div>

        <div>
          <div style={{ fontSize:13, fontWeight:500, color:'rgba(255,255,255,0.78)' }}>
            {lesson.unit}
          </div>
          <div style={{ fontSize:24, fontWeight:700, letterSpacing:'-0.02em', lineHeight:1.2, marginTop:2 }}>
            {lesson.topic}
          </div>
        </div>

        <div style={{ display:'flex', alignItems:'center', gap:18, flexWrap:'wrap',
          fontSize:13, color:'rgba(255,255,255,0.85)' }}>
          <span style={{ display:'inline-flex', alignItems:'center', gap:6 }}>
            <Icon.Clock size={14} stroke="rgba(255,255,255,0.7)"/>
            {lesson.startsAt} – {lesson.endsAt}
          </span>
          <span style={{ display:'inline-flex', alignItems:'center', gap:6 }}>
            <Icon.School size={14} stroke="rgba(255,255,255,0.7)"/>
            {lesson.room}
          </span>
          <span style={{ display:'inline-flex', alignItems:'center', gap:6 }}>
            <Icon.Users size={14} stroke="rgba(255,255,255,0.7)"/>
            {group.students} студентов
          </span>
        </div>

        <div style={{ display:'flex', gap:8, marginTop:4 }}>
          <a href="Lesson Create.html" style={{ textDecoration:'none' }}>
            <button style={{
              display:'inline-flex', alignItems:'center', gap:8,
              padding:'10px 16px', borderRadius:10, border:'none',
              background:'#fff', color:'#3730a3', fontSize:13.5, fontWeight:600,
              cursor:'pointer', fontFamily:'inherit',
            }}>
              <Icon.FileText size={15}/>Подготовить материалы
            </button>
          </a>
          <button style={{
            display:'inline-flex', alignItems:'center', gap:8,
            padding:'10px 14px', borderRadius:10,
            border:'1px solid rgba(255,255,255,0.22)',
            background:'rgba(255,255,255,0.08)', color:'#fff',
            fontSize:13, fontWeight:500, cursor:'pointer', fontFamily:'inherit',
          }}>
            <Icon.CalendarDays size={14}/>В календарь
          </button>
        </div>
      </div>

      {/* Right: countdown */}
      <div style={{
        position:'relative', textAlign:'center', padding:'4px 8px',
      }}>
        <div style={{ fontSize:11.5, color:'rgba(255,255,255,0.7)',
          textTransform:'uppercase', letterSpacing:'0.08em', fontWeight:600 }}>
          До урока
        </div>
        <div style={{
          fontSize:64, fontWeight:800, letterSpacing:'-0.04em', lineHeight:1,
          fontVariantNumeric:'tabular-nums', marginTop:6,
        }}>{days}</div>
        <div style={{ fontSize:13, fontWeight:500, color:'rgba(255,255,255,0.78)',
          marginTop:4 }}>{plural(days,'день','дня','дней')}</div>
      </div>
    </div>
  );
}

function plural(n, one, few, many) {
  const mod10 = n % 10, mod100 = n % 100;
  if (mod10 === 1 && mod100 !== 11) return one;
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return few;
  return many;
}

// ── Recent lessons list ──────────────────────────────────────────────
function RecentLessonsList({ rows, total }) {
  return (
    <div style={{ display:'flex', flexDirection:'column' }}>
      {rows.map((r, i) => {
        const isCancelled = r.status === 'cancelled';
        const pct = isCancelled ? 0 : Math.round(r.present / r.total * 100);
        return (
          <div key={r.id} style={{
            display:'grid',
            gridTemplateColumns:'56px 1fr 200px 88px',
            alignItems:'center', gap:14,
            padding:'12px 0',
            borderTop: i > 0 ? '1px solid #f1f5f9' : '0',
            opacity: isCancelled ? 0.72 : 1,
          }}>
            <div style={{ display:'flex', flexDirection:'column' }}>
              <div style={{ fontSize:14, fontWeight:600, color:'#0f172a',
                fontVariantNumeric:'tabular-nums' }}>{r.date}</div>
              <div style={{ fontSize:11, color:'#94a3b8' }}>{r.weekday}</div>
            </div>
            <div style={{ minWidth:0 }}>
              <div style={{ fontSize:13.5, fontWeight:500, color: isCancelled ? '#64748b' : '#0f172a',
                textDecoration: isCancelled ? 'line-through' : 'none' }}>
                {r.topic}
              </div>
              <div style={{ fontSize:12, color:'#94a3b8', marginTop:2 }}>
                {isCancelled ? 'Отменено' : r.unit}
              </div>
            </div>
            <div style={{ display:'flex', alignItems:'center', gap:10 }}>
              {isCancelled ? (
                <Badge variant="default" dot>Отменено</Badge>
              ) : (
                <>
                  <div style={{ flex:1, height:6, borderRadius:9999, background:'#f1f5f9',
                    overflow:'hidden' }}>
                    <div style={{
                      width:`${pct}%`, height:'100%',
                      background: pct >= 90 ? '#10b981' : pct >= 75 ? '#f59e0b' : '#ef4444',
                    }}/>
                  </div>
                  <span style={{ fontSize:12, fontWeight:600, color:'#475569',
                    fontVariantNumeric:'tabular-nums', minWidth:34, textAlign:'right' }}>
                    {pct}%
                  </span>
                </>
              )}
            </div>
            <div style={{ fontSize:12, color:'#64748b', textAlign:'right',
              fontVariantNumeric:'tabular-nums' }}>
              {isCancelled ? '—' : `${r.present}/${r.total}`}
              {!isCancelled && r.late > 0 && (
                <span style={{ color:'#f59e0b' }}> · {r.late} опозд.</span>
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
}

// ── Teacher card ─────────────────────────────────────────────────────
function TeacherCard({ teacher }) {
  return (
    <GdSection title="Преподаватель" padding={18}>
      <a href="Member Profile.html" style={{ textDecoration:'none', color:'inherit' }}>
        <div style={{
          display:'grid', gridTemplateColumns:'auto 1fr', gap:14, alignItems:'center',
          padding:'2px 0',
        }}>
          <Avatar name={teacher.name} size={48}/>
          <div style={{ minWidth:0 }}>
            <div style={{ fontSize:14, fontWeight:600 }}>{teacher.name}</div>
            <div style={{ fontSize:12, color:'#64748b', marginTop:2 }}>{teacher.role}</div>
            <div style={{ fontSize:11.5, color:'#94a3b8', marginTop:2 }}>
              в школе {teacher.yearsAtSchool} {plural(teacher.yearsAtSchool,'год','года','лет')}
            </div>
          </div>
        </div>
      </a>
      <div style={{ display:'flex', gap:8, marginTop:14 }}>
        <button style={tcrBtn}><Icon.Mail size={13}/>Написать</button>
        <button style={tcrBtn}><Icon.Phone size={13}/>Позвонить</button>
      </div>
    </GdSection>
  );
}
const tcrBtn = {
  flex:1, display:'inline-flex', alignItems:'center', justifyContent:'center', gap:6,
  height:34, borderRadius:8, border:'1px solid #e2e8f0', background:'#fff',
  fontSize:12.5, fontWeight:500, color:'#334155', cursor:'pointer', fontFamily:'inherit',
};

// ── Group info card ──────────────────────────────────────────────────
function GroupInfoCard({ group }) {
  const fillPct = Math.round(group.students / group.capacity * 100);
  return (
    <GdSection title="Информация о группе" padding={18}>
      <div style={{ display:'flex', flexDirection:'column' }}>
        <GdInfoRow icon="CalendarDays" label="Расписание" value={group.schedule}/>
        <div style={{ borderTop:'1px solid #f1f5f9' }}/>
        <GdInfoRow icon="School"       label="Формат" value={`${group.formatLabel} · ${group.room}`}/>
        <div style={{ borderTop:'1px solid #f1f5f9' }}/>
        <GdInfoRow icon="BookOpen"     label="Курс" value={group.course}/>
        <div style={{ borderTop:'1px solid #f1f5f9' }}/>
        <GdInfoRow icon="Clock"        label="Период" value={`${group.starts} – ${group.ends}`}/>
        <div style={{ borderTop:'1px solid #f1f5f9' }}/>
        <div style={{ padding:'12px 0 0' }}>
          <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between' }}>
            <div style={{ display:'flex', alignItems:'center', gap:10 }}>
              <Icon.Users size={15} stroke="#94a3b8"/>
              <span style={{ fontSize:12.5, color:'#64748b' }}>Заполненность</span>
            </div>
            <span style={{ fontSize:13, fontWeight:600, color:'#0f172a',
              fontVariantNumeric:'tabular-nums' }}>
              {group.students}/{group.capacity}
            </span>
          </div>
          <div style={{
            marginTop:8, height:8, borderRadius:9999, background:'#f1f5f9', overflow:'hidden',
            position:'relative',
          }}>
            <div style={{
              width:`${fillPct}%`, height:'100%',
              background:'linear-gradient(90deg, #4f46e5, #6366f1)',
              borderRadius:9999,
            }}/>
          </div>
          <div style={{
            display:'flex', justifyContent:'space-between',
            fontSize:11, color:'#94a3b8', marginTop:6,
          }}>
            <span>{fillPct}% заполнено</span>
            <span>{group.freeSeats} {plural(group.freeSeats,'место','места','мест')} свободно</span>
          </div>
        </div>
      </div>
    </GdSection>
  );
}

// ── At-risk students ─────────────────────────────────────────────────
function AtRiskCard({ at, students }) {
  return (
    <GdSection
      title="Требуют внимания"
      subtitle={`${at.length} студента из ${GRP.students}`}
      right={<Icon.AlertCircle size={16} stroke="#ef4444"/>}
      padding={18}
    >
      <div style={{ display:'flex', flexDirection:'column', gap:10 }}>
        {at.map(a => {
          const s = students[a.id];
          const tone = a.severity === 'high'   ? { bg:'rgba(239,68,68,0.06)', bd:'rgba(239,68,68,0.18)', fg:'#b91c1c' }
                     : a.severity === 'medium' ? { bg:'rgba(245,158,11,0.07)', bd:'rgba(245,158,11,0.20)', fg:'#92400e' }
                     :                           { bg:'#f8fafc', bd:'#e2e8f0', fg:'#475569' };
          return (
            <div key={a.id} style={{
              display:'grid', gridTemplateColumns:'auto 1fr auto', gap:10, alignItems:'center',
              padding:'10px 12px', borderRadius:10,
              background:tone.bg, border:`1px solid ${tone.bd}`,
            }}>
              <Avatar name={s.name} size={32}/>
              <div style={{ minWidth:0 }}>
                <div style={{ fontSize:13, fontWeight:600 }}>{s.name}</div>
                <div style={{ fontSize:11.5, color:tone.fg, marginTop:2 }}>{a.reason}</div>
              </div>
              <button style={{
                width:28, height:28, borderRadius:6, border:'none',
                background:'rgba(255,255,255,0.6)', color:'#475569', cursor:'pointer',
                display:'inline-flex', alignItems:'center', justifyContent:'center',
              }}><Icon.ChevronRight size={14}/></button>
            </div>
          );
        })}
      </div>
    </GdSection>
  );
}

// ── Finance card ─────────────────────────────────────────────────────
function FinanceCard({ fin, fee }) {
  const total = fin.paid + fin.expected + fin.overdue;
  const segs = [
    { v: fin.paid,     bg:'#10b981', label:'Оплачено' },
    { v: fin.expected, bg:'#f59e0b', label:'Ожидаем' },
    { v: fin.overdue,  bg:'#ef4444', label:'Просрочка' },
  ];
  return (
    <GdSection
      title="Финансы за май"
      subtitle={`Месячная стоимость ₽${fee.toLocaleString('ru-RU')}`}
      right={<Icon.CreditCard size={16} stroke="#94a3b8"/>}
      padding={18}
    >
      <div style={{ fontSize:24, fontWeight:700, letterSpacing:'-0.02em',
        fontVariantNumeric:'tabular-nums' }}>
        ₽{total.toLocaleString('ru-RU')}
      </div>
      <div style={{ fontSize:12, color:'#64748b', marginTop:2 }}>
        ожидаемый месячный сбор
      </div>

      <div style={{ display:'flex', gap:2, marginTop:14, height:8, borderRadius:9999, overflow:'hidden' }}>
        {segs.map((s, i) => (
          <div key={i} style={{ flex: s.v / total, background:s.bg }}/>
        ))}
      </div>

      <div style={{ display:'flex', flexDirection:'column', marginTop:14 }}>
        <FinRow color="#10b981" label="Оплачено"   amount={fin.paid}     count={fin.paidCount}     verb="плательщ." />
        <FinRow color="#f59e0b" label="Ожидаем"    amount={fin.expected} count={fin.expectedCount} verb="плательщ." />
        <FinRow color="#ef4444" label="Просрочка"  amount={fin.overdue}  count={fin.overdueCount}  verb="плательщ." />
      </div>

      <a href="#" style={{
        display:'inline-flex', alignItems:'center', gap:4, marginTop:12,
        fontSize:12.5, color:'#4f46e5', fontWeight:500,
      }}>Все платежи группы <Icon.ArrowRight size={13}/></a>
    </GdSection>
  );
}
function FinRow({ color, label, amount, count, verb }) {
  return (
    <div style={{
      display:'grid', gridTemplateColumns:'10px 1fr auto', alignItems:'center', gap:10,
      padding:'8px 0', borderTop:'1px solid #f1f5f9',
    }}>
      <span style={{ width:8, height:8, borderRadius:9999, background:color }}/>
      <span style={{ fontSize:13, color:'#334155' }}>
        {label}
        <span style={{ color:'#94a3b8', fontSize:12 }}> · {count} {verb}</span>
      </span>
      <span style={{ fontSize:13, fontWeight:600, color:'#0f172a',
        fontVariantNumeric:'tabular-nums' }}>
        ₽{amount.toLocaleString('ru-RU')}
      </span>
    </div>
  );
}

// ── Students preview ─────────────────────────────────────────────────
function StudentsPreview({ students }) {
  return (
    <div style={{ overflowX:'auto' }}>
      <table style={{ width:'100%', borderCollapse:'collapse', minWidth:760 }}>
        <thead>
          <tr style={{ fontSize:11, color:'#94a3b8', textTransform:'uppercase',
            letterSpacing:'0.06em' }}>
            <th style={{ ...th, paddingLeft:0 }}>Студент</th>
            <th style={th}>Посещаемость</th>
            <th style={th}>Последние 12 уроков</th>
            <th style={th}>Балл</th>
            <th style={{ ...th, textAlign:'right', paddingRight:0 }}>Оплата</th>
          </tr>
        </thead>
        <tbody>
          {students.map((s, i) => (
            <tr key={s.id} style={{ borderTop:'1px solid #f1f5f9' }}>
              <td style={{ ...td, paddingLeft:0 }}>
                <div style={{ display:'flex', alignItems:'center', gap:10 }}>
                  <Avatar name={s.name} size={32}/>
                  <div>
                    <div style={{ fontSize:13.5, fontWeight:500 }}>{s.name}</div>
                    <div style={{ fontSize:11.5, color:'#94a3b8' }}>
                      {s.role}{s.note ? ` · ${s.note}` : ''}
                    </div>
                  </div>
                </div>
              </td>
              <td style={td}>
                <div style={{ display:'flex', alignItems:'center', gap:10, maxWidth:200 }}>
                  <div style={{ flex:1, height:6, borderRadius:9999, background:'#f1f5f9',
                    overflow:'hidden' }}>
                    <div style={{
                      width:`${Math.round(s.attendance*100)}%`, height:'100%',
                      background: s.attendance >= 0.9 ? '#10b981'
                                : s.attendance >= 0.75 ? '#f59e0b'
                                : '#ef4444',
                    }}/>
                  </div>
                  <span style={{ fontSize:12.5, fontWeight:600,
                    color: s.attendance >= 0.9 ? '#047857'
                         : s.attendance >= 0.75 ? '#92400e'
                         : '#b91c1c',
                    fontVariantNumeric:'tabular-nums', minWidth:36, textAlign:'right' }}>
                    {Math.round(s.attendance*100)}%
                  </span>
                </div>
              </td>
              <td style={td}>
                <GdStudentSpark data={s.spark}/>
              </td>
              <td style={td}>
                <span style={{ fontSize:13, fontWeight:600,
                  fontVariantNumeric:'tabular-nums', color:'#0f172a' }}>
                  {s.grade.toFixed(1)}
                </span>
                <span style={{ fontSize:12, color:'#94a3b8' }}> / 5</span>
              </td>
              <td style={{ ...td, textAlign:'right', paddingRight:0 }}>
                {s.balance < 0 ? (
                  <Badge variant="danger" dot>
                    Долг ₽{Math.abs(s.balance).toLocaleString('ru-RU')}
                  </Badge>
                ) : (
                  <span style={{ fontSize:12.5, color:'#94a3b8' }}>оплачено</span>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
const th = { textAlign:'left', fontWeight:600, fontSize:11,
  padding:'10px 12px', borderBottom:'1px solid #e2e8f0' };
const td = { padding:'12px 12px', fontSize:13, color:'#0f172a', verticalAlign:'middle' };

// ── Activity feed ────────────────────────────────────────────────────
function ActivityFeed({ items }) {
  const tones = {
    primary: { bg:'rgba(79,70,229,0.10)',  fg:'#4338ca' },
    success: { bg:'rgba(16,185,129,0.12)', fg:'#047857' },
    warning: { bg:'rgba(245,158,11,0.16)', fg:'#92400e' },
    default: { bg:'#f1f5f9',               fg:'#475569' },
  };
  return (
    <div style={{ display:'grid', gridTemplateColumns:'1fr 1fr', gap:'2px 32px' }}>
      {items.map((a, i) => {
        const Ic = Icon[a.icon];
        const t = tones[a.tone] || tones.default;
        return (
          <div key={a.id} style={{
            display:'grid', gridTemplateColumns:'32px 1fr', gap:12,
            padding:'12px 0',
            borderTop: i > 1 ? '1px solid #f1f5f9' : '0',
          }}>
            <div style={{
              width:32, height:32, borderRadius:8,
              background:t.bg, color:t.fg,
              display:'flex', alignItems:'center', justifyContent:'center',
            }}><Ic size={15}/></div>
            <div style={{ minWidth:0 }}>
              <div style={{ fontSize:13, color:'#0f172a', lineHeight:1.5 }}>{a.text}</div>
              <div style={{ fontSize:11.5, color:'#94a3b8', marginTop:4 }}>
                {a.when} · {a.actor}
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
}

window.GroupApp = GroupApp;
