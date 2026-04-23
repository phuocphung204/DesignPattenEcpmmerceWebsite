import { ProductType } from "@/entities/product/type";
import ProductCartWidget from "./ProductCartWidget";

type Props = {
  title?: string;
  products: ProductType[];
  viewAllHref?: string;
}

export default function FeaturedProducts({ title, products, viewAllHref }: Props) {
  const safeTitle = title || "Sản phẩm nổi bật";

  return (
    <section className="mx-auto mt-14 w-full max-w-7xl border-y px-3 py-12 backdrop-blur-sm md:px-6">
      <div>
        <div className="mb-8 flex items-center justify-between gap-3">
          <h2 className="font-rubik text-2xl font-bold text-slate-900 md:text-3xl">{safeTitle}</h2>
          <a href={viewAllHref} className="text-sm font-semibold text-emerald-700 hover:underline">
            Xem tất cả &rarr;
          </a>
        </div>

        {/* Danh sách sản phẩm */}
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
          {products.map((product) => (
            <ProductCartWidget key={product.id} product={product} />
          ))}
        </div>

      </div>
    </section>
  )
}