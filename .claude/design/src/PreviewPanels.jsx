// ── Live preview panels ────────────────────────────────────────────────────

function PreviewLabel({ idx }) {
  const labels = [
    { eyebrow: 'Как увидят студенты',   title: 'Кабинет студента' },
    { eyebrow: 'Применение бренда',     title: 'Сертификат и вход' },
    { eyebrow: 'Что увидит команда',     title: 'Раздел «Команда»' },
    { eyebrow: 'База школы',            title: 'Раздел «Студенты»' },
    { eyebrow: 'Активные сервисы',      title: 'Подключённые интеграции' },
  ];
  const l = labels[idx];
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 24 }}>
      <Icon.Eye size={15} stroke="#94a3b8" sw={2}/>
      <span style={{
        fontSize: 11, fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase',
        color: '#94a3b8',
      }}>Предпросмотр · {l.eyebrow}</span>
      <span style={{ flex: 1, height: 1, background: '#e2e8f0' }}/>
    </div>
  );
}

// ── Shared mini chrome ─────────────────────────────────────────────────────
function BrowserChrome({ url, children, height = 'auto' }) {
  return (
    <div style={{
      borderRadius: 14, overflow: 'hidden', background: '#fff',
      border: '1px solid #e2e8f0', boxShadow: '0 12px 32px rgba(15,23,42,0.08)',
      height,
    }}>
      <div style={{
        display: 'flex', alignItems: 'center', gap: 10,
        padding: '10px 14px', borderBottom: '1px solid #f1f5f9',
        background: '#f8fafc',
      }}>
        <div style={{ display: 'flex', gap: 6 }}>
          {['#ef4444','#f59e0b','#10b981'].map((c,i) => (
            <span key={i} style={{ width: 10, height: 10, borderRadius: 999, background: c, opacity: 0.55 }}/>
          ))}
        </div>
        <div style={{
          flex: 1, background: '#fff', border: '1px solid #e2e8f0', borderRadius: 6,
          padding: '4px 10px', fontSize: 11.5, color: '#64748b', fontFamily: 'var(--edv-font-mono)',
          display: 'flex', alignItems: 'center', gap: 6,
        }}>
          <Icon.Lock size={10} sw={2} stroke="#10b981"/>{url}
        </div>
      </div>
      {children}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// PREVIEW 1 — School
// ═══════════════════════════════════════════════════════════════════════════
const FIELD_LABELS = {
  languages: 'Иностранные языки',
  school: 'Подготовка к экзаменам',
  it: 'IT и программирование',
  design: 'Дизайн и творчество',
  business: 'Бизнес и маркетинг',
  soft: 'Soft skills',
  kids: 'Дети и подростки',
  other: 'Другое',
};

const TZ_LABELS = {
  'Europe/Kaliningrad': { label: 'Калининград', off: 'UTC+2', cur: '14:32' },
  'Europe/Moscow':      { label: 'Москва',      off: 'UTC+3', cur: '15:32' },
  'Europe/Samara':      { label: 'Самара',      off: 'UTC+4', cur: '16:32' },
  'Asia/Yekaterinburg': { label: 'Екатеринбург', off: 'UTC+5', cur: '17:32' },
  'Asia/Novosibirsk':   { label: 'Новосибирск', off: 'UTC+7', cur: '19:32' },
  'Asia/Vladivostok':   { label: 'Владивосток', off: 'UTC+10', cur: '22:32' },
};

function PreviewSchool({ data }) {
  const s = data.school;
  const name = s.name || 'Название школы';
  const sub = s.subdomain || 'your-school';
  const tz = TZ_LABELS[s.timezone] || TZ_LABELS['Europe/Moscow'];
  return (
    <BrowserChrome url={`${sub}.edvantix.ru`}>
      {/* Mini hero */}
      <div style={{
        padding: '36px 32px 28px',
        background: 'linear-gradient(180deg, #f5f7ff 0%, #fff 100%)',
        borderBottom: '1px solid #f1f5f9',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 14, marginBottom: 18 }}>
          <div style={{
            width: 56, height: 56, borderRadius: 14,
            background: 'linear-gradient(135deg, #6366f1, #4338ca)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            color: '#fff', fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em',
            boxShadow: '0 4px 14px rgba(99,102,241,.32)',
          }}>{name[0]?.toUpperCase() || '?'}</div>
          <div>
            <div style={{ fontSize: 11, color: '#94a3b8', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em' }}>
              {FIELD_LABELS[s.field] || 'Школа'}
            </div>
            <div style={{ fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em', color: '#0f172a' }}>
              {name}
            </div>
          </div>
        </div>
        <p style={{ margin: 0, fontSize: 14, color: '#475569', lineHeight: 1.5 }}>
          Добро пожаловать! Здесь вы найдёте свои курсы, расписание и материалы.
        </p>
      </div>

      {/* Mock content */}
      <div style={{ padding: 24, display: 'flex', flexDirection: 'column', gap: 12 }}>
        <div style={{
          display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 10,
        }}>
          {[
            { l: 'Текущих курсов', v: '0', i: 'BookOpen' },
            { l: 'Уроков на неделе', v: '0', i: 'CalendarDays' },
            { l: 'Часовой пояс', v: tz.off, i: 'Clock' },
          ].map((m, i) => {
            const G = Icon[m.i];
            return (
              <div key={i} style={{
                padding: 12, background: '#f8fafc', borderRadius: 10, border: '1px solid #f1f5f9',
              }}>
                <G size={14} stroke="#6366f1" sw={2}/>
                <div style={{ marginTop: 8, fontSize: 20, fontWeight: 700, letterSpacing: '-0.02em', color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>{m.v}</div>
                <div style={{ fontSize: 11.5, color: '#64748b', marginTop: 2 }}>{m.l}</div>
              </div>
            );
          })}
        </div>

        <div style={{
          padding: 14, background: '#fff', borderRadius: 10, border: '1px dashed #cbd5e1',
          display: 'flex', alignItems: 'center', gap: 10,
        }}>
          <Icon.Globe size={16} stroke="#94a3b8" sw={2}/>
          <span style={{ fontSize: 13, color: '#64748b' }}>Адрес кабинета:</span>
          <span style={{ fontFamily: 'var(--edv-font-mono)', fontSize: 12.5, color: '#0f172a' }}>
            {sub}.edvantix.ru
          </span>
        </div>
      </div>
    </BrowserChrome>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// PREVIEW 2 — Branding (certificate + login)
// ═══════════════════════════════════════════════════════════════════════════
function PreviewBranding({ data }) {
  const color = data.branding.color;
  const dark = shade(color, -20);
  const name = data.school.name || 'Ваша школа';
  const letter = data.branding.logoLetter || name.trim()[0]?.toUpperCase() || 'Ш';

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
      {/* Certificate */}
      <div style={{
        borderRadius: 14, overflow: 'hidden',
        boxShadow: '0 12px 32px rgba(15,23,42,0.10)',
        background: '#fff', border: '1px solid #e2e8f0',
        position: 'relative',
      }}>
        {/* top stripe */}
        <div style={{ height: 8, background: `linear-gradient(90deg, ${color}, ${dark})` }}/>
        <div style={{ padding: '28px 32px 32px', position: 'relative' }}>
          {/* watermark seal */}
          <div style={{
            position: 'absolute', right: 28, top: 28,
            width: 56, height: 56, borderRadius: 999,
            background: `${color}15`, border: `1.5px solid ${color}40`,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            color: color,
          }}>
            <Icon.Award size={28} sw={1.75}/>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 22 }}>
            <div style={{
              width: 36, height: 36, borderRadius: 8,
              background: `linear-gradient(135deg, ${color}, ${dark})`,
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              color: '#fff', fontSize: 16, fontWeight: 700,
            }}>{letter}</div>
            <div>
              <div style={{ fontSize: 11, fontWeight: 600, letterSpacing: '0.08em', textTransform: 'uppercase', color: '#94a3b8' }}>Сертификат</div>
              <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{name}</div>
            </div>
          </div>
          <div style={{ fontSize: 11, color: '#94a3b8', letterSpacing: '0.06em' }}>НАСТОЯЩИМ ПОДТВЕРЖДАЕТСЯ, ЧТО</div>
          <div style={{ fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em', margin: '6px 0 4px', color: '#0f172a' }}>
            Анна Иванова
          </div>
          <div style={{ fontSize: 13, color: '#475569', maxWidth: 360, lineHeight: 1.5 }}>
            прошла курс <strong style={{ color: '#0f172a', fontWeight: 600 }}>«Английский B2 — Upper Intermediate»</strong> в объёме 64 академических часа
          </div>
          <div style={{
            display: 'flex', alignItems: 'flex-end', justifyContent: 'space-between',
            marginTop: 28, paddingTop: 18, borderTop: '1px solid #f1f5f9',
          }}>
            <div>
              <div style={{ fontFamily: '"Brush Script MT", cursive', fontSize: 24, color: color, lineHeight: 1, fontStyle: 'italic' }}>А.&nbsp;Петрова</div>
              <div style={{ fontSize: 11, color: '#94a3b8', marginTop: 4 }}>Директор школы</div>
            </div>
            <div style={{ textAlign: 'right' }}>
              <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>15 июля 2026</div>
              <div style={{ fontSize: 11, color: '#94a3b8', fontFamily: 'var(--edv-font-mono)' }}>ED‑2026‑000142</div>
            </div>
          </div>
        </div>
      </div>

      {/* Login button preview */}
      <div style={{
        padding: 20, borderRadius: 14, background: '#fff',
        border: '1px solid #e2e8f0',
        boxShadow: '0 1px 3px rgba(0,0,0,0.04)',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 14 }}>
          <div style={{ fontSize: 12.5, fontWeight: 600, color: '#475569' }}>Кнопки и ссылки в кабинете</div>
          <span style={{ fontSize: 11, color: '#94a3b8', fontFamily: 'var(--edv-font-mono)' }}>{color}</span>
        </div>
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8, alignItems: 'center' }}>
          <button style={{
            background: color, color: '#fff', border: 0,
            padding: '10px 18px', borderRadius: 10, fontSize: 13.5, fontWeight: 600,
            boxShadow: `0 4px 12px ${color}45`,
          }}>Начать урок</button>
          <button style={{
            background: '#fff', color: color, border: `1px solid ${color}55`,
            padding: '10px 16px', borderRadius: 10, fontSize: 13.5, fontWeight: 600,
          }}>Подробнее</button>
          <a style={{ color: color, fontSize: 13.5, fontWeight: 500, textDecoration: 'underline', textDecorationStyle: 'dotted', textUnderlineOffset: 3 }}>Открыть профиль →</a>
          <span style={{ marginLeft: 'auto', display: 'inline-flex', alignItems: 'center', gap: 6, padding: '4px 10px', borderRadius: 999, background: `${color}15`, color: color, fontSize: 12, fontWeight: 600 }}>
            <span style={{ width: 6, height: 6, borderRadius: 999, background: color }}/>
            Активно
          </span>
        </div>
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// PREVIEW 3 — Team
// ═══════════════════════════════════════════════════════════════════════════
function PreviewTeam({ data }) {
  const invites = data.team.invites;
  // include owner as already-active member
  const owner = {
    name: 'Алина Петрова', email: 'alina@' + (data.school.subdomain || 'school') + '.ru',
    role: 'Владелец', status: 'active',
  };
  const all = [
    owner,
    ...invites.map(i => ({ name: i.name, email: i.email, role: ROLES.find(r => r.id === i.role)?.label || i.role, status: 'invited' }))
  ];
  return (
    <BrowserChrome url={`${data.school.subdomain || 'your-school'}.edvantix.ru/team`}>
      <div style={{ padding: '20px 24px 8px', borderBottom: '1px solid #f1f5f9' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <div>
            <div style={{ fontSize: 18, fontWeight: 700, letterSpacing: '-0.02em', color: '#0f172a' }}>Команда</div>
            <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>
              {all.length} {plural(all.length, ['участник','участника','участников'])} · {invites.length} {plural(invites.length, ['приглашение','приглашения','приглашений'])}
            </div>
          </div>
          <button style={{
            background: '#4f46e5', color: '#fff', border: 0,
            padding: '7px 12px', borderRadius: 8, fontSize: 12.5, fontWeight: 600,
            display: 'inline-flex', alignItems: 'center', gap: 6,
          }}>
            <Icon.Plus size={12} sw={3}/> Пригласить
          </button>
        </div>
      </div>
      <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
        {all.slice(0, 6).map((m, i) => (
          <li key={i} style={{
            display: 'flex', alignItems: 'center', gap: 12,
            padding: '12px 24px', borderBottom: i < Math.min(5, all.length-1) ? '1px solid #f1f5f9' : 0,
          }}>
            <Avatar name={m.name} size={36}/>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{m.name}</div>
              <div style={{ fontSize: 12, color: '#64748b' }}>{m.email}</div>
            </div>
            <span style={{
              fontSize: 12, color: '#64748b', minWidth: 100, textAlign: 'right',
            }}>{m.role}</span>
            <span style={{
              display: 'inline-flex', alignItems: 'center', gap: 5,
              fontSize: 11.5, fontWeight: 600,
              padding: '3px 9px', borderRadius: 999,
              background: m.status === 'active' ? '#d1fae5' : '#fef3c7',
              color: m.status === 'active' ? '#047857' : '#92400e',
              minWidth: 92, justifyContent: 'center',
            }}>
              <span style={{ width: 5, height: 5, borderRadius: 999, background: 'currentColor' }}/>
              {m.status === 'active' ? 'Активен' : 'Ожидает'}
            </span>
          </li>
        ))}
        {all.length === 1 && (
          <li style={{
            padding: '28px 24px', textAlign: 'center', color: '#94a3b8', fontSize: 13,
          }}>
            <Icon.UserPlus size={28} stroke="#cbd5e1" sw={1.5}/>
            <div style={{ marginTop: 8 }}>Добавьте email слева, чтобы пригласить первого преподавателя</div>
          </li>
        )}
      </ul>
    </BrowserChrome>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// PREVIEW 4 — Students
// ═══════════════════════════════════════════════════════════════════════════
const SAMPLE_STUDENTS = [
  { name: 'Анна Иванова',       email: 'anna.ivanova@email.ru',  group: 'B2 · Утро',      paid: true,  prog: 64 },
  { name: 'Пётр Смирнов',        email: 'petr.smirnov@email.ru',   group: 'B1 · Вечер',     paid: true,  prog: 32 },
  { name: 'Елена Ковалёва',     email: 'elena.k@gmail.com',       group: 'B2 · Утро',      paid: false, prog: 18 },
  { name: 'Михаил Соколов',     email: 'mikhail.s@email.ru',      group: 'C1 · Интенсив',  paid: true,  prog: 47 },
  { name: 'Дарья Лебедева',     email: 'darya.l@yandex.ru',       group: 'A2 · Вечер',     paid: true,  prog: 12 },
  { name: 'Александр Новиков',  email: 'a.novikov@email.ru',      group: 'B2 · Утро',      paid: false, prog: 0  },
];

function PreviewStudents({ data }) {
  const method = data.students.method;
  const count = method === 'csv' ? data.students.csvCount : method === 'manual' ? data.students.manualCount : 0;
  const empty = !method || count === 0;

  return (
    <BrowserChrome url={`${data.school.subdomain || 'your-school'}.edvantix.ru/students`}>
      <div style={{ padding: '20px 24px 14px', borderBottom: '1px solid #f1f5f9' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
          <div>
            <div style={{ fontSize: 18, fontWeight: 700, letterSpacing: '-0.02em', color: '#0f172a' }}>Студенты</div>
            <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>
              {empty ? '0 студентов · база пуста' : `${count} ${plural(count, ['студент','студента','студентов'])} · готовы к приглашению`}
            </div>
          </div>
          <div style={{ display: 'flex', gap: 6 }}>
            <button style={{
              background: '#fff', border: '1px solid #e2e8f0', color: '#475569',
              padding: '6px 10px', borderRadius: 8, fontSize: 12.5, fontWeight: 500,
              display: 'inline-flex', alignItems: 'center', gap: 5,
            }}>
              <Icon.Upload size={12}/> Импорт
            </button>
            <button style={{
              background: '#4f46e5', color: '#fff', border: 0,
              padding: '6px 10px', borderRadius: 8, fontSize: 12.5, fontWeight: 600,
              display: 'inline-flex', alignItems: 'center', gap: 5,
            }}>
              <Icon.Plus size={12} sw={3}/> Добавить
            </button>
          </div>
        </div>
        {/* search row */}
        <div style={{
          display: 'flex', alignItems: 'center', gap: 8, padding: '7px 11px',
          background: '#f8fafc', border: '1px solid #f1f5f9', borderRadius: 8,
          color: '#94a3b8', fontSize: 12.5,
        }}>
          <Icon.Search size={13} sw={2}/> Поиск по имени, email или группе…
        </div>
      </div>
      {empty ? (
        <div style={{ padding: '36px 24px', textAlign: 'center' }}>
          <div style={{
            width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
            display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            color: '#94a3b8', marginBottom: 12,
          }}>
            <Icon.UserPlus size={26} sw={1.75}/>
          </div>
          <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a', marginBottom: 4 }}>База пуста</div>
          <div style={{ fontSize: 12.5, color: '#64748b', maxWidth: 240, margin: '0 auto' }}>
            Импортируйте таблицу или добавьте студентов вручную в форме слева.
          </div>
        </div>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
          <thead>
            <tr style={{ fontSize: 11, color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.06em', fontWeight: 600 }}>
              <th style={{ textAlign: 'left', padding: '10px 24px', fontWeight: 600 }}>Студент</th>
              <th style={{ textAlign: 'left', padding: '10px 8px', fontWeight: 600 }}>Группа</th>
              <th style={{ textAlign: 'left', padding: '10px 8px', fontWeight: 600 }}>Прогресс</th>
              <th style={{ textAlign: 'right', padding: '10px 24px', fontWeight: 600 }}>Оплата</th>
            </tr>
          </thead>
          <tbody>
            {SAMPLE_STUDENTS.slice(0, Math.min(6, count || 6)).map((s, i) => (
              <tr key={i} style={{ borderTop: '1px solid #f1f5f9' }}>
                <td style={{ padding: '10px 24px' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                    <Avatar name={s.name} size={28}/>
                    <div>
                      <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{s.name}</div>
                      <div style={{ fontSize: 11.5, color: '#94a3b8' }}>{s.email}</div>
                    </div>
                  </div>
                </td>
                <td style={{ padding: '10px 8px', color: '#475569', fontSize: 12.5 }}>{s.group}</td>
                <td style={{ padding: '10px 8px' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                    <div style={{ width: 60, height: 5, background: '#f1f5f9', borderRadius: 999, overflow: 'hidden' }}>
                      <div style={{ width: `${s.prog}%`, height: '100%', background: '#4f46e5', borderRadius: 999 }}/>
                    </div>
                    <span style={{ fontSize: 11.5, color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>{s.prog}%</span>
                  </div>
                </td>
                <td style={{ padding: '10px 24px', textAlign: 'right' }}>
                  <span style={{
                    display: 'inline-flex', alignItems: 'center', gap: 5,
                    padding: '3px 8px', borderRadius: 999,
                    background: s.paid ? '#d1fae5' : '#fef3c7',
                    color: s.paid ? '#047857' : '#92400e',
                    fontSize: 11.5, fontWeight: 600,
                  }}>
                    <span style={{ width: 5, height: 5, borderRadius: 999, background: 'currentColor' }}/>
                    {s.paid ? 'Оплачено' : 'Ожидает'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
      {!empty && count > 6 && (
        <div style={{
          padding: '10px 24px', borderTop: '1px solid #f1f5f9',
          fontSize: 12, color: '#94a3b8', textAlign: 'center',
        }}>
          …и ещё {count - 6} {plural(count - 6, ['студент','студента','студентов'])}
        </div>
      )}
    </BrowserChrome>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// PREVIEW 5 — Integrations
// ═══════════════════════════════════════════════════════════════════════════
function PreviewIntegrations({ data }) {
  const enabled = INTEGRATIONS.filter(it => data.integrations.enabled[it.id]);
  const disabled = INTEGRATIONS.filter(it => !data.integrations.enabled[it.id]);

  return (
    <BrowserChrome url={`${data.school.subdomain || 'your-school'}.edvantix.ru/settings/integrations`}>
      <div style={{ padding: '20px 24px 14px', borderBottom: '1px solid #f1f5f9' }}>
        <div style={{ fontSize: 18, fontWeight: 700, letterSpacing: '-0.02em', color: '#0f172a' }}>Интеграции</div>
        <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>
          {enabled.length} активн{enabled.length === 1 ? 'а' : enabled.length >= 2 && enabled.length <= 4 ? 'ы' : 'о'} · {disabled.length} доступн{disabled.length === 1 ? 'а' : 'о'}
        </div>
      </div>

      <div style={{ padding: 20 }}>
        {enabled.length > 0 && (
          <>
            <SectionTitle>Подключено</SectionTitle>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 10, marginBottom: 20 }}>
              {enabled.map(it => <IntegrationTile key={it.id} it={it} on/>)}
            </div>
          </>
        )}
        {disabled.length > 0 && (
          <>
            <SectionTitle>Можно подключить</SectionTitle>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 10 }}>
              {disabled.map(it => <IntegrationTile key={it.id} it={it}/>)}
            </div>
          </>
        )}
        {enabled.length === 0 && (
          <div style={{
            padding: 14, borderRadius: 10, background: '#fffbeb', border: '1px solid #fef3c7',
            display: 'flex', alignItems: 'center', gap: 10, marginTop: 12,
          }}>
            <Icon.Info size={16} stroke="#f59e0b" sw={2}/>
            <span style={{ fontSize: 12.5, color: '#92400e', lineHeight: 1.5 }}>
              Без платежей и Telegram‑бота школа всё равно работает, но придётся вручную выставлять счета и отправлять уведомления.
            </span>
          </div>
        )}
      </div>
    </BrowserChrome>
  );
}

function SectionTitle({ children }) {
  return (
    <div style={{
      fontSize: 10.5, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase',
      color: '#94a3b8', marginBottom: 10,
    }}>{children}</div>
  );
}

function IntegrationTile({ it, on }) {
  const G = Icon[it.icon];
  return (
    <div style={{
      padding: 12, borderRadius: 10,
      background: '#fff', border: '1px solid #e2e8f0',
      opacity: on ? 1 : 0.65,
      display: 'flex', flexDirection: 'column', gap: 8,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <div style={{
          width: 30, height: 30, borderRadius: 8,
          background: on ? it.color : '#f1f5f9',
          color: on ? '#fff' : '#94a3b8',
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <G size={15} sw={2}/>
        </div>
        <span style={{
          display: 'inline-flex', alignItems: 'center', gap: 4,
          fontSize: 10.5, fontWeight: 700,
          padding: '2px 7px', borderRadius: 999,
          background: on ? '#d1fae5' : '#f1f5f9',
          color: on ? '#047857' : '#94a3b8',
        }}>
          <span style={{ width: 5, height: 5, borderRadius: 999, background: 'currentColor' }}/>
          {on ? 'Активно' : 'Откл.'}
        </span>
      </div>
      <div>
        <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{it.label}</div>
        <div style={{ fontSize: 11.5, color: '#94a3b8', marginTop: 2 }}>{it.detail}</div>
      </div>
    </div>
  );
}

// Add Search icon (used here but not in Icons.jsx)
if (!Icon.Search) {
  Icon.Search = (p) => (
    <svg width={p?.size||16} height={p?.size||16} viewBox="0 0 24 24" fill="none" stroke={p?.stroke||'currentColor'} strokeWidth={p?.sw||2} strokeLinecap="round" strokeLinejoin="round">
      <circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/>
    </svg>
  );
}

Object.assign(window, {
  PreviewLabel, BrowserChrome,
  PreviewSchool, PreviewBranding, PreviewTeam, PreviewStudents, PreviewIntegrations,
  FIELD_LABELS, TZ_LABELS,
});
