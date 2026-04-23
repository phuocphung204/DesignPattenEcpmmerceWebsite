"use client";;
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
  FieldSet,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import Cookies from "js-cookie";
import { FileUpload, FileUploadTrigger } from "@/components/ui/file-upload";
import { Camera } from "lucide-react";
import { useEffect, useState } from "react";
import { CurrentUserType, useGetCurrentUser, useUpdateCurrentUser } from "@/entities/user";
import { Skeleton } from "@/components/ui/skeleton";
import { QueryStatus } from "@tanstack/react-query";
import { toast } from "sonner";
import UserAvatar from "@/entities/user/ui/UserAvatar";

const profileUpdateSchema = z.object({
  fullName: z.string().min(2, "Họ tên phải có ít nhất 2 ký tự"),
  avatarLink: z.string(),
});

type ProfileUpdateData = z.infer<typeof profileUpdateSchema>;


type NeedProfileDataType = Pick<CurrentUserType, "email" | "fullName" | "loyaltyPoints" | "avatarLink">;

export default function ProfileSection() {
  console.log(">>> Render ProfileSection");
  const {
    data: currentUserData,
    status: queryStatus,
    isError: isErrorCurrentUser,
    error: currentUserError
  } = useGetCurrentUser<NeedProfileDataType>((user) => ({
    email: user.email,
    fullName: user.fullName,
    loyaltyPoints: user.loyaltyPoints,
    avatarLink: user.avatarLink,
  }));
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isValid, isSubmitting, isDirty },
  } = useForm<ProfileUpdateData>({
    resolver: zodResolver(profileUpdateSchema),
    mode: "onChange",
    defaultValues: {
      fullName: currentUserData?.fullName || "",
      avatarLink: currentUserData?.avatarLink || "",
    },
    values: {
      fullName: currentUserData?.fullName || "",
      avatarLink: currentUserData?.avatarLink || "",
    }
  });
  const { mutate: updateProfileMutation } = useUpdateCurrentUser(
    (successMessage) => toast.success(successMessage),
    (errorMessage) => toast.error(errorMessage)
  );

  const [files, setFiles] = useState<File[]>([]);
  const avatarPreview = files.length > 0 ? URL.createObjectURL(files[0]) : "";

  const handleSubmitUpdateProfile = handleSubmit(async (data) => {
    console.log("Profile updated", {
      fullName: data.fullName,
      avatarLink: data.avatarLink || null,
    });
    updateProfileMutation({
      fullName: data.fullName,
    });
  });

  const handleImageFileUpdate = (files: File[]) => {
    if (files.length > 0) {
      const file = files[0];
      setValue("avatarLink", file.name, { shouldDirty: true });
    }
  }

  const onValueChangeFU = (newFiles: File[]) => {
    setFiles(newFiles);
    handleImageFileUpdate(newFiles);
  }

  const onFileRejectFU = (file: File) => {
    setFiles([file]);
    handleImageFileUpdate([file]);
  };

  useEffect(() => {
    const handleGetCurrentUserError = () => {
      toast.error("Không thể tải thông tin người dùng. Vui lòng thử lại sau.");
    }
    if (isErrorCurrentUser && currentUserError) {
      handleGetCurrentUserError()
    }
    console.log(">>> handleGetCurrentUserError useEffect called with isErrorCurrentUser:", isErrorCurrentUser, "currentUserError:", currentUserError);
  }, [isErrorCurrentUser, currentUserError]);

  return (
    <section className="mx-auto w-full max-w-5xl bg-background p-4">
      <Card className="mb-6">
        <CardContent>
          <div className="flex flex-col gap-5 sm:flex-row sm:items-center sm:justify-between">

            <div className="flex items-center gap-4">
              {/* Avatar Upload */}
              <FileUpload
                value={files}
                // defaultValue={}

                onFileReject={onFileRejectFU}
                onValueChange={onValueChangeFU}
                accept="image/*"
                maxFiles={1}
                maxSize={2 * 1024 * 1024}
              >

                <FileUploadTrigger asChild>
                  <button className="group relative cursor-pointer rounded-full">
                    <UserAvatar className="size-24" avatarPreview={avatarPreview} />
                    <div className="absolute inset-0 flex items-center justify-center rounded-full bg-black/50 opacity-0 transition-opacity group-hover:opacity-100">
                      <Camera className="size-6 text-white" />
                    </div>
                  </button>
                </FileUploadTrigger>
              </FileUpload>
              <div className="flex min-w-0 flex-col gap-1">
                <SafeDisplay queryStatus={queryStatus} skeletonClassName="h-5 w-32">
                  <h2 className="truncate text-lg font-semibold">{currentUserData?.fullName}</h2>
                </SafeDisplay>
                <p className="text-sm text-muted-foreground">Ảnh đại diện</p>
                <p className="text-sm text-muted-foreground">Nhấn vào để đổi</p>
              </div>
            </div>

            <div className="flex flex-col gap-1 text-left sm:text-right">
              <p className="text-sm text-muted-foreground">Điểm thành viên</p>
              <SafeDisplay queryStatus={queryStatus} skeletonClassName="h-6 w-20">
                <p className="text-2xl font-semibold">{currentUserData?.loyaltyPoints}</p>
              </SafeDisplay>
            </div>
          </div>
        </CardContent>
      </Card>

      <form id="profile-form" onSubmit={handleSubmitUpdateProfile} noValidate className="flex flex-col gap-5">
        <FieldSet>
          <FieldGroup>

            <Field data-disabled>
              <FieldLabel htmlFor="email">Email</FieldLabel>
              <Input id="email" defaultValue={currentUserData?.email} readOnly />
            </Field>

            <Field data-invalid={!!errors.fullName}>
              <FieldLabel htmlFor="fullName">Họ và tên</FieldLabel>
              <Input
                id="fullName"
                aria-invalid={!!errors.fullName}
                {...register("fullName")}
              />
              <FieldError errors={[errors.fullName]} />
            </Field>

          </FieldGroup>
        </FieldSet>

        <div className="flex justify-end">
          <Button type="submit" form="profile-form" disabled={!isValid || isSubmitting || !isDirty}>
            {isSubmitting ? "Đang lưu..." : "Lưu thay đổi"}
          </Button>
        </div>
      </form>
    </section>
  );
}

function SafeDisplay({
  skeletonClassName,
  queryStatus,
  children
}: {
  skeletonClassName?: string;
  queryStatus: QueryStatus;
  children: React.ReactNode
}) {
  return (
    <>
      {queryStatus === "success" ? (
        children
      ) : (
        <Skeleton className={skeletonClassName} />
      )}
    </>
  );
}