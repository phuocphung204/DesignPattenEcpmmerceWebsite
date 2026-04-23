

import { Header } from "@/app/_components";
import OrderDetailsClient from "./_components/OrderDetailsClients";


export default function Page() {

  return (
    <>
      <Header />
      <main className="mx-auto flex w-full max-w-6xl flex-col gap-6 px-4 py-10">
        <OrderDetailsClient />
      </main>
    </>
  )
}