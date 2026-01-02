'use client';

import { motion } from 'framer-motion';
import { Code, Server, Cloud, Database } from 'lucide-react';

const skills = [
  { category: "Backend", items: [".NET Core", "ASP.NET MVC", "Web API", "Node.js", "C#", "Microservices"] },
  { category: "Frontend", items: ["Angular (8-17)", "HTML5", "CSS3", "JavaScript", "TypeScript"] },
  { category: "Cloud & DevOps", items: ["Azure", "Docker", "Kubernetes", "CI/CD Pipelines", "Azure DevOps"] },
  { category: "Database", items: ["SQL Server", "Entity Framework", "Cosmos DB", "Redis Cache"] },
];

export default function About() {
  return (
    <section id="about" className="py-24 bg-zinc-950 text-white">
      <div className="container mx-auto px-6">
        <div className="grid md:grid-cols-2 gap-16 items-center">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.5 }}
          >
            <h2 className="text-3xl md:text-4xl font-bold mb-6">About Me</h2>
            <div className="w-20 h-1 bg-blue-500 mb-8" />
            
            <p className="text-gray-400 text-lg mb-6 leading-relaxed">
              As a professional Technical Lead and Full-Stack Engineer with over 13 years of experience, I thrive on crafting innovative software solutions that elevate business performance.
            </p>
            <p className="text-gray-400 text-lg mb-6 leading-relaxed">
              My expertise spans the entire Software Development Life Cycle (SDLC), with a passion for building scalable, high-performing systems. From designing robust backend architectures with .NET and Microservices to creating seamless front-end experiences with Angular, I am committed to delivering exceptional results.
            </p>
            
            <div className="grid grid-cols-2 gap-4 mt-8">
               <div className="p-4 bg-zinc-900 rounded-lg border border-zinc-800">
                  <h3 className="text-3xl font-bold text-white mb-1">13+</h3>
                  <p className="text-zinc-500">Years Experience</p>
               </div>
               <div className="p-4 bg-zinc-900 rounded-lg border border-zinc-800">
                  <h3 className="text-3xl font-bold text-white mb-1">50+</h3>
                  <p className="text-zinc-500">Projects Delivered</p>
               </div>
            </div>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, x: 20 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.5, delay: 0.2 }}
            className="grid gap-6"
          >
            {skills.map((skillGroup, index) => (
              <div key={index} className="p-6 bg-zinc-900/50 rounded-xl border border-white/5 hover:border-blue-500/30 transition-colors">
                <h3 className="text-xl font-semibold mb-4 text-blue-400 flex items-center gap-2">
                  {index === 0 && <Server size={20} />}
                  {index === 1 && <Code size={20} />}
                  {index === 2 && <Cloud size={20} />}
                  {index === 3 && <Database size={20} />}
                  {skillGroup.category}
                </h3>
                <div className="flex flex-wrap gap-2">
                  {skillGroup.items.map((item) => (
                    <span key={item} className="px-3 py-1 bg-white/5 text-gray-300 text-sm rounded-full border border-white/5">
                      {item}
                    </span>
                  ))}
                </div>
              </div>
            ))}
          </motion.div>
        </div>
      </div>
    </section>
  );
}
