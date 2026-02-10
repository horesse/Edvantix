import { z } from "zod";

import { Gender } from "@workspace/types/profile";

const MAX_NAME_LENGTH = 100;
const MIN_AGE = 14;
const MAX_AGE = 120;
const MAX_AVATAR_SIZE = 5 * 1024 * 1024;
const ALLOWED_IMAGE_TYPES = [
  "image/jpeg",
  "image/jpg",
  "image/png",
  "image/gif",
  "image/webp",
];

export const registrationSchema = z.object({
  lastName: z
    .string()
    .min(1, "Фамилия является обязательным полем")
    .max(MAX_NAME_LENGTH, `Фамилия не должна превышать ${MAX_NAME_LENGTH} символов`),
  firstName: z
    .string()
    .min(1, "Имя является обязательным полем")
    .max(MAX_NAME_LENGTH, `Имя не должно превышать ${MAX_NAME_LENGTH} символов`),
  middleName: z
    .string()
    .max(MAX_NAME_LENGTH, `Отчество не должно превышать ${MAX_NAME_LENGTH} символов`)
    .optional()
    .or(z.literal("")),
  birthDate: z
    .string()
    .min(1, "Дата рождения является обязательным полем")
    .refine(
      (value) => {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return false;

        const today = new Date();
        const age = today.getFullYear() - date.getFullYear();
        const monthDiff = today.getMonth() - date.getMonth();
        const dayDiff = today.getDate() - date.getDate();
        const exactAge =
          monthDiff < 0 || (monthDiff === 0 && dayDiff < 0) ? age - 1 : age;

        return exactAge >= MIN_AGE && exactAge <= MAX_AGE;
      },
      `Возраст должен быть от ${MIN_AGE} до ${MAX_AGE} лет`,
    ),
  gender: z.nativeEnum(Gender, {
    error: "Указан некорректный пол",
  }),
  avatar: z
    .instanceof(File)
    .refine(
      (file) => file.size <= MAX_AVATAR_SIZE,
      "Размер файла не должен превышать 5 МБ",
    )
    .refine(
      (file) => ALLOWED_IMAGE_TYPES.includes(file.type),
      "Допустимые форматы: JPEG, PNG, GIF, WebP",
    )
    .nullish(),
});

export type RegistrationFormData = z.infer<typeof registrationSchema>;
