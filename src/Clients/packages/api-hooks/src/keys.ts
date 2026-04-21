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
  myOrganizations: () => [...companyKeys.all, "organizations", "mine"] as const,
  organizations: (query?: unknown) =>
    [...companyKeys.all, "organizations", query] as const,
  organization: (id: string) =>
    [...companyKeys.all, "organizations", id] as const,
  members: (orgId: string, query?: unknown) =>
    [...companyKeys.all, "members", orgId, query] as const,
  member: (id: string) => [...companyKeys.all, "member", id] as const,
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
