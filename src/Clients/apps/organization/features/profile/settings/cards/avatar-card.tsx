"use client";

import { useRef, useState } from "react";

import { Camera, Loader2, Upload, User } from "lucide-react";
import { toast } from "sonner";

import useDeleteAvatar from "@workspace/api-hooks/profiles/useDeleteAvatar";
import useUploadAvatar from "@workspace/api-hooks/profiles/useUploadAvatar";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Input } from "@workspace/ui/components/input";
import { cn } from "@workspace/ui/lib/utils";
import { getInitials } from "@workspace/utils/format";
import {
  ALLOWED_IMAGE_TYPES,
  MAX_AVATAR_SIZE,
} from "@workspace/validations/profile";

export function AvatarCard({ profile }: { profile: OwnProfileDetails }) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [isDragging, setIsDragging] = useState(false);

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

  const deleteMutation = useDeleteAvatar({
    onSuccess: () => {
      toast.success("Аватар удалён");
      if (preview) {
        URL.revokeObjectURL(preview);
        setPreview(null);
      }
    },
    onError: () => {
      toast.error("Не удалось удалить аватар");
    },
  });

  function handleFile(file: File | null) {
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

  function handleDrop(e: React.DragEvent) {
    e.preventDefault();
    setIsDragging(false);
    handleFile(e.dataTransfer.files[0] ?? null);
  }

  const displayUrl = preview ?? profile.avatarUrl;
  const fullName = [profile.lastName, profile.firstName, profile.middleName]
    .filter(Boolean)
    .join(" ");
  const initials = fullName ? getInitials(fullName) : null;
  const isPending = uploadMutation.isPending || deleteMutation.isPending;

  return (
    <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
      {/* Header */}
      <div className="mb-4 flex items-center gap-2">
        <div className="bg-primary/10 flex size-6 items-center justify-center rounded-lg">
          <User className="text-primary size-3.5" />
        </div>
        <span className="text-foreground text-sm font-semibold">
          Фото профиля
        </span>
      </div>

      <div className="flex flex-col items-center gap-4">
        {/* Avatar with hover overlay */}
        <div
          className="group relative cursor-pointer"
          role="button"
          tabIndex={0}
          onClick={() => !isPending && fileInputRef.current?.click()}
          onKeyDown={(e) => {
            if ((e.key === "Enter" || e.key === " ") && !isPending) {
              fileInputRef.current?.click();
            }
          }}
        >
          <div className="to-primary flex size-28 items-center justify-center overflow-hidden rounded-2xl bg-gradient-to-br from-violet-400 shadow-md">
            {displayUrl ? (
              // eslint-disable-next-line @next/next/no-img-element
              <img
                src={displayUrl}
                alt={profile.firstName}
                className="size-full object-cover"
              />
            ) : (
              <span className="text-3xl font-bold text-white select-none">
                {initials ?? "?"}
              </span>
            )}
          </div>

          {/* Hover overlay */}
          <div className="absolute inset-0 flex flex-col items-center justify-center gap-1 rounded-2xl bg-slate-900/50 opacity-0 transition-opacity group-hover:opacity-100">
            {isPending ? (
              <Loader2 className="size-6 animate-spin text-white" />
            ) : (
              <>
                <Camera className="size-6 text-white" />
                <span className="text-[11px] font-medium text-white">
                  Сменить
                </span>
              </>
            )}
          </div>
        </div>

        {/* Drop zone */}
        <div
          className={cn(
            "w-full cursor-pointer rounded-xl border-2 border-dashed p-4 text-center transition-colors",
            isDragging
              ? "border-primary bg-primary/5"
              : "border-border hover:border-primary hover:bg-primary/5",
          )}
          role="button"
          tabIndex={0}
          onClick={() => !isPending && fileInputRef.current?.click()}
          onKeyDown={(e) => {
            if ((e.key === "Enter" || e.key === " ") && !isPending) {
              fileInputRef.current?.click();
            }
          }}
          onDragOver={(e) => {
            e.preventDefault();
            setIsDragging(true);
          }}
          onDragLeave={() => setIsDragging(false)}
          onDrop={handleDrop}
        >
          <Upload className="text-muted-foreground mx-auto mb-1.5 size-6" />
          <p className="text-foreground text-xs font-medium">Загрузить фото</p>
          <p className="text-muted-foreground mt-0.5 text-[11px]">
            JPG, PNG · до 5 МБ
          </p>
        </div>

        {/* Delete */}
        {displayUrl && (
          <button
            type="button"
            className="text-destructive hover:text-destructive/80 text-xs font-medium transition-colors disabled:opacity-50"
            disabled={isPending}
            onClick={() => deleteMutation.mutate()}
          >
            Удалить фото
          </button>
        )}
      </div>

      <Input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/gif,image/webp"
        className="hidden"
        onChange={(e) => {
          handleFile(e.target.files?.[0] ?? null);
          if (fileInputRef.current) fileInputRef.current.value = "";
        }}
      />
    </div>
  );
}
