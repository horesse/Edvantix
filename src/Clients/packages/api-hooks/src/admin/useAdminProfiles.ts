import { type UseQueryOptions, useQuery } from "@tanstack/react-query";

import adminApiClient from "@workspace/api-client/admin/admin";
import type { AdminProfileDto, GetAdminProfilesRequest } from "@workspace/types/admin";
import type { PagedResult } from "@workspace/types/shared";

import { adminKeys } from "../keys";

/** Fetches a paged list of all profiles for the admin panel. */
export default function useAdminProfiles(
  query?: GetAdminProfilesRequest,
  options?: Omit<UseQueryOptions<PagedResult<AdminProfileDto>>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: adminKeys.profiles(query),
    queryFn: () => adminApiClient.getProfiles(query ?? {}),
    ...options,
  });
}
