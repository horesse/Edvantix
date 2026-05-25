// Enum data for LegalForm, OrganizationType, ContactType
const LEGAL_FORMS = [
  { value: 'Llc', tag: 'ООО', label: 'Общество с ограниченной ответственностью', entity: true },
  { value: 'Ojsc', tag: 'ОАО', label: 'Открытое акционерное общество', entity: true },
  { value: 'Cjsc', tag: 'ЗАО', label: 'Закрытое акционерное общество', entity: true },
  { value: 'Ue', tag: 'УП', label: 'Унитарное предприятие', entity: true },
  { value: 'Pue', tag: 'ЧУП', label: 'Частное унитарное предприятие', entity: true },
  { value: 'IndividualEntrepreneur', tag: 'ИП', label: 'Индивидуальный предприниматель', entity: false },
  { value: 'ProductionCooperative', tag: 'Коопер.', label: 'Производственный кооператив', entity: true },
  { value: 'StateEducationalInstitution', tag: 'ГУО', label: 'Государственное учреждение образования', entity: true },
  { value: 'PrivateEducationalInstitution', tag: 'ЧУО', label: 'Частное учреждение образования', entity: true },
  { value: 'EducationalInstitution', tag: 'ОУ', label: 'Общее образовательное учреждение', entity: true },
];

const ORG_TYPES = [
  { value: 'EducationalInstitution', label: 'Учреждение образования' },
  { value: 'GeneralEducationSchool', label: 'Учреждение общего среднего образования' },
  { value: 'Lyceum', label: 'Лицей' },
  { value: 'Gymnasium', label: 'Гимназия' },
  { value: 'College', label: 'Колледж' },
  { value: 'VocationalSchool', label: 'Профессионально-техническое училище' },
  { value: 'University', label: 'Университет, институт' },
  { value: 'AdditionalEducation', label: 'Учреждение дополнительного образования детей и молодёжи' },
  { value: 'Preschool', label: 'Дошкольное учреждение образования' },
  { value: 'PrivateEducationalCenter', label: 'Частный образовательный центр' },
  { value: 'TrainingCompany', label: 'Учебный центр, обучающая компания' },
  { value: 'LlcEducation', label: 'ООО в сфере образования' },
  { value: 'IndividualEntrepreneur', label: 'Индивидуальный предприниматель' },
  { value: 'LanguageSchool', label: 'Языковая школа' },
  { value: 'ItSchool', label: 'IT-школа, школа программирования' },
  { value: 'TutoringCenter', label: 'Репетиторский центр' },
  { value: 'OnlinePlatform', label: 'Онлайн-платформа' },
];

const CONTACT_TYPES = [
  { value: 'Email', label: 'Электронная почта', short: 'Email', icon: 'Mail', placeholder: 'school@example.ru', hint: 'Используйте адрес, который проверяют ежедневно' },
  { value: 'MobilePhone', label: 'Мобильный телефон', short: 'Телефон', icon: 'Phone', placeholder: '+7 (900) 123-45-67', hint: 'С кодом страны, в международном формате' },
  { value: 'Telegram', label: 'Telegram', short: 'Telegram', icon: 'Send', placeholder: '@school_official', hint: 'Имя пользователя или ссылка t.me/…' },
  { value: 'WhatsApp', label: 'WhatsApp', short: 'WhatsApp', icon: 'MessageCircle', placeholder: '+7 (900) 123-45-67', hint: 'Номер, привязанный к WhatsApp' },
  { value: 'Viber', label: 'Viber', short: 'Viber', icon: 'MessageCircle', placeholder: '+7 (900) 123-45-67', hint: 'Номер, привязанный к Viber' },
];

window.LEGAL_FORMS = LEGAL_FORMS;
window.ORG_TYPES = ORG_TYPES;
window.CONTACT_TYPES = CONTACT_TYPES;
