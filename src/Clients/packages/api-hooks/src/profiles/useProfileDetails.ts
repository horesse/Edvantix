import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { OwnProfileDetails } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useProfileDetails(
  options?: Omit<UseQueryOptions<OwnProfileDetails>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: profileKeys.details(),
    queryFn: () => profileApiClient.getProfileDetails(),
    ...options,
  });
}
