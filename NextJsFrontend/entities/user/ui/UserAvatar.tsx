"use client"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { useGetCurrentUser } from "../model/hook";

export default function UserAvatar({
  className,
  avatarPreview

}: {
  className: string
  avatarPreview?: string | null
}) {

  const { data } = useGetCurrentUser((user) => ({
    fullName: user.fullName,
    avatarLink: user.avatarLink,
  }));

  const avatarSrc = avatarPreview || data?.avatarLink || undefined;

  return (
    <Avatar className={className}>
      <AvatarImage src={avatarSrc} alt="Avatar" />
      <AvatarFallback>{data?.fullName?.charAt(0) || "User"}</AvatarFallback>
    </Avatar>
  )
}