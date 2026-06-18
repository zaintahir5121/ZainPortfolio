const CACHE = 'worklog-v1';
const PRE = ['/', '/css/site.css'];

self.addEventListener('install', e => {
  self.skipWaiting();
  e.waitUntil(caches.open(CACHE).then(c => c.addAll(PRE).catch(() => {})));
});

self.addEventListener('activate', e => {
  e.waitUntil(Promise.all([
    clients.claim(),
    caches.keys().then(ks => Promise.all(ks.filter(k => k !== CACHE).map(k => caches.delete(k))))
  ]));
});

self.addEventListener('fetch', e => {
  if (e.request.method !== 'GET') return;
  const url = new URL(e.request.url);
  // Never cache API or auth routes
  if (['/Logs/', '/Account/', '/Feedback/', '/Timesheet/', '/Recurring/'].some(p => url.pathname.startsWith(p))) return;
  e.respondWith(
    caches.match(e.request).then(cached => {
      const fresh = fetch(e.request).then(res => {
        if (res && res.status === 200 && res.type === 'basic') {
          caches.open(CACHE).then(c => c.put(e.request, res.clone()));
        }
        return res;
      }).catch(() => cached);
      return cached || fresh;
    })
  );
});
