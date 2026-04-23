"use cache";

import LoginForm from "../_components/LoginForm";

export default async function LoginPage() {
  return (
    <div className="relative overflow-hidden px-4 py-12 sm:px-6 lg:px-10 lg:py-16">
      <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_top_right,rgba(5,150,105,0.14),transparent_42%),radial-gradient(circle_at_bottom_left,rgba(249,115,22,0.14),transparent_40%)]" />
      <div className="pointer-events-none absolute -left-12 top-8 h-36 w-36 rounded-full border border-emerald-300/60 bg-white/30 blur-sm" />
      <div className="pointer-events-none absolute -right-12 bottom-10 h-44 w-44 rounded-full border border-orange-300/60 bg-white/20 blur-sm" />

      <section className="relative mx-auto grid w-full max-w-6xl gap-8 lg:grid-cols-[1.1fr_0.9fr] lg:items-stretch">
        <aside className="overflow-hidden rounded-3xl border border-emerald-100/70 bg-white/75 p-7 shadow-[0_26px_80px_-38px_rgba(15,23,42,0.45)] backdrop-blur-sm sm:p-10">
          <p className="inline-flex items-center rounded-full border border-emerald-200 bg-emerald-50 px-4 py-1 text-xs font-semibold tracking-[0.18em] text-emerald-700 uppercase">
            TechStore Access
          </p>
          <h1 className="mt-4 text-balance text-3xl leading-tight font-semibold text-slate-900 sm:text-4xl">
            Đăng nhập để mở trọn hệ sinh thái laptop, PC và linh kiện hiệu năng cao.
          </h1>
          <p className="mt-4 max-w-xl text-pretty text-base leading-relaxed text-slate-600">
            Theo dõi đơn hàng realtime, đồng bộ giỏ hàng đa thiết bị và nhận ưu đãi độc quyền cho thành viên tại TechStore.
          </p>

          <div className="mt-8 grid gap-4 sm:grid-cols-2">
            <article className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition duration-200 hover:-translate-y-0.5 hover:shadow-md">
              <p className="text-sm font-semibold text-slate-900">Giữ cấu hình đang build</p>
              <p className="mt-1 text-sm text-slate-600">Lưu dàn PC, laptop wishlist và so sánh hiệu năng chỉ trong một tài khoản.</p>
            </article>
            <article className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition duration-200 hover:-translate-y-0.5 hover:shadow-md">
              <p className="text-sm font-semibold text-slate-900">Ưu đãi theo cấp thành viên</p>
              <p className="mt-1 text-sm text-slate-600">Nhận mã giảm giá linh kiện, combo nâng cấp và voucher freeship mỗi tháng.</p>
            </article>
            <article className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition duration-200 hover:-translate-y-0.5 hover:shadow-md sm:col-span-2">
              <p className="text-sm font-semibold text-slate-900">Hỗ trợ kỹ thuật ưu tiên</p>
              <p className="mt-1 text-sm text-slate-600">Đội ngũ kỹ thuật xác thực nhanh cấu hình tương thích cho CPU, mainboard, RAM và PSU.</p>
            </article>
          </div>
        </aside>

        <div className="animate-in fade-in slide-in-from-bottom-2 duration-700">
          <LoginForm />
        </div>
      </section>
    </div>
  );
}