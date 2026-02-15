import type {
  AddGroupMemberRequest,
  AddMemberRequest,
  AddOrganizationContactRequest,
  CreateGroupRequest,
  CreateInvitationRequest,
  CreateOrganizationRequest,
  GroupMemberModel,
  GroupModel,
  GroupSummaryModel,
  InvitationModel,
  OrganizationContactModel,
  OrganizationMemberModel,
  OrganizationModel,
  OrganizationSummaryModel,
  UpdateGroupMemberRoleRequest,
  UpdateGroupRequest,
  UpdateMemberRoleRequest,
  UpdateOrganizationContactRequest,
  UpdateOrganizationRequest,
} from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { apiClient } from "../client";
import type ApiClient from "../client";

// --- Query Types ---

export type OrganizationMembersQuery = {
  organizationId: number;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

export type OrganizationGroupsQuery = {
  organizationId: number;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

export type GroupMembersQuery = {
  groupId: number;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

export type MyGroupsQuery = {
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

export type OrganizationContactsQuery = {
  organizationId: number;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

const BASE = "/company/api/v1";

class CompanyApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  // --- Organizations ---

  public async createOrganization(
    request: CreateOrganizationRequest,
  ): Promise<number> {
    const response = await this.client.post<number>(
      `${BASE}/organizations`,
      request,
    );
    return response.data;
  }

  public async getOrganization(id: number): Promise<OrganizationModel> {
    const response = await this.client.get<OrganizationModel>(
      `${BASE}/organizations/${id}`,
    );
    return response.data;
  }

  public async getMyOrganizations(): Promise<OrganizationSummaryModel[]> {
    const response = await this.client.get<OrganizationSummaryModel[]>(
      `${BASE}/organizations/my`,
    );
    return response.data;
  }

  public async updateOrganization(
    id: number,
    request: UpdateOrganizationRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/organizations/${id}`, request);
  }

  // --- Members ---

  public async addMember(
    orgId: number,
    request: AddMemberRequest,
  ): Promise<void> {
    await this.client.post<void>(
      `${BASE}/organizations/${orgId}/members`,
      request,
    );
  }

  public async getMembers(
    query: OrganizationMembersQuery,
  ): Promise<PagedResult<OrganizationMemberModel>> {
    const response = await this.client.get<
      PagedResult<OrganizationMemberModel>
    >(`${BASE}/organizations/members`, {
      params: query,
    });
    return response.data;
  }

  public async removeMember(orgId: number, memberId: string): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/organizations/${orgId}/members/${memberId}`,
    );
  }

  public async updateMemberRole(
    orgId: number,
    memberId: string,
    request: UpdateMemberRoleRequest,
  ): Promise<void> {
    await this.client.put<void>(
      `${BASE}/organizations/${orgId}/members/${memberId}/role`,
      request,
    );
  }

  // --- Invitations ---

  public async createInvitation(
    orgId: number,
    request: CreateInvitationRequest,
  ): Promise<void> {
    await this.client.post<void>(
      `${BASE}/organizations/${orgId}/invitations`,
      request,
    );
  }

  public async getPendingInvitations(
    orgId: number,
  ): Promise<InvitationModel[]> {
    const response = await this.client.get<InvitationModel[]>(
      `${BASE}/organizations/${orgId}/invitations`,
    );
    return response.data;
  }

  public async cancelInvitation(
    orgId: number,
    invitationId: string,
  ): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/organizations/${orgId}/invitations/${invitationId}`,
    );
  }

  public async getMyInvitations(): Promise<InvitationModel[]> {
    const response = await this.client.get<InvitationModel[]>(
      `${BASE}/invitations/my`,
    );
    return response.data;
  }

  public async acceptInvitation(token: string): Promise<void> {
    await this.client.post<void>(`${BASE}/invitations/${token}/accept`);
  }

  public async declineInvitation(token: string): Promise<void> {
    await this.client.post<void>(`${BASE}/invitations/${token}/decline`);
  }

  // --- Groups ---

  public async createGroup(
    orgId: number,
    request: CreateGroupRequest,
  ): Promise<number> {
    const response = await this.client.post<number>(
      `${BASE}/organizations/${orgId}/groups`,
      request,
    );
    return response.data;
  }

  public async getOrganizationGroups(
    query: OrganizationGroupsQuery,
  ): Promise<PagedResult<GroupModel>> {
    const response = await this.client.get<PagedResult<GroupModel>>(
      `${BASE}/organizations/groups`,
      {
        params: query,
      },
    );
    return response.data;
  }

  public async getGroup(id: number): Promise<GroupModel> {
    const response = await this.client.get<GroupModel>(`${BASE}/groups/${id}`);
    return response.data;
  }

  public async getMyGroups(
    query?: MyGroupsQuery,
  ): Promise<PagedResult<GroupSummaryModel>> {
    const response = await this.client.get<PagedResult<GroupSummaryModel>>(
      `${BASE}/groups/my`,
      {
        params: query,
      },
    );
    return response.data;
  }

  public async updateGroup(
    id: number,
    request: UpdateGroupRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/groups/${id}`, request);
  }

  public async deleteGroup(id: number): Promise<void> {
    await this.client.delete<void>(`${BASE}/groups/${id}`);
  }

  // --- Group Members ---

  public async addGroupMember(
    groupId: number,
    request: AddGroupMemberRequest,
  ): Promise<void> {
    await this.client.post<void>(`${BASE}/groups/${groupId}/members`, request);
  }

  public async getGroupMembers(
    query: GroupMembersQuery,
  ): Promise<PagedResult<GroupMemberModel>> {
    const response = await this.client.get<PagedResult<GroupMemberModel>>(
      `${BASE}/groups/members`,
      {
        params: query,
      },
    );
    return response.data;
  }

  public async removeGroupMember(
    groupId: number,
    memberId: string,
  ): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/groups/${groupId}/members/${memberId}`,
    );
  }

  public async updateGroupMemberRole(
    groupId: number,
    memberId: string,
    request: UpdateGroupMemberRoleRequest,
  ): Promise<void> {
    await this.client.put<void>(
      `${BASE}/groups/${groupId}/members/${memberId}/role`,
      request,
    );
  }

  // --- Contacts ---

  public async addContact(
    orgId: number,
    request: AddOrganizationContactRequest,
  ): Promise<number> {
    const response = await this.client.post<number>(
      `${BASE}/organizations/${orgId}/contacts`,
      request,
    );
    return response.data;
  }

  public async getContacts(
    query: OrganizationContactsQuery,
  ): Promise<PagedResult<OrganizationContactModel>> {
    const response = await this.client.get<
      PagedResult<OrganizationContactModel>
    >(`${BASE}/organizations/contacts`, {
      params: query,
    });
    return response.data;
  }

  public async updateContact(
    orgId: number,
    contactId: number,
    request: UpdateOrganizationContactRequest,
  ): Promise<void> {
    await this.client.put<void>(
      `${BASE}/organizations/${orgId}/contacts/${contactId}`,
      request,
    );
  }

  public async deleteContact(orgId: number, contactId: number): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/organizations/${orgId}/contacts/${contactId}`,
    );
  }
}

export default new CompanyApiClient();
