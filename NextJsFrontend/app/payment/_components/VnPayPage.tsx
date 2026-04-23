"use client";

import Image from "next/image";
import { useState } from "react";
import { PaymentType } from "@/services/client-requests/order-requests";
import paymentRequests, { ProcessCallbackResponseType } from "@/services/client-requests/payment-request";
import { useSearchParams } from "next/navigation";
import { useRouter } from "next/navigation";

export function randomTransactionId() {
  return Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
}

// làm lấy datetime hiện tại theo định dạng "20260419153020"
function getCurrentDateTime(): string {
  const now = new Date();

  const year = now.getFullYear().toString();
  const month = (now.getMonth() + 1).toString().padStart(2, "0");
  const day = now.getDate().toString().padStart(2, "0");
  const hours = now.getHours().toString().padStart(2, "0");
  const minutes = now.getMinutes().toString().padStart(2, "0");
  const seconds = now.getSeconds().toString().padStart(2, "0");

  return `${year}${month}${day}${hours}${minutes}${seconds}`;
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(value || 0);
};

export default function PaymentPage() {
  const searchParams = useSearchParams();
  const router = useRouter();

  const paymentMethodType = searchParams.get("paymentMethodType") as PaymentType | null;
  const payType = searchParams.get("payType") as PaymentType | null;
  const cardType = searchParams.get("cardType");
  const orderId = searchParams.get("orderId");
  const amountParam = searchParams.get("amount");
  const amount = amountParam ? Number(amountParam) : 0;

  const vnp_TransactionNo = randomTransactionId();

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [response, setResponse] = useState<ProcessCallbackResponseType | null>(null);

  const canSubmit = Boolean(orderId) && amount > 0 && !Number.isNaN(amount);

  const handleMockPayment = async () => {
    if (!canSubmit) {
      setError("Vui lòng kiểm tra orderId và số tiền.");
      return;
    }

    setIsSubmitting(true);
    setError(null);
    setResponse(null);

    try {
      const res = await paymentRequests.mockVnPayCallback({
        vnp_TxnRef: orderId ?? "",
        vnp_Amount: amount,
        vnp_TransactionNo,
        vnp_ResponseCode: "00",
        vnp_BankCode: cardType ?? "NCB",
        vnp_CardType: cardType ?? "ATM",
        vnp_PayDate: getCurrentDateTime(),
        vnp_OrderInfo: `Thanh toan don hang ${orderId ?? ""}`,
      });
      setResponse(res.payload?.data ?? null);
      router.replace("thanh-toan/don-hang?orderId=" + orderId);
    } catch (err) {
      const message = err instanceof Error ? err.message : "Có lỗi xảy ra. Vui lòng thử lại.";
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <section className="relative overflow-hidden">
      <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_top,#e0f2fe_0%,#f8fafc_46%,#ffffff_100%)]" />
      <div className="relative mx-auto flex w-full max-w-5xl flex-col gap-6 px-4 py-12">
        <header className="flex flex-col gap-4">
          <div className="inline-flex items-center gap-3 rounded-full border border-sky-200 bg-white/90 px-4 py-2 text-sm font-medium text-sky-700 shadow-sm">
            <span className="h-2 w-2 rounded-full bg-sky-500" />
            Thanh toán an toàn qua VNPAY
          </div>
          <div className="flex flex-wrap items-center justify-between gap-4">
            <div className="flex items-center gap-4">
              <Image src="/vnpay-logo-inkythuatso.svg" alt="VNPAY" width={56} height={56} className="rounded-xl bg-white p-2 shadow" />
              <div>
                <h1 className="text-3xl font-semibold text-slate-900">VNPay Checkout</h1>
                <p className="text-sm text-slate-600">Kiểm tra chi tiết giao dịch trước khi gửi callback.</p>
              </div>
            </div>
            <div className="rounded-2xl border border-sky-100 bg-white/90 px-5 py-3 text-right shadow-sm">
              <p className="text-xs uppercase text-slate-500">Tổng thanh toán</p>
              <p className="text-2xl font-semibold text-sky-600">{formatCurrency(amount)}</p>
            </div>
          </div>
        </header>

        <div className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
          <div className="rounded-3xl border border-sky-100 bg-white/90 p-6 shadow-sm backdrop-blur">
            <h2 className="text-lg font-semibold text-slate-900">Thông tin giao dịch</h2>
            <div className="mt-4 grid gap-3 text-sm text-slate-700">
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Mã đơn hàng</span>
                <span className="font-medium text-slate-900">{orderId ?? "Chưa có"}</span>
              </div>
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Phương thức</span>
                <span className="font-medium text-slate-900">{paymentMethodType ?? "vnpay"}</span>
              </div>
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Loại thanh toán</span>
                <span className="font-medium text-slate-900">{payType ?? "vnpay"}</span>
              </div>
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Mã giao dịch</span>
                <span className="font-medium text-slate-900">{vnp_TransactionNo}</span>
              </div>
            </div>
          </div>

          <div className="flex flex-col gap-4 rounded-3xl border border-sky-100 bg-white/90 p-6 shadow-sm">
            <h2 className="text-lg font-semibold text-slate-900">Thực hiện giao dịch</h2>
            <p className="text-sm text-slate-600">
              Nhấn để gửi mock callback VNPay và cập nhật trạng thái đơn hàng.
            </p>
            <button
              type="button"
              onClick={handleMockPayment}
              disabled={!canSubmit || isSubmitting}
              className="w-full rounded-2xl bg-sky-600 px-5 py-3 text-sm font-semibold text-white shadow-lg shadow-sky-200 transition hover:bg-sky-700 disabled:cursor-not-allowed disabled:bg-slate-300"
            >
              {isSubmitting ? "Đang xử lý..." : "Gửi callback VNPay"}
            </button>
            {!canSubmit && (
              <p className="text-xs text-slate-500">Cần có orderId và số tiền hợp lệ từ URL.</p>
            )}
            {error && (
              <div className="rounded-2xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">
                {error}
              </div>
            )}
            {response && (
              <div className="rounded-2xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-xs text-emerald-700">
                <p className="font-semibold">Đã gửi callback thành công.</p>
                <pre className="mt-2 whitespace-pre-wrap font-mono text-[11px] text-emerald-800">
                  {JSON.stringify(response, null, 2)}
                </pre>
              </div>
            )}
          </div>
        </div>
      </div>
    </section>
  );
}