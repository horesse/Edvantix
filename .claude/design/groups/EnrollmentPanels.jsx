// Step 3 — Enrollment side-panels: invites, recruitment, welcome, roster, finish bar.

// ═══════════════════════════════════════════════════════════════════
// Invite picker — chip input for emails
// ═══════════════════════════════════════════════════════════════════
function InvitePicker({ invites, onAdd, onRemove }) {
  const [input, setInput] = React.useState('');
  const [name, setName]   = React.useState('');
  const [err, setErr]     = React.useState('');

  const submit = () => {
    const email = input.trim().toLowerCase();
    if (!email) return;
    if (!isValidEmailEN(email)) { setErr('Похоже на неправильный email'); return; }
    if (invites.some(i => i.email === email)) { setErr('Этот email уже в списке'); return; }
    onAdd({ email, name: name.trim() || null });
    setInput(''); setName(''); setErr('');
  };
  const onKey = (e) => {
    if (e.key === 'Enter') { e.preventDefault(); submit(); }
    if (e.key === ',' || e.key === ';') { e.preventDefault(); submit(); }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr auto', gap: 10,
        alignItems: 'flex-start' }}>
        <F.Field label="Email студента" required>
          <F.Text type="email" value={input}
            onChange={e => { setInput(e.target.value); setErr(''); }}
            onKeyDown={onKey}
            placeholder="student@example.com"
            error={err}
            icon={<Icon.Mail size={16} />}
          />
        </F.Field>
        <F.Field label="Имя" optional hint="Появится в письме как обращение">
          <F.Text value={name}
            onChange={e => setName(e.target.value)}
            onKeyDown={onKey}
            placeholder="Иван Петров"
          />
        </F.Field>
        <div style={{ paddingTop: 26 }}>
          <Button variant="secondary" onClick={submit} disabled={!input.trim()}>
            <Icon.Plus size={14} />Добавить
          </Button>
        </div>
      </div>

      {invites.length > 0 ? (
        <div style={{
          padding: 10, borderRadius: 12, background: '#fafbfc',
          border: '1px solid #e2e8f0', display: 'flex', flexDirection: 'column', gap: 4,
        }}>
          <div style={{
            padding: '4px 8px 8px', fontSize: 11.5, fontWeight: 600, color: '#64748b',
            letterSpacing: '0.05em', textTransform: 'uppercase',
            display: 'flex', alignItems: 'center', gap: 8,
          }}>
            <Icon.Mail size={12} stroke="#94a3b8" />
            <span>Будут отправлены ({invites.length})</span>
          </div>
          {invites.map((inv, i) => (
            <div key={i} style={{
              display: 'flex', alignItems: 'center', gap: 10,
              padding: '8px 10px', borderRadius: 8, background: '#fff',
              border: '1px solid #e2e8f0',
            }}>
              <Avatar name={inv.name || inv.email} size={28} />
              <div style={{ flex: 1, minWidth: 0 }}>
                {inv.name && (
                  <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>
                    {inv.name}
                  </div>
                )}
                <div style={{
                  fontSize: 12, color: inv.name ? '#64748b' : '#0f172a',
                  fontFamily: 'var(--edv-font-mono)',
                }}>{inv.email}</div>
              </div>
              <Badge variant="primary" dot>Новый</Badge>
              <button onClick={() => onRemove(i)}
                style={{
                  width: 26, height: 26, borderRadius: 6, border: 'none',
                  background: 'transparent', color: '#94a3b8', cursor: 'pointer',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                }}
                onMouseEnter={e => { e.currentTarget.style.background = '#fee2e2';
                  e.currentTarget.style.color = '#b91c1c'; }}
                onMouseLeave={e => { e.currentTarget.style.background = 'transparent';
                  e.currentTarget.style.color = '#94a3b8'; }}
                title="Удалить">
                <Icon.X size={13} />
              </button>
            </div>
          ))}
        </div>
      ) : (
        <div style={{
          padding: '14px 16px', borderRadius: 12,
          background: '#fafbfc', border: '1px dashed #e2e8f0',
          display: 'flex', alignItems: 'center', gap: 12,
        }}>
          <div style={{
            width: 32, height: 32, borderRadius: 8, flexShrink: 0,
            background: '#f1f5f9', color: '#94a3b8',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
          }}>
            <Icon.Mail size={16} stroke="#94a3b8" />
          </div>
          <div style={{ flex: 1, fontSize: 12.5, color: '#64748b', lineHeight: 1.5 }}>
            Введите email сверху — каждый получит письмо с ссылкой на регистрацию и&nbsp;автоматически попадёт в группу после активации.
          </div>
        </div>
      )}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Recruitment policy — open enrollment toggle + requirements
// ═══════════════════════════════════════════════════════════════════
function RecruitmentPanel({ cfg, update, seatsLeft, group }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <ToggleRowEN
        icon="Megaphone"
        title="Открытый набор"
        desc={cfg.openRecruitment
          ? 'Группа появится в каталоге школы. Студенты могут оставить заявку самостоятельно.'
          : 'Группа закрыта для записи. Состав можно дополнить только вручную или приглашениями.'}
        checked={cfg.openRecruitment}
        onChange={v => update({ openRecruitment: v })}
      />

      {cfg.openRecruitment && (
        <div style={{
          padding: 16, borderRadius: 12,
          background: 'rgba(79,70,229,0.03)', border: '1px solid rgba(79,70,229,0.15)',
          display: 'flex', flexDirection: 'column', gap: 14,
        }}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
            <F.Field label="Принимаем заявки до" required
              hint="После этой даты группа автоматически закроется для записи">
              <F.Text type="date" value={cfg.recruitDeadline}
                onChange={e => update({ recruitDeadline: e.target.value })}
                icon={<Icon.Calendar size={16} />}
              />
            </F.Field>
            <F.Field label="Минимальный уровень"
              hint="Заявки от студентов с другим уровнем потребуют ручного подтверждения">
              <F.Select
                value={group.level}
                onChange={() => {}}
                options={STUDENT_LEVELS.map(l => ({ value: l, label: l }))}
                placeholder="Уровень"
              />
            </F.Field>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            <div style={{ fontSize: 12, fontWeight: 600, color: '#64748b',
              letterSpacing: '0.05em', textTransform: 'uppercase' }}>
              Требования к записи
            </div>
            <CompactToggle
              icon="CreditCard" title="Предоплата при записи"
              desc="Студент бронирует место после оплаты"
              checked={cfg.requirePayment}
              onChange={v => update({ requirePayment: v })}
            />
            <CompactToggle
              icon="ClipboardCheck" title="Тест на определение уровня"
              desc="Перед записью студент проходит короткий тест"
              checked={cfg.requireTest}
              onChange={v => update({ requireTest: v })}
            />
          </div>

          <PublicPreview group={group} seatsLeft={seatsLeft}
            requirePayment={cfg.requirePayment}
            requireTest={cfg.requireTest}
            deadline={cfg.recruitDeadline} />
        </div>
      )}
    </div>
  );
}

function PublicPreview({ group, seatsLeft, requirePayment, requireTest, deadline }) {
  const tone = LEVEL_TONES[GROUP_LEVELS.find(l => l.value === group.level)?.tone || 'slate'];
  const [y, m, d] = (deadline || '').split('-');
  const dlText = deadline ? `до ${d}.${m}` : '—';

  return (
    <div>
      <div style={{
        fontSize: 11.5, fontWeight: 600, color: '#64748b',
        letterSpacing: '0.05em', textTransform: 'uppercase',
        display: 'flex', alignItems: 'center', gap: 6, marginBottom: 8,
      }}>
        <Icon.Eye size={12} stroke="#94a3b8" />
        Как студенты увидят в каталоге
      </div>
      <div style={{
        background: '#fff', border: '1px solid #e2e8f0', borderRadius: 12,
        padding: 14, display: 'flex', alignItems: 'center', gap: 14,
      }}>
        <div style={{
          width: 48, height: 48, borderRadius: 12, flexShrink: 0,
          background: tone.bg, color: tone.fg,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontFamily: 'var(--edv-font-mono)', fontSize: 14, fontWeight: 700,
        }}>{group.level}</div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{group.name}</div>
          <div style={{ fontSize: 12, color: '#64748b', marginTop: 2,
            display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap' }}>
            <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
              <Icon.Calendar size={11} stroke="#94a3b8" />{group.schedule}
            </span>
            <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
            <span>Старт {group.starts}</span>
          </div>
          <div style={{ marginTop: 8, display: 'flex', flexWrap: 'wrap', gap: 6 }}>
            {seatsLeft > 0 ? (
              <Badge variant="success" dot>
                {seatsLeft} {declensionEN(seatsLeft, ['место','места','мест'])} свободно
              </Badge>
            ) : (
              <Badge variant="warning" dot>Только лист ожидания</Badge>
            )}
            <Badge variant="outline">{dlText}</Badge>
            {requirePayment && (
              <Badge variant="outline"><Icon.CreditCard size={11} />Предоплата</Badge>
            )}
            {requireTest && (
              <Badge variant="outline"><Icon.ClipboardCheck size={11} />Тест</Badge>
            )}
          </div>
        </div>
        <button style={{
          padding: '0 16px', height: 36, borderRadius: 9, border: 'none',
          background: '#4f46e5', color: '#fff', fontWeight: 600, fontSize: 13,
          fontFamily: 'inherit', cursor: 'default', flexShrink: 0,
        }}>Записаться</button>
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Welcome message panel
// ═══════════════════════════════════════════════════════════════════
function WelcomePanel({ cfg, update, enrolled, invites }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <ToggleRowEN
        icon="Bell"
        title="Известить зачисленных"
        desc={`После сохранения отправим письмо и push-уведомление ${enrolled + invites} ${declensionEN(enrolled + invites, ['студенту','студентам','студентам'])}. Расписание сразу появится в их кабинете.`}
        checked={cfg.notifyOnSave}
        onChange={v => update({ notifyOnSave: v })}
      />
      <ToggleRowEN
        icon="Mail"
        title="Использовать приветственный шаблон"
        desc="Шаблон письма с расписанием, кабинетом и преподавателем"
        checked={cfg.sendWelcome}
        onChange={v => update({ sendWelcome: v })}
      />

      {cfg.sendWelcome && (
        <div style={{
          padding: 16, borderRadius: 12,
          background: '#fafbfc', border: '1px solid #e2e8f0',
          display: 'flex', flexDirection: 'column', gap: 12,
        }}>
          <F.Field label="Тема письма" required>
            <F.Text value={cfg.welcomeSubject}
              onChange={e => update({ welcomeSubject: e.target.value })}
              placeholder="Вы зачислены в группу…"
            />
          </F.Field>
          <F.Field label="Текст приветствия"
            hint="Используйте переменные: {{firstName}}, {{groupName}}, {{startsAt}}, {{room}}, {{teacher}}">
            <F.Textarea value={cfg.welcomeBody} rows={7}
              onChange={e => update({ welcomeBody: e.target.value })} />
          </F.Field>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
            {['firstName','groupName','startsAt','room','teacher','schedule'].map(v => (
              <button key={v} type="button"
                onClick={() => update({ welcomeBody: cfg.welcomeBody + ` {{${v}}}` })}
                style={{
                  padding: '4px 10px', borderRadius: 9999,
                  border: '1px solid #e2e8f0', background: '#fff',
                  fontSize: 11.5, fontFamily: 'var(--edv-font-mono)',
                  color: '#4338ca', cursor: 'pointer',
                }}
                onMouseEnter={e => e.currentTarget.style.background = '#f0f4ff'}
                onMouseLeave={e => e.currentTarget.style.background = '#fff'}>
                + {`{{${v}}}`}
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Compact toggle (smaller than ToggleRowEN, for sub-options)
// ═══════════════════════════════════════════════════════════════════
function CompactToggle({ icon, title, desc, checked, onChange }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 12,
      padding: '10px 12px', borderRadius: 10,
      background: '#fff', border: '1px solid #e2e8f0',
    }}>
      <div style={{
        width: 28, height: 28, borderRadius: 8, flexShrink: 0,
        background: checked ? 'rgba(79,70,229,0.10)' : '#f1f5f9',
        color: checked ? '#4338ca' : '#94a3b8',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Ic size={14} />
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>{title}</div>
        <div style={{ fontSize: 11.5, color: '#64748b', marginTop: 1 }}>{desc}</div>
      </div>
      <SwitchEN checked={checked} onChange={onChange} />
    </div>
  );
}

function ToggleRowEN({ icon, title, desc, checked, onChange }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      display: 'flex', alignItems: 'flex-start', gap: 14,
      padding: '14px 16px', borderRadius: 12,
      border: `1px solid ${checked ? 'rgba(79,70,229,0.2)' : '#e2e8f0'}`,
      background: checked ? 'rgba(79,70,229,0.03)' : '#fff',
      transition: 'all .15s',
    }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: checked ? 'rgba(79,70,229,0.10)' : '#f1f5f9',
        color: checked ? '#4338ca' : '#64748b',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Ic size={16} />
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>{title}</div>
        <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2, lineHeight: 1.45 }}>{desc}</div>
      </div>
      <SwitchEN checked={checked} onChange={onChange} />
    </div>
  );
}

function SwitchEN({ checked, onChange }) {
  return (
    <button type="button" role="switch" aria-checked={checked}
      onClick={() => onChange(!checked)}
      style={{
        width: 38, height: 22, borderRadius: 9999, border: 'none',
        background: checked ? '#4f46e5' : '#cbd5e1', position: 'relative',
        cursor: 'pointer', flexShrink: 0, padding: 0, transition: 'background .15s',
      }}>
      <span style={{
        position: 'absolute', top: 2, left: checked ? 18 : 2,
        width: 18, height: 18, borderRadius: 9999, background: '#fff',
        boxShadow: '0 1px 3px rgba(0,0,0,0.15)', transition: 'left .15s',
      }} />
    </button>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Roster sidebar card — sticky preview of enrolled + waitlist + invites
// ═══════════════════════════════════════════════════════════════════
function RosterCard({ capacity, enrolled, waitlisted, invites, onRemove, onPromote, onWaitlist, seatsLeft }) {
  const pct = Math.min(100, Math.round(enrolled.length / capacity * 100));
  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      display: 'flex', flexDirection: 'column', maxHeight: 620, overflow: 'hidden',
    }}>
      <div style={{
        padding: '14px 16px', borderBottom: '1px solid #f1f5f9',
      }}>
        <div style={{
          display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12,
          fontSize: 11.5, fontWeight: 600, color: '#64748b',
          letterSpacing: '0.05em', textTransform: 'uppercase',
        }}>
          <Icon.Users size={13} stroke="#4f46e5" />
          <span style={{ flex: 1 }}>Состав группы</span>
          <span style={{
            fontSize: 11.5, fontWeight: 600,
            color: enrolled.length === capacity ? '#047857' : '#4338ca',
            textTransform: 'none', letterSpacing: 0,
            fontVariantNumeric: 'tabular-nums',
          }}>{enrolled.length} / {capacity}</span>
        </div>
        <div style={{
          height: 6, background: '#f1f5f9', borderRadius: 9999, overflow: 'hidden',
        }}>
          <div style={{
            width: `${pct}%`, height: '100%',
            background: enrolled.length === capacity ? '#10b981' : '#4f46e5',
            borderRadius: 9999, transition: 'width .3s',
          }} />
        </div>
      </div>

      <div style={{ flex: 1, overflowY: 'auto' }}>
        {/* Enrolled list */}
        {enrolled.length === 0 ? (
          <RosterEmpty />
        ) : (
          <div>
            <RosterSubheader label="Зачислены" count={enrolled.length} icon="Check" tone="primary" />
            {enrolled.map((s, i) => (
              <RosterRow key={s.id} student={s} number={i + 1}
                onRemove={() => onRemove(s.id)}
                onSecondary={() => onWaitlist(s.id)}
                secondaryLabel="В лист ожидания"
                secondaryIcon="Clock"
              />
            ))}
          </div>
        )}

        {/* Waitlist */}
        {waitlisted.length > 0 && (
          <div>
            <RosterSubheader label="Лист ожидания" count={waitlisted.length} icon="Clock" tone="warning" />
            {waitlisted.map((s, i) => (
              <RosterRow key={s.id} student={s} number={enrolled.length + i + 1}
                onRemove={() => onRemove(s.id)}
                onSecondary={() => onPromote(s.id)}
                secondaryLabel={seatsLeft > 0 ? 'Зачислить' : 'Мест нет'}
                secondaryIcon="ArrowUp"
                secondaryDisabled={seatsLeft <= 0}
                muted
              />
            ))}
          </div>
        )}

        {/* Invitations */}
        {invites.length > 0 && (
          <div>
            <RosterSubheader label="Приглашения" count={invites.length} icon="Mail" tone="slate" />
            {invites.map((inv, i) => (
              <div key={i} style={{
                display: 'flex', alignItems: 'center', gap: 10,
                padding: '8px 14px', borderTop: '1px solid #f8fafc',
              }}>
                <Avatar name={inv.name || inv.email} size={26} />
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div style={{ fontSize: 12.5, fontWeight: 600, color: '#0f172a',
                    overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {inv.name || inv.email}
                  </div>
                  {inv.name && (
                    <div style={{ fontSize: 10.5, color: '#94a3b8',
                      fontFamily: 'var(--edv-font-mono)',
                      overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      {inv.email}
                    </div>
                  )}
                </div>
                <Badge variant="outline">приглашён</Badge>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function RosterSubheader({ label, count, icon, tone }) {
  const tones = {
    primary: { bg: 'rgba(79,70,229,0.06)',  fg: '#4338ca' },
    warning: { bg: 'rgba(245,158,11,0.10)', fg: '#92400e' },
    slate:   { bg: '#f8fafc',                fg: '#475569' },
  };
  const t = tones[tone];
  const Ic = Icon[icon];
  return (
    <div style={{
      padding: '8px 16px', background: t.bg,
      borderTop: '1px solid #f1f5f9', borderBottom: '1px solid #f1f5f9',
      display: 'flex', alignItems: 'center', gap: 8,
      fontSize: 11, fontWeight: 700, color: t.fg,
      letterSpacing: '0.06em', textTransform: 'uppercase',
    }}>
      <Ic size={12} stroke={t.fg} />
      <span style={{ flex: 1 }}>{label}</span>
      <span style={{ fontVariantNumeric: 'tabular-nums' }}>{count}</span>
    </div>
  );
}

function RosterRow({ student: s, number, onRemove, onSecondary, secondaryLabel, secondaryIcon, secondaryDisabled, muted }) {
  const [hover, setHover] = React.useState(false);
  const Ic = Icon[secondaryIcon];
  return (
    <div
      onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      style={{
        display: 'flex', alignItems: 'center', gap: 10,
        padding: '8px 14px', borderTop: '1px solid #f8fafc',
        background: hover ? '#fafbfc' : 'transparent',
        opacity: muted ? 0.85 : 1,
      }}>
      <span style={{
        width: 18, fontSize: 10.5, color: '#94a3b8', textAlign: 'right',
        fontVariantNumeric: 'tabular-nums', fontFamily: 'var(--edv-font-mono)', flexShrink: 0,
      }}>{number}</span>
      <Avatar name={s.name} size={26} />
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 12.5, fontWeight: 600, color: '#0f172a',
          overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
          {s.name}
        </div>
        <div style={{ fontSize: 10.5, color: '#94a3b8',
          display: 'flex', alignItems: 'center', gap: 6 }}>
          <span style={{ fontFamily: 'var(--edv-font-mono)', fontWeight: 700 }}>{s.level}</span>
          <span style={{ width: 2, height: 2, borderRadius: 9999, background: '#cbd5e1' }} />
          <span>{s.age} лет</span>
        </div>
      </div>
      {hover && (
        <div style={{ display: 'flex', gap: 4, flexShrink: 0 }}>
          <button onClick={onSecondary} disabled={secondaryDisabled}
            title={secondaryLabel}
            style={{
              width: 24, height: 24, borderRadius: 6, border: 'none',
              background: '#f0f4ff', color: '#4338ca',
              cursor: secondaryDisabled ? 'not-allowed' : 'pointer',
              opacity: secondaryDisabled ? 0.4 : 1,
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            }}>
            <Ic size={11} stroke="#4338ca" sw={2.5} />
          </button>
          <button onClick={onRemove} title="Убрать"
            style={{
              width: 24, height: 24, borderRadius: 6, border: 'none',
              background: '#fef2f2', color: '#b91c1c', cursor: 'pointer',
              display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
            }}>
            <Icon.X size={11} sw={2.5} />
          </button>
        </div>
      )}
    </div>
  );
}

function RosterEmpty() {
  return (
    <div style={{
      padding: '34px 18px', textAlign: 'center', color: '#64748b',
      display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 10,
    }}>
      <div style={{
        width: 56, height: 56, borderRadius: 14, background: '#f1f5f9',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Users size={26} stroke="#cbd5e1" />
      </div>
      <div style={{ fontSize: 13, fontWeight: 600, color: '#0f172a' }}>
        Пока никого
      </div>
      <div style={{ fontSize: 12, color: '#64748b', lineHeight: 1.5, maxWidth: 240 }}>
        Зачислите студентов из базы, отправьте приглашения или откройте набор — и они появятся здесь.
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Ready hint — what happens on save
// ═══════════════════════════════════════════════════════════════════
function ReadyHint({ enrolled, invites, openRecruitment }) {
  return (
    <div style={{
      padding: 16, borderRadius: 12,
      background: 'rgba(16,185,129,0.04)', border: '1px solid rgba(16,185,129,0.2)',
    }}>
      <div style={{
        display: 'flex', alignItems: 'center', gap: 6, marginBottom: 10,
        fontSize: 11.5, fontWeight: 600, color: '#047857',
        letterSpacing: '0.05em', textTransform: 'uppercase',
      }}>
        <Icon.CircleCheck size={13} stroke="#047857" />
        После сохранения
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        <ReadyItem text={`Группа появится в списке активных`} icon="Users" done />
        <ReadyItem
          text={enrolled > 0
            ? `${enrolled} ${declensionEN(enrolled, ['студент получит','студента получат','студентов получат'])} расписание в кабинете`
            : 'Студентов пока нет — добавите позже'}
          icon="Calendar"
          done={enrolled > 0}
        />
        <ReadyItem
          text={invites > 0
            ? `${invites} ${declensionEN(invites, ['приглашение уйдёт','приглашения уйдут','приглашений уйдёт'])} на указанные email`
            : 'Приглашения не отправляются'}
          icon="Mail"
          done={invites > 0}
        />
        <ReadyItem
          text={openRecruitment
            ? 'Группа будет открыта для записи на сайте школы'
            : 'Группа останется закрытой'}
          icon="Megaphone"
          done={openRecruitment}
        />
      </div>
    </div>
  );
}
function ReadyItem({ text, icon, done }) {
  const Ic = Icon[icon];
  return (
    <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10, fontSize: 12.5 }}>
      <div style={{
        width: 18, height: 18, borderRadius: 9999, flexShrink: 0,
        background: done ? '#10b981' : '#e2e8f0',
        color: done ? '#fff' : '#94a3b8', marginTop: 1,
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      }}>
        {done ? <Icon.Check size={10} stroke="#fff" sw={3} /> : <Ic size={10} stroke="#94a3b8" />}
      </div>
      <div style={{ flex: 1, color: done ? '#0f172a' : '#64748b', lineHeight: 1.4 }}>
        {text}
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Progress indicator (matches steps 1 & 2)
// ═══════════════════════════════════════════════════════════════════
function ProgressIndicatorEN({ current, steps }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
      {steps.map((s, i) => {
        const done = current > s.id;
        const active = current === s.id;
        return (
          <React.Fragment key={s.id}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <div style={{
                width: 24, height: 24, borderRadius: 9999, flexShrink: 0,
                background: done ? '#4f46e5' : active ? 'rgba(79,70,229,0.12)' : '#f1f5f9',
                color: done ? '#fff' : active ? '#4338ca' : '#94a3b8',
                border: active ? '1.5px solid #4f46e5' : 'none',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                fontSize: 11.5, fontWeight: 700, fontVariantNumeric: 'tabular-nums',
              }}>
                {done ? <Icon.Check size={12} stroke="#fff" sw={3} /> : s.id}
              </div>
              <span style={{
                fontSize: 12.5, fontWeight: active ? 600 : 500,
                color: active ? '#0f172a' : done ? '#475569' : '#94a3b8',
              }}>{s.label}</span>
            </div>
            {i < steps.length - 1 && (
              <div style={{ width: 18, height: 1, background: '#e2e8f0' }} />
            )}
          </React.Fragment>
        );
      })}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════
// Finish bar — sticky bottom CTA
// ═══════════════════════════════════════════════════════════════════
function FinishBar({ enrolled, waitlist, invites, capacity, openRecruitment }) {
  const [saving, setSaving] = React.useState('idle'); // idle | saving | saved
  const total = enrolled + invites;

  const submit = () => {
    setSaving('saving');
    setTimeout(() => setSaving('saved'), 900);
  };

  return (
    <div style={{
      position: 'absolute', left: 0, right: 0, bottom: 0,
      background: '#fff', borderTop: '1px solid #e2e8f0',
      boxShadow: '0 -4px 12px rgba(15,23,42,0.06)',
      padding: '14px 32px',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 20,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, fontSize: 13 }}>
        <div style={{
          width: 32, height: 32, borderRadius: 9999, flexShrink: 0,
          background: 'rgba(16,185,129,0.12)', color: '#047857',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <Icon.CircleCheck size={16} stroke="#047857" />
        </div>
        <div>
          <div style={{ fontWeight: 600, color: '#0f172a' }}>
            Группа готова к запуску
          </div>
          <div style={{ fontSize: 12, color: '#64748b' }}>
            {enrolled} {declensionEN(enrolled, ['студент','студента','студентов'])} зачислен{enrolled === 1 ? '' : 'о'}
            {invites > 0 && <>, {invites} {declensionEN(invites, ['приглашение','приглашения','приглашений'])} в очереди</>}
            {waitlist > 0 && <>, {waitlist} в листе ожидания</>}
            {enrolled === 0 && invites === 0 && !openRecruitment &&
              ' — без студентов и закрытым набором'}
          </div>
        </div>
      </div>
      <div style={{ display: 'flex', gap: 10 }}>
        <a href="Group Schedule Setup.html"><Button variant="ghost">Назад</Button></a>
        <Button variant="secondary">Сохранить как черновик</Button>
        <Button onClick={submit} disabled={saving === 'saving'}>
          {saving === 'saving' ? (
            <>
              <span style={{
                display: 'inline-block', width: 14, height: 14,
                border: '2px solid rgba(255,255,255,0.35)', borderTopColor: '#fff',
                borderRadius: 9999, animation: 'spin 0.7s linear infinite',
              }} />
              Завершаем…
            </>
          ) : (
            <>
              <Icon.Check size={15} sw={2.5} />Завершить создание
            </>
          )}
        </Button>
      </div>
    </div>
  );
}

window.InvitePicker = InvitePicker;
window.RecruitmentPanel = RecruitmentPanel;
window.WelcomePanel = WelcomePanel;
window.RosterCard = RosterCard;
window.ReadyHint = ReadyHint;
window.ProgressIndicatorEN = ProgressIndicatorEN;
window.FinishBar = FinishBar;
