"use client";

import { useRef, useState } from "react";

import { Camera, Loader2, UserCircle } from "lucide-react";
import { toast } from "sonner";

import useUploadAvatar from "@workspace/api-hooks/profiles/useUploadAvatar";
import type { OwnProfileDetails } from "@workspace/types/profile";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@workspace/ui/components/avatar";
import { Input } from "@workspace/ui/components/input";
import { getInitials } from "@workspace/utils/format";
import {
  ALLOWED_IMAGE_TYPES,
  MAX_AVATAR_SIZE,
} from "@workspace/validations/profile";

export function AvatarBlock({ profile }: { profile: OwnProfileDetails }) {
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

    if (preview) URL.revokeObjectURL(preview);
    setPreview(URL.createObjectURL(file));

    uploadMutation.mutate(file);
  }

  const displayUrl = preview ?? profile.avatarUrl;
  const fullName = [profile.lastName, profile.firstName, profile.middleName]
    .filter(Boolean)
    .join(" ");

  return (
    <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:gap-5">
      <div className="relative shrink-0">
        <Avatar className="size-16 ring-2 ring-background">
          <AvatarImage src={displayUrl ?? undefined} alt={profile.firstName} />
          <AvatarFallback className="bg-muted text-base font-medium text-foreground">
            {fullName ? (
              getInitials(fullName)
            ) : (
              <UserCircle className="size-7" />
            )}
          </AvatarFallback>
        </Avatar>

        <button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadMutation.isPending}
          className="absolute -bottom-1 -right-1 flex size-6 items-center justify-center rounded-full border border-border/60 bg-background shadow-sm transition-all hover:scale-110 active:scale-95 disabled:opacity-50"
          aria-label="Изменить фото"
        >
          {uploadMutation.isPending ? (
            <Loader2 className="size-3 animate-spin text-muted-foreground" />
          ) : (
            <Camera className="size-3" />
          )}
        </button>
      </div>

      <div className="min-w-0">
        <p className="truncate text-sm font-semibold">{fullName || "—"}</p>
        <p className="text-xs text-muted-foreground">{profile.login}</p>
        <button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadMutation.isPending}
          className="mt-1.5 text-xs text-muted-foreground transition-colors hover:text-foreground disabled:opacity-50"
        >
          {uploadMutation.isPending ? "Загружаю…" : "Изменить фото"}
        </button>
      </div>

      <Input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/gif,image/webp"
        className="hidden"
        onChange={(e) => {
          handleFileChange(e.target.files?.[0] ?? null);
          if (fileInputRef.current) fileInputRef.current.value = "";
        }}
      />
    </div>
  );
}
