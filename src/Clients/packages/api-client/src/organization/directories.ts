import type {
  DirectoryListQuery,
  ReorderDirectoryRequest,
} from "@workspace/types/organization";
import type { PagedResult } from "@workspace/types/shared";

import { apiClient } from "../client";
import type ApiClient from "../client";
import { parsePagedResponse } from "../paged-response";

const BASE = "/organisational/api/v1";

const SELECTED_ORG_KEY = "selectedOrgId";

function orgConfig(
  extra?: object,
): { headers: Record<string, string> } & typeof extra {
  const orgId =
    typeof window !== "undefined"
      ? (window.localStorage.getItem(SELECTED_ORG_KEY) ?? "")
      : "";
  return { headers: { "X-Organization-Id": orgId }, ...extra };
}

/** Generic-клиент для CRUD-операций над любым справочником организации. */
class DirectoriesApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  /** Получить постраничный список элементов справочника. */
  public async list<T>(
    code: string,
    query?: DirectoryListQuery,
  ): Promise<PagedResult<T>> {
    const response = await this.client.get<T[]>(
      `${BASE}/directories/${code}`,
      orgConfig({ params: query }),
    );
    return parsePagedResponse(response);
  }

  /** Получить элемент справочника по ID. */
  public async getById<T>(code: string, id: string): Promise<T> {
    const response = await this.client.get<T>(
      `${BASE}/directories/${code}/${id}`,
      orgConfig(),
    );
    return response.data;
  }

  /** Создать элемент справочника. Возвращает ID созданной записи. */
  public async create<TReq>(
    code: string,
    body: TReq,
  ): Promise<string> {
    const response = await this.client.post<string>(
      `${BASE}/directories/${code}`,
      body,
      orgConfig(),
    );
    return response.data;
  }

  /** Обновить элемент справочника. */
  public async update<TReq>(
    code: string,
    id: string,
    body: TReq,
  ): Promise<void> {
    await this.client.put<void>(
      `${BASE}/directories/${code}/${id}`,
      { id, ...body },
      orgConfig(),
    );
  }

  /** Перевести элемент справочника в архив. */
  public async archive(code: string, id: string): Promise<void> {
    await this.client.post<void>(
      `${BASE}/directories/${code}/${id}/archive`,
      {},
      orgConfig(),
    );
  }

  /** Восстановить элемент справочника из архива. */
  public async restore(code: string, id: string): Promise<void> {
    await this.client.post<void>(
      `${BASE}/directories/${code}/${id}/restore`,
      {},
      orgConfig(),
    );
  }

  /** Переупорядочить элементы справочника. */
  public async reorder(
    code: string,
    body: ReorderDirectoryRequest,
  ): Promise<void> {
    await this.client.patch<void>(
      `${BASE}/directories/${code}/reorder`,
      body,
      orgConfig(),
    );
  }
}

export default new DirectoriesApiClient();
