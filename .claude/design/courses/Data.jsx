// Courses catalog — учебные программы преподавателя
// Course = программа со списком занятий и материалов; на её базе создаются группы.

const COURSE_STATUSES = {
  Active:   { label: 'Активен',   bg: '#d1fae5', fg: '#047857', dot: '#10b981' },
  Draft:    { label: 'Черновик',  bg: '#fef3c7', fg: '#92400e', dot: '#f59e0b' },
  Review:   { label: 'На проверке',bg: '#e0eaff', fg: '#4338ca', dot: '#6366f1' },
  Archived: { label: 'Архив',     bg: '#f1f5f9', fg: '#64748b', dot: '#94a3b8' },
};

// Subjects = категории, по которым группируется каталог
const COURSE_SUBJECTS = {
  english:  { label: 'Английский язык',    icon: 'BookOpen',     tone: 'indigo' },
  math:     { label: 'Математика',         icon: 'BarChart2',    tone: 'blue'   },
  exam:     { label: 'Подготовка к экзаменам', icon: 'GraduationCap', tone: 'violet' },
  speaking: { label: 'Разговорные клубы',  icon: 'MessageCircle',tone: 'teal'   },
  kids:     { label: 'Программы для детей',icon: 'Sparkles',     tone: 'amber'  },
};

const SUBJECT_TONES = {
  indigo: { bg: '#eef2ff', fg: '#4338ca', cover: 'linear-gradient(135deg, #6366f1, #818cf8)' },
  blue:   { bg: '#e0f2fe', fg: '#0369a1', cover: 'linear-gradient(135deg, #0ea5e9, #38bdf8)' },
  violet: { bg: '#f3e8ff', fg: '#6d28d9', cover: 'linear-gradient(135deg, #8b5cf6, #a78bfa)' },
  teal:   { bg: '#ccfbf1', fg: '#0f766e', cover: 'linear-gradient(135deg, #14b8a6, #2dd4bf)' },
  amber:  { bg: '#fef3c7', fg: '#92400e', cover: 'linear-gradient(135deg, #f59e0b, #fbbf24)' },
};

const COURSE_LEVELS = [
  { value: 'A1',   label: 'A1' },
  { value: 'A2',   label: 'A2' },
  { value: 'B1',   label: 'B1' },
  { value: 'B2',   label: 'B2' },
  { value: 'C1',   label: 'C1' },
  { value: 'KIDS', label: '7–10 лет' },
  { value: 'TEEN', label: '11–14 лет' },
  { value: 'ANY',  label: 'Любой' },
];

// Course row: code, name, subject, level, durationWeeks, lessons, status, owner, cover (initials),
// groups (количество групп использующих курс), updated.
const COURSES = [
  { id: 1,  code: 'EN-GEN-A2',
    name: 'General English · Elementary',
    subject: 'english', level: 'A2', durationWeeks: 32, lessons: 64,
    status: 'Active',   owner: 'Анна Мельникова', groups: 3, students: 26,
    updated: '24.04.2026', cover: 'GE' },
  { id: 2,  code: 'EN-GEN-B1',
    name: 'General English · Intermediate',
    subject: 'english', level: 'B1', durationWeeks: 36, lessons: 72,
    status: 'Active',   owner: 'Анна Мельникова', groups: 4, students: 38,
    updated: '02.05.2026', cover: 'GI' },
  { id: 3,  code: 'EN-GEN-B2',
    name: 'General English · Upper-Intermediate',
    subject: 'english', level: 'B2', durationWeeks: 36, lessons: 72,
    status: 'Active',   owner: 'Анна Мельникова', groups: 2, students: 19,
    updated: '17.04.2026', cover: 'UI' },
  { id: 4,  code: 'EN-BIZ-B1',
    name: 'Business English · Practice',
    subject: 'english', level: 'B1', durationWeeks: 24, lessons: 48,
    status: 'Active',   owner: 'Анна Мельникова', groups: 1, students: 8,
    updated: '11.03.2026', cover: 'BE' },
  { id: 5,  code: 'EN-IELTS-7',
    name: 'IELTS 6.5+ · подготовка',
    subject: 'exam',    level: 'B2', durationWeeks: 20, lessons: 40,
    status: 'Active',   owner: 'Анна Мельникова', groups: 2, students: 14,
    updated: '28.04.2026', cover: 'IL' },
  { id: 6,  code: 'EN-EGE-11',
    name: 'ЕГЭ — английский, 11 класс',
    subject: 'exam',    level: 'B2', durationWeeks: 32, lessons: 64,
    status: 'Active',   owner: 'Анна Мельникова', groups: 1, students: 6,
    updated: '06.05.2026', cover: 'EG' },
  { id: 7,  code: 'EN-EGE-10',
    name: 'ЕГЭ — английский, 10 класс',
    subject: 'exam',    level: 'B1', durationWeeks: 32, lessons: 64,
    status: 'Review',   owner: 'Анна Мельникова', groups: 0, students: 0,
    updated: '05.05.2026', cover: 'E1' },
  { id: 8,  code: 'EN-CONV-C1',
    name: 'Advanced Conversation Club',
    subject: 'speaking',level: 'C1', durationWeeks: 12, lessons: 24,
    status: 'Active',   owner: 'Анна Мельникова', groups: 1, students: 5,
    updated: '21.04.2026', cover: 'AC' },
  { id: 9,  code: 'EN-CONV-TN',
    name: 'Teens Speaking Club',
    subject: 'speaking',level: 'TEEN', durationWeeks: 16, lessons: 16,
    status: 'Active',   owner: 'Анна Мельникова', groups: 1, students: 7,
    updated: '03.05.2026', cover: 'TS' },
  { id: 10, code: 'EN-KIDS-7',
    name: 'English for Kids · 7–8 лет',
    subject: 'kids',    level: 'KIDS', durationWeeks: 32, lessons: 64,
    status: 'Active',   owner: 'Анна Мельникова', groups: 2, students: 18,
    updated: '15.04.2026', cover: 'K7' },
  { id: 11, code: 'EN-KIDS-9',
    name: 'English for Kids · 9–10 лет',
    subject: 'kids',    level: 'KIDS', durationWeeks: 32, lessons: 64,
    status: 'Active',   owner: 'Анна Мельникова', groups: 1, students: 11,
    updated: '15.04.2026', cover: 'K9' },
  { id: 12, code: 'EN-KIDS-RD',
    name: 'Kids Reading Club',
    subject: 'kids',    level: 'KIDS', durationWeeks: 16, lessons: 16,
    status: 'Draft',    owner: 'Анна Мельникова', groups: 0, students: 0,
    updated: '06.05.2026', cover: 'KR' },
  { id: 13, code: 'EN-GRAM-INT',
    name: 'Grammar Intensive',
    subject: 'english', level: 'B1', durationWeeks: 8, lessons: 16,
    status: 'Draft',    owner: 'Анна Мельникова', groups: 0, students: 0,
    updated: '07.05.2026', cover: 'GR' },
  { id: 14, code: 'EN-GEN-A1',
    name: 'Beginner с нуля',
    subject: 'english', level: 'A1', durationWeeks: 28, lessons: 56,
    status: 'Active',   owner: 'Анна Мельникова', groups: 2, students: 14,
    updated: '12.04.2026', cover: 'BG' },
  { id: 15, code: 'EN-GEN-A2-25',
    name: 'Pre-Intermediate · 2024',
    subject: 'english', level: 'A2', durationWeeks: 32, lessons: 64,
    status: 'Archived', owner: 'Анна Мельникова', groups: 0, students: 0,
    updated: '03.06.2025', cover: 'PI' },
];

window.COURSES = COURSES;
window.COURSE_STATUSES = COURSE_STATUSES;
window.COURSE_SUBJECTS = COURSE_SUBJECTS;
window.SUBJECT_TONES = SUBJECT_TONES;
window.COURSE_LEVELS = COURSE_LEVELS;
