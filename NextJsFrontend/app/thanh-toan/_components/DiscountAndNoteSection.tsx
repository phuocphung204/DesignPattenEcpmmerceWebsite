"use client"
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Field, FieldDescription, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { useForm } from "react-hook-form";
import { useEffect } from "react";

export const discountAndNoteSchema = z.object({
  loyaltyPoints: z.number().int(),
  discountCode: z.string().optional(),
  pointsUsed: z.number("Vui lòng nhập số điểm sử dụng hợp lệ"),
  note: z.string().optional(),
}).superRefine((data, ctx) => {
  // console.log(">>> validate discountAndNoteSchema with data: ", data);
  if (data.pointsUsed !== undefined && !Number.isNaN(data.pointsUsed) && data.pointsUsed > data.loyaltyPoints) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      path: ["pointsUsed"],
      message: "Số điểm sử dụng không được vượt quá số điểm hiện có",
    })
  }
});

export type DiscountAndNoteFormData = z.infer<typeof discountAndNoteSchema>;

export default function DiscountAndNoteSection({
  formHook,
  onApplyDiscount,
}: {
  formHook: ReturnType<typeof useForm<DiscountAndNoteFormData>>
  onApplyDiscount: () => void;
}) {
  const { register, formState: { errors } } = formHook;
  const loyaltyPoints = formHook.watch("loyaltyPoints");
  // useEffect(() => { console.log(">> errors", errors) }, [errors])
  return (
    <>
      <Field>
        <div className="flex gap-2 flex-row items-center">
          <Input placeholder="Nhập mã giảm giá nếu có"
            {...register("discountCode")}
          />
          <Button
            size="sm"
            onClick={onApplyDiscount}
          >
            Áp dụng
          </Button>
        </div>
        <FieldError>{errors.discountCode?.message}</FieldError>
      </Field>
      <Field>
        <FieldLabel htmlFor="giftCode">Điểm quy đổi</FieldLabel>
        <FieldDescription className="mb-0">Bạn có {loyaltyPoints ?? 0} điểm. Quy đổi 1 điểm = 1.000đ</FieldDescription>
        <Input placeholder="0"
          {...register("pointsUsed", { valueAsNumber: true })}
        />
        <FieldError>{errors.pointsUsed?.message}</FieldError>
      </Field>
      <Field>
        <FieldLabel htmlFor="note">Ghi chú đơn hàng</FieldLabel>
        <Textarea id="note" placeholder="Nhập ghi chú cho đơn hàng (ví dụ: yêu cầu giao hàng vào buổi chiều)" {...register("note")} />
      </Field>
    </>
  )
}