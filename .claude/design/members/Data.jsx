// Members directory data
const MEMBER_STATUSES = {
  Active: { label: 'Активен', bg: '#d1fae5', fg: '#047857', dot: '#10b981' },
  Invited: { label: 'Приглашён', bg: '#e0eaff', fg: '#4338ca', dot: '#6366f1' },
  Blocked: { label: 'Заблокирован', bg: '#fee2e2', fg: '#b91c1c', dot: '#ef4444' },
  Archived: { label: 'Архив', bg: '#f1f5f9', fg: '#475569', dot: '#94a3b8' },
};

// Custom roles (specific to this org)
const MEMBER_ROLES = [
  { value: 'Owner', label: 'Владелец', tone: 'violet' },
  { value: 'Director', label: 'Директор', tone: 'indigo' },
  { value: 'Admin', label: 'Администратор', tone: 'indigo' },
  { value: 'Methodist', label: 'Методист', tone: 'teal' },
  { value: 'Teacher', label: 'Преподаватель', tone: 'blue' },
  { value: 'Curator', label: 'Куратор групп', tone: 'amber' },
  { value: 'Accountant', label: 'Бухгалтер', tone: 'slate' },
];

const ROLE_TONES = {
  violet: { bg: 'rgba(139,92,246,0.12)', fg: '#6d28d9' },
  indigo: { bg: 'rgba(79,70,229,0.12)', fg: '#4338ca' },
  blue:   { bg: 'rgba(14,165,233,0.12)', fg: '#0369a1' },
  teal:   { bg: 'rgba(20,184,166,0.12)', fg: '#0f766e' },
  amber:  { bg: 'rgba(245,158,11,0.14)', fg: '#92400e' },
  slate:  { bg: '#f1f5f9', fg: '#475569' },
};

const MEMBERS = [
  { id: 1,  name: 'Мельникова Анна Сергеевна',    email: 'a.melnikova@eureka-school.ru', role: 'Owner',      status: 'Active',   lastActive: '5 мин назад',  joined: '14.03.2019' },
  { id: 2,  name: 'Соколов Дмитрий Павлович',     email: 'd.sokolov@eureka-school.ru',   role: 'Director',   status: 'Active',   lastActive: '1 ч назад',    joined: '22.05.2020' },
  { id: 3,  name: 'Иванова Екатерина Олеговна',   email: 'e.ivanova@eureka-school.ru',   role: 'Admin',      status: 'Active',   lastActive: '12 мин назад', joined: '11.09.2021' },
  { id: 4,  name: 'Петров Артём Николаевич',      email: 'a.petrov@eureka-school.ru',    role: 'Teacher',    status: 'Active',   lastActive: '2 ч назад',    joined: '03.02.2022' },
  { id: 5,  name: 'Коваленко Наталья Игоревна',   email: 'n.kovalenko@eureka-school.ru', role: 'Teacher',    status: 'Active',   lastActive: '30 мин назад', joined: '19.08.2022' },
  { id: 6,  name: 'Романов Илья Викторович',       email: 'i.romanov@mail.ru',            role: 'Teacher',    status: 'Invited',  lastActive: '—',            joined: '15.04.2026' },
  { id: 7,  name: 'Захарова Мария Алексеевна',     email: 'm.zakharova@eureka-school.ru', role: 'Methodist',  status: 'Active',   lastActive: '1 д назад',    joined: '07.11.2022' },
  { id: 8,  name: 'Белов Сергей Андреевич',        email: 's.belov@eureka-school.ru',     role: 'Curator',    status: 'Active',   lastActive: '4 ч назад',    joined: '01.09.2023' },
  { id: 9,  name: 'Тарасова Ольга Николаевна',     email: 'o.tarasova@eureka-school.ru',  role: 'Accountant', status: 'Active',   lastActive: '3 д назад',    joined: '12.01.2023' },
  { id: 10, name: 'Жуков Никита Владимирович',     email: 'n.zhukov@eureka-school.ru',    role: 'Teacher',    status: 'Blocked',  lastActive: '18 д назад',   joined: '24.06.2021' },
  { id: 11, name: 'Морозова Валерия Дмитриевна',   email: 'v.morozova@gmail.com',         role: 'Teacher',    status: 'Invited',  lastActive: '—',            joined: '21.04.2026' },
  { id: 12, name: 'Лебедев Павел Олегович',        email: 'p.lebedev@eureka-school.ru',   role: 'Curator',    status: 'Active',   lastActive: 'вчера',        joined: '14.10.2023' },
  { id: 13, name: 'Новиков Андрей Станиславович',  email: 'a.novikov@eureka-school.ru',   role: 'Admin',      status: 'Archived', lastActive: '—',            joined: '03.07.2020' },
  { id: 14, name: 'Смирнова Дарья Константиновна', email: 'd.smirnova@eureka-school.ru',  role: 'Teacher',    status: 'Active',   lastActive: '40 мин назад', joined: '16.02.2024' },
  { id: 15, name: 'Кузнецов Роман Юрьевич',        email: 'r.kuznetsov@eureka-school.ru', role: 'Methodist',  status: 'Active',   lastActive: '20 мин назад', joined: '28.08.2023' },
  { id: 16, name: 'Орлова Светлана Михайловна',    email: 's.orlova@yandex.ru',           role: 'Teacher',    status: 'Invited',  lastActive: '—',            joined: '22.04.2026' },
  { id: 17, name: 'Фёдоров Максим Викторович',     email: 'm.fedorov@eureka-school.ru',   role: 'Teacher',    status: 'Active',   lastActive: '6 ч назад',    joined: '09.03.2024' },
  { id: 18, name: 'Васильева Алина Павловна',      email: 'a.vasilyeva@eureka-school.ru', role: 'Curator',    status: 'Active',   lastActive: '3 ч назад',    joined: '11.05.2024' },
  { id: 19, name: 'Павлов Глеб Антонович',         email: 'g.pavlov@eureka-school.ru',    role: 'Teacher',    status: 'Blocked',  lastActive: '2 мес назад',  joined: '07.01.2023' },
  { id: 20, name: 'Сидорова Юлия Александровна',   email: 'y.sidorova@eureka-school.ru',  role: 'Methodist',  status: 'Active',   lastActive: '1 ч назад',    joined: '18.11.2023' },
  { id: 21, name: 'Громов Егор Денисович',         email: 'e.gromov@eureka-school.ru',    role: 'Teacher',    status: 'Active',   lastActive: '9 ч назад',    joined: '25.09.2024' },
  { id: 22, name: 'Николаева Вероника Игоревна',   email: 'v.nikolaeva@outlook.com',      role: 'Teacher',    status: 'Invited',  lastActive: '—',            joined: '23.04.2026' },
];

window.MEMBERS = MEMBERS;
window.MEMBER_STATUSES = MEMBER_STATUSES;
window.MEMBER_ROLES = MEMBER_ROLES;
window.ROLE_TONES = ROLE_TONES;
