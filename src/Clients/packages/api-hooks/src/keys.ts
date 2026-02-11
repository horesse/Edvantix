export const profileKeys = {
  all: ["profile"] as const,
  profile: () => [...profileKeys.all, "own"] as const,
  details: () => [...profileKeys.all, "details"] as const,
};
