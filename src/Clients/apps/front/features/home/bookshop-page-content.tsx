"use client";

import { useRef } from "react";

import { ChatBot, type ChatBotRef } from "@/components/chat-bot";
import AiRecommendationsSection from "@/features/home/ai-recommendations-section";
import HeroSection from "@/features/home/hero-section";

export default function BookshopPageContent() {
  const chatBotRef = useRef<ChatBotRef>(null);

  return (
    <main id="main-content">
      <HeroSection />
      {/*<AiRecommendationsSection chatBotRef={chatBotRef} />*/}
      {/*<ChatBot ref={chatBotRef} />*/}
    </main>
  );
}
