import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { AddressType } from "../type";

export default function AddressItem({
  className,
  address,
  isDefault,
  actionButtons = null,
}: {
  className?: string;
  address: AddressType,
  isDefault: boolean,
  actionButtons?: React.ReactNode,
}) {
  return (
    <Card
      key={address.id}
      className={cn(
        "cursor-pointer gap-0 p-0 transition-colors",
        isDefault ? "bg-emerald-100/55" : "",
        className
      )}
    >
      <CardContent className="p-4">

        <div className="flex gap-4">

          <div className="flex-1">
            <div className="flex items-start justify-between gap-2">
              <div>
                <div className="flex items-center gap-2">
                  <span className="font-semibold">
                    {address.receiverName}
                  </span>
                  {isDefault && (
                    <Badge
                      variant="secondary"
                      className="text-xs font-normal bg-emerald-200"
                    >
                      Mặc định
                    </Badge>
                  )}
                </div>
              </div>
              {actionButtons}
            </div>

            <div className="mt-2 text-sm text-muted-foreground">
              <p>{address.street}</p>
              <p>
                {address.ward}, {address.district}, {address.province}
              </p>
              <p className="mt-1">{address.phoneNumber}</p>
            </div>
          </div>
        </div>

      </CardContent>
    </Card>
  )
}