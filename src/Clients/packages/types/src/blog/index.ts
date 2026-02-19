/** Post publication status */
export enum PostStatus {
  Draft = 0,
  Scheduled = 1,
  Published = 2,
  Archived = 3,
}

/** Post content type */
export enum PostType {
  News = 0,
  Changelog = 1,
}

export type AuthorModel = {
  id: string;
  fullName: string;
};

export type CategoryModel = {
  id: number;
  name: string;
  slug: string;
  description?: string;
  createdAt: string;
};

export type TagModel = {
  id: number;
  name: string;
  slug: string;
  createdAt?: string;
};

/** Summary model used in post listings */
export type PostSummaryModel = {
  id: number;
  title: string;
  slug: string;
  summary?: string;
  status: PostStatus;
  type: PostType;
  isPremium: boolean;
  coverImageUrl?: string;
  likesCount: number;
  publishedAt?: string;
  author?: AuthorModel;
  categories: CategoryModel[];
  tags: TagModel[];
};

/** Full post model including Markdown content */
export type PostModel = PostSummaryModel & {
  content: string;
  status: PostStatus;
  /** Whether the currently authenticated user has liked this post. */
  isLikedByMe: boolean;
  scheduledAt?: string;
  createdAt: string;
  updatedAt: string;
};

export type CreatePostRequest = {
  title: string;
  slug: string;
  content: string;
  summary?: string;
  type: PostType;
  isPremium: boolean;
  coverImageUrl?: string;
  categoryIds: number[];
  tagIds: number[];
};

export type UpdatePostRequest = CreatePostRequest;

export type PublishPostRequest = {
  scheduledAt?: string;
};

export type CreateCategoryRequest = {
  name: string;
  slug: string;
  description?: string;
};

export type UpdateCategoryRequest = CreateCategoryRequest;

export type CreateTagRequest = {
  name: string;
  slug: string;
};

export type UpdateTagRequest = CreateTagRequest;

export type GetPostsQuery = {
  type?: PostType;
  categoryId?: number;
  tagId?: number;
  search?: string;
  pageIndex?: number;
  pageSize?: number;
};

export type GetAdminPostsQuery = {
  status?: PostStatus;
  type?: PostType;
  categoryId?: number;
  tagId?: number;
  search?: string;
  pageIndex?: number;
  pageSize?: number;
};
