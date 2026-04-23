import Image from "next/image";
import Link from "next/link";
import { Suspense } from "react";
import { productRequest } from "@/services/backend-requests/product-request";
import { Id } from "@/types/common-type";
import ImagesCarousel from "./_components/ImagesCarousel";
import { formatCurrency } from "@/utils/formatter";
import ProductVariant from "@/entities/product/ui/ProductVariant";
import AddToCardButton from "@/features/cart/ui/AddToCardButton";
import BuyNowButton from "./_components/BuyNowButton";

type ProductDetailPageProps = {
  params: Promise<{ "product-slug": string[] }>;
  searchParams: Promise<{ id?: string }>;
};

export default function ProductDetailPage({ params, searchParams }: ProductDetailPageProps) {
  return (
    <div className="min-h-screen bg-[#F8FAFC]">
      <div className="relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,#d1fae5_0%,transparent_55%),radial-gradient(circle_at_bottom,#ffedd5_0%,transparent_55%)]" />
        <div className="relative mx-auto w-full max-w-6xl px-4 pb-16 pt-10 sm:px-6 lg:px-8">
          <Suspense fallback={<ProductDetailSkeleton />}>
            <ProductDetailContent params={params} searchParams={searchParams} />
          </Suspense>
        </div>
      </div>
    </div>
  );
}

async function ProductDetailContent({ params, searchParams }: ProductDetailPageProps) {
  const { "product-slug": productSlug } = await params;
  const { id } = await searchParams;
  const productDetail = await productRequest.getProductDetail(id as Id)
    .then((res) => res.payload?.data);

  const otherProducts = await productRequest.getProductVariants(productDetail?.variantGroupId || "")
    .then((res) => res.payload?.data?.filter((prod) => prod.id !== productDetail?.id) || []);


  if (!productDetail) {
    return (
      <div className="rounded-3xl border border-emerald-100 bg-white/80 p-10 text-center shadow-[0_30px_60px_-45px_rgba(15,23,42,0.45)] backdrop-blur">
        <p className="text-sm uppercase tracking-[0.3em] text-emerald-600">Product unavailable</p>
        <h1 className="mt-4 text-3xl font-semibold text-slate-900">We could not find this product.</h1>
        <p className="mt-3 text-slate-600">Try another item or return to the catalog.</p>
        <Link
          href="/"
          className="mt-8 inline-flex cursor-pointer items-center justify-center rounded-full bg-emerald-600 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-emerald-200 transition hover:bg-emerald-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
        >
          Back to catalog
        </Link>
      </div>
    );
  }

  const priceText = formatCurrency(productDetail.sellingPrice);

  return (
    <div className="space-y-12">

      <div className="flex flex-wrap items-center justify-between gap-3 text-sm text-slate-600">
        <div className="flex items-center gap-2">
          <Link href="/" className="cursor-pointer font-semibold text-emerald-600 hover:text-emerald-700">
            Home
          </Link>
          <span className="text-slate-400">/</span>
          <span className="text-slate-500">{productSlug.join(" / ")}</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="rounded-full border border-emerald-200 bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700">
            {productDetail.stockQuantity > 0 ? "In stock" : "Out of stock"}
          </span>
          <span className="text-xs text-slate-500">SKU {productDetail.sku}</span>
        </div>
      </div>

      <div className="grid gap-10 lg:grid-cols-[1.2fr_0.8fr]">

        <div>
          <div className="w-full rounded-3xl border border-white/60 bg-white/80 p-6 shadow-[0_30px_60px_-45px_rgba(15,23,42,0.45)] backdrop-blur">
            {/* <div className="w-full"> */}
            <ImagesCarousel images={[...productDetail.images, ...productDetail.images]} />
            {/* </div> */}
          </div>

          <div className="mt-8 rounded-3xl border border-white/60 bg-white/80 p-6 shadow-[0_30px_60px_-45px_rgba(15,23,42,0.45)] backdrop-blur pt-6">
            <h2 className="text-lg font-semibold text-slate-900">Thông số kỹ thuật</h2>
            <ProductAttributesSection attributes={productDetail.attributes} />
          </div>
        </div>

        {/* Phần bên phải */}
        <div className="lg:sticky lg:top-6 h-fit rounded-3xl border border-slate-200/80 bg-white/90 p-6 shadow-[0_25px_50px_-35px_rgba(15,23,42,0.5)] backdrop-blur">
          <h1 className="mt-6 text-3xl font-semibold text-slate-900 sm:text-4xl font-serif">{productDetail.name}</h1>
          <p className="mt-4 text-base leading-relaxed text-slate-600">
            {productDetail.shortDescription || "No short description provided yet."}
          </p>
          <div className="flex flex-wrap items-center gap-6">
            <div>
              <p className="text-xs uppercase tracking-[0.25em] text-slate-400">Price</p>
              <p className="mt-1 text-3xl font-semibold text-slate-900">{priceText}</p>
            </div>
            {/* <div className="rounded-2xl border border-emerald-100 bg-emerald-50 px-4 py-3">
              <p className="text-xs uppercase tracking-[0.2em] text-emerald-700">Rating</p>
              <p className="mt-1 text-lg font-semibold text-emerald-700">{productDetail.rating.toFixed(1)} / 5</p>
            </div>
            <div className="rounded-2xl border border-orange-100 bg-orange-50 px-4 py-3">
              <p className="text-xs uppercase tracking-[0.2em] text-orange-600">Sold</p>
              <p className="mt-1 text-lg font-semibold text-orange-600">{productDetail.soldQuantity}</p>
            </div> */}
            {otherProducts.length > 0 && (
              <div className="flex flex-wrap gap-2">
                {otherProducts.map((product) => (
                  <ProductVariant
                    key={product.id}
                    id={product.id}
                    name={product.name}
                    image={product.image}
                    sellingPrice={product.sellingPrice}
                  />
                ))}
              </div>
            )}
          </div>
          <div className="mt-8 space-y-3">
            <BuyNowButton item={{
              productId: productDetail.id,
              name: productDetail.name,
              sellingPrice: productDetail.sellingPrice,
              imageLink: productDetail.images[0] || "",
              quantity: 1,
              attributes: productDetail.attributes.filter(attr => attr.type === 1),
              sku: productDetail.sku,
            }} />
            <AddToCardButton
              productId={productDetail.id}
              className="flex w-full cursor-pointer items-center justify-center rounded-full border border-emerald-200 bg-emerald-600 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-emerald-200 transition hover:bg-emerald-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
            ></AddToCardButton>
            {/* <button
              type="button"
              className="flex w-full cursor-pointer items-center justify-center rounded-full border border-slate-200 bg-white px-6 py-3 text-sm font-semibold text-slate-700 transition hover:border-slate-300 hover:text-slate-900 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
              aria-label="Compare product"
            >
              Compare
            </button> */}
          </div>
          {/* <div className="mt-6 rounded-2xl border border-slate-100 bg-slate-50 p-4 text-xs text-slate-600">
            <p className="font-semibold text-slate-700">Shipping</p>
            <p className="mt-2">Free shipping for orders over 2,000,000 VND.</p>
            <p className="mt-1">Standard delivery: 2-4 business days.</p>
          </div> */}
        </div>
      </div>

      <div className="rounded-3xl border border-white/60 bg-white/80 p-8 shadow-[0_30px_60px_-45px_rgba(15,23,42,0.45)] backdrop-blur">
        <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">

          <div className="max-w-2xl">
            <h2 className="text-lg font-semibold text-slate-900">Chi tiết sản phẩm</h2>
            <p className="mt-3 text-base leading-relaxed text-slate-600">
              {productDetail.detailDescription || "Details are being updated. Please check back soon for full specs and performance notes."}
            </p>
          </div>

          <div className="rounded-2xl border border-emerald-100 bg-emerald-50 p-5 text-sm text-emerald-700">
            <p className="text-xs uppercase tracking-[0.25em] text-emerald-500">Warranty</p>
            <p className="mt-2 font-semibold">24-month official warranty</p>
            <p className="mt-1 text-emerald-600">Includes premium support and swap-in service.</p>
          </div>

        </div>
      </div>
    </div>
  );
}

function ProductAttributesSection({ attributes }: { attributes: { name: string; value: string }[] }) {
  if (!attributes.length) {
    return <p className="mt-4 text-sm text-slate-500">No specifications published yet.</p>;
  }

  return (
    <div className="mt-4 overflow-hidden rounded-2xl border border-slate-100 bg-white">
      <table className="w-full text-sm">
        <tbody className="divide-y divide-slate-100">
          {attributes.map((attr, index) => (
            <tr key={`${attr.name}-${index}`}>
              <th className="w-40 px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.2em] text-slate-400 sm:w-48">
                {attr.name}
              </th>
              <td className="px-4 py-3 text-sm font-semibold text-slate-700">
                {attr.value}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function ProductDetailSkeleton() {
  return (
    <div className="space-y-10">
      <div className="h-5 w-40 rounded-full bg-slate-200" />
      <div className="grid gap-10 lg:grid-cols-[1.1fr_0.9fr]">
        <div className="h-96 rounded-3xl bg-white/70" />
        <div className="h-80 rounded-3xl bg-white/70" />
      </div>
      <div className="h-40 rounded-3xl bg-white/70" />
    </div>
  );
}