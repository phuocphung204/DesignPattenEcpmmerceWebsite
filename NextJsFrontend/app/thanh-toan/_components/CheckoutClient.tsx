"use client"
import { CartItemCard } from "@/app/gio-hang/_components";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { Button } from "@/components/ui/button";
import { AddressFormData, AddressType } from "@/entities/address/type";
import { useCardCheckout } from "@/entities/cart";
import { AddressForm, addressSchema } from "@/features/address";
import { zodResolver } from "@hookform/resolvers/zod";
import { AlertCircleIcon, Minus, Plus } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { UserAddressBookDialog } from "./UserAddressBookDialog";
import orderRequests, { applyDiscount, ApplyDiscountResponseType, CreateOrderRequest, PaymentInfoRequest, PaymentType, ShippingType } from "@/services/client-requests/order-requests";
import { z } from "zod";
import ShippingMethodSection, { shippingFeeMap } from "./ShippingMethodSection";
import PaymentSection, { paymentSchema, type PaymentFormData } from "./PaymentSection";
import DiscountAndNoteSection, { DiscountAndNoteFormData, discountAndNoteSchema } from "./DiscountAndNoteSection";
import SummaryOrderSection from "./SummaryOrderSection";
import { useGetCurrentUser } from "@/entities/user";
import { HttpError, MyCustomResponse } from "@/services/http";
import getErrorMessage from "@/utils/error-helper";
import { toast } from "sonner";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { useRouter, useSearchParams } from "next/navigation";
import { useOrderInfo } from "@/entities/order";

const shippingSchema = z.object({
  type: z.enum(ShippingType, {
    message: "Vui lòng chọn phương thức giao hàng"
  }),
});


export type ShippingFormData = z.infer<typeof shippingSchema>;

export default function CheckoutClient() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const loadBackup = searchParams.get("loadBackup") === "true";
  const { orderInfo: backupOrderInfo } = useOrderInfo<AddressFormData, ShippingFormData, DiscountAndNoteFormData, PaymentFormData>();
  const { data: loyaltyPoints } = useGetCurrentUser((user) => user.loyaltyPoints);
  const addressFormHook = useForm<AddressFormData>({
    resolver: zodResolver(addressSchema),
    mode: "onChange",
  });
  const [shippingAddressId, setShippingAddressId] = useState<string | null>(null);
  const shippingFormHook = useForm<ShippingFormData>({
    resolver: zodResolver(shippingSchema),
    mode: "onChange",
  });
  const paymentFormHook = useForm<PaymentFormData>({
    resolver: zodResolver(paymentSchema),
    mode: "onChange",
  });
  const discountAndNoteFormHook = useForm<DiscountAndNoteFormData>({
    resolver: zodResolver(discountAndNoteSchema),
    mode: "all",
    defaultValues: {
      pointsUsed: 0,
    },
    values: {
      loyaltyPoints: loyaltyPoints ?? 0,
      pointsUsed: 0
    }
  });
  const [activeAccordion, setActiveAccordion] = useState<string[]>(["item-2"]);
  const { removeItem, updateItemQuantity, items: productItems } = useCardCheckout();

  // useEffect(() => {
  //   setItems([
  //     {
  //       productId: "019da54f-498f-7cef-83e5-138710103d26",
  //       imageLink: "https://example.com/image1.jpg",
  //       sku: "SKU001",
  //       name: "Sản phẩm 1",
  //       quantity: 2,
  //       sellingPrice: 100000,
  //       attributes: [
  //         { name: "Màu sắc", value: "Đỏ" },
  //         { name: "Kích thước", value: "L" },
  //       ]
  //     }
  //   ]);
  // }, []);

  const paymentType = paymentFormHook.getValues("paymentType");
  const isValidFormHookPayment = paymentFormHook.formState.isValid;

  const isValidAddress = addressFormHook.formState.isValid;
  const isValidShipping = shippingFormHook.formState.isValid;
  const isValidPayment = useMemo(() => {
    if (paymentType === PaymentType.Cash)
      return true;
    return isValidFormHookPayment;
  }, [isValidFormHookPayment, paymentType]);
  // const isValidOrder = !paymentFormHook.formState.isValid || isValidAddress || isValidShipping;
  const isValidOrder = isValidAddress && isValidShipping && isValidPayment;
  useEffect(() => { console.log(">>> isValidPayment: ", isValidPayment, isValidOrder) }, [isValidPayment, isValidOrder])

  const shippingType = shippingFormHook.getValues("type");
  const subTotal = productItems.reduce((total, item) => total + (item.sellingPrice * item.quantity), 0);
  const pointsUsed = discountAndNoteFormHook.formState.isValid ? (discountAndNoteFormHook.getValues("pointsUsed") ?? 0) : 0;
  const shippingFee = shippingType !== undefined ? shippingFeeMap[shippingType] : 0;
  const [applyDiscountResponse, setApplyDiscountResponse] = useState<ApplyDiscountResponseType | null>(null);
  const orderTotal = useMemo(() => {
    let total = subTotal + shippingFee;
    if (applyDiscountResponse) {
      total -= applyDiscountResponse.discountAmount;
    }
    if (pointsUsed > 0) {
      total -= pointsUsed * 1000;
    }
    return Math.max(0, total);
  }, [subTotal, shippingFee, pointsUsed, applyDiscountResponse]);

  // Load backup từ tanstack query nếu tạo đơn hàng bị lỗi và redirect về trang checkout với ?loadBackup=true
  // lúc này sẽ load lại dữ liệu đã lưu trong tanstack query store vào form
  useEffect(() => {
    if (!loadBackup || !backupOrderInfo) return;
    if (backupOrderInfo.address)
      addressFormHook.reset(backupOrderInfo.address);
    if (backupOrderInfo.shipping)
      shippingFormHook.reset(backupOrderInfo.shipping);
    if (backupOrderInfo.payment)
      paymentFormHook.reset(backupOrderInfo.payment);
    if (backupOrderInfo.discountAndNote)
      discountAndNoteFormHook.reset(backupOrderInfo.discountAndNote);
  }, [loadBackup, backupOrderInfo, addressFormHook, shippingFormHook, paymentFormHook, discountAndNoteFormHook]);

  const onContinue = (value: string) => {
    setActiveAccordion(prev => [...prev, value]);
  };

  const handleOnValueChange = (value: string[]) => {
    setActiveAccordion(value);
  };

  const handleSelectAddress = (address: AddressType) => {
    addressFormHook.reset(address)
    setShippingAddressId(address.id);
  }

  const handleApplyDiscount = async () => {
    const discountCode = discountAndNoteFormHook.getValues("discountCode");
    if (!discountCode) return;
    try {
      const res = await applyDiscount({ code: discountCode, subTotal });
      discountAndNoteFormHook.clearErrors("discountCode");
      console.log(">>> apply discount response: ", res);
      setApplyDiscountResponse(res.payload?.data ?? null);
    } catch (error) {
      if (error instanceof HttpError) {
        const errorRes = error.payload as MyCustomResponse;
        const message = getErrorMessage(errorRes.errorCode);
        discountAndNoteFormHook.setError("discountCode", { message: message || "Áp dụng mã giảm giá thất bại. Vui lòng thử lại." })
      } else {
        toast.error("Server đang có lỗi. Vui lòng thử lại sau.");
      }
      setApplyDiscountResponse(null);
    }
  }

  const handleSubmitOrder = async ({
    shipping, payment, discountAndNote
  }: {
    shipping: ShippingFormData; payment: PaymentFormData; discountAndNote: DiscountAndNoteFormData
  }) => {
    if (shippingAddressId === null) {
      toast.error("Vui lòng chọn địa chỉ giao hàng.");
      return;
    }
    try {
      if (productItems.length === 0) {
        toast.error("Giỏ hàng trống, vui lòng chọn sản phẩm.");
        return;
      }

      let paymentPayload: PaymentInfoRequest | null = null;
      if (payment.paymentMode === "COD") {
        const payload: PaymentInfoRequest = {
          type: PaymentType.Cash,
        }
        paymentPayload = payload;
      } else if (payment.paymentType === PaymentType.CreditCard) {
        const payload: PaymentInfoRequest = {
          type: PaymentType.CreditCard,
          cardHolder: payment.cardCreditInfo?.cardHolder || "",
          cardNumber: payment.cardCreditInfo?.cardNumber || "",
          cardType: payment.cardCreditInfo?.cardType || "Visa",
        }
        paymentPayload = payload;
      } else if (payment.paymentType === PaymentType.BankTransfer) {
        const payload: PaymentInfoRequest = {
          type: PaymentType.BankTransfer,
          accountNumber: payment.bankTransferInfo?.accountNumber || "",
          bankName: payment.bankTransferInfo?.bankName || "",
        }
        paymentPayload = payload;
      }

      if (!paymentPayload) {
        toast.error("Vui lòng chọn hình thức thanh toán hợp lệ.");
        return;
      }

      const orderPayload: CreateOrderRequest = {
        pointsUsed: discountAndNote.pointsUsed,
        discountCode: discountAndNote.discountCode || null,
        shippingInfo: { shippingAddressId: shippingAddressId, type: shipping.type },
        note: discountAndNote.note,
        paymentInfo: paymentPayload,
        items: productItems.map((item) => ({
          productId: item.productId,
          requiredQuantity: item.quantity,
        }))
      }

      const res = await orderRequests.createOrder(orderPayload);
      const createdOrder = res.payload?.data;

      if (payment.paymentMode === "ONLINE" && createdOrder) {
        if (payment.paymentMethodType === undefined || payment.paymentType === undefined) {
          toast.error("Vui lòng chọn cổng thanh toán online hợp lệ.");
          return;
        }

        const cardType = payment.cardCreditInfo?.cardType ?? payment.bankTransferInfo?.bankName ?? "";
        const query = new URLSearchParams({
          paymentMethodType: String(payment.paymentMethodType),
          payType: String(payment.paymentType),
          cardType,
          orderId: createdOrder.id,
          amount: String(createdOrder.grandAmount),
        });

        router.push(`/payment?${query.toString()}`);
      }
    } catch (error) {
      if (error instanceof HttpError) {
        const errorRes = error.payload as MyCustomResponse;
        const message = getErrorMessage(errorRes.errorCode);
        toast.error(message);
      } else {
        toast.error("Máy chủ đang bận. Vui lòng thử lại sau.");
      }
    }
  }

  const prepareOrderData = () => {
    const addressData = addressFormHook.getValues();
    const shippingData = shippingFormHook.getValues();
    const paymentData = paymentFormHook.getValues();
    const discountAndNoteData = discountAndNoteFormHook.getValues();
    const orderData = {
      address: addressData,
      shipping: shippingData,
      payment: paymentData,
      discountAndNote: discountAndNoteData,
    };
    return orderData;
  }

  return (
    <section className="py-32">
      <div className="container">
        <div className="flex flex-col gap-6 pb-8 md:flex-row md:items-center md:justify-between md:gap-8">
          <div className="flex flex-col gap-4">
            <div className="flex flex-col gap-2">
              <h1 className="text-4xl font-bold tracking-tight md:text-5xl">
                Đơn hàng
              </h1>
              {/* <p className="text-sm text-muted-foreground md:text-base">
                Complete your purchase securely
              </p> */}
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 gap-0 lg:grid-cols-2 lg:gap-17.5">

          <div>
            <Accordion
              type="multiple"
              // collapsible
              className="w-full"
              value={activeAccordion}
              onValueChange={handleOnValueChange}

            >

              {/* Địa chỉ giao hàng */}
              <AccordionItem value="item-2">
                <AccordionTrigger className="px-1 py-7 text-lg font-semibold hover:no-underline [&>svg:last-child]:hidden [&[data-state=closed]>svg:nth-of-type(2)]:hidden [&[data-state=open]>svg:nth-of-type(1)]:hidden [&[data-state=open]>svg:nth-of-type(2)]:block">
                  Địa chỉ giao hàng
                  <Plus className="pointer-events-none size-4 shrink-0 self-center text-muted-foreground" />
                  <Minus className="pointer-events-none hidden size-4 shrink-0 self-center text-muted-foreground" />
                </AccordionTrigger>
                <AccordionContent className="px-1 pb-7 h-auto">
                  <div className="space-y-7">
                    {/* button section */}
                    <section className="flex">
                      <UserAddressBookDialog onSelectAddress={handleSelectAddress} />
                    </section>
                    <AddressForm
                      formHook={addressFormHook}
                    />
                    <Button
                      className="w-full"
                      variant="outline"
                      onClick={() => onContinue("item-3")}
                      disabled={!isValidAddress}
                    >
                      Tiếp
                    </Button>
                  </div>
                </AccordionContent>
              </AccordionItem>

              {/* Phương thức giao hàng */}
              <AccordionItem value="item-3">
                <AccordionTrigger className="px-1 py-7 text-lg font-semibold hover:no-underline [&>svg:last-child]:hidden [&[data-state=closed]>svg:nth-of-type(2)]:hidden [&[data-state=open]>svg:nth-of-type(1)]:hidden [&[data-state=open]>svg:nth-of-type(2)]:block">
                  Phương thức giao hàng
                  <Plus className="pointer-events-none size-4 shrink-0 self-center text-muted-foreground" />
                  <Minus className="pointer-events-none hidden size-4 shrink-0 self-center text-muted-foreground" />
                </AccordionTrigger>
                <AccordionContent className="px-1 pb-7 h-auto">
                  <div className="space-y-7">
                    {/* <ShippingMethodFields /> */}
                    <ShippingMethodSection formHook={shippingFormHook} />
                    <Button
                      type="button"
                      className="w-full"
                      variant="outline"
                      onClick={() => onContinue("item-4")}
                      disabled={!isValidShipping}
                    >
                      Tiếp
                    </Button>
                  </div>
                </AccordionContent>
              </AccordionItem>

              <AccordionItem value="item-4">
                <AccordionTrigger className="px-1 py-7 text-lg font-semibold hover:no-underline [&>svg:last-child]:hidden [&[data-state=closed]>svg:nth-of-type(2)]:hidden [&[data-state=open]>svg:nth-of-type(1)]:hidden [&[data-state=open]>svg:nth-of-type(2)]:block">
                  Mã giảm giá và ghi chú đơn hàng
                  <Plus className="pointer-events-none size-4 shrink-0 self-center text-muted-foreground" />
                  <Minus className="pointer-events-none hidden size-4 shrink-0 self-center text-muted-foreground" />
                </AccordionTrigger>
                <AccordionContent className="px-1 pb-7 h-auto">
                  <div className="space-y-7">
                    {/* <ShippingMethodFields /> */}
                    <DiscountAndNoteSection formHook={discountAndNoteFormHook} onApplyDiscount={handleApplyDiscount} />
                    <Button
                      type="button"
                      className="w-full"
                      variant="outline"
                      onClick={() => onContinue("item-5")}
                    // disabled={isDisabledContinueToPayment}
                    >
                      Tiếp
                    </Button>
                  </div>
                </AccordionContent>
              </AccordionItem>

              {/* Thanh toán */}
              <AccordionItem value="item-5">
                <AccordionTrigger className="px-1 py-7 text-lg font-semibold hover:no-underline [&>svg:last-child]:hidden [&[data-state=closed]>svg:nth-of-type(2)]:hidden [&[data-state=open]>svg:nth-of-type(1)]:hidden [&[data-state=open]>svg:nth-of-type(2)]:block">
                  Thanh toán
                  <Plus className="pointer-events-none size-4 shrink-0 self-center text-muted-foreground" />
                  <Minus className="pointer-events-none hidden size-4 shrink-0 self-center text-muted-foreground" />
                </AccordionTrigger>
                <AccordionContent className="px-1 pb-7 h-auto">
                  <div className="space-y-7">
                    <PaymentSection formHook={paymentFormHook} />
                    <Button
                      // type="submit"
                      className="w-full"
                      disabled={!isValidOrder}
                      onClick={() => {
                        if (isValidOrder) {
                          const orderData = prepareOrderData();
                          handleSubmitOrder(orderData)
                          return;
                        }
                        // trigger validation for payment form
                        paymentFormHook.trigger();
                      }}>
                      Thanh toán
                    </Button>
                    {!isValidOrder && (
                      // Không valid thì hiển thị lỗi chung, không đi sâu vào từng lỗi cụ
                      (<Alert variant="destructive">
                        <AlertCircleIcon />
                        <AlertTitle>Thông tin đơn hàng chưa đầy đủ</AlertTitle>
                        <AlertDescription>
                          {!isValidAddress && (
                            <span className="block">Vui lòng kiểm tra lại thông tin địa chỉ giao hàng.</span>
                          )}
                          {!isValidShipping && (
                            <span className="block">Vui lòng kiểm tra lại thông tin phương thức giao hàng.</span>
                          )}
                          {!isValidPayment && (
                            <span className="block">Vui lòng kiểm tra lại thông tin thanh toán.</span>
                          )}
                        </AlertDescription>
                      </Alert>)
                    )}
                  </div>
                </AccordionContent>
              </AccordionItem>

            </Accordion>
          </div>
          <div>

            <div className="sticky top-4">
              {/* Sản phẩm */}
              <div className="border-b py-7 mb-5">
                <h2 className="text-lg leading-relaxed font-semibold">Sản phẩm</h2>
              </div>
              {productItems.length > 0 ? (productItems.map((item) => (
                <CartItemCard
                  className="mb-2"
                  key={item.productId}
                  item={item}
                  showCheckbox={false}
                  isSelected={false}
                  disableDecrease={item.quantity <= 1}
                  disableRemove={productItems.length <= 1}
                  onDecrease={() => { updateItemQuantity(item.productId, item.quantity - 1) }}
                  onIncrease={() => { updateItemQuantity(item.productId, item.quantity + 1) }}
                  onRemove={() => { removeItem(item.productId) }}
                />
              ))) : (
                <p>Giỏ hàng trống</p>
              )}


              {/* Tóm tắt đơn hàng */}
              <SummaryOrderSection
                subTotal={subTotal}
                shippingFee={shippingFee}
                orderTotal={orderTotal}
                pointsUsed={pointsUsed}
                applyDiscountResponse={applyDiscountResponse}
              />

            </div>
          </div>
        </div>

      </div>
    </section>
  );
}

