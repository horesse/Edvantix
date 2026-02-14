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
  invitations: (orgId: number) =>
    [...companyKeys.all, "invitations", orgId] as const,
  myInvitations: () => [...companyKeys.all, "invitations", "my"] as const,
  groups: (orgId: number) => [...companyKeys.all, "groups", orgId] as const,
  group: (id: number) => [...companyKeys.all, "group", id] as const,
  myGroups: () => [...companyKeys.all, "groups", "my"] as const,
  groupMembers: (groupId: number) =>
    [...companyKeys.all, "groupMembers", groupId] as const,
  contacts: (orgId: number) => [...companyKeys.all, "contacts", orgId] as const,
};
