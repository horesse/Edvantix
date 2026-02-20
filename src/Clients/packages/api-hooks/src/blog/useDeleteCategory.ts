import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";

import { blogKeys } from "../keys";

export default function useDeleteCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => blogApiClient.deleteCategory(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.categories() });
    },
  });
}
