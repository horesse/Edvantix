import { z } from "zod";

export const AVATAR_OPTIONS = [
  { value: "av1", emoji: "🧑‍💼", bg: "#dbeafe" },
  { value: "av2", emoji: "👩‍🏫", bg: "#dcfce7" },
  { value: "av3", emoji: "🧑‍🎓", bg: "#fef9c3" },
  { value: "av4", emoji: "👨‍💻", bg: "#ede9fe" },
  { value: "av5", emoji: "👩‍💻", bg: "#fce7f3" },
  { value: "av6", emoji: "🧑‍🔬", bg: "#ffedd5" },
  { value: "av7", emoji: "👩‍🎨", bg: "#e0f2fe" },
  { value: "av8", emoji: "🧑‍🏫", bg: "#d1fae5" },
  { value: "av9", emoji: "👨‍🎓", bg: "#fee2e2" },
  { value: "av10", emoji: "🤓", bg: "#f3e8ff" },
] as const;

export const COUNTRIES = [
  { flag: "🇧🇾", name: "Беларусь", code: "+375" },
  { flag: "🇷🇺", name: "Россия", code: "+7" },
  { flag: "🇺🇦", name: "Украина", code: "+380" },
  { flag: "🇰🇿", name: "Казахстан", code: "+7" },
  { flag: "🇺🇿", name: "Узбекистан", code: "+998" },
  { flag: "🇦🇿", name: "Азербайджан", code: "+994" },
  { flag: "🇦🇲", name: "Армения", code: "+374" },
  { flag: "🇬🇪", name: "Грузия", code: "+995" },
  { flag: "🇲🇩", name: "Молдова", code: "+373" },
  { flag: "🇱🇹", name: "Литва", code: "+370" },
  { flag: "🇵🇱", name: "Польша", code: "+48" },
  { flag: "🇩🇪", name: "Германия", code: "+49" },
  { flag: "🇬🇧", name: "Великобритания", code: "+44" },
  { flag: "🇫🇷", name: "Франция", code: "+33" },
  { flag: "🇹🇷", name: "Турция", code: "+90" },
  { flag: "🇺🇸", name: "США", code: "+1" },
] as const;

export const avatarStepSchema = z.object({
  /** Either a preset emoji key or "upload" when a file was chosen. */
  avatarType: z.enum(["preset", "upload"]),
  presetValue: z.string().optional(),
  /** Base64 data URL of the uploaded file (client-side preview only). */
  uploadedDataUrl: z.string().optional(),
});

export const personalStepSchema = z.object({
  lastName: z.string().min(1, "Введите фамилию"),
  firstName: z.string().min(1, "Введите имя"),
  patronymic: z.string().optional(),
  birthDate: z.string().min(1, "Укажите дату рождения"),
  gender: z.enum(["male", "female", "other"]),
  countryCode: z.string().default("+375"),
  phone: z.string().optional(),
});

export const profileSetupSchema = avatarStepSchema.merge(personalStepSchema);

export type AvatarStepValues = z.infer<typeof avatarStepSchema>;
export type PersonalStepValues = z.infer<typeof personalStepSchema>;
export type ProfileSetupValues = z.infer<typeof profileSetupSchema>;
