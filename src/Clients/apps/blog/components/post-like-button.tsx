"use client";

import { useState } from "react";

import { Heart } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import useLikePost from "@workspace/api-hooks/blog/useLikePost";

import { useSession } from "@/lib/auth-client";
import { signIn } from "@/lib/auth-client";

type PostLikeButtonProps = {
  postId: number;
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
    <Button
      variant={liked ? "default" : "outline"}
      size="sm"
      className="gap-2"
      onClick={handleLike}
      disabled={isPending}
    >
      <Heart
        className={`h-4 w-4 transition-transform ${liked ? "fill-current scale-110" : ""}`}
      />
      <span>{optimisticCount}</span>
    </Button>
  );
}
