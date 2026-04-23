"use client";

import CloudinaryImageExample from "@/components/CloudinaryImageExample";
import CloudinaryUpload from "@/components/CloudinaryUpload";

const demoPublicId =
  process.env.NEXT_PUBLIC_CLOUDINARY_DEMO_PUBLIC_ID ||
  "samples/landscapes/nature-mountains";

export default function CloudinaryDemoPage() {
  return (
    <main className="mx-auto max-w-4xl space-y-8 px-4 py-10">
      <section className="space-y-2">
        <h1 className="text-3xl font-semibold">Cloudinary Signed Upload Demo</h1>
        <p className="text-sm text-muted-foreground">
          Upload signed qua API Route, sau do luu public_id ve backend C# cua ban.
        </p>
      </section>

      <section className="space-y-3 rounded-xl border p-6">
        <h2 className="text-xl font-medium">1) Upload anh</h2>
        <CloudinaryUpload />
      </section>

      <section className="space-y-3 rounded-xl border p-6">
        <h2 className="text-xl font-medium">2) Hien thi bang public_id</h2>
        <CloudinaryImageExample
          publicId={demoPublicId}
          alt="Demo Cloudinary image"
        />
      </section>
    </main>
  );
}
