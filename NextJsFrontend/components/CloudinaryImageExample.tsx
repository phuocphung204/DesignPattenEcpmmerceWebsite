import { CldImage } from "next-cloudinary";

type CloudinaryImageExampleProps = {
  publicId: string;
  alt?: string;
};

export default function CloudinaryImageExample({
  publicId,
  alt = "Cloudinary image",
}: CloudinaryImageExampleProps) {
  return (
    <CldImage
      src={publicId}
      alt={alt}
      width={800}
      height={600}
      crop="thumb"
      gravity="auto"
      quality="auto"
      format="auto"
      sizes="(max-width: 768px) 100vw, 800px"
      className="h-auto w-full rounded-xl border"
    />
  );
}
