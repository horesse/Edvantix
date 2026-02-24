import { useQuery } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";

import { blogKeys } from "../keys";

export default function useGetAdminPost(id: string) {
  return useQuery({
    queryKey: blogKeys.adminPost(id),
    queryFn: () => blogApiClient.getAdminPost(id),
    enabled: Boolean(id),
  });
}
