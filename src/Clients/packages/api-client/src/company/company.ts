import type {
  CreateOrganizationMemberRequest,
  CreateOrganizationRequest,
  OrganizationDetailDto,
  OrganizationDto,
  OrganizationMemberDto,
  OrganizationMembersQuery,
  OrganizationsQuery,
  UpdateOrganizationMemberRequest,
  UpdateOrganizationRequest,
} from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { apiClient } from "../client";
import type ApiClient from "../client";

const BASE = "/organisational/api/v1";

/** Ключ localStorage, под которым хранится ID выбранной организации. */
const SELECTED_ORG_KEY = "selectedOrgId";

/** Возвращает axios-config с заголовком X-OrganizationId-Id для multi-tenant запросов. */
function orgConfig(
  extra?: object,
): { headers: Record<string, string> } & typeof extra {
  const orgId =
    typeof window !== "undefined"
      ? (window.localStorage.getItem(SELECTED_ORG_KEY) ?? "")
      : "";
  return { headers: { "X-OrganizationId-Id": orgId }, ...extra };
}

class CompanyApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  // --- Organizations ---

  /** Получить список всех организаций с опциональным поиском/фильтрацией. */
  public async getOrganizations(
    query?: OrganizationsQuery,
  ): Promise<PagedResult<OrganizationDto>> {
    const response = await this.client.get<PagedResult<OrganizationDto>>(
      `${BASE}/organizations`,
      { params: query },
    );
    return response.data;
  }

  /** Получить детальную информацию об организации по ID. */
  public async getOrganization(id: string): Promise<OrganizationDetailDto> {
    const response = await this.client.get<OrganizationDetailDto>(
      `${BASE}/organizations/${id}`,
    );
    return response.data;
  }

  /** Создать новую организацию. Возвращает ID созданной организации. */
  public async createOrganization(
    request: CreateOrganizationRequest,
  ): Promise<string> {
    const response = await this.client.post<string>(
      `${BASE}/organizations`,
      request,
    );
    return response.data;
  }

  /**
   * Обновить базовые реквизиты организации.
   * Требует заголовок X-OrganizationId-Id (читается из localStorage).
   */
  public async updateOrganization(
    id: string,
    request: UpdateOrganizationRequest,
  ): Promise<void> {
    await this.client.patch<void>(
      `${BASE}/organizations/${id}`,
      request,
      orgConfig(),
    );
  }

  /**
   * Удалить организацию.
   * Требует заголовок X-OrganizationId-Id (читается из localStorage).
   */
  public async deleteOrganization(id: string): Promise<void> {
    await this.client.delete<void>(`${BASE}/organizations/${id}`, orgConfig());
  }

  // --- Organization Members ---

  /**
   * Получить постраничный список участников текущей организации.
   * Организация определяется заголовком X-OrganizationId-Id из localStorage.
   */
  public async getMembers(
    query?: OrganizationMembersQuery,
  ): Promise<PagedResult<OrganizationMemberDto>> {
    const response = await this.client.get<PagedResult<OrganizationMemberDto>>(
      `${BASE}/members`,
      orgConfig({ params: query }),
    );
    return response.data;
  }

  /**
   * Получить участника организации по ID.
   * Организация определяется заголовком X-OrganizationId-Id из localStorage.
   */
  public async getMember(id: string): Promise<OrganizationMemberDto> {
    const response = await this.client.get<OrganizationMemberDto>(
      `${BASE}/members/${id}`,
      orgConfig(),
    );
    return response.data;
  }

  /**
   * Добавить участника в организацию. Возвращает ID записи участника.
   * Организация определяется заголовком X-OrganizationId-Id из localStorage.
   */
  public async addMember(
    request: CreateOrganizationMemberRequest,
  ): Promise<string> {
    const response = await this.client.post<string>(
      `${BASE}/members`,
      request,
      orgConfig(),
    );
    return response.data;
  }

  /**
   * Обновить роль участника организации.
   * Организация определяется заголовком X-OrganizationId-Id из localStorage.
   */
  public async updateMember(
    id: string,
    request: UpdateOrganizationMemberRequest,
  ): Promise<void> {
    await this.client.put<void>(
      `${BASE}/members/${id}`,
      { id, ...request },
      orgConfig(),
    );
  }

  /**
   * Удалить участника из организации.
   * Организация определяется заголовком X-OrganizationId-Id из localStorage.
   */
  public async removeMember(id: string): Promise<void> {
    await this.client.delete<void>(`${BASE}/members/${id}`, orgConfig());
  }
}

export default new CompanyApiClient();
