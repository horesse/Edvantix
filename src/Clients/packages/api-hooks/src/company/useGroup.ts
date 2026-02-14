import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { GroupModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useGroup(
  id: number,
  options?: Omit<UseQueryOptions<GroupModel>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.group(id),
    queryFn: () => companyApiClient.getGroup(id),
    enabled: id > 0,
    ...options,
  });
}
