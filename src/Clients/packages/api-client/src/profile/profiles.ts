import type {
  OwnProfile,
  OwnProfileDetails,
  RegisterProfileRequest,
  SkillSearchResult,
  UpdateProfileRequest,
} from "@workspace/types/profile";

import { apiClient } from "../client";
import type ApiClient from "../client";

class ProfileApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  public async getProfile(): Promise<OwnProfile> {
    const response = await this.client.get<OwnProfile>(
      `/persona/api/v1/profile`,
    );
    return response.data;
  }

  public async getProfileDetails(): Promise<OwnProfileDetails> {
    const response = await this.client.get<OwnProfileDetails>(
      `/persona/api/v1/profile/details`,
    );
    return response.data;
  }

  public async uploadAvatar(avatar: File): Promise<OwnProfileDetails> {
    const formData = new FormData();
    formData.append("avatar", avatar);

    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile/avatar`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } },
    );
    return response.data;
  }

  /** Единый метод обновления профиля: личные данные, контакты, опыт, образование, навыки, bio. */
  public async updateProfile(
    request: UpdateProfileRequest,
  ): Promise<OwnProfileDetails> {
    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile`,
      request,
    );
    return response.data;
  }

  public async registerProfile(
    request: RegisterProfileRequest,
  ): Promise<string> {
    const formData = new FormData();
    formData.append("firstName", request.firstName);
    formData.append("lastName", request.lastName);
    formData.append("birthDate", request.birthDate);
    formData.append("gender", String(request.gender));

    if (request.middleName) {
      formData.append("middleName", request.middleName);
    }

    if (request.avatar) {
      formData.append("avatar", request.avatar);
    }

    const response = await this.client.post<string>(
      `/persona/api/v1/profile/registration`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } },
    );
    return response.data;
  }

  /**
   * Поиск навыков для автодополнения.
   * @param query Подстрока для поиска (минимум 1 символ).
   * @param limit Максимальное количество результатов (по умолчанию 20, макс. 50).
   */
  public async searchSkills(
    query: string,
    limit = 20,
  ): Promise<SkillSearchResult[]> {
    const response = await this.client.get<SkillSearchResult[]>(
      `/persona/api/v1/skills`,
      { params: { query, limit } },
    );
    return response.data;
  }
}

export default new ProfileApiClient();
