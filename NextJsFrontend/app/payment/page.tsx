import { CreditType, PaymentMethodType, PaymentType } from "@/services/client-requests/order-requests";
import { MomoPage } from "./_components/MomoPage";
import VnPayPage from "./_components/VnPayPage";

type Props = {
  searchParams: Promise<{
    paymentMethodType: PaymentMethodType;
    payType: PaymentType;
    cardType: CreditType;
    orderId: string;
    amount: number;
  }>
}

const paymentMethodTypeToNameMap: Record<PaymentMethodType, React.JSX.Element> = {
  [PaymentMethodType.VnPay]: <MomoPage />,
  [PaymentMethodType.Momo]: <VnPayPage />,
}

export type BANK_CODE = "MB" | "VCB" | "TCB" | "AGRIBANK" | "ACB" | "VPBANK" | "NCB" | "SACOMBANK" | "EXIMBANK" | "MSBANK" | "OCB" | "IVB" | "SHB";

export default async function PaymentPage({ searchParams }: Props) {
  const { paymentMethodType } = await searchParams;
  console.log(">>> paymentMethodType", paymentMethodType);
  return (
    <>
      {renderPaymentMethod(paymentMethodType as PaymentMethodType)}
    </>
  )
}

function renderPaymentMethod(paymentMethodType: PaymentMethodType) {
  return paymentMethodTypeToNameMap[paymentMethodType];
}
