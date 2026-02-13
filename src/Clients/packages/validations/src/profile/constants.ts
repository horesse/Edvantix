import { z } from "zod";

import { Gender } from "@workspace/types/profile";

export const MAX_NAME_LENGTH = 100;
export const MIN_AGE = 14;
export const MAX_AGE = 120;
export const MAX_AVATAR_SIZE = 5 * 1024 * 1024;
export const ALLOWED_IMAGE_TYPES = [
  "image/jpeg",
  "image/jpg",
  "image/png",
  "image/gif",
  "image/webp",
];

/** Reusable name field schema with configurable label for error messages. */
export function nameField(label: string) {
  return z
    .string()
    .min(1, `${label} является обязательным полем`)
    .max(
      MAX_NAME_LENGTH,
      `${label} не должна превышать ${MAX_NAME_LENGTH} символов`,
    );
}

/** Optional middle name field — allows empty string. */
export const middleNameField = z
  .string()
  .max(
    MAX_NAME_LENGTH,
    `Отчество не должно превышать ${MAX_NAME_LENGTH} символов`,
  )
  .optional()
  .or(z.literal(""));

/** Birth date field with age range validation. */
export const birthDateField = z
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
  }, `Возраст должен быть от ${MIN_AGE} до ${MAX_AGE} лет`);

/** Gender enum field. */
export const genderField = z.nativeEnum(Gender, {
  error: "Указан некорректный пол",
});
