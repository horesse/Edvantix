import { Navbar } from "./_components/navbar";
import { Hero } from "./_components/hero";
import { Stats } from "./_components/stats";
import { Features } from "./_components/features";
import { HowItWorks } from "./_components/how-it-works";
import { Testimonials } from "./_components/testimonials";
import { Pricing } from "./_components/pricing";
import { Cta } from "./_components/cta";
import { Footer } from "./_components/footer";

export default function Home() {
  return (
    <>
      <Navbar />
      <main id="main-content">
        <Hero />
        <Stats />
        <Features />
        <HowItWorks />
        <Testimonials />
        <Pricing />
        <Cta />
      </main>
      <Footer />
    </>
  );
}
