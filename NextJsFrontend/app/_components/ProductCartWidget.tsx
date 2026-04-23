import { ProductType } from "@/entities/product/type"
import ProductCard from "@/entities/product/ui/ProductCard"
import AddToCardButton from "@/features/cart/ui/AddToCardButton"

type Props = {
  product: ProductType
}
export default function ProductCartWidget({ product }: Props) {

  return (
    <ProductCard product={product} addToCartButton={
      <AddToCardButton productId={product.id} />
    } />
  )
}