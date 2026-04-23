
import { Button } from "@/components/ui/button";
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList } from "@/components/ui/combobox"
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input"
import { AddressFormData } from "@/entities/address/type"
import { getDistricts, getProvinces, getWards } from "@/features/address-action";
import { cn } from "@/lib/utils";
import { District, Province, Ward } from "@/services/address-api";
import { MapPin, Save, X } from "lucide-react"
import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";

export default function AddressForm({
  className,
  showSaveButton = false,
  showCloseButton = false,
  onSaveClick,
  onCloseClick,
  title = "Thêm địa chỉ mới",
  formHook,
}: {
  className?: string,
  formHook: ReturnType<typeof useForm<AddressFormData>>,
  showSaveButton?: boolean,
  showCloseButton?: boolean,
  onSaveClick?: (data: AddressFormData) => void,
  onCloseClick?: () => void,
  title?: string
}) {
  const { register, watch, setValue, formState: { errors, isDirty, isSubmitting, isValid } } = formHook
  const [provinces, setProvinces] = useState<Province[]>([]);
  const [districts, setDistricts] = useState<District[]>([]);
  const [wards, setWards] = useState<Ward[]>([]);

  const provinceCode = watch("provinceCode");
  const districtCode = watch("districtCode");
  const wardCode = watch("wardCode");

  const disableSaveButton = !isDirty || isSubmitting || !isValid

  // load danh sách tỉnh/thành phố khi component
  useEffect(() => {
    const loadProvinces = async () => {
      const data = await getProvinces();
      setProvinces(data);
    };

    loadProvinces();
  }, []);

  // load danh sách quận/huyện khi người dùng chọn tỉnh/thành phố
  useEffect(() => {
    if (!provinceCode) {
      return;
    }

    const loadDistricts = async () => {
      const districts = await getDistricts(Number(provinceCode));
      setDistricts(districts || []);
      setWards([]);

      // setValue("district", "");
      // setValue("ward", "");
      // setValue("districtCode", "");
      // setValue("wardCode", "");
    };

    loadDistricts();
  }, [provinceCode, setValue]);

  // load danh sách phường/xã khi người dùng chọn quận/huyện
  useEffect(() => {
    if (!districtCode) {
      return;
    }

    const loadWards = async () => {
      const wards = await getWards(Number(districtCode));
      setWards(wards || []);

      // setValue("ward", "");
      // setValue("wardCode", "");
    };

    loadWards();
  }, [districtCode, setValue]);

  const provinceMap = useMemo(() => {
    return new Map(provinces.map((province) => [String(province.code), province.name]));
  }, [provinces]);

  const districtMap = useMemo(() => {
    return new Map(districts.map((district) => [String(district.code), district.name]));
  }, [districts]);

  const wardMap = useMemo(() => {
    return new Map(wards.map((ward) => [String(ward.code), ward.name]));
  }, [wards]);

  const selectedProvince = useMemo(() => {
    return provinces.find((province) => String(province.code) === provinceCode) ?? null;
  }, [provinceCode, provinces]);

  const selectedDistrict = useMemo(() => {
    return districts.find((district) => String(district.code) === districtCode) ?? null;
  }, [districtCode, districts]);

  const selectedWard = useMemo(() => {
    return wards.find((ward) => String(ward.code) === wardCode) ?? null;
  }, [wardCode, wards]);

  useEffect(() => { console.log(">>>", provinceCode, districtCode, wardCode) }, [provinceCode, districtCode, wardCode]);

  return (
    <div className={cn("rounded-2xl border border-slate-200 bg-slate-50/70 p-4 sm:p-5", className)}>
      <div className="flex items-center">
        <p className="mb-4 text-sm font-semibold text-slate-800">{title}</p>

        {(showSaveButton || showCloseButton) && (
          <div className="ml-auto flex gap-2">
            {showCloseButton && (
              <Button onClick={onCloseClick} variant="ghost" size="sm">
                <X className="size-4" />
              </Button>
            )}
            {showSaveButton && (
              <Button disabled={disableSaveButton} size="sm" onClick={() => {
                if (typeof onSaveClick === "function") {
                  formHook.handleSubmit(onSaveClick)()
                }
              }}>
                <Save className="mr-2 size-4" /> Lưu
              </Button>
            )}
          </div>
        )}
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="receiverName" className="text-slate-700">Người nhận</FieldLabel>
          <Input
            id="receiverName"
            {...register("receiverName")}
            className="h-11 rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
          />
          <FieldError>{errors.receiverName?.message}</FieldError>
        </Field>

        <Field>
          <FieldLabel htmlFor="phoneNumber" className="text-slate-700">SĐT nhận hàng</FieldLabel>
          <Input
            id="phoneNumber"
            {...register("phoneNumber")}
            className="h-11 rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
          />
          <FieldError>{errors.phoneNumber?.message}</FieldError>
        </Field>

        <Field>
          <FieldLabel htmlFor="country" className="text-slate-700">Quốc gia</FieldLabel>
          <Input
            id="country"
            {...register("country")}
            className="h-11 rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
          />
          <FieldError>{errors.country?.message}</FieldError>
        </Field>

        <Field>
          <FieldLabel htmlFor="street" className="text-slate-700">Số nhà, tên đường</FieldLabel>
          <div className="relative">
            <MapPin className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
            <Input
              id="street"
              {...register("street")}
              className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-3 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
            />
          </div>
          <FieldError>{errors.street?.message}</FieldError>
        </Field>
      </div>

      {/* phần chọn tỉnh/thành, quận/huyện, phường/xã */}
      <div className="mt-4 grid gap-4 sm:grid-cols-3">
        <Field>
          <FieldLabel htmlFor="provinceCode" className="text-slate-700">Tỉnh/Thành phố</FieldLabel>
          <Combobox
            items={provinces}
            value={selectedProvince ?? null}
            itemToStringLabel={(item) => item.name}
            itemToStringValue={(item) => String(item.code)}
            onValueChange={(value) => {
              if (!value) {
                setValue("provinceCode", "", { shouldDirty: true, shouldValidate: true });
                setValue("province", "", { shouldDirty: true, shouldValidate: true });
                setValue("district", "", { shouldDirty: true, shouldValidate: true });
                setValue("ward", "", { shouldDirty: true, shouldValidate: true });
                setValue("districtCode", "", { shouldDirty: true, shouldValidate: true });
                setValue("wardCode", "", { shouldDirty: true, shouldValidate: true });
                return;
              }

              const code = String(value.code);
              setValue("provinceCode", code, { shouldDirty: true, shouldValidate: true });
              setValue("province", provinceMap.get(code) ?? value.name, {
                shouldDirty: true,
                shouldValidate: true,
              });
            }}
          >
            <ComboboxInput
              id="provinceCode"
              placeholder="Chọn tỉnh/thành"
              showClear
              aria-invalid={Boolean(errors.provinceCode || errors.province)}
              className="h-11 w-full rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
            />
            <ComboboxContent>
              <ComboboxEmpty>Không tìm thấy tỉnh/thành</ComboboxEmpty>
              <ComboboxList>
                {(province) => (
                  <ComboboxItem key={province.code} value={province}>
                    {province.name}
                  </ComboboxItem>
                )}
              </ComboboxList>
            </ComboboxContent>
          </Combobox>
          <input type="hidden" {...register("provinceCode")} />
          <input type="hidden" {...register("province")} />
          <FieldError>{errors.province?.message}</FieldError>
        </Field>

        <Field>
          <FieldLabel htmlFor="districtCode" className="text-slate-700">Quận/Huyện</FieldLabel>
          <Combobox
            items={districts}
            value={selectedDistrict ?? null}
            itemToStringLabel={(item) => item.name}
            itemToStringValue={(item) => String(item.code)}
            onValueChange={(value) => {
              if (!value) {
                setValue("districtCode", "", { shouldDirty: true, shouldValidate: true });
                setValue("district", "", { shouldDirty: true, shouldValidate: true });
                setValue("ward", "", { shouldDirty: true, shouldValidate: true });
                setValue("wardCode", "", { shouldDirty: true, shouldValidate: true });
                return;
              }

              const code = String(value.code);
              setValue("districtCode", code, { shouldDirty: true, shouldValidate: true });
              setValue("district", districtMap.get(code) ?? value.name, {
                shouldDirty: true,
                shouldValidate: true,
              });
            }}
          >
            <ComboboxInput
              id="districtCode"
              placeholder="Chọn quận/huyện"
              showClear
              disabled={!provinceCode}
              aria-invalid={Boolean(errors.districtCode || errors.district)}
              className="h-11 w-full rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200 disabled:cursor-not-allowed disabled:bg-slate-100"
            />
            <ComboboxContent>
              <ComboboxEmpty>Không tìm thấy quận/huyện</ComboboxEmpty>
              <ComboboxList>
                {(district) => (
                  <ComboboxItem key={district.code} value={district}>
                    {district.name}
                  </ComboboxItem>
                )}
              </ComboboxList>
            </ComboboxContent>
          </Combobox>
          <input type="hidden" {...register("districtCode")} />
          <input type="hidden" {...register("district")} />
          <FieldError>{errors.district?.message}</FieldError>
        </Field>

        <Field>
          <FieldLabel htmlFor="wardCode" className="text-slate-700">Phường/Xã</FieldLabel>
          <Combobox
            items={wards}
            value={selectedWard ?? null}
            itemToStringLabel={(item) => item.name}
            itemToStringValue={(item) => String(item.code)}
            onValueChange={(value) => {
              if (!value) {
                setValue("wardCode", "", { shouldDirty: true, shouldValidate: true });
                setValue("ward", "", { shouldDirty: true, shouldValidate: true });
                return;
              }

              const code = String(value.code);
              setValue("wardCode", code, { shouldDirty: true, shouldValidate: true });
              setValue("ward", wardMap.get(code) ?? value.name, {
                shouldDirty: true,
                shouldValidate: true,
              });
            }}
          >
            <ComboboxInput
              id="wardCode"
              placeholder="Chọn phường/xã"
              showClear
              disabled={!districtCode}
              aria-invalid={Boolean(errors.wardCode || errors.ward)}
              className="h-11 w-full rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200 disabled:cursor-not-allowed disabled:bg-slate-100"
            />
            <ComboboxContent>
              <ComboboxEmpty>Không tìm thấy phường/xã</ComboboxEmpty>
              <ComboboxList>
                {(ward) => (
                  <ComboboxItem key={ward.code} value={ward}>
                    {ward.name}
                  </ComboboxItem>
                )}
              </ComboboxList>
            </ComboboxContent>
          </Combobox>
          <input type="hidden" {...register("wardCode")} />
          <input type="hidden" {...register("ward")} />
          <FieldError>{errors.ward?.message}</FieldError>
        </Field>

      </div>
    </div>
  )
}
