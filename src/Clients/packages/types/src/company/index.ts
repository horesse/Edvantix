import type { ContactType } from "../profile";

// --- Enums ---

export enum OrganizationRole {
  Owner = 1,
  Manager = 2,
  Teacher = 3,
  Student = 4,
}

export enum GroupRole {
  Teacher = 1,
  Student = 2,
  Manager = 3,
}

export enum InvitationStatus {
  Pending = 1,
  Accepted = 2,
  Declined = 3,
  Cancelled = 4,
  Expired = 5,
}

/** Тип организации (информационное поле). */
export enum OrganizationType {
  EducationalInstitution = 1,
  GeneralSecondaryEducation = 2,
  Lyceum = 3,
  Gymnasium = 4,
  College = 5,
  VocationalTechnicalSchool = 6,
  University = 7,
  AdditionalEducationForYouth = 8,
  Preschool = 9,
  PrivateEducationalCenter = 10,
  TrainingCenter = 11,
  LlcEducational = 12,
  IndividualEntrepreneur = 13,
  LanguageSchool = 14,
  ItSchool = 15,
  TutoringCenter = 16,
  OnlinePlatform = 17,
}

/** Русские названия типов организаций. */
export const ORGANIZATION_TYPE_LABELS: Record<OrganizationType, string> = {
  [OrganizationType.EducationalInstitution]: "Учреждение образования",
  [OrganizationType.GeneralSecondaryEducation]:
    "Учреждение общего среднего образования",
  [OrganizationType.Lyceum]: "Лицей",
  [OrganizationType.Gymnasium]: "Гимназия",
  [OrganizationType.College]: "Колледж",
  [OrganizationType.VocationalTechnicalSchool]:
    "Профессионально-техническое училище",
  [OrganizationType.University]: "Университет, институт",
  [OrganizationType.AdditionalEducationForYouth]:
    "Учреждение дополнительного образования детей и молодёжи",
  [OrganizationType.Preschool]: "Дошкольное учреждение образования",
  [OrganizationType.PrivateEducationalCenter]: "Частный образовательный центр",
  [OrganizationType.TrainingCenter]: "Учебный центр, обучающая компания",
  [OrganizationType.LlcEducational]: "ООО в сфере образования",
  [OrganizationType.IndividualEntrepreneur]: "Индивидуальный предприниматель",
  [OrganizationType.LanguageSchool]: "Языковая школа",
  [OrganizationType.ItSchool]: "IT-школа, школа программирования",
  [OrganizationType.TutoringCenter]: "Репетиторский центр",
  [OrganizationType.OnlinePlatform]: "Онлайн-платформа",
};

// --- Legal Forms ---

export type LegalFormModel = {
  id: string;
  name: string;
  shortName: string;
};

// --- Organization ---

export type OrganizationModel = {
  id: string;
  name: string;
  nameLatin: string;
  shortName: string;
  printName?: string | null;
  description?: string | null;
  registrationDate: string;
  membersCount: number;
  groupsCount: number;
  organizationType: OrganizationType;
  legalForm: LegalFormModel;
};

export type OrganizationSummaryModel = {
  id: string;
  name: string;
  shortName: string;
  description?: string | null;
  role: string;
};

export type CreateOrganizationRequest = {
  name: string;
  nameLatin: string;
  shortName: string;
  organizationType: OrganizationType;
  legalFormId: string;
  printName?: string | null;
  description?: string | null;
};

export type UpdateOrganizationRequest = {
  name: string;
  nameLatin: string;
  shortName: string;
  organizationType: OrganizationType;
  legalFormId: string;
  printName?: string | null;
  description?: string | null;
};

// --- Members ---

export type OrganizationMemberModel = {
  id: string;
  organizationId: string;
  profileId: string;
  role: OrganizationRole;
  joinedAt: string;
  displayName?: string | null;
};

export type AddMemberRequest = {
  profileId: string;
  role: OrganizationRole;
};

export type UpdateMemberRoleRequest = {
  newRole: OrganizationRole;
};

// --- Invitations ---

export type InvitationModel = {
  id: string;
  organizationId: string;
  organizationName?: string | null;
  invitedByProfileId: string;
  inviteeProfileId?: string | null;
  inviteeEmail?: string | null;
  role: OrganizationRole;
  status: InvitationStatus;
  token: string;
  createdAt: string;
  expiresAt: string;
  respondedAt?: string | null;
};

export type CreateInvitationRequest = {
  inviteeEmail?: string | null;
  inviteeProfileId?: string | null;
  role: OrganizationRole;
  ttlDays?: number;
};

// --- Groups ---

export type GroupModel = {
  id: string;
  organizationId: string;
  name: string;
  description?: string | null;
  membersCount: number;
};

export type GroupSummaryModel = {
  id: string;
  organizationId: string;
  name: string;
  description?: string | null;
  role: string;
};

export type CreateGroupRequest = {
  name: string;
  description?: string | null;
};

export type UpdateGroupRequest = {
  name: string;
  description?: string | null;
};

// --- Group Members ---

export type GroupMemberModel = {
  id: string;
  groupId: string;
  profileId: string;
  role: GroupRole;
  joinedAt: string;
  displayName?: string | null;
};

export type AddGroupMemberRequest = {
  profileId: string;
  role: GroupRole;
};

export type UpdateGroupMemberRoleRequest = {
  newRole: GroupRole;
};

// --- Organization Contacts ---

export type OrganizationContactModel = {
  id: string;
  organizationId: string;
  type: ContactType;
  value: string;
  description?: string | null;
};

export type AddOrganizationContactRequest = {
  type: ContactType;
  value: string;
  description?: string | null;
};

export type UpdateOrganizationContactRequest = {
  type: ContactType;
  value: string;
  description?: string | null;
};
