import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { CreatePostRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useCreatePost() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreatePostRequest) =>
      blogApiClient.createPost(request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.posts() });
    },
  });
}
