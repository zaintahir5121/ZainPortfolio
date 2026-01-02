'use client';

import { motion } from 'framer-motion';
import { Globe, Code2, CloudCog, Settings, Zap, Database } from 'lucide-react';

const services = [
  {
    icon: <Globe className="w-8 h-8 text-blue-400" />,
    title: "Custom Web Development",
    description: "Tailored websites and web applications built with .NET, Angular, and React to meet specific business needs."
  },
  {
    icon: <CloudCog className="w-8 h-8 text-purple-400" />,
    title: "Cloud Solutions",
    description: "Azure-based cloud architecture, serverless functions, and scalable infrastructure setup."
  },
  {
    icon: <Code2 className="w-8 h-8 text-green-400" />,
    title: "API Development",
    description: "Robust RESTful and GraphQL APIs, social media integrations, and secure data exchange."
  },
  {
    icon: <Settings className="w-8 h-8 text-orange-400" />,
    title: "DevOps & Automation",
    description: "CI/CD pipelines with Azure DevOps, process automation, and automated scheduling systems."
  },
  {
    icon: <Database className="w-8 h-8 text-red-400" />,
    title: "Microservices Architecture",
    description: "Scalable, distributed systems using Microservices and CQRS patterns for enterprise growth."
  },
  {
    icon: <Zap className="w-8 h-8 text-yellow-400" />,
    title: "Performance Optimization",
    description: "Enhancing website speed, database query optimization, and improving user experience."
  }
];

export default function Services() {
  return (
    <section id="services" className="py-24 bg-black text-white relative overflow-hidden">
      {/* Background decoration */}
      <div className="absolute top-0 right-0 w-1/3 h-1/3 bg-blue-900/10 rounded-full blur-3xl" />
      <div className="absolute bottom-0 left-0 w-1/3 h-1/3 bg-purple-900/10 rounded-full blur-3xl" />

      <div className="container mx-auto px-6 relative z-10">
        <div className="text-center mb-16">
          <motion.h2 
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl md:text-5xl font-bold mb-4"
          >
            Services I Offer
          </motion.h2>
          <motion.p 
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="text-gray-400 max-w-2xl mx-auto"
          >
            Leveraging cutting-edge technology to build digital experiences that drive business growth.
          </motion.p>
        </div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {services.map((service, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className="p-8 bg-zinc-900/40 border border-white/5 rounded-2xl hover:bg-zinc-800/60 hover:border-blue-500/30 transition-all group"
            >
              <div className="mb-6 p-4 bg-white/5 rounded-xl inline-block group-hover:scale-110 transition-transform">
                {service.icon}
              </div>
              <h3 className="text-xl font-bold mb-3 group-hover:text-blue-400 transition-colors">{service.title}</h3>
              <p className="text-gray-400 leading-relaxed">
                {service.description}
              </p>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
}
