const priceFormatter = new Intl.NumberFormat("vi-VN");

export function formatCurrency(value: number): string {
  return `${priceFormatter.format(value)}đ`;
}

export function formatDatetime(value?: string | null): string | null {
  if (!value) return null;
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return null;
  return date.toLocaleString("vi-VN");
}

export function formatAddress(address: {
  street?: string | null;
  ward?: string | null;
  district?: string | null;
  province?: string | null;
  country?: string | null;
}): string {
  return [
    address.street,
    address.ward,
    address.district,
    address.province,
    address.country,
  ]
    .filter(Boolean)
    .join(", ");
}