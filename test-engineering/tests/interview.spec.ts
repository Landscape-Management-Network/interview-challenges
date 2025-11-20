// tests/form.spec.ts
import { test, expect } from '@playwright/test';

test('submits the form', async ({ page }) => {
  await page.goto('http://localhost:3000/form');
  await page.fill('#name-input', 'Alice');
  await page.fill('#email', 'alice@example.com');
  await page.click('#submit');
  await page.waitForTimeout(2000);
  const message = await page.$('.success');
  expect(await message?.textContent()).toContain('Submitted');
});

test('required field shows error', async ({ page }) => {
  await page.goto('http://localhost:3000/form');
  await page.click('#submit');
  const error = await page.$('.error');
  expect(await error?.textContent()).toContain('required');
});
