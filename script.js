/* ============ THREE.JS PARTICLE NETWORK ============ */
(function initThree() {
  const canvas = document.getElementById('bg-canvas');
  const renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true });
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  renderer.setSize(window.innerWidth, window.innerHeight);

  const scene = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 0.1, 1000);
  camera.position.z = 30;

  // Particles
  const count = 180;
  const positions = new Float32Array(count * 3);
  const velocities = [];
  for (let i = 0; i < count; i++) {
    positions[i * 3]     = (Math.random() - .5) * 80;
    positions[i * 3 + 1] = (Math.random() - .5) * 50;
    positions[i * 3 + 2] = (Math.random() - .5) * 30;
    velocities.push({
      x: (Math.random() - .5) * 0.03,
      y: (Math.random() - .5) * 0.03,
      z: (Math.random() - .5) * 0.01
    });
  }

  const geo = new THREE.BufferGeometry();
  geo.setAttribute('position', new THREE.BufferAttribute(positions, 3));
  const mat = new THREE.PointsMaterial({ color: 0x7c3aed, size: 0.3, transparent: true, opacity: 0.7 });
  const points = new THREE.Points(geo, mat);
  scene.add(points);

  // Lines between nearby particles
  const lineMat = new THREE.LineBasicMaterial({ color: 0x7c3aed, transparent: true, opacity: 0.15 });
  let lineObj = null;

  function updateLines() {
    if (lineObj) scene.remove(lineObj);
    const lineGeo = new THREE.BufferGeometry();
    const linePositions = [];
    for (let i = 0; i < count; i++) {
      for (let j = i + 1; j < count; j++) {
        const dx = positions[i*3] - positions[j*3];
        const dy = positions[i*3+1] - positions[j*3+1];
        const dz = positions[i*3+2] - positions[j*3+2];
        const dist = Math.sqrt(dx*dx + dy*dy + dz*dz);
        if (dist < 12) {
          linePositions.push(positions[i*3], positions[i*3+1], positions[i*3+2]);
          linePositions.push(positions[j*3], positions[j*3+1], positions[j*3+2]);
        }
      }
    }
    lineGeo.setAttribute('position', new THREE.BufferAttribute(new Float32Array(linePositions), 3));
    lineObj = new THREE.LineSegments(lineGeo, lineMat);
    scene.add(lineObj);
  }

  // Mouse parallax
  let mx = 0, my = 0;
  document.addEventListener('mousemove', e => {
    mx = (e.clientX / window.innerWidth - .5) * 2;
    my = (e.clientY / window.innerHeight - .5) * 2;
  });

  let frame = 0;
  function animate() {
    requestAnimationFrame(animate);
    frame++;
    for (let i = 0; i < count; i++) {
      positions[i*3]   += velocities[i].x;
      positions[i*3+1] += velocities[i].y;
      positions[i*3+2] += velocities[i].z;
      if (Math.abs(positions[i*3])   > 40) velocities[i].x *= -1;
      if (Math.abs(positions[i*3+1]) > 25) velocities[i].y *= -1;
      if (Math.abs(positions[i*3+2]) > 15) velocities[i].z *= -1;
    }
    geo.attributes.position.needsUpdate = true;
    if (frame % 3 === 0) updateLines();

    camera.position.x += (mx * 3 - camera.position.x) * 0.04;
    camera.position.y += (-my * 2 - camera.position.y) * 0.04;

    renderer.render(scene, camera);
  }
  animate();

  window.addEventListener('resize', () => {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
  });
})();

/* ============ CUSTOM CURSOR ============ */
const cur = document.getElementById('cursor');
const trail = document.getElementById('cursor-trail');
let tx = 0, ty = 0, cx = 0, cy = 0;
document.addEventListener('mousemove', e => { tx = e.clientX; ty = e.clientY; });
function animCursor() {
  cx += (tx - cx) * 0.15;
  cy += (ty - cy) * 0.15;
  cur.style.left = tx + 'px';
  cur.style.top = ty + 'px';
  trail.style.left = cx + 'px';
  trail.style.top = cy + 'px';
  requestAnimationFrame(animCursor);
}
animCursor();
document.querySelectorAll('a,button,.tilt-card').forEach(el => {
  el.addEventListener('mouseenter', () => { cur.style.transform = 'translate(-50%,-50%) scale(2)'; trail.style.borderColor = 'rgba(124,58,237,.7)'; });
  el.addEventListener('mouseleave', () => { cur.style.transform = 'translate(-50%,-50%) scale(1)'; trail.style.borderColor = 'rgba(124,58,237,.4)'; });
});

/* ============ 3D CARD TILT ============ */
function applyTilt(el) {
  el.addEventListener('mousemove', e => {
    const r = el.getBoundingClientRect();
    const x = (e.clientX - r.left) / r.width - .5;
    const y = (e.clientY - r.top) / r.height - .5;
    const intensity = el.classList.contains('profile-3d') ? 15 : 10;
    el.style.transform = `perspective(800px) rotateY(${x * intensity}deg) rotateX(${-y * intensity}deg) translateZ(8px)`;
    el.style.boxShadow = `${-x * 20}px ${y * 20}px 40px rgba(124,58,237,0.25)`;
  });
  el.addEventListener('mouseleave', () => {
    el.style.transform = 'perspective(800px) rotateY(0deg) rotateX(0deg) translateZ(0px)';
    el.style.boxShadow = '';
  });
}
document.querySelectorAll('.tilt-card').forEach(applyTilt);

/* ============ NAV ============ */
const nav = document.getElementById('nav');
window.addEventListener('scroll', () => nav.classList.toggle('stuck', scrollY > 50), { passive: true });

const burger = document.getElementById('burger');
const navLinks = document.getElementById('nav-links');
burger.addEventListener('click', () => {
  burger.classList.toggle('on');
  navLinks.classList.toggle('open');
});
navLinks.querySelectorAll('.nl').forEach(a => a.addEventListener('click', () => {
  burger.classList.remove('on');
  navLinks.classList.remove('open');
}));

/* ============ COUNTER ============ */
function countUp(el) {
  const t = +el.dataset.n, d = 1800, s = performance.now();
  const tick = n => {
    const p = Math.min((n - s) / d, 1);
    el.textContent = Math.round((1 - (1-p)**3) * t);
    if (p < 1) requestAnimationFrame(tick);
  };
  requestAnimationFrame(tick);
}
new IntersectionObserver((es, ob) => {
  es.forEach(e => { if (e.isIntersecting) { countUp(e.target); ob.unobserve(e.target); }});
}, { threshold: .7 }).observe(document.querySelector('.hero-stats'));
document.querySelectorAll('.s-num').forEach(el => {
  new IntersectionObserver((es, ob) => {
    es.forEach(e => { if (e.isIntersecting) { countUp(e.target); ob.unobserve(e.target); }});
  }, { threshold: .7 }).observe(el);
});

/* ============ SKILL BARS ============ */
new IntersectionObserver(es => {
  es.forEach(e => {
    if (!e.isIntersecting) return;
    e.target.querySelectorAll('.orb-fill').forEach(bar => {
      const p = bar.dataset.p;
      setTimeout(() => bar.style.width = p + '%', 100);
    });
  });
}, { threshold: .2 }).observe(document.getElementById('stack'));

/* ============ REVEAL ============ */
const ro = new IntersectionObserver(es => {
  es.forEach(e => { if (e.isIntersecting) { e.target.classList.add('vis'); ro.unobserve(e.target); }});
}, { threshold: .1 });
document.querySelectorAll('.reveal').forEach(el => ro.observe(el));

/* ============ WORK FILTER ============ */
document.querySelectorAll('.wfb').forEach(btn => {
  btn.addEventListener('click', function() {
    document.querySelectorAll('.wfb').forEach(b => b.classList.remove('active'));
    this.classList.add('active');
    const f = this.dataset.f;
    document.querySelectorAll('.work-card').forEach(c => {
      c.classList.toggle('hidden', f !== 'all' && !(c.dataset.c || '').includes(f));
    });
  });
});

/* ============ CONTACT FORM ============ */
document.getElementById('cf').addEventListener('submit', function(e) {
  e.preventDefault();
  const ok = document.getElementById('cf-ok');
  const btn = this.querySelector('button[type=submit] .btn-face');
  btn.textContent = 'Sending…';
  setTimeout(() => {
    ok.textContent = '✓ Message sent! Zain will reply soon.';
    this.reset();
    btn.textContent = 'Send Message';
    setTimeout(() => ok.textContent = '', 5000);
  }, 1200);
});
