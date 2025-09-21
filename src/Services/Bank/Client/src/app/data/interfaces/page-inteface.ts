export interface PageResult<T>{
    items: T[];
    totalCount: number;
    pageCount: number;
    pageNumber: number;
    pageSize: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}