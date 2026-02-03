import { showSection } from "./ui.js";

const ROLE_KEY = "app_role";

function setRole(role) {
  try {
    localStorage.setItem(ROLE_KEY, role);
  } catch {
    // ignore
  }
}

function wireLogoFallback() {
  const img = document.getElementById("landing-logo-img");
  const fallback = document.getElementById("landing-logo-fallback");
  if (!img || !fallback) return;

  // hide fallback when image loads
  fallback.hidden = true;

  img.addEventListener("load", () => {
    fallback.hidden = true;
    img.style.display = "block";
  });

  img.addEventListener("error", () => {
    img.style.display = "none";
    fallback.hidden = false;
  });
}

export function wireLanding() {
  const landing = document.getElementById("landing");
  if (!landing) return;

  wireLogoFallback();

  const seeker = document.getElementById("role-seeker");
  const company = document.getElementById("role-company");

  // On this step we only determine the category.
  // Next step: open register (login is still available inside the auth screens).
  seeker?.addEventListener("click", () => {
    setRole("seeker");
    showSection("register");
  });

  company?.addEventListener("click", () => {
    setRole("company");
    showSection("register");
  });
}
