import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";

export default function AddressItemSkeleton() {
  return (
    <Card
      className={cn(
        "cursor-pointer gap-0 p-0 transition-colors",
      )}
    >
      <CardContent className="p-4">

        <div className="flex gap-4">

          <div className="flex-1">
            <div className="flex items-start justify-between gap-2">
              <div>
                <div className="flex items-center gap-2">
                  <span className="font-semibold">
                    <Skeleton className="h-8 w-32" />
                  </span>
                </div>
              </div>
            </div>

            <div className="mt-2 text-sm text-muted-foreground">
              <Skeleton className="h-5 w-full" />
              <Skeleton className="h-5 w-full" />
              <Skeleton className="mt-1 h-5 w-24" />
            </div>
          </div>
        </div>

      </CardContent>
    </Card>
  )
}