import type { ContactRequest, EducationRequest, EmploymentHistoryRequest } from "@workspace/types/profile";

/** Personal info payload returned by GeneralSection. */
export type GeneralData = {
  firstName: string;
  lastName: string;
  middleName: string | null;
  birthDate: string;
};

export interface GeneralSectionHandle {
  /** Validates the form. Returns payload or null if validation fails. */
  getPayload(): Promise<GeneralData | null>;
  acknowledgeServerState(): void;
}

export interface ContactsSectionHandle {
  getPayload(): ContactRequest[];
  acknowledgeServerState(): void;
}

export interface EmploymentSectionHandle {
  getPayload(): EmploymentHistoryRequest[];
  acknowledgeServerState(): void;
}

export interface EducationSectionHandle {
  getPayload(): EducationRequest[];
  acknowledgeServerState(): void;
}

export interface BioSectionHandle {
  getPayload(): string | null;
  acknowledgeServerState(): void;
}

export interface SkillsSectionHandle {
  /** Returns current skill names. */
  getPayload(): string[];
  acknowledgeServerState(): void;
}
