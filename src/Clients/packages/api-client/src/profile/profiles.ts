import type {
  OwnProfile,
  OwnProfileDetails,
  RegisterProfileRequest,
  UpdateContactRequest,
  UpdateEducationRequest,
  UpdateEmploymentHistoryRequest,
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
      `/profile/api/v1/profile`,
    );
    return response.data;
  }

  public async getProfileDetails(): Promise<OwnProfileDetails> {
    const response = await this.client.get<OwnProfileDetails>(
      `/profile/api/v1/profile/details`,
    );
    return response.data;
  }

  public async updateProfile(request: UpdateProfileRequest): Promise<void> {
    await this.client.put<void>(`/profile/api/v1/profile`, request);
  }

  public async updateContacts(contacts: UpdateContactRequest[]): Promise<void> {
    await this.client.put<void>(`/profile/api/v1/profile/contacts`, contacts);
  }

  public async updateEmploymentHistories(
    employmentHistories: UpdateEmploymentHistoryRequest[],
  ): Promise<void> {
    await this.client.put<void>(
      `/profile/api/v1/profile/employment-histories`,
      employmentHistories,
    );
  }

  public async updateEducations(
    educations: UpdateEducationRequest[],
  ): Promise<void> {
    await this.client.put<void>(
      `/profile/api/v1/profile/educations`,
      educations,
    );
  }

  public async uploadAvatar(file: File): Promise<void> {
    const formData = new FormData();
    formData.append("image", file);
    await this.client.post<void>(`/profile/api/v1/profile/avatar`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  }

  public async registerProfile(
    request: RegisterProfileRequest,
  ): Promise<number> {
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

    const response = await this.client.post<number>(
      `/profile/api/v1/profile/registration`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } },
    );
    return response.data;
  }
}

export default new ProfileApiClient();
