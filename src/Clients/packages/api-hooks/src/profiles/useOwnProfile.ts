import { UseQueryOptions, useQuery } from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import { OwnProfile } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function UseOwnProfile(
  options?: Omit<UseQueryOptions<OwnProfile>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: profileKeys.profile(),
    queryFn: () => profileApiClient.profile(),
    ...options,
  });
}
