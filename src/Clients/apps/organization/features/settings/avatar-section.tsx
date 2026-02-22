"use client";

import { useRef, useState } from "react";

import { Camera, Loader2, UserCircle } from "lucide-react";
import { toast } from "sonner";

import useUpdateProfile from "@workspace/api-hooks/profiles/useUpdateProfile";
import type { OwnProfileDetails } from "@workspace/types/profile";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@workspace/ui/components/avatar";
import { Button } from "@workspace/ui/components/button";
import { Input } from "@workspace/ui/components/input";
import { getInitials } from "@workspace/utils/format";
import {
  ALLOWED_IMAGE_TYPES,
  MAX_AVATAR_SIZE,
} from "@workspace/validations/profile";

import { buildProfileUpdateRequest } from "@/lib/profile-update";

type AvatarSectionProps = {
  profile: OwnProfileDetails;
  subtitle?: string;
};

export function AvatarSection({
  profile,
  subtitle,
}: AvatarSectionProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);

  const uploadMutation = useUpdateProfile({
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
    uploadMutation.mutate(buildProfileUpdateRequest(profile, { avatar: file }));
  }

  function clearFileInput() {
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  }

  const displayUrl = preview ?? profile.avatarUrl;

  return (
    <div className="flex flex-col items-center gap-4 sm:flex-row sm:gap-6">
      <div className="group relative">
        <Avatar className="ring-background size-20 ring-4 sm:size-24">
          <AvatarImage src={displayUrl ?? undefined} alt={profile.firstName} />
          <AvatarFallback className="bg-primary/10 text-primary text-xl sm:text-2xl">
            {profile.firstName || profile.lastName ? (
              getInitials(
                [profile.lastName, profile.firstName, profile.middleName]
                  .filter(Boolean)
                  .join(" "),
              )
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
        <h2 className="text-lg font-semibold">
          {[profile.lastName, profile.firstName, profile.middleName]
            .filter(Boolean)
            .join(" ")}
        </h2>
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
