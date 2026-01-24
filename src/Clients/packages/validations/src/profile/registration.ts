import { z } from "zod";

import { Gender } from "@workspace/types/profile";

export const registerProfileSchema = z.object({
  gender: z.nativeEnum(Gender, {
    required_error: "Пол обязателен для заполнения",
  }),
  firstName: z
    .string()
    .min(1, "Имя обязательно для заполнения")
    .max(50, "Имя должно содержать не более 50 символов"),
  lastName: z
    .string()
    .min(1, "Фамилия обязательна для заполнения")
    .max(50, "Фамилия должна содержать не более 50 символов"),
  middleName: z
    .string()
    .max(50, "Отчество должно содержать не более 50 символов")
    .optional()
    .nullable(),
  birthDate: z
    .string()
    .min(1, "Дата рождения обязательна для заполнения")
    .refine(
      (date) => {
        const birthDate = new Date(date);
        const today = new Date();
        const age = today.getFullYear() - birthDate.getFullYear();
        return age >= 14 && age <= 120;
      },
      { message: "Возраст должен быть от 14 до 120 лет" },
    ),
});

export type RegisterProfileInput = z.infer<typeof registerProfileSchema>;
