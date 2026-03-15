"use client";

import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import type { OwnProfileDetails } from "@workspace/types/profile";
import { Textarea } from "@workspace/ui/components/textarea";

import type { BioSectionHandle } from "../types";

const MAX_BIO = 600;

export const BioSection = forwardRef<
  BioSectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function BioSection({ profile, onDirtyChange }, ref) {
  const [bio, setBio] = useState(profile.bio ?? "");
  const [savedBio, setSavedBio] = useState(profile.bio ?? "");

  const isDirty = bio !== savedBio;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    getPayload(): string | null {
      return bio || null;
    },
    acknowledgeServerState() {
      setSavedBio(bio);
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
