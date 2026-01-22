export const profileKeys = {
  all: "profile",
  profile: () => [...profileKeys.all] as const,
};
