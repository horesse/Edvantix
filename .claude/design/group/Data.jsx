// Group dashboard data — обзор одной группы (EN-B1-12)
// Сегодня — четверг, 14 мая 2026. Следующее занятие — пн 18.05.
const GRP = {
  id: 1,
  code: 'EN-B1-12',
  name: 'English Intermediate · вечерняя',
  description: 'Вечерняя группа взрослых для уверенного овладения английским уровня B1. Фокус на разговорной практике и подготовке к B2.',
  level: 'B1',
  levelFull: 'B1 · Средний',
  course: 'General English',
  courseCode: 'COURSE-GE-B1',
  teacher: { name: 'Петров А. Н.', role: 'Старший преподаватель', email: 'a.petrov@school.ru', phone: '+7 916 421-50-08', yearsAtSchool: 6 },
  format: 'offline',
  formatLabel: 'Очно',
  room: 'Каб. 204',
  schedule: 'Пн / Ср · 18:00–19:30',
  weekdayLabels: ['пн', 'ср'],
  timeLabel: '18:00 – 19:30',
  duration: '90 мин',
  starts: '02.09.2025',
  ends: '29.05.2026',
  status: 'Active',
  capacity: 12,
  students: 11,
  freeSeats: 1,
  // Progress
  totalLessons: 56,
  completedLessons: 13,
  upcomingLessons: 43,
  cancelledLessons: 1,
  // Attendance (за всё время; кроме отменённого занятия)
  attendanceRate: 0.87,     // 87% присутствие
  attendanceDelta: 0.03,    // +3 п.п. за 4 недели
  lateRate: 0.06,
  excusedRate: 0.04,
  absentRate: 0.07,
  // Grades
  avgGrade: 4.4,
  avgGradeDelta: 0.1,
  // Finance — все суммы в ₽
  monthlyFee: 8800,
  finance: {
    paid: 79200,       // 9 студентов оплатили май
    expected: 17600,   // 2 студента ожидаем
    overdue: 8800,     // 1 студент просрочка
    paidCount: 9,
    expectedCount: 2,
    overdueCount: 1,
  },
};

// Учебный план — юниты курса
const GRP_PROGRAM = [
  { id: 1, code: 'Unit 1', title: 'Present tenses overview',  lessons: 6, done: 6, status: 'done' },
  { id: 2, code: 'Unit 2', title: 'Past tenses & narrative',  lessons: 6, done: 6, status: 'done' },
  { id: 3, code: 'Unit 3', title: 'Future forms',             lessons: 6, done: 6, status: 'done' },
  { id: 4, code: 'Unit 4', title: 'Modal verbs',              lessons: 6, done: 6, status: 'done' },
  { id: 5, code: 'Unit 5', title: 'Present Perfect Continuous', lessons: 5, done: 5, status: 'done' },
  { id: 6, code: 'Unit 6', title: 'Conditionals',             lessons: 6, done: 4, status: 'current' },
  { id: 7, code: 'Unit 7', title: 'Reported speech',          lessons: 6, done: 0, status: 'next' },
  { id: 8, code: 'Unit 8', title: 'Passive voice',            lessons: 5, done: 0, status: 'planned' },
  { id: 9, code: 'Unit 9', title: 'Articles & determiners',   lessons: 5, done: 0, status: 'planned' },
  { id:10, code: 'Unit 10',title: 'Final review + exam',      lessons: 5, done: 0, status: 'planned' },
];

// Посещаемость по неделям — 7 недель, последняя — текущая (неполная)
const GRP_WEEKS = [
  { id: 1, label: '01–05.04', present: 90, late: 5,  absent: 5,  lessons: 2 },
  { id: 2, label: '08–12.04', present: 82, late: 9,  absent: 9,  lessons: 2 },
  { id: 3, label: '15–19.04', present: 86, late: 5,  absent: 9,  lessons: 2 },
  { id: 4, label: '22–26.04', present: 91, late: 4,  absent: 5,  lessons: 2 },
  { id: 5, label: '29.04–03.05', present: 77, late: 14, absent: 9,  lessons: 2 },
  { id: 6, label: '06–10.05', present: 88, late: 6,  absent: 6,  lessons: 1, note: '1 урок отменён' },
  { id: 7, label: '13–17.05', present: 95, late: 5,  absent: 0,  lessons: 1, isCurrent: true },
];

// Студенты группы — копируем имена из attendance/Data.jsx и добавляем агрегаты
const GRP_STUDENTS = [
  { id:1,  name:'Алексеев Кирилл',    role:'взрослый',  attendance:0.92, grade:4.7, balance: 0,     trend:'up',    spark:[1,1,1,1,1,2,1,1,1,1,1,1] },
  { id:2,  name:'Бородина Мария',     role:'взрослый',  attendance:0.85, grade:4.5, balance: 0,     trend:'flat',  spark:[1,1,2,1,1,1,0,1,1,1,1,2] },
  { id:3,  name:'Васильев Артём',     role:'взрослый',  attendance:0.69, grade:3.9, balance: -8800, trend:'down',  spark:[1,0,0,3,3,1,1,1,1,1,1,1] },
  { id:4,  name:'Григорьева Светлана',role:'взрослый',  attendance:1.00, grade:5.0, balance: 0,     trend:'flat',  spark:[1,1,1,1,1,1,1,1,1,1,1,1] },
  { id:5,  name:'Демин Никита',       role:'взрослый',  attendance:0.46, grade:3.2, balance: 0,     trend:'down',  spark:[0,1,0,2,1,0,2,0,1,0,2,0] },
  { id:6,  name:'Ерёмина Анастасия',  role:'взрослый',  attendance:0.88, grade:4.6, balance: 0,     trend:'up',    spark:[1,1,1,2,1,1,1,2,1,1,1,1] },
  { id:7,  name:'Жуков Андрей',       role:'взрослый',  attendance:0.92, grade:4.4, balance: 0,     trend:'flat',  spark:[1,1,1,1,3,3,3,1,1,1,1,1], note:'болел 2 недели' },
  { id:8,  name:'Зайцева Елена',      role:'взрослый',  attendance:1.00, grade:4.9, balance: 0,     trend:'up',    spark:[1,1,1,1,1,1,1,1,1,1,1,1] },
  { id:9,  name:'Никитина Полина',    role:'подросток', attendance:0.85, grade:4.3, balance: 0,     trend:'flat',  spark:[1,2,1,1,1,0,1,1,2,1,1,1] },
  { id:10, name:'Соколов Артём',      role:'подросток', attendance:0.69, grade:3.8, balance: -8800, trend:'down',  spark:[1,0,1,1,0,1,1,0,1,2,1,9] },
  { id:11, name:'Хохлова Елизавета',  role:'взрослый',  attendance:0.86, grade:4.5, balance: 0,     trend:'flat',  spark:[9,9,9,9,1,1,1,1,1,1,2,1], note:'присоединилась позже' },
];
// spark: 1=present, 2=late, 0=absent, 3=excused, 9=not-in-group-yet

// Студенты под риском — рассчитываются автоматически, но фиксируем для UI
const GRP_AT_RISK = [
  { id:5,  reason:'Посещаемость 46% (4 пропуска подряд)', severity:'high' },
  { id:3,  reason:'Долг по оплате · ₽8 800',              severity:'medium' },
  { id:10, reason:'Долг по оплате · ₽8 800',              severity:'medium' },
];

// События / активность — последние 7 дней
const GRP_ACTIVITY = [
  { id:1, when:'сегодня, 11:42', icon:'Megaphone', tone:'primary',
    text:'Преподаватель опубликовал материалы к Unit 7 — Reported speech.',
    actor:'Петров А. Н.' },
  { id:2, when:'сегодня, 09:15', icon:'CreditCard', tone:'warning',
    text:'Соколов Артём: оплата за май просрочена на 4 дня. Отправлено напоминание.',
    actor:'Авто-уведомление' },
  { id:3, when:'вчера, 19:48',   icon:'ClipboardCheck', tone:'success',
    text:'Журнал посещаемости за 13.05 заполнен — 10 присутствуют, 1 опоздание.',
    actor:'Петров А. Н.' },
  { id:4, when:'13.05, 17:30',   icon:'Sparkles', tone:'success',
    text:'Григорьева Светлана сдала контрольную по Unit 5 — оценка 5.',
    actor:'Петров А. Н.' },
  { id:5, when:'12.05, 14:22',   icon:'UserCheck', tone:'primary',
    text:'Хохлова Елизавета зачислена в группу на свободное место.',
    actor:'Мельникова А. (адм.)' },
  { id:6, when:'11.05, 10:08',   icon:'CalendarDays', tone:'default',
    text:'Занятие 04.05 (Майские праздники) отмечено как отменённое.',
    actor:'Мельникова А. (адм.)' },
];

// Следующее занятие
const GRP_NEXT_LESSON = {
  id: 14,
  date: '18.05.2026',
  weekday: 'понедельник',
  startsAt: '18:00',
  endsAt: '19:30',
  duration: 90,
  unit: 'Unit 7',
  topic: 'Reported speech — practice',
  room: 'Каб. 204',
  format: 'offline',
  homework: 'Workbook p. 48–49, упр. 3–7',
  daysAway: 4,    // считается от 14.05
  hoursAway: 4 * 24 + 4, // ~100 ч
};

// Последние уроки
const GRP_RECENT_LESSONS = [
  { id:13, date:'13.05', weekday:'ср', unit:'Unit 7', topic:'Reported speech — intro', present:10, total:11, late:1, absent:0, status:'done' },
  { id:12, date:'11.05', weekday:'пн', unit:'Unit 7', topic:'Reported speech — overview', present:10, total:11, late:0, absent:1, status:'done' },
  { id:11, date:'06.05', weekday:'ср', unit:'Unit 6', topic:'Mini-project presentations', present:9, total:11, late:1, absent:1, status:'done' },
  { id:10, date:'04.05', weekday:'пн', unit:'—',     topic:'Майские праздники', present:0, total:0, late:0, absent:0, status:'cancelled' },
  { id:9,  date:'29.04', weekday:'ср', unit:'Unit 6', topic:'Speaking practice',     present:10, total:11, late:1, absent:0, status:'done' },
  { id:8,  date:'27.04', weekday:'пн', unit:'Unit 6', topic:'Discussion',            present:10, total:11, late:0, absent:1, status:'done' },
];

window.GRP = GRP;
window.GRP_PROGRAM = GRP_PROGRAM;
window.GRP_WEEKS = GRP_WEEKS;
window.GRP_STUDENTS = GRP_STUDENTS;
window.GRP_AT_RISK = GRP_AT_RISK;
window.GRP_ACTIVITY = GRP_ACTIVITY;
window.GRP_NEXT_LESSON = GRP_NEXT_LESSON;
window.GRP_RECENT_LESSONS = GRP_RECENT_LESSONS;
