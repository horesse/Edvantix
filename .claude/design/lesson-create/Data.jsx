// Lesson create — данные формы и библиотека типов блоков.
// Берём типы уроков/блоков из существующего course/Data.jsx; здесь добавляем
// расширенную инфу о блоках — описание, рекомендованная длительность и т.п.

// Расширение BLOCK_TYPES: к каждому типу — описание (что это в плеере) и
// рекомендованная длительность. Иконка и label наследуются из глобального
// window.BLOCK_TYPES (загружается раньше — см. Course.html ordering ниже).
const BLOCK_LIBRARY = [
  { type: 'intro',     description: 'Короткое введение — цели и план занятия',     defaultMin: 5,  category: 'theory' },
  { type: 'theory',    description: 'Грамматика, правила, объяснение с примерами', defaultMin: 18, category: 'theory' },
  { type: 'video',     description: 'Видео-объяснение преподавателя или с YouTube',defaultMin: 8,  category: 'theory' },
  { type: 'exercise',  description: 'Упражнения с автопроверкой — fill-in, match', defaultMin: 12, category: 'practice' },
  { type: 'speaking',  description: 'Pair-work, ролевая, обсуждение',              defaultMin: 15, category: 'practice' },
  { type: 'listening', description: 'Аудио и вопросы на понимание',                defaultMin: 14, category: 'practice' },
  { type: 'writing',   description: 'Письменное задание с обратной связью',        defaultMin: 16, category: 'practice' },
  { type: 'quiz',      description: 'Короткий контроль — 5–10 вопросов',           defaultMin: 7,  category: 'assess' },
  { type: 'homework',  description: 'Что сделать дома — ссылки, приложение',       defaultMin: 5,  category: 'assess' },
];

const BLOCK_CATEGORIES = {
  theory:   { label: 'Теория',     fg: '#4338ca' },
  practice: { label: 'Практика',   fg: '#0369a1' },
  assess:   { label: 'Проверка',   fg: '#b45309' },
};

// Modules available — берём имена/номера из COURSE MODULES.
// В реальной реализации это будет передано пропсом или подгружено,
// здесь — используем существующий window.MODULES.
const MODULES_AS_OPTIONS = () => window.MODULES.map(m => ({
  value: m.id,
  label: `Модуль ${m.n}. ${m.name}`,
  n: m.n,
  lessonCount: m.lessons.length,
  weeks: m.weeks,
  summary: m.summary,
}));

// Группы курса для секции видимости — берём из window.COURSE_GROUPS
const ALL_GROUPS_AS_OPTIONS = () => window.COURSE_GROUPS.map(g => ({
  value: g.id, label: g.name, students: g.students, progress: g.progress,
}));

// Шаблоны уроков — стартовая структура из 5-7 блоков под выбранный тип.
const LESSON_TEMPLATES = {
  lecture:   ['intro','theory','video','exercise','quiz','homework'],
  practice:  ['intro','theory','exercise','exercise','quiz','homework'],
  speaking:  ['intro','theory','speaking','speaking','quiz'],
  listening: ['intro','listening','listening','exercise','quiz','homework'],
  writing:   ['intro','theory','writing','exercise','homework'],
  test:      ['intro','quiz','quiz','exercise'],
  review:    ['intro','theory','exercise','exercise','speaking','quiz','homework'],
};

const BLOCK_DEFAULT_TITLES = {
  intro:     'Цели и план занятия',
  theory:    'Объяснение темы',
  video:     'Видео-объяснение',
  exercise:  'Упражнение на отработку',
  speaking:  'Pair-work · обсуждение',
  listening: 'Аудио и вопросы',
  writing:   'Письменное задание',
  quiz:      'Мини-квиз',
  homework:  'Домашняя работа',
};

// Стартовое состояние формы — пустой урок в модуле 4 (где есть planned уроки).
// Длительность и дата живут на уровне курса/расписания, а не урока.
function makeInitialLesson() {
  const initialType = 'practice';
  const blockTypes = LESSON_TEMPLATES[initialType];
  return {
    moduleId: 'm4',
    title: '',
    type: initialType,
    status: 'draft',
    objectives: [''],
    blocks: blockTypes.map((t, i) => ({
      id: `b${Date.now()}-${i}`,
      type: t,
      title: BLOCK_DEFAULT_TITLES[t],
      durationMin: BLOCK_LIBRARY.find(b => b.type === t).defaultMin,
      notes: '',
      links: [],
      reference: '',
    })),
    materials: [
      { id: 'mat1', kind: 'pdf',  name: 'Раздаточный материал.pdf', size: '420 КБ' },
      { id: 'mat2', kind: 'link', name: 'BBC Learning English · interview tips', url: 'bbc.co.uk/learningenglish' },
    ],
  };
}

function makeNewBlock(type) {
  const meta = BLOCK_LIBRARY.find(b => b.type === type);
  return {
    id: `b${Date.now()}-${Math.random().toString(36).slice(2,6)}`,
    type,
    title: BLOCK_DEFAULT_TITLES[type],
    durationMin: meta.defaultMin,
    notes: '',
    links: [],
    reference: '',
  };
}

window.BLOCK_LIBRARY = BLOCK_LIBRARY;
window.BLOCK_CATEGORIES = BLOCK_CATEGORIES;
window.MODULES_AS_OPTIONS = MODULES_AS_OPTIONS;
window.ALL_GROUPS_AS_OPTIONS = ALL_GROUPS_AS_OPTIONS;
window.LESSON_TEMPLATES = LESSON_TEMPLATES;
window.BLOCK_DEFAULT_TITLES = BLOCK_DEFAULT_TITLES;
window.makeInitialLesson = makeInitialLesson;
window.makeNewBlock = makeNewBlock;
