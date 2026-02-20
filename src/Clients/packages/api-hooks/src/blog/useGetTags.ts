import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { TagModel } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useGetTags(
  options?: Omit<UseQueryOptions<TagModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: blogKeys.tags(),
    queryFn: () => blogApiClient.getTags(),
    staleTime: 5 * 60 * 1000,
    ...options,
  });
}
