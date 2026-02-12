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
  educationLevelId: number;
  educationLevelName?: string | null;
};

export type OwnProfileDetails = {
  gender: Gender;
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  avatarUrl?: string | null;
  contacts?: Contact[] | null;
  employmentHistories?: EmploymentHistory[] | null;
  educations?: Education[] | null;
};

export type UpdateProfileRequest = {
  gender: Gender;
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  contacts?: UpdateContactRequest[] | null;
  employmentHistories?: UpdateEmploymentHistoryRequest[] | null;
  educations?: UpdateEducationRequest[] | null;
};

export type UpdateContactRequest = {
  type: ContactType;
  value: string;
  description?: string | null;
};

export type UpdateEmploymentHistoryRequest = {
  workplace: string;
  position: string;
  startDate: string;
  endDate?: string | null;
  description?: string | null;
};

export type UpdateEducationRequest = {
  dateStart: string;
  dateEnd?: string | null;
  institution: string;
  specialty?: string | null;
  educationLevelId: number;
};

export type RegisterProfileRequest = {
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  gender: Gender;
  avatar?: File | null;
};
