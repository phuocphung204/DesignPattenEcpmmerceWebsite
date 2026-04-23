"use client";

import Image from "next/image";
import { useState } from "react";
import { PaymentType } from "@/services/client-requests/order-requests";
import paymentRequests, { ProcessCallbackResponseType } from "@/services/client-requests/payment-request";
import { useSearchParams } from "next/navigation";
import { useRouter } from "next/navigation";

// lấy current datetime offset theo định dạng "1776593500000"
const getCurrentDateTimeOffset = () => {
  const now = new Date();
  const timezoneOffset = now.getTimezoneOffset() * 60000;
  const localTime = new Date(now.getTime() - timezoneOffset);
  return localTime.getTime();
};

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(value || 0);
};

export function MomoPage() {
  const searchParams = useSearchParams();
  const router = useRouter();

  const paymentMethodType = searchParams.get("paymentMethodType") as PaymentType | null;
  const payType = searchParams.get("payType") as PaymentType | null;
  const cardType = searchParams.get("cardType");
  const orderId = searchParams.get("orderId");
  const amountParam = searchParams.get("amount");
  const amount = amountParam ? Number(amountParam) : 0;

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
      const responseTime = getCurrentDateTimeOffset();
      const res = await paymentRequests.mockMomoCallback({
        orderId: orderId ?? "",
        requestId: `${orderId}-${responseTime}`,
        amount,
        transId: responseTime,
        resultCode: 0,
        message: "Thanh toán thành công",
        payType: payType?.toString() ?? "momo",
        responseTime,
        bankCode: cardType ?? "MOMO",
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
      <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_top,#ffe4f3_0%,#fff7fb_45%,#ffffff_100%)]" />
      <div className="relative mx-auto flex w-full max-w-5xl flex-col gap-6 px-4 py-12">
        <header className="flex flex-col gap-4">
          <div className="inline-flex items-center gap-3 rounded-full border border-pink-200 bg-white/90 px-4 py-2 text-sm font-medium text-pink-700 shadow-sm">
            <span className="h-2 w-2 rounded-full bg-pink-500" />
            Thanh toán nhanh qua MoMo
          </div>
          <div className="flex flex-wrap items-center justify-between gap-4">
            <div className="flex items-center gap-4">
              <Image src="/logo-momo.svg.svg" alt="MoMo" width={56} height={56} className="rounded-xl bg-white p-2 shadow" />
              <div>
                <h1 className="text-3xl font-semibold text-slate-900">MoMo Payment</h1>
                <p className="text-sm text-slate-600">Xác nhận giao dịch trước khi gửi callback thử.</p>
              </div>
            </div>
            <div className="rounded-2xl border border-pink-100 bg-white/90 px-5 py-3 text-right shadow-sm">
              <p className="text-xs uppercase text-slate-500">Số tiền</p>
              <p className="text-2xl font-semibold text-pink-600">{formatCurrency(amount)}</p>
            </div>
          </div>
        </header>

        <div className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
          <div className="rounded-3xl border border-pink-100 bg-white/90 p-6 shadow-sm backdrop-blur">
            <h2 className="text-lg font-semibold text-slate-900">Thông tin giao dịch</h2>
            <div className="mt-4 grid gap-3 text-sm text-slate-700">
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Mã đơn hàng</span>
                <span className="font-medium text-slate-900">{orderId ?? "Chưa có"}</span>
              </div>
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Phương thức</span>
                <span className="font-medium text-slate-900">{paymentMethodType ?? "momo"}</span>
              </div>
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Loại thanh toán</span>
                <span className="font-medium text-slate-900">{payType ?? "momo"}</span>
              </div>
              <div className="flex items-center justify-between rounded-2xl border border-slate-100 bg-white px-4 py-3">
                <span>Kênh ngân hàng</span>
                <span className="font-medium text-slate-900">{cardType ?? "MoMo Wallet"}</span>
              </div>
            </div>
          </div>

          <div className="flex flex-col gap-4 rounded-3xl border border-pink-100 bg-white/90 p-6 shadow-sm">
            <h2 className="text-lg font-semibold text-slate-900">Thực hiện giao dịch</h2>
            <p className="text-sm text-slate-600">
              Nút bên dưới sẽ gửi mock callback về backend để cập nhật trạng thái đơn hàng.
            </p>
            <button
              type="button"
              onClick={handleMockPayment}
              disabled={!canSubmit || isSubmitting}
              className="w-full rounded-2xl bg-pink-600 px-5 py-3 text-sm font-semibold text-white shadow-lg shadow-pink-200 transition hover:bg-pink-700 disabled:cursor-not-allowed disabled:bg-slate-300"
            >
              {isSubmitting ? "Đang xử lý..." : "Gửi callback MoMo"}
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