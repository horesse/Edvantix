import { z } from "zod";

export const levelDirectorySchema = z.object({
  name: z
    .string()
    .min(1, "Название обязательно")
    .max(64, "Название не должно превышать 64 символа"),
  code: z
    .string()
    .min(1, "Код обязателен")
    .max(16, "Код не должен превышать 16 символов")
    .regex(
      /^[A-Z0-9_-]+$/,
      "Код может содержать только латинские буквы (A–Z), цифры, дефисы и подчёркивания"
    ),
  description: z
    .string()
    .max(256, "Описание не должно превышать 256 символов")
    .optional()
    .or(z.literal("")),
  tone: z.string().default("indigo"),
  isArchived: z.boolean().default(false),
});

export type LevelDirectoryFormValues = z.infer<typeof levelDirectorySchema>;
