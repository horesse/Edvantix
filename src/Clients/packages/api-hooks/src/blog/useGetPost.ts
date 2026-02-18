import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { PostModel } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useGetPost(
  slug: string,
  options?: Omit<UseQueryOptions<PostModel>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: blogKeys.post(slug),
    queryFn: () => blogApiClient.getPostBySlug(slug),
    enabled: Boolean(slug),
    ...options,
  });
}
