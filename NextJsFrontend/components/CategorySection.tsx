import { Laptop, Keyboard, Headphones, Usb, BatteryCharging } from "lucide-react";
import type { CategoryCardModel, CategoryIconKey } from "@/features/type";

interface CategorySectionProps {
  categories: CategoryCardModel[];
}

const iconMap: Record<CategoryIconKey, typeof Laptop> = {
  laptop: Laptop,
  keyboard: Keyboard,
  headphones: Headphones,
  usb: Usb,
  charger: BatteryCharging,
};

export function CategorySection({ categories }: CategorySectionProps) {
  return (
    <section className="mx-auto mt-11 w-full max-w-7xl px-3 md:px-6">
      <div>
        <h2 className="mb-2 font-rubik text-2xl font-bold text-slate-900 md:text-3xl">
          Danh mục nổi bật
        </h2>
        <p className="text-sm text-slate-600 md:text-base">
          Chuyển nhanh đến nhóm sản phẩm bạn cần chỉ với một lần chạm.
        </p>
        <div className="mt-6 grid grid-cols-2 gap-3 md:grid-cols-5">
          {categories.map((category) => {
            const IconComponent = iconMap[category.iconKey];

            return (
              <div
                key={category.id}
                className="group cursor-pointer rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition-all duration-200 hover:-translate-y-1 hover:border-emerald-200 hover:shadow-lg"
              >
                <div className="mb-4 inline-flex rounded-xl bg-emerald-50 p-3 text-emerald-700">
                  <IconComponent className="h-6 w-6" />
                </div>
                <h3 className="font-semibold text-slate-800 transition-colors duration-200 group-hover:text-emerald-700">
                  {category.name}
                </h3>
                <p className="mt-1 text-xs text-slate-500">Tối ưu lựa chọn linh kiện</p>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
