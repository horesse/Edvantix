import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { GetPostsQuery, PostSummaryModel } from "@workspace/types/blog";
import type { PagedResult } from "@workspace/types/shared";

import { blogKeys } from "../keys";

export default function useGetPosts(
  query?: GetPostsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<PostSummaryModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: blogKeys.posts(query),
    queryFn: () => blogApiClient.getPosts(query),
    ...options,
  });
}
