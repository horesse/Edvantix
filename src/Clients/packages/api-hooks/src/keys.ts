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

export const organizationKeys = {
  all: ["organizational"] as const,
  myOrganizations: () =>
    [...organizationKeys.all, "organizations", "mine"] as const,
  organizations: (query?: unknown) =>
    [...organizationKeys.all, "organizations", query] as const,
  organization: (id: string) =>
    [...organizationKeys.all, "organizations", id] as const,
  members: (orgId: string, query?: unknown) =>
    [...organizationKeys.all, "members", orgId, query] as const,
  membersKpi: (orgId: string) =>
    [...organizationKeys.all, "members", orgId, "kpi"] as const,
  member: (id: string) => [...organizationKeys.all, "member", id] as const,
  roles: (orgId: string, query?: unknown) =>
    [...organizationKeys.all, "roles", orgId, query] as const,
  role: (orgId: string, roleId: string) =>
    [...organizationKeys.all, "roles", orgId, roleId] as const,
  organizationSummary: (orgId: string) =>
    [...organizationKeys.all, "organization", orgId, "summary"] as const,
  directoriesCatalog: (orgId: string) =>
    [...organizationKeys.all, "settings", "directories", orgId] as const,
  rolesSummary: (orgId: string) =>
    [...organizationKeys.all, "roles", orgId, "summary"] as const,
};

export const notificationKeys = {
  all: ["notifications"] as const,
  list: (params?: unknown) =>
    [...notificationKeys.all, "list", params] as const,
  unreadCount: () => [...notificationKeys.all, "unread-count"] as const,
};

export const adminKeys = {
  all: ["admin"] as const,
  profiles: (query?: unknown) => [...adminKeys.all, "profiles", query] as const,
  profile: (id: string) => [...adminKeys.all, "profile", id] as const,
};
