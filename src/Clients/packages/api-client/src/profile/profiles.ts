import type {
  OwnProfile,
  OwnProfileDetails,
  RegisterProfileRequest,
  UpdateBioRequest,
  UpdateContactsRequest,
  UpdateEducationRequest,
  UpdateEmploymentRequest,
  UpdatePersonalInfoRequest,
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

  public async updatePersonalInfo(
    request: UpdatePersonalInfoRequest,
  ): Promise<OwnProfileDetails> {
    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile/personal-info`,
      request,
    );
    return response.data;
  }

  public async updateContacts(
    request: UpdateContactsRequest,
  ): Promise<OwnProfileDetails> {
    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile/contacts`,
      request,
    );
    return response.data;
  }

  public async updateEducation(
    request: UpdateEducationRequest,
  ): Promise<OwnProfileDetails> {
    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile/education`,
      request,
    );
    return response.data;
  }

  public async updateEmployment(
    request: UpdateEmploymentRequest,
  ): Promise<OwnProfileDetails> {
    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile/employment`,
      request,
    );
    return response.data;
  }

  public async updateBio(
    request: UpdateBioRequest,
  ): Promise<OwnProfileDetails> {
    const response = await this.client.patch<OwnProfileDetails>(
      `/persona/api/v1/profile/bio`,
      request,
    );
    return response.data;
  }

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
}

export default new ProfileApiClient();
