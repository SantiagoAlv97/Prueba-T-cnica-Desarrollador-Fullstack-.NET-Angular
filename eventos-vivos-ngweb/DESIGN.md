---
name: High-Efficiency Event Interface
colors:
  surface: '#f9f9f9'
  surface-dim: '#dadada'
  surface-bright: '#f9f9f9'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f3f3'
  surface-container: '#eeeeee'
  surface-container-high: '#e8e8e8'
  surface-container-highest: '#e2e2e2'
  on-surface: '#1a1c1c'
  on-surface-variant: '#4c4546'
  inverse-surface: '#2f3131'
  inverse-on-surface: '#f1f1f1'
  outline: '#7e7576'
  outline-variant: '#cfc4c5'
  surface-tint: '#5e5e5e'
  primary: '#000000'
  on-primary: '#ffffff'
  primary-container: '#1b1b1b'
  on-primary-container: '#848484'
  inverse-primary: '#c6c6c6'
  secondary: '#0052d1'
  on-secondary: '#ffffff'
  secondary-container: '#346deb'
  on-secondary-container: '#fefcff'
  tertiary: '#000000'
  on-tertiary: '#ffffff'
  tertiary-container: '#1b1b1b'
  on-tertiary-container: '#848484'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#e2e2e2'
  primary-fixed-dim: '#c6c6c6'
  on-primary-fixed: '#1b1b1b'
  on-primary-fixed-variant: '#474747'
  secondary-fixed: '#dbe1ff'
  secondary-fixed-dim: '#b3c5ff'
  on-secondary-fixed: '#001849'
  on-secondary-fixed-variant: '#003fa4'
  tertiary-fixed: '#e2e2e2'
  tertiary-fixed-dim: '#c6c6c6'
  on-tertiary-fixed: '#1b1b1b'
  on-tertiary-fixed-variant: '#474747'
  background: '#f9f9f9'
  on-background: '#1a1c1c'
  surface-variant: '#e2e2e2'
  deep-navy: '#003180'
  electric-blue: '#0052D1'
  surface-gray: '#F2F2F2'
typography:
  display:
    fontFamily: Hanken Grotesk
    fontSize: 48px
    fontWeight: '800'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Hanken Grotesk
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Hanken Grotesk
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
  headline-md:
    fontFamily: Hanken Grotesk
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Hanken Grotesk
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Hanken Grotesk
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-bold:
    fontFamily: Hanken Grotesk
    fontSize: 14px
    fontWeight: '700'
    lineHeight: 20px
  label-sm:
    fontFamily: Hanken Grotesk
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
spacing:
  unit: 8px
  container-max: 1200px
  gutter: 24px
  margin-desktop: 48px
  margin-mobile: 16px
---

## Brand & Style

This design system is built on the philosophy of **Functional Minimalism**. It prioritizes clarity of information and speed of transaction over decorative elements. Inspired by the utility-first aesthetic of ticketing platforms, it utilizes a high-contrast environment to guide the user's eye toward calls to action and event details. 

The target audience consists of event-goers who value efficiency and professional reliability. The emotional response should be one of "effortless utility"—the interface stays out of the way, providing a clean, structured stage for the content (events) to shine. The visual style is **Corporate / Modern** with a lean toward **High-Contrast**, emphasizing grid alignment and intentional whitespace.

The UI must be implemented with a Tailwind-first structure, using utility classes for layout, spacing, responsiveness, borders, typography, and component composition while preserving the functional minimalist visual system.

## Colors

The palette is strictly limited to ensure a high-signal environment. **Black (#000000)** is the primary driver of structure, used for typography and high-priority UI boundaries. **Electric Blue (#0052D1)** serves as the sole functional accent, reserved exclusively for interactive elements and primary conversion paths.

**Surface Gray (#F2F2F2)** provides subtle tonal separation for background containers, preventing the interface from feeling overly stark. Use white (#FFFFFF) for the main canvas and primary card backgrounds to maintain a "breathable" aesthetic. Text should always adhere to high accessibility standards against the background.

## Typography

This design system uses **Hanken Grotesk** across all levels to maintain a cohesive, modern, and highly legible appearance. It replaces the reference font with a sharper, more contemporary sans-serif that excels in both large-scale display and small-scale functional labels.

Headlines use heavy weights (700-800) and tight letter-spacing to create a bold, editorial feel. Body text remains clean and neutral. Labels for categories or dates should often use the `label-bold` style with uppercase transformations to provide immediate visual anchors within the scan-heavy event listings.

## Layout & Spacing

The design system employs a **Fixed Grid** model for desktop to ensure content remains focused and legible, transitioning to a fluid model for mobile devices. 

- **Desktop:** 12-column grid, 1200px max-width, 24px gutters.
- **Tablet:** 8-column grid, fluid width, 24px margins.
- **Mobile:** 4-column grid, fluid width, 16px margins.

Spacing is governed by an 8px base unit. Generous whitespace is a requirement; components should be separated by large vertical gaps (64px+) to distinguish different sections of the event discovery experience.

## Tailwind CSS Structure

The interface structure must be implemented using Tailwind CSS as the primary utility-first framework for layout, spacing, alignment, responsiveness, and component composition. Tailwind should be used to translate the design system into consistent, reusable UI patterns without relying on excessive custom CSS.

Use Tailwind utilities for:

Grid and flexbox layouts.
Responsive behavior across desktop, tablet, and mobile.
Container sizing and max-width control.
Spacing based on the 8px system.
Typography sizing, weights, and line-height.
Borders, backgrounds, and interaction states.
Form layouts, cards, filters, buttons, and admin tables.

The layout should follow a Tailwind-first approach:

Use max-w-[1200px] mx-auto px-4 md:px-6 lg:px-12 for main containers.
Use grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 for event listing cards.
Use flex, grid, gap-*, space-y-*, and divide-* utilities for internal structure.
Use border, border-black/30, border-[#D1D1D1], and bg-[#F2F2F2] instead of shadows.
Use sharp corners with rounded-none across cards, buttons, inputs, filters, and modal containers.
Use responsive utilities such as sm:, md:, lg:, and xl: to adapt layouts cleanly.

Custom CSS should be limited to global design tokens, font loading, and reusable component-level refinements only when Tailwind utilities are not enough. The default implementation should favor Tailwind utility classes directly in Angular templates.

Recommended Tailwind structure:

<section class="bg-white px-4 py-16 md:px-6 lg:px-12">
  <div class="mx-auto max-w-[1200px]">
    <div class="mb-10 flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
      <div>
        <p class="text-xs font-bold uppercase tracking-wide text-[#0052D1]">
          Eventos disponibles
        </p>
        <h1 class="mt-2 text-3xl font-bold tracking-tight text-black md:text-5xl">
          Encuentra tu próximo evento
        </h1>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
      <!-- Event cards -->
    </div>
  </div>
</section>

Tailwind should support the design system, not override it. The visual result must remain minimal, high-contrast, sharp, structured, and highly readable.

## Elevation & Depth

To maintain a minimalist aesthetic, this design system avoids traditional shadows. Instead, it utilizes **Tonal Layers** and **Low-Contrast Outlines**.

- **Level 0 (Background):** White (#FFFFFF) or Surface Gray (#F2F2F2).
- **Level 1 (Cards/Sections):** White background with a 1px solid border in Surface Gray (#F2F2F2) or a very thin Neutral Gray (#D1D1D1).
- **Active State:** Use a 2px Black (#000000) border to indicate focus or selection.

Depth is achieved through the stacking of containers rather than Z-axis shadows, ensuring a flat, modern, and high-performance feel.

## Shapes

The design system adopts a **Sharp (0)** roundedness profile. All containers, buttons, and input fields utilize 90-degree corners. This architectural approach reinforces the "functional" and "precise" nature of the brand, moving away from the soft, consumer-app trends and toward a more professional, industrial aesthetic.

## Components

### Buttons
- **Primary:** Solid Black (#000000) background, White text, sharp corners. High-impact for "Buy Tickets."
- **Secondary:** Solid Electric Blue (#0052D1) background, White text. Used for "View Details."
- **Ghost:** 1px Black border, no background. Used for filter actions.

### Input Fields
- Underlined or fully boxed with a 1px Black border. No rounded corners. Focus state changes border thickness to 2px.

### Cards
- Event cards should feature a large image header with a 0px border radius. Information below is strictly aligned to a internal 16px padding grid. Use `label-bold` for event dates.

### Chips/Tags
- Small, rectangular blocks with #F2F2F2 backgrounds and Black text. Used for genre or category tagging.

### Progress Indicators
- For checkout flows, use a simple 2px thick Black line to indicate steps, with bold text for the active stage. Avoid circular step indicators.