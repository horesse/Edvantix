import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { CreateTagRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useCreateTag() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateTagRequest) =>
      blogApiClient.createTag(request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.tags() });
    },
  });
}
