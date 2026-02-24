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
  id: string;
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
