import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
    { protocol: "https", hostname: "**" },
      { protocol: "http", hostname: "**" },
      { protocol: "https", hostname: "res.cloudinary.com", },
      { protocol: "https", hostname: "cdn2.cellphones.com.vn" },
      { protocol: "https", hostname: "3kshop.vn" },
      { protocol: "https", hostname: "www.sony.com.vn" },
      { protocol: "https", hostname: "example.com" },
    ],
  },
  cacheComponents: true,
};

export default nextConfig;
