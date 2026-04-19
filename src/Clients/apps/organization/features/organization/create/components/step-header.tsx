interface StepHeaderProps {
  eyebrow: string;
  title: string;
  subtitle?: string;
}

export function StepHeader({ eyebrow, title, subtitle }: StepHeaderProps) {
  return (
    <div className="mb-7">
      <div className="text-brand-600 mb-2.5 text-[11px] font-semibold tracking-widest uppercase">
        {eyebrow}
      </div>
      <h2 className="text-foreground text-[26px] leading-tight font-bold tracking-tight">
        {title}
      </h2>
      {subtitle && (
        <p className="text-muted-foreground mt-2.5 max-w-[560px] text-[14.5px] leading-relaxed">
          {subtitle}
        </p>
      )}
    </div>
  );
}
