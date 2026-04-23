import { buildPath } from "@/utils/build-path-query-param"
import http from "../http";
import { ProductType } from "@/entities/product/type";
import { Id } from "@/types/common-type";
import { get } from "http";

export type PageResult<T> = {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export type SortDirection = 0 | 1

export enum SortBy {
  Price = 0,
  Rating = 1,
  SoldQuantity = 2,
  CreatedAt = 3
  // mở rộng theo backend nếu cần
}

export type ProductQueryParams = {
  name?: string | null
  brandId?: string | null
  categoryId?: string | null
  variantGroupId?: string | null
  sku?: string | null
  minRating?: number | null
  maxRating?: number | null
  minPrice?: number | null
  maxPrice?: number | null
  sortBy?: SortBy | number
  sortDirection?: SortDirection
  pageIndex?: number
  pageSize?: number
}

/**
 * Chuyển từ object query string (tất cả là string) sang ProductQueryParams với kiểu phù hợp.
 */
export function parseProductQueryParams(raw: Record<string, string | undefined>): ProductQueryParams {
  const parseNumber = (v?: string) => {
    if (v == null || v === "") return undefined
    const n = Number(v)
    return Number.isNaN(n) ? undefined : n
  }

  const nullOrString = (v?: string) => (v == null || v === "null" || v === "" ? undefined : v)
  
  return {
    brandId: nullOrString(raw.brandId) ?? null,
    categoryId: nullOrString(raw.categoryId) ?? null,
    variantGroupId: nullOrString(raw.variantGroupId) ?? null,
    sku: nullOrString(raw.sku) ?? null,
    minRating: parseNumber(raw.minRating) ?? null,
    maxRating: parseNumber(raw.maxRating) ?? null,
    minPrice: parseNumber(raw.minPrice) ?? null,
    maxPrice: parseNumber(raw.maxPrice) ?? null,
    sortBy: parseNumber(raw.sortBy) ?? undefined,
    sortDirection: parseNumber(raw.sortDirection) as SortDirection | undefined,
    pageIndex: parseNumber(raw.pageIndex) ?? undefined,
    pageSize: parseNumber(raw.pageSize) ?? undefined,
  }
}

export type AttributeType = 0 | 1

export type ProductAttribute = {
  name: string
  value: string
  type: AttributeType
}

export type Seo = {
  title: string
  description: string
  slug: string
}

export type ProductDetailType = {
  id: Id
  name: string
  brandId: Id
  brandName: string
  categoryId: Id
  categoryName: string
  sku: string
  variantGroupId?: string
  images: string[]
  attributes: ProductAttribute[]
  sellingPrice: number
  purchasePrice: number
  stockQuantity: number
  soldQuantity: number
  status: number
  shortDescription?: string | null
  detailDescription?: string | null
  rating: number
  seo?: Seo | null
}

export type ProductVariantType = {
  id: Id
  name: string
  variantGroupId?: Id | null
  image: string
  sellingPrice: number
  soldQuantity: number
  rating: number
  shortDescription?: string | null
  seo?: Seo | null
}

export const productRequest = {
  getSearchedProducts: async (query: ProductQueryParams) => { 
    "use cache"

    return http.get<PageResult<ProductType>>(
      buildPath("/products", query)
    )
  },

  getProductDetail: async (id: Id) => {
    "use cache"
    return http.get<ProductDetailType>(`/products/${id}`)
  },

  getProductVariants: async (variantGroupId: string) => {
    "use cache"
    return http.get<ProductVariantType[]>(`/products/by-group?variantGroupId=${variantGroupId}`)
  }
}