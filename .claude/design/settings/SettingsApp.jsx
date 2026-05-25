// Settings hub — индекс-страница настроек организации.
// Группирует: Организация → редактирование, Справочники → отдельные страницы,
// Доступы → роли и права, Платформа → прочие модули (заглушки).

const { useState: useS, useMemo: useM } = React;

// Локальные иконки, отсутствующие в общем kit/Icons.jsx
Object.assign(Icon, {
  Layers: (p) => <svg width={p.size||16} height={p.size||16} viewBox="0 0 24 24" fill="none" stroke={p.stroke||'currentColor'} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="m12 2 10 6-10 6L2 8z"/><path d="m2 17 10 6 10-6"/><path d="m2 12 10 6 10-6"/></svg>,
});

// ── Данные текущей организации (статический снимок) ───────────────────
const ORG_PREVIEW = {
  shortName: 'Школа «Эврика»',
  fullName: 'ООО «Образовательный центр Эврика»',
  legalForm: 'ООО',
  type: 'Частный учебный центр',
  registrationDate: '14 марта 2019',
  contact: 'director@eureka-school.ru',
  lastEditedAt: '18 апреля, 14:32',
  lastEditedBy: 'Анна Мельникова',
};

// ── Справочники: единый каталог ───────────────────────────────────────
const DIRECTORIES = [
  { id: 'levels', name: 'Уровни', icon: 'Layers',
    description: 'Уровни обучения для групп и курсов.',
    count: 7, archivedCount: 1, lastEdited: 'вчера', href: 'Levels.html' },
  { id: 'subjects', name: 'Предметы', icon: 'BookOpen',
    description: 'Учебные предметы и направления.',
    count: 24, archivedCount: 3, lastEdited: '3 дня назад', href: null },
  { id: 'lesson-types', name: 'Типы занятий', icon: 'CalendarDays',
    description: 'Урок, консультация, тест, мастер-класс.',
    count: 8, archivedCount: 0, lastEdited: '2 недели назад', href: null },
  { id: 'student-statuses', name: 'Статусы студентов', icon: 'UserCheck',
    description: 'Активный, в академе, выпускник, отчислен.',
    count: 7, archivedCount: 2, lastEdited: 'месяц назад', href: null,
    badge: 'системный' },
  { id: 'rooms', name: 'Кабинеты', icon: 'Building2',
    description: 'Помещения и аудитории школы.',
    count: 12, archivedCount: 0, lastEdited: '5 дней назад', href: null },
  { id: 'sources', name: 'Источники привлечения', icon: 'Megaphone',
    description: 'Откуда студенты узнают о школе.',
    count: 11, archivedCount: 4, lastEdited: 'неделю назад', href: null },
  { id: 'payment-methods', name: 'Способы оплаты', icon: 'CreditCard',
    description: 'Карта, перевод, рассрочка, материнский капитал.',
    count: 6, archivedCount: 1, lastEdited: 'месяц назад', href: null },
  { id: 'tags', name: 'Теги студентов', icon: 'Sparkles',
    description: 'Свободные метки для сегментации.',
    count: 18, archivedCount: 0, lastEdited: '4 дня назад', href: null },
];

// ── Прочие разделы платформы ──────────────────────────────────────────
const PLATFORM = [
  { id: 'notifications', name: 'Уведомления', icon: 'Bell',
    description: 'Шаблоны email и SMS, расписание автосообщений.',
    meta: '12 шаблонов', tone: 'indigo' },
  { id: 'integrations', name: 'Интеграции', icon: 'Sparkles',
    description: 'Платёжки, мессенджеры, телефония, аналитика.',
    meta: '3 из 12 подключено', tone: 'violet' },
  { id: 'branding', name: 'Брендинг', icon: 'School',
    description: 'Логотип, цвет, поддомен и фирменные письма.',
    meta: 'настроено', tone: 'amber' },
  { id: 'security', name: 'Безопасность', icon: 'Shield',
    description: 'Двухфакторная аутентификация, политика паролей, активные сессии.',
    meta: '2FA включён', tone: 'emerald' },
  { id: 'billing', name: 'Биллинг и тариф', icon: 'CreditCard',
    description: 'Текущий тариф, история платежей, лимиты.',
    meta: 'Pro · до 12 июня', tone: 'rose' },
  { id: 'audit', name: 'Журнал действий', icon: 'FileText',
    description: 'История изменений и действий пользователей.',
    meta: '247 за неделю', tone: 'slate' },
];

const TONE_BG = {
  indigo: { bg: 'rgba(79,70,229,0.10)', fg: '#4338ca' },
  violet: { bg: 'rgba(139,92,246,0.10)', fg: '#6d28d9' },
  amber:  { bg: 'rgba(245,158,11,0.12)', fg: '#92400e' },
  emerald:{ bg: 'rgba(16,185,129,0.12)', fg: '#047857' },
  rose:   { bg: 'rgba(244,63,94,0.10)',  fg: '#be123c' },
  slate:  { bg: 'rgba(100,116,139,0.10)',fg: '#475569' },
};

// ── Tweaks defaults ───────────────────────────────────────────────────
const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "directoriesLayout": "grid",
  "density": "comfortable",
  "showSearch": true,
  "showDirCounts": true,
  "showLastEdited": true,
  "showPlatform": true
}/*EDITMODE-END*/;

// ── Главный компонент ─────────────────────────────────────────────────
function SettingsApp() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);
  const [query, setQuery] = useS('');

  const q = query.trim().toLowerCase();
  const matches = (s) => !q || (s || '').toLowerCase().includes(q);

  const dirsFiltered = useM(() =>
    DIRECTORIES.filter(d => matches(d.name) || matches(d.description)),
  [q]);

  const platformFiltered = useM(() =>
    PLATFORM.filter(p => matches(p.name) || matches(p.description)),
  [q]);

  const orgMatches = matches('организация') || matches(ORG_PREVIEW.shortName) || matches(ORG_PREVIEW.fullName);
  const rolesMatch = matches('роли') || matches('доступы') || matches('права');

  const sectionVisible = {
    org: !q || orgMatches,
    directories: !q || dirsFiltered.length > 0,
    access: !q || rolesMatch,
    platform: t.showPlatform && (!q || platformFiltered.length > 0),
  };
  const anyVisible = sectionVisible.org || sectionVisible.directories || sectionVisible.access || sectionVisible.platform;

  return (
    <div style={{ display: 'flex', height: '100vh', minHeight: 700, background: '#f8fafc', overflow: 'hidden' }}>
      <Sidebar active="settings" onNavigate={navigate}/>
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Breadcrumb */}
        <div style={{
          padding: '14px 32px', borderBottom: '1px solid #e2e8f0',
          background: '#fff', display: 'flex', alignItems: 'center', gap: 10,
          fontSize: 13, color: '#64748b',
        }}>
          <span>{ORG_PREVIEW.shortName}</span>
          <Icon.ChevronRight size={14} stroke="#cbd5e1"/>
          <span style={{ color: '#0f172a', fontWeight: 500 }}>Настройки</span>
        </div>

        {/* Header */}
        <div style={{
          padding: '24px 32px 22px', borderBottom: '1px solid #e2e8f0',
          background: '#fff',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 18, marginBottom: t.showSearch ? 18 : 0 }}>
            <div style={{
              width: 48, height: 48, borderRadius: 12, flexShrink: 0,
              background: 'rgba(79,70,229,0.10)', color: '#4338ca',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}>
              <Icon.Settings size={24}/>
            </div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, letterSpacing: '-0.02em' }}>
                Настройки
              </h1>
              <div style={{ fontSize: 13.5, color: '#64748b', marginTop: 2 }}>
                Управление организацией, справочниками и подключёнными сервисами
              </div>
            </div>
          </div>
          {t.showSearch && (
            <div style={{ position: 'relative', maxWidth: 480 }}>
              <Icon.Search size={16} stroke="#94a3b8"
                style={{ position: 'absolute', left: 13, top: 12, pointerEvents: 'none' }}/>
              <input
                value={query} onChange={e => setQuery(e.target.value)}
                placeholder="Поиск по настройкам — справочники, разделы, опции…"
                style={{
                  width: '100%', height: 40, paddingLeft: 38, paddingRight: 12,
                  borderRadius: 10, border: '1px solid #e2e8f0', background: '#fff',
                  fontSize: 14, fontFamily: 'inherit', outline: 'none',
                }}
                onFocus={e => { e.target.style.borderColor = '#6366f1'; e.target.style.boxShadow = '0 0 0 3px rgba(99,102,241,0.18)'; }}
                onBlur={e => { e.target.style.borderColor = '#e2e8f0'; e.target.style.boxShadow = 'none'; }}
              />
              {query && (
                <button onClick={() => setQuery('')}
                  style={{
                    position: 'absolute', right: 8, top: 8, width: 24, height: 24,
                    borderRadius: 6, border: 0, background: 'transparent',
                    color: '#94a3b8', display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                  }}
                  onMouseEnter={e => e.currentTarget.style.background = '#f1f5f9'}
                  onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
                  <Icon.X size={14}/>
                </button>
              )}
            </div>
          )}
        </div>

        {/* Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px 32px 64px' }}>
          <div style={{ maxWidth: 1180, margin: '0 auto', display: 'flex', flexDirection: 'column', gap: 32 }}>

            {sectionVisible.org && <OrgSection/>}

            {sectionVisible.directories && (
              <DirectoriesSection
                items={dirsFiltered}
                layout={t.directoriesLayout}
                density={t.density}
                showCounts={t.showDirCounts}
                showLastEdited={t.showLastEdited}
                query={query}
              />
            )}

            {sectionVisible.access && <AccessSection/>}

            {sectionVisible.platform && (
              <PlatformSection items={platformFiltered} density={t.density}/>
            )}

            {!anyVisible && (
              <div style={{
                background: '#fff', border: '1px dashed #cbd5e1', borderRadius: 14,
                padding: '56px 24px', textAlign: 'center',
              }}>
                <div style={{
                  width: 56, height: 56, borderRadius: 14, margin: '0 auto 16px',
                  background: 'rgba(148,163,184,0.12)', color: '#64748b',
                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  <Icon.Search size={26}/>
                </div>
                <div style={{ fontSize: 16, fontWeight: 600, color: '#0f172a', marginBottom: 6 }}>
                  Ничего не найдено
                </div>
                <div style={{ fontSize: 13.5, color: '#64748b' }}>
                  По запросу «{query}» нет совпадений. Попробуйте другие слова.
                </div>
              </div>
            )}

          </div>
        </div>
      </div>

      <TweaksPanel title="Tweaks">
        <TweakSection label="Справочники">
          <TweakRadio label="Раскладка" value={t.directoriesLayout}
            onChange={v => setTweak('directoriesLayout', v)}
            options={[
              { value: 'grid', label: 'Сетка' },
              { value: 'list', label: 'Список' },
            ]}/>
          <TweakToggle label="Счётчики записей" value={t.showDirCounts} onChange={v => setTweak('showDirCounts', v)}/>
          <TweakToggle label="Когда меняли"       value={t.showLastEdited} onChange={v => setTweak('showLastEdited', v)}/>
        </TweakSection>
        <TweakSection label="Страница">
          <TweakRadio label="Плотность" value={t.density}
            onChange={v => setTweak('density', v)}
            options={[
              { value: 'compact',     label: 'Компактно' },
              { value: 'comfortable', label: 'Стандарт' },
            ]}/>
          <TweakToggle label="Поиск в шапке" value={t.showSearch} onChange={v => setTweak('showSearch', v)}/>
          <TweakToggle label="Раздел «Платформа»" value={t.showPlatform} onChange={v => setTweak('showPlatform', v)}/>
        </TweakSection>
      </TweaksPanel>
    </div>
  );
}

// ── Маршрутизация по сайдбару ─────────────────────────────────────────
function navigate(id) {
  const routes = {
    dashboard: null,
    students: null,
    groups: 'Groups.html',
    courses: 'Courses.html',
    schedule: null,
    attendance: 'Attendance.html',
    profiles: 'Organization Members.html',
    org: 'Organization Edit.html',
    settings: 'Settings.html',
  };
  const href = routes[id];
  if (href) window.location.href = href;
}

// ── Заголовок секции ──────────────────────────────────────────────────
function SectionHeader({ icon, title, subtitle, action }) {
  const IC = Icon[icon];
  return (
    <div style={{ display: 'flex', alignItems: 'flex-end', gap: 16, marginBottom: 14 }}>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{
          display: 'inline-flex', alignItems: 'center', gap: 8,
          fontSize: 11, fontWeight: 600, letterSpacing: '0.08em',
          textTransform: 'uppercase', color: '#64748b', marginBottom: 4,
        }}>
          <IC size={13} stroke="#64748b"/>
          {title}
        </div>
        {subtitle && (
          <div style={{ fontSize: 13.5, color: '#94a3b8' }}>{subtitle}</div>
        )}
      </div>
      {action}
    </div>
  );
}

// ── ОРГАНИЗАЦИЯ ──────────────────────────────────────────────────────
function OrgSection() {
  return (
    <section>
      <SectionHeader
        icon="Building2"
        title="Организация"
        subtitle="Юридические данные, контакты и фирменные документы"
      />
      <div style={{
        background: '#fff', border: '1px solid #e2e8f0', borderRadius: 16,
        overflow: 'hidden',
        boxShadow: '0 1px 3px rgba(15,23,42,0.04)',
      }}>
        <div style={{
          padding: 24, display: 'flex', alignItems: 'center', gap: 20,
          background: 'linear-gradient(135deg, rgba(99,102,241,0.05), rgba(139,92,246,0.04))',
          borderBottom: '1px solid #f1f5f9',
        }}>
          <div style={{
            width: 60, height: 60, borderRadius: 14, flexShrink: 0,
            background: 'linear-gradient(135deg, #6366f1, #8b5cf6)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            color: '#fff', fontSize: 22, fontWeight: 700,
            boxShadow: '0 4px 14px rgba(99,102,241,0.3)',
          }}>
            {ORG_PREVIEW.shortName.replace(/[«»"]/g,'').trim().charAt(0)}
          </div>
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 4 }}>
              <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, letterSpacing: '-0.02em', color: '#0f172a' }}>
                {ORG_PREVIEW.shortName}
              </h2>
              <Badge variant="primary">{ORG_PREVIEW.legalForm}</Badge>
            </div>
            <div style={{ fontSize: 13, color: '#64748b' }}>
              {ORG_PREVIEW.fullName}
            </div>
          </div>
          <a href="Organization Edit.html" style={{ flexShrink: 0 }}>
            <Button variant="primary">
              Редактировать
              <Icon.ArrowRight size={15}/>
            </Button>
          </a>
        </div>

        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(4, 1fr)',
          gap: 0,
        }}>
          <OrgStat label="Тип организации"   value={ORG_PREVIEW.type}/>
          <OrgStat label="Дата регистрации"  value={ORG_PREVIEW.registrationDate}/>
          <OrgStat label="Основной контакт"  value={ORG_PREVIEW.contact} mono/>
          <OrgStat label="Изменено"          value={ORG_PREVIEW.lastEditedAt} hint={ORG_PREVIEW.lastEditedBy}/>
        </div>

        <div style={{
          borderTop: '1px solid #f1f5f9',
          padding: '14px 24px',
          display: 'flex', alignItems: 'center', gap: 14,
          fontSize: 13, color: '#475569',
          background: '#fafbfc',
        }}>
          <Icon.Info size={15} stroke="#94a3b8"/>
          <span>Сотрудники организации</span>
          <span style={{ color: '#cbd5e1' }}>·</span>
          <a href="Organization Members.html"
            style={{ color: '#4f46e5', fontWeight: 500 }}
            onMouseEnter={e => e.currentTarget.style.textDecoration = 'underline'}
            onMouseLeave={e => e.currentTarget.style.textDecoration = 'none'}>
            Управление профилями и приглашениями →
          </a>
        </div>
      </div>
    </section>
  );
}

function OrgStat({ label, value, hint, mono }) {
  return (
    <div style={{
      padding: '16px 24px',
      borderRight: '1px solid #f1f5f9',
    }}>
      <div style={{ fontSize: 11, fontWeight: 600, color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: 6 }}>
        {label}
      </div>
      <div style={{
        fontSize: 13.5, fontWeight: 500, color: '#0f172a',
        fontFamily: mono ? 'var(--edv-font-mono, monospace)' : 'inherit',
        whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
      }}>
        {value}
      </div>
      {hint && (
        <div style={{ fontSize: 11.5, color: '#94a3b8', marginTop: 2 }}>
          {hint}
        </div>
      )}
    </div>
  );
}

// ── СПРАВОЧНИКИ ──────────────────────────────────────────────────────
function DirectoriesSection({ items, layout, density, showCounts, showLastEdited, query }) {
  return (
    <section>
      <SectionHeader
        icon="Layers"
        title="Справочники организации"
        subtitle="Наборы значений, которые используются в курсах, группах и студентах"
        action={
          <a href="#" style={{ fontSize: 13, color: '#4f46e5', fontWeight: 500 }}
            onClick={e => e.preventDefault()}>
            Импорт и экспорт →
          </a>
        }
      />
      {items.length === 0 ? (
        <EmptySearch query={query} label="справочников"/>
      ) : layout === 'grid' ? (
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
          gap: 14,
        }}>
          {items.map(d => <DirectoryCard key={d.id} dir={d} density={density}
            showCounts={showCounts} showLastEdited={showLastEdited}/>)}
        </div>
      ) : (
        <div style={{
          background: '#fff', border: '1px solid #e2e8f0', borderRadius: 14,
          overflow: 'hidden', boxShadow: '0 1px 3px rgba(15,23,42,0.04)',
        }}>
          {items.map((d, i) => (
            <DirectoryRow key={d.id} dir={d} isLast={i === items.length - 1}
              density={density} showCounts={showCounts} showLastEdited={showLastEdited}/>
          ))}
        </div>
      )}
    </section>
  );
}

function DirectoryCard({ dir, density, showCounts, showLastEdited }) {
  const IC = Icon[dir.icon] || Icon.FileText;
  const [hover, setHover] = useS(false);
  const padding = density === 'compact' ? 16 : 20;
  return (
    <a href={dir.href || '#'}
      onClick={e => { if (!dir.href) e.preventDefault(); }}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      style={{
        display: 'block',
        background: '#fff',
        border: '1px solid ' + (hover ? '#c7d2fe' : '#e2e8f0'),
        borderRadius: 14, padding,
        boxShadow: hover
          ? '0 4px 16px rgba(79,70,229,0.10), 0 0 0 1px rgba(79,70,229,0.06)'
          : '0 1px 2px rgba(15,23,42,0.03)',
        transition: 'all .15s ease',
        cursor: dir.href ? 'pointer' : 'default',
        opacity: dir.href ? 1 : 0.92,
        position: 'relative',
      }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: density === 'compact' ? 8 : 12 }}>
        <div style={{
          width: 36, height: 36, borderRadius: 10, flexShrink: 0,
          background: hover ? 'rgba(79,70,229,0.14)' : 'rgba(79,70,229,0.08)',
          color: '#4338ca',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          transition: 'background .15s',
        }}>
          <IC size={18}/>
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <h3 style={{ margin: 0, fontSize: 14.5, fontWeight: 600, color: '#0f172a', letterSpacing: '-0.01em' }}>
              {dir.name}
            </h3>
            {dir.badge && (
              <span style={{
                fontSize: 10, fontWeight: 600, textTransform: 'uppercase',
                letterSpacing: '0.06em', color: '#64748b',
                background: '#f1f5f9', padding: '2px 6px', borderRadius: 4,
              }}>{dir.badge}</span>
            )}
          </div>
        </div>
        <Icon.ChevronRight size={16} stroke={hover ? '#6366f1' : '#cbd5e1'}/>
      </div>
      <div style={{
        fontSize: 12.5, color: '#64748b', lineHeight: 1.5,
        marginBottom: showCounts || showLastEdited ? (density === 'compact' ? 10 : 14) : 0,
        minHeight: density === 'compact' ? 0 : 36,
      }}>
        {dir.description}
      </div>
      {(showCounts || showLastEdited) && (
        <div style={{
          display: 'flex', alignItems: 'center', gap: 12,
          paddingTop: density === 'compact' ? 8 : 10,
          borderTop: '1px solid #f1f5f9',
          fontSize: 12, color: '#94a3b8',
          fontVariantNumeric: 'tabular-nums',
        }}>
          {showCounts && (
            <span style={{ display: 'inline-flex', alignItems: 'baseline', gap: 4 }}>
              <strong style={{ color: '#0f172a', fontWeight: 600, fontSize: 13 }}>{dir.count}</strong>
              <span>{declension(dir.count, ['запись','записи','записей'])}</span>
              {dir.archivedCount > 0 && (
                <span style={{ color: '#cbd5e1', marginLeft: 4 }}>
                  +{dir.archivedCount} в архиве
                </span>
              )}
            </span>
          )}
          {showCounts && showLastEdited && (
            <span style={{ color: '#e2e8f0' }}>·</span>
          )}
          {showLastEdited && (
            <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
              <Icon.Clock size={11} stroke="#cbd5e1"/>
              {dir.lastEdited}
            </span>
          )}
        </div>
      )}
    </a>
  );
}

function DirectoryRow({ dir, isLast, density, showCounts, showLastEdited }) {
  const IC = Icon[dir.icon] || Icon.FileText;
  const [hover, setHover] = useS(false);
  const pad = density === 'compact' ? 12 : 16;
  return (
    <a href={dir.href || '#'}
      onClick={e => { if (!dir.href) e.preventDefault(); }}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      style={{
        display: 'grid',
        gridTemplateColumns: '36px minmax(220px, 1.6fr) 2fr auto auto 16px',
        gap: 16, alignItems: 'center',
        padding: `${pad}px 18px`,
        borderBottom: isLast ? '0' : '1px solid #f1f5f9',
        background: hover ? '#fafbfc' : '#fff',
        transition: 'background .12s',
        cursor: dir.href ? 'pointer' : 'default',
      }}>
      <div style={{
        width: 36, height: 36, borderRadius: 10,
        background: 'rgba(79,70,229,0.08)', color: '#4338ca',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <IC size={18}/>
      </div>
      <div style={{ minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <span style={{ fontSize: 14, fontWeight: 600, color: '#0f172a' }}>{dir.name}</span>
          {dir.badge && (
            <span style={{
              fontSize: 10, fontWeight: 600, textTransform: 'uppercase',
              letterSpacing: '0.06em', color: '#64748b',
              background: '#f1f5f9', padding: '2px 6px', borderRadius: 4,
            }}>{dir.badge}</span>
          )}
        </div>
      </div>
      <div style={{
        fontSize: 12.5, color: '#64748b',
        whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
      }}>
        {dir.description}
      </div>
      <div style={{ fontSize: 12.5, color: '#64748b', fontVariantNumeric: 'tabular-nums', whiteSpace: 'nowrap' }}>
        {showCounts ? (
          <>
            <strong style={{ color: '#0f172a' }}>{dir.count}</strong>
            <span style={{ color: '#94a3b8' }}> {declension(dir.count, ['запись','записи','записей'])}</span>
            {dir.archivedCount > 0 && (
              <span style={{ color: '#cbd5e1' }}> · +{dir.archivedCount} архив</span>
            )}
          </>
        ) : null}
      </div>
      <div style={{ fontSize: 12, color: '#94a3b8', whiteSpace: 'nowrap' }}>
        {showLastEdited ? dir.lastEdited : null}
      </div>
      <Icon.ChevronRight size={16} stroke={hover ? '#6366f1' : '#cbd5e1'}/>
    </a>
  );
}

// ── ДОСТУПЫ ──────────────────────────────────────────────────────────
function AccessSection() {
  return (
    <section>
      <SectionHeader
        icon="Shield"
        title="Доступы"
        subtitle="Кто что может делать в системе"
      />
      <a href="Roles.html" style={{ display: 'block', textDecoration: 'none' }}>
        <AccessCard/>
      </a>
    </section>
  );
}

function AccessCard() {
  const [hover, setHover] = useS(false);
  return (
    <div onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      style={{
        background: '#fff',
        border: '1px solid ' + (hover ? '#c7d2fe' : '#e2e8f0'),
        borderRadius: 16, padding: '20px 24px',
        display: 'flex', alignItems: 'center', gap: 18,
        boxShadow: hover
          ? '0 4px 16px rgba(79,70,229,0.10)'
          : '0 1px 3px rgba(15,23,42,0.04)',
        transition: 'all .15s',
        cursor: 'pointer',
      }}>
      <div style={{
        width: 44, height: 44, borderRadius: 12, flexShrink: 0,
        background: 'rgba(79,70,229,0.10)', color: '#4338ca',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon.Shield size={22}/>
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 3 }}>
          <h3 style={{ margin: 0, fontSize: 15, fontWeight: 600, color: '#0f172a' }}>
            Роли и права
          </h3>
          <Badge variant="default">5 ролей</Badge>
        </div>
        <div style={{ fontSize: 13, color: '#64748b' }}>
          Шаблоны прав для сотрудников: администратор, методист, преподаватель, менеджер, бухгалтер
        </div>
      </div>
      <div style={{
        display: 'flex', gap: 6, alignItems: 'center',
        fontSize: 12, color: '#64748b', whiteSpace: 'nowrap',
      }}>
        <span style={{ fontVariantNumeric: 'tabular-nums' }}>
          <strong style={{ color: '#0f172a', fontWeight: 600 }}>23</strong> сотрудника назначены
        </span>
      </div>
      <Icon.ChevronRight size={16} stroke={hover ? '#6366f1' : '#cbd5e1'}/>
    </div>
  );
}

// ── ПЛАТФОРМА ────────────────────────────────────────────────────────
function PlatformSection({ items, density }) {
  return (
    <section>
      <SectionHeader
        icon="Sparkles"
        title="Платформа"
        subtitle="Подключения, оповещения, безопасность и тариф"
      />
      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
        gap: 14,
      }}>
        {items.map(p => <PlatformCard key={p.id} item={p} density={density}/>)}
      </div>
    </section>
  );
}

function PlatformCard({ item, density }) {
  const IC = Icon[item.icon] || Icon.Settings;
  const tone = TONE_BG[item.tone] || TONE_BG.indigo;
  const [hover, setHover] = useS(false);
  const padding = density === 'compact' ? 16 : 18;
  return (
    <div onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}
      style={{
        background: '#fff',
        border: '1px solid ' + (hover ? '#e2e8f0' : '#e2e8f0'),
        borderRadius: 14, padding,
        boxShadow: hover ? '0 4px 12px rgba(15,23,42,0.05)' : '0 1px 2px rgba(15,23,42,0.03)',
        transition: 'all .15s',
        cursor: 'pointer',
        opacity: 0.95,
        position: 'relative',
      }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: density === 'compact' ? 8 : 12 }}>
        <div style={{
          width: 34, height: 34, borderRadius: 10, flexShrink: 0,
          background: tone.bg, color: tone.fg,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <IC size={17}/>
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <h3 style={{ margin: 0, fontSize: 14, fontWeight: 600, color: '#0f172a' }}>
            {item.name}
          </h3>
        </div>
        <span style={{
          fontSize: 10, fontWeight: 600, textTransform: 'uppercase',
          letterSpacing: '0.06em', color: '#94a3b8',
          background: '#f8fafc', padding: '2px 6px', borderRadius: 4,
          border: '1px dashed #e2e8f0',
        }}>скоро</span>
      </div>
      <div style={{ fontSize: 12.5, color: '#64748b', lineHeight: 1.5,
        minHeight: density === 'compact' ? 0 : 34 }}>
        {item.description}
      </div>
      {item.meta && (
        <div style={{
          marginTop: density === 'compact' ? 8 : 12,
          paddingTop: density === 'compact' ? 8 : 10,
          borderTop: '1px solid #f1f5f9',
          fontSize: 12, color: '#475569', fontWeight: 500,
        }}>
          {item.meta}
        </div>
      )}
    </div>
  );
}

// ── helpers ──────────────────────────────────────────────────────────
function EmptySearch({ query, label }) {
  return (
    <div style={{
      background: '#fff', border: '1px dashed #e2e8f0', borderRadius: 14,
      padding: '28px 24px', textAlign: 'center',
      fontSize: 13, color: '#64748b',
    }}>
      По запросу «<strong style={{ color: '#0f172a' }}>{query}</strong>» среди {label} ничего не найдено.
    </div>
  );
}

function declension(n, forms) {
  const abs = Math.abs(n);
  const m10 = abs % 10, m100 = abs % 100;
  if (m10 === 1 && m100 !== 11) return forms[0];
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return forms[1];
  return forms[2];
}

window.SettingsApp = SettingsApp;
