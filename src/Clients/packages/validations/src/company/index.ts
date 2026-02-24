import { z } from "zod";

import { OrganizationRole } from "@workspace/types/company";
import { ContactType } from "@workspace/types/profile";

const MAX_NAME_LENGTH = 200;
const MAX_SHORT_NAME_LENGTH = 50;
const MAX_DESCRIPTION_LENGTH = 1000;
const MAX_CONTACT_VALUE_LENGTH = 255;
const MAX_CONTACT_DESCRIPTION_LENGTH = 500;
const MAX_EMAIL_LENGTH = 255;
const MIN_TTL_DAYS = 1;
const MAX_TTL_DAYS = 30;

// --- Organization ---

export const createOrganizationSchema = z.object({
  name: z
    .string()
    .min(1, "Название организации обязательно")
    .max(
      MAX_NAME_LENGTH,
      `Название не должно превышать ${MAX_NAME_LENGTH} символов`,
    ),
  nameLatin: z
    .string()
    .min(1, "Латинское название обязательно")
    .max(
      MAX_NAME_LENGTH,
      `Название не должно превышать ${MAX_NAME_LENGTH} символов`,
    ),
  shortName: z
    .string()
    .min(1, "Краткое название обязательно")
    .max(
      MAX_SHORT_NAME_LENGTH,
      `Краткое название не должно превышать ${MAX_SHORT_NAME_LENGTH} символов`,
    ),
  printName: z
    .string()
    .max(
      MAX_NAME_LENGTH,
      `Название не должно превышать ${MAX_NAME_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
  description: z
    .string()
    .max(
      MAX_DESCRIPTION_LENGTH,
      `Описание не должно превышать ${MAX_DESCRIPTION_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
});

export type CreateOrganizationInput = z.infer<typeof createOrganizationSchema>;

export const updateOrganizationSchema = createOrganizationSchema;

export type UpdateOrganizationInput = z.infer<typeof updateOrganizationSchema>;

// --- Invitations ---

export const createInvitationSchema = z.object({
  inviteeEmail: z
    .string()
    .max(
      MAX_EMAIL_LENGTH,
      `Email не должен превышать ${MAX_EMAIL_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
  inviteeProfileId: z
    .string()
    .uuid("Некорректный ID профиля")
    .optional()
    .or(z.literal("")),
  role: z.nativeEnum(OrganizationRole, {
    error: "Укажите роль",
  }),
  ttlDays: z
    .number()
    .min(MIN_TTL_DAYS, `Минимальный срок — ${MIN_TTL_DAYS} день`)
    .max(MAX_TTL_DAYS, `Максимальный срок — ${MAX_TTL_DAYS} дней`),
});

export type CreateInvitationInput = z.infer<typeof createInvitationSchema>;

// --- Groups ---

export const createGroupSchema = z.object({
  name: z
    .string()
    .min(1, "Название группы обязательно")
    .max(
      MAX_NAME_LENGTH,
      `Название не должно превышать ${MAX_NAME_LENGTH} символов`,
    ),
  description: z
    .string()
    .max(
      MAX_DESCRIPTION_LENGTH,
      `Описание не должно превышать ${MAX_DESCRIPTION_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
});

export type CreateGroupInput = z.infer<typeof createGroupSchema>;

export const updateGroupSchema = createGroupSchema;

export type UpdateGroupInput = z.infer<typeof updateGroupSchema>;

// --- Organization Contacts ---

export const organizationContactSchema = z.object({
  type: z.nativeEnum(ContactType, {
    error: "Указан некорректный тип контакта",
  }),
  value: z
    .string()
    .min(1, "Значение контакта обязательно")
    .max(
      MAX_CONTACT_VALUE_LENGTH,
      `Значение не должно превышать ${MAX_CONTACT_VALUE_LENGTH} символов`,
    ),
  description: z
    .string()
    .max(
      MAX_CONTACT_DESCRIPTION_LENGTH,
      `Описание не должно превышать ${MAX_CONTACT_DESCRIPTION_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
});

export type OrganizationContactInput = z.infer<
  typeof organizationContactSchema
>;
