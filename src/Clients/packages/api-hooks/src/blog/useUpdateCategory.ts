import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { UpdateCategoryRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useUpdateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      request,
    }: {
      id: number;
      request: UpdateCategoryRequest;
    }) => blogApiClient.updateCategory(id, request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.categories() });
    },
  });
}
