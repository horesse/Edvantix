import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { UpdatePostRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useUpdatePost() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      postId,
      request,
    }: {
      postId: number;
      request: UpdatePostRequest;
    }) => blogApiClient.updatePost(postId, request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.all });
    },
  });
}
