"use server";

import addressApi from "@/services/address-api";

export async function getProvinces() {
  const result = await addressApi.listProvinces();
  return result;
}

export async function getDistricts(provinceCode: number) {
  const result = await addressApi.getProvince(provinceCode, 2);
  return result.districts ?? [];
}

export async function getWards(districtCode: number) {
  const result = await addressApi.getDistrict(districtCode, 2);
  return result.wards ?? [];
}