import {
  GroupRole,
  InvitationStatus,
  OrganizationRole,
} from "@workspace/types/company";
import { ContactType } from "@workspace/types/profile";

export const organizationRoleLabels: Record<OrganizationRole, string> = {
  [OrganizationRole.Owner]: "Владелец",
  [OrganizationRole.Manager]: "Менеджер",
  [OrganizationRole.Teacher]: "Преподаватель",
  [OrganizationRole.Student]: "Ученик",
};

export const groupRoleLabels: Record<GroupRole, string> = {
  [GroupRole.Teacher]: "Преподаватель",
  [GroupRole.Student]: "Ученик",
  [GroupRole.Manager]: "Менеджер",
};

export const invitationStatusLabels: Record<InvitationStatus, string> = {
  [InvitationStatus.Pending]: "Ожидает",
  [InvitationStatus.Accepted]: "Принято",
  [InvitationStatus.Declined]: "Отклонено",
  [InvitationStatus.Cancelled]: "Отменено",
  [InvitationStatus.Expired]: "Истекло",
};

export const contactTypeLabels: Record<ContactType, string> = {
  [ContactType.Email]: "Email",
  [ContactType.Phone]: "Телефон",
  [ContactType.Uri]: "Веб-сайт",
  [ContactType.Other]: "Другое",
};

/** Parses an OrganizationRole from a string role name returned by the API. */
export function parseOrganizationRole(role: string): OrganizationRole | null {
  const map: Record<string, OrganizationRole> = {
    Owner: OrganizationRole.Owner,
    Manager: OrganizationRole.Manager,
    Teacher: OrganizationRole.Teacher,
    Student: OrganizationRole.Student,
  };
  return map[role] ?? null;
}
