"use client";

import {
  MapPin,
  MoreVertical,
  Pencil,
  Plus,
  Save,
  Trash2,
  X,
} from "lucide-react";
import { useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { cn } from "@/lib/utils";

interface Address {
  id: string;
  name: string;
  street: string;
  city: string;
  state: string;
  zip: string;
  country: string;
  phone: string;
  isDefault: boolean;
  type: "home" | "work" | "other";
}

const DEFAULT_ADDRESSES: Address[] = [
  {
    id: "1",
    name: "John Doe",
    street: "123 Main Street, Apt 4B",
    city: "New York",
    state: "NY",
    zip: "10001",
    country: "United States",
    phone: "(212) 555-1234",
    isDefault: true,
    type: "home",
  },
  {
    id: "2",
    name: "John Doe",
    street: "456 Business Ave, Suite 100",
    city: "New York",
    state: "NY",
    zip: "10018",
    country: "United States",
    phone: "(212) 555-5678",
    isDefault: false,
    type: "work",
  },
];

interface AddressBook1Props {
  addresses?: Address[];
  className?: string;
}

const AddressBook1 = ({
  addresses: initialAddresses = DEFAULT_ADDRESSES,
  className,
}: AddressBook1Props) => {
  const [addresses, setAddresses] = useState(initialAddresses);
  const [selectedId, setSelectedId] = useState(
    addresses.find((a) => a.isDefault)?.id || addresses[0]?.id,
  );
  const [editingId, setEditingId] = useState<string | null>(null);
  const [isAddingNew, setIsAddingNew] = useState(false);
  const [editForm, setEditForm] = useState<Partial<Address>>({});

  const startEditing = (address: Address) => {
    setEditingId(address.id);
    setEditForm({ ...address });
  };

  const cancelEditing = () => {
    setEditingId(null);
    setEditForm({});
  };

  const saveEdit = () => {
    if (
      editingId &&
      editForm.name &&
      editForm.street &&
      editForm.city &&
      editForm.state &&
      editForm.zip &&
      editForm.country &&
      editForm.phone &&
      editForm.type
    ) {
      setAddresses((prev) =>
        prev.map((addr) =>
          addr.id === editingId
            ? ({
                ...addr,
                ...editForm,
              } as Address)
            : addr,
        ),
      );
      setEditingId(null);
      setEditForm({});
    }
  };

  const startAddingNew = () => {
    setIsAddingNew(true);
    setEditForm({
      name: "",
      street: "",
      city: "",
      state: "",
      zip: "",
      country: "United States",
      phone: "",
      type: "home",
      isDefault: false,
    });
  };

  const cancelAddingNew = () => {
    setIsAddingNew(false);
    setEditForm({});
  };

  const saveNewAddress = () => {
    if (
      editForm.name &&
      editForm.street &&
      editForm.city &&
      editForm.state &&
      editForm.zip &&
      editForm.country &&
      editForm.phone &&
      editForm.type
    ) {
      const newAddress: Address = {
        id: Date.now().toString(),
        name: editForm.name,
        street: editForm.street,
        city: editForm.city,
        state: editForm.state,
        zip: editForm.zip,
        country: editForm.country,
        phone: editForm.phone,
        type: editForm.type as "home" | "work" | "other",
        isDefault: addresses.length === 0,
      };
      setAddresses((prev) => [...prev, newAddress]);
      setIsAddingNew(false);
      setEditForm({});
      if (addresses.length === 0) {
        setSelectedId(newAddress.id);
      }
    }
  };

  const deleteAddress = (id: string) => {
    setAddresses((prev) => prev.filter((addr) => addr.id !== id));
    if (selectedId === id && addresses.length > 1) {
      setSelectedId(addresses.find((a) => a.id !== id)?.id || "");
    }
    if (editingId === id) {
      setEditingId(null);
      setEditForm({});
    }
  };

  return (
    <section className={cn("py-16 md:py-24", className)}>
      <div className="container max-w-3xl">
        {/* Header */}
        <div className="mb-6 flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold tracking-tight md:text-3xl">
              My Addresses
            </h1>
            <p className="mt-1 text-muted-foreground">
              Manage your shipping addresses
            </p>
          </div>
          <Button onClick={startAddingNew}>
            <Plus className="mr-2 size-4" />
            Add New
          </Button>
        </div>

        {/* Address List */}
        <RadioGroup value={selectedId} onValueChange={setSelectedId}>
          <div className="space-y-4">
            {addresses.map((address) => (
              <Card
                key={address.id}
                className={cn(
                  "cursor-pointer gap-0 p-0 transition-colors",
                  selectedId === address.id && "border-primary",
                  editingId === address.id && "border-primary",
                )}
                onClick={() =>
                  editingId !== address.id && setSelectedId(address.id)
                }
              >
                <CardContent className="p-4">
                  {editingId === address.id ? (
                    <div className="space-y-4">
                      <div className="flex items-center justify-between">
                        <h3 className="font-semibold">Edit Address</h3>
                        <div className="flex gap-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={(e) => {
                              e.stopPropagation();
                              cancelEditing();
                            }}
                          >
                            <X className="size-4" />
                          </Button>
                          <Button
                            size="sm"
                            onClick={(e) => {
                              e.stopPropagation();
                              saveEdit();
                            }}
                          >
                            <Save className="mr-2 size-4" />
                            Save
                          </Button>
                        </div>
                      </div>
                      <div className="grid gap-4 sm:grid-cols-2">
                        <div className="space-y-2">
                          <Label htmlFor={`name-${address.id}`}>
                            Full Name
                          </Label>
                          <Input
                            id={`name-${address.id}`}
                            value={editForm.name || ""}
                            onChange={(e) =>
                              setEditForm({ ...editForm, name: e.target.value })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor={`phone-${address.id}`}>Phone</Label>
                          <Input
                            id={`phone-${address.id}`}
                            value={editForm.phone || ""}
                            onChange={(e) =>
                              setEditForm({
                                ...editForm,
                                phone: e.target.value,
                              })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2 sm:col-span-2">
                          <Label htmlFor={`street-${address.id}`}>
                            Street Address
                          </Label>
                          <Input
                            id={`street-${address.id}`}
                            value={editForm.street || ""}
                            onChange={(e) =>
                              setEditForm({
                                ...editForm,
                                street: e.target.value,
                              })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor={`city-${address.id}`}>City</Label>
                          <Input
                            id={`city-${address.id}`}
                            value={editForm.city || ""}
                            onChange={(e) =>
                              setEditForm({ ...editForm, city: e.target.value })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor={`state-${address.id}`}>State</Label>
                          <Input
                            id={`state-${address.id}`}
                            value={editForm.state || ""}
                            onChange={(e) =>
                              setEditForm({
                                ...editForm,
                                state: e.target.value,
                              })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor={`zip-${address.id}`}>ZIP Code</Label>
                          <Input
                            id={`zip-${address.id}`}
                            value={editForm.zip || ""}
                            onChange={(e) =>
                              setEditForm({ ...editForm, zip: e.target.value })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor={`country-${address.id}`}>
                            Country
                          </Label>
                          <Input
                            id={`country-${address.id}`}
                            value={editForm.country || ""}
                            onChange={(e) =>
                              setEditForm({
                                ...editForm,
                                country: e.target.value,
                              })
                            }
                            onClick={(e) => e.stopPropagation()}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor={`type-${address.id}`}>Type</Label>
                          <Select
                            value={editForm.type || "home"}
                            onValueChange={(value: "home" | "work" | "other") =>
                              setEditForm({ ...editForm, type: value })
                            }
                          >
                            <SelectTrigger
                              id={`type-${address.id}`}
                              onClick={(e) => e.stopPropagation()}
                            >
                              <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                              <SelectItem value="home">Home</SelectItem>
                              <SelectItem value="work">Work</SelectItem>
                              <SelectItem value="other">Other</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                    </div>
                  ) : (
                    <div className="flex gap-4">
                      <RadioGroupItem
                        value={address.id}
                        id={address.id}
                        className="mt-1"
                      />
                      <div className="flex-1">
                        <div className="flex items-start justify-between gap-2">
                          <div>
                            <div className="flex items-center gap-2">
                              <span className="font-semibold">
                                {address.name}
                              </span>
                              {address.isDefault && (
                                <Badge
                                  variant="secondary"
                                  className="text-xs font-normal"
                                >
                                  Default
                                </Badge>
                              )}
                              <Badge
                                variant="outline"
                                className="text-xs capitalize"
                              >
                                {address.type}
                              </Badge>
                            </div>
                          </div>
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button
                                variant="ghost"
                                size="icon"
                                className="size-8"
                                onClick={(e) => e.stopPropagation()}
                              >
                                <MoreVertical className="size-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem
                                onClick={(e) => {
                                  e.stopPropagation();
                                  startEditing(address);
                                }}
                              >
                                <Pencil className="mr-2 size-4" />
                                Edit
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                className="text-destructive"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  deleteAddress(address.id);
                                }}
                              >
                                <Trash2 className="mr-2 size-4" />
                                Delete
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        </div>

                        <div className="mt-2 text-sm text-muted-foreground">
                          <p>{address.street}</p>
                          <p>
                            {address.city}, {address.state} {address.zip}
                          </p>
                          <p>{address.country}</p>
                          <p className="mt-1">{address.phone}</p>
                        </div>
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
            ))}

            {isAddingNew && (
              <Card className="gap-0 border-primary p-0">
                <CardContent className="p-4">
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <h3 className="font-semibold">Add New Address</h3>
                      <div className="flex gap-2">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={cancelAddingNew}
                        >
                          <X className="size-4" />
                        </Button>
                        <Button size="sm" onClick={saveNewAddress}>
                          <Save className="mr-2 size-4" />
                          Save
                        </Button>
                      </div>
                    </div>
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div className="space-y-2">
                        <Label htmlFor="new-name">Full Name</Label>
                        <Input
                          id="new-name"
                          value={editForm.name || ""}
                          onChange={(e) =>
                            setEditForm({ ...editForm, name: e.target.value })
                          }
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="new-phone">Phone</Label>
                        <Input
                          id="new-phone"
                          value={editForm.phone || ""}
                          onChange={(e) =>
                            setEditForm({ ...editForm, phone: e.target.value })
                          }
                        />
                      </div>
                      <div className="space-y-2 sm:col-span-2">
                        <Label htmlFor="new-street">Street Address</Label>
                        <Input
                          id="new-street"
                          value={editForm.street || ""}
                          onChange={(e) =>
                            setEditForm({ ...editForm, street: e.target.value })
                          }
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="new-city">City</Label>
                        <Input
                          id="new-city"
                          value={editForm.city || ""}
                          onChange={(e) =>
                            setEditForm({ ...editForm, city: e.target.value })
                          }
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="new-state">State</Label>
                        <Input
                          id="new-state"
                          value={editForm.state || ""}
                          onChange={(e) =>
                            setEditForm({ ...editForm, state: e.target.value })
                          }
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="new-zip">ZIP Code</Label>
                        <Input
                          id="new-zip"
                          value={editForm.zip || ""}
                          onChange={(e) =>
                            setEditForm({ ...editForm, zip: e.target.value })
                          }
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="new-country">Country</Label>
                        <Input
                          id="new-country"
                          value={editForm.country || ""}
                          onChange={(e) =>
                            setEditForm({
                              ...editForm,
                              country: e.target.value,
                            })
                          }
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="new-type">Type</Label>
                        <Select
                          value={editForm.type || "home"}
                          onValueChange={(value: "home" | "work" | "other") =>
                            setEditForm({ ...editForm, type: value })
                          }
                        >
                          <SelectTrigger id="new-type">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="home">Home</SelectItem>
                            <SelectItem value="work">Work</SelectItem>
                            <SelectItem value="other">Other</SelectItem>
                          </SelectContent>
                        </Select>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>
        </RadioGroup>

        {addresses.length === 0 && (
          <Card className="p-0">
            <CardContent className="flex flex-col items-center justify-center py-12">
              <MapPin className="mb-4 size-12 text-muted-foreground" />
              <h2 className="text-xl font-semibold">No addresses saved</h2>
              <p className="mt-2 text-muted-foreground">
                Add an address to make checkout faster
              </p>
              <Button className="mt-4" onClick={startAddingNew}>
                <Plus className="mr-2 size-4" />
                Add Address
              </Button>
            </CardContent>
          </Card>
        )}
      </div>
    </section>
  );
};

export { AddressBook1 };
