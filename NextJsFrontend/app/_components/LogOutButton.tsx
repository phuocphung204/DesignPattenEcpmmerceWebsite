import { Button } from "@base-ui/react";
import { LogOut } from "lucide-react";

type Props = {

  onLogOut?: () => void
};
export default function LogOutButton(props: Props) {
  return (
    <Button
      type="button"
      // variant="ghost"
      className="h-11 w-full justify-start rounded-xl px-3 text-sm text-rose-600 hover:bg-rose-50 hover:text-rose-700 focus-visible:ring-2 focus-visible:ring-rose-500"
      aria-label="Đăng xuất"
      onClick={props.onLogOut}
    >
      <LogOut className="mr-2 size-4" />
      Đăng xuất
    </Button>
  );
}