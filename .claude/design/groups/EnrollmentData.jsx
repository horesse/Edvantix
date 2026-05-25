// Students pool — fictional directory of students of the school.
// Used by Step 3 (enrollment). Field meanings:
//   status:  'free'      — активный студент школы, без группы
//            'enrolled'  — уже учится в другой группе того же курса/уровня
//            'waitlist'  — записан в лист ожидания этого курса
//            'invited'   — приглашён, ещё не принял
//            'new'       — заявка с сайта, без аккаунта
//   tags:    ['trial', 'paid', 'tested'] — особые отметки

const STUDENT_LEVELS = ['A1','A2','B1','B2','C1','JR','TN','PR'];

const STUDENT_POOL = [
  // ── Подходят группе B1 ─────────────────────────────────────
  { id: 1,  name: 'Алексеев Кирилл',        age: 28, category: 'adult', level: 'B1', email: 'k.alekseev@gmail.com',     phone: '+7 905 421-08-19', status: 'waitlist', tags: ['paid','tested'], note: 'Лист ожидания · 12.04.2026' },
  { id: 2,  name: 'Бородина Мария',         age: 24, category: 'adult', level: 'B1', email: 'm.borodina@yandex.ru',     phone: '+7 916 244-37-92', status: 'waitlist', tags: ['tested'],         note: 'Лист ожидания · 18.04.2026' },
  { id: 3,  name: 'Васильев Артём',          age: 31, category: 'adult', level: 'B1', email: 'artem.v@outlook.com',     phone: '+7 926 188-04-55', status: 'waitlist', tags: ['paid','tested'], note: 'Лист ожидания · 02.05.2026' },
  { id: 4,  name: 'Григорьева Светлана',     age: 26, category: 'adult', level: 'B1', email: 's.grigoreva@mail.ru',     phone: '+7 903 712-50-08', status: 'free',     tags: ['tested'],         note: 'Завершила A2 в апреле' },
  { id: 5,  name: 'Демин Никита',            age: 22, category: 'adult', level: 'B1', email: 'nikita.demin@gmail.com',  phone: '+7 999 405-66-31', status: 'free',     tags: ['tested'],         note: 'Прошёл вступительный тест' },
  { id: 6,  name: 'Ерёмина Анастасия',       age: 29, category: 'adult', level: 'B1', email: 'a.eremina@yandex.ru',     phone: '+7 985 332-21-77', status: 'free',     tags: ['trial'],          note: 'Прошла пробное занятие' },
  { id: 7,  name: 'Жуков Андрей',            age: 34, category: 'adult', level: 'B1', email: 'andrey.zhukov@gmail.com', phone: '+7 916 504-99-12', status: 'free',     tags: [],                 note: 'Перешёл с уровня A2' },
  { id: 8,  name: 'Зайцева Елена',           age: 33, category: 'adult', level: 'B1', email: 'elena.zayceva@mail.ru',   phone: '+7 925 117-43-08', status: 'new',      tags: [],                 note: 'Заявка с сайта · 04.05.2026' },

  // ── Активные студенты в других B1-группах ─────────────────
  { id: 9,  name: 'Иванов Денис',            age: 27, category: 'adult', level: 'B1', email: 'd.ivanov@gmail.com',      phone: '+7 903 219-77-04', status: 'enrolled', tags: [], note: 'EN-B1-15 · дневная' },
  { id: 10, name: 'Климова Алина',           age: 30, category: 'adult', level: 'B1', email: 'klimova.a@mail.ru',       phone: '+7 916 588-30-44', status: 'enrolled', tags: [], note: 'EN-B1-08 · Business' },

  // ── Другие уровни (показываются в "все") ───────────────────
  { id: 11, name: 'Лазарев Михаил',          age: 25, category: 'adult', level: 'A2', email: 'm.lazarev@gmail.com',     phone: '+7 905 671-25-08', status: 'free',     tags: [],                 note: 'Завершит A2 в июне' },
  { id: 12, name: 'Михайлова Полина',        age: 21, category: 'adult', level: 'B2', email: 'p.mikhaylova@yandex.ru',  phone: '+7 926 304-19-65', status: 'free',     tags: ['tested'],         note: 'Готова к B2 — тест 92%' },
  { id: 13, name: 'Новиков Сергей',          age: 38, category: 'adult', level: 'A1', email: 's.novikov@outlook.com',   phone: '+7 999 218-77-30', status: 'new',      tags: [],                 note: 'Заявка с сайта · 09.05.2026' },
  { id: 14, name: 'Орлова Юлия',             age: 28, category: 'adult', level: 'B2', email: 'y.orlova@gmail.com',      phone: '+7 985 442-08-19', status: 'waitlist', tags: ['paid'],           note: 'Лист ожидания B2' },
  { id: 15, name: 'Павлова Дарья',           age: 19, category: 'adult', level: 'A2', email: 'd.pavlova@mail.ru',       phone: '+7 916 277-65-43', status: 'free',     tags: [],                 note: 'Завершила A1' },

  // ── Подростки / дети ───────────────────────────────────────
  { id: 16, name: 'Романов Тимур',           age: 13, category: 'teen',  level: 'TN', email: 'romanov.parent@mail.ru',  phone: '+7 903 800-12-04', status: 'free',     tags: [],                 note: 'Контакт родителя' },
  { id: 17, name: 'Соколова Алиса',          age: 9,  category: 'kid',   level: 'JR', email: 'sokolova.mom@gmail.com',  phone: '+7 926 405-99-30', status: 'waitlist', tags: ['paid'],           note: 'Лист ожидания Kids' },
  { id: 18, name: 'Тимофеев Егор',           age: 8,  category: 'kid',   level: 'JR', email: 'timofeev.dad@yandex.ru',  phone: '+7 905 332-77-21', status: 'free',     tags: [],                 note: 'Прошёл пробное' },

  // ── ЕГЭ / подготовка ───────────────────────────────────────
  { id: 19, name: 'Устинов Максим',          age: 17, category: 'teen',  level: 'PR', email: 'ustinov.m@gmail.com',     phone: '+7 916 188-04-65', status: 'free',     tags: ['paid','tested'],  note: 'Подготовка к ЕГЭ' },
  { id: 20, name: 'Фокина Виктория',         age: 16, category: 'teen',  level: 'PR', email: 'fokina.v@mail.ru',        phone: '+7 999 244-08-19', status: 'free',     tags: ['tested'],         note: 'Подготовка к ОГЭ' },

  // ── Приглашённые (ждут активации) ─────────────────────────
  { id: 21, name: 'Хохлова Елизавета',       age: 27, category: 'adult', level: 'B1', email: 'liza.h@gmail.com',        phone: '+7 985 122-04-37', status: 'invited',  tags: [],                 note: 'Приглашение от 28.04.2026' },
  { id: 22, name: 'Цветков Иван',            age: 24, category: 'adult', level: 'B1', email: 'ivan.c@outlook.com',      phone: '+7 926 088-37-04', status: 'invited',  tags: [],                 note: 'Приглашение от 02.05.2026' },

  // ── Свободные адалты разного уровня ────────────────────────
  { id: 23, name: 'Чернова Анна',            age: 32, category: 'adult', level: 'C1', email: 'a.chernova@mail.ru',      phone: '+7 905 504-99-30', status: 'free',     tags: [],                 note: 'Готова к Advanced' },
  { id: 24, name: 'Шарапова Кристина',       age: 26, category: 'adult', level: 'B1', email: 'k.sharapova@yandex.ru',   phone: '+7 916 432-08-19', status: 'free',     tags: ['trial'],          note: 'Пробное прошло' },
  { id: 25, name: 'Щукин Глеб',              age: 29, category: 'adult', level: 'B1', email: 'gleb.shukin@gmail.com',   phone: '+7 999 117-65-43', status: 'free',     tags: ['paid'],           note: 'Внёс предоплату' },
  { id: 26, name: 'Юдина Татьяна',           age: 35, category: 'adult', level: 'B1', email: 't.yudina@mail.ru',        phone: '+7 985 077-30-12', status: 'free',     tags: ['tested'],         note: 'Тест 84%' },
  { id: 27, name: 'Яковлев Денис',           age: 23, category: 'adult', level: 'B1', email: 'd.yakovlev@gmail.com',    phone: '+7 926 188-21-77', status: 'new',      tags: [],                 note: 'Заявка с сайта · 06.05.2026' },
];

const STUDENT_TAGS = {
  paid:    { label: 'Предоплата',   bg: 'rgba(16,185,129,0.12)',  fg: '#047857' },
  tested:  { label: 'Тест пройден', bg: 'rgba(99,102,241,0.10)',  fg: '#4338ca' },
  trial:   { label: 'Пробное',      bg: 'rgba(245,158,11,0.14)',  fg: '#92400e' },
};

const STUDENT_STATUS_LABELS = {
  free:     { label: 'Без группы',         tone: 'slate'   },
  waitlist: { label: 'Лист ожидания',      tone: 'amber'   },
  enrolled: { label: 'В другой группе',    tone: 'blue'    },
  invited:  { label: 'Приглашён',           tone: 'indigo'  },
  new:      { label: 'Новая заявка',        tone: 'violet'  },
};

window.STUDENT_POOL = STUDENT_POOL;
window.STUDENT_TAGS = STUDENT_TAGS;
window.STUDENT_STATUS_LABELS = STUDENT_STATUS_LABELS;
window.STUDENT_LEVELS = STUDENT_LEVELS;
