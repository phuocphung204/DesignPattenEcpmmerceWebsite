import { ProductCategory } from "@/entities/product-category/ui/ProductCategory";
import productCategoryRequest from "@/services/backend-requests/product-category-request";
import { Search, ShoppingCart, Menu } from "lucide-react";
import Link from "next/link";
import TempName from "./TempName";
import CartCount from "./CartCount";

interface HeaderProps {
  siteName?: string;
};

export default async function Header({ siteName = "My Store" }: HeaderProps) {
  const result = await productCategoryRequest.getLookupList();
  const proCateList = result.payload?.data || [];
  const rootProCate = proCateList?.filter(cate => cate.level === 0);

  // throw new Error("Lỗi test ở Header component");

  return (
    <header className="sticky top-0 z-50 w-full bg-white/90 border border-emerald-100 shadow-[0_10px_35px_-18px_rgba(5,150,105,0.45)] backdrop-blur-md">
      {/* Header top */}
      <div className="mx-auto flex w-full max-w-7xl h-16 items-center justify-between gap-3 md:gap-5">
        <Link className="flex items-center gap-2"
          href="/"
        >
          <button
            type="button"
            className="rounded-lg p-1.5 text-slate-700 transition-colors duration-200 hover:bg-emerald-50 hover:text-emerald-700 focus-visible:ring-2 focus-visible:ring-emerald-500 sm:hidden"
            aria-label="Mở menu"
          >
            <Menu className="h-5 w-5" />
          </button>
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-600 font-rubik text-xl font-bold text-white">
            T
          </div>
          <span className="hidden font-rubik text-xl font-bold tracking-tight sm:block">
            {siteName.replace("Store", "")}
            <span className="text-emerald-600">Store</span>
          </span>
        </Link>

        <div className="relative hidden max-w-2xl flex-1 md:block">
          <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
            <Search className="h-5 w-5 text-slate-400" />
          </div>
          <input
            type="text"
            className="block h-11 w-full rounded-xl border border-slate-200 bg-slate-50 pl-10 pr-3 text-sm text-slate-900 placeholder:text-slate-400 focus-visible:ring-2 focus-visible:ring-emerald-500"
            placeholder="Tìm kiếm theo tên hoặc SKU (vd: FX607VJ-RL034W)..."
            aria-label="Tìm kiếm sản phẩm theo tên hoặc SKU"
          />
        </div>

        <div className="flex items-center gap-2 sm:gap-3">
          <Link
            href="/gio-hang"
            type="button"
            className="rounded-xl p-2 text-slate-700 transition-colors duration-200 hover:bg-emerald-50 hover:text-emerald-700 focus-visible:ring-2 focus-visible:ring-emerald-500"
            aria-label="Giỏ hàng"
          >
            <span className="relative block">
              <ShoppingCart className="h-6 w-6" />
              <CartCount />
            </span>
          </Link>
          {/* Button đăng nhập */}
          <TempName />
        </div>
      </div>
      {/* Header bottom */}
      <div className="border-t border-emerald-100/80 bg-linear-to-r from-emerald-100/70 via-white to-emerald-100/70">
        <nav
          className="mx-auto flex not-first-of-type:w-full max-w-7xl items-center justify-center overflow-x-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
          aria-label="Danh mục sản phẩm"
        >
          {rootProCate?.map((cate) => (
            <div key={cate.id} className="flex items-center">
              <ProductCategory {...cate} />
            </div>
          ))}
        </nav>
      </div>
    </header>
  );
}
