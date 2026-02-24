import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { PublishPostRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function usePublishPost() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      postId,
      request,
    }: {
      postId: string;
      request?: PublishPostRequest;
    }) => blogApiClient.publishPost(postId, request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.all });
    },
  });
}
