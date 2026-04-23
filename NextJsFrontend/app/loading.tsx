export default function Loading() {
  return (
    <main className="relative min-h-screen overflow-hidden">
      <div className="pointer-events-none absolute inset-0">
        <div className="absolute -top-24 left-1/2 h-72 w-72 -translate-x-1/2 rounded-full bg-emerald-300/40 blur-3xl" />
        <div className="absolute bottom-0 right-[-10%] h-80 w-80 rounded-full bg-orange-300/35 blur-3xl" />
        <div className="absolute top-28 left-[-10%] h-64 w-64 rounded-full bg-sky-300/30 blur-3xl" />
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,rgba(16,185,129,0.14),transparent_45%),radial-gradient(circle_at_78%_32%,rgba(249,115,22,0.18),transparent_52%)]" />
      </div>

      <div className="relative mx-auto flex min-h-screen max-w-5xl items-center justify-center px-6">
        <div className="w-full max-w-xl rounded-3xl border border-white/60 bg-white/70 p-8 shadow-[0_24px_70px_-36px_rgba(15,23,42,0.55)] backdrop-blur-xl">
          <div className="flex flex-wrap items-center justify-between gap-3">
            <span className="inline-flex items-center gap-2 rounded-full bg-emerald-100/80 px-3 py-1 text-xs font-semibold text-emerald-700">
              TechStore Loading
            </span>
            <span className="text-xs text-slate-500">Bao mat ket noi</span>
          </div>

          <div className="mt-6">
            <h1 className="text-2xl font-semibold text-slate-900">
              Dang chuan bi trai nghiem mua sam
            </h1>
            <p className="mt-2 text-sm text-slate-600">
              Dang dong bo san pham may tinh, laptop va linh kien moi nhat.
            </p>
          </div>

          <div className="mt-6 space-y-4">
            <div className="h-3 w-full overflow-hidden rounded-full bg-slate-200">
              <div className="h-full w-2/3 animate-pulse rounded-full bg-gradient-to-r from-emerald-500 via-emerald-400 to-orange-400" />
            </div>
            <div className="flex items-center justify-between text-xs text-slate-500">
              <span>Dang tinh chinh ton kho</span>
              <span className="font-medium text-emerald-700">On dinh</span>
            </div>
          </div>

          <div className="mt-8 grid gap-3 sm:grid-cols-3">
            {Array.from({ length: 3 }).map((_, index) => (
              <div
                key={`loading-card-${index}`}
                className="rounded-2xl border border-white/60 bg-white/60 p-4 shadow-sm"
              >
                <div className="h-4 w-12 rounded-full bg-emerald-200/70" />
                <div className="mt-4 space-y-2">
                  <div className="h-3 w-3/4 animate-pulse rounded-full bg-slate-200" />
                  <div className="h-3 w-2/3 animate-pulse rounded-full bg-slate-200" />
                </div>
                <div className="mt-4 h-16 rounded-xl bg-slate-100" />
              </div>
            ))}
          </div>

          <div className="mt-8 flex flex-wrap items-center gap-3" role="status" aria-live="polite" aria-busy="true">
            <span className="sr-only">Dang tai</span>
            <span className="h-2.5 w-2.5 animate-bounce rounded-full bg-emerald-500 [animation-delay:-0.3s]" />
            <span className="h-2.5 w-2.5 animate-bounce rounded-full bg-emerald-400 [animation-delay:-0.15s]" />
            <span className="h-2.5 w-2.5 animate-bounce rounded-full bg-orange-400" />
            <span className="text-xs text-slate-600">Dang xu ly du lieu</span>
          </div>
        </div>
      </div>
    </main>
  );
}
