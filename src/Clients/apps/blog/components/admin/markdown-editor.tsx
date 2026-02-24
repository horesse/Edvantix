"use client";

import { useCallback, useRef, useState } from "react";

import {
  Bold,
  Code2,
  Heading2,
  Heading3,
  Image,
  Italic,
  Link2,
  List,
  ListOrdered,
  Minus,
  Quote,
  Strikethrough,
} from "lucide-react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

import { Textarea } from "@workspace/ui/components/textarea";

type EditorMode = "write" | "split" | "preview";

type MarkdownEditorProps = {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  minRows?: number;
};

/** Inserts markdown syntax around the current textarea selection. */
function applyMarkdown(
  textarea: HTMLTextAreaElement,
  prefix: string,
  suffix = "",
  placeholder = "text",
): string {
  const { selectionStart: start, selectionEnd: end, value } = textarea;
  const selected = value.substring(start, end) || placeholder;
  return (
    value.substring(0, start) +
    prefix +
    selected +
    suffix +
    value.substring(end)
  );
}

const TOOLBAR = [
  {
    icon: Bold,
    label: "Bold (Ctrl+B)",
    prefix: "**",
    suffix: "**",
    placeholder: "bold text",
  },
  {
    icon: Italic,
    label: "Italic (Ctrl+I)",
    prefix: "_",
    suffix: "_",
    placeholder: "italic text",
  },
  {
    icon: Strikethrough,
    label: "Strikethrough",
    prefix: "~~",
    suffix: "~~",
    placeholder: "strikethrough",
  },
  null, // separator
  {
    icon: Heading2,
    label: "Heading 2",
    prefix: "## ",
    suffix: "",
    placeholder: "Heading",
  },
  {
    icon: Heading3,
    label: "Heading 3",
    prefix: "### ",
    suffix: "",
    placeholder: "Heading",
  },
  null,
  {
    icon: Link2,
    label: "Link",
    prefix: "[",
    suffix: "](url)",
    placeholder: "link text",
  },
  {
    icon: Image,
    label: "Image",
    prefix: "![",
    suffix: "](url)",
    placeholder: "alt text",
  },
  {
    icon: Code2,
    label: "Inline code",
    prefix: "`",
    suffix: "`",
    placeholder: "code",
  },
  null,
  {
    icon: Quote,
    label: "Blockquote",
    prefix: "> ",
    suffix: "",
    placeholder: "quote",
  },
  {
    icon: List,
    label: "Unordered list",
    prefix: "- ",
    suffix: "",
    placeholder: "item",
  },
  {
    icon: ListOrdered,
    label: "Ordered list",
    prefix: "1. ",
    suffix: "",
    placeholder: "item",
  },
  {
    icon: Minus,
    label: "Horizontal rule",
    prefix: "\n---\n",
    suffix: "",
    placeholder: "",
  },
] as const;

/**
 * A custom split-view Markdown editor with a formatting toolbar and live preview.
 * Supports Write / Split / Preview modes.
 */
export function MarkdownEditor({
  value,
  onChange,
  placeholder = "Write your post in Markdown…",
  minRows = 20,
}: MarkdownEditorProps) {
  const [mode, setMode] = useState<EditorMode>("split");
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const applyToolbar = useCallback(
    (prefix: string, suffix: string, placeholder: string) => {
      const textarea = textareaRef.current;
      if (!textarea) return;
      const newValue = applyMarkdown(textarea, prefix, suffix, placeholder);
      onChange(newValue);
      // Restore focus after React re-renders
      requestAnimationFrame(() => textarea.focus());
    },
    [onChange],
  );

  const minHeight = `${minRows * 1.5}rem`;

  return (
    <div className="border-border bg-card overflow-hidden rounded-xl border shadow-sm">
      {/* ── Toolbar ── */}
      <div className="border-border bg-muted/30 flex flex-wrap items-center gap-0.5 border-b px-2 py-2">
        {TOOLBAR.map((item, i) =>
          item === null ? (
            // separator
            <div key={`sep-${i}`} className="bg-border mx-1.5 h-5 w-px" />
          ) : (
            <button
              key={item.label}
              type="button"
              title={item.label}
              onClick={() =>
                applyToolbar(item.prefix, item.suffix, item.placeholder)
              }
              className="text-muted-foreground hover:text-foreground hover:bg-accent inline-flex h-7 w-7 items-center justify-center rounded-md transition-colors"
            >
              <item.icon className="h-3.5 w-3.5" />
            </button>
          ),
        )}

        {/* Mode switcher — right-aligned */}
        <div className="border-border bg-background ml-auto flex items-center gap-1 rounded-lg border p-0.5">
          {(["write", "split", "preview"] as EditorMode[]).map((m) => (
            <button
              key={m}
              type="button"
              onClick={() => setMode(m)}
              className={`rounded-md px-2.5 py-1 text-xs font-medium capitalize transition-all duration-150 ${
                mode === m
                  ? "bg-primary text-primary-foreground shadow-sm"
                  : "text-muted-foreground hover:text-foreground"
              }`}
            >
              {m}
            </button>
          ))}
        </div>
      </div>

      {/* ── Editor / Preview panes ── */}
      <div
        className={`divide-border flex ${mode === "split" ? "divide-x" : ""}`}
        style={{ minHeight }}
      >
        {/* Write pane */}
        {(mode === "write" || mode === "split") && (
          <div
            className={`flex flex-col ${mode === "split" ? "w-1/2" : "w-full"}`}
          >
            <Textarea
              ref={textareaRef}
              value={value}
              onChange={(e) => onChange(e.target.value)}
              placeholder={placeholder}
              className="flex-1 resize-none rounded-none border-0 bg-transparent p-4 font-mono text-sm leading-relaxed ring-0 focus-visible:ring-0"
              style={{ minHeight }}
            />
          </div>
        )}

        {/* Preview pane */}
        {(mode === "preview" || mode === "split") && (
          <div
            className={`overflow-auto p-4 ${mode === "split" ? "w-1/2" : "w-full"}`}
            style={{ minHeight }}
          >
            {value.trim() ? (
              <div className="prose prose-neutral dark:prose-invert prose-headings:font-bold prose-headings:tracking-tight prose-a:text-primary prose-a:no-underline hover:prose-a:underline prose-code:text-primary prose-pre:bg-muted prose-pre:border prose-pre:border-border max-w-none text-sm">
                <ReactMarkdown remarkPlugins={[remarkGfm]}>
                  {value}
                </ReactMarkdown>
              </div>
            ) : (
              <div className="flex h-full items-center justify-center">
                <p className="text-muted-foreground text-sm">
                  Preview will appear here…
                </p>
              </div>
            )}
          </div>
        )}
      </div>

      {/* ── Footer ── */}
      <div className="border-border bg-muted/20 text-muted-foreground flex items-center justify-between border-t px-4 py-2 text-xs">
        <span>Markdown supported · GFM enabled</span>
        <span className="font-mono">{value.length.toLocaleString()} chars</span>
      </div>
    </div>
  );
}
