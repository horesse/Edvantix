import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type {
  GetAdminPostsQuery,
  PostSummaryModel,
} from "@workspace/types/blog";
import type { PagedResult } from "@workspace/types/shared";

import { blogKeys } from "../keys";

export default function useGetAdminPosts(
  query?: GetAdminPostsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<PostSummaryModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: blogKeys.adminPosts(query),
    queryFn: () => blogApiClient.getAdminPosts(query),
    ...options,
  });
}
