import { z } from "zod";

import {
  ALLOWED_IMAGE_TYPES,
  MAX_AVATAR_SIZE,
  birthDateField,
  genderField,
  middleNameField,
  nameField,
} from "./constants";

export const registrationSchema = z.object({
  lastName: nameField("Фамилия"),
  firstName: nameField("Имя"),
  middleName: middleNameField,
  birthDate: birthDateField,
  gender: genderField,
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
