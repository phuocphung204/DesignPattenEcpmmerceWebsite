import { Star } from "lucide-react";
import { ProductType } from "../type";
import { formatCurrency } from "@/utils/formatter";
import Image from "next/image";
import { AspectRatio } from "@/components/ui/aspect-ratio";
import Link from "next/link";

export default function ProductCard({
  product,
  addToCartButton,
}: {
  product: ProductType
  addToCartButton?: React.JSX.Element
}) {
  // const isSlowStock = product.soldQuantity > 0 && product.soldQuantity < 10;
  // const asHref = `/san-pham/${product.id}`;
  // const desHref = "/[product-slug]";
  const productSlug = `${product.name.toLowerCase().replace(/\s+/g, '-')}`;
  const href = `/${productSlug}?id=${product.id}`;
  return (
    <>
      {/* <p>{product.id}</p>
      <p>{product.name}</p>
      <p>{product.image}</p>
      <p>{product.sellingPrice}</p>
      <p>{product.soldQuantity}</p>
      <p>{product.rating}</p>
      <p>{product.shortDescription}</p> */}
      <article
        key={product.id}
        className="flex flex-col rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition-all duration-200 hover:-translate-y-1 hover:shadow-xl"
      >
        <div className="mb-4 flex h-auto w-full items-center justify-center rounded-lg bg-linear-to-br from-slate-100 to-slate-50 cursor-pointer">
          {/* <div className="w-full px-4">
            <div className="h-3 w-1/3 animate-pulse rounded bg-slate-300" />
            <div className="mt-3 h-24 animate-pulse rounded-xl bg-slate-200" />
          </div> */}

          <AspectRatio ratio={16 / 9} className="rounded-lg">
            <Link className="absolute inset-0 rounded-2xl" href={href}>
              <Image
                unoptimized
                src={product.image}
                alt={product.name}
                fill
                className="rounded-lg w-full"
              // width={300}
              // height={300}
              />

            </Link>
          </AspectRatio>
        </div>


        <div className="flex flex-1 flex-col">
          <div className="mb-2 flex items-start justify-between">
            {/* <span
              className={`rounded px-2 py-0.5 text-[10px] font-bold ${isSlowStock ? "bg-orange-50 text-orange-600" : "bg-emerald-50 text-emerald-600"
                }`}
            >
              {product.}
            </span> */}
            <div className="flex items-center gap-1 text-xs font-bold text-amber-500">
              <Star className="h-3 w-3 fill-current" />
              {product.rating}
            </div>
          </div>

          <h3 className="mb-1 min-h-12 line-clamp-2 font-semibold text-slate-900 cursor-pointer">
            <Link href={href}>
              {product.name}
            </Link>
          </h3>

          {/* <p className="mb-2 text-[11px] font-semibold uppercase tracking-wide text-slate-500">{product.sku}</p> */}
          {/* <p className="mb-3 truncate rounded bg-slate-100 p-1.5 text-xs text-slate-600">{product.specTag}</p> */}

          <div className="mt-auto flex flex-col gap-1 pt-2">
            <div className="flex items-baseline gap-2">
              <span className="text-lg font-bold text-orange-600">{formatCurrency(product.sellingPrice)}</span>
              {/* {product.formattedOriginalPrice && (
                <span className="text-sm text-slate-400 line-through">{product.formattedOriginalPrice}</span>
              )} */}
            </div>
            {addToCartButton ? addToCartButton : (
              <button
                type="button"
                className="mt-2 rounded-lg bg-emerald-600 py-2 font-medium text-white transition-colors duration-200 hover:bg-emerald-700 focus-visible:ring-2 focus-visible:ring-emerald-500"
              >
                Thêm vào giỏ
              </button>
            )}
          </div>
        </div>
      </article>
    </>
  )
}