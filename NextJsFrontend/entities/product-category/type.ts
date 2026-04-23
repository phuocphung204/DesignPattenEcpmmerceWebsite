type ProductCategoryType = {
  id: string,
  name: string,
  slug: string,
  level: number,
  parentCategory: ProductCategoryType | null
}

export default ProductCategoryType