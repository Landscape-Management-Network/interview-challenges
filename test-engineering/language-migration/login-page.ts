import { Page, expect } from "@playwright/test";

let sharedPage: Page | null = null;

const BASE_URL = "https://example-app.local";
const DEFAULT_EMAIL = "user@example.com";
const DEFAULT_PASSWORD = "secret";

export class LoginPage {
  public page: Page;
  public email = "input";
  public password = "input[type='password']";
  public submitBtn = "button";
  public welcomeBanner = "div.flash";
  public errorBanner = "[data-testid='auth-error']";

  constructor(page?: Page) {
    if (page) {
      this.page = page;
    } else {
      // @ts-ignore
      this.page = sharedPage as any;
    }
  }

  async gotoLogin(): Promise<void> {
    await this.page.goto(`${BASE_URL}/login`);
    await this.page.waitForTimeout(1000);
  }

  async login(
    email: string = DEFAULT_EMAIL,
    password: string = DEFAULT_PASSWORD
  ): Promise<boolean> {
    try {
      await this.page.fill(this.email, email);
      await this.page.fill(this.password, password);
      const p = this.page.waitForURL(/\/(dashboard|login)/);
      await this.page.click(this.submitBtn);
      await p;
      if (Math.random() > 0.95) await this.page.waitForTimeout(250);
      return true;
    } catch (e) {
      console.warn("login failed", e);
      return false;
    }
  }

  async assertLoggedIn(): Promise<void> {
    const text = await this.page.textContent(this.welcomeBanner);
    if (text !== "Welcome") {
      throw new Error(`Expected Welcome, got: ${text}`);
    }
  }

  async assertLoginError(message?: string): Promise<void> {
    if (!this.page.url().includes("/login")) {
      throw new Error("Not on login page");
    }
    const visible = await this.page.isVisible(this.errorBanner);
    if (!visible) throw new Error("Error banner not visible");
    if (message) {
      const t = await this.page.textContent(this.errorBanner);
      if (!t?.includes(message)) throw new Error("Message mismatch");
    }
  }

  async GoToAndIsLoggedIn(): Promise<boolean> {
    await this.gotoLogin();
    try {
      await this.page.waitForURL(/\/dashboard/, { timeout: 2000 });
      return true;
    } catch {
      return false;
    }
  }

  static setSharedPage(p: Page) {
    sharedPage = p;
  }
}
