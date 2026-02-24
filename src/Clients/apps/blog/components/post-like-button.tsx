"use client";

import { useState } from "react";

import { Heart } from "lucide-react";

import useLikePost from "@workspace/api-hooks/blog/useLikePost";

import { signIn, useSession } from "@/lib/auth-client";

type PostLikeButtonProps = {
  postId: string;
  initialLikesCount: number;
  initialLiked?: boolean;
};

export function PostLikeButton({
  postId,
  initialLikesCount,
  initialLiked = false,
}: PostLikeButtonProps) {
  const { data: session } = useSession();
  const [liked, setLiked] = useState(initialLiked);
  const [optimisticCount, setOptimisticCount] = useState(initialLikesCount);
  const { mutate: toggleLike, isPending } = useLikePost();

  const handleLike = () => {
    if (!session) {
      void signIn.social({ provider: "keycloak" });
      return;
    }

    const newLiked = !liked;
    setLiked(newLiked);
    setOptimisticCount((c) => (newLiked ? c + 1 : c - 1));

    toggleLike(
      { postId, liked },
      {
        onError: () => {
          setLiked(liked);
          setOptimisticCount(initialLikesCount);
        },
      },
    );
  };

  return (
    <button
      onClick={handleLike}
      disabled={isPending}
      className={`group flex items-center gap-3 rounded-2xl border px-5 py-3 transition-all duration-200 disabled:cursor-not-allowed disabled:opacity-60 ${
        liked
          ? "border-red-500/30 bg-red-500/10 text-red-500 hover:bg-red-500/15"
          : "border-border bg-muted/50 text-muted-foreground hover:border-muted-foreground/30 hover:bg-muted hover:text-foreground"
      }`}
    >
      <Heart
        className={`h-5 w-5 transition-transform duration-200 ${
          liked ? "scale-110 fill-current" : "group-hover:scale-110"
        }`}
      />
      <span className="font-medium tabular-nums">{optimisticCount}</span>
      <span className="text-sm">{liked ? "Liked" : "Like this post"}</span>
    </button>
  );
}
