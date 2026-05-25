// Sample data for "Уровень" — the example reference.
// Other directories will follow the same shape: id, name, code?, color?, description?, usage, status, order.
const LEVELS = [
  {
    id: 'l-a1', name: 'A1 — Начальный', code: 'A1', color: 'sky',
    description: 'Beginner. Базовая лексика, простые фразы, бытовые ситуации.',
    usage: { groups: 12, courses: 4, students: 138 },
    status: 'active', order: 1,
  },
  {
    id: 'l-a2', name: 'A2 — Элементарный', code: 'A2', color: 'teal',
    description: 'Elementary. Уверенное использование часто употребляемых выражений.',
    usage: { groups: 18, courses: 5, students: 204 },
    status: 'active', order: 2,
  },
  {
    id: 'l-b1', name: 'B1 — Средний', code: 'B1', color: 'indigo',
    description: 'Intermediate. Чёткие тексты на знакомые темы, личный опыт.',
    usage: { groups: 22, courses: 6, students: 256 },
    status: 'active', order: 3,
  },
  {
    id: 'l-b2', name: 'B2 — Выше среднего', code: 'B2', color: 'violet',
    description: 'Upper-Intermediate. Сложные тексты, абстрактные темы, профессиональная дискуссия.',
    usage: { groups: 14, courses: 5, students: 142 },
    status: 'active', order: 4,
  },
  {
    id: 'l-c1', name: 'C1 — Продвинутый', code: 'C1', color: 'amber',
    description: 'Advanced. Свободное и спонтанное общение без подбора выражений.',
    usage: { groups: 7, courses: 3, students: 64 },
    status: 'active', order: 5,
  },
  {
    id: 'l-c2', name: 'C2 — В совершенстве', code: 'C2', color: 'rose',
    description: 'Mastery. Понимание практически любых форм языка, нюансы значений.',
    usage: { groups: 2, courses: 1, students: 12 },
    status: 'active', order: 6,
  },
  {
    id: 'l-kids', name: 'Дошкольный', code: 'PRE', color: 'pink',
    description: 'Программа 5–6 лет. Игровой формат, без письменных тестов.',
    usage: { groups: 4, courses: 2, students: 38 },
    status: 'active', order: 7,
  },
  {
    id: 'l-zero', name: 'Zero — без подготовки', code: 'A0', color: 'slate',
    description: 'Снят с использования с сентября 2024 — заменён на A1.',
    usage: { groups: 0, courses: 0, students: 0 },
    status: 'archived', order: 8,
  },
];

const COLOR_DOTS = {
  sky:    '#0ea5e9',
  teal:   '#14b8a6',
  indigo: '#6366f1',
  violet: '#8b5cf6',
  amber:  '#f59e0b',
  rose:   '#f43f5e',
  pink:   '#ec4899',
  slate:  '#94a3b8',
  emerald:'#10b981',
  blue:   '#3b82f6',
};

window.LEVELS = LEVELS;
window.COLOR_DOTS = COLOR_DOTS;
