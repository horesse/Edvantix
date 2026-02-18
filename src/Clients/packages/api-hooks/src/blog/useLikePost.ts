import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";

import { blogKeys } from "../keys";

export default function useLikePost() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ postId, liked }: { postId: number; liked: boolean }) =>
      liked ? blogApiClient.unlikePost(postId) : blogApiClient.likePost(postId),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.all });
    },
  });
}
