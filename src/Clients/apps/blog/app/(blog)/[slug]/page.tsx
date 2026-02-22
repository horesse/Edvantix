import { PostDetailsSection } from "@/features/blog/post-details";

type PostPageParams = { params: Promise<{ slug: string }> };

export default function PostPage({ params }: PostPageParams) {
  return <PostDetailsSection params={params} />;
}
