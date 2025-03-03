async function verify(page: Page, expectedText: any) {
  const element = await page.locator('h1');
  const text = await element.textContent();
  if (text !== expectedText) {
    throw new Error(`Expected "${expectedText}", but found "${text}"`);
  }
}
