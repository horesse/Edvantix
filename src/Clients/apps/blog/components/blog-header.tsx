"use client";

import { useState } from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { LogIn, LogOut, Menu, Settings, Sparkles, X } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";

import { useIsAdmin } from "@/hooks/use-realm-roles";
import { signIn, signOut, useSession } from "@/lib/auth-client";

import { ThemeToggle } from "./theme-toggle";

const navLinks = [
  { href: "/", label: "Blog" },
  { href: "/category", label: "Categories" },
];

export function BlogHeader() {
  const pathname = usePathname();
  const { data: session } = useSession();
  const [mobileOpen, setMobileOpen] = useState(false);
  const isAdmin = useIsAdmin();

  const handleSignIn = () => {
    void signIn.social({ provider: "keycloak" });
  };

  const handleSignOut = () => {
    void signOut();
  };

  return (
    <header className="border-border/60 bg-background/80 supports-[backdrop-filter]:bg-background/70 sticky top-0 z-50 border-b backdrop-blur-xl">
      <div className="mx-auto flex h-14 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        {/* Logo */}
        <Link
          href="/"
          className="group flex items-center gap-2 transition-opacity hover:opacity-80"
        >
          <div className="flex items-center gap-1.5">
            <Sparkles className="text-primary h-4 w-4 transition-transform duration-300 group-hover:rotate-12" />
            <span className="text-primary text-lg font-bold tracking-tight">
              Edvantix
            </span>
          </div>
          <span className="text-muted-foreground border-border hidden border-l pl-2 text-sm font-normal sm:inline">
            Blog
          </span>
        </Link>

        {/* Desktop nav */}
        <nav className="hidden items-center gap-1 md:flex">
          {navLinks.map((link) => {
            const isActive = pathname === link.href;
            return (
              <Link
                key={link.href}
                href={link.href}
                className={`relative rounded-md px-3 py-1.5 text-sm font-medium transition-colors ${
                  isActive
                    ? "text-foreground bg-accent"
                    : "text-muted-foreground hover:text-foreground hover:bg-accent/50"
                }`}
              >
                {link.label}
                {isActive && (
                  <span className="bg-primary absolute inset-x-3 -bottom-px h-0.5 rounded-full" />
                )}
              </Link>
            );
          })}
        </nav>

        {/* Right side */}
        <div className="flex items-center gap-1.5">
          <ThemeToggle />

          {session ? (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="ghost"
                  size="sm"
                  className="gap-2 rounded-full pr-1 pl-2"
                >
                  <span className="hidden text-sm sm:inline">
                    {session.user.name}
                  </span>
                  {/* Avatar initials */}
                  <div className="bg-primary/15 text-primary ring-primary/20 flex h-7 w-7 items-center justify-center rounded-full text-xs font-semibold ring-2">
                    {session.user.name?.charAt(0).toUpperCase()}
                  </div>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-48">
                {isAdmin && (
                  <>
                    <DropdownMenuItem asChild>
                      <Link href="/admin" className="flex items-center gap-2">
                        <Settings className="h-4 w-4" />
                        Admin Panel
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                  </>
                )}
                <DropdownMenuItem
                  onClick={handleSignOut}
                  className="text-destructive focus:text-destructive flex items-center gap-2"
                >
                  <LogOut className="h-4 w-4" />
                  Sign out
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            <Button
              variant="default"
              size="sm"
              className="gap-2 rounded-full"
              onClick={handleSignIn}
            >
              <LogIn className="h-4 w-4" />
              <span className="hidden sm:inline">Sign in</span>
            </Button>
          )}

          {/* Mobile menu toggle */}
          <Button
            variant="ghost"
            size="icon"
            className="rounded-full md:hidden"
            onClick={() => setMobileOpen((v) => !v)}
            aria-label="Toggle mobile menu"
          >
            {mobileOpen ? (
              <X className="h-4 w-4" />
            ) : (
              <Menu className="h-4 w-4" />
            )}
          </Button>
        </div>
      </div>

      {/* Mobile nav — slides down */}
      {mobileOpen && (
        <div className="border-border/60 animate-in slide-in-from-top-2 border-t duration-200 md:hidden">
          <nav className="flex flex-col gap-1 px-4 py-3">
            {navLinks.map((link) => (
              <Link
                key={link.href}
                href={link.href}
                className={`hover:bg-accent rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
                  pathname === link.href
                    ? "text-foreground bg-accent"
                    : "text-muted-foreground"
                }`}
                onClick={() => setMobileOpen(false)}
              >
                {link.label}
              </Link>
            ))}
            {isAdmin && (
              <Link
                href="/admin"
                className="hover:bg-accent text-muted-foreground rounded-lg px-3 py-2 text-sm font-medium transition-colors"
                onClick={() => setMobileOpen(false)}
              >
                Admin Panel
              </Link>
            )}
          </nav>
        </div>
      )}
    </header>
  );
}
