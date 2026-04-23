import { AspectRatio } from "@/components/ui/aspect-ratio"
import { Id } from "@/types/common-type"
import { formatCurrency } from "@/utils/formatter"
import Image from "next/image"
import Link from "next/link"

type Props = {
  id: Id
  name: string
  image: string
  sellingPrice: number
}
export default function ProductVariant({ id, name, image, sellingPrice }: Props) {
  const productSlug = `${name.toLowerCase().replace(/\s+/g, '-')}`;
  const href = `/${productSlug}?id=${id}`;
  return (
    <Link href={href} className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white p-1 shadow-sm transition hover:border-slate-300 hover:shadow-md cursor-pointer">
      <div className="flex h-10 w-10 items-center justify-center overflow-hidden rounded-lg bg-slate-50">
        <AspectRatio ratio={1}>
          <Image
            src={image}
            alt={name}
            width={40}
            height={40}
            className="h-full w-full object-cover"
          />
        </AspectRatio>
      </div>
      <div className="min-w-0">
        <h3 className="truncate text-sm font-semibold text-slate-900">{name}</h3>
        <p className="text-xs font-semibold text-slate-700">
          {formatCurrency(sellingPrice)}
        </p>
      </div>
    </Link>
  )
}