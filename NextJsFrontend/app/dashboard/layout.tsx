import type { Metadata } from "next";
import { Noto_Serif } from "next/font/google";
import "@/app/globals.css";
import { cn } from "@/lib/utils";
import { Toaster } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import QueryProvider from "@/lib/QueryProvider";
import '@/wdyr';

export const notoSerif = Noto_Serif({
  variable: "--font-sans",
  subsets: ["latin"],
  display: "swap",
});

export const metadata: Metadata = {
  title: "TechStore - E-commerce",
  description: "Chuyên cung cấp Laptop, PC và linh kiện máy tính cao cấp.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // throw new Error("Lỗi test ở RootLayout component");
  return (
    <html
      lang="vi"
      className={cn("h-full", "antialiased", "font-sans", notoSerif.variable)}
    >
      <body className="min-h-full flex flex-col">
        <div className="min-h-screen bg-[radial-gradient(circle_at_top,#ecfeff_0%,#f8fafc_42%,#ffffff_100%)] text-slate-900 selection:bg-emerald-200">
          <QueryProvider>
            <TooltipProvider>
              {children}
            </TooltipProvider>
          </QueryProvider>
        </div>
        <Toaster toastOptions={{
          closeButton: true,
          duration: 3500,
          // unstyled: true,
          // classNames: {
          //   error: 'bg-red-400',
          //   success: 'text-green-400',
          //   warning: 'text-yellow-400',
          //   info: 'bg-blue-400',
          // },
        }} />
      </body>
    </html>
  );
}
