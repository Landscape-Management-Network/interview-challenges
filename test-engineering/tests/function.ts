async function waitForText(page: Page, selector: string, expectedText: string) {
  const element = await page.waitForSelector(selector);
  const text = await element?.textContent();
  if (text !== expectedText) {
    throw new Error(`Expected "${expectedText}", but found "${text}"`);
  }
}