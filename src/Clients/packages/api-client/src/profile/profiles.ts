import { OwnProfile } from "@workspace/types/profile";

import ApiClient from "../client";

class ProfileApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = new ApiClient();
  }

  public async profile(): Promise<OwnProfile> {
    const response = await this.client.get<OwnProfile>(`/profile/api/v1/`);
    return response.data;
  }
}

export default new ProfileApiClient();
