import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { OwnProfile } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useOwnProfile(
  options?: Omit<UseQueryOptions<OwnProfile>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: profileKeys.profile(),
    queryFn: () => profileApiClient.getProfile(),
    ...options,
  });
}
