import { z } from "zod";

import { ContactType, Gender } from "@workspace/types/profile";

const MAX_NAME_LENGTH = 100;
const MIN_AGE = 14;
const MAX_AGE = 120;

const MAX_CONTACT_VALUE_LENGTH = 255;
const MAX_CONTACT_DESCRIPTION_LENGTH = 500;

const MAX_WORKPLACE_LENGTH = 200;
const MAX_POSITION_LENGTH = 200;
const MAX_EMPLOYMENT_DESCRIPTION_LENGTH = 1000;

const MAX_INSTITUTION_LENGTH = 200;
const MAX_SPECIALTY_LENGTH = 200;

export const profileSettingsSchema = z.object({
  lastName: z
    .string()
    .min(1, "Фамилия является обязательным полем")
    .max(
      MAX_NAME_LENGTH,
      `Фамилия не должна превышать ${MAX_NAME_LENGTH} символов`,
    ),
  firstName: z
    .string()
    .min(1, "Имя является обязательным полем")
    .max(
      MAX_NAME_LENGTH,
      `Имя не должно превышать ${MAX_NAME_LENGTH} символов`,
    ),
  middleName: z
    .string()
    .max(
      MAX_NAME_LENGTH,
      `Отчество не должно превышать ${MAX_NAME_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
  birthDate: z
    .string()
    .min(1, "Дата рождения является обязательным полем")
    .refine((value) => {
      const date = new Date(value);
      if (Number.isNaN(date.getTime())) return false;

      const today = new Date();
      const age = today.getFullYear() - date.getFullYear();
      const monthDiff = today.getMonth() - date.getMonth();
      const dayDiff = today.getDate() - date.getDate();
      const exactAge =
        monthDiff < 0 || (monthDiff === 0 && dayDiff < 0) ? age - 1 : age;

      return exactAge >= MIN_AGE && exactAge <= MAX_AGE;
    }, `Возраст должен быть от ${MIN_AGE} до ${MAX_AGE} лет`),
  gender: z.nativeEnum(Gender, {
    error: "Указан некорректный пол",
  }),
});

export type ProfileSettingsInput = z.infer<typeof profileSettingsSchema>;

export const contactSchema = z.object({
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

export type ContactInput = z.infer<typeof contactSchema>;

export const employmentSchema = z.object({
  workplace: z
    .string()
    .min(1, "Место работы обязательно")
    .max(
      MAX_WORKPLACE_LENGTH,
      `Место работы не должно превышать ${MAX_WORKPLACE_LENGTH} символов`,
    ),
  position: z
    .string()
    .min(1, "Должность обязательна")
    .max(
      MAX_POSITION_LENGTH,
      `Должность не должна превышать ${MAX_POSITION_LENGTH} символов`,
    ),
  startDate: z.string().min(1, "Дата начала обязательна"),
  endDate: z.string().optional().or(z.literal("")),
  description: z
    .string()
    .max(
      MAX_EMPLOYMENT_DESCRIPTION_LENGTH,
      `Описание не должно превышать ${MAX_EMPLOYMENT_DESCRIPTION_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
});

export type EmploymentInput = z.infer<typeof employmentSchema>;

export const educationSchema = z.object({
  institution: z
    .string()
    .min(1, "Учебное заведение обязательно")
    .max(
      MAX_INSTITUTION_LENGTH,
      `Название не должно превышать ${MAX_INSTITUTION_LENGTH} символов`,
    ),
  specialty: z
    .string()
    .max(
      MAX_SPECIALTY_LENGTH,
      `Специальность не должна превышать ${MAX_SPECIALTY_LENGTH} символов`,
    )
    .optional()
    .or(z.literal("")),
  dateStart: z.string().min(1, "Дата начала обязательна"),
  dateEnd: z.string().optional().or(z.literal("")),
  educationLevelId: z.number({ message: "Уровень образования обязателен" }),
});

export type EducationInput = z.infer<typeof educationSchema>;
