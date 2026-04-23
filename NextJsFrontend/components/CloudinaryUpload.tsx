"use client";

import {
  CldUploadWidget,
  type CloudinaryUploadWidgetResults,
} from "next-cloudinary";
import { Suspense, useState } from "react";
import { Button } from "@/components/ui/button";

type CloudinaryUploadInfo = {
  public_id?: string;
};

function isUploadInfo(value: unknown): value is CloudinaryUploadInfo {
  return typeof value === "object" && value !== null;
}

function CloudinarySkeleton() {
  return (
    <div>Skeleton</div>
  );
}

export default function CloudinaryUpload() {
  return (
    <Suspense fallback={<CloudinarySkeleton />}>
      <DynamicCloudinaryUpload />
    </Suspense>
  )
}

function DynamicCloudinaryUpload() {
  const [publicId, setPublicId] = useState<string | null>(null);

  return (
    <div className="space-y-3">
      <CldUploadWidget
        signatureEndpoint="/sign-cloudinary"
        uploadPreset={process.env.NEXT_PUBLIC_CLOUDINARY_UPLOAD_PRESET}
        options={{
          multiple: false,
          resourceType: "image",
          folder: "users-avatars",
        }}
        onSuccess={(results: CloudinaryUploadWidgetResults) => {
          if (!results?.info || !isUploadInfo(results.info)) {
            return;
          }

          if (results.info.public_id) {
            setPublicId(results.info.public_id);
            console.log("Cloudinary public_id:", results.info.public_id);
          }
        }}
      >
        {({ open, isLoading }) => (
          <Button
            type="button"
            onClick={() => open()}
            disabled={isLoading}
            className="cursor-pointer"
          >
            {isLoading ? "Dang tai..." : "Upload anh len Cloudinary"}
          </Button>
        )}
      </CldUploadWidget>

      {publicId && (
        <p className="text-sm text-muted-foreground">
          Uploaded public_id: <span className="font-medium">{publicId}</span>
        </p>
      )}
    </div>
  );
}