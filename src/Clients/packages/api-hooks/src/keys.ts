export const profileKeys = {
  all: ["profile"] as const,
  profile: () => [...profileKeys.all, "own"] as const,
  details: () => [...profileKeys.all, "details"] as const,
};

export const companyKeys = {
  all: ["company"] as const,
  myOrganizations: () => [...companyKeys.all, "organizations", "my"] as const,
  organization: (id: number) =>
    [...companyKeys.all, "organizations", id] as const,
  members: (orgId: number) => [...companyKeys.all, "members", orgId] as const,
  membersPaginated: (orgId: number, query?: unknown) =>
    [...companyKeys.all, "members", orgId, "paginated", query] as const,
  invitations: (orgId: number) =>
    [...companyKeys.all, "invitations", orgId] as const,
  pendingInvitationsPaginated: (orgId: number, query?: unknown) =>
    [...companyKeys.all, "invitations", orgId, "paginated", query] as const,
  myInvitations: () => [...companyKeys.all, "invitations", "my"] as const,
  groups: (orgId: number) => [...companyKeys.all, "groups", orgId] as const,
  groupsPaginated: (orgId: number, query?: unknown) =>
    [...companyKeys.all, "groups", orgId, "paginated", query] as const,
  group: (id: number) => [...companyKeys.all, "group", id] as const,
  myGroups: () => [...companyKeys.all, "groups", "my"] as const,
  groupMembers: (groupId: number) =>
    [...companyKeys.all, "groupMembers", groupId] as const,
  groupMembersPaginated: (groupId: number, query?: unknown) =>
    [...companyKeys.all, "groupMembers", groupId, "paginated", query] as const,
  contacts: (orgId: number) => [...companyKeys.all, "contacts", orgId] as const,
};
