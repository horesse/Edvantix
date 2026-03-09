"use client";

import { useState, useEffect } from "react";

import Link from "next/link";
import { Menu, X, GraduationCap, ChevronRight } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

const NAV_LINKS = [
  { label: "Возможности", href: "#features" },
  { label: "Как это работает", href: "#how-it-works" },
  { label: "Тарифы", href: "#pricing" },
  { label: "Отзывы", href: "#testimonials" },
] as const;

export function Navbar() {
  const [scrolled, setScrolled] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 16);
    };

    window.addEventListener("scroll", handleScroll, { passive: true });

    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, []);

  return (
    <header
      className={`fixed top-0 left-0 right-0 z-50 transition-all duration-500 ${
        scrolled
          ? "bg-background/90 backdrop-blur-xl border-b border-border shadow-lg shadow-background/50"
          : "bg-transparent"
      }`}
    >
      {/* Skip to content */}
      <a
        href="#main-content"
        className="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-primary text-primary-foreground px-4 py-2 rounded-md text-sm z-50"
      >
        Перейти к содержимому
      </a>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link
            href="/"
            className="flex items-center gap-2.5 group focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring rounded-md"
            aria-label="Edvantix — на главную"
          >
            <div className="w-8 h-8 rounded-lg bg-primary flex items-center justify-center shadow-lg shadow-primary/30 group-hover:shadow-primary/50 transition-all duration-300 group-hover:scale-105">
              <GraduationCap className="w-4 h-4 text-primary-foreground" aria-hidden="true" />
            </div>
            <span className="font-bold text-card-foreground text-lg tracking-tight">
              Edv<span className="text-primary">antix</span>
            </span>
          </Link>

          {/* Desktop navigation */}
          <nav
            className="hidden md:flex items-center gap-1"
            aria-label="Основная навигация"
          >
            {NAV_LINKS.map((link) => (
              <Link
                key={link.href}
                href={link.href}
                className="px-4 py-2 text-sm text-muted-foreground hover:text-foreground transition-colors duration-200 rounded-md hover:bg-accent focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              >
                {link.label}
              </Link>
            ))}
          </nav>

          {/* Desktop CTA */}
          <div className="hidden md:flex items-center gap-3">
            <Link
              href="/login"
              className="text-sm text-muted-foreground hover:text-foreground transition-colors px-4 py-2 rounded-md hover:bg-accent focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              Войти
            </Link>
            <Button
              asChild
              className="bg-primary hover:bg-primary/90 text-primary-foreground shadow-lg shadow-primary/20 hover:shadow-primary/30 transition-all duration-200 focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background"
            >
              <Link href="/signup">
                Начать бесплатно
                <ChevronRight className="w-3.5 h-3.5 ml-0.5" aria-hidden="true" />
              </Link>
            </Button>
          </div>

          {/* Mobile menu button */}
          <button
            className="md:hidden p-2 text-muted-foreground hover:text-foreground rounded-md hover:bg-accent transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            onClick={() => setMenuOpen(!menuOpen)}
            aria-label={menuOpen ? "Закрыть меню" : "Открыть меню"}
            aria-expanded={menuOpen}
            aria-controls="mobile-menu"
            type="button"
          >
            {menuOpen
              ? <X className="w-5 h-5" aria-hidden="true" />
              : <Menu className="w-5 h-5" aria-hidden="true" />}
          </button>
        </div>

        {/* Mobile menu */}
        <div
          id="mobile-menu"
          className={`md:hidden overflow-hidden transition-all duration-300 ${
            menuOpen ? "max-h-96 opacity-100" : "max-h-0 opacity-0"
          }`}
          aria-hidden={!menuOpen}
        >
          <div className="pb-4 border-t border-border mt-1">
            <nav
              className="flex flex-col gap-1 pt-3"
              aria-label="Мобильная навигация"
            >
              {NAV_LINKS.map((link) => (
                <Link
                  key={link.href}
                  href={link.href}
                  className="px-4 py-2.5 text-sm text-muted-foreground hover:text-foreground transition-colors rounded-md hover:bg-accent focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                  onClick={() => setMenuOpen(false)}
                >
                  {link.label}
                </Link>
              ))}
              <div className="flex flex-col gap-2 mt-3 px-1">
                <Link
                  href="/login"
                  className="text-sm text-muted-foreground hover:text-foreground transition-colors px-4 py-2.5 rounded-md hover:bg-accent text-center"
                  onClick={() => setMenuOpen(false)}
                >
                  Войти
                </Link>
                <Button
                  asChild
                  className="bg-primary hover:bg-primary/90 text-primary-foreground w-full"
                >
                  <Link href="/signup" onClick={() => setMenuOpen(false)}>
                    Начать бесплатно
                  </Link>
                </Button>
              </div>
            </nav>
          </div>
        </div>
      </div>
    </header>
  );
}
