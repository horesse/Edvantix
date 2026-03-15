"use client";

import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import { toast } from "sonner";

import useUpdateBio from "@workspace/api-hooks/profiles/useUpdateBio";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Textarea } from "@workspace/ui/components/textarea";

import type { SectionHandle } from "../types";

const MAX_BIO = 600;

export const BioSection = forwardRef<
  SectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function BioSection({ profile, onDirtyChange }, ref) {
  const [bio, setBio] = useState(profile.bio ?? "");
  const [savedBio, setSavedBio] = useState(profile.bio ?? "");

  const mutation = useUpdateBio({
    onSuccess: (data) => {
      toast.success("О себе сохранено");
      setSavedBio(data.bio ?? "");
    },
    onError: () => toast.error("Не удалось сохранить"),
  });

  const isDirty = bio !== savedBio;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    submit: () => {
      if (isDirty) mutation.mutate({ bio: bio || null });
    },
  }));

  return (
    <div className="relative">
      <Textarea
        rows={4}
        maxLength={MAX_BIO}
        placeholder="Расскажите о своём опыте, специализации и достижениях…"
        className="resize-none pb-7 leading-relaxed"
        value={bio}
        onChange={(e) => setBio(e.target.value)}
      />
      <span className="text-muted-foreground pointer-events-none absolute right-4 bottom-3 text-[11px]">
        {bio.length}/{MAX_BIO}
      </span>
    </div>
  );
});
