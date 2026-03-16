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

export type Skill = {
  id: string;
  name: string;
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
  bio?: string | null;
  contacts: Contact[];
  employmentHistories: EmploymentHistory[];
  educations: Education[];
  skills: Skill[];
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

export type UpdateProfileRequest = {
  firstName: string;
  lastName: string;
  middleName?: string | null;
  birthDate: string;
  bio?: string | null;
  contacts: ContactRequest[];
  educations: EducationRequest[];
  employmentHistories: EmploymentHistoryRequest[];
  /** Названия навыков. Дубликаты и регистр обрабатываются на бэкенде. */
  skills: string[];
};

export type RegisterProfileRequest = {
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  gender: Gender;
  avatar?: File | null;
};

export type SkillSearchResult = {
  id: string;
  name: string;
};
