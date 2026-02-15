export type PagedResult<T> = {
  items: T[];
  pageIndex: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};
