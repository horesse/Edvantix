import type {
  OwnProfile,
  RegisterProfileRequest,
} from "@workspace/types/profile";

import ApiClient from "../client";

class ProfileApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = new ApiClient();
  }

  public async getProfile(): Promise<OwnProfile> {
    const response = await this.client.get<OwnProfile>(
      `/profile/api/v1/profile`,
    );
    return response.data;
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
