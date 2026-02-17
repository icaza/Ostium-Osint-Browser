self.addEventListener('install', (event) => {
    console.log('Service Worker installed');
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    console.log('Service Worker activated');
    event.waitUntil(clients.claim());
});

self.addEventListener('message', (event) => {
    if (event.data.type === 'CACHE_DATA') {
        caches.open('netlog-data').then(cache => {
            cache.put('/data.json', new Response(JSON.stringify(event.data.data)));
        });
    }
});