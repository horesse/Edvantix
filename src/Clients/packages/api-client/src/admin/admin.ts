import type {
  AdminProfileDetailDto,
  AdminProfileDto,
  AdminUpdateProfileRequest,
  GetAdminProfilesRequest,
  SendAdminNotificationRequest,
} from "@workspace/types/admin";
import type { PagedResult } from "@workspace/types/shared";

import { apiClient } from "../client";

const BASE = "/persona/api/v1";

class AdminApiClient {
  /** Retrieves a paged list of all profiles for admin management. */
  public async getProfiles(
    query: GetAdminProfilesRequest,
  ): Promise<PagedResult<AdminProfileDto>> {
    const response = await apiClient.get<AdminProfileDto[]>(
      `${BASE}/admin/profiles`,
      { params: query },
    );

    const totalCount = Number(
      response.headers?.["pagination-count"] ?? response.data.length,
    );

    return {
      items: response.data,
      totalCount,
    };
  }

  /** Blocks a profile, preventing the user from authenticating. */
  public async blockProfile(profileId: string): Promise<void> {
    await apiClient.post<void>(`${BASE}/admin/profiles/${profileId}/block`);
  }

  /** Unblocks a previously blocked profile. */
  public async unblockProfile(profileId: string): Promise<void> {
    await apiClient.post<void>(`${BASE}/admin/profiles/${profileId}/unblock`);
  }

  /** Sends a direct notification to a specific profile. */
  public async sendNotification(
    profileId: string,
    request: SendAdminNotificationRequest,
  ): Promise<void> {
    await apiClient.post<void>(
      `${BASE}/admin/profiles/${profileId}/notify`,
      request,
    );
  }

  /** Retrieves detailed profile data for admin editing. */
  public async getProfile(profileId: string): Promise<AdminProfileDetailDto> {
    const response = await apiClient.get<AdminProfileDetailDto>(
      `${BASE}/admin/profiles/${profileId}`,
    );
    return response.data;
  }

  /** Updates a profile with admin-specified changes and reason. */
  public async updateProfile(
    profileId: string,
    request: AdminUpdateProfileRequest,
  ): Promise<void> {
    await apiClient.patch<void>(`${BASE}/admin/profiles/${profileId}`, request);
  }

  /** Records the current admin's last login timestamp. */
  public async recordLastLogin(): Promise<void> {
    await apiClient.post<void>(`${BASE}/me/session`);
  }
}

export default new AdminApiClient();
