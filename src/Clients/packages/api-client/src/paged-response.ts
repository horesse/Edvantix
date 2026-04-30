import type { AxiosResponse } from "axios";

import type { PagedResult } from "@workspace/types/shared";

/**
 * Конструирует `PagedResult<T>` из ответа axios, читая метаданные пагинации
 * из HTTP-заголовков, которые добавляет `PaginationHeaderFilter` на бэкенде:
 *
 * - Тело ответа — массив `T[]` (бэкенд `PagedResult<T> : List<T>` сериализуется как массив).
 * - `Pagination-Count` — общее число элементов во всей выборке.
 * - `Link` — RFC 5988 ссылки (first, prev, self, next, last).
 */
export function parsePagedResponse<T>(
  response: AxiosResponse<T[]>,
): PagedResult<T> {
  return {
    items: response.data,
    totalCount: parseInt(
      (response.headers["pagination-count"] as string | undefined) ?? "0",
      10,
    ),
    link: response.headers["link"] as string | undefined,
  };
}
