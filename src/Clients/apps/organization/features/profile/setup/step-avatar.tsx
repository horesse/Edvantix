"use client";

import { ChevronRight } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

import { AvatarPicker } from "./avatar-picker";
import { AVATAR_OPTIONS } from "./schema";
import type { AvatarStepValues } from "./schema";

interface StepAvatarProps {
  values: AvatarStepValues;
  uploadedDataUrl: string | null;
  onChange: (values: AvatarStepValues) => void;
  onUploadChange: (file: File | null, dataUrl: string | null) => void;
  onNext: () => void;
}

/**
 * Step 1 of profile setup: choose a preset avatar or upload a photo.
 */
export function StepAvatar({
  values,
  uploadedDataUrl,
  onChange,
  onUploadChange,
  onNext,
}: StepAvatarProps) {
  function handlePresetSelect(presetValue: string) {
    onUploadChange(null, null);
    onChange({ avatarType: "preset", presetValue, uploadedDataUrl: undefined });
  }

  function handleFileSelect(file: File, dataUrl: string) {
    onUploadChange(file, dataUrl);
    onChange({
      avatarType: "upload",
      presetValue: undefined,
      uploadedDataUrl: dataUrl,
    });
  }

  return (
    <div className="p-7">
      <h2 className="text-foreground mb-1 text-base font-semibold">
        Выберите аватар
      </h2>
      <p className="text-muted-foreground mb-6 text-sm">
        Выберите готовый или загрузите своё фото
      </p>

      <AvatarPicker
        value={values.presetValue ?? AVATAR_OPTIONS[0].value}
        uploadedDataUrl={uploadedDataUrl}
        onPresetSelect={handlePresetSelect}
        onFileSelect={handleFileSelect}
      />

      <div className="mt-7">
        <Button className="w-full" onClick={onNext}>
          Далее
          <ChevronRight className="ml-1 size-4" />
        </Button>
      </div>
    </div>
  );
}
