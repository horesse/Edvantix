import { CategoryPostsSection } from "@/features/blog/category-posts";

type Props = { params: Promise<{ slug: string }> };

export default function CategoryPage({ params }: Props) {
  return <CategoryPostsSection params={params} />;
}
