// ══════════════ Step 1 — Legal form ══════════════
function StepLegalForm({ data, errors, update }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 28 }}>
      <StepHeader
        eyebrow="Шаг 1 из 4"
        title="Какая у вас форма собственности?"
        subtitle="От этого зависит, какие документы и отчёты мы подготовим. Переключиться можно позже в настройках."
      />

      <F.Field
        label="Форма собственности"
        required
        error={errors.legalForm}
        hint="Выберите правовую форму вашей организации"
      >
        <F.CardRadio
          value={data.legalForm}
          onChange={v => {
            const lf = LEGAL_FORMS.find(x => x.value === v);
            update({ legalForm: v, isLegalEntity: lf?.entity ?? true });
          }}
          options={LEGAL_FORMS}
          columns={2}
        />
      </F.Field>

      {data.legalForm && (
        <InfoCallout
          variant={data.isLegalEntity ? 'primary' : 'neutral'}
          icon={data.isLegalEntity ? 'Building2' : 'UserPlus'}
          title={data.isLegalEntity
            ? 'Регистрируется как юридическое лицо'
            : 'Регистрируется как физическое лицо'}
          description={data.isLegalEntity
            ? 'Для документов потребуются реквизиты: УНП, расчётный счёт, юридический адрес. Их можно заполнить позже.'
            : 'Для ИП достаточно паспортных данных и УНП — реквизитный блок упростим.'}
        />
      )}
    </div>
  );
}

// ══════════════ Step 2 — About organization ══════════════
function StepAbout({ data, errors, update }) {
  const lf = LEGAL_FORMS.find(x => x.value === data.legalForm);
  const today = new Date().toISOString().slice(0, 10);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 22 }}>
      <StepHeader
        eyebrow="Шаг 2 из 4"
        title="Расскажите об организации"
        subtitle="Эти данные появятся в договорах и кабинете школы."
      />

      <F.Field
        label="Полное наименование"
        required
        error={errors.fullLegalName}
        hint={lf ? `Как в учредительных документах — начните с «${lf.tag}»` : 'Как в учредительных документах'}
      >
        <F.Text
          value={data.fullLegalName}
          onChange={e => update({ fullLegalName: e.target.value })}
          placeholder={lf ? `${lf.tag} «Название школы»` : 'Полное юридическое наименование'}
          error={errors.fullLegalName}
        />
      </F.Field>

      <F.Field
        label="Краткое название"
        optional
        hint="Используется в интерфейсе и письмах студентам. По умолчанию — первые слова из полного."
        error={errors.shortName}
      >
        <F.Text
          value={data.shortName}
          onChange={e => update({ shortName: e.target.value })}
          placeholder="Например: Школа «Эврика»"
          error={errors.shortName}
        />
      </F.Field>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
        <F.Field
          label="Дата регистрации"
          required
          error={errors.registrationDate}
          hint="Из свидетельства о государственной регистрации"
        >
          <F.Text
            type="date"
            max={today}
            value={data.registrationDate}
            onChange={e => update({ registrationDate: e.target.value })}
            error={errors.registrationDate}
            icon={<Icon.Calendar size={16} />}
          />
        </F.Field>

        <F.Field
          label="Тип организации"
          required
          error={errors.organizationType}
          hint="Категория по роду образовательной деятельности"
        >
          <F.Select
            value={data.organizationType}
            onChange={v => update({ organizationType: v })}
            options={ORG_TYPES.map(o => ({ value: o.value, label: o.label }))}
            placeholder="Выберите тип"
            error={errors.organizationType}
          />
        </F.Field>
      </div>
    </div>
  );
}

// ══════════════ Step 3 — Contact ══════════════
function StepContact({ data, errors, update }) {
  const ct = CONTACT_TYPES.find(c => c.value === data.primaryContactType);
  const IconC = ct ? Icon[ct.icon] : Icon.Mail;

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 22 }}>
      <StepHeader
        eyebrow="Шаг 3 из 4"
        title="Как с вами связаться?"
        subtitle="Основной контакт — канал для уведомлений о платежах, новых студентах и системных сообщениях."
      />

      <F.Field
        label="Канал связи"
        required
        error={errors.primaryContactType}
      >
        <F.Segmented
          value={data.primaryContactType}
          onChange={v => update({ primaryContactType: v })}
          options={CONTACT_TYPES.map(c => {
            const IC = Icon[c.icon];
            return { value: c.value, label: c.short, icon: <IC size={14} /> };
          })}
        />
      </F.Field>

      <F.Field
        label={ct ? ct.label : 'Контакт'}
        required
        error={errors.primaryContactValue}
        hint={ct?.hint}
      >
        <F.Text
          value={data.primaryContactValue}
          onChange={e => update({ primaryContactValue: e.target.value })}
          placeholder={ct?.placeholder || ''}
          error={errors.primaryContactValue}
          icon={<IconC size={16} />}
          type={ct?.value === 'Email' ? 'email' : 'text'}
        />
      </F.Field>

      <F.Field
        label="Комментарий"
        optional
        hint="Краткое описание — кому и когда звонить/писать. Видно только сотрудникам школы."
      >
        <F.Textarea
          value={data.primaryContactDescription}
          onChange={e => update({ primaryContactDescription: e.target.value })}
          placeholder="Например: основной рабочий email директора, проверяется с 9:00 до 18:00 в будни"
          maxLength={500}
        />
        <div style={{
          marginTop: 4, fontSize: 11, color: '#94a3b8', textAlign: 'right',
          fontVariantNumeric: 'tabular-nums',
        }}>
          {(data.primaryContactDescription || '').length} / 500
        </div>
      </F.Field>
    </div>
  );
}

// ══════════════ Step 4 — Review ══════════════
function StepReview({ data, goTo }) {
  const lf = LEGAL_FORMS.find(x => x.value === data.legalForm);
  const ot = ORG_TYPES.find(x => x.value === data.organizationType);
  const ct = CONTACT_TYPES.find(x => x.value === data.primaryContactType);
  const IconC = ct ? Icon[ct.icon] : Icon.Mail;

  const fmtDate = (iso) => {
    if (!iso) return '—';
    try {
      const d = new Date(iso);
      return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
    } catch { return iso; }
  };

  const section = (title, stepIdx, children) => (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      padding: '18px 20px',
    }}>
      <div style={{
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        marginBottom: 14, paddingBottom: 12, borderBottom: '1px solid #f1f5f9',
      }}>
        <h3 style={{ margin: 0, fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{title}</h3>
        <button
          type="button"
          onClick={() => goTo(stepIdx)}
          style={{
            background: 'transparent', border: 'none', color: '#4f46e5',
            fontSize: 12.5, fontWeight: 500, cursor: 'pointer', fontFamily: 'inherit',
            padding: '2px 6px', borderRadius: 6,
          }}
          onMouseEnter={e => e.currentTarget.style.background = 'rgba(79,70,229,0.08)'}
          onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
        >
          Изменить
        </button>
      </div>
      {children}
    </div>
  );

  const row = (label, value) => (
    <div style={{
      display: 'grid', gridTemplateColumns: '180px 1fr', gap: 16,
      padding: '7px 0', fontSize: 13.5,
    }}>
      <div style={{ color: '#64748b' }}>{label}</div>
      <div style={{ color: value ? '#0f172a' : '#cbd5e1', fontWeight: 500 }}>{value || '—'}</div>
    </div>
  );

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
      <StepHeader
        eyebrow="Шаг 4 из 4"
        title="Проверьте данные"
        subtitle="Проверьте введённую информацию. После подтверждения школа появится в вашем кабинете."
      />

      {section('Форма собственности', 0, (
        <>
          {row('Форма', lf ? `${lf.tag} — ${lf.label}` : '—')}
          {row('Регистрируется как', data.isLegalEntity ? 'Юридическое лицо' : 'Физическое лицо')}
        </>
      ))}

      {section('Об организации', 1, (
        <>
          {row('Полное наименование', data.fullLegalName)}
          {row('Краткое название', data.shortName || <span style={{ color: '#94a3b8', fontWeight: 400 }}>не указано</span>)}
          {row('Дата регистрации', fmtDate(data.registrationDate))}
          {row('Тип организации', ot?.label)}
        </>
      ))}

      {section('Основной контакт', 2, (
        <>
          {row('Канал', ct ? (
            <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
              <IconC size={14} stroke="#4f46e5" />{ct.label}
            </span>
          ) : '—')}
          {row('Значение', data.primaryContactValue)}
          {row('Комментарий', data.primaryContactDescription || <span style={{ color: '#94a3b8', fontWeight: 400 }}>не указан</span>)}
        </>
      ))}

      <InfoCallout
        variant="success"
        icon="Shield"
        title="Данные защищены"
        description="Информация передаётся по защищённому каналу и хранится в соответствии с требованиями 152-ФЗ о персональных данных."
      />
    </div>
  );
}

// ══════════════ Step 5 — Done ══════════════
function StepDone({ data }) {
  return (
    <div style={{
      display: 'flex', flexDirection: 'column', alignItems: 'center',
      gap: 18, padding: '40px 20px', textAlign: 'center',
    }}>
      <div style={{
        width: 72, height: 72, borderRadius: 9999,
        background: 'rgba(16,185,129,0.12)',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <div style={{
          width: 52, height: 52, borderRadius: 9999, background: '#10b981',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          boxShadow: '0 8px 24px rgba(16,185,129,0.35)',
        }}>
          <Icon.Check size={28} stroke="#fff" sw={3} />
        </div>
      </div>
      <h2 style={{
        margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em',
      }}>Организация зарегистрирована</h2>
      <p style={{
        margin: 0, fontSize: 14.5, color: '#64748b', maxWidth: 420, lineHeight: 1.55,
      }}>
        <strong style={{ color: '#0f172a', fontWeight: 600 }}>{data.shortName || data.fullLegalName}</strong> появилась
        в вашем кабинете. Теперь добавьте первый курс и пригласите преподавателей.
      </p>
      <div style={{ display: 'flex', gap: 10, marginTop: 8 }}>
        <Button variant="secondary">Добавить курс</Button>
        <Button>Перейти в кабинет<Icon.ArrowRight size={16} /></Button>
      </div>
    </div>
  );
}

// ── Helpers ─────────────────────────────────────────────────────────
function StepHeader({ eyebrow, title, subtitle }) {
  return (
    <div>
      <div style={{
        fontSize: 11, fontWeight: 600, letterSpacing: '0.1em',
        textTransform: 'uppercase', color: '#4f46e5', marginBottom: 10,
      }}>{eyebrow}</div>
      <h2 style={{
        margin: 0, fontSize: 26, fontWeight: 700, letterSpacing: '-0.02em',
        color: '#0f172a', lineHeight: 1.2,
      }}>{title}</h2>
      {subtitle && (
        <p style={{
          margin: '10px 0 0 0', fontSize: 14.5, color: '#64748b',
          lineHeight: 1.55, maxWidth: 560,
        }}>{subtitle}</p>
      )}
    </div>
  );
}

function InfoCallout({ variant = 'primary', icon = 'Info', title, description }) {
  const styles = {
    primary: { bg: 'rgba(79,70,229,0.05)', bd: 'rgba(79,70,229,0.2)', ic: '#4f46e5', ico_bg: 'rgba(79,70,229,0.1)' },
    success: { bg: 'rgba(16,185,129,0.05)', bd: 'rgba(16,185,129,0.2)', ic: '#047857', ico_bg: 'rgba(16,185,129,0.1)' },
    neutral: { bg: '#f8fafc', bd: '#e2e8f0', ic: '#475569', ico_bg: '#f1f5f9' },
  }[variant];
  const IconC = Icon[icon] || Icon.Info;
  return (
    <div style={{
      display: 'flex', gap: 12, padding: '14px 16px',
      background: styles.bg, border: `1px solid ${styles.bd}`, borderRadius: 12,
    }}>
      <div style={{
        width: 32, height: 32, borderRadius: 8, flexShrink: 0,
        background: styles.ico_bg, color: styles.ic,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <IconC size={16} stroke={styles.ic} />
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontSize: 13.5, fontWeight: 600, color: '#0f172a' }}>{title}</div>
        {description && (
          <div style={{ fontSize: 13, color: '#475569', marginTop: 2, lineHeight: 1.5 }}>{description}</div>
        )}
      </div>
    </div>
  );
}

Object.assign(window, { StepLegalForm, StepAbout, StepContact, StepReview, StepDone });
