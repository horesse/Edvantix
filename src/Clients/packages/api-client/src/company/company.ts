import type { AddGroupMemberRequest, AddMemberRequest, AddOrganizationContactRequest, CreateGroupRequest, CreateInvitationRequest, CreateOrganizationRequest, GroupMemberModel, GroupModel, GroupSummaryModel, InvitationModel, OrganizationContactModel, OrganizationMemberModel, OrganizationModel, OrganizationSummaryModel, UpdateGroupMemberRoleRequest, UpdateGroupRequest, UpdateMemberRoleRequest, UpdateOrganizationContactRequest, UpdateOrganizationRequest } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";



import { apiClient } from "../client";
import type ApiClient from "../client";











// --- Query Types ---

export type OrganizationMembersQuery = {
  organizationId: string;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

export type OrganizationGroupsQuery = {
  organizationId: string;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

export type GroupMembersQuery = {
  groupId: string;
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
  organizationId: string;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
};

const BASE = "/organizational/api/v1";

class CompanyApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  // --- Organizations ---

  public async createOrganization(
    request: CreateOrganizationRequest,
  ): Promise<string> {
    const response = await this.client.post<string>(
      `${BASE}/organizations`,
      request,
    );
    return response.data;
  }

  public async getOrganization(id: string): Promise<OrganizationModel> {
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
    id: string,
    request: UpdateOrganizationRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/organizations/${id}`, request);
  }

  // --- Members ---

  public async addMember(
    orgId: string,
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

  public async removeMember(orgId: string, memberId: string): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/organizations/${orgId}/members/${memberId}`,
    );
  }

  public async updateMemberRole(
    orgId: string,
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
    orgId: string,
    request: CreateInvitationRequest,
  ): Promise<void> {
    await this.client.post<void>(
      `${BASE}/organizations/${orgId}/invitations`,
      request,
    );
  }

  public async getPendingInvitations(
    orgId: string,
  ): Promise<InvitationModel[]> {
    const response = await this.client.get<InvitationModel[]>(
      `${BASE}/organizations/${orgId}/invitations`,
    );
    return response.data;
  }

  public async cancelInvitation(
    orgId: string,
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
    orgId: string,
    request: CreateGroupRequest,
  ): Promise<string> {
    const response = await this.client.post<string>(
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

  public async getGroup(id: string): Promise<GroupModel> {
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
    id: string,
    request: UpdateGroupRequest,
  ): Promise<void> {
    await this.client.put<void>(`${BASE}/groups/${id}`, request);
  }

  public async deleteGroup(id: string): Promise<void> {
    await this.client.delete<void>(`${BASE}/groups/${id}`);
  }

  // --- Group Members ---

  public async addGroupMember(
    groupId: string,
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
    groupId: string,
    memberId: string,
  ): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/groups/${groupId}/members/${memberId}`,
    );
  }

  public async updateGroupMemberRole(
    groupId: string,
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
    orgId: string,
    request: AddOrganizationContactRequest,
  ): Promise<string> {
    const response = await this.client.post<string>(
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
    orgId: string,
    contactId: string,
    request: UpdateOrganizationContactRequest,
  ): Promise<void> {
    await this.client.put<void>(
      `${BASE}/organizations/${orgId}/contacts/${contactId}`,
      request,
    );
  }

  public async deleteContact(orgId: string, contactId: string): Promise<void> {
    await this.client.delete<void>(
      `${BASE}/organizations/${orgId}/contacts/${contactId}`,
    );
  }
}

export default new CompanyApiClient();
