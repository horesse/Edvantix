import type {
  Contact,
  ContactRequest,
  Education,
  EducationRequest,
  EmploymentHistory,
  EmploymentHistoryRequest,
  Skill,
} from "../profile";

export type AdminProfileDto = {
  id: string;
  accountId: string;
  fullName: string;
  userName: string;
  avatarUrl: string | null;
  isBlocked: boolean;
  lastLoginAt: string | null;
};

export type GetAdminProfilesRequest = {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
  isBlocked?: boolean;
};

export type SendAdminNotificationRequest = {
  title: string;
  message: string;
  /** NotificationType: 0=Info, 1=Success, 2=Warning, 3=Error, 6=System */
  type?: number;
};

export type AdminProfileDetailDto = {
  id: string;
  accountId: string;
  userName: string;
  firstName: string;
  lastName: string;
  middleName: string | null;
  gender: number;
  birthDate: string;
  bio: string | null;
  avatarUrl: string | null;
  isBlocked: boolean;
  lastLoginAt: string | null;
  contacts: Contact[];
  employmentHistories: EmploymentHistory[];
  educations: Education[];
  skills: Skill[];
};

export type AdminUpdateProfileRequest = {
  firstName: string;
  lastName: string;
  middleName?: string | null;
  birthDate: string;
  bio?: string | null;
  contacts: ContactRequest[];
  employmentHistories: EmploymentHistoryRequest[];
  educations: EducationRequest[];
  skills: string[];
  reason: string;
};
