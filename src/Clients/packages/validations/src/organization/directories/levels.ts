import { z } from "zod";

export const levelDirectorySchema = z.object({
  name: z
    .string()
    .min(1, "Название обязательно")
    .max(64, "Название не должно превышать 64 символа"),
  description: z
    .string()
    .max(256, "Описание не должно превышать 256 символов")
    .optional()
    .or(z.literal("")),
});

export type LevelDirectoryFormValues = z.infer<typeof levelDirectorySchema>;
