import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";

import { blogKeys } from "../keys";

export default function useDeletePost() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (postId: number) => blogApiClient.deletePost(postId),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.all });
    },
  });
}
