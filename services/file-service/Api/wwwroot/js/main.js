import { showSection, previewImage } from "./ui.js";
import { handleAuth } from "./auth.js";
import { loadProfileData, handleProfileUpdate } from "./profile.js";
import { wireLanding } from "./landing.js";
import {
  prefillEmailToPublicChangePassword,
  handleChangePasswordPublic,
  handleChangePasswordLoggedIn,
} from "./password.js";
import { logout } from "./session.js";

function wireForms() {
  const loginForm = document.getElementById("login-form");
  loginForm?.addEventListener("submit", (e) =>
    handleAuth(e, "/Auth/login", "login"),
  );

  const registerForm = document.getElementById("register-form");
  registerForm?.addEventListener("submit", (e) =>
    handleAuth(e, "/Auth/register", "register"),
  );

  const cpPublicForm = document.getElementById("cp-public-form");
  cpPublicForm?.addEventListener("submit", handleChangePasswordPublic);

  const profileForm = document.getElementById("profile-form");
  profileForm?.addEventListener("submit", handleProfileUpdate);

  const cpLoggedForm = document.getElementById("cp-logged-form");
  cpLoggedForm?.addEventListener("submit", handleChangePasswordLoggedIn);
}

function wireAvatar() {
  const upload = document.getElementById("avatar-upload");
  const input = document.getElementById("avatar-input");

  upload?.addEventListener("click", () => input?.click());
  input?.addEventListener("change", () => {
    if (input) previewImage(input);
  });
}

function activateByKeyboard(e) {
  // allow Enter/Space on spans with role=button
  if (e.key !== "Enter" && e.key !== " ") return;
  const el = e.target?.closest?.("[data-show],[data-action]");
  if (!el) return;
  e.preventDefault();
  el.click();
}

function wireNavigation() {
  document.addEventListener("click", (e) => {
    const showEl = e.target?.closest?.("[data-show]");
    if (showEl) {
      showSection(showEl.getAttribute("data-show"));
      return;
    }

    const actionEl = e.target?.closest?.("[data-action]");
    if (!actionEl) return;

    const action = actionEl.getAttribute("data-action");
    if (action === "logout") logout();
    if (action === "open-public-change-password")
      prefillEmailToPublicChangePassword();
  });

  document.addEventListener("keydown", activateByKeyboard);
}

function boot() {
  if (localStorage.getItem("token")) {
    loadProfileData();
  } else {
    showSection("landing");
  }
}

document.addEventListener("DOMContentLoaded", () => {
  wireForms();
  wireAvatar();
  wireNavigation();
  wireLanding();
  boot();
});
