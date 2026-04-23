import {
  Field,
  FieldContent,
  FieldDescription,
  FieldError,
  FieldGroup,
  FieldLabel,
  FieldLegend,
  FieldSet,
  FieldTitle,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import {
  PaymentMethodType,
  PaymentType,
  type CreditType,
} from "@/services/client-requests/order-requests";
import Image from "next/image";
import { useForm } from "react-hook-form";
import { z } from "zod";

const paymentModes = ["COD", "ONLINE"] as const;

const creditCardTypes: CreditType[] = ["Visa", "MasterCard", "JCB", "AMEX"]

export const paymentSchema = z.object({
  paymentMode: z.enum(paymentModes, {
    message: "Vui lòng chọn phương thức thanh toán",
  }),
  paymentMethodType: z.enum(PaymentMethodType).optional(),
  paymentType: z.enum(PaymentType).optional(),
  bankTransferInfo: z
    .object({
      bankName: z.string().min(1, "Vui lòng nhập tên ngân hàng"),
      accountNumber: z.string().min(1, "Vui lòng nhập số tài khoản"),
    })
    .optional(),
  cardCreditInfo: z
    .object({
      cardNumber: z.string().min(12, "Vui lòng nhập số thẻ"),
      cardHolder: z.string().min(1, "Vui lòng nhập tên chủ thẻ"),
      cardType: z.enum(creditCardTypes, {
        message: "Vui lòng chọn loại thẻ",
      }),
    })
    .optional(),
}).superRefine((data, ctx) => {
  // if (data.paymentMode === "COD") {
  //   if (data.paymentType && data.paymentType !== PaymentType.OCD) {
  //     ctx.addIssue({
  //       code: z.ZodIssueCode.custom,
  //       message: "Thanh toán khi nhận hàng không yêu cầu lựa chọn khác",
  //       path: ["paymentType"],
  //     })
  //   }
  //   return
  // }

  if (data.paymentMethodType === undefined) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Vui lòng chọn cổng thanh toán online",
      path: ["paymentMethodType"],
    })
  }

  if (data.paymentType === undefined) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Vui lòng chọn hình thức thanh toán online",
      path: ["paymentType"],
    })
  }

  if (data.paymentType === PaymentType.BankTransfer) {
    if (!data.bankTransferInfo?.bankName) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Vui lòng nhập tên ngân hàng",
        path: ["bankTransferInfo", "bankName"],
      })
    }

    if (!data.bankTransferInfo?.accountNumber) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Vui lòng nhập số tài khoản",
        path: ["bankTransferInfo", "accountNumber"],
      })
    }
  }

  if (data.paymentType === PaymentType.CreditCard) {
    if (!data.cardCreditInfo?.cardNumber) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Vui lòng nhập số thẻ",
        path: ["cardCreditInfo", "cardNumber"],
      })
    }

    if (!data.cardCreditInfo?.cardHolder) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Vui lòng nhập tên chủ thẻ",
        path: ["cardCreditInfo", "cardHolder"],
      })
    }

    if (!data.cardCreditInfo?.cardType) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Vui lòng chọn loại thẻ",
        path: ["cardCreditInfo", "cardType"],
      })
    }
  }
})

export type PaymentFormData = z.infer<typeof paymentSchema>
type PaymentMode = (typeof paymentModes)[number]

type Props = {
  formHook: ReturnType<typeof useForm<PaymentFormData>>
}


export default function PaymentSection({ formHook }: Props) {
  const {
    register,
    setValue,
    formState: { errors },
  } = formHook
  const paymentMode = formHook.watch("paymentMode") ?? ""
  const paymentMethodType = formHook.watch("paymentMethodType")
  const paymentType = formHook.watch("paymentType")
  const isOnline = paymentMode === "ONLINE"

  const handlePaymentModeChange = (value: string) => {
    const mode = value as PaymentMode

    setValue("paymentMode", mode, {
      shouldDirty: true,
      shouldTouch: true,
      shouldValidate: true,
    })

    if (mode === "COD") {
      setValue("paymentType", PaymentType.Cash, {
        shouldDirty: true,
        shouldTouch: true,
        shouldValidate: true,
      })
      setValue("paymentMethodType", undefined, {
        shouldDirty: true,
        shouldTouch: true,
        shouldValidate: true,
      })
      setValue("bankTransferInfo", undefined, {
        shouldDirty: true,
        shouldTouch: true,
        shouldValidate: true,
      })
      setValue("cardCreditInfo", undefined, {
        shouldDirty: true,
        shouldTouch: true,
        shouldValidate: true,
      })
      return
    }

    setValue("paymentType", undefined, {
      shouldDirty: true,
      shouldTouch: true,
      shouldValidate: true,
    })
    setValue("bankTransferInfo", undefined, {
      shouldDirty: true,
      shouldTouch: true,
      shouldValidate: true,
    })
    setValue("cardCreditInfo", undefined, {
      shouldDirty: true,
      shouldTouch: true,
      shouldValidate: true,
    })
  }

  const handleOnlineMethodChange = (value: string) => {
    const methodType = Number(value) as PaymentMethodType

    setValue("paymentMethodType", methodType, {
      shouldDirty: true,
      shouldTouch: true,
      shouldValidate: true,
    })
  }

  const handlePaymentTypeChange = (value: string) => {
    const selectedType = Number(value) as PaymentType

    setValue("paymentType", selectedType, {
      shouldDirty: true,
      shouldTouch: true,
      shouldValidate: true,
    })

    if (selectedType === PaymentType.BankTransfer) {
      setValue("cardCreditInfo", undefined, {
        shouldDirty: true,
        shouldTouch: true,
        shouldValidate: true,
      })
      return
    }

    if (selectedType === PaymentType.CreditCard) {
      setValue("bankTransferInfo", undefined, {
        shouldDirty: true,
        shouldTouch: true,
        shouldValidate: true,
      })
    }
  }

  return (
    <FieldGroup className="gap-6">
      <FieldSet>
        <FieldLegend>Hình thức thanh toán</FieldLegend>
        <Field data-invalid={!!errors.paymentMode}>
          <RadioGroup
            {...register("paymentMode")}
            value={paymentMode}
            onValueChange={handlePaymentModeChange}
            className="flex max-sm:flex-col"
          >
            <FieldLabel htmlFor="payment-mode-cod">
              <Field orientation="horizontal" data-invalid={!!errors.paymentMode}>
                <FieldContent>
                  <FieldTitle>Thanh toán khi nhận hàng</FieldTitle>
                  <FieldDescription>Thanh toán tiền mặt khi nhận hàng.</FieldDescription>
                </FieldContent>
                <RadioGroupItem
                  value="COD"
                  id="payment-mode-cod"
                  aria-invalid={!!errors.paymentMode}
                />
              </Field>
            </FieldLabel>
            <FieldLabel htmlFor="payment-mode-online">
              <Field orientation="horizontal" data-invalid={!!errors.paymentMode}>
                <FieldContent>
                  <FieldTitle>Thanh toán online</FieldTitle>
                  <FieldDescription>Chọn cổng thanh toán phù hợp.</FieldDescription>
                </FieldContent>
                <RadioGroupItem
                  value="ONLINE"
                  id="payment-mode-online"
                  aria-invalid={!!errors.paymentMode}
                />
              </Field>
            </FieldLabel>
          </RadioGroup>
          {errors.paymentMode && <FieldError errors={[errors.paymentMode]} />}
        </Field>
      </FieldSet>

      {isOnline && (
        <>
          <FieldSet>
            <FieldLegend>Cổng thanh toán online</FieldLegend>
            <Field data-invalid={!!errors.paymentMethodType}>
              <RadioGroup
                {...register("paymentMethodType")}
                value={paymentMethodType?.toString() ?? ""}
                onValueChange={handleOnlineMethodChange}
                className="flex max-sm:flex-col"
              >
                <FieldLabel htmlFor="payment-online-momo">
                  <Field orientation="horizontal" data-invalid={!!errors.paymentMethodType}>
                    <FieldContent>
                      <FieldTitle>
                        <Image src="/logo-momo.svg.svg" alt="Momo Logo" width={32} height={32} />
                      </FieldTitle>
                      <FieldDescription>Thanh toán nhanh qua ví Momo.</FieldDescription>
                    </FieldContent>
                    <RadioGroupItem
                      value={PaymentMethodType.Momo.toString()}
                      id="payment-online-momo"
                      aria-invalid={!!errors.paymentMethodType}
                    />
                  </Field>
                </FieldLabel>
                <FieldLabel htmlFor="payment-online-vnpay">
                  <Field orientation="horizontal" data-invalid={!!errors.paymentMethodType}>
                    <FieldContent>
                      <FieldTitle>
                        <Image src="/vnpay-logo-inkythuatso.svg" alt="VNPay Logo" width={32} height={32} />
                      </FieldTitle>
                      <FieldDescription>Thanh toán an toàn qua VNPay.</FieldDescription>
                    </FieldContent>
                    <RadioGroupItem
                      value={PaymentMethodType.VnPay.toString()}
                      id="payment-online-vnpay"
                      aria-invalid={!!errors.paymentMethodType}
                    />
                  </Field>
                </FieldLabel>
              </RadioGroup>
              {errors.paymentMethodType && <FieldError errors={[errors.paymentMethodType]} />}
            </Field>
          </FieldSet>

          <FieldSet>
            <FieldLegend>Hình thức thanh toán online</FieldLegend>
            <Field data-invalid={!!errors.paymentType}>
              <RadioGroup
                {...register("paymentType")}
                value={paymentType?.toString() ?? ""}
                onValueChange={handlePaymentTypeChange}
                className="flex max-sm:flex-col"
              >
                <FieldLabel htmlFor="payment-type-bank">
                  <Field orientation="horizontal" data-invalid={!!errors.paymentType}>
                    <FieldContent>
                      <FieldTitle>Chuyển khoản ngân hàng</FieldTitle>
                      <FieldDescription>Nhập thông tin tài khoản ngân hàng.</FieldDescription>
                    </FieldContent>
                    <RadioGroupItem
                      value={PaymentType.BankTransfer.toString()}
                      id="payment-type-bank"
                      aria-invalid={!!errors.paymentType}
                    />
                  </Field>
                </FieldLabel>
                <FieldLabel htmlFor="payment-type-card">
                  <Field orientation="horizontal" data-invalid={!!errors.paymentType}>
                    <FieldContent>
                      <FieldTitle>Thẻ tín dụng</FieldTitle>
                      <span className="flex gap-1">
                        <Image src="/visa.svg" alt="Visa Logo" width={24} height={24} />
                        <Image src="/master-card.svg" alt="MasterCard Logo" width={24} height={24} />
                        <Image src="/jcb.svg" alt="JCB Logo" width={24} height={24} />
                        <Image src="/amex.svg" alt="AMEX Logo" width={24} height={24} />
                      </span>
                      <FieldDescription>Visa, MasterCard, JCB, AMEX.</FieldDescription>
                    </FieldContent>
                    <RadioGroupItem
                      value={PaymentType.CreditCard.toString()}
                      id="payment-type-card"
                      aria-invalid={!!errors.paymentType}
                    />
                  </Field>
                </FieldLabel>
              </RadioGroup>
              {errors.paymentType && <FieldError errors={[errors.paymentType]} />}
            </Field>
          </FieldSet>

          {paymentType === PaymentType.BankTransfer && (
            <FieldSet>
              <FieldLegend>Thông tin chuyển khoản</FieldLegend>
              <FieldGroup className="gap-4">
                <Field data-invalid={!!errors.bankTransferInfo?.bankName}>
                  <FieldLabel htmlFor="bank-transfer-name">Tên ngân hàng</FieldLabel>
                  <Input
                    id="bank-transfer-name"
                    aria-invalid={!!errors.bankTransferInfo?.bankName}
                    {...register("bankTransferInfo.bankName")}
                  />
                  {errors.bankTransferInfo?.bankName && (
                    <FieldError errors={[errors.bankTransferInfo.bankName]} />
                  )}
                </Field>
                <Field data-invalid={!!errors.bankTransferInfo?.accountNumber}>
                  <FieldLabel htmlFor="bank-transfer-account">Số tài khoản</FieldLabel>
                  <Input
                    id="bank-transfer-account"
                    aria-invalid={!!errors.bankTransferInfo?.accountNumber}
                    {...register("bankTransferInfo.accountNumber")}
                  />
                  {errors.bankTransferInfo?.accountNumber && (
                    <FieldError errors={[errors.bankTransferInfo.accountNumber]} />
                  )}
                </Field>
              </FieldGroup>
            </FieldSet>
          )}

          {paymentType === PaymentType.CreditCard && (
            <FieldSet>
              <FieldLegend>Thông tin thẻ</FieldLegend>
              <FieldGroup className="gap-4">
                <Field data-invalid={!!errors.cardCreditInfo?.cardNumber}>
                  <FieldLabel htmlFor="credit-card-number">Số thẻ</FieldLabel>
                  <Input
                    id="credit-card-number"
                    aria-invalid={!!errors.cardCreditInfo?.cardNumber}
                    {...register("cardCreditInfo.cardNumber")}
                  />
                  {errors.cardCreditInfo?.cardNumber && (
                    <FieldError errors={[errors.cardCreditInfo.cardNumber]} />
                  )}
                </Field>
                <Field data-invalid={!!errors.cardCreditInfo?.cardHolder}>
                  <FieldLabel htmlFor="credit-card-holder">Tên chủ thẻ</FieldLabel>
                  <Input
                    id="credit-card-holder"
                    aria-invalid={!!errors.cardCreditInfo?.cardHolder}
                    {...register("cardCreditInfo.cardHolder")}
                  />
                  {errors.cardCreditInfo?.cardHolder && (
                    <FieldError errors={[errors.cardCreditInfo.cardHolder]} />
                  )}
                </Field>
              </FieldGroup>
              <Field data-invalid={!!errors.cardCreditInfo?.cardType}>
                <FieldLabel>Loại thẻ</FieldLabel>
                <RadioGroup
                  value={formHook.watch("cardCreditInfo.cardType") ?? ""}
                  onValueChange={(value) => {
                    setValue("cardCreditInfo.cardType", value as CreditType, {
                      shouldDirty: true,
                      shouldTouch: true,
                      shouldValidate: true,
                    })
                  }}
                  className="flex max-sm:flex-col"
                >
                  {creditCardTypes.map((type) => {
                    const cardTypeId = `credit-card-${type.toLowerCase()}`

                    return (
                      <FieldLabel key={type} htmlFor={cardTypeId}>
                        <Field orientation="horizontal" data-invalid={!!errors.cardCreditInfo?.cardType}>
                          <FieldContent>
                            <FieldTitle>{type}</FieldTitle>
                          </FieldContent>
                          <RadioGroupItem
                            value={type}
                            id={cardTypeId}
                            aria-invalid={!!errors.cardCreditInfo?.cardType}
                          />
                        </Field>
                      </FieldLabel>
                    )
                  })}
                </RadioGroup>
                {errors.cardCreditInfo?.cardType && (
                  <FieldError errors={[errors.cardCreditInfo.cardType]} />
                )}
              </Field>
            </FieldSet>
          )}
        </>
      )}
    </FieldGroup>
  )
}
