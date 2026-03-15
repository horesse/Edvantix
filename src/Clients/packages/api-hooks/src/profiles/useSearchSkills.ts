import { useQuery } from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";

export function useSearchSkills(query: string) {
  return useQuery({
    queryKey: ["skills", "search", query],
    queryFn: () => profileApiClient.searchSkills(query),
    enabled: query.trim().length >= 1,
    staleTime: 30_000,
  });
}
