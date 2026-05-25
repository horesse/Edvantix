// Lesson create — главный компонент страницы.
const { useState: useStateLC, useMemo: useMemoLC, useEffect: useEffectLC } = React;

function CreateLessonApp() {
  const [data, setData] = useStateLC(makeInitialLesson);
  const [savingState, setSavingState] = useStateLC('idle');

  const update = (patch) => setData(d => ({ ...d, ...patch }));

  // Loading template when type changes — но только если структура пустая
  // или единственная (избежать перезатирания юзерских блоков).
  const onTypeChange = (newType) => {
    setData(d => {
      const isUntouched = d.blocks.length === 0
        || d.blocks.every(b => b.title === BLOCK_DEFAULT_TITLES[b.type]);
      if (isUntouched) {
        const tpl = LESSON_TEMPLATES[newType] || [];
        return {
          ...d, type: newType,
          blocks: tpl.map((t, i) => ({
            id: `b${Date.now()}-${i}`,
            type: t,
            title: BLOCK_DEFAULT_TITLES[t],
            durationMin: BLOCK_LIBRARY.find(b => b.type === t).defaultMin,
          })),
        };
      }
      return { ...d, type: newType };
    });
  };

  // Validation
  const errors = useMemoLC(() => {
    const e = {};
    if (!data.title.trim()) e.title = 'Укажите название урока';
    if (!data.moduleId) e.moduleId = 'Выберите модуль';
    if (data.blocks.length === 0) e.blocks = 'Добавьте хотя бы один блок';
    return e;
  }, [data]);
  const errorCount = Object.keys(errors).length;

  // Validation items shown in the bottom bar
  const validationItems = useMemoLC(() => [
    { ok: !!data.title.trim(),         label: 'Название' },
    { ok: !!data.moduleId,             label: 'Модуль' },
    { ok: data.objectives.filter(o => o.trim()).length >= 2, label: '≥ 2 цели' },
    { ok: data.blocks.length > 0,      label: 'Структура' },
    { ok: data.materials.length > 0,   label: 'Материалы' },
  ], [data]);

  const totalMin = useMemoLC(
    () => data.blocks.reduce((a, b) => a + (b.durationMin || 0), 0),
    [data.blocks]);

  const moduleN = useMemoLC(() => {
    const m = window.MODULES.find(m => m.id === data.moduleId);
    return m ? m.n : '—';
  }, [data.moduleId]);

  // Save flow
  const doSave = (then) => {
    setSavingState('saving');
    setTimeout(() => {
      setSavingState('idle');
      then?.();
    }, 900);
  };

  const onSaveDraft = () => doSave(() => update({ status: 'draft' }));
  const onPublish = () => doSave(() => update({ status: 'published' }));

  // Block prompt navigation away if dirty
  useEffectLC(() => {
    const h = (e) => { e.preventDefault(); e.returnValue = ''; };
    window.addEventListener('beforeunload', h);
    return () => window.removeEventListener('beforeunload', h);
  }, []);

  return (
    <div style={{
      display: 'flex', height: '100vh', minHeight: 700,
      background: '#f8fafc', overflow: 'hidden',
    }}>
      <Sidebar active="courses" />

      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0, position: 'relative' }}>

        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>Школа</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Courses.html" style={{ color: '#64748b' }}>Курсы</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <a href="Course.html" style={{ color: '#64748b' }}>{window.COURSE.name}</a>
          <Icon.ChevronRight size={14} stroke="#cbd5e1" />
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Новый урок</span>
        </div>

        {/* Page header */}
        <CreateHeader data={data} totalMin={totalMin} moduleN={moduleN} />

        {/* Scrollable content */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 110px' }}>
          <div style={{
            maxWidth: 1240, margin: '0 auto',
            display: 'grid', gridTemplateColumns: 'minmax(0, 1fr) 320px', gap: 24,
            alignItems: 'flex-start',
          }}>
            {/* Main column */}
            <div style={{ display: 'flex', flexDirection: 'column', gap: 20, minWidth: 0 }}>
              <PlacementSection value={data} onChange={update} />
              <AboutSection value={data} onChange={update}
                errors={errors} onTypeChange={onTypeChange} />
              <ObjectivesSection value={data} onChange={update} />
              <StructureSection value={data} onChange={update}
                totalMin={totalMin} />
              <MaterialsSection value={data} onChange={update} />
            </div>

            {/* Right rail */}
            <div style={{
              display: 'flex', flexDirection: 'column', gap: 16,
              position: 'sticky', top: 24, alignSelf: 'flex-start',
            }}>
              <StatusRail value={data} onChange={update} />
              <LessonPreviewRail value={data} totalMin={totalMin} moduleN={moduleN} />
              <AiAssistRail onClick={() => {}} />
            </div>
          </div>
        </div>

        {/* Sticky save bar */}
        <LcSaveBar
          canPublish={errorCount === 0}
          savingState={savingState}
          errorCount={errorCount}
          validationItems={validationItems}
          onSaveDraft={onSaveDraft}
          onPublish={onPublish}
          onCancel={() => { /* back to course */ }}
        />
      </div>
    </div>
  );
}

// ── Page header — hero-like row with cover, title preview, KPIs ─────
function CreateHeader({ data, totalMin, moduleN }) {
  const t = window.LESSON_TYPES[data.type];
  const Ic = Icon[t.icon];
  const c = window.COURSE;
  const subjTone = window.SUBJECT_TONES?.[window.COURSE_SUBJECTS?.[c.subject]?.tone];
  return (
    <div style={{
      padding: '20px 32px', borderBottom: '1px solid #e2e8f0', background: '#fff',
    }}>
      <div style={{ maxWidth: 1240, margin: '0 auto',
        display: 'flex', alignItems: 'center', gap: 20 }}>

        {/* Cover */}
        <div style={{
          width: 56, height: 56, borderRadius: 14, flexShrink: 0,
          background: t.bg, color: t.fg,
          display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 22, fontWeight: 700,
        }}>
          <Ic size={26} stroke="currentColor" />
        </div>

        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 4 }}>
            <span style={{
              fontFamily: 'var(--edv-font-mono)', fontSize: 11, color: '#4338ca',
              padding: '2px 8px', borderRadius: 5, background: '#eef2ff', fontWeight: 600,
            }}>МОД {moduleN}</span>
            <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
            <span style={{ fontSize: 12.5, color: '#64748b' }}>
              {c.name}
            </span>
            <span style={{ width: 3, height: 3, borderRadius: 9999, background: '#cbd5e1' }} />
            <span style={{ fontSize: 12.5, color: '#64748b',
              fontFamily: 'var(--edv-font-mono)' }}>{c.code}</span>
          </div>
          <h1 style={{
            margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em',
            color: '#0f172a',
          }}>Создание урока</h1>
          <div style={{ fontSize: 13, color: '#64748b', marginTop: 4 }}>
            Соберите урок из блоков, добавьте цели и материалы. После публикации появится в плане модуля.
          </div>
        </div>

        {/* Mini stats */}
        <div style={{ display: 'flex', gap: 8, flexShrink: 0 }}>
          <HeaderStat icon="Sparkles" value={data.blocks.length} label="блоков" />
          <HeaderStat icon="Clock"     value={totalMin}             label="минут" mono />
          <HeaderStat icon="FileText"  value={data.materials.length} label="материалов" />
          <HeaderStat icon="CircleCheck"
            value={data.objectives.filter(o => o.trim()).length}
            label="целей" />
        </div>
      </div>
    </div>
  );
}

function HeaderStat({ icon, value, label, mono }) {
  const Ic = Icon[icon];
  return (
    <div style={{
      minWidth: 96, padding: '10px 14px', borderRadius: 12,
      background: '#fafbfc', border: '1px solid #e2e8f0',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginBottom: 4 }}>
        <Ic size={12} stroke="#94a3b8" />
        <span style={{ fontSize: 10.5, color: '#94a3b8', fontWeight: 500,
          textTransform: 'uppercase', letterSpacing: '0.06em' }}>{label}</span>
      </div>
      <div style={{ fontSize: 18, fontWeight: 700, color: '#0f172a',
        letterSpacing: '-0.01em', lineHeight: 1,
        fontVariantNumeric: 'tabular-nums',
        fontFamily: mono ? 'var(--edv-font-mono)' : 'inherit' }}>{value}</div>
    </div>
  );
}

window.CreateLessonApp = CreateLessonApp;
