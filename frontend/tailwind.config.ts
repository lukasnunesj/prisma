import type { Config } from "tailwindcss";
import tailwindcssAnimate from "tailwindcss-animate";

export default {
  darkMode: ["class"],
  content: ["./pages/**/*.{ts,tsx}", "./components/**/*.{ts,tsx}", "./app/**/*.{ts,tsx}", "./src/**/*.{ts,tsx}"],
  prefix: "",
  theme: {
    container: {
      center: true,
      padding: "2rem",
      screens: {
        "2xl": "1400px",
      },
    },
    extend: {
      colors: {
        border: "oklch(from var(--border) l c h / <alpha-value>)",
        input: "oklch(from var(--input) l c h / <alpha-value>)",
        ring: "oklch(from var(--ring) l c h / <alpha-value>)",
        background: "oklch(from var(--background) l c h / <alpha-value>)",
        foreground: "oklch(from var(--foreground) l c h / <alpha-value>)",
        primary: {
          DEFAULT: "oklch(from var(--primary) l c h / <alpha-value>)",
          foreground: "oklch(from var(--primary-foreground) l c h / <alpha-value>)",
          light: "oklch(from var(--primary-light) l c h / <alpha-value>)",
          dark: "oklch(from var(--primary-dark) l c h / <alpha-value>)",
          glow: "oklch(from var(--primary-glow) l c h / <alpha-value>)",
        },
        secondary: {
          DEFAULT: "oklch(from var(--secondary) l c h / <alpha-value>)",
          foreground: "oklch(from var(--secondary-foreground) l c h / <alpha-value>)",
        },
        destructive: {
          DEFAULT: "oklch(from var(--destructive) l c h / <alpha-value>)",
          foreground: "oklch(from var(--destructive-foreground) l c h / <alpha-value>)",
          light: "oklch(from var(--destructive-light) l c h / <alpha-value>)",
        },
        muted: {
          DEFAULT: "oklch(from var(--muted) l c h / <alpha-value>)",
          foreground: "oklch(from var(--muted-foreground) l c h / <alpha-value>)",
        },
        accent: {
          DEFAULT: "oklch(from var(--accent) l c h / <alpha-value>)",
          foreground: "oklch(from var(--accent-foreground) l c h / <alpha-value>)",
        },
        popover: {
          DEFAULT: "oklch(from var(--popover) l c h / <alpha-value>)",
          foreground: "oklch(from var(--popover-foreground) l c h / <alpha-value>)",
        },
        card: {
          DEFAULT: "oklch(from var(--card) l c h / <alpha-value>)",
          foreground: "oklch(from var(--card-foreground) l c h / <alpha-value>)",
        },
        success: {
          DEFAULT: "oklch(from var(--success) l c h / <alpha-value>)",
          foreground: "oklch(from var(--success-foreground) l c h / <alpha-value>)",
          light: "oklch(from var(--success-light) l c h / <alpha-value>)",
        },
        warning: {
          DEFAULT: "oklch(from var(--warning) l c h / <alpha-value>)",
          foreground: "oklch(from var(--warning-foreground) l c h / <alpha-value>)",
          light: "oklch(from var(--warning-light) l c h / <alpha-value>)",
        },
        info: {
          DEFAULT: "oklch(from var(--info) l c h / <alpha-value>)",
          foreground: "oklch(from var(--info-foreground) l c h / <alpha-value>)",
          light: "oklch(from var(--info-light) l c h / <alpha-value>)",
        },
        sidebar: {
          DEFAULT: "oklch(from var(--sidebar) l c h / <alpha-value>)",
          foreground: "oklch(from var(--sidebar-foreground) l c h / <alpha-value>)",
          primary: "oklch(from var(--sidebar-primary) l c h / <alpha-value>)",
          "primary-foreground": "oklch(from var(--sidebar-primary-foreground) l c h / <alpha-value>)",
          accent: "oklch(from var(--sidebar-accent) l c h / <alpha-value>)",
          "accent-foreground": "oklch(from var(--sidebar-accent-foreground) l c h / <alpha-value>)",
          border: "oklch(from var(--sidebar-border) l c h / <alpha-value>)",
          ring: "oklch(from var(--sidebar-ring) l c h / <alpha-value>)",
        },
        chart: {
          "1": "oklch(from var(--chart-1) l c h / <alpha-value>)",
          "2": "oklch(from var(--chart-2) l c h / <alpha-value>)",
          "3": "oklch(from var(--chart-3) l c h / <alpha-value>)",
          "4": "oklch(from var(--chart-4) l c h / <alpha-value>)",
          "5": "oklch(from var(--chart-5) l c h / <alpha-value>)",
          "6": "oklch(from var(--chart-6) l c h / <alpha-value>)",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
      boxShadow: {
        sm: "var(--shadow-sm)",
        md: "var(--shadow-md)",
        lg: "var(--shadow-lg)",
        glow: "var(--shadow-glow)",
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
      },
      keyframes: {
        "accordion-down": {
          from: { height: "0" },
          to: { height: "var(--radix-accordion-content-height)" },
        },
        "accordion-up": {
          from: { height: "var(--radix-accordion-content-height)" },
          to: { height: "0" },
        },
        "fade-in": {
          from: { opacity: "0", transform: "translateY(10px)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
        "slide-in-bottom": {
          from: { opacity: "0", transform: "translateY(20px)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
        "scale-in": {
          from: { opacity: "0", transform: "scale(0.95)" },
          to: { opacity: "1", transform: "scale(1)" },
        },
        "pulse-glow": {
          "0%, 100%": { boxShadow: "0 0 5px oklch(from var(--primary) l c h / 0.2)" },
          "50%": { boxShadow: "0 0 20px oklch(from var(--primary) l c h / 0.4)" },
        },
        "highlight-flash": {
          "0%": { backgroundColor: "oklch(from var(--success) l c h / 0.2)" },
          "100%": { backgroundColor: "transparent" },
        },
      },
      animation: {
        "accordion-down": "accordion-down 0.2s ease-out",
        "accordion-up": "accordion-up 0.2s ease-out",
        "fade-in": "fade-in 0.3s ease-out forwards",
        "slide-in-bottom": "slide-in-bottom 0.4s ease-out forwards",
        "scale-in": "scale-in 0.2s ease-out forwards",
        "pulse-glow": "pulse-glow 2s ease-in-out infinite",
        "highlight-flash": "highlight-flash 0.5s ease-out",
      },
    },
  },
  plugins: [tailwindcssAnimate],
} satisfies Config;
