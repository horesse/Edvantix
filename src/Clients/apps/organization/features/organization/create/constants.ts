import type { CreateOrganizationInput } from "@workspace/validations/organization";

export type { ContactTypeEntry, LegalFormEntry } from "../constants";
export { CONTACT_TYPE_DATA, LEGAL_FORM_DATA } from "../constants";

// ── Wizard steps ──────────────────────────────────────────────────────────────

export const WIZARD_STEPS = [
  { id: "legal", title: "Форма собственности", hint: "Правовой статус школы" },
  { id: "about", title: "Об организации", hint: "Название, дата, тип" },
  { id: "contact", title: "Основной контакт", hint: "Канал связи с нами" },
  { id: "review", title: "Проверка", hint: "Подтверждение данных" },
] as const;

export type WizardStep = (typeof WIZARD_STEPS)[number];

// ── Fields validated per step ─────────────────────────────────────────────────

export const STEP_FIELDS: Record<number, (keyof CreateOrganizationInput)[]> = {
  0: ["legalForm"],
  1: ["fullLegalName", "registrationDate", "organizationType"],
  2: ["primaryContactType", "primaryContactValue", "primaryContactDescription"],
};
