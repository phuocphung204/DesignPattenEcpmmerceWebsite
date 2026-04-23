"use client";
import { useEffect, useState, useMemo, useCallback } from "react";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselApi,
} from "@/components/ui/carousel";
import Image from "next/image";
import { AspectRatio } from "@/components/ui/aspect-ratio";

interface GalleryProps {
  images: string[];
}

export default function ImagesCarousel({ images }: GalleryProps) {
  const [mainApi, setMainApi] = useState<CarouselApi>();
  const [thumbnailApi, setThumbnailApi] = useState<CarouselApi>();
  const [current, setCurrent] = useState(0);

  const mainImage = useMemo(
    () =>
      images.map((image, index) => (
        <CarouselItem key={index} className="relative w-full p-0 overflow-hidden">
          <AspectRatio ratio={16 / 9} className="rounded-lg">
            <Image
              // className="object-cover"
              src={image}
              alt={`Carousel Main Image ${index + 1}`}
              fill
            />
          </AspectRatio>
        </CarouselItem>
      )),
    [images],
  );

  const handleClick = useCallback((index: number) => {
    if (!mainApi || !thumbnailApi) {
      return;
    }
    thumbnailApi.scrollTo(index);
    mainApi.scrollTo(index);
    setCurrent(index);
  }, [mainApi, thumbnailApi]);

  const thumbnailImages = useMemo(
    () =>
      images.map((image, index) => (
        <CarouselItem
          key={index}
          className={`relative w-full basis-1/4 rounded-lg border ${index === current ? "border-amber-500 border-2" : "border-slate-200"}  p-0 overflow-hidden cursor-pointer`}
          onClick={() => handleClick(index)}
        >
          <AspectRatio ratio={16 / 9}>
            <Image
              className={`object-cover`}
              src={image}
              fill
              alt={`Carousel Thumbnail Image ${index + 1}`}
            />
          </AspectRatio>
        </CarouselItem>
      )),
    [images, current, handleClick],
  );

  useEffect(() => {
    if (!mainApi || !thumbnailApi) {
      return;
    }

    const handleTopSelect = () => {
      const selected = mainApi.selectedScrollSnap();
      setCurrent(selected);
      thumbnailApi.scrollTo(selected);
    };

    const handleBottomSelect = () => {
      const selected = thumbnailApi.selectedScrollSnap();
      setCurrent(selected);
      mainApi.scrollTo(selected);
    };

    mainApi.on("select", handleTopSelect);
    thumbnailApi.on("select", handleBottomSelect);

    return () => {
      mainApi.off("select", handleTopSelect);
      thumbnailApi.off("select", handleBottomSelect);
    };
  }, [mainApi, thumbnailApi]);

  return (
    <div className="w-96 max-w-xl sm:w-auto">
      <Carousel setApi={setMainApi} className="border border-slate-200 rounded-lg">
        <CarouselContent className="m-1">{mainImage}</CarouselContent>
      </Carousel>
      <Carousel setApi={setThumbnailApi} className="mt-4">
        <CarouselContent className="m-1 gap-4">{thumbnailImages}</CarouselContent>
      </Carousel>
    </div>
  );
};
