import Dropdown from "@/components/shadcn-space/blocks/dropdown-01/dropdown";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import UserAvatar from "@/entities/user/ui/UserAvatar";
import UserDropdownMenu from "../_components/UserDropdownMenu";
import { Suspense } from "react";

const page = () => {
  return (
    <div>

      <Dropdown
        defaultOpen={true}
        align="center"
        trigger={
          <div className="rounded-full">
            <Avatar className="size-10 cursor-pointer">
              <AvatarImage
                src="https://images.shadcnspace.com/assets/profiles/user-11.jpg"
                alt="David McMichael"
              />
              <AvatarFallback>DM</AvatarFallback>
            </Avatar>
          </div>
        }
      />
      <UserDropdownMenu
        defaultOpen={false}
        align="end"
        trigger={
          <UserAvatar className="size-10" />
        }
      />
    </div>
  );
};

export default page;