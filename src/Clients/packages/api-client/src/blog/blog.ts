import type {
  CategoryModel,
  CreateCategoryRequest,
  CreatePostRequest,
  CreateTagRequest,
  GetAdminPostsQuery,
  GetPostsQuery,
  PostModel,
  PostSummaryModel,
  PublishPostRequest,
  TagModel,
  UpdateCategoryRequest,
  UpdatePostRequest,
  UpdateTagRequest,
} from "@workspace/types/blog";
import type { PagedResult } from "@workspace/types/shared";

import { apiClient } from "../client";
import type ApiClient from "../client";

const BASE = "/blog/api/v1";

class BlogApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  // --- Posts (public) ---

  /** Returns paginated list of published posts. Anonymous access allowed. */
  public async getPosts(
    query?: GetPostsQuery,
  ): Promise<PagedResult<PostSummaryModel>> {
    const response = await this.client.get<PagedResult<PostSummaryModel>>(
      `${BASE}/posts`,
      { params: query },
    );
    return response.data;
  }

  /** Returns full post by slug. Anonymous access allowed. */
  public async getPostBySlug(slug: string): Promise<PostModel> {
    const response = await this.client.get<PostModel>(
      `${BASE}/posts/${slug}`,
    );
    return response.data;
  }

  /** Likes a post (requires auth). */
  public async likePost(postId: number): Promise<void> {
    await this.client.post<void>(`${BASE}/posts/${postId}/like`);
  }

  /** Removes like from a post (requires auth). */
  public async unlikePost(postId: number): Promise<void> {
    await this.client.delete<void>(`${BASE}/posts/${postId}/like`);
  }

  // --- Posts (admin) ---

  /** Returns full post by ID regardless of status. Requires admin role. */
  public async getAdminPost(id: number): Promise<PostModel> {
    const response = await this.client.get<PostModel>(
      `${BASE}/admin/posts/${id}`,
    );
    return response.data;
  }

  /** Returns paginated list of all posts (any status). Requires admin role. */
  public async getAdminPosts(
    query?: GetAdminPostsQuery,
  ): Promise<PagedResult<PostSummaryModel>> {
    const response = await this.client.get<PagedResult<PostSummaryModel>>(
      `${BASE}/admin/posts`,
      { params: query },
    );
    return response.data;
  }

  /** Creates a new draft post. Requires admin role. */
  public async createPost(request: CreatePostRequest): Promise<number> {
    const response = await this.client.post<number>(
      `${BASE}/admin/posts`,
      request,
    );
    return response.data;
  }

  /** Updates post content. Requires admin role. */
  public async updatePost(
    postId: number,
    request: UpdatePostRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/admin/posts/${postId}`, request);
  }

  /** Publishes or schedules a post. Requires admin role. */
  public async publishPost(
    postId: number,
    request?: PublishPostRequest,
  ): Promise<void> {
    await this.client.post<void>(
      `${BASE}/admin/posts/${postId}/publish`,
      request ?? {},
    );
  }

  /** Archives (soft-deletes) a post. Requires admin role. */
  public async deletePost(postId: number): Promise<void> {
    await this.client.delete<void>(`${BASE}/admin/posts/${postId}`);
  }

  // --- Categories ---

  /** Returns all categories. Anonymous access allowed. */
  public async getCategories(): Promise<CategoryModel[]> {
    const response = await this.client.get<CategoryModel[]>(
      `${BASE}/categories`,
    );
    return response.data;
  }

  /** Creates a new category. Requires admin role. */
  public async createCategory(
    request: CreateCategoryRequest,
  ): Promise<number> {
    const response = await this.client.post<number>(
      `${BASE}/admin/categories`,
      request,
    );
    return response.data;
  }

  /** Updates a category. Requires admin role. */
  public async updateCategory(
    id: number,
    request: UpdateCategoryRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/admin/categories/${id}`, request);
  }

  /** Deletes a category. Requires admin role. */
  public async deleteCategory(id: number): Promise<void> {
    await this.client.delete<void>(`${BASE}/admin/categories/${id}`);
  }

  // --- Tags ---

  /** Returns all tags. Anonymous access allowed. */
  public async getTags(): Promise<TagModel[]> {
    const response = await this.client.get<TagModel[]>(`${BASE}/tags`);
    return response.data;
  }

  /** Creates a new tag. Requires admin role. */
  public async createTag(request: CreateTagRequest): Promise<number> {
    const response = await this.client.post<number>(
      `${BASE}/admin/tags`,
      request,
    );
    return response.data;
  }

  /** Updates a tag. Requires admin role. */
  public async updateTag(
    id: number,
    request: UpdateTagRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/admin/tags/${id}`, request);
  }

  /** Deletes a tag. Requires admin role. */
  public async deleteTag(id: number): Promise<void> {
    await this.client.delete<void>(`${BASE}/admin/tags/${id}`);
  }
}

export default new BlogApiClient();
