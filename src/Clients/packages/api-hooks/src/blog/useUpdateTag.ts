import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { UpdateTagRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useUpdateTag() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateTagRequest }) =>
      blogApiClient.updateTag(id, request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.tags() });
    },
  });
}
