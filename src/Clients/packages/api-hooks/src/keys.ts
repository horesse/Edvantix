export const blogKeys = {
  all: ["blog"] as const,
  posts: (query?: unknown) => [...blogKeys.all, "posts", query] as const,
  post: (slug: string) => [...blogKeys.all, "posts", slug] as const,
  categories: () => [...blogKeys.all, "categories"] as const,
  tags: () => [...blogKeys.all, "tags"] as const,
};

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
  members: (orgId: number, query?: unknown) =>
    [...companyKeys.all, "members", orgId, query] as const,
  invitations: (orgId: number) =>
    [...companyKeys.all, "invitations", orgId] as const,
  myInvitations: () => [...companyKeys.all, "invitations", "my"] as const,
  groups: (orgId: number, query?: unknown) =>
    [...companyKeys.all, "groups", orgId, query] as const,
  group: (id: number) => [...companyKeys.all, "group", id] as const,
  myGroups: (query?: unknown) =>
    [...companyKeys.all, "groups", "my", query] as const,
  groupMembers: (groupId: number, query?: unknown) =>
    [...companyKeys.all, "groupMembers", groupId, query] as const,
  contacts: (orgId: number, query?: unknown) =>
    [...companyKeys.all, "contacts", orgId, query] as const,
};
