// Attendance — журнал посещаемости одной группы (EN-B1-12)
// Контекст группы — берём фактически из window.GROUPS, но фиксируем для удобства
const ATT_GROUP = {
  id: 1,
  code: 'EN-B1-12',
  name: 'English Intermediate · вечерняя',
  level: 'B1',
  course: 'General English',
  teacher: 'Петров А. Н.',
  format: 'offline',
  room: 'Каб. 204',
  schedule: 'Пн / Ср · 18:00–19:30',
  capacity: 12,
  startsAt: '02.09.2025',
};

// Состояния отметки
const ATT_STATUSES = {
  present:  { code: 'present',  short: 'П',  label: 'Присутствовал',
              bg: 'rgba(16,185,129,0.14)', bgStrong: '#10b981',
              fg: '#047857', dot: '#10b981', icon: 'Check' },
  absent:   { code: 'absent',   short: 'Н',  label: 'Отсутствовал',
              bg: 'rgba(239,68,68,0.12)',  bgStrong: '#ef4444',
              fg: '#b91c1c', dot: '#ef4444', icon: 'X' },
  late:     { code: 'late',     short: 'О',  label: 'Опоздал',
              bg: 'rgba(245,158,11,0.16)', bgStrong: '#f59e0b',
              fg: '#92400e', dot: '#f59e0b', icon: 'Clock' },
  excused:  { code: 'excused',  short: 'Б',  label: 'Уваж. причина',
              bg: 'rgba(99,102,241,0.10)', bgStrong: '#6366f1',
              fg: '#4338ca', dot: '#6366f1', icon: 'Shield' },
  none:     { code: 'none',     short: '·',  label: 'Не отмечен',
              bg: 'transparent',            bgStrong: '#cbd5e1',
              fg: '#94a3b8', dot: '#cbd5e1', icon: null },
  cancelled:{ code: 'cancelled',short: '—',  label: 'Отменено',
              bg: 'repeating-linear-gradient(45deg,#f8fafc,#f8fafc 4px,#f1f5f9 4px,#f1f5f9 8px)',
              bgStrong: '#94a3b8',
              fg: '#64748b', dot: '#94a3b8', icon: null },
};

// Порядок цикла при клике (cancelled пропускаем — это статус занятия)
const ATT_CYCLE = ['none','present','absent','late','excused'];

// Студенты группы (11 из 12 — есть свободное место)
const ATT_STUDENTS = [
  { id: 1,  name: 'Алексеев Кирилл',     phone: '+7 905 421-08-19', parent: null,                    age: 28 },
  { id: 2,  name: 'Бородина Мария',      phone: '+7 916 244-37-92', parent: null,                    age: 24 },
  { id: 3,  name: 'Васильев Артём',      phone: '+7 926 188-04-55', parent: null,                    age: 31 },
  { id: 4,  name: 'Григорьева Светлана', phone: '+7 903 712-50-08', parent: null,                    age: 26 },
  { id: 5,  name: 'Демин Никита',        phone: '+7 999 405-66-31', parent: null,                    age: 22 },
  { id: 6,  name: 'Ерёмина Анастасия',   phone: '+7 985 332-21-77', parent: null,                    age: 29 },
  { id: 7,  name: 'Жуков Андрей',        phone: '+7 916 504-99-12', parent: null,                    age: 34 },
  { id: 8,  name: 'Зайцева Елена',       phone: '+7 925 117-43-08', parent: null,                    age: 33 },
  { id: 9,  name: 'Никитина Полина',     phone: '+7 985 332-08-19', parent: 'мама — Никитина О. В.', age: 16 },
  { id: 10, name: 'Соколов Артём',       phone: '+7 916 077-30-44', parent: 'папа — Соколов В. И.',  age: 15 },
  { id: 11, name: 'Хохлова Елизавета',   phone: '+7 985 122-04-37', parent: null,                    age: 27 },
];

// Даты занятий с начала курса. Шаблон Пн/Ср, длительность 90 мин.
// Сегодня — 14.05.2026 (Чт). Сегодняшнее ближайшее занятие — Ср 13.05.
// Берём ~6 недель назад до конца следующей.
// Каждая запись: { date 'DD.MM', weekday, weekIdx, isFuture, isToday, isCancelled?, reason? }
const ATT_LESSONS = [
  // Апрель
  { id: 1,  date: '01.04', d: 1,  full: 'Среда, 1 апреля',       weekIdx: 1, isFuture: false, topic: 'Unit 5 · Present Perfect Continuous' },
  { id: 2,  date: '06.04', d: 6,  full: 'Понедельник, 6 апреля', weekIdx: 2, isFuture: false, topic: 'Unit 5 · Reading + vocab' },
  { id: 3,  date: '08.04', d: 8,  full: 'Среда, 8 апреля',       weekIdx: 2, isFuture: false, topic: 'Unit 5 · Speaking' },
  { id: 4,  date: '13.04', d: 13, full: 'Понедельник, 13 апреля',weekIdx: 3, isFuture: false, topic: 'Unit 5 · Writing task' },
  { id: 5,  date: '15.04', d: 15, full: 'Среда, 15 апреля',      weekIdx: 3, isFuture: false, topic: 'Unit 5 · Test' },
  { id: 6,  date: '20.04', d: 20, full: 'Понедельник, 20 апреля',weekIdx: 4, isFuture: false, topic: 'Unit 6 · Conditionals' },
  { id: 7,  date: '22.04', d: 22, full: 'Среда, 22 апреля',      weekIdx: 4, isFuture: false, topic: 'Unit 6 · Listening' },
  { id: 8,  date: '27.04', d: 27, full: 'Понедельник, 27 апреля',weekIdx: 5, isFuture: false, topic: 'Unit 6 · Discussion' },
  { id: 9,  date: '29.04', d: 29, full: 'Среда, 29 апреля',      weekIdx: 5, isFuture: false, topic: 'Unit 6 · Speaking practice' },
  // Май
  { id: 10, date: '04.05', d: 4,  full: 'Понедельник, 4 мая',    weekIdx: 6, isFuture: false, isCancelled: true,
    reason: 'Майские праздники', topic: 'Праздничный день' },
  { id: 11, date: '06.05', d: 6,  full: 'Среда, 6 мая',          weekIdx: 6, isFuture: false, topic: 'Unit 6 · Mini-project' },
  { id: 12, date: '11.05', d: 11, full: 'Понедельник, 11 мая',   weekIdx: 7, isFuture: false, topic: 'Unit 7 · Reported speech' },
  { id: 13, date: '13.05', d: 13, full: 'Среда, 13 мая',         weekIdx: 7, isFuture: false, topic: 'Unit 7 · Practice', isJustHappened: true },
  // будущие — пустые колонки
  { id: 14, date: '18.05', d: 18, full: 'Понедельник, 18 мая',   weekIdx: 8, isFuture: true,  topic: 'Unit 7 · Speaking' },
  { id: 15, date: '20.05', d: 20, full: 'Среда, 20 мая',         weekIdx: 8, isFuture: true,  topic: 'Unit 7 · Test' },
];

// Журнал — мапа { studentId: { lessonId: statusCode } }
// Подбираем правдоподобно: средняя посещаемость ~85%, есть «проблемные» студенты
const ATT_LOG = {
  1: {  // Алексеев Кирилл — почти идеально
    1:'present',2:'present',3:'present',4:'present',5:'present',6:'late',
    7:'present',8:'present',9:'present',10:'cancelled',11:'present',12:'present',13:'present' },
  2: {  // Бородина Мария
    1:'present',2:'present',3:'late',4:'present',5:'present',6:'present',
    7:'absent',8:'present',9:'present',10:'cancelled',11:'present',12:'late',13:'present' },
  3: {  // Васильев Артём
    1:'present',2:'absent',3:'absent',4:'excused',5:'excused',6:'present',
    7:'present',8:'present',9:'present',10:'cancelled',11:'present',12:'present',13:'present' },
  4: {  // Григорьева Светлана — отличница
    1:'present',2:'present',3:'present',4:'present',5:'present',6:'present',
    7:'present',8:'present',9:'present',10:'cancelled',11:'present',12:'present',13:'present' },
  5: {  // Демин Никита — проблемный
    1:'absent',2:'present',3:'absent',4:'late',5:'present',6:'absent',
    7:'late',8:'absent',9:'present',10:'cancelled',11:'absent',12:'late',13:'absent' },
  6: {  // Ерёмина Анастасия
    1:'present',2:'present',3:'present',4:'late',5:'present',6:'present',
    7:'present',8:'late',9:'present',10:'cancelled',11:'present',12:'present',13:'present' },
  7: {  // Жуков Андрей — болел
    1:'present',2:'present',3:'present',4:'present',5:'excused',6:'excused',
    7:'excused',8:'present',9:'present',10:'cancelled',11:'present',12:'present',13:'present' },
  8: {  // Зайцева Елена
    1:'present',2:'present',3:'present',4:'present',5:'present',6:'present',
    7:'present',8:'present',9:'present',10:'cancelled',11:'present',12:'present',13:'present' },
  9: {  // Никитина Полина — подросток
    1:'present',2:'late',3:'present',4:'present',5:'present',6:'absent',
    7:'present',8:'present',9:'late',10:'cancelled',11:'present',12:'present',13:'present' },
  10: { // Соколов Артём — подросток, есть пропуски
    1:'present',2:'absent',3:'present',4:'present',5:'absent',6:'present',
    7:'present',8:'absent',9:'present',10:'cancelled',11:'late',12:'present',13:'none' },
  11: { // Хохлова Елизавета — добавилась позже
    1:'none',2:'none',3:'none',4:'none',5:'present',6:'present',
    7:'present',8:'present',9:'present',10:'cancelled',11:'present',12:'late',13:'present' },
};

// Комментарии к отдельным отметкам (на лету)
const ATT_NOTES = {
  '3-2': 'Семейные обстоятельства',
  '3-3': 'Без предупреждения',
  '5-1': 'Не пришёл',
  '5-13':'Третий пропуск подряд',
  '7-5': 'Справка от врача',
  '7-6': 'Справка от врача',
  '7-7': 'Справка от врача',
  '10-2':'Соревнования по плаванию',
};

window.ATT_GROUP = ATT_GROUP;
window.ATT_STATUSES = ATT_STATUSES;
window.ATT_CYCLE = ATT_CYCLE;
window.ATT_STUDENTS = ATT_STUDENTS;
window.ATT_LESSONS = ATT_LESSONS;
window.ATT_LOG = ATT_LOG;
window.ATT_NOTES = ATT_NOTES;
