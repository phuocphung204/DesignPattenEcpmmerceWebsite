"use client"

import { Send } from "lucide-react";
import { FacebookIcon, YouTubeIcon } from "@/components/icons";
import { Phone, Mail, MapPin } from "lucide-react";
import type { FooterColumn } from "@/features/type";
import { siteConfig } from "@/config";
import { useEffect, useState } from "react";

interface SiteContact {
  address: string;
  phone: string;
  email: string;
}

interface SiteSummary {
  name: string;
  description: string;
  contact: SiteContact;
}

export default function Footer() {
  const site: SiteSummary = siteConfig.site;
  const footerColumns: FooterColumn[] = siteConfig.footerColumns;
  const newsletterDescription: string = siteConfig.newsletterDescription;

  const [year, setYear] = useState<number | null>(null);
  // eslint-disable-next-line react-hooks/set-state-in-effect
  useEffect(() => setYear(new Date().getFullYear()), []);

  return (
    <footer className="border-t border-slate-200 bg-slate-950 text-slate-300">
      <div className="mx-auto w-full max-w-7xl px-4 py-16 md:px-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-12">
          <div>
            <div className="flex items-center gap-2 mb-6">
              <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-600 font-rubik text-xl font-bold text-white">
                T
              </div>
              <span className="font-rubik font-bold text-2xl tracking-tight text-white">
                {site.name.replace("Store", "")}
                <span className="text-emerald-500">Store</span>
              </span>
            </div>
            <p className="mb-6 text-sm leading-relaxed text-slate-400">
              {site.description}
            </p>
            <ul className="space-y-3 text-sm text-slate-400">
              <li className="flex items-start gap-2">
                <MapPin className="mt-0.5 h-4 w-4" />
                <span>{site.contact.address}</span>
              </li>
              <li className="flex items-center gap-2">
                <Phone className="h-4 w-4" />
                <span>{site.contact.phone}</span>
              </li>
              <li className="flex items-center gap-2">
                <Mail className="h-4 w-4" />
                <span>{site.contact.email}</span>
              </li>
            </ul>
          </div>

          {footerColumns.map((column, idx) => (
            <div key={idx}>
              <h4 className="mb-6 font-rubik text-lg font-bold text-white">
                {column.title}
              </h4>
              <ul className="space-y-3">
                {column.links.map((link, linkIdx) => (
                  <li key={linkIdx}>
                    <a
                      href={link.href}
                      className="text-sm text-slate-400 transition-colors duration-200 hover:text-emerald-400"
                    >
                      {link.title}
                    </a>
                  </li>
                ))}
              </ul>
            </div>
          ))}

          <div>
            <h4 className="mb-6 font-rubik text-lg font-bold text-white">
              Nhận ưu đãi
            </h4>
            <p className="mb-4 text-sm text-slate-400">
              {newsletterDescription}
            </p>
            <form className="flex mb-6" onSubmit={(e) => e.preventDefault()}>
              <label htmlFor="newsletter-email" className="sr-only">
                Email nhận ưu đãi
              </label>
              <input
                id="newsletter-email"
                type="email"
                placeholder="Email của bạn"
                className="w-full rounded-l-md border border-slate-700 bg-slate-900 px-4 py-2 text-sm text-white placeholder:text-slate-500 focus-visible:ring-2 focus-visible:ring-emerald-500"
              />
              <button
                type="submit"
                className="rounded-r-md bg-emerald-600 px-4 py-2 text-white transition-colors duration-200 hover:bg-emerald-700 focus-visible:ring-2 focus-visible:ring-emerald-500"
                aria-label="Đăng ký"
              >
                <Send className="w-4 h-4" />
              </button>
            </form>
            <div className="flex gap-4">
              <a
                href="#"
                aria-label="Facebook"
                className="flex h-10 w-10 items-center justify-center rounded-full bg-slate-800 transition-colors duration-200 hover:bg-emerald-600"
              >
                <FacebookIcon />
              </a>
              <a
                href="#"
                aria-label="YouTube"
                className="flex h-10 w-10 items-center justify-center rounded-full bg-slate-800 transition-colors duration-200 hover:bg-emerald-600"
              >
                <YouTubeIcon />
              </a>
            </div>
            <div className="mt-6 rounded-lg border border-slate-700 bg-slate-900 px-3 py-2 text-sm font-semibold text-slate-300">
              Đã thông báo Bộ Công Thương
            </div>
          </div>
        </div>

        <div className="mt-16 flex flex-col items-center justify-between gap-4 border-t border-slate-800 pt-8 md:flex-row">
          <p className="text-sm text-slate-500">&copy; {year} {site.name}. Đã bảo lưu mọi quyền.</p>
          <div className="text-sm text-slate-500">Hotline kỹ thuật 24/7: {site.contact.phone}</div>
        </div>
      </div>
    </footer>
  );
}
