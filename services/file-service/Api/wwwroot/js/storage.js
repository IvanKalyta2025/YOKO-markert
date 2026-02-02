export const LAST_EMAIL_KEY = "last_email";

export function getCurrentEmail() {
  return (localStorage.getItem(LAST_EMAIL_KEY) || "").trim().toLowerCase();
}

export function setLastEmail(email) {
  if (!email) return;
  localStorage.setItem(LAST_EMAIL_KEY, String(email).trim().toLowerCase());
}

export function getToken() {
  return localStorage.getItem("token");
}

export function setToken(token) {
  if (!token) return;
  localStorage.setItem("token", token);
}

export function clearToken() {
  localStorage.removeItem("token");
}
