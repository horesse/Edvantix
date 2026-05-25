// Group Create — basic fields. Schedule + students come later in dedicated steps.
const { useState: useStateGC, useMemo: useMemoGC, useEffect: useEffectGC } = React;

// Source data — mirror enums for select options
const COURSES = [
  { value: 'general',  label: 'General English' },
  { value: 'business', label: 'Business English' },
  { value: 'kids',     label: 'Программа Kids' },
  { value: 'club',     label: 'Разговорный клуб' },
  { value: 'ege',      label: 'Подготовка к ЕГЭ' },
  { value: 'ielts',    label: 'IELTS' },
  { value: 'oge',      label: 'Подготовка к ОГЭ' },
];

const TEACHERS = [
  { value: 't_petrov',     label: 'Петров А. Н.',     role: 'Преподаватель' },
  { value: 't_kovalenko',  label: 'Коваленко Н. И.',  role: 'Преподаватель' },
  { value: 't_zakharova',  label: 'Захарова М. А.',   role: 'Методист' },
  { value: 't_belov',      label: 'Белов С. А.',      role: 'Куратор' },
  { value: 't_vasilyeva',  label: 'Васильева А. П.',  role: 'Куратор' },
  { value: 't_smirnova',   label: 'Смирнова Д. К.',   role: 'Преподаватель' },
  { value: 't_fedorov',    label: 'Фёдоров М. В.',    role: 'Преподаватель' },
  { value: 't_kuznetsov',  label: 'Кузнецов Р. Ю.',   role: 'Методист' },
  { value: 't_lebedev',    label: 'Лебедев П. О.',    role: 'Куратор' },
  { value: 't_gromov',     label: 'Громов Е. Д.',     role: 'Преподаватель' },
  { value: 't_sidorova',   label: 'Сидорова Ю. А.',   role: 'Методист' },
  { value: 't_morozova',   label: 'Морозова В. Д.',   role: 'Преподаватель' },
];

const ROOMS = [
  { value: 'r101', label: 'Каб. 101', floor: 1, seats: 8  },
  { value: 'r102', label: 'Каб. 102', floor: 1, seats: 10 },
  { value: 'r103', label: 'Каб. 103', floor: 1, seats: 10 },
  { value: 'r105', label: 'Каб. 105', floor: 1, seats: 10 },
  { value: 'r108', label: 'Каб. 108', floor: 1, seats: 10 },
  { value: 'r109', label: 'Каб. 109', floor: 1, seats: 10 },
  { value: 'r204', label: 'Каб. 204', floor: 2, seats: 12 },
  { value: 'r205', label: 'Каб. 205', floor: 2, seats: 12 },
  { value: 'r206', label: 'Каб. 206', floor: 2, seats: 12 },
  { value: 'r207', label: 'Каб. 207', floor: 2, seats: 12 },
  { value: 'r301', label: 'Каб. 301', floor: 3, seats: 8  },
  { value: 'r304', label: 'Каб. 304', floor: 3, seats: 8  },
  { value: 'r305', label: 'Каб. 305', floor: 3, seats: 8  },
];
function roomLabel(r) { return `${r.label} (${r.floor} эт., ${r.seats} мест)`; }

const LEVEL_OPTIONS = [
  { value: 'A1', tag: 'A1', label: 'Начальный — c нуля' },
  { value: 'A2', tag: 'A2', label: 'Базовый — Elementary' },
  { value: 'B1', tag: 'B1', label: 'Средний — Intermediate' },
  { value: 'B2', tag: 'B2', label: 'Продвинутый — Upper-Inter.' },
  { value: 'C1', tag: 'C1', label: 'Высокий — Advanced' },
  { value: 'JR', tag: 'JR', label: 'Дети 7–10 лет' },
  { value: 'TN', tag: 'TN', label: 'Подростки 11–14 лет' },
  { value: 'PR', tag: 'PR', label: 'Подготовка к экзаменам' },
];

const FORMAT_OPTIONS = [
  { value: 'offline', label: 'Очно',     icon: 'School' },
  { value: 'online',  label: 'Онлайн',   icon: 'MessageCircle' },
  { value: 'mixed',   label: 'Смешанный',icon: 'Users' },
];

// ── Code generator ───────────────────────────────────────────────────
function suggestCode(level, course, existing = []) {
  if (!level) return '';
  const c = COURSES.find(x => x.value === course);
  const courseTag = c ? c.value.toUpperCase().slice(0, 4) : 'GRP';
  // Find next free 2-digit suffix
  const prefix = `EN-${level}-`;
  let n = 1;
  const taken = new Set(existing.filter(s => s.startsWith(prefix))
    .map(s => parseInt(s.slice(prefix.length), 10)).filter(Number.isFinite));
  while (taken.has(n)) n++;
  return `${prefix}${String(n).padStart(2, '0')}`;
}

// ── Validation ───────────────────────────────────────────────────────
function validateGroup(d) {
  const e = {};
  if (!d.name.trim()) e.name = 'Укажите название группы';
  else if (d.name.trim().length < 3) e.name = 'Минимум 3 символа';
  if (!d.code.trim()) e.code = 'Укажите код';
  else if (!/^[A-Z0-9\-]+$/.test(d.code)) e.code = 'Только латиница, цифры и дефис';
  if (!d.level) e.level = 'Выберите уровень';
  if (!d.course) e.course = 'Выберите курс';
  if (!d.teacher) e.teacher = 'Назначьте преподавателя';
  if (!d.format) e.format = 'Выберите формат';
  if (d.format === 'offline' && !d.room) e.room = 'Укажите кабинет';
  if (!d.capacity || d.capacity < 1) e.capacity = 'Минимум 1 место';
  else if (d.capacity > 50) e.capacity = 'Максимум 50';
  if (!d.starts) e.starts = 'Укажите дату начала';
  if (!d.ends)   e.ends = 'Укажите дату окончания';
  if (d.starts && d.ends && d.ends < d.starts) e.ends = 'Окончание раньше начала';
  return e;
}

const EMPTY_GROUP = {
  name: '',
  code: '',
  level: '',
  course: '',
  teacher: '',
  format: 'offline',
  room: '',
  capacity: 10,
  starts: '',
  ends: '',
  description: '',
};

// ── Section card ─────────────────────────────────────────────────────
function GCSection({ icon, title, subtitle, children, step }) {
  const IC = Icon[icon];
  return (
    <section style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
      overflow: 'hidden',
    }}>
      <header style={{
        padding: '16px 24px', borderBottom: '1px solid #f1f5f9',
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
        {step && (
          <span style={{
            fontFamily: 'var(--edv-font-mono)', fontSize: 11, color: '#94a3b8',
            padding: '3px 8px', borderRadius: 9999, background: '#f1f5f9',
          }}>Шаг {step}</span>
        )}
      </header>
      <div style={{ padding: '22px 24px' }}>{children}</div>
    </section>
  );
}

// ── Main app ─────────────────────────────────────────────────────────
function GroupCreateApp() {
  const [data, setData] = useStateGC(EMPTY_GROUP);
  const [submitAttempted, setSubmitAttempted] = useStateGC(false);
  const [savingState, setSavingState] = useStateGC('idle'); // idle | saving | saved
  const [codeAuto, setCodeAuto] = useStateGC(true);
  const [createdId, setCreatedId] = useStateGC(null);

  const errors = submitAttempted ? validateGroup(data) : {};
  const errorCount = Object.keys(validateGroup(data)).length;

  const update = (patch) => setData(d => ({ ...d, ...patch }));

  // Auto-generate code when level / course changes (only if user hasn't manually edited)
  useEffectGC(() => {
    if (!codeAuto || !data.level) return;
    const taken = (window.GROUPS || []).map(g => g.code);
    update({ code: suggestCode(data.level, data.course, taken) });
  }, [data.level, data.course, codeAuto]);

  const onCodeChange = (val) => {
    setCodeAuto(false);
    update({ code: val.toUpperCase() });
  };

  const onCreate = () => {
    setSubmitAttempted(true);
    if (Object.keys(validateGroup(data)).length > 0) {
      // Scroll to first error
      setTimeout(() => {
        const el = document.querySelector('[data-error="true"]');
        if (el) el.scrollIntoView ? null : null; // avoid scrollIntoView per guidelines
      }, 50);
      return;
    }
    setSavingState('saving');
    setTimeout(() => {
      setSavingState('saved');
      setCreatedId(Math.floor(Math.random() * 9000) + 1000);
    }, 900);
  };

  const onCreateAndContinue = () => {
    onCreate();
    // After save, in real app would navigate to schedule step
  };

  if (createdId) {
    return <SuccessScreen group={data} id={createdId} onCreateAnother={() => {
      setData(EMPTY_GROUP); setCreatedId(null); setSavingState('idle');
      setSubmitAttempted(false); setCodeAuto(true);
    }} />;
  }

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="groups" />
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0, position: 'relative' }}>
        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <a href="Groups.html" style={{ color: '#64748b' }}>Школа</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Groups.html" style={{ color: '#64748b' }}>Группы</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Новая группа</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '22px 32px 20px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 20,
        }}>
          <a href="Groups.html" style={{
            width: 36, height: 36, borderRadius: 10, border: '1px solid #e2e8f0',
            background: '#fff', display: 'inline-flex', alignItems: 'center',
            justifyContent: 'center', color: '#64748b', flexShrink: 0,
          }}><Icon.ArrowLeft size={16} /></a>
          <div style={{ flex: 1, minWidth: 0 }}>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
              Новая группа
            </h1>
            <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
              Шаг 1 из 3 — основные данные. Расписание и состав студентов вы добавите следом.
            </div>
          </div>
          <ProgressIndicator current={1} steps={[
            { id: 1, label: 'Основное' },
            { id: 2, label: 'Расписание' },
            { id: 3, label: 'Студенты' },
          ]} />
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 120px' }}>
          <div style={{ maxWidth: 1100, margin: '0 auto', display: 'grid',
            gridTemplateColumns: 'minmax(0, 1fr) 320px', gap: 24, alignItems: 'start' }}>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 20, minWidth: 0 }}>

              <GCSection icon="Briefcase" title="Идентификация" subtitle="Как группа называется и из какого курса"
                step="1.1">
                <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
                  <F.Field label="Название группы" required error={errors.name}
                    hint="Краткое и понятное — будет отображаться в расписании, журнале и кабинете студента">
                    <F.Text
                      value={data.name}
                      onChange={e => update({ name: e.target.value })}
                      placeholder="Например, English Intermediate · вечерняя"
                      error={errors.name}
                      maxLength={80}
                    />
                  </F.Field>

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
                    <F.Field label="Курс / программа" required error={errors.course}
                      hint="Программа, по которой будет идти обучение">
                      <F.Select
                        value={data.course}
                        onChange={v => update({ course: v })}
                        options={COURSES}
                        placeholder="Выберите курс"
                        error={errors.course}
                      />
                    </F.Field>
                    <CodeField
                      value={data.code}
                      onChange={onCodeChange}
                      auto={codeAuto}
                      onAutoToggle={() => {
                        if (!codeAuto) {
                          setCodeAuto(true);
                          const taken = (window.GROUPS || []).map(g => g.code);
                          update({ code: suggestCode(data.level, data.course, taken) });
                        }
                      }}
                      error={errors.code}
                    />
                  </div>

                  <F.Field label="Размер группы" required error={errors.capacity}
                    hint="Максимум студентов — от этого числа зависит, какие кабинеты подойдут на следующем шаге">
                    <CapacityStepper
                      value={data.capacity}
                      onChange={v => update({ capacity: v, ...(data.room && (ROOMS.find(r => r.value === data.room)?.seats || 0) < v ? { room: '' } : {}) })}
                      error={errors.capacity}
                    />
                  </F.Field>
                </div>
              </GCSection>

              <GCSection icon="GraduationCap" title="Уровень и формат" subtitle="Кому подходит группа и где проходят занятия"
                step="1.2">
                <div style={{ display: 'flex', flexDirection: 'column', gap: 22 }}>
                  <F.Field label="Уровень / возрастная категория" required error={errors.level}>
                    <F.CardRadio
                      value={data.level}
                      onChange={v => update({ level: v })}
                      options={LEVEL_OPTIONS}
                      columns={4}
                    />
                  </F.Field>

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
                    <F.Field label="Формат проведения" required error={errors.format}>
                      <F.Segmented
                        value={data.format}
                        onChange={v => update({ format: v, ...(v === 'online' ? { room: '' } : {}) })}
                        options={FORMAT_OPTIONS.map(o => {
                          const IC = Icon[o.icon];
                          return { value: o.value, label: o.label, icon: <IC size={14} /> };
                        })}
                      />
                    </F.Field>
                    <F.Field
                      label={data.format === 'online' ? 'Платформа' : 'Кабинет'}
                      required={data.format !== 'online'}
                      optional={data.format === 'online'}
                      error={errors.room}
                      hint={data.format === 'online' ? 'Если выбираете платформу — необязательно, можно настроить позже' : undefined}
                    >
                      {data.format === 'online' ? (
                        <F.Select
                          value={data.room}
                          onChange={v => update({ room: v })}
                          options={[
                            { value: 'zoom',  label: 'Zoom' },
                            { value: 'meet',  label: 'Google Meet' },
                            { value: 'tg',    label: 'Telegram (звонок)' },
                            { value: 'teams', label: 'Microsoft Teams' },
                          ]}
                          placeholder="Выберите платформу"
                        />
                      ) : (
                        <RoomSelect
                          value={data.room}
                          onChange={v => update({ room: v })}
                          capacity={data.capacity || 0}
                          error={errors.room}
                        />
                      )}
                    </F.Field>
                  </div>
                </div>
              </GCSection>

              <GCSection icon="Users" title="Преподаватель и сроки" subtitle="Кто ведёт и в какие даты идёт курс"
                step="1.3">
                <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
                  <F.Field label="Преподаватель" required error={errors.teacher}
                    hint="Можно изменить позже. Дополнительных преподавателей добавите на следующих шагах.">
                    <TeacherPicker
                      value={data.teacher}
                      onChange={v => update({ teacher: v })}
                      error={errors.teacher}
                    />
                  </F.Field>

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
                    <F.Field label="Дата начала" required error={errors.starts}>
                      <F.Text type="date" value={data.starts}
                        onChange={e => update({ starts: e.target.value })}
                        error={errors.starts}
                        icon={<Icon.Calendar size={16} />}
                      />
                    </F.Field>
                    <F.Field label="Дата окончания" required error={errors.ends}>
                      <F.Text type="date" value={data.ends}
                        min={data.starts || undefined}
                        onChange={e => update({ ends: e.target.value })}
                        error={errors.ends}
                        icon={<Icon.Calendar size={16} />}
                      />
                    </F.Field>
                  </div>
                </div>
              </GCSection>

              <GCSection icon="FileText" title="Описание" subtitle="Заметки для администраторов и студентов — опционально"
                step="1.4">
                <F.Field label="Описание группы" optional
                  hint="Видно в карточке группы и при записи. Расскажите про целевую аудиторию, особенности программы, требования к уровню.">
                  <F.Textarea
                    value={data.description}
                    onChange={e => update({ description: e.target.value })}
                    placeholder="Например: интенсивный курс для тех, кто готовится к собеседованию на английском. Упор на разговорную практику и кейсы..."
                    maxLength={600}
                    rows={4}
                  />
                  <div style={{
                    marginTop: 4, fontSize: 11, color: '#94a3b8', textAlign: 'right',
                    fontVariantNumeric: 'tabular-nums',
                  }}>
                    {(data.description || '').length} / 600
                  </div>
                </F.Field>
              </GCSection>
            </div>

            {/* Sticky preview */}
            <div style={{ position: 'sticky', top: 0, alignSelf: 'start' }}>
              <PreviewCard data={data} />
              <NextStepsHint />
            </div>
          </div>
        </div>

        {/* Sticky save bar */}
        <CreateBar
          errorCount={submitAttempted ? errorCount : 0}
          savingState={savingState}
          onCreate={onCreate}
          onCreateContinue={onCreateAndContinue}
        />
      </div>
    </div>
  );
}

// ── Code field with auto / manual toggle ─────────────────────────────
function CodeField({ value, onChange, auto, onAutoToggle, error }) {
  return (
    <F.Field
      label="Код группы"
      required
      error={error}
      hint={auto ? 'Сформирован автоматически — можно отредактировать' : 'Используется в журнале и расписании'}
    >
      <div style={{ position: 'relative' }}>
        <F.Text
          value={value}
          onChange={e => onChange(e.target.value)}
          placeholder="EN-B1-12"
          error={error}
          style={{ fontFamily: 'var(--edv-font-mono)', textTransform: 'uppercase', paddingRight: 84 }}
        />
        <button
          type="button"
          onClick={onAutoToggle}
          style={{
            position: 'absolute', right: 6, top: 6, height: 30, padding: '0 10px',
            borderRadius: 8, border: '1px solid',
            borderColor: auto ? 'rgba(79,70,229,0.25)' : '#e2e8f0',
            background: auto ? 'rgba(79,70,229,0.08)' : '#fff',
            color: auto ? '#4338ca' : '#64748b',
            fontSize: 11.5, fontWeight: 600, fontFamily: 'inherit', cursor: 'pointer',
            display: 'inline-flex', alignItems: 'center', gap: 5,
          }}
          title={auto ? 'Авто-генерация по уровню и курсу' : 'Восстановить авто-код'}
        >
          <Icon.Sparkles size={12} />{auto ? 'авто' : 'вернуть'}
        </button>
      </div>
    </F.Field>
  );
}

// ── Teacher picker (search + dropdown) ───────────────────────────────
function TeacherPicker({ value, onChange, error }) {
  const [open, setOpen] = React.useState(false);
  const [query, setQuery] = React.useState('');
  const ref = React.useRef(null);

  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);

  const selected = TEACHERS.find(t => t.value === value);
  const filtered = query
    ? TEACHERS.filter(t => t.label.toLowerCase().includes(query.toLowerCase())
        || t.role.toLowerCase().includes(query.toLowerCase()))
    : TEACHERS;

  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button
        type="button"
        onClick={() => setOpen(o => !o)}
        style={{
          width: '100%', height: 42, borderRadius: 12,
          border: `1px solid ${error ? '#ef4444' : open ? '#6366f1' : '#e2e8f0'}`,
          boxShadow: open && !error ? '0 0 0 3px rgba(99,102,241,0.25)' : error ? '0 0 0 3px rgba(239,68,68,0.15)' : 'none',
          background: '#fff', padding: '0 14px', textAlign: 'left',
          fontFamily: 'inherit', cursor: 'pointer',
          display: 'flex', alignItems: 'center', gap: 10,
        }}>
        {selected ? (
          <>
            <Avatar name={selected.label} size={26} />
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontSize: 13.5, color: '#0f172a', fontWeight: 500 }}>{selected.label}</div>
              <div style={{ fontSize: 11.5, color: '#64748b' }}>{selected.role}</div>
            </div>
          </>
        ) : (
          <span style={{ flex: 1, color: '#94a3b8', fontSize: 14 }}>Выберите преподавателя…</span>
        )}
        <Icon.ChevronDown size={16} stroke="#94a3b8" />
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 4px)', left: 0, right: 0, zIndex: 20,
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 12,
          boxShadow: '0 10px 30px rgba(15,23,42,0.10)', padding: 6, maxHeight: 320, overflowY: 'auto',
        }}>
          <div style={{ position: 'sticky', top: 0, background: '#fff', padding: '4px 4px 8px' }}>
            <div style={{ position: 'relative' }}>
              <Icon.Search size={13} stroke="#94a3b8"
                style={{ position: 'absolute', left: 10, top: 9 }} />
              <input
                autoFocus
                value={query}
                onChange={e => setQuery(e.target.value)}
                placeholder="Поиск по имени или роли"
                style={{
                  width: '100%', height: 32, paddingLeft: 30, paddingRight: 10,
                  borderRadius: 8, border: '1px solid #e2e8f0', background: '#f8fafc',
                  fontSize: 13, fontFamily: 'inherit', outline: 'none',
                }}
              />
            </div>
          </div>
          {filtered.length === 0 ? (
            <div style={{ padding: '12px', fontSize: 13, color: '#94a3b8', textAlign: 'center' }}>
              Никого не нашли
            </div>
          ) : filtered.map(t => {
            const active = value === t.value;
            return (
              <button key={t.value} type="button"
                onClick={() => { onChange(t.value); setOpen(false); setQuery(''); }}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: active ? '#f0f4ff' : 'transparent', cursor: 'pointer',
                  textAlign: 'left', fontFamily: 'inherit',
                }}
                onMouseEnter={e => { if (!active) e.currentTarget.style.background = '#f8fafc'; }}
                onMouseLeave={e => { if (!active) e.currentTarget.style.background = 'transparent'; }}>
                <Avatar name={t.label} size={28} />
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div style={{ fontSize: 13.5, color: '#0f172a', fontWeight: active ? 600 : 500 }}>
                    {t.label}
                  </div>
                  <div style={{ fontSize: 11.5, color: '#64748b' }}>{t.role}</div>
                </div>
                {active && <Icon.Check size={14} stroke="#4f46e5" sw={2.5} />}
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

// ── Room select (capacity-aware) ─────────────────────────────────────
function RoomSelect({ value, onChange, capacity, error }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);

  React.useEffect(() => {
    if (!open) return;
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);

  const selected = ROOMS.find(r => r.value === value);
  const fits = (r) => !capacity || r.seats >= capacity;
  const fitting = ROOMS.filter(fits);
  const tooSmall = ROOMS.filter(r => !fits(r));

  return (
    <div ref={ref} style={{ position: 'relative' }}>
      <button
        type="button"
        onClick={() => setOpen(o => !o)}
        style={{
          width: '100%', height: 42, borderRadius: 12,
          border: `1px solid ${error ? '#ef4444' : open ? '#6366f1' : '#e2e8f0'}`,
          boxShadow: open && !error ? '0 0 0 3px rgba(99,102,241,0.25)' : error ? '0 0 0 3px rgba(239,68,68,0.15)' : 'none',
          background: '#fff', padding: '0 14px', textAlign: 'left',
          fontFamily: 'inherit', cursor: 'pointer',
          display: 'flex', alignItems: 'center', gap: 10,
        }}>
        {selected ? (
          <>
            <div style={{
              width: 26, height: 26, borderRadius: 7, flexShrink: 0,
              background: 'rgba(79,70,229,0.08)', color: '#4338ca',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontFamily: 'var(--edv-font-mono)', fontSize: 11, fontWeight: 700,
            }}>{selected.floor}э</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontSize: 13.5, color: '#0f172a', fontWeight: 500 }}>{selected.label}</div>
              <div style={{ fontSize: 11.5, color: '#64748b', fontVariantNumeric: 'tabular-nums' }}>
                {selected.seats} мест{capacity ? ` · нужно ${capacity}` : ''}
              </div>
            </div>
          </>
        ) : (
          <span style={{ flex: 1, color: '#94a3b8', fontSize: 14 }}>
            {capacity ? `Кабинет от ${capacity} мест…` : 'Выберите кабинет…'}
          </span>
        )}
        <Icon.ChevronDown size={16} stroke="#94a3b8" />
      </button>
      {open && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 4px)', left: 0, right: 0, zIndex: 20,
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 12,
          boxShadow: '0 10px 30px rgba(15,23,42,0.10)', padding: 6, maxHeight: 360, overflowY: 'auto',
        }}>
          {capacity > 0 && (
            <div style={{
              padding: '6px 10px 8px', fontSize: 11, color: '#64748b',
              display: 'flex', alignItems: 'center', gap: 6,
              borderBottom: '1px solid #f1f5f9', marginBottom: 4,
            }}>
              <Icon.Users size={12} stroke="#94a3b8" />
              <span>Группа на <strong style={{ color: '#0f172a', fontVariantNumeric: 'tabular-nums' }}>{capacity}</strong> {declensionGC(capacity, ['место', 'места', 'мест'])}</span>
              <span style={{ flex: 1 }} />
              <span style={{ fontVariantNumeric: 'tabular-nums', color: '#94a3b8' }}>
                подходит {fitting.length} из {ROOMS.length}
              </span>
            </div>
          )}

          {fitting.length === 0 && (
            <div style={{ padding: '14px 12px', fontSize: 12.5, color: '#64748b', textAlign: 'center', lineHeight: 1.5 }}>
              Нет кабинетов на {capacity} мест.<br/>
              Уменьшите размер группы или выберите онлайн-формат.
            </div>
          )}

          {fitting.map(r => {
            const active = value === r.value;
            const tight = capacity && r.seats - capacity <= 2;
            return (
              <button key={r.value} type="button"
                onClick={() => { onChange(r.value); setOpen(false); }}
                style={{
                  display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                  padding: '8px 10px', borderRadius: 8, border: 'none',
                  background: active ? '#f0f4ff' : 'transparent', cursor: 'pointer',
                  textAlign: 'left', fontFamily: 'inherit',
                }}
                onMouseEnter={e => { if (!active) e.currentTarget.style.background = '#f8fafc'; }}
                onMouseLeave={e => { if (!active) e.currentTarget.style.background = 'transparent'; }}>
                <div style={{
                  width: 28, height: 28, borderRadius: 7, flexShrink: 0,
                  background: active ? '#4f46e5' : 'rgba(79,70,229,0.08)',
                  color: active ? '#fff' : '#4338ca',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  fontFamily: 'var(--edv-font-mono)', fontSize: 11, fontWeight: 700,
                }}>{r.floor}э</div>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div style={{ fontSize: 13.5, color: '#0f172a', fontWeight: active ? 600 : 500 }}>
                    {r.label}
                  </div>
                  <div style={{ fontSize: 11.5, color: '#64748b', display: 'flex', gap: 6, alignItems: 'center' }}>
                    <Icon.Users size={11} stroke="#94a3b8" />
                    <span style={{ fontVariantNumeric: 'tabular-nums' }}>{r.seats} мест</span>
                    {tight ? (
                      <span style={{
                        marginLeft: 'auto', fontSize: 10.5, padding: '1px 6px', borderRadius: 9999,
                        background: 'rgba(217,119,6,0.10)', color: '#b45309', fontWeight: 600,
                      }}>впритык</span>
                    ) : capacity ? (
                      <span style={{
                        marginLeft: 'auto', fontSize: 10.5, padding: '1px 6px', borderRadius: 9999,
                        background: 'rgba(16,185,129,0.10)', color: '#047857', fontWeight: 600,
                      }}>+{r.seats - capacity}</span>
                    ) : null}
                  </div>
                </div>
                {active && <Icon.Check size={14} stroke="#4f46e5" sw={2.5} />}
              </button>
            );
          })}

          {tooSmall.length > 0 && (
            <>
              <div style={{
                padding: '10px 10px 4px', fontSize: 10.5, color: '#94a3b8',
                letterSpacing: '0.05em', textTransform: 'uppercase', fontWeight: 600,
                marginTop: 4, borderTop: '1px solid #f1f5f9',
              }}>Слишком маленькие</div>
              {tooSmall.map(r => (
                <div key={r.value}
                  style={{
                    display: 'flex', alignItems: 'center', gap: 10, width: '100%',
                    padding: '8px 10px', borderRadius: 8,
                    opacity: 0.5, cursor: 'not-allowed',
                  }}>
                  <div style={{
                    width: 28, height: 28, borderRadius: 7, flexShrink: 0,
                    background: '#f1f5f9', color: '#94a3b8',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    fontFamily: 'var(--edv-font-mono)', fontSize: 11, fontWeight: 700,
                  }}>{r.floor}э</div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ fontSize: 13.5, color: '#475569', fontWeight: 500 }}>
                      {r.label}
                    </div>
                    <div style={{ fontSize: 11.5, color: '#94a3b8', fontVariantNumeric: 'tabular-nums' }}>
                      {r.seats} мест — мало на {capacity - r.seats}
                    </div>
                  </div>
                </div>
              ))}
            </>
          )}
        </div>
      )}
    </div>
  );
}

// ── Capacity stepper ─────────────────────────────────────────────────
function CapacityStepper({ value, onChange, error }) {
  const dec = () => onChange(Math.max(1, (value || 1) - 1));
  const inc = () => onChange(Math.min(50, (value || 1) + 1));
  return (
    <div style={{
      display: 'flex', alignItems: 'center',
      border: `1px solid ${error ? '#ef4444' : '#e2e8f0'}`, borderRadius: 12,
      background: '#fff', overflow: 'hidden', height: 42,
    }}>
      <button type="button" onClick={dec}
        style={{ width: 42, height: 42, border: 'none', background: 'transparent',
          color: '#64748b', cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round">
          <path d="M5 12h14"/>
        </svg>
      </button>
      <input
        type="number" min={1} max={50}
        value={value}
        onChange={e => onChange(parseInt(e.target.value, 10) || 0)}
        style={{
          flex: 1, height: 42, border: 'none', outline: 'none', textAlign: 'center',
          fontSize: 15, fontWeight: 600, color: '#0f172a', fontFamily: 'inherit',
          fontVariantNumeric: 'tabular-nums', background: 'transparent',
          MozAppearance: 'textfield',
        }}
      />
      <button type="button" onClick={inc}
        style={{ width: 42, height: 42, border: 'none', background: 'transparent',
          color: '#64748b', cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
        <Icon.Plus size={14} sw={2.5} />
      </button>
    </div>
  );
}

// ── Live preview card ────────────────────────────────────────────────
function PreviewCard({ data }) {
  const lvl = LEVEL_OPTIONS.find(l => l.value === data.level);
  const course = COURSES.find(c => c.value === data.course);
  const teacher = TEACHERS.find(t => t.value === data.teacher);
  const room = ROOMS.find(r => r.value === data.room);
  const fmt = FORMAT_OPTIONS.find(f => f.value === data.format);

  const filled = [data.name, data.code, data.level, data.course, data.teacher,
    data.room || data.format === 'online', data.starts, data.ends].filter(Boolean).length;
  const pct = Math.round(filled / 8 * 100);

  return (
    <div style={{
      background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
      overflow: 'hidden',
    }}>
      <div style={{
        padding: '12px 16px', borderBottom: '1px solid #f1f5f9',
        display: 'flex', alignItems: 'center', gap: 8,
        fontSize: 11.5, fontWeight: 600, color: '#64748b',
        letterSpacing: '0.05em', textTransform: 'uppercase',
      }}>
        <Icon.Sparkles size={13} stroke="#4f46e5" />
        <span style={{ flex: 1 }}>Предпросмотр карточки</span>
        <span style={{
          fontVariantNumeric: 'tabular-nums', color: pct === 100 ? '#047857' : '#94a3b8',
          textTransform: 'none', letterSpacing: 0,
        }}>{pct}%</span>
      </div>

      {/* Card preview */}
      <div style={{ padding: 16 }}>
        <div style={{
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 12, padding: 14,
          display: 'flex', flexDirection: 'column', gap: 10,
        }}>
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10 }}>
            <div style={{
              width: 36, height: 36, borderRadius: 10, flexShrink: 0,
              background: lvl ? LEVEL_TONES[GROUP_LEVELS.find(l => l.value === lvl.value)?.tone || 'slate'].bg : '#f1f5f9',
              color: lvl ? LEVEL_TONES[GROUP_LEVELS.find(l => l.value === lvl.value)?.tone || 'slate'].fg : '#94a3b8',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontSize: 11.5, fontWeight: 700, fontFamily: 'var(--edv-font-mono)',
            }}>{data.level || '—'}</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{
                fontSize: 13.5, fontWeight: 600, color: data.name ? '#0f172a' : '#cbd5e1',
                lineHeight: 1.3,
              }}>
                {data.name || 'Название группы'}
              </div>
              <div style={{
                fontFamily: 'var(--edv-font-mono)', fontSize: 11, color: '#94a3b8', marginTop: 3,
              }}>
                {data.code || 'код'} · {course?.label || 'курс'}
              </div>
            </div>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 12 }}>
            {teacher ? (
              <>
                <Avatar name={teacher.label} size={20} />
                <span style={{ color: '#475569' }}>{teacher.label}</span>
              </>
            ) : (
              <span style={{ color: '#cbd5e1' }}>Преподаватель не выбран</span>
            )}
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: 5, fontSize: 12, color: '#475569' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              {fmt && (() => { const Ic = Icon[fmt.icon]; return <Ic size={12} stroke="#94a3b8" />; })()}
              <span>{fmt?.label}{room && ` · ${room.label}`}</span>
            </div>
            {(data.starts || data.ends) && (
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <Icon.Calendar size={12} stroke="#94a3b8" />
                <span style={{ fontVariantNumeric: 'tabular-nums' }}>
                  {data.starts ? formatDate(data.starts) : '—'} → {data.ends ? formatDate(data.ends) : '—'}
                </span>
              </div>
            )}
          </div>

          <div style={{ paddingTop: 10, borderTop: '1px solid #f1f5f9' }}>
            <div style={{ fontSize: 11.5, color: '#64748b', marginBottom: 4 }}>
              0 <span style={{ color: '#94a3b8' }}>/ {data.capacity || 0} мест</span>
            </div>
            <div style={{ width: '100%', height: 4, background: '#f1f5f9', borderRadius: 9999 }}>
              <div style={{ width: '0%', height: '100%', background: '#cbd5e1', borderRadius: 9999 }} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function NextStepsHint() {
  return (
    <div style={{
      marginTop: 14, padding: 14, borderRadius: 12,
      background: 'rgba(79,70,229,0.04)', border: '1px solid rgba(79,70,229,0.15)',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginBottom: 8,
        fontSize: 11.5, fontWeight: 600, color: '#4338ca',
        letterSpacing: '0.05em', textTransform: 'uppercase' }}>
        <Icon.ArrowRight size={12} stroke="#4338ca" />
        Дальше
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        <NextItem n="2" icon="Calendar" title="Расписание"
          desc="Дни недели, время, длительность занятий" />
        <NextItem n="3" icon="UserPlus" title="Студенты"
          desc="Зачислите участников или откройте набор" />
      </div>
    </div>
  );
}
function NextItem({ n, icon, title, desc }) {
  const Ic = Icon[icon];
  return (
    <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10, fontSize: 12.5 }}>
      <div style={{
        width: 22, height: 22, borderRadius: 9999, flexShrink: 0,
        background: '#fff', color: '#4338ca', border: '1px solid rgba(79,70,229,0.2)',
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        fontSize: 11, fontWeight: 700, fontVariantNumeric: 'tabular-nums',
      }}>{n}</div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ fontWeight: 600, color: '#0f172a', display: 'inline-flex', alignItems: 'center', gap: 6 }}>
          <Ic size={12} stroke="#475569" />{title}
        </div>
        <div style={{ color: '#64748b', marginTop: 1 }}>{desc}</div>
      </div>
    </div>
  );
}

function formatDate(s) {
  if (!s) return '';
  const [y, m, d] = s.split('-');
  return `${d}.${m}.${y}`;
}

// ── Progress indicator (top-right of header) ─────────────────────────
function ProgressIndicator({ current, steps }) {
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

// ── Sticky create bar ────────────────────────────────────────────────
function CreateBar({ errorCount, savingState, onCreate, onCreateContinue }) {
  return (
    <div style={{
      position: 'absolute', left: 0, right: 0, bottom: 0,
      background: '#fff', borderTop: '1px solid #e2e8f0',
      boxShadow: '0 -4px 12px rgba(15,23,42,0.06)',
      padding: '14px 32px',
      display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 20,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, fontSize: 13 }}>
        {errorCount > 0 ? (
          <>
            <div style={{
              width: 32, height: 32, borderRadius: 9999, flexShrink: 0,
              background: 'rgba(239,68,68,0.12)', color: '#b91c1c',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}>
              <Icon.AlertCircle size={16} stroke="#b91c1c" />
            </div>
            <div>
              <div style={{ fontWeight: 600, color: '#991b1b' }}>
                {errorCount} {declensionGC(errorCount, ['ошибка', 'ошибки', 'ошибок'])} в форме
              </div>
              <div style={{ fontSize: 12, color: '#64748b' }}>
                Исправьте поля, отмеченные красным
              </div>
            </div>
          </>
        ) : (
          <>
            <div style={{
              width: 32, height: 32, borderRadius: 9999, flexShrink: 0,
              background: 'rgba(79,70,229,0.10)', color: '#4338ca',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}>
              <Icon.Info size={16} stroke="#4338ca" />
            </div>
            <div>
              <div style={{ fontWeight: 600, color: '#0f172a' }}>
                Шаг 1 из 3 — основные данные
              </div>
              <div style={{ fontSize: 12, color: '#64748b' }}>
                Расписание и состав студентов настроите на следующих шагах
              </div>
            </div>
          </>
        )}
      </div>
      <div style={{ display: 'flex', gap: 10 }}>
        <a href="Groups.html">
          <Button variant="ghost" disabled={savingState === 'saving'}>Отмена</Button>
        </a>
        <Button variant="secondary"
          onClick={onCreate}
          disabled={savingState === 'saving'}>
          Сохранить как черновик
        </Button>
        <Button onClick={onCreateContinue} disabled={savingState === 'saving'}
          style={savingState === 'saving' ? { opacity: 0.7, cursor: 'wait' } : {}}>
          {savingState === 'saving' ? (
            <>
              <span style={{
                display: 'inline-block', width: 14, height: 14,
                border: '2px solid rgba(255,255,255,0.35)', borderTopColor: '#fff',
                borderRadius: 9999, animation: 'spin 0.7s linear infinite',
              }} />
              Создание…
            </>
          ) : (
            <>
              Создать и продолжить<Icon.ArrowRight size={15} sw={2.5} />
            </>
          )}
        </Button>
      </div>
    </div>
  );
}

// ── Success screen ───────────────────────────────────────────────────
function SuccessScreen({ group, id, onCreateAnother }) {
  const lvl = LEVEL_OPTIONS.find(l => l.value === group.level);
  const teacher = TEACHERS.find(t => t.value === group.teacher);

  return (
    <div style={{ display: 'flex', height: '100vh', minHeight: 700, background: '#f8fafc', overflow: 'hidden' }}>
      <Sidebar active="groups" />
      <div style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 32 }}>
        <div style={{ maxWidth: 480, textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 18 }}>
          <div style={{
            width: 72, height: 72, borderRadius: 9999,
            background: 'rgba(16,185,129,0.12)', color: '#047857',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            animation: 'scaleIn .25s ease-out',
          }}>
            <Icon.CircleCheck size={38} stroke="#047857" sw={2} />
          </div>
          <div>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
              Группа создана
            </h1>
            <div style={{ fontSize: 14, color: '#64748b', marginTop: 6, lineHeight: 1.55 }}>
              <strong style={{ color: '#0f172a' }}>{group.name}</strong> ·{' '}
              <span style={{ fontFamily: 'var(--edv-font-mono)', fontSize: 12.5 }}>{group.code}</span>
              <br/>
              {lvl?.tag} · {teacher?.label}
            </div>
          </div>
          <div style={{
            width: '100%', padding: '14px 16px', background: '#fff',
            border: '1px solid #e2e8f0', borderRadius: 12, textAlign: 'left',
            display: 'flex', flexDirection: 'column', gap: 10,
          }}>
            <div style={{ fontSize: 11.5, fontWeight: 600, color: '#64748b',
              letterSpacing: '0.05em', textTransform: 'uppercase' }}>
              Что дальше
            </div>
            <a href="Group Schedule Setup.html" style={NextLinkStyle}>
              <span style={NextNumStyle}>2</span>
              <span style={{ flex: 1, fontSize: 13.5, color: '#0f172a', fontWeight: 500 }}>
                Настроить расписание
              </span>
              <Icon.ArrowRight size={14} stroke="#94a3b8" />
            </a>
            <a href="Group Students.html" style={NextLinkStyle}>
              <span style={NextNumStyle}>3</span>
              <span style={{ flex: 1, fontSize: 13.5, color: '#0f172a', fontWeight: 500 }}>
                Зачислить студентов
              </span>
              <Icon.ArrowRight size={14} stroke="#94a3b8" />
            </a>
          </div>
          <div style={{ display: 'flex', gap: 10, marginTop: 4 }}>
            <a href="Groups.html"><Button variant="secondary">К списку групп</Button></a>
            <Button onClick={onCreateAnother} variant="ghost">
              <Icon.Plus size={15} />Создать ещё одну
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
const NextLinkStyle = {
  display: 'flex', alignItems: 'center', gap: 10,
  padding: '8px 10px', borderRadius: 8, background: '#f8fafc',
};
const NextNumStyle = {
  width: 22, height: 22, borderRadius: 9999, flexShrink: 0,
  background: '#fff', color: '#4338ca', border: '1px solid rgba(79,70,229,0.2)',
  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
  fontSize: 11.5, fontWeight: 700, fontVariantNumeric: 'tabular-nums',
};

function declensionGC(n, forms) {
  const abs = Math.abs(n);
  const m10 = abs % 10, m100 = abs % 100;
  if (m10 === 1 && m100 !== 11) return forms[0];
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return forms[1];
  return forms[2];
}

window.GroupCreateApp = GroupCreateApp;
