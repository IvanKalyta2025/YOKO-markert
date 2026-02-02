import { clearToken } from "./storage.js";
import { setAvatarEverywhere, showSection } from "./ui.js";

export function logout() {
  clearToken();
  setAvatarEverywhere("default-avatar.svg");
  showSection("login");
}
