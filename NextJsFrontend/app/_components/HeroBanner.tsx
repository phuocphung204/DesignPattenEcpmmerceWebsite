"use client";

import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { Button } from "@/components/ui/button";
import Image from "next/image";
import AutoPlay from "embla-carousel-autoplay";
import { useRef } from "react";

const slides = [
  {
    image: "/slides/laptop-acer.png",
    alt: "Acer premium laptop banner",
    title: "ACER SWIFT X:",
    subtitle: "NÂNG TẦM SÁNG TẠO,\nHIỆU NĂNG BỨC PHÁ",
    specs: "Màn hình 14 inch 2.8K OLED | Ryzen AI | 16GB RAM",
    price: "Giá từ: 32.990.000 VND",
    cta: "MUA NGAY",
  },
  {
    image: "/slides/laptop-dell.png",
    alt: "Dell XPS premium laptop banner",
    title: "DELL XPS 13 PLUS:",
    subtitle: "ĐỘT PHÁ THIẾT KẾ,\nHIỆU NĂNG TỐI ĐA",
    specs: "Màn hình 13.4 inch 3.5K OLED | Bàn phím Edge-to-Edge",
    price: "Giá từ: 48.990.000 VND",
    cta: "MUA NGAY",
  },
]

export default function HeroBanner() {
  const plugin = useRef(
    AutoPlay({ delay: 2500, stopOnInteraction: true, active: true })
  )

  return (
    <Carousel className="mx-auto w-full max-w-7xl pt-3"
      opts={{
        loop: true,
      }}
      plugins={[plugin.current]}
      onMouseEnter={plugin.current.stop}
    // onMouseLeave={plugin.current.play}
    >
      <CarouselContent>
        {slides.map((slide, index) => (
          <CarouselItem key={slide.image}>
            <div className="relative overflow-hidden rounded-2xl flex justify-center">
              <Image
                src={slide.image}
                alt={slide.alt}
                width={1200}
                height={600}
                priority={index === 0}
                className="h-auto w-auto object-cover max-w-7xl"
              />

              <div className="absolute inset-0 bg-linear-to-r from-black/80 via-black/35 to-transparent" />

              <div className="absolute inset-y-0 left-0 flex w-full max-w-xl items-center px-6 sm:px-10 lg:px-14">
                <div className="flex flex-col gap-2 text-white sm:gap-3">
                  <p className="text-sm font-semibold uppercase tracking-[0.18em] text-amber-300/90">
                    {slide.title}
                  </p>
                  <h2 className="whitespace-pre-line text-2xl font-black uppercase leading-tight sm:text-3xl lg:text-4xl">
                    {slide.subtitle}
                  </h2>
                  <p className="max-w-lg text-xs text-zinc-100 sm:text-sm lg:text-base">
                    {slide.specs}
                  </p>
                  <p className="text-base font-bold text-white sm:text-lg">
                    {slide.price}
                  </p>
                  <Button
                    className="mt-2 h-10 w-fit cursor-pointer rounded-full bg-amber-500 px-6 text-sm font-semibold text-black transition-colors duration-200 hover:bg-amber-400 sm:mt-3 sm:h-11 sm:px-8 sm:text-base"
                  >
                    {slide.cta}
                  </Button>
                </div>
              </div>
            </div>
          </CarouselItem>
        ))}
      </CarouselContent>
      <CarouselPrevious className="left-3 border-zinc-200/40 bg-white/75 text-zinc-900 backdrop-blur-sm hover:bg-white" />
      <CarouselNext className="right-3 border-zinc-200/40 bg-white/75 text-zinc-900 backdrop-blur-sm hover:bg-white" />
    </Carousel>
  )
}