import { Gift, Rocket, ShieldCheck, Wrench } from "lucide-react";
import { siteConfig } from "@/config";
import { ValueIconKey } from "@/config/site";

const iconMap: Record<ValueIconKey, typeof Gift> = {
  gift: Gift,
  shield: ShieldCheck,
  rocket: Rocket,
  wrench: Wrench,
};

export default function ValueProposition() {
  const siteName = siteConfig.site.name;
  const siteDescription = siteConfig.site.description;
  const valuePropositions = siteConfig.valuePropositions;

  return (
    <section className="mx-auto mt-14 w-full max-w-7xl px-3 md:px-6">
      <div className="rounded-3xl border border-emerald-100 bg-linear-to-br from-emerald-50 via-white to-sky-50 px-5 py-10 md:px-8 md:py-12">
        <div className="mx-auto mb-10 max-w-3xl text-center">
          <h2 className="mb-4 font-rubik text-3xl font-bold text-slate-900">Tại sao chọn {siteName}?</h2>
          <p className="text-slate-600">
            {siteDescription}
          </p>
        </div>

        <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-4">
          {valuePropositions.map((item) => {
            const IconComponent = iconMap[item.iconKey];

            return (
              <article
                key={item.id}
                className="rounded-2xl border border-white bg-white/90 p-5 shadow-sm transition-all duration-200 hover:-translate-y-1 hover:shadow-lg"
              >
                <div className="mb-5 inline-flex rounded-xl bg-emerald-100 p-3 text-emerald-700">
                  <IconComponent className="h-6 w-6" />
                </div>
                <h3 className="mb-2 text-lg font-bold text-slate-900">{item.title}</h3>
                <p className="text-sm leading-relaxed text-slate-600">{item.description}</p>
              </article>
            );
          })}
        </div>
      </div>
    </section>
  );
}
