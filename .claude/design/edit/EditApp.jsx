// Edit Organization page
const { useState: useStateE, useMemo: useMemoE, useEffect: useEffectE } = React;

// ── Existing (prefilled) organization data ─────────────────────────────
const CURRENT_ORG = {
  legalForm: 'Llc',
  isLegalEntity: true,
  fullLegalName: 'ООО «Образовательный центр Эврика»',
  shortName: 'Школа «Эврика»',
  registrationDate: '2019-03-14',
  organizationType: 'PrivateEducationalCenter',
  primaryContactType: 'Email',
  primaryContactValue: 'director@eureka-school.ru',
  primaryContactDescription: 'Основной рабочий email директора, проверяется с 9:00 до 18:00 в будни',
};

// ── Validation (same rules as wizard) ─────────────────────────────────
function validateAll(data) {
  const e = {};
  if (!data.legalForm) e.legalForm = 'Выберите форму собственности';
  if (!data.fullLegalName.trim()) e.fullLegalName = 'Укажите полное наименование';
  else if (data.fullLegalName.trim().length < 3) e.fullLegalName = 'Минимум 3 символа';
  if (!data.registrationDate) e.registrationDate = 'Укажите дату регистрации';
  else if (new Date(data.registrationDate) > new Date()) e.registrationDate = 'Дата не может быть в будущем';
  if (!data.organizationType) e.organizationType = 'Выберите тип организации';
  if (!data.primaryContactType) e.primaryContactType = 'Выберите канал';
  if (!data.primaryContactValue.trim()) e.primaryContactValue = 'Укажите контакт';
  else {
    if (data.primaryContactType === 'Email' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.primaryContactValue))
      e.primaryContactValue = 'Введите корректный email';
    if (['MobilePhone', 'WhatsApp', 'Viber'].includes(data.primaryContactType)
      && !/^[+\d\s()\-]{10,}$/.test(data.primaryContactValue))
      e.primaryContactValue = 'Введите номер в международном формате';
  }
  return e;
}

// ── Section card ──────────────────────────────────────────────────────
function Section({ icon, title, subtitle, children, rightSlot }) {
  const IC = Icon[icon];
  return (
    <section style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
      overflow: 'hidden',
    }}>
      <header style={{
        padding: '18px 24px', borderBottom: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', gap: 14,
      }}>
        <div style={{
          width: 36, height: 36, borderRadius: 10, flexShrink: 0,
          background: 'rgba(79,70,229,0.08)', color: '#4f46e5',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <IC size={18} stroke="#4f46e5" />
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <h2 style={{ margin: 0, fontSize: 15, fontWeight: 600, color: '#0f172a', letterSpacing: '-0.01em' }}>{title}</h2>
          {subtitle && <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>{subtitle}</div>}
        </div>
        {rightSlot}
      </header>
      <div style={{ padding: '22px 24px' }}>{children}</div>
    </section>
  );
}

// ── "Changed" indicator ──────────────────────────────────────────────
function ChangedBadge() {
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 4,
      padding: '2px 8px', borderRadius: 9999, fontSize: 11, fontWeight: 500,
      background: 'rgba(245,158,11,0.12)', color: '#92400e',
    }}>
      <span style={{ width: 5, height: 5, borderRadius: 9999, background: '#f59e0b' }} />
      изменено
    </span>
  );
}

// ── Main Edit app ────────────────────────────────────────────────────
function EditOrgApp() {
  const [data, setData] = useStateE(CURRENT_ORG);
  const [submitAttempted, setSubmitAttempted] = useStateE(false);
  const [savingState, setSavingState] = useStateE('idle'); // idle | saving | saved
  const [confirmOpen, setConfirmOpen] = useStateE(false);
  const [resetOpen, setResetOpen] = useStateE(false);

  const errors = submitAttempted ? validateAll(data) : {};
  const errorCount = Object.keys(validateAll(data)).length;

  const changedFields = useMemoE(() => {
    const diff = {};
    Object.keys(CURRENT_ORG).forEach(k => {
      if (data[k] !== CURRENT_ORG[k]) diff[k] = true;
    });
    return diff;
  }, [data]);
  const hasChanges = Object.keys(changedFields).length > 0;
  const legalFormChanged = !!changedFields.legalForm;

  const update = (patch) => {
    setData(d => ({ ...d, ...patch }));
    if (savingState === 'saved') setSavingState('idle');
  };

  // Warn on page leave with unsaved changes
  useEffectE(() => {
    const h = (e) => { if (hasChanges) { e.preventDefault(); e.returnValue = ''; } };
    window.addEventListener('beforeunload', h);
    return () => window.removeEventListener('beforeunload', h);
  }, [hasChanges]);

  const doSave = () => {
    setSavingState('saving');
    setTimeout(() => {
      setSavingState('saved');
      // "persist" — baseline becomes current so changes clear
      Object.assign(CURRENT_ORG, data);
      setData({ ...CURRENT_ORG });
    }, 900);
  };

  const onSaveClick = () => {
    setSubmitAttempted(true);
    if (Object.keys(validateAll(data)).length > 0) return;
    if (legalFormChanged) setConfirmOpen(true);
    else doSave();
  };

  const onResetConfirm = () => {
    setData({ ...CURRENT_ORG });
    setSubmitAttempted(false);
    setSavingState('idle');
    setResetOpen(false);
  };

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="org" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Организация</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span>{CURRENT_ORG.shortName}</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Редактирование</span>
        </div>

        {/* Page header */}
        <EditHeader
          data={data}
          hasChanges={hasChanges}
          changedCount={Object.keys(changedFields).length}
          savingState={savingState}
        />

        {/* Scrollable content */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 120px' }}>
          <div style={{ maxWidth: 880, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 20 }}>

            <Section
              icon="Briefcase"
              title="Правовая форма"
              subtitle="Влияет на реквизиты, отчёты и шаблоны договоров"
              rightSlot={changedFields.legalForm && <ChangedBadge />}
            >
              <F.Field
                label="Форма собственности"
                required
                error={errors.legalForm}
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

              <div style={{
                marginTop: 16, padding: '12px 14px', borderRadius: 10,
                background: '#f8fafc', border: '1px solid #e2e8f0',
                display: 'flex', alignItems: 'center', gap: 10, fontSize: 13,
              }}>
                <Icon.Info size={15} stroke="#64748b" />
                <span style={{ color: '#475569' }}>
                  Статус:{' '}
                  <strong style={{ color: '#0f172a' }}>
                    {data.isLegalEntity ? 'Юридическое лицо' : 'Физическое лицо'}
                  </strong>
                  {' '}· определяется автоматически по выбранной форме
                </span>
              </div>

              {legalFormChanged && (
                <div style={{
                  marginTop: 12, padding: '12px 14px', borderRadius: 10,
                  background: 'rgba(245,158,11,0.08)', border: '1px solid rgba(245,158,11,0.25)',
                  display: 'flex', gap: 10, alignItems: 'flex-start',
                }}>
                  <Icon.AlertCircle size={16} stroke="#92400e" style={{ flexShrink: 0, marginTop: 1 }} />
                  <div style={{ fontSize: 12.5, color: '#78350f', lineHeight: 1.5 }}>
                    Смена формы собственности затронет формирование договоров и отчётов.
                    Потребуется дополнительное подтверждение перед сохранением.
                  </div>
                </div>
              )}
            </Section>

            <Section
              icon="FileText"
              title="Основные сведения"
              subtitle="Данные, которые появятся в документах и интерфейсе"
            >
              <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
                <F.Field
                  label="Полное наименование"
                  required
                  error={errors.fullLegalName}
                  hint="Как в учредительных документах"
                >
                  <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                    <div style={{ flex: 1 }}>
                      <F.Text
                        value={data.fullLegalName}
                        onChange={e => update({ fullLegalName: e.target.value })}
                        error={errors.fullLegalName}
                      />
                    </div>
                    {changedFields.fullLegalName && <ChangedBadge />}
                  </div>
                </F.Field>

                <F.Field
                  label="Краткое название"
                  optional
                  hint="Используется в интерфейсе и письмах"
                >
                  <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                    <div style={{ flex: 1 }}>
                      <F.Text
                        value={data.shortName}
                        onChange={e => update({ shortName: e.target.value })}
                      />
                    </div>
                    {changedFields.shortName && <ChangedBadge />}
                  </div>
                </F.Field>

                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
                  <F.Field
                    label="Дата регистрации"
                    required
                    error={errors.registrationDate}
                    hint="Из свидетельства о регистрации"
                  >
                    <F.Text
                      type="date"
                      max={new Date().toISOString().slice(0, 10)}
                      value={data.registrationDate}
                      onChange={e => update({ registrationDate: e.target.value })}
                      error={errors.registrationDate}
                      icon={<Icon.Calendar size={16} />}
                    />
                    {changedFields.registrationDate && <div style={{ marginTop: 6 }}><ChangedBadge /></div>}
                  </F.Field>

                  <F.Field
                    label="Тип организации"
                    required
                    error={errors.organizationType}
                    hint="Категория по роду деятельности"
                  >
                    <F.Select
                      value={data.organizationType}
                      onChange={v => update({ organizationType: v })}
                      options={ORG_TYPES.map(o => ({ value: o.value, label: o.label }))}
                      placeholder="Выберите тип"
                      error={errors.organizationType}
                    />
                    {changedFields.organizationType && <div style={{ marginTop: 6 }}><ChangedBadge /></div>}
                  </F.Field>
                </div>
              </div>
            </Section>

            <Section
              icon="Mail"
              title="Основной контакт"
              subtitle="Канал связи для уведомлений и системных сообщений"
              rightSlot={(changedFields.primaryContactType || changedFields.primaryContactValue || changedFields.primaryContactDescription) && <ChangedBadge />}
            >
              <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
                <F.Field label="Канал связи" required error={errors.primaryContactType}>
                  <F.Segmented
                    value={data.primaryContactType}
                    onChange={v => update({ primaryContactType: v })}
                    options={CONTACT_TYPES.map(c => {
                      const IC = Icon[c.icon];
                      return { value: c.value, label: c.short, icon: <IC size={14} /> };
                    })}
                  />
                </F.Field>

                {(() => {
                  const ct = CONTACT_TYPES.find(c => c.value === data.primaryContactType);
                  const IC = ct ? Icon[ct.icon] : Icon.Mail;
                  return (
                    <F.Field
                      label={ct ? ct.label : 'Контакт'}
                      required
                      error={errors.primaryContactValue}
                      hint={ct?.hint}
                    >
                      <F.Text
                        value={data.primaryContactValue}
                        onChange={e => update({ primaryContactValue: e.target.value })}
                        placeholder={ct?.placeholder}
                        error={errors.primaryContactValue}
                        icon={<IC size={16} />}
                        type={ct?.value === 'Email' ? 'email' : 'text'}
                      />
                    </F.Field>
                  );
                })()}

                <F.Field
                  label="Комментарий"
                  optional
                  hint="Кому и когда писать/звонить. Видно только сотрудникам."
                >
                  <F.Textarea
                    value={data.primaryContactDescription}
                    onChange={e => update({ primaryContactDescription: e.target.value })}
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
            </Section>

            <DangerZone />
          </div>
        </div>

        {/* Sticky save bar */}
        <SaveBar
          hasChanges={hasChanges}
          changedCount={Object.keys(changedFields).length}
          errorCount={submitAttempted ? errorCount : 0}
          savingState={savingState}
          onSave={onSaveClick}
          onReset={() => setResetOpen(true)}
        />
      </div>

      {/* Confirmation modal for LegalForm change */}
      {confirmOpen && (
        <Modal
          icon="AlertCircle"
          iconVariant="warning"
          title="Сменить форму собственности?"
          description={
            <>
              Вы меняете правовую форму на <strong>«{LEGAL_FORMS.find(l => l.value === data.legalForm)?.tag}»</strong>.
              Это затронет шаблоны договоров и формат отчётов. Существующие документы останутся в архиве, но новые будут формироваться по новой форме.
            </>
          }
          confirmLabel="Да, сменить"
          confirmVariant="primary"
          onCancel={() => setConfirmOpen(false)}
          onConfirm={() => { setConfirmOpen(false); doSave(); }}
        />
      )}

      {resetOpen && (
        <Modal
          icon="AlertCircle"
          iconVariant="danger"
          title="Отменить изменения?"
          description="Все несохранённые изменения будут потеряны. Это действие нельзя отменить."
          confirmLabel="Да, отменить"
          confirmVariant="destructive"
          onCancel={() => setResetOpen(false)}
          onConfirm={onResetConfirm}
        />
      )}
    </div>
  );
}

// ── Page header with org identity ────────────────────────────────────
function EditHeader({ data, hasChanges, changedCount, savingState }) {
  const lf = LEGAL_FORMS.find(x => x.value === data.legalForm);
  const fmtSaved = new Date(2026, 3, 18, 14, 32).toLocaleString('ru-RU', {
    day: 'numeric', month: 'long', hour: '2-digit', minute: '2-digit',
  });
  return (
    <div style={{
      padding: '24px 32px', borderBottom: '1px solid #e2e8f0',
      background: '#fff', display: 'flex', alignItems: 'center', gap: 20,
    }}>
      <div style={{
        width: 56, height: 56, borderRadius: 14, flexShrink: 0,
        background: 'linear-gradient(135deg, #6366f1, #8b5cf6)',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        color: '#fff', fontSize: 20, fontWeight: 700,
        boxShadow: '0 4px 12px rgba(99,102,241,0.3)',
      }}>
        {(data.shortName || data.fullLegalName).trim().charAt(0)}
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 2 }}>
          <h1 style={{ margin: 0, fontSize: 22, fontWeight: 700, letterSpacing: '-0.02em' }}>
            Редактирование организации
          </h1>
          {lf && (
            <Badge variant="primary" style={{ fontSize: 11 }}>{lf.tag}</Badge>
          )}
        </div>
        <div style={{ fontSize: 13, color: '#64748b' }}>
          {hasChanges ? (
            <span style={{ color: '#b45309' }}>
              <strong style={{ fontWeight: 600 }}>{changedCount}</strong>{' '}
              {declensionE(changedCount, ['несохранённое изменение', 'несохранённых изменения', 'несохранённых изменений'])}
            </span>
          ) : savingState === 'saved' ? (
            <span style={{ color: '#047857', display: 'inline-flex', alignItems: 'center', gap: 6 }}>
              <Icon.CircleCheck size={14} stroke="#047857" />
              Сохранено только что
            </span>
          ) : (
            <span>Последнее изменение: {fmtSaved}</span>
          )}
        </div>
      </div>
      <Button variant="secondary">
        <Icon.FileText size={15} />История изменений
      </Button>
    </div>
  );
}

// ── Danger zone (archive / delete) ───────────────────────────────────
function DangerZone() {
  return (
    <section style={{
      background: '#fff', border: '1px solid #fecaca', borderRadius: 16,
      overflow: 'hidden',
    }}>
      <header style={{
        padding: '14px 24px', borderBottom: '1px solid #fee2e2',
        background: 'rgba(239,68,68,0.03)',
        display: 'flex', alignItems: 'center', gap: 12,
      }}>
        <Icon.AlertCircle size={16} stroke="#b91c1c" />
        <h2 style={{ margin: 0, fontSize: 14, fontWeight: 600, color: '#991b1b' }}>
          Опасная зона
        </h2>
      </header>
      <div style={{ padding: '18px 24px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ fontSize: 13.5, fontWeight: 500, color: '#0f172a' }}>
              Архивировать организацию
            </div>
            <div style={{ fontSize: 12.5, color: '#64748b', marginTop: 2 }}>
              Скрыть организацию из списка. Данные сохранятся, восстановить можно в течение 90 дней.
            </div>
          </div>
          <Button variant="secondary" style={{ color: '#b91c1c', borderColor: '#fecaca' }}>
            Архивировать
          </Button>
        </div>
      </div>
    </section>
  );
}

// ── Sticky save bar ──────────────────────────────────────────────────
function SaveBar({ hasChanges, changedCount, errorCount, savingState, onSave, onReset }) {
  const visible = hasChanges || savingState === 'saving';
  return (
    <div style={{
      position: 'absolute', left: 240, right: 0, bottom: 0,
      transform: visible ? 'translateY(0)' : 'translateY(100%)',
      transition: 'transform .25s cubic-bezier(.4,0,.2,1)',
      background: '#fff', borderTop: '1px solid #e2e8f0',
      boxShadow: '0 -4px 12px rgba(15,23,42,0.06)',
      padding: '14px 32px',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 20,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, fontSize: 13 }}>
        <div style={{
          width: 32, height: 32, borderRadius: 9999, flexShrink: 0,
          background: errorCount > 0 ? 'rgba(239,68,68,0.12)' : 'rgba(245,158,11,0.12)',
          color: errorCount > 0 ? '#b91c1c' : '#92400e',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          {errorCount > 0
            ? <Icon.AlertCircle size={16} stroke="#b91c1c" />
            : <div style={{ width: 8, height: 8, borderRadius: 9999, background: '#f59e0b' }} />}
        </div>
        <div>
          {errorCount > 0 ? (
            <>
              <div style={{ fontWeight: 600, color: '#991b1b' }}>
                {errorCount} {declensionE(errorCount, ['ошибка', 'ошибки', 'ошибок'])} в форме
              </div>
              <div style={{ fontSize: 12, color: '#64748b' }}>Исправьте поля, отмеченные красным</div>
            </>
          ) : (
            <>
              <div style={{ fontWeight: 600, color: '#0f172a' }}>
                {changedCount} {declensionE(changedCount, ['несохранённое изменение', 'несохранённых изменения', 'несохранённых изменений'])}
              </div>
              <div style={{ fontSize: 12, color: '#64748b' }}>Сохраните, чтобы применить</div>
            </>
          )}
        </div>
      </div>
      <div style={{ display: 'flex', gap: 10 }}>
        <Button variant="ghost" onClick={onReset} disabled={savingState === 'saving'}>
          Отменить изменения
        </Button>
        <Button
          onClick={onSave}
          disabled={savingState === 'saving' || errorCount > 0}
          style={savingState === 'saving' ? { opacity: 0.7, cursor: 'wait' } : {}}
        >
          {savingState === 'saving' ? (
            <>
              <Spinner />Сохранение…
            </>
          ) : (
            <>
              <Icon.Check size={16} sw={2.5} />Сохранить изменения
            </>
          )}
        </Button>
      </div>
    </div>
  );
}

function Spinner() {
  return (
    <span style={{
      display: 'inline-block', width: 14, height: 14,
      border: '2px solid rgba(255,255,255,0.35)', borderTopColor: '#fff',
      borderRadius: 9999, animation: 'spin 0.7s linear infinite',
    }} />
  );
}

// ── Modal ────────────────────────────────────────────────────────────
function Modal({ icon = 'AlertCircle', iconVariant = 'warning', title, description, confirmLabel, confirmVariant = 'primary', onCancel, onConfirm }) {
  const IC = Icon[icon];
  const iconColors = {
    warning: { bg: 'rgba(245,158,11,0.12)', fg: '#b45309' },
    danger: { bg: 'rgba(239,68,68,0.12)', fg: '#b91c1c' },
    primary: { bg: 'rgba(79,70,229,0.12)', fg: '#4338ca' },
  }[iconVariant];
  return (
    <div style={{
      position: 'fixed', inset: 0, zIndex: 100,
      background: 'rgba(15,23,42,0.45)', backdropFilter: 'blur(3px)',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      padding: 20, animation: 'fadeIn .15s ease-out',
    }} onClick={onCancel}>
      <div
        onClick={e => e.stopPropagation()}
        style={{
          background: '#fff', borderRadius: 16, maxWidth: 440, width: '100%',
          boxShadow: '0 25px 50px -12px rgba(0,0,0,0.25)',
          animation: 'scaleIn .15s ease-out', overflow: 'hidden',
        }}
      >
        <div style={{ padding: '24px 24px 20px' }}>
          <div style={{
            width: 44, height: 44, borderRadius: 12, marginBottom: 14,
            background: iconColors.bg, color: iconColors.fg,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
          }}>
            <IC size={22} stroke={iconColors.fg} />
          </div>
          <h3 style={{ margin: 0, fontSize: 18, fontWeight: 700, letterSpacing: '-0.01em' }}>{title}</h3>
          <p style={{ margin: '8px 0 0', fontSize: 13.5, color: '#475569', lineHeight: 1.55 }}>
            {description}
          </p>
        </div>
        <div style={{
          padding: '14px 24px', background: '#f8fafc', borderTop: '1px solid #f1f5f9',
          display: 'flex', justifyContent: 'flex-end', gap: 10,
        }}>
          <Button variant="secondary" onClick={onCancel}>Отмена</Button>
          <Button variant={confirmVariant} onClick={onConfirm}>{confirmLabel}</Button>
        </div>
      </div>
    </div>
  );
}

function declensionE(n, forms) {
  const abs = Math.abs(n);
  const mod10 = abs % 10, mod100 = abs % 100;
  if (mod10 === 1 && mod100 !== 11) return forms[0];
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return forms[1];
  return forms[2];
}

window.EditOrgApp = EditOrgApp;
