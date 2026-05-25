// Shared data: features, permissions, roles
const FEATURES = [
  { id: 'students',   label: 'Студенты',      icon: 'GraduationCap', desc: 'Профили учеников, зачисления, родители' },
  { id: 'courses',    label: 'Курсы',         icon: 'BookOpen',      desc: 'Программы, уроки, материалы' },
  { id: 'schedule',   label: 'Расписание',    icon: 'CalendarDays',  desc: 'Занятия, группы, преподаватели' },
  { id: 'attendance', label: 'Посещаемость',  icon: 'BarChart2',     desc: 'Журналы, отметки, пропуски' },
  { id: 'finance',    label: 'Финансы',       icon: 'Briefcase',     desc: 'Платежи, договоры, задолженности' },
  { id: 'reports',    label: 'Отчёты',        icon: 'FileText',      desc: 'Аналитика и выгрузки' },
  { id: 'org',        label: 'Организация',   icon: 'Building2',     desc: 'Реквизиты, участники, роли' },
];

const PERMISSIONS = {
  students: [
    { id: 'students.view',    label: 'Просмотр списка студентов' },
    { id: 'students.create',  label: 'Добавление студентов' },
    { id: 'students.edit',    label: 'Редактирование профилей' },
    { id: 'students.delete',  label: 'Удаление студентов' },
    { id: 'students.import',  label: 'Импорт из файла' },
    { id: 'students.export',  label: 'Экспорт списков' },
    { id: 'students.parents', label: 'Управление контактами родителей' },
  ],
  courses: [
    { id: 'courses.view',    label: 'Просмотр курсов' },
    { id: 'courses.create',  label: 'Создание курсов' },
    { id: 'courses.edit',    label: 'Редактирование программы' },
    { id: 'courses.delete',  label: 'Удаление курсов' },
    { id: 'courses.publish', label: 'Публикация и снятие с публикации' },
    { id: 'courses.content', label: 'Загрузка учебных материалов' },
  ],
  schedule: [
    { id: 'schedule.view',   label: 'Просмотр расписания' },
    { id: 'schedule.create', label: 'Создание занятий' },
    { id: 'schedule.edit',   label: 'Перенос и замена занятий' },
    { id: 'schedule.delete', label: 'Отмена занятий' },
    { id: 'schedule.groups', label: 'Управление учебными группами' },
  ],
  attendance: [
    { id: 'attendance.view',   label: 'Просмотр журнала' },
    { id: 'attendance.mark',   label: 'Отметка присутствия' },
    { id: 'attendance.edit',   label: 'Изменение прошлых отметок' },
    { id: 'attendance.export', label: 'Экспорт журнала' },
  ],
  finance: [
    { id: 'finance.view',     label: 'Просмотр финансов' },
    { id: 'finance.invoice',  label: 'Выставление счетов' },
    { id: 'finance.payment',  label: 'Приём оплат' },
    { id: 'finance.refund',   label: 'Возвраты' },
    { id: 'finance.contract', label: 'Работа с договорами' },
    { id: 'finance.export',   label: 'Выгрузка для бухгалтерии' },
  ],
  reports: [
    { id: 'reports.view',   label: 'Просмотр отчётов' },
    { id: 'reports.build',  label: 'Построение пользовательских отчётов' },
    { id: 'reports.export', label: 'Экспорт в Excel/PDF' },
    { id: 'reports.share',  label: 'Отправка отчётов по email' },
  ],
  org: [
    { id: 'org.view',     label: 'Просмотр организации' },
    { id: 'org.edit',     label: 'Редактирование реквизитов' },
    { id: 'org.members',  label: 'Управление участниками' },
    { id: 'org.roles',    label: 'Настройка ролей и прав' },
    { id: 'org.billing',  label: 'Управление тарифом и оплатой' },
    { id: 'org.archive',  label: 'Архивирование организации' },
  ],
};

const ALL_PERM_IDS = Object.values(PERMISSIONS).flat().map(p => p.id);

function permsFor(list) { return new Set(list); }
function allPermsSet() { return new Set(ALL_PERM_IDS); }

const ROLES = [
  {
    id: 'owner', name: 'Владелец', description: 'Полный доступ ко всем разделам, включая удаление организации',
    tone: 'violet', system: true, members: 1,
    permissions: allPermsSet(),
  },
  {
    id: 'director', name: 'Директор', description: 'Управление всеми разделами, кроме биллинга и удаления',
    tone: 'indigo', system: true, members: 1,
    permissions: permsFor(ALL_PERM_IDS.filter(p => !['org.billing','org.archive'].includes(p))),
  },
  {
    id: 'admin', name: 'Администратор', description: 'Операционное управление: студенты, расписание, платежи',
    tone: 'indigo', system: false, members: 3,
    permissions: permsFor([
      'students.view','students.create','students.edit','students.import','students.export','students.parents',
      'courses.view','courses.edit','courses.content',
      'schedule.view','schedule.create','schedule.edit','schedule.delete','schedule.groups',
      'attendance.view','attendance.mark','attendance.edit','attendance.export',
      'finance.view','finance.invoice','finance.payment','finance.contract',
      'reports.view','reports.build','reports.export',
      'org.view','org.members',
    ]),
  },
  {
    id: 'methodist', name: 'Методист', description: 'Курсы, программы и учебные материалы',
    tone: 'teal', system: false, members: 3,
    permissions: permsFor([
      'students.view',
      'courses.view','courses.create','courses.edit','courses.publish','courses.content',
      'schedule.view','schedule.groups',
      'attendance.view',
      'reports.view',
    ]),
  },
  {
    id: 'teacher', name: 'Преподаватель', description: 'Ведение занятий и журнала своих групп',
    tone: 'blue', system: true, members: 9,
    permissions: permsFor([
      'students.view',
      'courses.view','courses.content',
      'schedule.view',
      'attendance.view','attendance.mark',
      'reports.view',
    ]),
  },
  {
    id: 'curator', name: 'Куратор групп', description: 'Сопровождение студентов и коммуникация с родителями',
    tone: 'amber', system: false, members: 3,
    permissions: permsFor([
      'students.view','students.edit','students.parents','students.export',
      'courses.view',
      'schedule.view',
      'attendance.view','attendance.mark',
      'reports.view',
    ]),
  },
  {
    id: 'accountant', name: 'Бухгалтер', description: 'Финансы, договоры, выгрузки',
    tone: 'slate', system: false, members: 1,
    permissions: permsFor([
      'students.view','students.export',
      'finance.view','finance.invoice','finance.payment','finance.refund','finance.contract','finance.export',
      'reports.view','reports.export',
      'org.view',
    ]),
  },
];

const TONE_COLORS = {
  violet: { bg: 'rgba(139,92,246,0.12)', fg: '#6d28d9' },
  indigo: { bg: 'rgba(79,70,229,0.12)',  fg: '#4338ca' },
  blue:   { bg: 'rgba(14,165,233,0.12)', fg: '#0369a1' },
  teal:   { bg: 'rgba(20,184,166,0.12)', fg: '#0f766e' },
  amber:  { bg: 'rgba(245,158,11,0.14)', fg: '#92400e' },
  slate:  { bg: '#f1f5f9',               fg: '#475569' },
};

window.FEATURES = FEATURES;
window.PERMISSIONS = PERMISSIONS;
window.ALL_PERM_IDS = ALL_PERM_IDS;
window.ROLES = ROLES;
window.TONE_COLORS = TONE_COLORS;
