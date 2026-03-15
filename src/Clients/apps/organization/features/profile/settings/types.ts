import type { Gender } from "@workspace/types/profile";

/** Ref handle exposed by each profile section for external submit triggering. */
export interface SectionHandle {
  submit(): void;
}

export interface GeneralSectionHandle {
  /** Trigger form validation. Returns validated data or null if invalid. */
  validate(): Promise<GeneralData | null>;
  isDirty(): boolean;
}

export type GeneralData = {
  firstName: string;
  lastName: string;
  middleName: string | null;
  birthDate: string;
  gender: Gender;
};
