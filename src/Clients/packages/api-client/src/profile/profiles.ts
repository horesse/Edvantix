import type { OwnProfile, RegisterProfileRequest } from "@workspace/types/profile";

import ApiClient from "../client";

class ProfileApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = new ApiClient();
  }

  public async getProfile(): Promise<OwnProfile> {
    const response = await this.client.get<OwnProfile>(`/profile/api/v1/profile`);
    return response.data;
  }

  public async registerProfile(request: RegisterProfileRequest): Promise<number> {
    const response = await this.client.post<number>(`/profile/api/person/registration`, request);
    return response.data;
  }
}

export default new ProfileApiClient();
