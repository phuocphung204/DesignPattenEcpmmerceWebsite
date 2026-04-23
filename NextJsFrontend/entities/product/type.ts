import { Id } from "@/types/common-type"

export type ProductType = {
  id: Id
  name: string
  image: string
  sellingPrice: number
  soldQuantity: number
  rating: number
  shortDescription: string
}