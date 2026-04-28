import type { z } from "zod";

import { updateOrganizationSchema } from "@workspace/validations/company";

export { updateOrganizationSchema as editFormSchema };
export type EditFormValues = z.infer<typeof updateOrganizationSchema>;

export function pluralRu(
  n: number,
  one: string,
  few: string,
  many: string,
): string {
  const mod10 = Math.abs(n) % 10;
  const mod100 = Math.abs(n) % 100;
  if (mod10 === 1 && mod100 !== 11) return one;
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return few;
  return many;
}
