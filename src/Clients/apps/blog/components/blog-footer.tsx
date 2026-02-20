import Link from "next/link";

export function BlogFooter() {
  return (
    <footer className="border-t border-border mt-auto">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-2">
            <span className="text-primary font-semibold">Edvantix</span>
            <span className="text-muted-foreground text-sm">Blog</span>
          </div>
          <nav className="flex flex-wrap gap-6 justify-center">
            <Link
              href="/"
              className="text-sm text-muted-foreground hover:text-foreground transition-colors"
            >
              Blog
            </Link>
            <Link
              href="/category"
              className="text-sm text-muted-foreground hover:text-foreground transition-colors"
            >
              Categories
            </Link>
          </nav>
          <p className="text-xs text-muted-foreground">
            &copy; {new Date().getFullYear()} Edvantix. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
}
