import { useMutation, useQueryClient } from "@tanstack/react-query";

import blogApiClient from "@workspace/api-client/blog/blog";

import { blogKeys } from "../keys";

export default function useDeleteTag() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => blogApiClient.deleteTag(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: blogKeys.tags() });
    },
  });
}
