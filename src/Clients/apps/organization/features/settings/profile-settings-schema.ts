import { z } from "zod";

import type {
  OwnProfileDetails,
  UpdateProfileRequest,
} from "@workspace/types/profile";
import {
  type ContactInput,
  type EducationInput,
  type EmploymentInput,
  contactSchema,
  educationSchema,
  employmentSchema,
  profileSettingsSchema,
} from "@workspace/validations/profile";

export const profileFormSchema = profileSettingsSchema.extend({
  contacts: z.array(contactSchema).default([]),
  employmentHistories: z.array(employmentSchema).default([]),
  educations: z.array(educationSchema).default([]),
});

export type ProfileFormValues = z.infer<typeof profileFormSchema>;

export type { ContactInput, EducationInput, EmploymentInput };

/** Extracts YYYY-MM-DD from either a date string or ISO datetime string. */
export function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

export function getDefaultValues(profile: OwnProfileDetails): ProfileFormValues {
  return {
    lastName: profile.lastName,
    firstName: profile.firstName,
    middleName: profile.middleName ?? "",
    birthDate: profile.birthDate,
    contacts: profile.contacts.map((c) => ({
      type: c.type,
      value: c.value,
      description: c.description ?? "",
    })),
    employmentHistories: profile.employmentHistories.map((e) => ({
      workplace: e.workplace,
      position: e.position,
      startDate: toDateString(e.startDate),
      endDate: toDateString(e.endDate),
      description: e.description ?? "",
    })),
    educations: profile.educations.map((e) => ({
      institution: e.institution,
      specialty: e.specialty ?? "",
      dateStart: toDateString(e.dateStart),
      dateEnd: toDateString(e.dateEnd),
      level: e.educationLevel,
    })),
  };
}

export function buildUpdateRequest(
  values: ProfileFormValues,
  avatar?: File,
): UpdateProfileRequest {
  return {
    firstName: values.firstName,
    lastName: values.lastName,
    middleName: values.middleName || null,
    birthDate: values.birthDate,
    contacts: values.contacts.map((c) => ({
      type: c.type,
      value: c.value,
      description: c.description || null,
    })),
    employmentHistories: values.employmentHistories.map((e) => ({
      workplace: e.workplace,
      position: e.position,
      startDate: e.startDate,
      endDate: e.endDate || null,
      description: e.description || null,
    })),
    educations: values.educations.map((e) => ({
      institution: e.institution,
      specialty: e.specialty || null,
      dateStart: e.dateStart,
      dateEnd: e.dateEnd || null,
      level: e.level,
    })),
    avatar,
  };
}
