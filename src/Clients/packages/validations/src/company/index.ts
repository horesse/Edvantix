import { z } from "zod";

import {
  ContactType,
  LegalForm,
  OrganizationType,
} from "@workspace/types/company";

const MAX_NAME_LENGTH = 200;
const MAX_SHORT_NAME_LENGTH = 50;

// --- Organization ---

export const createOrganizationSchema = z.object({
  fullLegalName: z
    .string()
    .min(1, "Полное наименование организации обязательно")
    .max(
      MAX_NAME_LENGTH,
      `Название не должно превышать ${MAX_NAME_LENGTH} символов`,
    ),
  shortName: z
    .string()
    .max(
      MAX_SHORT_NAME_LENGTH,
      `Краткое название не должно превышать ${MAX_SHORT_NAME_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
  isLegalEntity: z.boolean(),
  registrationDate: z.string().min(1, "Укажите дату регистрации"),
  legalForm: z.nativeEnum(LegalForm, {
    error: "Укажите организационно-правовую форму",
  }),
  organizationType: z.nativeEnum(OrganizationType, {
    error: "Укажите тип организации",
  }),
  primaryContactType: z.nativeEnum(ContactType, {
    error: "Укажите тип контакта",
  }),
  primaryContactValue: z
    .string()
    .min(1, "Значение контакта обязательно")
    .max(255, "Контакт не должен превышать 255 символов"),
  primaryContactDescription: z
    .string()
    .min(1, "Описание контакта обязательно")
    .max(500, "Описание не должно превышать 500 символов"),
});

export type CreateOrganizationInput = z.infer<typeof createOrganizationSchema>;

export const updateOrganizationSchema = z.object({
  fullLegalName: z
    .string()
    .min(1, "Полное наименование организации обязательно")
    .max(
      MAX_NAME_LENGTH,
      `Название не должно превышать ${MAX_NAME_LENGTH} символов`,
    ),
  shortName: z
    .string()
    .max(
      MAX_SHORT_NAME_LENGTH,
      `Краткое название не должно превышать ${MAX_SHORT_NAME_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
  organizationType: z.nativeEnum(OrganizationType, {
    error: "Укажите тип организации",
  }),
  legalForm: z.nativeEnum(LegalForm, {
    error: "Укажите организационно-правовую форму",
  }),
});

export type UpdateOrganizationInput = z.infer<typeof updateOrganizationSchema>;
