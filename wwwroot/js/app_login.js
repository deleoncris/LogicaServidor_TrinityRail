/* ===================================================
   Login — app_login.js  (solo frontend)
   =================================================== */
(function () {
  "use strict";

  const card      = document.querySelector(".card");
  const inputs    = document.querySelectorAll(".field__input");
  const toggleBtn = document.getElementById("togglePass");
  const passInput = document.getElementById("password");
  const eyeOn     = toggleBtn.querySelector(".icon-eye");
  const eyeOff    = toggleBtn.querySelector(".icon-eye-off");
  const canvas    = document.getElementById("bgCanvas");
  const ctx       = canvas.getContext("2d");

  // ── Partículas de fondo ────────────────────────
  let W, H, particles = [];

  function resize() {
    W = canvas.width  = window.innerWidth;
    H = canvas.height = window.innerHeight;
  }

  function makeParticle() {
    return {
      x:  Math.random() * W,
      y:  Math.random() * H,
      r:  Math.random() * 1.2 + 0.3,
      vx: (Math.random() - 0.5) * 0.25,
      vy: (Math.random() - 0.5) * 0.25,
      a:  Math.random() * 0.5 + 0.1,
    };
  }

  function initParticles(n = 70) {
    particles = Array.from({ length: n }, makeParticle);
  }

  function draw() {
    ctx.clearRect(0, 0, W, H);
    particles.forEach(p => {
      p.x += p.vx; p.y += p.vy;
      if (p.x < 0) p.x = W; if (p.x > W) p.x = 0;
      if (p.y < 0) p.y = H; if (p.y > H) p.y = 0;
      ctx.beginPath();
      ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
      ctx.fillStyle = `rgba(184,255,60,${p.a})`;
      ctx.fill();
    });
    requestAnimationFrame(draw);
  }

  resize();
  initParticles();
  draw();
  window.addEventListener("resize", () => { resize(); initParticles(); });

  // ── Estado activo en tarjeta ───────────────────
  inputs.forEach(el => {
    el.addEventListener("focus", () => card.classList.add("card--active"));
    el.addEventListener("blur",  () => {
      const any = [...inputs].some(i => i === document.activeElement);
      if (!any) card.classList.remove("card--active");
    });
  });

  // ── Mostrar / ocultar contraseña ───────────────
  toggleBtn.addEventListener("click", () => {
    const show = passInput.type === "password";
    passInput.type       = show ? "text"  : "password";
    eyeOn.style.display  = show ? "none"  : "";
    eyeOff.style.display = show ? ""      : "none";
    toggleBtn.setAttribute("aria-label", show ? "Ocultar contraseña" : "Mostrar contraseña");
  });

})();
