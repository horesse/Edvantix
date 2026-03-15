"use client";

import { useRef, useState } from "react";

import { Upload } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

import { AVATAR_OPTIONS } from "./schema";

interface AvatarPickerProps {
  value: string | null;
  uploadedDataUrl: string | null;
  onPresetSelect: (value: string) => void;
  onFileSelect: (dataUrl: string) => void;
}

/**
 * Avatar picker with preset emoji options and a file upload zone.
 * Selecting a preset clears any uploaded photo and vice-versa.
 */
export function AvatarPicker({
  value,
  uploadedDataUrl,
  onPresetSelect,
  onFileSelect,
}: AvatarPickerProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [isDragging, setIsDragging] = useState(false);

  function handleFile(file: File) {
    if (!file.type.startsWith("image/")) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      const result = e.target?.result;
      if (typeof result === "string") {
        onFileSelect(result);
      }
    };
    reader.readAsDataURL(file);
  }

  function handleInputChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (file) handleFile(file);
  }

  function handleDrop(e: React.DragEvent) {
    e.preventDefault();
    setIsDragging(false);
    const file = e.dataTransfer.files[0];
    if (file) handleFile(file);
  }

  return (
    <div className="space-y-5">
      {/* Preset emoji grid */}
      <div className="grid grid-cols-5 gap-3">
        {AVATAR_OPTIONS.map((opt) => {
          const isSelected = value === opt.value && !uploadedDataUrl;
          return (
            <button
              key={opt.value}
              type="button"
              onClick={() => onPresetSelect(opt.value)}
              className="flex flex-col items-center"
              aria-pressed={isSelected}
            >
              <div
                className={cn(
                  "flex size-[68px] items-center justify-center rounded-full border-2 p-0.5 transition-all",
                  isSelected
                    ? "border-primary shadow-[0_0_0_3px_hsl(var(--primary)/0.15)]"
                    : "hover:border-primary/30 border-transparent",
                )}
              >
                <div
                  className="flex size-full items-center justify-center rounded-full text-[26px] select-none"
                  style={{ background: opt.bg }}
                >
                  {opt.emoji}
                </div>
              </div>
            </button>
          );
        })}
      </div>

      {/* Divider */}
      <div className="relative">
        <div className="absolute inset-0 flex items-center">
          <div className="border-border w-full border-t" />
        </div>
        <div className="relative flex justify-center">
          <span className="bg-card text-muted-foreground px-3 text-xs">
            или загрузите фото
          </span>
        </div>
      </div>

      {/* Upload zone */}
      <label
        className={cn(
          "border-border flex cursor-pointer flex-col items-center gap-2 rounded-xl border-2 border-dashed p-5 transition-colors",
          isDragging
            ? "border-primary bg-primary/5"
            : "hover:border-primary/50 hover:bg-muted/50",
        )}
        onDragOver={(e) => {
          e.preventDefault();
          setIsDragging(true);
        }}
        onDragLeave={() => setIsDragging(false)}
        onDrop={handleDrop}
      >
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          className="hidden"
          onChange={handleInputChange}
        />

        {uploadedDataUrl ? (
          <>
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={uploadedDataUrl}
              alt="Предпросмотр"
              className="border-primary size-16 rounded-full border-2 object-cover"
            />
            <span className="text-primary text-xs font-medium">
              Фото выбрано · нажмите, чтобы заменить
            </span>
          </>
        ) : (
          <>
            <div className="bg-muted flex size-10 items-center justify-center rounded-full">
              <Upload className="text-muted-foreground size-5" />
            </div>
            <span className="text-muted-foreground text-sm">
              Нажмите для загрузки или перетащите файл
            </span>
            <span className="text-muted-foreground/60 text-xs">
              PNG, JPG до 5 МБ
            </span>
          </>
        )}
      </label>
    </div>
  );
}
