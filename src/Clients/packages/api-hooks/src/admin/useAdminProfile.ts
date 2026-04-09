import { type UseQueryOptions, useQuery } from "@tanstack/react-query";

import adminApiClient from "@workspace/api-client/admin/admin";
import type { AdminProfileDetailDto } from "@workspace/types/admin";

import { adminKeys } from "../keys";

/** Fetches detailed profile data for admin editing. */
export default function useAdminProfile(
  profileId: string | null,
  options?: UseQueryOptions<AdminProfileDetailDto>,
) {
  return useQuery({
    queryKey: adminKeys.profile(profileId ?? ""),
    queryFn: () => adminApiClient.getProfile(profileId!),
    enabled: !!profileId,
    ...options,
  });
}
