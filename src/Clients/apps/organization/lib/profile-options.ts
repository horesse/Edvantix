import { Gender } from "@workspace/types/profile";

export const genderOptions = [
  { value: Gender.Male, label: "Мужской" },
  { value: Gender.Female, label: "Женский" },
  { value: Gender.None, label: "Не указан" },
] as const;
