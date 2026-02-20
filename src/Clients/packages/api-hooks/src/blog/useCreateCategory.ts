import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";
import type { CreateCategoryRequest } from "@workspace/types/blog";

import { blogKeys } from "../keys";

export default function useCreateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateCategoryRequest) =>
      blogApiClient.createCategory(request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.categories() });
    },
  });
}
