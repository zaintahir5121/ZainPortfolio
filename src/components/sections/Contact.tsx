'use client';

import { motion } from 'framer-motion';
import { Mail, Linkedin, MapPin, Send } from 'lucide-react';

export default function Contact() {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    alert("Thank you for your message! This is a demo form.");
  };

  return (
    <section id="contact" className="py-24 bg-zinc-950 text-white">
      <div className="container mx-auto px-6">
        <div className="grid md:grid-cols-2 gap-16">
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
          >
            <h2 className="text-3xl md:text-5xl font-bold mb-6">Let&apos;s Work Together</h2>
            <p className="text-gray-400 mb-8 text-lg">
              Have a project in mind? Looking for a consultant or a lead engineer?
              Let&apos;s discuss how I can help you achieve your goals.
            </p>
            
            <div className="space-y-6">
              <a href="mailto:zabbastahir@gmail.com" className="flex items-center gap-4 text-gray-300 hover:text-white transition-colors p-4 bg-zinc-900 rounded-xl border border-zinc-800">
                <div className="p-3 bg-blue-500/10 text-blue-400 rounded-lg">
                  <Mail size={24} />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Email</p>
                  <p className="font-medium">zabbastahir@gmail.com</p>
                </div>
              </a>
              
              <a href="https://www.linkedin.com/in/zainabbastahir/" target="_blank" className="flex items-center gap-4 text-gray-300 hover:text-white transition-colors p-4 bg-zinc-900 rounded-xl border border-zinc-800">
                <div className="p-3 bg-blue-500/10 text-blue-400 rounded-lg">
                  <Linkedin size={24} />
                </div>
                <div>
                  <p className="text-sm text-gray-500">LinkedIn</p>
                  <p className="font-medium">linkedin.com/in/zainabbastahir</p>
                </div>
              </a>

              <div className="flex items-center gap-4 text-gray-300 p-4 bg-zinc-900 rounded-xl border border-zinc-800">
                <div className="p-3 bg-blue-500/10 text-blue-400 rounded-lg">
                  <MapPin size={24} />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Location</p>
                  <p className="font-medium">Global / Remote</p>
                </div>
              </div>
            </div>
          </motion.div>

          <motion.form
            initial={{ opacity: 0, x: 20 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            onSubmit={handleSubmit}
            className="p-8 bg-zinc-900 border border-zinc-800 rounded-2xl space-y-6"
          >
            <div>
              <label htmlFor="name" className="block text-sm font-medium text-gray-400 mb-2">Name</label>
              <input type="text" id="name" className="w-full bg-zinc-950 border border-zinc-800 rounded-lg p-3 text-white focus:outline-none focus:border-blue-500 transition-colors" placeholder="Your Name" />
            </div>
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-400 mb-2">Email</label>
              <input type="email" id="email" className="w-full bg-zinc-950 border border-zinc-800 rounded-lg p-3 text-white focus:outline-none focus:border-blue-500 transition-colors" placeholder="your@email.com" />
            </div>
            <div>
              <label htmlFor="message" className="block text-sm font-medium text-gray-400 mb-2">Message</label>
              <textarea id="message" rows={4} className="w-full bg-zinc-950 border border-zinc-800 rounded-lg p-3 text-white focus:outline-none focus:border-blue-500 transition-colors" placeholder="Tell me about your project..."></textarea>
            </div>
            
            <button type="submit" className="w-full py-4 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-lg transition-all flex items-center justify-center gap-2">
              Send Message <Send size={18} />
            </button>
          </motion.form>
        </div>
      </div>
    </section>
  );
}
