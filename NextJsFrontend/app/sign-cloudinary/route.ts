import { v2 as cloudinary } from "cloudinary";
import { NextResponse } from "next/server";
import { getUserJwtToken } from "@/lib/auth-helper";
import http from "@/services/http";
type SignatureRequestBody = {
  paramsToSign?: Record<string, string | number | boolean>;
};

function getCloudinaryConfig() {
  const cloudName = process.env.NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME;
  const apiKey = process.env.NEXT_PUBLIC_CLOUDINARY_API_KEY;
  const apiSecret = process.env.CLOUDINARY_API_SECRET;

  if (!cloudName || !apiKey || !apiSecret) {
    return null;
  }

  return { cloudName, apiKey, apiSecret };
}

export async function POST(request: Request) {
  const cloudinaryConfig = getCloudinaryConfig();

  if (!cloudinaryConfig) {
    return NextResponse.json(
      { error: "Missing Cloudinary environment variables." },
      { status: 500 }
    );
  }

  try {
    const body = (await request.json()) as SignatureRequestBody;

    if (!body.paramsToSign || typeof body.paramsToSign !== "object") {
      return NextResponse.json(
        { error: "paramsToSign is required." },
        { status: 400 }
      );
    }

    cloudinary.config({
      cloud_name: cloudinaryConfig.cloudName,
      api_key: cloudinaryConfig.apiKey,
      api_secret: cloudinaryConfig.apiSecret,
      secure: true,
    });

    const signature = cloudinary.utils.api_sign_request(
      body.paramsToSign,
      cloudinaryConfig.apiSecret
    );

    return NextResponse.json({ signature });
  } catch {
    return NextResponse.json(
      { error: "Failed to generate Cloudinary signature." },
      { status: 500 }
    );
  }
}

export async function GET() {
  const cloudinaryConfig = getCloudinaryConfig();
  if (!cloudinaryConfig) { 
    return NextResponse.json(
      { error: "Missing Cloudinary environment variables." },
      { status: 500 }
    );
  }

  const jwtToken = getUserJwtToken();
    if (!jwtToken) {
    return NextResponse.json(
      { error: "Unauthorized" },
      { status: 401 }
    );
  }
  const res =  await http.head("/auth/validate-token", {
    headers: {
      Authorization: `Bearer ${jwtToken}`,
    },
  });
  if (res.status === 401) {
    return NextResponse.json(
      { error: "Unauthorized" },
      { status: 401 }
    );
  }

  const timestamp = Math.round(new Date().getTime() / 1000);
  const signature = cloudinary.utils.api_sign_request(
      { timestamp: timestamp, folder: 'user_uploads' }, 
      cloudinaryConfig.apiSecret
  );

  return NextResponse.json({ signature, timestamp});
}