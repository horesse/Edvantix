// Lucide-style icons — stroke 2, rounded join. Inlined to avoid a CDN dep.
const Icon = ({d, size=20, stroke=2, fill='none', children, ...p}) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill={fill} stroke="currentColor"
       strokeWidth={stroke} strokeLinecap="round" strokeLinejoin="round"
       aria-hidden="true" {...p}>
    {d ? <path d={d}/> : children}
  </svg>
);

const IconMail = (p) => <Icon {...p}><rect width="20" height="16" x="2" y="4" rx="2"/><path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7"/></Icon>;
const IconLock = (p) => <Icon {...p}><rect width="18" height="11" x="3" y="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></Icon>;
const IconUser = (p) => <Icon {...p}><path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></Icon>;
const IconSchool = (p) => <Icon {...p}><path d="M22 10v6M2 10l10-5 10 5-10 5z"/><path d="M6 12v5c0 1.66 4 3 6 3s6-1.34 6-3v-5"/></Icon>;
const IconEye = (p) => <Icon {...p}><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7z"/><circle cx="12" cy="12" r="3"/></Icon>;
const IconEyeOff = (p) => <Icon {...p}><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" x2="22" y1="2" y2="22"/></Icon>;
const IconArrowRight = (p) => <Icon {...p}><path d="M5 12h14M12 5l7 7-7 7"/></Icon>;
const IconArrowLeft = (p) => <Icon {...p}><path d="M19 12H5M12 19l-7-7 7-7"/></Icon>;
const IconCheck = (p) => <Icon {...p}><path d="M20 6 9 17l-5-5"/></Icon>;
const IconCheckCircle = (p) => <Icon {...p}><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><path d="m9 11 3 3L22 4"/></Icon>;
const IconGoogle = ({size=18}) => (
  <svg width={size} height={size} viewBox="0 0 18 18" aria-hidden="true">
    <path fill="#4285F4" d="M17.64 9.2c0-.64-.06-1.25-.16-1.84H9v3.48h4.84a4.14 4.14 0 0 1-1.8 2.72v2.26h2.92c1.7-1.57 2.68-3.88 2.68-6.62z"/>
    <path fill="#34A853" d="M9 18c2.43 0 4.47-.8 5.96-2.18l-2.92-2.26c-.8.54-1.84.86-3.04.86-2.34 0-4.32-1.58-5.03-3.7H.98v2.32A9 9 0 0 0 9 18z"/>
    <path fill="#FBBC05" d="M3.97 10.72A5.41 5.41 0 0 1 3.68 9c0-.6.1-1.18.29-1.72V4.96H.98A9 9 0 0 0 0 9c0 1.45.35 2.82.98 4.04l2.99-2.32z"/>
    <path fill="#EA4335" d="M9 3.58c1.32 0 2.5.45 3.44 1.35l2.58-2.58A8.997 8.997 0 0 0 9 0 9 9 0 0 0 .98 4.96L3.97 7.28C4.68 5.16 6.66 3.58 9 3.58z"/>
  </svg>
);
const IconYandex = ({size=18}) => (
  <svg width={size} height={size} viewBox="0 0 18 18" aria-hidden="true">
    <circle cx="9" cy="9" r="9" fill="#FC3F1D"/>
    <path fill="#fff" d="M10.05 14.4h1.65V3.6H9.43c-2.32 0-3.54 1.2-3.54 2.95 0 1.4.66 2.22 1.83 3.06l-2.04 3.81-.04.08v.9h1.8l2.45-4.83-.83-.56c-.95-.65-1.4-1.15-1.4-2.23 0-.95.67-1.59 1.95-1.59h.45V14.4z"/>
  </svg>
);
const IconSparkles = (p) => <Icon {...p}><path d="m12 3-1.9 5.8a2 2 0 0 1-1.3 1.3L3 12l5.8 1.9a2 2 0 0 1 1.3 1.3L12 21l1.9-5.8a2 2 0 0 1 1.3-1.3L21 12l-5.8-1.9a2 2 0 0 1-1.3-1.3z"/><path d="M5 3v4M19 17v4M3 5h4M17 19h4"/></Icon>;
const IconShield = (p) => <Icon {...p}><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/><path d="m9 12 2 2 4-4"/></Icon>;

Object.assign(window, {
  IconMail, IconLock, IconUser, IconSchool, IconEye, IconEyeOff,
  IconArrowRight, IconArrowLeft, IconCheck, IconCheckCircle,
  IconGoogle, IconYandex, IconSparkles, IconShield,
});
