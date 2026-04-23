import { Footer, Header } from "@/app/_components";

export default function AuthLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <>
      <Header siteName="My Store" />
      <main>
        {children}
      </main>
      <Footer />
    </>
  );
}