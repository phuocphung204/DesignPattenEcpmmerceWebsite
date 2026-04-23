export type FooterColumn = {
  title: string;
  links: {
    title: string;
    href: string;
  }[];
};

export type CategoryIconKey = "laptop" | "keyboard" | "headphones" | "usb" |  "charger";

export type CategoryCardModel = {
  id: number;
  name: string;
  slug: string;
  iconKey: CategoryIconKey;
};