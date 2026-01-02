'use client';

import { motion } from 'framer-motion';
import { Quote } from 'lucide-react';

const testimonials = [
  {
    name: "Chris F",
    role: "Client",
    text: "Built precisely what we requested quickly and professionally."
  },
  {
    name: "Abhilash Reddy Y",
    role: "Data & Product Professional",
    text: "I have had the pleasure of working closely with Zain and can wholeheartedly vouch for his exceptional attention to detail and his proficiency in conducting detailed technical analysis."
  },
  {
    name: "William T.",
    role: "Client",
    text: "A++++ HIGHLY RECOMMENDED. Perfect customer service and attention. Lots of experience. I will be hiring again."
  },
  {
    name: "Imran Ahmad Qureshi",
    role: "Manager IT @ CareCloud",
    text: "Highly Expert Software Architect. He strive to learn and work on new things. I highly recommend Zain."
  },
  {
    name: "Sreedhar K.",
    role: "Client",
    text: "He is very quick in responding to my questions and delivery was done to meet the scope of the project in first round."
  }
];

export default function Testimonials() {
  return (
    <section id="testimonials" className="py-24 bg-black text-white relative">
      <div className="container mx-auto px-6">
        <motion.div 
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          className="text-center mb-16"
        >
          <h2 className="text-3xl md:text-5xl font-bold mb-4">What People Say</h2>
          <p className="text-gray-400">Feedback from clients and colleagues.</p>
        </motion.div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          {testimonials.map((t, i) => (
            <motion.div
              key={i}
              initial={{ opacity: 0, scale: 0.9 }}
              whileInView={{ opacity: 1, scale: 1 }}
              viewport={{ once: true }}
              transition={{ delay: i * 0.1 }}
              className="p-8 bg-zinc-900/30 border border-white/5 rounded-2xl relative"
            >
              <Quote className="absolute top-6 left-6 text-blue-500/20 w-10 h-10" />
              <p className="text-gray-300 italic mb-6 relative z-10 pt-4">
                &quot;{t.text}&quot;
              </p>
              <div className="flex items-center gap-4">
                <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center font-bold text-sm">
                  {t.name.charAt(0)}
                </div>
                <div>
                  <h4 className="font-bold text-white text-sm">{t.name}</h4>
                  <p className="text-xs text-gray-500">{t.role}</p>
                </div>
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
}
