import Link from "next/link";
import ProductCategoryType from "../type";

export function ProductCategory({ name, slug }: ProductCategoryType) {
  return (
    <Link
      href={`/?category=${encodeURIComponent(slug)}`}
      className="group relative inline-flex h-10 shrink-0 cursor-pointer items-center px-3 text-sm font-semibold text-slate-700 transition-all duration-200 hover:bg-emerald-100 hover:text-emerald-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
      aria-label={`Xem danh mục ${name}`}
    >
      <span className="max-w-44 truncate">{name}</span>
      <span className="pointer-events-none absolute inset-x-3 bottom-0 h-0.75 origin-left scale-x-0 rounded-full bg-emerald-600 transition-transform duration-200 group-hover:scale-x-100 group-focus-visible:scale-x-100" />
    </Link>
  );
}