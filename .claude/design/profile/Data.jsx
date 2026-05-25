// Member work profile data — worker, not account.
// All fields below describe employment/role inside the organization.

const PROFILE = {
  id: 5,
  // Identity
  name: 'Коваленко Наталья Игоревна',
  shortName: 'Наталья Игоревна',
  pronoun: 'она',

  // Employment
  position: 'Преподаватель математики',
  roles: ['Teacher', 'Curator'],          // assigned org roles
  primaryRole: 'Teacher',
  status: 'Active',
  employmentType: 'Полная занятость',
  contract: 'Трудовой договор',
  contractNumber: 'ТД-2022/41',
  rate: 1.0,                              // ставка
  branch: 'Корпус на Маросейке',
  department: 'Кафедра точных наук',
  manager: { name: 'Захарова Мария Алексеевна', role: 'Methodist', position: 'Заведующая кафедрой' },
  joined: '19.08.2022',
  joinedDays: 1357,                       // days in org as of 07.05.2026
  lastActive: '30 минут назад',

  // Contacts (work)
  workEmail: 'n.kovalenko@eureka-school.ru',
  workPhone: '+7 (495) 120-44-08, доб. 318',
  internalCode: '318',
  cabinet: 'каб. 214',
  telegram: '@n_kovalenko_math',

  // Identifiers
  staffNumber: 'СТФ-00041',

  // Subjects taught
  subjects: ['Математика', 'Алгебра', 'Геометрия', 'Подготовка к ЕГЭ'],

  // Groups assigned (taught and/or curated)
  groups: [
    { id: 'G-104', code: 'М-10А', name: 'Алгебра, 10 класс «А»',     students: 24, role: 'teacher', schedule: 'Пн, Ср, Пт · 14:00', progress: 0.62 },
    { id: 'G-117', code: 'М-10Б', name: 'Алгебра, 10 класс «Б»',     students: 22, role: 'teacher', schedule: 'Пн, Ср, Пт · 15:30', progress: 0.58 },
    { id: 'G-121', code: 'Г-9В',  name: 'Геометрия, 9 класс «В»',    students: 19, role: 'teacher', schedule: 'Вт, Чт · 13:00',     progress: 0.71 },
    { id: 'G-203', code: 'ЕГЭ-М', name: 'Подготовка к ЕГЭ — математика', students: 12, role: 'teacher', schedule: 'Сб · 11:00',         progress: 0.44 },
    { id: 'G-088', code: 'К-11А', name: '11 «А» — кураторство',      students: 26, role: 'curator', schedule: 'Кл. час: Пн · 09:00',  progress: null },
  ],

  // Workload
  workload: {
    weeklyHours: 24,
    contractHours: 25,
    studentsTotal: 103,
    avgAttendance: 0.94,
    avgGrade: 4.6,
    activeGroups: 4,
    curated: 1,
  },

  // Upcoming lessons
  schedule: [
    { day: 'Сегодня',    date: '07 мая',  time: '14:00–14:45', subject: 'Алгебра',   group: 'М-10А', room: '214', kind: 'lesson' },
    { day: 'Сегодня',    date: '07 мая',  time: '15:30–16:15', subject: 'Алгебра',   group: 'М-10Б', room: '214', kind: 'lesson' },
    { day: 'Завтра',     date: '08 мая',  time: '13:00–13:45', subject: 'Геометрия', group: 'Г-9В',  room: '107', kind: 'lesson' },
    { day: 'Сб',         date: '10 мая',  time: '11:00–12:30', subject: 'ЕГЭ',       group: 'ЕГЭ-М', room: '301', kind: 'consult' },
    { day: 'Пн',         date: '12 мая',  time: '09:00–09:25', subject: 'Кл. час',   group: 'К-11А', room: '218', kind: 'meeting' },
  ],

  // Qualifications
  qualifications: [
    { title: 'МГУ им. Ломоносова', meta: 'Механико-математический факультет, 2009', kind: 'edu' },
    { title: 'Высшая категория',   meta: 'Аттестация — действительна до 09.2027',   kind: 'category' },
    { title: 'Курсы ФИПИ — ЕГЭ-2026', meta: 'Сертификат № 14-228, 36 ак. ч.', kind: 'cert' },
    { title: 'Учитель года Москвы — финалист 2024', meta: 'Городской этап',  kind: 'award' },
  ],

  // Documents on file
  documents: [
    { name: 'Трудовой договор ТД-2022/41', meta: 'PDF · 1.4 МБ · загружен 19.08.2022', kind: 'pdf' },
    { name: 'Должностная инструкция',       meta: 'PDF · 412 КБ · 01.09.2024',         kind: 'pdf' },
    { name: 'Согласие на обработку ПДн',    meta: 'PDF · 188 КБ · 19.08.2022',         kind: 'pdf' },
    { name: 'Диплом о высшем образовании',  meta: 'JPG · 2.8 МБ · скан',                kind: 'img' },
    { name: 'Аттестация 2022',              meta: 'PDF · 720 КБ',                       kind: 'pdf' },
  ],

  // Internal notes (visible to admins/methodists, not to the member)
  notes: [
    { author: 'Захарова М.А.', when: '14 апр. 2026', text: 'Подготовила сильную группу к пробному ЕГЭ — средний балл 78. Рассмотреть на премию по итогам полугодия.' },
    { author: 'Соколов Д.П.',  when: '03 фев. 2026', text: 'Согласована замена кабинета на 214 после ремонта.' },
  ],

  // Activity timeline (work events)
  activity: [
    { when: 'Сегодня · 12:14', icon: 'CalendarDays', text: 'Отметила посещаемость на занятии «Алгебра 10А»' },
    { when: 'Сегодня · 09:02', icon: 'CircleCheck',  text: 'Опубликовала оценки за контрольную работу №7' },
    { when: 'Вчера · 18:30',   icon: 'FileText',     text: 'Загрузила учебный план на 4 четверть' },
    { when: '05 мая · 11:40',  icon: 'Users',        text: 'Назначена куратором группы 11 «А»' },
    { when: '28 апр. · 16:05', icon: 'Send',         text: 'Отправила домашнее задание группе ЕГЭ-М (12 учеников)' },
    { when: '14 апр. · 09:20', icon: 'Sparkles',     text: 'Получила благодарность от методиста' },
  ],
};

window.PROFILE = PROFILE;
