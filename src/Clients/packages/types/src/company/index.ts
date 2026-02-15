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

// --- Organization ---

export type OrganizationModel = {
  id: number;
  name: string;
  nameLatin: string;
  shortName: string;
  printName?: string | null;
  description?: string | null;
  registrationDate: string;
  membersCount: number;
  groupsCount: number;
};

export type OrganizationSummaryModel = {
  id: number;
  name: string;
  shortName: string;
  description?: string | null;
  role: string;
};

export type CreateOrganizationRequest = {
  name: string;
  nameLatin: string;
  shortName: string;
  printName?: string | null;
  description?: string | null;
};

export type UpdateOrganizationRequest = {
  name: string;
  nameLatin: string;
  shortName: string;
  printName?: string | null;
  description?: string | null;
};

// --- Members ---

export type OrganizationMemberModel = {
  id: string;
  organizationId: number;
  profileId: number;
  role: OrganizationRole;
  joinedAt: string;
  displayName?: string | null;
};

export type AddMemberRequest = {
  profileId: number;
  role: OrganizationRole;
};

export type UpdateMemberRoleRequest = {
  newRole: OrganizationRole;
};

// --- Invitations ---

export type InvitationModel = {
  id: string;
  organizationId: number;
  organizationName?: string | null;
  invitedByProfileId: number;
  inviteeProfileId?: number | null;
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
  inviteeProfileId?: number | null;
  role: OrganizationRole;
  ttlDays?: number;
};

// --- Groups ---

export type GroupModel = {
  id: number;
  organizationId: number;
  name: string;
  description?: string | null;
  membersCount: number;
};

export type GroupSummaryModel = {
  id: number;
  organizationId: number;
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
  groupId: number;
  profileId: number;
  role: GroupRole;
  joinedAt: string;
  displayName?: string | null;
};

export type AddGroupMemberRequest = {
  profileId: number;
  role: GroupRole;
};

export type UpdateGroupMemberRoleRequest = {
  newRole: GroupRole;
};

// --- Organization Contacts ---

export type OrganizationContactModel = {
  id: number;
  organizationId: number;
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
