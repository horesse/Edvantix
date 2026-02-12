"use client";

import { useRef, useState } from "react";

import { Camera, Loader2, UserCircle } from "lucide-react";
import { toast } from "sonner";

import useUploadAvatar from "@workspace/api-hooks/profiles/useUploadAvatar";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@workspace/ui/components/avatar";
import { Button } from "@workspace/ui/components/button";
import { Input } from "@workspace/ui/components/input";

const MAX_AVATAR_SIZE = 5 * 1024 * 1024;
const ALLOWED_IMAGE_TYPES = [
  "image/jpeg",
  "image/jpg",
  "image/png",
  "image/gif",
  "image/webp",
];

type AvatarSectionProps = {
  avatarUrl?: string | null;
  fullName: string;
  subtitle?: string;
};

function getInitials(name: string): string {
  return name
    .split(" ")
    .map((part) => part[0])
    .filter(Boolean)
    .slice(0, 2)
    .join("")
    .toUpperCase();
}

export function AvatarSection({
  avatarUrl,
  fullName,
  subtitle,
}: AvatarSectionProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);

  const uploadMutation = useUploadAvatar({
    onSuccess: () => {
      toast.success("Аватар обновлён");
      if (preview) {
        URL.revokeObjectURL(preview);
        setPreview(null);
      }
    },
    onError: () => {
      toast.error("Не удалось загрузить аватар");
      if (preview) {
        URL.revokeObjectURL(preview);
        setPreview(null);
      }
    },
  });

  function handleFileChange(file: File | null) {
    if (!file) return;

    if (file.size > MAX_AVATAR_SIZE) {
      toast.error("Размер файла не должен превышать 5 МБ");
      return;
    }

    if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
      toast.error("Допустимые форматы: JPEG, PNG, GIF, WebP");
      return;
    }

    if (preview) {
      URL.revokeObjectURL(preview);
    }
    setPreview(URL.createObjectURL(file));
    uploadMutation.mutate(file);
  }

  function clearFileInput() {
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  }

  const displayUrl = preview ?? avatarUrl;

  return (
    <div className="flex flex-col items-center gap-4 sm:flex-row sm:gap-6">
      <div className="group relative">
        <Avatar className="ring-background size-20 ring-4 sm:size-24">
          <AvatarImage src={displayUrl ?? undefined} alt={fullName} />
          <AvatarFallback className="bg-primary/10 text-primary text-xl sm:text-2xl">
            {fullName ? (
              getInitials(fullName)
            ) : (
              <UserCircle className="size-10" />
            )}
          </AvatarFallback>
        </Avatar>
        <button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadMutation.isPending}
          className="bg-primary text-primary-foreground ring-background absolute -right-1 -bottom-1 flex size-8 items-center justify-center rounded-full ring-2 transition-transform hover:scale-110 active:scale-95 disabled:opacity-50"
        >
          {uploadMutation.isPending ? (
            <Loader2 className="size-4 animate-spin" />
          ) : (
            <Camera className="size-4" />
          )}
        </button>
      </div>
      <div className="text-center sm:text-left">
        <h2 className="text-lg font-semibold">{fullName}</h2>
        {subtitle && (
          <p className="text-muted-foreground text-sm">{subtitle}</p>
        )}
        <Button
          type="button"
          variant="link"
          size="sm"
          className="text-muted-foreground mt-1 h-auto p-0 text-xs"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadMutation.isPending}
        >
          Изменить фото
        </Button>
      </div>
      <Input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/gif,image/webp"
        className="hidden"
        onChange={(e) => {
          handleFileChange(e.target.files?.[0] ?? null);
          clearFileInput();
        }}
      />
    </div>
  );
}
