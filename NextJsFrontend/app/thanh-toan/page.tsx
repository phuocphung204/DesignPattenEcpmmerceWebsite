import { Header } from "../_components";
import CheckoutClient from "./_components/CheckoutClient";

export default function CheckoutPage() {

  return (
    <>
      <Header />
      <main className="mx-auto max-w-7xl">
        <CheckoutClient />
        {/* <Checkout1 /> */}
      </main>
    </>
  );
}