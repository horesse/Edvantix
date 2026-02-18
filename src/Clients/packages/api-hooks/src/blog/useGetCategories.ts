import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { CategoryModel } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useGetCategories(
  options?: Omit<UseQueryOptions<CategoryModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: blogKeys.categories(),
    queryFn: () => blogApiClient.getCategories(),
    staleTime: 5 * 60 * 1000,
    ...options,
  });
}
