'use client';

import { motion } from 'framer-motion';
import { ArrowRight, Download } from 'lucide-react';
import Link from 'next/link';

export default function Hero() {
  return (
    <section id="home" className="relative min-h-screen flex items-center pt-20 overflow-hidden bg-black text-white">
      {/* Background Effects */}
      <div className="absolute inset-0 z-0">
        <div className="absolute top-0 left-1/4 w-96 h-96 bg-blue-600/20 rounded-full blur-3xl" />
        <div className="absolute bottom-0 right-1/4 w-96 h-96 bg-purple-600/20 rounded-full blur-3xl" />
        <div className="absolute inset-0 bg-[url('/grid.svg')] bg-center [mask-image:linear-gradient(180deg,white,rgba(255,255,255,0))]" />
      </div>

      <div className="container mx-auto px-6 relative z-10 grid md:grid-cols-2 gap-12 items-center">
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5 }}
        >
          <div className="inline-block px-4 py-1.5 mb-4 border border-blue-500/30 rounded-full bg-blue-500/10 text-blue-400 text-sm font-medium">
            Senior Full-Stack & Cloud Engineer
          </div>
          <h1 className="text-5xl md:text-7xl font-bold leading-tight mb-6">
            I Build <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-400 to-purple-500">Scalable</span> & Intelligent Systems
          </h1>
          <p className="text-xl text-gray-400 mb-8 max-w-lg">
            Specializing in .NET, Angular, Azure, and AI. Delivering enterprise-grade solutions for 13+ years.
          </p>
          
          <div className="flex flex-wrap gap-4">
            <Link
              href="#contact"
              className="px-8 py-4 bg-white text-black font-bold rounded-lg hover:bg-gray-200 transition-colors flex items-center gap-2"
            >
              Start a Project <ArrowRight size={20} />
            </Link>
            <Link
              href="https://zainabbastahir.com/wp-content/uploads/2025/02/ZainAbbasTahirCv_Tehnical_Lead-6.pdf"
              target="_blank"
              className="px-8 py-4 border border-white/20 hover:border-white/50 bg-white/5 hover:bg-white/10 text-white font-medium rounded-lg transition-all flex items-center gap-2"
            >
              Download CV <Download size={20} />
            </Link>
          </div>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, scale: 0.9 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.5, delay: 0.2 }}
          className="relative"
        >
          <div className="relative rounded-2xl overflow-hidden shadow-2xl border border-white/10 aspect-video group">
             {/* YouTube Embed */}
             <iframe 
                width="100%" 
                height="100%" 
                src="https://www.youtube.com/embed/2JtZ1fcE39g?rel=0&autoplay=0" 
                title="Zain Abbas Tahir Video CV"
                frameBorder="0" 
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" 
                allowFullScreen
                className="w-full h-full object-cover"
              ></iframe>
          </div>
          
          {/* Decorative elements around video */}
          <div className="absolute -top-4 -right-4 w-24 h-24 bg-blue-500/20 rounded-full blur-xl -z-10" />
          <div className="absolute -bottom-4 -left-4 w-24 h-24 bg-purple-500/20 rounded-full blur-xl -z-10" />
        </motion.div>
      </div>
    </section>
  );
}
