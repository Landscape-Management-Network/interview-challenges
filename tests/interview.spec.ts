import { test, expect } from '@playwright/test';

test('User can log in and submit a job form', async ({ page }) => {
  // Navigate to login page and log in
  await page.goto('https://example.com/login');
  await page.fill('#username', 'testuser');
  await page.fill('#password', 'password123');
  await page.click('button[type="submit"]');

  // Navigate to jobs page and verify heading
  await page.goto('https://example.com/jobs');
  const heading = await page.textContent('h1');
  expect(heading).toBe('JOBS');

  // Fill out job form
  await page.fill('#title', 'Software Engineer');
  await page.fill('#description', 'Write code and tests.');
  await page.click('button[type="submit"]');

  // Verify the submitted job appears on the new page
  const jobTitle = await page.textContent('.job-title');
  expect(jobTitle).toBe('Software Engineer');
});
