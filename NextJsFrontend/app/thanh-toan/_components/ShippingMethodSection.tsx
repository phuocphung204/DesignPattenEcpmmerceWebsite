import { Field, FieldContent, FieldDescription, FieldError, FieldLabel, FieldTitle } from "@/components/ui/field";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { useForm } from "react-hook-form";
import { ShippingFormData } from "./CheckoutClient";
import { ShippingType } from "@/services/client-requests/order-requests";
import { formatCurrency } from "@/utils/formatter";


type Props = {
  formHook: ReturnType<typeof useForm<ShippingFormData>>;
}

export const shippingFeeMap: Record<ShippingType, number> = {
  [ShippingType.Standard]: 10000,
  [ShippingType.Express]: 20000,
}

export default function ShippingMethodSection({ formHook }: Props) {
  const { register, setValue, formState: { errors } } = formHook;
  const selectedShippingType = formHook.watch("type")?.toString() || "";

  return (
    <Field>
      <RadioGroup
        {...register("type")}
        value={selectedShippingType}
        onValueChange={(value) => {
          const shippingType = Number(value) as ShippingType;
          setValue("type", shippingType, { shouldDirty: true, shouldTouch: true, shouldValidate: true });
        }}
        className="flex max-sm:flex-col"
      >
        <FieldLabel htmlFor="checkout-shippingMethod-1">
          <Field orientation="horizontal" data-invalid={!!errors.type}>
            <FieldContent>
              <FieldTitle>Tiêu chuẩn</FieldTitle>
              <FieldDescription>Thời gian: 3 - 5 ngày</FieldDescription>
            </FieldContent>
            <div className="flex gap-3.5">
              <p className="text-sm">{formatCurrency(shippingFeeMap[ShippingType.Standard])}</p>
              <RadioGroupItem
                value={ShippingType.Standard.toString()}
                id="checkout-shippingMethod-1"
                aria-invalid={!!errors.type}
              />
            </div>
          </Field>
        </FieldLabel>
        <FieldLabel htmlFor="checkout-shippingMethod-2">
          <Field orientation="horizontal" data-invalid={!!errors.type}>
            <FieldContent>
              <FieldTitle>Express</FieldTitle>
              <FieldDescription>Thời gian: 1 - 2 ngày</FieldDescription>
            </FieldContent>
            <div className="flex gap-3.5">
              <p className="text-sm">{formatCurrency(shippingFeeMap[ShippingType.Express])}</p>
              <RadioGroupItem
                value={ShippingType.Express.toString()}
                id="checkout-shippingMethod-2"
                aria-invalid={!!errors.type}
              />
            </div>
          </Field>
        </FieldLabel>
      </RadioGroup>
      {errors.type && <FieldError errors={[errors.type]} />}
    </Field>
  )
}