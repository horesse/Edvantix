import { type UseQueryOptions, useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { LegalFormModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useLegalForms(
  options?: Omit<UseQueryOptions<LegalFormModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.legalForms(),
    queryFn: () => companyApiClient.getLegalForms(),
    staleTime: Infinity, // Справочные данные не изменяются в рантайме.
    ...options,
  });
}
