import useGetAdminPosts from "@workspace/api-hooks/blog/useGetAdminPosts";
import { PostStatus } from "@workspace/types/blog";

export function useStatusCount(status: PostStatus) {
  const { data } = useGetAdminPosts({ status, pageSize: 1 });
  return data?.totalCount ?? null;
}
