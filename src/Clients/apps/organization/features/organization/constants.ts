import type { ElementType } from "react";

import { Mail, MessageCircle, Phone, Send } from "lucide-react";

import { ContactType, LegalForm } from "@workspace/types/organization";

// ── Legal form data ───────────────────────────────────────────────────────────

export type LegalFormEntry = {
  value: LegalForm;
  tag: string;
  label: string;
  isLegalEntity: boolean;
};

export const LEGAL_FORM_DATA: LegalFormEntry[] = [
  {
    value: LegalForm.Llc,
    tag: "ООО",
    label: "Общество с ограниченной ответственностью",
    isLegalEntity: true,
  },
  {
    value: LegalForm.Ojsc,
    tag: "ОАО",
    label: "Открытое акционерное общество",
    isLegalEntity: true,
  },
  {
    value: LegalForm.Cjsc,
    tag: "ЗАО",
    label: "Закрытое акционерное общество",
    isLegalEntity: true,
  },
  {
    value: LegalForm.Ue,
    tag: "УП",
    label: "Унитарное предприятие",
    isLegalEntity: true,
  },
  {
    value: LegalForm.Pue,
    tag: "ЧУП",
    label: "Частное унитарное предприятие",
    isLegalEntity: true,
  },
  {
    value: LegalForm.IndividualEntrepreneur,
    tag: "ИП",
    label: "Индивидуальный предприниматель",
    isLegalEntity: false,
  },
  {
    value: LegalForm.ProductionCooperative,
    tag: "Коопер.",
    label: "Производственный кооператив",
    isLegalEntity: true,
  },
  {
    value: LegalForm.StateEducationalInstitution,
    tag: "ГУО",
    label: "Государственное учреждение образования",
    isLegalEntity: true,
  },
  {
    value: LegalForm.PrivateEducationalInstitution,
    tag: "ЧУО",
    label: "Частное учреждение образования",
    isLegalEntity: true,
  },
  {
    value: LegalForm.EducationalInstitution,
    tag: "ОУ",
    label: "Общее образовательное учреждение",
    isLegalEntity: true,
  },
];

// ── Contact type data ─────────────────────────────────────────────────────────

export type ContactTypeEntry = {
  value: ContactType;
  label: string;
  short: string;
  Icon: ElementType;
  placeholder: string;
  hint: string;
  inputType: string;
};

export const CONTACT_TYPE_DATA: ContactTypeEntry[] = [
  {
    value: ContactType.Email,
    label: "Электронная почта",
    short: "Email",
    Icon: Mail,
    placeholder: "school@example.ru",
    hint: "Используйте адрес, который проверяют ежедневно",
    inputType: "email",
  },
  {
    value: ContactType.MobilePhone,
    label: "Мобильный телефон",
    short: "Телефон",
    Icon: Phone,
    placeholder: "+7 (900) 123-45-67",
    hint: "С кодом страны, в международном формате",
    inputType: "tel",
  },
  {
    value: ContactType.Telegram,
    label: "Telegram",
    short: "Telegram",
    Icon: Send,
    placeholder: "@school_official",
    hint: "Имя пользователя или ссылка t.me/…",
    inputType: "text",
  },
  {
    value: ContactType.WhatsApp,
    label: "WhatsApp",
    short: "WhatsApp",
    Icon: MessageCircle,
    placeholder: "+7 (900) 123-45-67",
    hint: "Номер, привязанный к WhatsApp",
    inputType: "tel",
  },
  {
    value: ContactType.Viber,
    label: "Viber",
    short: "Viber",
    Icon: MessageCircle,
    placeholder: "+7 (900) 123-45-67",
    hint: "Номер, привязанный к Viber",
    inputType: "tel",
  },
];
