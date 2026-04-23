import { productRequest, SortBy } from "@/services/backend-requests/product-request";
import { ValueProposition, HeroBanner, Footer, Header } from "./_components";
import FeaturedProducts from "./_components/FeaturedProducts";

export default async function Home() {
  const laptopProducts = await productRequest.getSearchedProducts({
    categoryId: "019cf26f-e565-7be2-97af-4a1398a0a979",
    sortBy: SortBy.SoldQuantity,
    sortDirection: 1,
    pageIndex: 1,
    pageSize: 8
  }).then(res => res.payload?.data?.items ?? [])

  const headphonesProducts = await productRequest.getSearchedProducts({
    categoryId: "019d0ba6-2325-7ad0-b5d1-ed24ea14176b",
    sortBy: SortBy.SoldQuantity,
    sortDirection: 1,
    pageIndex: 1,
    pageSize: 8
  }).then(res => res.payload?.data?.items ?? [])

  return (
    <>
      <Header siteName="My Store" />
      <main>
        <HeroBanner />
        <ValueProposition />
        <FeaturedProducts products={laptopProducts} title="Laptop bán chạy" />
        <FeaturedProducts products={headphonesProducts} title="Tai nghe - chụp tai" />
      </main>
      <Footer />
    </>
  );
}
