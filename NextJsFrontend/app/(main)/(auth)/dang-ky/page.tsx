"use cache";

import addressApi from "@/services/address-api";
import RegisterForm from "../_components/RegisterForm";

export default async function RegisterPage() {
  const provinces = await addressApi.listProvinces();

  return (
    <div className="relative overflow-hidden px-4 py-12 sm:px-6 lg:px-10 lg:py-16">
      <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_top_right,rgba(5,150,105,0.14),transparent_42%),radial-gradient(circle_at_bottom_left,rgba(249,115,22,0.14),transparent_40%)]" />
      <div className="pointer-events-none absolute -left-12 top-8 h-36 w-36 rounded-full border border-emerald-300/60 bg-white/30 blur-sm" />
      <div className="pointer-events-none absolute -right-12 bottom-10 h-44 w-44 rounded-full border border-orange-300/60 bg-white/20 blur-sm" />

      <section className="relative mx-auto w-full max-w-5xl">
        <div className="animate-in fade-in slide-in-from-bottom-2 duration-700">
          <RegisterForm initialProvinces={provinces} />
        </div>
      </section>
    </div>
  );
}