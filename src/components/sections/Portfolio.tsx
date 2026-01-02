'use client';

import { motion } from 'framer-motion';
import { ExternalLink, Github, Play } from 'lucide-react';
import Image from 'next/image';
import Link from 'next/link';

const projects = [
  {
    title: "Point of Sale System",
    category: "Desktop Application",
    image: "https://zainabbastahir.com/wp-content/uploads/2025/02/14-300x160.webp", // Using the image from the source
    tech: [".NET", "C#", "WinForms", "SQL"],
    description: "A comprehensive POS system with inventory management, barcode support, and financial reporting.",
    demoLink: "#",
    githubLink: "#"
  },
  {
    title: "Object Detection Login",
    category: "AI / Machine Learning",
    image: "https://zainabbastahir.com/wp-content/uploads/2025/02/th.jpg",
    tech: [".NET", "C#", "ML.NET", "Computer Vision"],
    description: "Secure login system using person object detection and facial recognition technology.",
    demoLink: "#",
    githubLink: "#"
  },
  {
    title: "Document Management",
    category: "Web Application",
    image: "https://zainabbastahir.com/wp-content/uploads/2025/02/Login-300x194.webp",
    tech: [".NET Core", "SQL Server", "ADO.NET"],
    description: "Enterprise-grade document management system with role-based access control and secure file sharing.",
    demoLink: "#",
    githubLink: "#"
  },
  {
    title: "URL Shortener & Analytics",
    category: "Web Tool",
    image: "https://zainabbastahir.com/wp-content/uploads/2025/02/2020-11-06_2-50-22-1-1-300x146.webp",
    tech: [".NET Core", "ASP.NET", "AJAX"],
    description: "High-performance URL shortener with detailed click tracking and analytics dashboard.",
    demoLink: "#",
    githubLink: "#"
  }
];

export default function Portfolio() {
  return (
    <section id="portfolio" className="py-24 bg-zinc-950 text-white">
      <div className="container mx-auto px-6">
        <div className="flex flex-col md:flex-row justify-between items-end mb-12">
          <div>
            <motion.h2 
              initial={{ opacity: 0, x: -20 }}
              whileInView={{ opacity: 1, x: 0 }}
              viewport={{ once: true }}
              className="text-3xl md:text-5xl font-bold mb-4"
            >
              Featured Projects
            </motion.h2>
            <motion.div 
              initial={{ opacity: 0, width: 0 }}
              whileInView={{ opacity: 1, width: 100 }}
              viewport={{ once: true }}
              className="h-1 bg-blue-500"
            />
          </div>
          <motion.a 
            href="#" 
            initial={{ opacity: 0, x: 20 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            className="hidden md:flex items-center gap-2 text-blue-400 hover:text-blue-300 mt-4 md:mt-0"
          >
            View All Projects <ExternalLink size={16} />
          </motion.a>
        </div>

        <div className="grid md:grid-cols-2 gap-8">
          {projects.map((project, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className="group relative bg-zinc-900 border border-zinc-800 rounded-xl overflow-hidden hover:border-zinc-700 transition-colors"
            >
              {/* Image Container */}
              <div className="relative h-64 w-full overflow-hidden bg-zinc-800">
                <div className="absolute inset-0 bg-black/20 group-hover:bg-black/40 transition-colors z-10" />
                <Image
                  src={project.image} 
                  alt={project.title}
                  fill
                  unoptimized
                  className="object-cover transform group-hover:scale-105 transition-transform duration-500"
                />
                
                {/* Overlay Actions */}
                <div className="absolute inset-0 z-20 flex items-center justify-center gap-4 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                  <Link 
                    href={project.demoLink}
                    className="p-3 bg-blue-600 rounded-full text-white hover:bg-blue-700 hover:scale-110 transition-all shadow-lg"
                    title="View Demo"
                  >
                    <Play size={24} fill="currentColor" />
                  </Link>
                  <Link 
                    href={project.githubLink}
                    className="p-3 bg-zinc-800 rounded-full text-white hover:bg-zinc-700 hover:scale-110 transition-all shadow-lg"
                    title="View Code"
                  >
                    <Github size={24} />
                  </Link>
                </div>
              </div>

              {/* Content */}
              <div className="p-6">
                <div className="flex justify-between items-start mb-2">
                  <div>
                    <span className="text-xs font-medium text-blue-400 uppercase tracking-wider">{project.category}</span>
                    <h3 className="text-xl font-bold mt-1 text-white">{project.title}</h3>
                  </div>
                </div>
                <p className="text-gray-400 mb-4 line-clamp-2">
                  {project.description}
                </p>
                <div className="flex flex-wrap gap-2">
                  {project.tech.map((t) => (
                    <span key={t} className="px-2 py-1 bg-zinc-800 text-xs text-zinc-300 rounded border border-zinc-700">
                      {t}
                    </span>
                  ))}
                </div>
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
}
