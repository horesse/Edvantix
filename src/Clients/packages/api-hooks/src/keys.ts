export const blogKeys = {
  all: ["blog"] as const,
  posts: (query?: unknown) => [...blogKeys.all, "posts", query] as const,
  adminPosts: (query?: unknown) =>
    [...blogKeys.all, "admin", "posts", query] as const,
  adminPost: (id: string) => [...blogKeys.all, "admin", "posts", id] as const,
  post: (slug: string) => [...blogKeys.all, "posts", slug] as const,
  categories: () => [...blogKeys.all, "categories"] as const,
  tags: () => [...blogKeys.all, "tags"] as const,
};

export const profileKeys = {
  all: ["persona"] as const,
  profile: () => [...profileKeys.all, "own"] as const,
  details: () => [...profileKeys.all, "details"] as const,
};

export const companyKeys = {
  all: ["organizational"] as const,
  myOrganizations: () => [...companyKeys.all, "organizations", "my"] as const,
  organization: (id: string) =>
    [...companyKeys.all, "organizations", id] as const,
  members: (orgId: string, query?: unknown) =>
    [...companyKeys.all, "members", orgId, query] as const,
  invitations: (orgId: string) =>
    [...companyKeys.all, "invitations", orgId] as const,
  myInvitations: () => [...companyKeys.all, "invitations", "my"] as const,
  groups: (orgId: string, query?: unknown) =>
    [...companyKeys.all, "groups", orgId, query] as const,
  group: (id: string) => [...companyKeys.all, "group", id] as const,
  myGroups: (query?: unknown) =>
    [...companyKeys.all, "groups", "my", query] as const,
  groupMembers: (groupId: string, query?: unknown) =>
    [...companyKeys.all, "groupMembers", groupId, query] as const,
  contacts: (orgId: string, query?: unknown) =>
    [...companyKeys.all, "contacts", orgId, query] as const,
  legalForms: () => [...companyKeys.all, "legal-forms"] as const,
};

export const notificationKeys = {
  all: ["notifications"] as const,
  list: (params?: unknown) =>
    [...notificationKeys.all, "list", params] as const,
  unreadCount: () => [...notificationKeys.all, "unread-count"] as const,
};
