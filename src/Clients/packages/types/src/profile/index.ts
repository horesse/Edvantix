export type OwnProfile = {
  id: string;
  name: string;
  userName: string;
  avatarUrl?: string;
};

export enum Gender {
  Male = 1,
  Female = 2,
  None = 3,
}

export enum ContactType {
  Email = 1,
  Phone = 2,
  Uri = 3,
  Other = 4,
}

export enum EducationLevel {
  Preschool = 1,
  GeneralSecondary = 2,
  VocationalTechnical = 3,
  SecondarySpecialized = 4,
  HigherBachelor = 5,
  HigherMaster = 6,
  Postgraduate = 7,
  AdditionalChildren = 8,
  AdditionalAdults = 9,
  Special = 10,
}

export type Contact = {
  type: ContactType;
  value: string;
  description?: string | null;
};

export type EmploymentHistory = {
  workplace: string;
  position: string;
  startDate: string;
  endDate?: string | null;
  description?: string | null;
};

export type Education = {
  dateStart: string;
  dateEnd?: string | null;
  institution: string;
  specialty?: string | null;
  educationLevel: EducationLevel;
};

export type OwnProfileDetails = {
  id: string;
  accountId: string;
  login: string;
  gender: Gender;
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  avatarUrl?: string | null;
  contacts: Contact[];
  employmentHistories: EmploymentHistory[];
  educations: Education[];
};

export type ContactRequest = {
  type: ContactType;
  value: string;
  description?: string | null;
};

export type EmploymentHistoryRequest = {
  workplace: string;
  position: string;
  startDate: string;
  endDate?: string | null;
  description?: string | null;
};

export type EducationRequest = {
  dateStart: string;
  institution: string;
  level: EducationLevel;
  specialty?: string | null;
  dateEnd?: string | null;
};

export type UpdatePersonalInfoRequest = {
  firstName: string;
  lastName: string;
  middleName?: string | null;
  birthDate: string;
};

export type UpdateContactsRequest = {
  contacts: ContactRequest[];
};

export type UpdateEducationRequest = {
  educations: EducationRequest[];
};

export type UpdateEmploymentRequest = {
  employmentHistories: EmploymentHistoryRequest[];
};

export type RegisterProfileRequest = {
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  gender: Gender;
  avatar?: File | null;
};
